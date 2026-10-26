using ArvidsonFoto.Controllers;
using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.Models;
using ArvidsonFoto.Core.Services;
using ArvidsonFoto.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArvidsonFoto.Tests.Unit.ServiceTests;

public class TranslationReviewTests
{
    private static ArvidsonFotoCoreDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<ArvidsonFotoCoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    [Fact]
    public void AdminListsFieldsIndividuallyAndFiltersReviewed()
    {
        using var db = CreateContext();
        db.TblMenus.Add(new TblMenu
        {
            Id = 1,
            MenuCategoryId = 10,
            MenuDisplayName = "Bäver",
            MenuDisplayNameEn = "Beaver",
            MenuDisplayNameEnReviewed = true,
            MenuUrlSegment = "Baver",
            MenuUrlSegmentEn = "beaver"
        });
        db.SaveChanges();

        var controller = new TranslationReviewController(db);
        var pending = Assert.IsType<TranslationReviewViewModel>(Assert.IsType<ViewResult>(controller.Index()).Model);
        Assert.Single(pending.Items);
        Assert.Equal("url", pending.Items[0].Field);
        Assert.Equal(1, pending.PendingCount);
        Assert.Equal(2, pending.TotalCount);

        var all = Assert.IsType<TranslationReviewViewModel>(Assert.IsType<ViewResult>(controller.Index(pendingOnly: false)).Model);
        Assert.Equal(2, all.Items.Count);
    }

    [Fact]
    public void SaveRequiresExistingTranslationBeforeReviewAndResetsWhenChanged()
    {
        using var db = CreateContext();
        var row = new TblMenu { Id = 1, MenuCategoryId = 10, MenuDisplayName = "Bäver", MenuUrlSegment = "Baver" };
        db.TblMenus.Add(row);
        db.SaveChanges();
        var controller = new TranslationReviewController(db);

        Assert.IsType<RedirectToActionResult>(controller.Save("menu", 10, "name", "Beaver", "Bäver", null, true));
        Assert.False(row.MenuDisplayNameEnReviewed);
        Assert.IsType<RedirectToActionResult>(controller.Save("menu", 10, "name", "Beaver", "Bäver", "Beaver", true));
        Assert.True(row.MenuDisplayNameEnReviewed);

        row.MenuDisplayName = "Europeisk bäver";
        db.SaveChanges();
        Assert.False(row.MenuDisplayNameEnReviewed);
        Assert.IsType<ConflictResult>(controller.Save("menu", 10, "name", "Beaver", "Bäver", "Beaver", true));
        Assert.False(row.MenuDisplayNameEnReviewed);
    }

    [Fact]
    public void SavingMenuTranslationClearsCategoryCache()
    {
        using var db = CreateContext();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var categories = new ApiCategoryService(NullLogger<ApiCategoryService>.Instance, db, cache);
        db.TblMenus.Add(new TblMenu { Id = 1, MenuCategoryId = 10, MenuDisplayName = "Bäver" });
        db.SaveChanges();
        cache.Set("Api_AllCategories", new object());

        var controller = new TranslationReviewController(db, categories);
        Assert.IsType<RedirectToActionResult>(controller.Save("menu", 10, "name", "Beaver", "Bäver", null, false));
        Assert.False(cache.TryGetValue("Api_AllCategories", out _));
    }

    [Fact]
    public void ImageFiltersFindMissingAndUnreviewedSeparately()
    {
        using var db = CreateContext();
        db.TblImages.AddRange(
            new TblImage { Id = 1, ImageId = 11, ImageUrlName = "A.jpg", ImageDescription = "Bäver", ImageDescriptionEn = null },
            new TblImage { Id = 2, ImageId = 12, ImageUrlName = "B.jpg", ImageDescription = "Utter", ImageDescriptionEn = "   " },
            new TblImage { Id = 3, ImageId = 13, ImageUrlName = "C.jpg", ImageDescription = "Älg", ImageDescriptionEn = "Moose" },
            new TblImage { Id = 4, ImageId = 14, ImageUrlName = "D.jpg", ImageDescription = "Räv", ImageDescriptionEn = "Fox", ImageDescriptionEnReviewed = true },
            new TblImage { Id = 5, ImageId = 15, ImageUrlName = "E.jpg", ImageDescription = "", ImageDescriptionEn = null },
            new TblImage { Id = 6, ImageId = 16, ImageUrlName = "F.jpg", ImageDescription = "   ", ImageDescriptionEn = null });
        db.SaveChanges();
        var controller = new TranslationReviewController(db);

        var missing = Assert.IsType<TranslationReviewViewModel>(Assert.IsType<ViewResult>(
            controller.Index("images", missingOnly: true)).Model);
        Assert.Equal([11, 12], missing.Items.Select(i => i.Id));
        Assert.Equal(2, missing.MissingCount);
        Assert.Equal(3, missing.PendingCount);
        Assert.Equal(4, missing.TotalCount);
        Assert.Equal(3, missing.AllPendingCount);
        Assert.Equal(4, missing.AllFieldsCount);

        var pending = Assert.IsType<TranslationReviewViewModel>(Assert.IsType<ViewResult>(
            controller.Index("images")).Model);
        Assert.Equal([11, 12, 13], pending.Items.Select(i => i.Id));

        var all = Assert.IsType<TranslationReviewViewModel>(Assert.IsType<ViewResult>(
            controller.Index("images", pendingOnly: false)).Model);
        Assert.Equal(4, all.Items.Count);

        var searched = Assert.IsType<TranslationReviewViewModel>(Assert.IsType<ViewResult>(
            controller.Index("images", missingOnly: true, page: 9, search: " A.jpg ")).Model);
        Assert.Equal("A.jpg", searched.Search);
        Assert.Equal(1, searched.Page);
        Assert.Equal(1, searched.MissingCount);
        Assert.Equal(11, Assert.Single(searched.Items).Id);
        Assert.IsType<BadRequestResult>(controller.Index("images", search: new string('x', 101)));
    }

