using ArvidsonFoto.Services;
using ArvidsonFoto.Tests.Unit.Security.Mocks;
using System.Net;
using System.Text.Json;

namespace ArvidsonFoto.Tests.Unit.ServiceTests;

public class FacebookServiceTests
{
    private readonly MockConfiguration _mockConfiguration;
    private readonly MockHttpMessageHandler _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;

    public FacebookServiceTests()
    {
        _mockConfiguration = new MockConfiguration();
        _mockHttpMessageHandler = new MockHttpMessageHandler();
        _httpClient = new HttpClient(_mockHttpMessageHandler);
    }

    [Fact]
    public void IsConfigured_ReturnsFalse_WhenPageAccessTokenIsMissing()
    {
        // Arrange
        _mockConfiguration.SetValue("Facebook:PageAccessToken", null);
        _mockConfiguration.SetValue("Facebook:PageId", "123456789");
        
        var service = new FacebookService(_mockConfiguration, _httpClient);

        // Act
        var result = service.IsConfigured();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsConfigured_ReturnsFalse_WhenPageIdIsMissing()
    {
        // Arrange
        _mockConfiguration.SetValue("Facebook:PageAccessToken", "test_token");
        _mockConfiguration.SetValue("Facebook:PageId", null);
        
        var service = new FacebookService(_mockConfiguration, _httpClient);

        // Act
        var result = service.IsConfigured();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsConfigured_ReturnsTrue_WhenBothValuesArePresent()
    {
        // Arrange
        _mockConfiguration.SetValue("Facebook:PageAccessToken", "test_token");
        _mockConfiguration.SetValue("Facebook:PageId", "123456789");
        
        var service = new FacebookService(_mockConfiguration, _httpClient);

        // Act
        var result = service.IsConfigured();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CreatePostAsync_ReturnsNull_WhenNotConfigured()
    {
        // Arrange
        _mockConfiguration.SetValue("Facebook:PageAccessToken", null);
        _mockConfiguration.SetValue("Facebook:PageId", "123456789");
        
        var service = new FacebookService(_mockConfiguration, _httpClient);
        var imageUrls = new List<string> { "https://example.com/image1.jpg" };
        var message = "Test message";

        // Act
        var result = await service.CreatePostAsync(imageUrls, message);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreatePostAsync_ReturnsFacebookUrl_WhenSingleImageUploadSucceeds()
    {
        // Arrange
        _mockConfiguration.SetValue("Facebook:PageAccessToken", "test_token");
        _mockConfiguration.SetValue("Facebook:PageId", "123456789");

        var responseContent = JsonSerializer.Serialize(new { id = "photo_12345" });
        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(responseContent)
        };

        _mockHttpMessageHandler.SetResponse(httpResponse);

        var service = new FacebookService(_mockConfiguration, _httpClient);
        var imageUrls = new List<string> { "https://example.com/image1.jpg" };
        var message = "Test message";

        // Act
        var result = await service.CreatePostAsync(imageUrls, message);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("facebook.com", result);
        Assert.Contains("photo_12345", result);
    }

    [Fact]
    public async Task CreatePostAsync_ReturnsNull_WhenFacebookApiReturnsError()
    {
        // Arrange
        _mockConfiguration.SetValue("Facebook:PageAccessToken", "test_token");
        _mockConfiguration.SetValue("Facebook:PageId", "123456789");

        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = new StringContent("{\"error\":{\"message\":\"Invalid OAuth access token\"}}")
        };

        _mockHttpMessageHandler.SetResponse(httpResponse);

        var service = new FacebookService(_mockConfiguration, _httpClient);
        var imageUrls = new List<string> { "https://example.com/image1.jpg" };
        var message = "Test message";

        // Act
        var result = await service.CreatePostAsync(imageUrls, message);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreatePostAsync_HandlesEmptyImageList()
    {
        // Arrange
        _mockConfiguration.SetValue("Facebook:PageAccessToken", "test_token");
        _mockConfiguration.SetValue("Facebook:PageId", "123456789");
        
        var service = new FacebookService(_mockConfiguration, _httpClient);
        var imageUrls = new List<string>();
        var message = "Test message";

        // Act
        var result = await service.CreatePostAsync(imageUrls, message);

        // Assert
        // The service should handle empty lists gracefully
        Assert.Null(result);
    }
}
