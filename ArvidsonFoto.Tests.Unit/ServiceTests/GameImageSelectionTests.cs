using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.Models;
using ArvidsonFoto.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json;

namespace ArvidsonFoto.Tests.Unit.ServiceTests;

public class GameImageSelectionTests
{
    [Fact]
    public void SelectionFlags_DefaultToTrue()
    {
        Assert.True(new TblMenu().IsGameSelectable);
        Assert.True(new TblImage().IsGameSelectable);
    }

    [Fact]
    public void GetGameSelectableImages_RequiresSelectableImageAndCategory()
    {
        var options = new DbContextOptionsBuilder<ArvidsonFotoCoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var dbContext = new ArvidsonFotoCoreDbContext(options);
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());

        dbContext.TblMenus.AddRange(
            new TblMenu
            {
                Id = 1,
                MenuCategoryId = 1,
                MenuParentCategoryId = 0,
                MenuDisplayName = "Fåglar",
                MenuUrlSegment = "Faglar"
            },
            new TblMenu
            {
                Id = 2,
                MenuCategoryId = 2,
                MenuParentCategoryId = 1,
                MenuDisplayName = "Tättingar",
                MenuUrlSegment = "Tattingar"
            },
            new TblMenu
            {
                Id = 3,
                MenuCategoryId = 3,
                MenuParentCategoryId = 0,
                MenuDisplayName = "Landskap",
                MenuUrlSegment = "Landskap",
                IsGameSelectable = false
            },
            new TblMenu
            {
                Id = 4,
                MenuCategoryId = 4,
                MenuParentCategoryId = 1,
                MenuDisplayName = "Ej valbar underkategori",
                MenuUrlSegment = "ej-valbar-underkategori",
                IsGameSelectable = false
            },
            new TblMenu
            {
                Id = 5,
                MenuCategoryId = 5,
                MenuParentCategoryId = 4,
                MenuDisplayName = "Valbar under-underkategori",
                MenuUrlSegment = "valbar-under-underkategori"
            });
        dbContext.TblImages.AddRange(
            new TblImage { Id = 1, ImageId = 100, ImageCategoryId = 2, ImageUrlName = "bird-enabled" },
            new TblImage { Id = 2, ImageId = 101, ImageCategoryId = 2, ImageUrlName = "bird-disabled", IsGameSelectable = false },
            new TblImage { Id = 3, ImageId = 102, ImageCategoryId = 3, ImageUrlName = "landscape-enabled" },
            new TblImage { Id = 4, ImageId = 103, ImageCategoryId = 5, ImageUrlName = "nested-disabled-category" });
        dbContext.SaveChanges();

        var categoryService = new ApiCategoryService(
            NullLogger<ApiCategoryService>.Instance,
            dbContext,
            memoryCache);
        var imageService = new ApiImageService(
            NullLogger<ApiImageService>.Instance,
            dbContext,
            new ConfigurationBuilder().Build(),
            categoryService);

        var allSelectable = imageService.GetGameSelectableImages(20);
        var birdsOnly = imageService.GetGameSelectableImages(20, 1);
        var disabledCategory = imageService.GetGameSelectableImages(20, 3);
        var disabledDescendantCategory = imageService.GetGameSelectableImages(20, 4);
        var randomImages = imageService.GetRandomNumberOfImages(20);
        var regularCategoryImages = imageService.GetImagesByCategoryID(5);
        var paginatedCategoryImages = imageService.GetImagesByCategoryIDPaginated(5, 1, 48);

