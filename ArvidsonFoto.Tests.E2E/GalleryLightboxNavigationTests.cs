using Microsoft.Playwright;

namespace ArvidsonFoto.Tests.E2E;

[Collection("E2E")]
public sealed class GalleryLightboxNavigationTests(E2EWebApplicationFactory app) : IAsyncLifetime
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    async ValueTask IAsyncLifetime.InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync().ConfigureAwait(true);
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        }).ConfigureAwait(true);
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (_browser != null)
        {
            await _browser.CloseAsync();
        }

        GC.SuppressFinalize(this);
        _playwright?.Dispose();
    }

    [Fact]
    public async Task ClosingLightbox_UsesPageOfCurrentImageAfterBoundary()
    {
        var page = await _browser!.NewPageAsync();
        await page.GotoAsync($"{app.BaseUrl}/Info/Kontakta?keep=1&sida=2");
        await page.SetContentAsync("""
            <div id="gallery" data-current-page="2" data-total-pages="4" data-page-size="48" data-total-images="192"></div>
            <script>
                window.lightboxCallbacks = {};
                window.lightbox = {
                    on: (event, callback) => window.lightboxCallbacks[event] = callback,
                    getActiveSlideIndex: () => window.activeSlideIndex
                };
                window.GLightbox = () => window.lightbox;
            </script>
            """);
        await page.AddScriptTagAsync(new PageAddScriptTagOptions
        {
            Url = $"{app.BaseUrl}/js/gLightBoxOptions.js"
        });
        await page.EvaluateAsync("() => initGalleryInfiniteScroll()");

        await page.EvaluateAsync("() => { window.activeSlideIndex = 47; window.lightboxCallbacks.close(); }");
        Assert.Equal($"{app.BaseUrl}/Info/Kontakta?keep=1&sida=2", page.Url);

        await page.EvaluateAsync("() => { window.activeSlideIndex = 48; window.lightboxCallbacks.close(); }");
        await page.WaitForURLAsync(url => new Uri(url).Query.Contains("sida=3", StringComparison.Ordinal));
        Assert.Contains("keep=1", new Uri(page.Url).Query, StringComparison.Ordinal);
    }
}
