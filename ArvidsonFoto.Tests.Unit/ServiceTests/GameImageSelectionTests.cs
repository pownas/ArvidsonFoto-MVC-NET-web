using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.Models;
using ArvidsonFoto.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

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
            });
        dbContext.TblImages.AddRange(
            new TblImage { Id = 1, ImageId = 100, ImageCategoryId = 2, ImageUrlName = "bird-enabled" },
            new TblImage { Id = 2, ImageId = 101, ImageCategoryId = 2, ImageUrlName = "bird-disabled", IsGameSelectable = false },
            new TblImage { Id = 3, ImageId = 102, ImageCategoryId = 3, ImageUrlName = "landscape-enabled" });
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

        Assert.Equal([100], allSelectable.Select(image => image.ImageId));
        Assert.Equal([100], birdsOnly.Select(image => image.ImageId));
        Assert.Empty(disabledCategory);
    }
}