        Assert.Equal([100], allSelectable.Select(image => image.ImageId));
        Assert.Equal([100], birdsOnly.Select(image => image.ImageId));
        Assert.Empty(disabledCategory);
        Assert.Empty(disabledDescendantCategory);
        var disabledCategoryImage = Assert.Single(randomImages.Where(image => image.ImageId == 102));
        Assert.True(disabledCategoryImage.IsGameSelectable);
        Assert.False(disabledCategoryImage.IsCategoryGameSelectable);
        Assert.False(Assert.Single(randomImages.Where(image => image.ImageId == 103)).IsCategoryGameSelectable);
        var disabledImage = Assert.Single(randomImages.Where(image => image.ImageId == 101));
        Assert.False(disabledImage.IsGameSelectable);
        Assert.True(disabledImage.IsCategoryGameSelectable);
        var disabledCategoryJson = JsonSerializer.Serialize(disabledCategoryImage, JsonSerializerOptions.Web);
        Assert.Contains("\"isGameSelectable\":true", disabledCategoryJson);
        Assert.Contains("\"isCategoryGameSelectable\":false", disabledCategoryJson);
        Assert.False(Assert.Single(regularCategoryImages).IsGameSelectable);
        Assert.False(Assert.Single(paginatedCategoryImages).IsGameSelectable);
        Assert.Contains("\"isGameSelectable\":false", JsonSerializer.Serialize(Assert.Single(regularCategoryImages), JsonSerializerOptions.Web));
    }

    [Fact]
    public void PaginatedImages_UseDisplayNamesForPhysicalFilesAndAsciiSlugsForCategoryLinks()
    {
        var options = new DbContextOptionsBuilder<ArvidsonFotoCoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var dbContext = new ArvidsonFotoCoreDbContext(options);
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        dbContext.TblMenus.AddRange(
            new TblMenu
            {
                Id = 1,
                MenuCategoryId = 2,
                MenuParentCategoryId = 0,
                MenuDisplayName = "Däggdjur",
                MenuUrlSegment = "Däggdjur",
                MenuUrlSegmentEn = "Mammals"
            },
            new TblMenu
            {
                Id = 2,
                MenuCategoryId = 10,
                MenuParentCategoryId = 2,
                MenuDisplayName = "Björn",
                MenuUrlSegment = "Björn",
                MenuUrlSegmentEn = "Brown Bear"
            },
            new TblMenu
            {
                Id = 3,
                MenuCategoryId = 1,
                MenuParentCategoryId = 0,
                MenuDisplayName = "Fåglar",
                MenuUrlSegment = "Fåglar",
                MenuUrlSegmentEn = "Birds"
            },
            new TblMenu
            {
                Id = 4,
                MenuCategoryId = 11,
                MenuParentCategoryId = 1,
                MenuDisplayName = "Vadare",
                MenuUrlSegment = "Vadare",
                MenuUrlSegmentEn = "Waders"
            },
            new TblMenu
            {
                Id = 5,
                MenuCategoryId = 12,
                MenuParentCategoryId = 11,
                MenuDisplayName = "Morkulla",
                MenuUrlSegment = "Morkulla",
                MenuUrlSegmentEn = "Woodcock"
            });
        dbContext.TblImages.AddRange(
            new TblImage
            {
                Id = 1,
                ImageId = 100,
                ImageCategoryId = 10,
                ImageUrlName = "Bear 01"
            },
            new TblImage
            {
                Id = 2,
                ImageId = 101,
                ImageCategoryId = 12,
                ImageUrlName = "Woodcock 01"
            });
        dbContext.SaveChanges();

        var categoryService = new ApiCategoryService(NullLogger<ApiCategoryService>.Instance, dbContext, memoryCache);
        var imageService = new ApiImageService(
            NullLogger<ApiImageService>.Instance,
            dbContext,
            new ConfigurationBuilder().Build(),
            categoryService);

        var image = Assert.Single(imageService.GetImagesByCategoryIDPaginated(10, 1, 48));
        var birdImage = Assert.Single(imageService.GetImagesByCategoryIDPaginated(12, 1, 48));

        Assert.Equal("bilder/D%C3%A4ggdjur/Bj%C3%B6rn/Bear%2001", image.UrlImage);
        Assert.Equal("bilder/daggdjur/bjorn", image.UrlCategorySv);
        Assert.Equal("images/mammals/brown-bear", image.UrlCategoryEn);
        Assert.Equal("bilder/Vadare/Morkulla/Woodcock%2001", birdImage.UrlImage);
        Assert.Equal("bilder/faglar/vadare/morkulla", birdImage.UrlCategorySv);
        Assert.Equal("images/birds/waders/woodcock", birdImage.UrlCategoryEn);
    }
}
