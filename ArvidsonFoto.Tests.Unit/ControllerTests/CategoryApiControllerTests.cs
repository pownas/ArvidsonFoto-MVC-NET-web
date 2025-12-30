using ArvidsonFoto.Controllers.ApiControllers;
using ArvidsonFoto.Core.DTOs;
using ArvidsonFoto.Tests.Unit.MockServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ArvidsonFoto.Tests.Unit.ControllerTests;

/// <summary>
/// Unit tests for CategoryApiController using in-memory mock services
/// </summary>
public class CategoryApiControllerTests
{
    private readonly CategoryApiController _controller;
    private readonly MockApiCategoryService _mockCategoryService;

    public CategoryApiControllerTests()
    {
        // Create logger mock
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<CategoryApiController>();

        // Create mock service
        _mockCategoryService = new MockApiCategoryService();

        // Create controller
        _controller = new CategoryApiController(logger, _mockCategoryService);

        // Setup HTTP context
        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public void AddCategory_ValidCategory_ReturnsOkResult()
    {
        // Arrange
        var category = new CategoryDto
        {
            Name = "Test Category",
            UrlCategoryPath = "test-category"
        };

        // Act
        var result = _controller.AddCategory(category);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value!);
    }

    [Fact]
    public void AddCategory_NullCategory_ReturnsProblem()
    {
        // Arrange
        CategoryDto? category = null;

        // Act
        var result = _controller.AddCategory(category!);

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problemResult.StatusCode);
    }

    [Fact]
    public void GetAll_ReturnsAllCategories()
    {
        // Act
        var result = _controller.GetAll();

        // Assert
        Assert.IsType<List<CategoryDto>>(result);
        Assert.True(result.Count > 0);
        // Använd verkliga kategorier från DbSeederExtension
        Assert.Contains(result, c => c.Name == "Fåglar");
        Assert.Contains(result, c => c.Name == "Däggdjur");
        Assert.Contains(result, c => c.Name == "Insekter");
    }

    [Fact]
    public void GetAllPublicCached_ReturnsOkWithCategories()
    {
        // Act
        var result = _controller.GetAllPublicCached();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var categories = Assert.IsType<List<CategoryDto>>(okResult.Value);
        Assert.True(categories.Count > 0);
    }

    [Fact]
    public void GetById_ValidId_ReturnsOkWithCategory()
    {
        // Arrange - Använd verkligt ID från DbSeederExtension (MenuId=1 = Fåglar)
        int categoryId = 1;

        // Act
        var result = _controller.GetById(categoryId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var category = Assert.IsType<CategoryDto>(okResult.Value);
        Assert.Equal(categoryId, category.CategoryId);
        Assert.Equal("Fåglar", category.Name);
    }

    [Fact]
    public void GetById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        int categoryId = 0;

        // Act
        var result = _controller.GetById(categoryId);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var problemResult = result as ObjectResult;
        Assert.Equal(StatusCodes.Status400BadRequest, problemResult!.StatusCode);
    }

    [Fact]
    public void GetById_NonExistentId_ReturnsNotFound()
    {
        // Arrange
        int categoryId = 999;

        // Act
        var result = _controller.GetById(categoryId);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var problemResult = result as ObjectResult;
        Assert.Equal(StatusCodes.Status404NotFound, problemResult!.StatusCode);
    }

    [Fact]
    public void GetByName_ValidName_ReturnsCategory()
    {
        // Arrange - Använd verkligt namn från DbSeederExtension
        // Blåmes finns på Id=238 med MenuId=243
        string categoryName = "Blåmes";

        // Act
        var result = _controller.GetByName(categoryName);

        // Assert
        Assert.IsType<CategoryDto>(result);
        Assert.Equal("Blåmes", result.Name);
        Assert.Equal(243, result.CategoryId); // MenuId från DbSeederExtension (Id=238, MenuId=243)
    }

    [Fact]
    public void GetByName_InvalidName_ReturnsNotFoundCategory()
    {
        // Arrange
        string categoryName = "NonExistentCategory";

        // Act
        var result = _controller.GetByName(categoryName);

        // Assert
        Assert.IsType<CategoryDto>(result);
        Assert.Equal(-1, result.CategoryId);
        Assert.Equal("Not found", result.Name);
    }

    [Fact]
    public void GetSubsList_ValidParentId_ReturnsOkWithSubcategories()
    {
        // Arrange - Fåglar (MenuId=1) har många underkategorier
        int parentId = 1;

        // Act
        var result = _controller.GetSubsList(parentId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var subcategories = Assert.IsType<List<CategoryDto>>(okResult.Value);
        Assert.True(subcategories.Count > 0);
        // Tättingar (MenuId=23) är en underkategori till Fåglar
        Assert.Contains(subcategories, c => c.Name == "Tättingar");
        Assert.Contains(subcategories, c => c.Name == "Dagrovfåglar");
    }

    [Fact]
    public void GetSubsList_InvalidParentId_ReturnsBadRequest()
    {
        // Arrange
        int parentId = 0;

        // Act
        var result = _controller.GetSubsList(parentId);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var problemResult = result as ObjectResult;
        Assert.Equal(StatusCodes.Status400BadRequest, problemResult!.StatusCode);
    }

    [Fact]
    public void GetChildren_ValidParentId_ReturnsOkWithChildren()
    {
        // Arrange - Tättingar (MenuId=23) har många underkategorier
        int parentId = 23;

        // Act
        var result = _controller.GetChildren(parentId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var children = Assert.IsType<List<CategoryDto>>(okResult.Value);
        Assert.True(children.Count > 0);
        // Mesar (MenuId=208) är en underkategori till Tättingar
        Assert.Contains(children, c => c.Name == "Mesar");
        Assert.Contains(children, c => c.Name == "Finkar och siskor");
    }

    [Fact]
    public void GetNameById_ValidId_ReturnsOkWithName()
    {
        // Arrange - Blåmes (MenuId=243)
        int categoryId = 243;

        // Act
        var result = _controller.GetNameById(categoryId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Blåmes", okResult.Value);
    }

    [Fact]
    public void GetIdByName_ValidName_ReturnsOkWithId()
    {
        // Arrange
        string categoryName = "Blåmes";

        // Act
        var result = _controller.GetIdByName(categoryName);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(243, okResult.Value); // MenuId från DbSeederExtension
    }

    [Fact]
    public void GetLastId_ReturnsOkWithLastId()
    {
        // Act
        var result = _controller.GetLastId();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var lastId = Assert.IsType<int>(okResult.Value);
        Assert.True(lastId > 0);
    }

    [Fact]
    public void GetMainCategories_ReturnsOkWithMainCategories()
    {
        // Act
        var result = _controller.GetMainCategories();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var mainCategories = Assert.IsType<List<CategoryDto>>(okResult.Value);
        Assert.True(mainCategories.Count > 0);
        Assert.All(mainCategories, c => Assert.Null(c.ParentCategoryId));
    }

    [Fact]
    public void GetByUrlSegment_ValidSegment_ReturnsOkWithCategory()
    {
        // Arrange - Använd verkligt URL-segment från DbSeederExtension
        string urlSegment = "Blames"; // MenuUrltext från DbSeederExtension

        // Act
        var result = _controller.GetByUrlSegment(urlSegment);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var category = Assert.IsType<CategoryDto>(okResult.Value);
        Assert.Equal("Blåmes", category.Name);
        Assert.Equal(243, category.CategoryId);
    }

    [Fact]
    public void GetByUrlSegment_InvalidSegment_ReturnsBadRequest()
    {
        // Arrange
        string urlSegment = "";

        // Act
        var result = _controller.GetByUrlSegment(urlSegment);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var problemResult = result as ObjectResult;
        Assert.Equal(StatusCodes.Status400BadRequest, problemResult!.StatusCode);
    }

    [Fact]
    public void GetIdByUrlSegment_ValidSegment_ReturnsOkWithId()
    {
        // Arrange
        string urlSegment = "Blames";

        // Act
        var result = _controller.GetIdByUrlSegment(urlSegment);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(243, okResult.Value);
    }

    [Fact]
    public void GetByUrlSegmentWithFallback_ValidSegment_ReturnsOkWithCategory()
    {
        // Arrange
        string urlSegment = "Blames";

        // Act
        var result = _controller.GetByUrlSegmentWithFallback(urlSegment);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var category = Assert.IsType<CategoryDto>(okResult.Value);
        Assert.Equal("Blåmes", category.Name);
        Assert.Equal(243, category.CategoryId);
    }

    [Fact]
    public void GetByUrlSegmentWithFallback_NumericId_ReturnsOkWithCategory()
    {
        // Arrange - Använd numeriskt ID för Blåmes (MenuId=243)
        string urlSegment = "243";

        // Act
        var result = _controller.GetByUrlSegmentWithFallback(urlSegment);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var category = Assert.IsType<CategoryDto>(okResult.Value);
        Assert.Equal("Blåmes", category.Name);
        Assert.Equal(243, category.CategoryId);
    }

    [Fact]
    public void ByCategoryPath_ValidPath_ReturnsOkWithPathInfo()
    {
        // Arrange - Använd verklig kategori-path från DbSeederExtension
        string categoryPath = "Faglar/Tattingar";

        // Act
        var result = _controller.ByCategoryPath(categoryPath);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public void ByCategoryPath_EmptyPath_ReturnsBadRequest()
    {
        // Arrange
        string categoryPath = "";

        // Act
        var result = _controller.ByCategoryPath(categoryPath);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Category path cannot be empty", badRequestResult.Value);
    }

    [Fact]
    public void ByCategoryPath_InvalidPath_ReturnsNotFound()
    {
        // Arrange
        string categoryPath = "nonexistent/category";

        // Act
        var result = _controller.ByCategoryPath(categoryPath);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains("not found", notFoundResult.Value!.ToString()!.ToLower());
    }
}