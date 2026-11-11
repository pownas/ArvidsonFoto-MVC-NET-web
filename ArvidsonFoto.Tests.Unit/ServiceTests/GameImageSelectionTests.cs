using ArvidsonFoto.Controllers.ApiControllers;
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
    [Theory]
    [InlineData(false, true, true, true)]
    [InlineData(false, true, true, false)]
    [InlineData(false, false, true, true)]
    [InlineData(false, true, false, true)]
    [InlineData(false, false, false, false)]
    [InlineData(true, true, true, true)]
    [InlineData(true, true, true, false)]
    [InlineData(true, false, true, true)]
    [InlineData(true, true, false, true)]
    [InlineData(true, false, false, false)]
    public void RandomEndpoint_ExposesCategoryAndEffectiveImageGameSelection(
        bool newGalleryEnabled, bool categorySelectable, bool parentSelectable, bool imageSelectable)
    {
        var options = new DbContextOptionsBuilder<ArvidsonFotoCoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var dbContext = new ArvidsonFotoCoreDbContext(options);
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        dbContext.TblMenus.AddRange(
            new TblMenu
            {
                Id = 1, MenuCategoryId = 10, MenuParentCategoryId = 0,
                MenuDisplayName = "Fåglar", MenuUrlSegment = "faglar",
                IsGameSelectable = parentSelectable
            },
            new TblMenu
            {
                Id = 2, MenuCategoryId = 11, MenuParentCategoryId = 10,
                MenuDisplayName = "Blåmes", MenuDisplayNameEn = "Blue tit", MenuUrlSegment = "blames",
                IsGameSelectable = categorySelectable
            });
        dbContext.TblImages.Add(new TblImage
        {
            Id = 1, ImageId = 100, ImageCategoryId = 11, ImageUrlName = "bird",
            IsGameSelectable = imageSelectable
        });
        dbContext.SaveChanges();

        var categoryService = new ApiCategoryService(NullLogger<ApiCategoryService>.Instance, dbContext, memoryCache);
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureFlags:NewGalleryCategory:Enabled"] = newGalleryEnabled.ToString()
            })
            .Build();
        var imageService = new ApiImageService(
            NullLogger<ApiImageService>.Instance, dbContext, configuration, categoryService);
        var controller = new ImageApiController(
            NullLogger<ImageApiController>.Instance, imageService, dbContext, categoryService);

        var image = Assert.Single(controller.GetRandomNumberOfImages(1));
        var expectedCategorySelection = categorySelectable && parentSelectable;
        Assert.Equal(expectedCategorySelection, image.CategoryIsGameSelectable);
        Assert.Equal(imageSelectable && expectedCategorySelection, image.IsGameSelectable);
        Assert.Equal("Blåmes", image.CategoryNameSv);
        Assert.Equal("Blue tit", image.CategoryNameEn);

        using var json = JsonDocument.Parse(JsonSerializer.Serialize(image, JsonSerializerOptions.Web));
        Assert.Equal(expectedCategorySelection, json.RootElement.GetProperty("categoryIsGameSelectable").GetBoolean());
        Assert.Equal(image.IsGameSelectable, json.RootElement.GetProperty("isGameSelectable").GetBoolean());
        Assert.False(json.RootElement.TryGetProperty("name", out _));
        Assert.Equal(imageSelectable, dbContext.TblImages.AsNoTracking().Single().IsGameSelectable);
        Assert.Equal(categorySelectable, categoryService.GetById(11).IsGameSelectable);

        var gameImages = controller.GetGameSelectableImages(1);
        if (image.IsGameSelectable)
        {
            Assert.True(Assert.Single(gameImages).CategoryIsGameSelectable);
        }
        else
        {
            Assert.Empty(gameImages);
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    [InlineData(999)]
    public void RandomImages_WithoutExistingCategory_AreUnavailableForGames(int? categoryId)
    {
        var options = new DbContextOptionsBuilder<ArvidsonFotoCoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var dbContext = new ArvidsonFotoCoreDbContext(options);
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        dbContext.TblImages.Add(new TblImage
        {
            Id = 1, ImageId = 100, ImageCategoryId = categoryId, ImageUrlName = "orphan"
        });
        dbContext.SaveChanges();
        var categoryService = new ApiCategoryService(NullLogger<ApiCategoryService>.Instance, dbContext, memoryCache);
        var imageService = new ApiImageService(
            NullLogger<ApiImageService>.Instance, dbContext, new ConfigurationBuilder().Build(), categoryService);

        var image = Assert.Single(imageService.GetRandomNumberOfImages(1));

        Assert.False(image.CategoryIsGameSelectable);
        Assert.False(image.IsGameSelectable);
        Assert.True(dbContext.TblImages.AsNoTracking().Single().IsGameSelectable);
    }

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
        Assert.False(disabledCategoryImage.IsGameSelectable);
        Assert.False(disabledCategoryImage.CategoryIsGameSelectable);
        Assert.False(Assert.Single(randomImages.Where(image => image.ImageId == 103)).CategoryIsGameSelectable);
        var disabledImage = Assert.Single(randomImages.Where(image => image.ImageId == 101));
        Assert.False(disabledImage.IsGameSelectable);
        Assert.True(disabledImage.CategoryIsGameSelectable);
        var disabledCategoryJson = JsonSerializer.Serialize(disabledCategoryImage, JsonSerializerOptions.Web);
        Assert.Contains("\"isGameSelectable\":false", disabledCategoryJson);
        Assert.Contains("\"categoryIsGameSelectable\":false", disabledCategoryJson);
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
