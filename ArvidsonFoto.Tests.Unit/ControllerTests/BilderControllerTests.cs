using ArvidsonFoto.Controllers;
using ArvidsonFoto.Core.ViewModels;
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
        var mockImageService = new MockImageService();
        var mockApiCategoryService = new MockApiCategoryService();
        var mockPageCounterService = new MockPageCounterService();

        _controller = new BilderController(
            mockImageService,
            mockApiCategoryService,
            mockPageCounterService);

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
        // Arrange - Använd riktig kategori från DbSeederExtension (MenuId=1 = Fåglar)
        string subLevel1 = "Fåglar"; // MenuUrltext från DbSeederExtension

        // Act
        var result = _controller.Index(subLevel1, null, null, null, null, 1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<GalleryViewModel>(viewResult.Model);
        Assert.Equal("/Bilder/Fåglar", model.CurrentUrl);
        Assert.NotNull(model.SelectedCategory);
        Assert.Equal("Fåglar", model.SelectedCategory.Name);
    }

    [Fact]
    public void Index_ReturnsViewResult_WithSubCategory()
    {
        // Arrange - Använd underkategori från DbSeederExtension
        // Blåmes (MenuId=243) -> Mesar (MenuId=208) -> Tättingar (MenuId=23) -> Fåglar (MenuId=1)
        string subLevel1 = "Tättingar";
        string subLevel2 = "Mesar";
        string subLevel3 = "Blåmes";

        // Act
        var result = _controller.Index(subLevel1, subLevel2, subLevel3, null, null, 1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<GalleryViewModel>(viewResult.Model);
        Assert.Equal("/Bilder/Tättingar/Mesar/Blåmes", model.CurrentUrl);
        Assert.NotNull(model.SelectedCategory);
        Assert.Equal("Blåmes", model.SelectedCategory.Name);
    }

    [Fact]
    public void Index_ReturnsNotFound_WhenCategoryDoesNotExist()
    {
        // Arrange
        string subLevel1 = "NonExistentCategory";

        // Act
        var result = _controller.Index(subLevel1, null, null, null, null, 1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void Bilder_RedirectsToCorrectUrl_WhenIDIsValid()
    {
        // Arrange - Använd verkligt ID från DbSeederExtension (MenuId=1 = Fåglar)
        int? id = 1;

        // Act
        var result = _controller.Bilder(id);

        // Assert
        var redirectResult = Assert.IsType<RedirectResult>(result);
        // Controller använder GetNameById som returnerar MenuText (display-namn), inte MenuUrltext
        Assert.Equal("/Bilder/Fåglar", redirectResult.Url);
        Assert.True(redirectResult.Permanent);
    }

    [Fact]
    public void Bilder_RedirectsToCorrectUrl_ForInsekterCategory()
    {
        // Arrange - Använd verkligt ID från DbSeederExtension (MenuId=5 = Insekter)
        int? id = 5;

        // Act
        var result = _controller.Bilder(id);

        // Assert
        var redirectResult = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/Bilder/Insekter", redirectResult.Url);
        Assert.True(redirectResult.Permanent);
    }

    [Fact]
    public void Search_ReturnsViewResult_WithGalleryViewModel()
    {
        // Arrange - Sök efter riktigt kategori från DbSeederExtension
        string searchString = "Fåglar";

        // Act
        var result = _controller.Search(searchString);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<GalleryViewModel>(viewResult.Model);
        Assert.Equal("Söker efter: " + searchString, viewResult.ViewData["Title"]);
        Assert.NotNull(model.DisplayImagesList);
    }

    [Fact]
    public void Search_ReturnsEmptyResults_WhenNoMatch()
    {
        // Arrange
        string searchString = "ZzzNonExistentCategory123";

        // Act
        var result = _controller.Search(searchString);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<GalleryViewModel>(viewResult.Model);
        Assert.NotNull(model.DisplayImagesList);
        Assert.Empty(model.DisplayImagesList);
    }

    [Fact]
    public void Search_ReturnsViewResult_WithoutSearchQuery()
    {
        // Arrange
        string? searchString = null;

        // Act
        var result = _controller.Search(searchString);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Sök bland bild-kategorierna", viewResult.ViewData["Title"]);
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
        Assert.False(redirectResult.Permanent);
    }

    [Fact]
    public void Bilder_ReturnsRedirectResult_WhenIDIsTooLarge()
    {
        // Arrange - Använd ID större än max från DbSeederExtension
        int? id = 99999;

        // Act
        var result = _controller.Bilder(id);
        
        // Assert
        var redirectResult = Assert.IsType<RedirectResult>(result);
        Assert.Equal("./Senast/Fotograferad", redirectResult.Url);
    }

    [Fact]
    public void Index_HandlesPaginationCorrectly()
    {
        // Arrange - Använd kategori med många bilder
        string subLevel1 = "Fåglar";
        int page = 2;

        // Act
        var result = _controller.Index(subLevel1, null, null, null, null, page);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<GalleryViewModel>(viewResult.Model);
        Assert.Equal(page, model.CurrentPage);
    }

    [Fact]
    public void Index_DefaultsToPageOne_WhenPageIsNull()
    {
        // Arrange
        string subLevel1 = "Fåglar";

        // Act
        var result = _controller.Index(subLevel1, null, null, null, null, null);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<GalleryViewModel>(viewResult.Model);
        Assert.Equal(1, model.CurrentPage);
    }

    [Fact]
    public void Index_DefaultsToPageOne_WhenPageIsLessThanOne()
    {
        // Arrange
        string subLevel1 = "Fåglar";

        // Act
        var result = _controller.Index(subLevel1, null, null, null, null, 0);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<GalleryViewModel>(viewResult.Model);
        Assert.Equal(1, model.CurrentPage);
    }
}

