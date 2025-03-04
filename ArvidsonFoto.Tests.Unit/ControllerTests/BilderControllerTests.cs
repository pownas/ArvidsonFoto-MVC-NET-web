using ArvidsonFoto.Controllers;
using ArvidsonFoto.Data;
using ArvidsonFoto.Models;
using ArvidsonFoto.Tests.Unit.MockServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace ArvidsonFoto.Tests.Unit.ControllerTests;
public class BilderControllerTests
{
    private readonly BilderController _controller;

    public BilderControllerTests()
    {
        var mockDbContext = new ArvidsonFotoDbContext();
        var mockImageService = new MockImageService();
        var mockCategoryService = new MockCategoryService();
        var mockPageCounterService = new MockPageCounterService();

        _controller = new BilderController(mockDbContext)
        {
            _imageService = mockImageService,
            _categoryService = mockCategoryService,
            _pageCounterService = mockPageCounterService
        };

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost");

        var actionContext = new ActionContext
        {
            HttpContext = httpContext,
            RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
        };

        var urlHelper = new UrlHelper(actionContext);
        _controller.Url = urlHelper;
    }

    [Fact]
    public void Index_ReturnsViewResult_WithGalleryViewModel()
    {
        // Arrange
        string subLevel1 = "Blames";

        // Act
        var result = _controller.Index(subLevel1, null, null, null, null, 1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<GalleryViewModel>(viewResult.Model);
        Assert.Equal("/Bilder/Blames", model.CurrentUrl);
    }

    [Fact]
    public void Bilder_RedirectsToCorrectUrl_WhenIDIsValid()
    {
        // Arrange
        int? id = 1;

        // Act
        var result = _controller.Bilder(id);

        // Assert
        var redirectResult = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/Bilder/Fåglar", redirectResult.Url);
    }

    [Fact]
    public void Sök_ReturnsViewResult_WithGalleryViewModel()
    {
        // Arrange
        string searchString = "test";

        // Act
        var result = _controller.Search(searchString);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<GalleryViewModel>(viewResult.Model);
        Assert.Equal("Söker efter: test", viewResult.ViewData["Title"]);
    }

    [Fact]
    public void Bilder_ReturnsRedirectResult_WhenIDIsInvalid()
    {
        // Arrange
        int? id = 0;

        // Act
        var result = _controller.Bilder(id);
        
        // Assert
        var redirectResult = Assert.IsType<RedirectResult>(result);
        Assert.Equal("./Senast/Fotograferad", redirectResult.Url);
    }
}

