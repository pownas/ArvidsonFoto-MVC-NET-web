using ArvidsonFoto.Controllers;
using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.Models;
using ArvidsonFoto.Core.Services;
using ArvidsonFoto.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArvidsonFoto.Tests.Unit.ControllerTests;

public class UploadAdminImageFiltersTests
{
    [Fact]
    public void RedigeraBilderCombinesFiltersAndPaginatesMatchingImages()
    {
        using var db = CreateContext();
        db.TblImages.AddRange(
            Image(1, "bird-one", "Blue bird", "Oiseau", new DateTime(2026, 4, 10), 3),
            Image(2, "bird-two", "Red bird", "Bird", new DateTime(2026, 4, 20), 3),
            Image(3, "bird-three", "Green bird", "Bird", new DateTime(2026, 5, 1), 3),
            Image(4, "bird-four", "Blue bird", "Bird", new DateTime(2026, 4, 15), 4));
        db.SaveChanges();

        var result = Assert.IsType<ViewResult>(CreateController(db).RedigeraBilder(
            "", "", 1, "bird", 3, new DateTime(2026, 4, 1), new DateTime(2026, 4, 30)));
        var model = Assert.IsType<UploadEditImagesViewModel>(result.Model);

        Assert.Equal(2, model.ResultCount);
        Assert.Equal(new[] { "bird-two", "bird-one" }, model.DisplayImagesList.Select(image => image.ImageUrl));
        Assert.Equal("bird", model.SearchQuery);
        Assert.Equal(3, model.CategoryId);
    }

    [Fact]
    public void RedigeraBilderCanFilterMissingDescriptions()
    {
        using var db = CreateContext();
        db.TblImages.AddRange(
            Image(1, "missing-sv", "", "English", new DateTime(2026, 4, 10), 3),
            Image(2, "missing-en", "Swedish", null, new DateTime(2026, 4, 10), 3),
            Image(3, "complete", "Swedish", "English", new DateTime(2026, 4, 10), 3));
        db.SaveChanges();

        var result = Assert.IsType<ViewResult>(CreateController(db).RedigeraBilder(
            "", "", null, null, 3, null, null, missingEnglishDescription: true));
        var model = Assert.IsType<UploadEditImagesViewModel>(result.Model);

        Assert.Equal(1, model.ResultCount);
        Assert.Equal("missing-en", Assert.Single(model.DisplayImagesList).ImageUrl);
    }

    private static TblImage Image(int id, string fileName, string? description, string? descriptionEn, DateTime date, int categoryId) =>
        new()
        {
            ImageId = id,
            ImageUrlName = fileName,
            ImageDescription = description,
            ImageDescriptionEn = descriptionEn,
            ImageDate = date,
            ImageCategoryId = categoryId
        };

    private static ArvidsonFotoCoreDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<ArvidsonFotoCoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static UploadAdminController CreateController(ArvidsonFotoCoreDbContext db) =>
        new(
            db,
            null!,
            null!,
            NullLogger<ApiImageService>.Instance,
            NullLogger<ApiCategoryService>.Instance,
            new ConfigurationBuilder().Build(),
            new MemoryCache(new MemoryCacheOptions()),
            new TestLocalizer(),
            new PageCounterService(db));

    private sealed class TestLocalizer : IStringLocalizer<UploadAdminResource>
    {
        public LocalizedString this[string name] => new(name, name);

        public LocalizedString this[string name, params object[] arguments] =>
            new(name, string.Format(System.Globalization.CultureInfo.InvariantCulture, name, arguments));

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => [];
    }
}