    [Fact]
    public void ImageSavePreservesMissingFilterAndSearch()
    {
        using var db = CreateContext();
        db.TblImages.Add(new TblImage { Id = 1, ImageId = 11, ImageDescription = "Bäver" });
        db.SaveChanges();
        var controller = new TranslationReviewController(db);
        var result = Assert.IsType<RedirectToActionResult>(
            controller.Save("images", 11, "description", "Beaver", "Bäver", null, false,
                missingOnly: true, search: "Bäver"));
        Assert.Equal(true, result.RouteValues!["missingOnly"]);
        Assert.Equal("Bäver", result.RouteValues["search"]);
        Assert.Empty(Assert.IsType<TranslationReviewViewModel>(
            Assert.IsType<ViewResult>(controller.Index("images", missingOnly: true, search: "Bäver")).Model).Items);
    }

    [Fact]
    public async Task ChangesResetOnlyTheAffectedFieldAcrossAllTables()
    {
        using var db = CreateContext();
        var menu = new TblMenu
        {
            Id = 1,
            MenuCategoryId = 1,
            MenuDisplayName = "Bäver",
            MenuDisplayNameEn = "Beaver",
            MenuUrlSegment = "Baver",
            MenuUrlSegmentEn = "beaver",
            MenuDisplayNameEnReviewed = true,
            MenuUrlSegmentEnReviewed = true
        };
        var image = new TblImage { Id = 2, ImageId = 2, ImageDescription = "Svensk", ImageDescriptionEn = "English", ImageDescriptionEnReviewed = true };
        var guest = new TblGb
        {
            Id = 3,
            GbId = 3,
            GbName = "Namn",
            GbNameEn = "Name",
            GbNameEnReviewed = true,
            GbText = "Hej",
            GbTextEn = "Hello",
            GbTextEnReviewed = true
        };
        db.AddRange(menu, image, guest);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        menu.MenuUrlSegmentEn = "new-beaver";
        image.ImageDescription = "Ny text";
        guest.GbNameEn = "New name";
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        Assert.True(menu.MenuDisplayNameEnReviewed);
        Assert.False(menu.MenuUrlSegmentEnReviewed);
        Assert.False(image.ImageDescriptionEnReviewed);
        Assert.False(guest.GbNameEnReviewed);
        Assert.True(guest.GbTextEnReviewed);
        guest.GbTextEn = null;
        db.SaveChanges();
        Assert.False(guest.GbTextEnReviewed);
    }

    [Fact]
    public void ReviewImageAndGuestbookFieldsIndependently()
    {
        using var db = CreateContext();
        var image = new TblImage { Id = 1, ImageId = 7, ImageDescription = "Vatten", ImageDescriptionEn = "Water" };
        var guest = new TblGb { Id = 2, GbId = 8, GbName = "Namn", GbText = "Hej" };
        db.AddRange(image, guest);
        db.SaveChanges();
        var controller = new TranslationReviewController(db);

        var images = Assert.IsType<TranslationReviewViewModel>(Assert.IsType<ViewResult>(controller.Index("images")).Model);
        Assert.Single(images.Items);
        Assert.Equal("Vatten", images.Items[0].Swedish);
        Assert.IsType<RedirectToActionResult>(controller.Save("images", 7, "description", "Water", "Vatten", "Water", true));
        Assert.True(image.ImageDescriptionEnReviewed);

        Assert.IsType<RedirectToActionResult>(controller.Save("guestbook", 2, "text", "Hello", "Hej", null, true));
        Assert.False(guest.GbTextEnReviewed);
        Assert.IsType<RedirectToActionResult>(controller.Save("guestbook", 2, "text", "Hello", "Hej", "Hello", true));
        Assert.True(guest.GbTextEnReviewed);
        Assert.False(guest.GbNameEnReviewed);
        Assert.IsType<BadRequestResult>(controller.Save("guestbook", 2, "name", null, "Namn", null, true));
    }
}