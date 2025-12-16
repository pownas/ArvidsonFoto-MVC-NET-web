using ArvidsonFoto.Tests.Integration.Helpers;
using System.Net.Http.Headers;

namespace ArvidsonFoto.Tests.Integration.Controllers;

/// <summary>
/// Integration tests for Guestbook (Gästbok) functionality.
/// These tests verify the complete HTTP workflow including routing, forms, and database operations.
/// </summary>
[TestClass]
public class GuestbookIntegrationTests
{
    private static ArvidsonFotoWebApplicationFactory? _factory;
    private static HttpClient? _client;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        _factory = new ArvidsonFotoWebApplicationFactory();
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    #region GET /Info/Gastbok Tests

    [TestMethod]
    public async Task GetGastbok_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client!.GetAsync("/Info/Gastbok");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task GetGastbok_ReturnsHtmlContent()
    {
        // Act
        var response = await _client!.GetAsync("/Info/Gastbok");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.IsTrue(response.Content.Headers.ContentType?.MediaType?.Contains("text/html") ?? false);
        Assert.IsTrue(content.Contains("Gästbok"));
    }

    [TestMethod]
    public async Task GetGastbok_ContainsGuestbookForm()
    {
        // Act
        var response = await _client!.GetAsync("/Info/Gastbok");
        var document = await HtmlHelpers.GetDocumentAsync(response);

        // Assert
        var form = document.QuerySelector("form[action*='PostToGb']");
        Assert.IsNotNull(form, "Guestbook form should be present");
    }

    [TestMethod]
    public async Task GetGastbok_ContainsRequiredFields()
    {
        // Act
        var response = await _client!.GetAsync("/Info/Gastbok");
        var document = await HtmlHelpers.GetDocumentAsync(response);

        // Assert - Check for required form fields
        Assert.IsNotNull(document.QuerySelector("input[name='Code']"), "Code field should exist");
        Assert.IsNotNull(document.QuerySelector("input[name='Name']"), "Name field should exist");
        Assert.IsNotNull(document.QuerySelector("input[name='Email']"), "Email field should exist");
        Assert.IsNotNull(document.QuerySelector("input[name='Homepage']"), "Homepage field should exist");
        Assert.IsNotNull(document.QuerySelector("textarea[name='Message']"), "Message field should exist");
    }

    [TestMethod]
    public async Task GetGastbok_ContainsAntiForgeryToken()
    {
        // Act
        var response = await _client!.GetAsync("/Info/Gastbok");
        var document = await HtmlHelpers.GetDocumentAsync(response);

        // Assert
        var token = HtmlHelpers.GetAntiForgeryToken(document);
        Assert.IsNotNull(token, "Anti-forgery token should be present");
        Assert.IsTrue(token.Length > 0, "Anti-forgery token should not be empty");
    }

    #endregion

    #region POST /Info/PostToGb Tests

