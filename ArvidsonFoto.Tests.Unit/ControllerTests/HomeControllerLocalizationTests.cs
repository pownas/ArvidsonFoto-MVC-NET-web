using ArvidsonFoto.Controllers;
using ArvidsonFoto.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;

namespace ArvidsonFoto.Tests.Unit.ControllerTests;

public class HomeControllerLocalizationTests
{
    private readonly HomeController _controller;

    public HomeControllerLocalizationTests()
    {
        var mockDbContext = new ArvidsonFotoDbContext();
        _controller = new HomeController(mockDbContext);

        // Setup HttpContext
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Fact]
    public void SetLanguage_SetsSwedishCulture_ReturnRedirect()
    {
        // Arrange
        var culture = "sv-SE";
        var returnUrl = "/";

        // Act
        var result = _controller.SetLanguage(culture, returnUrl);

        // Assert
        Assert.IsType<LocalRedirectResult>(result);
        var redirectResult = result as LocalRedirectResult;
        Assert.Equal(returnUrl, redirectResult?.Url);
        
        // Verify cookie was set
        var cookies = _controller.Response.Headers["Set-Cookie"];
        Assert.Contains(CookieRequestCultureProvider.DefaultCookieName, cookies.ToString());
        Assert.Contains("sv-SE", cookies.ToString());
    }

    [Fact]
    public void SetLanguage_SetsEnglishCulture_ReturnRedirect()
    {
        // Arrange
        var culture = "en-US";
        var returnUrl = "/Info/Gästbok";

        // Act
        var result = _controller.SetLanguage(culture, returnUrl);

        // Assert
        Assert.IsType<LocalRedirectResult>(result);
        var redirectResult = result as LocalRedirectResult;
        Assert.Equal(returnUrl, redirectResult?.Url);
        
        // Verify cookie was set
        var cookies = _controller.Response.Headers["Set-Cookie"];
        Assert.Contains(CookieRequestCultureProvider.DefaultCookieName, cookies.ToString());
        Assert.Contains("en-US", cookies.ToString());
    }

    [Fact]
    public void SetLanguage_WithComplexReturnUrl_PreservesUrl()
    {
        // Arrange
        var culture = "en-US";
        var returnUrl = "/Bilder/Fåglar?page=2";

        // Act
        var result = _controller.SetLanguage(culture, returnUrl);

        // Assert
        Assert.IsType<LocalRedirectResult>(result);
        var redirectResult = result as LocalRedirectResult;
        Assert.Equal(returnUrl, redirectResult?.Url);
    }
}
