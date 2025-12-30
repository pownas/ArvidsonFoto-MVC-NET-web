using ArvidsonFoto.Controllers.ApiControllers;
using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.DTOs;
using ArvidsonFoto.Core.Models;
using ArvidsonFoto.Tests.Unit.MockServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ArvidsonFoto.Tests.Unit.ControllerTests;

/// <summary>
/// Unit tests for ImageApiController using in-memory mock services and database
/// </summary>
public class ImageApiControllerTests : IDisposable
{
    private readonly ImageApiController _controller;
    private readonly MockApiImageService _mockImageService;
    private readonly MockApiCategoryService _mockCategoryService;
    private readonly ArvidsonFotoCoreDbContext _dbContext;

    public ImageApiControllerTests()
    {
        // Create in-memory database
        var options = new DbContextOptionsBuilder<ArvidsonFotoCoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new ArvidsonFotoCoreDbContext(options);

        // Create logger mock
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<ImageApiController>();

        // Create configuration mock
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureFlags:NewGalleryCategory:Enabled"] = "true"
            })
            .Build();

        // Create mock services
        _mockImageService = new MockApiImageService();
        _mockCategoryService = new MockApiCategoryService();

        // Create controller
        _controller = new ImageApiController(logger, _mockImageService, _dbContext, _mockCategoryService, configuration);

        // Setup HTTP context
        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public void AddImage_ValidImage_ReturnsTrue()
    {
        // Arrange
        var image = new ImageDto
        {
            UrlImage = "TEST999", // Use different ID to avoid conflicts
            CategoryId = 99, // Use different category
            Description = "Test image",
            DateImageTaken = DateTime.Now
        };

        // Act
        var result = _controller.AddImage(image);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void AddImage_NullImage_ReturnsFalse()
    {
        // Arrange
        ImageDto? image = null;

        // Act
        var result = _controller.AddImage(image!);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GetAll_ReturnsAllImages()
    {
        // Act
        var result = _controller.GetAll();

        // Assert
        Assert.IsType<List<ImageDto>>(result);
        Assert.True(result.Count > 0);
        // Använd verkliga bilder från DbSeederExtension (första 100 bilder)
        Assert.Contains(result, i => i.UrlImage == "AP2D6321"); // ImageId=1
        Assert.Contains(result, i => i.UrlImage == "AP2D6366"); // ImageId=2
    }

    [Fact]
    public void GetImagesByCategoryID_ValidCategoryId_ReturnsImages()
    {
        // Arrange - Tretåig hackspett (ImageArt=49) från DbSeederExtension
        int categoryId = 49;

        // Act
        var result = _controller.GetImagesByCategoryID(categoryId);

        // Assert
        Assert.IsType<List<ImageDto>>(result);
        var images = result.Where(i => i.CategoryId == categoryId).ToList();
        Assert.True(images.Count > 0);
        // Första bilderna i DbSeederExtension är av Tretåig hackspett
        Assert.Contains(result, i => i.UrlImage == "AP2D6321");
    }

    [Fact]
    public void GetImagesByCategoryID_InvalidCategoryId_ReturnsEmptyList()
    {
        // Arrange
        int categoryId = 999; // Non-existent category

        // Act
        var result = _controller.GetImagesByCategoryID(categoryId);

        // Assert
        Assert.IsType<List<ImageDto>>(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetById_ValidId_ReturnsImage()
    {
        // Arrange - Första bilden från DbSeederExtension (ImageId=1)
        int imageId = 1;

        // Act
        var result = _controller.GetById(imageId);

        // Assert
        Assert.IsType<ImageDto>(result);
        Assert.Equal(imageId, result.ImageId);
        Assert.Equal("AP2D6321", result.UrlImage);
        // ImageArt=49 (Tretåig hackspett), ImageFamilj=27 (Hackspettar)
    }

    [Fact]
    public void GetById_InvalidId_ReturnsNotFoundImage()
    {
        // Arrange
        int imageId = 0;

        // Act
        var result = _controller.GetById(imageId);

        // Assert
        Assert.IsType<ImageDto>(result);
        Assert.Equal(-1, result.ImageId);
        Assert.Equal("404-NotFound", result.UrlImage);
    }

    [Fact]
    public void GetById_NonExistentId_ReturnsNotFoundImage()
    {
        // Arrange
        int imageId = 999;

        // Act
        var result = _controller.GetById(imageId);

        // Assert
        Assert.IsType<ImageDto>(result);
        Assert.Equal(-1, result.ImageId);
        Assert.Equal("404-NotFound", result.UrlImage);
    }

    [Fact]
    public void GetImageLastId_ReturnsValidId()
    {
        // Act
        var result = _controller.GetImageLastId();

        // Assert
        Assert.True(result > 0);
    }

    [Fact]
    public void GetOneImageFromCategory_ValidCategoryId_ReturnsImage()
    {
        // Arrange - Apollofjäril (MenuId=50) från DbSeederExtension
        int categoryId = 50;

        // Act
        var result = _controller.GetOneImageFromCategory(categoryId);

        // Assert
        Assert.IsType<ImageDto>(result);
        // Ska returnera en bild från kategorin Apollofjäril
        Assert.NotEqual("404-NotFound", result.UrlImage);
    }

    [Fact]
    public void GetOneImageFromCategory_MainCategory_ReturnsCategoryImage()
    {
        // Arrange - Fåglar (MenuId=1) ska returnera en bild från fågelkategorin
        int categoryId = 1;

        // Act
        var result = _controller.GetOneImageFromCategory(categoryId);

        // Assert
        Assert.IsType<ImageDto>(result);
        // Ska returnera en bild, inte 404
        Assert.NotEqual("404-NotFound", result.UrlImage);
        Assert.NotEqual(-1, result.ImageId);
    }

    [Fact]
    public void GetRandomNumberOfImages_ValidCount_ReturnsImages()
    {
        // Arrange
        int count = 3;

        // Act
        var result = _controller.GetRandomNumberOfImages(count);

        // Assert
        Assert.IsType<List<ImageDto>>(result);
        Assert.True(result.Count <= count);
        Assert.True(result.Count > 0);
    }

    [Fact]
    public void GetLatestImages_ValidCount_ReturnsOkWithImages()
    {
        // Arrange
        int count = 3;

        // Act
        var result = _controller.GetLatestImages(count);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var images = Assert.IsType<List<ImageDto>>(okResult.Value);
        Assert.True(images.Count <= count);
        Assert.True(images.Count > 0);
    }

    [Fact]
    public void GetLatestImages_InvalidCount_ReturnsBadRequest()
    {
        // Arrange
        int count = 0; // Invalid count

        // Act
        var result = _controller.GetLatestImages(count);

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, problemResult.StatusCode);
    }

    [Fact]
    public void GetLatestImages_CountTooHigh_ReturnsBadRequest()
    {
        // Arrange
        int count = 101; // Count too high

        // Act
        var result = _controller.GetLatestImages(count);

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, problemResult.StatusCode);
    }

    [Fact]
    public void UpdateImage_ValidImage_ReturnsTrue()
    {
        // Arrange - Använd verkliga värden från DbSeederExtension
        var uploadImage = new UploadImageInputDto
        {
            ImageId = 1, // Första bilden från DbSeederExtension
            ImageHuvudfamilj = null,
            ImageFamilj = 27, // Hackspettar
            ImageArt = 49, // Tretåig hackspett
            ImageUrl = "AP2D6321_Updated",
            ImageDescription = "Updated description",
            ImageDate = new DateTime(2011, 3, 2, 0, 0, 0),
            ImageHuvudfamiljNamn = null,
            ImageFamiljNamn = "Hackspettar",
            ImageArtNamn = "Tretåig hackspett",
            ImageUrlFullSrc = "full/path/AP2D6321_Updated"
        };

        var existingImage = new TblImage
        {
            Id = 1,
            ImageId = 1,
            ImageMainFamilyId = null,
            ImageFamilyId = 27,
            ImageCategoryId = 49,
            ImageUrlName = "AP2D6321",
            ImageDescription = null,
            ImageDate = new DateTime(2011, 3, 2, 0, 0, 0),
            ImageUpdate = new DateTime(2011, 8, 11, 18, 37, 29)
        };

        // Add a test image to the in-memory database first
        _dbContext.TblImages.Add(existingImage);
        _dbContext.SaveChanges();

        // Act
        var result = _controller.UpdateImage(uploadImage);

        // Assert
        Assert.True(result);
        
        // Verify the image was updated in the database
        var actualImage = _dbContext.TblImages.FirstOrDefault(i => i.ImageId == uploadImage.ImageId);
        Assert.NotNull(actualImage);
        Assert.Equal(uploadImage.ImageHuvudfamilj, actualImage.ImageMainFamilyId);
        Assert.Equal(uploadImage.ImageFamilj, actualImage.ImageFamilyId);
        Assert.Equal(uploadImage.ImageDescription, actualImage.ImageDescription);
        Assert.Equal(uploadImage.ImageUrl, actualImage.ImageUrlName);
    }

    [Fact]
    public void UpdateImage_NullImage_ReturnsFalse()
    {
        // Arrange
        UploadImageInputDto? uploadImage = null;

        // Act
        var result = _controller.UpdateImage(uploadImage!);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void UpdateImage_InvalidImageId_ReturnsFalse()
    {
        // Arrange
        var uploadImage = new UploadImageInputDto
        {
            ImageId = 0, // Invalid ID
            ImageHuvudfamilj = 10,
            ImageFamilj = 12,
            ImageArt = 13,
            ImageUrl = "TEST001",
            ImageDescription = "Test description",
            ImageDate = DateTime.Now,
            ImageHuvudfamiljNamn = "Tättingar",
            ImageFamiljNamn = "Mesar",
            ImageArtNamn = "Blåmes",
            ImageUrlFullSrc = "full/path/TEST001"
        };

        // Act
        var result = _controller.UpdateImage(uploadImage);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GetImagesByCategoryPath_ValidPath_ReturnsOkWithImages()
    {
        // Arrange - Använd verklig kategoripath från DbSeederExtension
        string categoryPath = "Faglar/Hackspettar/Tretaig-hackspett";

        // Act
        var result = _controller.GetImagesByCategoryPath(categoryPath);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public void GetImagesByCategoryPath_EmptyPath_ReturnsBadRequest()
    {
        // Arrange
        string categoryPath = "";

        // Act
        var result = _controller.GetImagesByCategoryPath(categoryPath);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Category path cannot be empty", badRequestResult.Value);
    }

    [Fact]
    public void GetImagesByCategoryPath_InvalidPath_ReturnsNotFound()
    {
        // Arrange
        string categoryPath = "nonexistent/category/path";

        // Act
        var result = _controller.GetImagesByCategoryPath(categoryPath);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains("not found", notFoundResult.Value!.ToString()!.ToLower());
    }

    [Fact]
    public void DeleteImgId_ShouldThrowNotImplementedException()
    {
        // Arrange
        int imageId = 1;

        // Act & Assert
        Assert.Throws<NotImplementedException>(() => _controller.DeleteImgId(imageId));
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}