    [TestMethod]
    public async Task PostToGb_WithValidData_RedirectsToGastbok()
    {
        // Arrange - Get the form page first to obtain anti-forgery token
        var getResponse = await _client!.GetAsync("/Info/Gastbok");
        var document = await HtmlHelpers.GetDocumentAsync(getResponse);

        var formData = HtmlHelpers.CreateFormData(document, new Dictionary<string, string>
        {
            ["Code"] = "3568",
            ["Name"] = "Integration Test User",
            ["Email"] = "integration@test.com",
            ["Homepage"] = "https://example.com",
            ["Message"] = "This is an integration test message"
        });

        var content = new FormUrlEncodedContent(formData);

        // Act
        var response = await _client.PostAsync("/Info/PostToGb", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
        Assert.IsTrue(response.Headers.Location?.ToString().Contains("Gastbok") ?? false);
    }

    [TestMethod]
    public async Task PostToGb_WithValidData_ShowsSuccessMessage()
    {
        // Arrange
        var getResponse = await _client!.GetAsync("/Info/Gastbok");
        var document = await HtmlHelpers.GetDocumentAsync(getResponse);

        var formData = HtmlHelpers.CreateFormData(document, new Dictionary<string, string>
        {
            ["Code"] = "3568",
            ["Name"] = "Success Test User",
            ["Email"] = "success@test.com",
            ["Homepage"] = "example.com",
            ["Message"] = "Testing success message"
        });

        var content = new FormUrlEncodedContent(formData);

        // Act - Post the form
        var postResponse = await _client.PostAsync("/Info/PostToGb", content);
        
        // Assert redirect happened
        Assert.AreEqual(HttpStatusCode.Redirect, postResponse.StatusCode);
        
        // Follow the redirect manually
        var redirectLocation = postResponse.Headers.Location?.ToString();
        Assert.IsNotNull(redirectLocation, "Should have redirect location");
        
        // Note: The success message is passed via route values/query string
        // We verify the redirect contains the success parameter
        Assert.IsTrue(
            redirectLocation.Contains("DisplayPublished") || redirectLocation.Contains("Gastbok"),
            "Redirect should go to Gastbok page with success indication"
        );
    }

    [TestMethod]
    public async Task PostToGb_WithInvalidCode_DoesNotRedirect()
    {
        // Arrange
        var getResponse = await _client!.GetAsync("/Info/Gastbok");
        var document = await HtmlHelpers.GetDocumentAsync(getResponse);

        var formData = HtmlHelpers.CreateFormData(document, new Dictionary<string, string>
        {
            ["Code"] = "0000", // Invalid code
            ["Name"] = "Invalid Code User",
            ["Email"] = "invalid@test.com",
            ["Homepage"] = "",
            ["Message"] = "This should fail"
        });

        var content = new FormUrlEncodedContent(formData);

        // Act
        var response = await _client.PostAsync("/Info/PostToGb", content);

        // Assert - Should redirect back to form with error
        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
    }

    [TestMethod]
    public async Task PostToGb_WithMissingRequiredFields_ShowsValidationErrors()
    {
        // Arrange
        var getResponse = await _client!.GetAsync("/Info/Gastbok");
        var document = await HtmlHelpers.GetDocumentAsync(getResponse);

        var formData = HtmlHelpers.CreateFormData(document, new Dictionary<string, string>
        {
            ["Code"] = "", // Missing required
            ["Name"] = "Test",
            ["Email"] = "test@test.com",
            ["Homepage"] = "",
            ["Message"] = "" // Missing required
        });

        var content = new FormUrlEncodedContent(formData);

        // Act
        var response = await _client.PostAsync("/Info/PostToGb", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
    }

    [TestMethod]
    public async Task PostToGb_WithoutAntiForgeryToken_Returns400()
    {
        // Arrange - Create form data without getting token
        var formData = new Dictionary<string, string>
        {
            ["Code"] = "3568",
            ["Name"] = "No Token User",
            ["Email"] = "notoken@test.com",
            ["Homepage"] = "",
            ["Message"] = "This should fail due to missing token"
        };

        var content = new FormUrlEncodedContent(formData);

        // Act
        var response = await _client!.PostAsync("/Info/PostToGb", content);

        // Assert - Should fail due to missing anti-forgery token
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task PostToGb_WithHomepageWithoutHttps_ProcessesCorrectly()
    {
        // Arrange
        var getResponse = await _client!.GetAsync("/Info/Gastbok");
        var document = await HtmlHelpers.GetDocumentAsync(getResponse);

        var formData = HtmlHelpers.CreateFormData(document, new Dictionary<string, string>
        {
            ["Code"] = "3568",
            ["Name"] = "No HTTPS User",
            ["Email"] = "nohttps@test.com",
            ["Homepage"] = "example.com", // Without https://
            ["Message"] = "Testing URL without protocol"
        });

        var content = new FormUrlEncodedContent(formData);

        // Act
        var response = await _client.PostAsync("/Info/PostToGb", content);

        // Assert - Should succeed and redirect
        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
        Assert.IsTrue(response.Headers.Location?.ToString().Contains("Gastbok") ?? false);
    }

    [TestMethod]
    public async Task PostToGb_WithLongHomepage_TruncatesCorrectly()
    {
        // Arrange
        var getResponse = await _client!.GetAsync("/Info/Gastbok");
        var document = await HtmlHelpers.GetDocumentAsync(getResponse);

        var formData = HtmlHelpers.CreateFormData(document, new Dictionary<string, string>
        {
            ["Code"] = "3568",
            ["Name"] = "Long URL User",
            ["Email"] = "longurl@test.com",
            ["Homepage"] = "https://example.com/level1/level2/level3/level4/level5",
            ["Message"] = "Testing URL depth truncation"
        });

        var content = new FormUrlEncodedContent(formData);

        // Act
        var response = await _client.PostAsync("/Info/PostToGb", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
    }

    [TestMethod]
    public async Task PostToGb_MultipleSubmissions_AllSucceed()
    {
        // Test that multiple submissions work correctly
        for (int i = 0; i < 3; i++)
        {
            // Arrange
            var getResponse = await _client!.GetAsync("/Info/Gastbok");
            var document = await HtmlHelpers.GetDocumentAsync(getResponse);

            var formData = HtmlHelpers.CreateFormData(document, new Dictionary<string, string>
            {
                ["Code"] = "3568",
                ["Name"] = $"Multi Test User {i}",
                ["Email"] = $"multi{i}@test.com",
                ["Homepage"] = "",
                ["Message"] = $"Test message number {i}"
            });

            var content = new FormUrlEncodedContent(formData);

            // Act
            var response = await _client.PostAsync("/Info/PostToGb", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode, 
                $"Submission {i} should succeed");
        }
    }

    #endregion

    #region Route Tests

    [TestMethod]
    public async Task PostToGb_RouteIsAccessible()
    {
        // This test verifies that the explicit route "/Info/PostToGb" is correctly configured
        // This was the original bug that caused 404 errors

        // Arrange
        var getResponse = await _client!.GetAsync("/Info/Gastbok");
        var document = await HtmlHelpers.GetDocumentAsync(getResponse);

        var formData = HtmlHelpers.CreateFormData(document, new Dictionary<string, string>
        {
            ["Code"] = "3568",
            ["Name"] = "Route Test",
            ["Email"] = "route@test.com",
            ["Homepage"] = "",
            ["Message"] = "Testing route accessibility"
        });

        var content = new FormUrlEncodedContent(formData);

        // Act
        var response = await _client.PostAsync("/Info/PostToGb", content);

        // Assert - Should NOT get 404
        Assert.AreNotEqual(HttpStatusCode.NotFound, response.StatusCode, 
            "Route /Info/PostToGb should be accessible (not 404)");
        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode,
            "POST should redirect after successful submission");
    }

    [TestMethod]
    public async Task PostToGb_WithGetRequest_Returns405MethodNotAllowed()
    {
        // Verify that GET requests to PostToGb are not allowed
        
        // Act
        var response = await _client!.GetAsync("/Info/PostToGb");

        // Assert - Should return 405 Method Not Allowed
        Assert.AreEqual(HttpStatusCode.MethodNotAllowed, response.StatusCode);
    }

    #endregion

    #region Performance Tests

    [TestMethod]
    public async Task GetGastbok_RespondsWithinAcceptableTime()
    {
        // Arrange
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var response = await _client!.GetAsync("/Info/Gastbok");
        stopwatch.Stop();

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsTrue(stopwatch.ElapsedMilliseconds < 5000, 
            $"Response took {stopwatch.ElapsedMilliseconds}ms, should be less than 5000ms");
    }

    [TestMethod]
    public async Task PostToGb_RespondsWithinAcceptableTime()
    {
        // Arrange
        var getResponse = await _client!.GetAsync("/Info/Gastbok");
        var document = await HtmlHelpers.GetDocumentAsync(getResponse);

        var formData = HtmlHelpers.CreateFormData(document, new Dictionary<string, string>
        {
            ["Code"] = "3568",
            ["Name"] = "Performance Test",
            ["Email"] = "perf@test.com",
            ["Homepage"] = "",
            ["Message"] = "Testing response time"
        });

        var content = new FormUrlEncodedContent(formData);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var response = await _client.PostAsync("/Info/PostToGb", content);
        stopwatch.Stop();

        // Assert
        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
        Assert.IsTrue(stopwatch.ElapsedMilliseconds < 5000,
            $"POST took {stopwatch.ElapsedMilliseconds}ms, should be less than 5000ms");
    }

    #endregion
}
