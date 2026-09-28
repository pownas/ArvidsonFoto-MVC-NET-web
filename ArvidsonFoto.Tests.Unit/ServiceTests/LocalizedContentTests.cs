using System.Globalization;
using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.DTOs;
using ArvidsonFoto.Core.Extensions;
using ArvidsonFoto.Core.Models;
using ArvidsonFoto.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArvidsonFoto.Tests.Unit.ServiceTests;

public class LocalizedContentTests
{
    [Theory]
    [InlineData("sv-SE", "Fåglar", "faglar", "Svensk beskrivning")]
    [InlineData("en-US", "Birds", "birds", "English description")]
    public void Dtos_SelectRequestedLanguage(string culture, string name, string slug, string description)
    {
        var original = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
            var category = new TblMenu
            {
                MenuDisplayName = "Fåglar", MenuDisplayNameEn = "Birds",
                MenuUrlSegment = "faglar", MenuUrlSegmentEn = "birds"
            }.ToCategoryDto("faglar", string.Empty);
            var image = new TblImage
            {
                ImageDescription = "Svensk beskrivning",
                ImageDescriptionEn = "English description"
            }.ToImageDto();

            Assert.Equal(name, category.Name);
            Assert.Equal(slug, category.DisplayUrlCategoryPath);
            Assert.Equal(description, image.Description);
            Assert.Equal("Fåglar", category.ToTblMenu().MenuDisplayName);
            Assert.Equal("Birds", category.ToTblMenu().MenuDisplayNameEn);
            Assert.Equal("Svensk beskrivning", image.ToTblImage().ImageDescription);
            Assert.Equal("English description", image.ToTblImage().ImageDescriptionEn);
        }
        finally
        {
            CultureInfo.CurrentUICulture = original;
        }
    }

    [Fact]
    public void EnglishFallsBackWhenTranslationIsEmpty()
    {
        var original = CultureInfo.CurrentUICulture;
        try
        {
            var category = new CategoryDto { Name = "Älg", NameEn = "  ", UrlCategoryPath = "alg" };
            var image = new ImageDto { Description = "Älg i skogen", DescriptionEn = null };
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            Assert.Equal("Älg", category.Name);
            Assert.Equal("alg", category.DisplayUrlCategoryPath);
            Assert.Equal("Älg i skogen", image.Description);
            Assert.Equal("Svensk text", LocalizedText.Select("Svensk text", ""));
        }

        finally
        {
            CultureInfo.CurrentUICulture = original;
        }
    }

    [Fact]
    public void BulkCategoryNames_UseRequestLanguageAndFallbackAfterSwedishCache()
    {
        using var db = new ArvidsonFotoCoreDbContext(
            new DbContextOptionsBuilder<ArvidsonFotoCoreDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        db.TblMenus.AddRange(
            new TblMenu { Id = 1, MenuCategoryId = 1, MenuDisplayName = "Bäver", MenuDisplayNameEn = "Beaver" },
            new TblMenu { Id = 2, MenuCategoryId = 2, MenuDisplayName = "Älg", MenuDisplayNameEn = " " });
        db.SaveChanges();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new ApiCategoryService(NullLogger<ApiCategoryService>.Instance, db, cache);
        var original = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("sv-SE");
            Assert.Equal("Bäver", service.GetCategoryNamesBulk([1])[1]);

            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            var names = service.GetCategoryNamesBulk([1, 2]);
            Assert.Equal("Beaver", names[1]);
            Assert.Equal("Älg", names[2]);
        }
        finally
        {
            CultureInfo.CurrentUICulture = original;
            service.ClearCache();
        }
    }

    [Fact]
    public void NewDatabaseColumnsAreNullableAndMatchSwedishLengths()
    {
        using var db = new ArvidsonFotoCoreDbContext(
            new DbContextOptionsBuilder<ArvidsonFotoCoreDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        foreach (var (entity, column, length) in new[]
        {
            (typeof(TblGb), nameof(TblGb.GbNameEn), (int?)100),
            (typeof(TblGb), nameof(TblGb.GbTextEn), null),
            (typeof(TblMenu), nameof(TblMenu.MenuDisplayNameEn), (int?)50),
            (typeof(TblMenu), nameof(TblMenu.MenuUrlSegmentEn), (int?)50),
            (typeof(TblImage), nameof(TblImage.ImageDescriptionEn), (int?)150)
        })
        {
            var property = db.Model.FindEntityType(entity)!.FindProperty(column)!;
            Assert.True(property.IsNullable);
            Assert.Equal(length, property.GetMaxLength());
        }
    }

    [Fact]
    public void SeededCategoriesHaveEnglishNamesAndDistinctSlugs()
    {
        var categories = ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_MenuCategories;

        Assert.Equal(543, categories.Count);
        Assert.All(categories, category =>
        {
            Assert.False(string.IsNullOrWhiteSpace(category.MenuDisplayNameEn), $"Missing English name for {category.MenuCategoryId}");
            Assert.False(string.IsNullOrWhiteSpace(category.MenuUrlSegmentEn), $"Missing English URL for {category.MenuCategoryId}");
            Assert.True(category.MenuDisplayNameEn!.Length <= 50, $"English name too long for {category.MenuCategoryId}");
            Assert.True(category.MenuUrlSegmentEn!.Length <= 50, $"English URL too long for {category.MenuCategoryId}");
            Assert.Matches("^[a-z0-9]+(-[a-z0-9]+)*$", category.MenuUrlSegmentEn);
        });
        Assert.Equal(categories.Count, categories.Select(category => category.MenuUrlSegmentEn)
            .Distinct(StringComparer.OrdinalIgnoreCase).Count());
        Assert.All(categories, category =>
            Assert.DoesNotContain(categories, other =>
                other.Id != category.Id &&
                string.Equals(other.MenuUrlSegment, category.MenuUrlSegmentEn, StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void SeededGuestbookAndNonEmptyImageDescriptionsHaveEnglishContent()
    {
        Assert.All(ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_Guestbook, post =>
        {
            Assert.False(string.IsNullOrWhiteSpace(post.GbNameEn));
            Assert.False(string.IsNullOrWhiteSpace(post.GbTextEn));
        });

        var images = ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_Image;
        Assert.Equal(8, images.Count(image => !string.IsNullOrWhiteSpace(image.ImageDescription)));
        Assert.All(images, image =>
        {
            if (string.IsNullOrWhiteSpace(image.ImageDescription))
                Assert.Null(image.ImageDescriptionEn);
            else
                Assert.False(string.IsNullOrWhiteSpace(image.ImageDescriptionEn));
        });
    }
}
