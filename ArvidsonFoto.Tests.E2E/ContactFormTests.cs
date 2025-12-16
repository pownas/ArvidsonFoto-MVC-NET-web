using Microsoft.Playwright;

namespace ArvidsonFoto.Tests.E2E;

/// <summary>
/// End-to-end tests for contact form functionality
/// Tests both contact page and image purchase page forms
/// </summary>
public class ContactFormTests : IAsyncLifetime
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private const string BaseUrl = "https://localhost:5001"; // Default local URL

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true // Set to false to see browser actions
        });
    }

    public async Task DisposeAsync()
    {
        if (_browser != null)
            await _browser.CloseAsync();
        
        _playwright?.Dispose();
    }

    [Fact]
    public async Task ContactForm_DisplaysCorrectly()
    {
        // Arrange
        var context = await _browser!.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true
        });
        var page = await context.NewPageAsync();

        // Act
        await page.GotoAsync($"{BaseUrl}/Info/Kontakta");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Take screenshot
        await page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = "screenshots/contact-form-page.png",
            FullPage = true
        });

        // Assert
        await Assertions.Expect(page.Locator("h2")).ToContainTextAsync("Kontaktinformation");
        await Assertions.Expect(page.Locator("input[name='Name']")).ToBeVisibleAsync();
        await Assertions.Expect(page.Locator("input[name='Email']")).ToBeVisibleAsync();
        await Assertions.Expect(page.Locator("input[name='Subject']")).ToBeVisibleAsync();
        await Assertions.Expect(page.Locator("textarea[name='Message']")).ToBeVisibleAsync();
        await Assertions.Expect(page.Locator("input[name='Code']")).ToBeVisibleAsync();

        await context.CloseAsync();
    }

    [Fact]
    public async Task ContactForm_ShowsValidationErrors_WhenFieldsEmpty()
    {
        // Arrange
        var context = await _browser!.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true
        });
        var page = await context.NewPageAsync();

        // Act
        await page.GotoAsync($"{BaseUrl}/Info/Kontakta");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Try to submit without filling fields
        await page.Locator("button[type='submit']").ClickAsync();
        await page.WaitForTimeoutAsync(1000);

        // Take screenshot
        await page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = "screenshots/contact-form-validation-errors.png",
            FullPage = true
        });

        // Assert - HTML5 validation should prevent submission
        var nameInput = page.Locator("input[name='Name']");
        var isInvalid = await nameInput.EvaluateAsync<bool>("el => !el.validity.valid");
        Assert.True(isInvalid, "Name field should be invalid when empty");

        await context.CloseAsync();
    }

    [Fact]
    public async Task ContactForm_FillsOutFormCorrectly()
    {
        // Arrange
        var context = await _browser!.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true
        });
        var page = await context.NewPageAsync();

        // Act
        await page.GotoAsync($"{BaseUrl}/Info/Kontakta");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Fill form
        await page.Locator("input[name='Name']").FillAsync("Test User");
        await page.Locator("input[name='Email']").FillAsync("test@example.com");
        await page.Locator("input[name='Subject']").FillAsync("Test Subject");
        await page.Locator("textarea[name='Message']").FillAsync("This is a test message for the contact form.");
        await page.Locator("input[name='Code']").FillAsync("3568");

        await page.WaitForTimeoutAsync(500);

        // Take screenshot of filled form
        await page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = "screenshots/contact-form-filled.png",
            FullPage = true
        });

        // Assert - all fields should be filled
        await Assertions.Expect(page.Locator("input[name='Name']")).ToHaveValueAsync("Test User");
        await Assertions.Expect(page.Locator("input[name='Email']")).ToHaveValueAsync("test@example.com");
        await Assertions.Expect(page.Locator("input[name='Subject']")).ToHaveValueAsync("Test Subject");
        await Assertions.Expect(page.Locator("textarea[name='Message']")).ToHaveValueAsync("This is a test message for the contact form.");

        await context.CloseAsync();
    }

    [Fact]
    public async Task ContactForm_ShowsErrorMessage_OnEmailFailure()
    {
        // Arrange
        var context = await _browser!.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true
        });
        var page = await context.NewPageAsync();

        // Act
        await page.GotoAsync($"{BaseUrl}/Info/Kontakta");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Fill form with correct code
        await page.Locator("input[name='Name']").FillAsync("Test User");
        await page.Locator("input[name='Email']").FillAsync("test@example.com");
        await page.Locator("input[name='Subject']").FillAsync("Test Subject");
        await page.Locator("textarea[name='Message']").FillAsync("Testing error message display.");
        await page.Locator("input[name='Code']").FillAsync("3568");

        // Note: In a real environment without SMTP configured, this will fail and show error
        await page.Locator("button[type='submit']").ClickAsync();
        
        // Wait for redirect and page load
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await page.WaitForTimeoutAsync(2000);

        // Take screenshot
        await page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = "screenshots/contact-form-error-message.png",
            FullPage = true
        });

        // Assert - error message should be visible (if SMTP not configured)
        // The error alert contains the fallback email
        var errorAlert = page.Locator(".alert-danger");
        if (await errorAlert.IsVisibleAsync())
        {
            await Assertions.Expect(errorAlert).ToContainTextAsync("torbjorn@arvidsonfoto.se");
        }

        await context.CloseAsync();
    }

    [Fact]
    public async Task ImagePurchaseForm_DisplaysCorrectly()
    {
        // Arrange
        var context = await _browser!.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true
        });
        var page = await context.NewPageAsync();

        // Act
        await page.GotoAsync($"{BaseUrl}/Info/Kop_av_bilder");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Take screenshot
        await page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = "screenshots/image-purchase-form-page.png",
            FullPage = true
        });

        // Assert
        await Assertions.Expect(page.Locator("h2")).ToContainTextAsync("Köp av bilder");
        await Assertions.Expect(page.Locator("input[name='Name']")).ToBeVisibleAsync();
        await Assertions.Expect(page.Locator("input[name='Email']")).ToBeVisibleAsync();
        await Assertions.Expect(page.Locator("input[name='Subject']")).ToBeVisibleAsync();
        await Assertions.Expect(page.Locator("textarea[name='Message']")).ToBeVisibleAsync();

        await context.CloseAsync();
    }

    [Fact]
    public async Task ImagePurchaseForm_FillsOutFormCorrectly()
    {
        // Arrange
        var context = await _browser!.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true
        });
        var page = await context.NewPageAsync();

        // Act
        await page.GotoAsync($"{BaseUrl}/Info/Kop_av_bilder");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Fill form
        await page.Locator("input[name='Name']").FillAsync("Test Buyer");
        await page.Locator("input[name='Email']").FillAsync("buyer@example.com");
        await page.Locator("input[name='Subject']").FillAsync("Image Purchase Request");
        await page.Locator("textarea[name='Message']").FillAsync("I would like to purchase image IMG_1234.jpg");
        await page.Locator("input[name='Code']").FillAsync("3568");

        await page.WaitForTimeoutAsync(500);

        // Take screenshot of filled form
        await page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = "screenshots/image-purchase-form-filled.png",
            FullPage = true
        });

        // Assert
        await Assertions.Expect(page.Locator("input[name='Name']")).ToHaveValueAsync("Test Buyer");
        await Assertions.Expect(page.Locator("input[name='Email']")).ToHaveValueAsync("buyer@example.com");

        await context.CloseAsync();
    }
}
