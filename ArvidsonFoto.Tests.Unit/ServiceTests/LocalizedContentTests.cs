using System.Globalization;
using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.DTOs;
using ArvidsonFoto.Core.Extensions;
using ArvidsonFoto.Core.Models;
using Microsoft.EntityFrameworkCore;

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
}
