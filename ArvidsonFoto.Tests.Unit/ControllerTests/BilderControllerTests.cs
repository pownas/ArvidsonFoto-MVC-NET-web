using System.Diagnostics;
using System.Globalization;
using ArvidsonFoto.Controllers;
using ArvidsonFoto.Core.ViewModels;
using ArvidsonFoto.Tests.Unit.MockServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace ArvidsonFoto.Tests.Unit.ControllerTests;

public class BilderControllerTests : IDisposable
{
    private readonly BilderController _controller;
    private readonly CultureInfo _originalCulture;

    public BilderControllerTests()
    {
        _originalCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("sv-SE");

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

    public void Dispose()
    {
        CultureInfo.CurrentUICulture = _originalCulture;
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
        Assert.Equal("/bilder/faglar", model.CurrentUrl);
        Assert.NotNull(model.SelectedCategory);
        Assert.Equal("Fåglar", model.SelectedCategory.NameSv);
    }

    [Fact]
    public void Index_CreatesChildActivity_WithSameTraceIdAsRequest()
    {
        ActivityTraceId traceId = default;
        ActivitySpanId parentSpanId = default;
        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "ArvidsonFoto",
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
            ActivityStarted = activity =>
            {
                if (activity.OperationName == "BilderController.Index")
                {
                    traceId = activity.TraceId;
                    parentSpanId = activity.ParentSpanId;
                }
            }
        };
        ActivitySource.AddActivityListener(listener);

        using var requestActivity = new Activity("ASP.NET Core request")
            .SetIdFormat(ActivityIdFormat.W3C)
            .Start();

        _ = _controller.Index("Fåglar", null, null, null, null, 1);

        Assert.Equal(requestActivity.TraceId, traceId);
        Assert.Equal(requestActivity.SpanId, parentSpanId);
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
        Assert.Equal("/bilder/tattingar/mesar/blames", model.CurrentUrl);
        Assert.NotNull(model.SelectedCategory);
        Assert.Equal("Blåmes", model.SelectedCategory.NameSv);
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
        Assert.Equal("/bilder/faglar", redirectResult.Url);
        Assert.False(redirectResult.Permanent);
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
        Assert.Equal("/bilder/insekter", redirectResult.Url);
        Assert.False(redirectResult.Permanent);
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
        Assert.Equal("/senast/fotograferad", redirectResult.Url);
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
        Assert.Equal("/senast/fotograferad", redirectResult.Url);
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