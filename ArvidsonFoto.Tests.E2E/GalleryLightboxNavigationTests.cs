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
    public async Task Dagrovfaglar_TooltipHoverUsesPageContextAndDoesNotRedirect()
    {
        var page = await _browser!.NewPageAsync();
        await page.SetViewportSizeAsync(1920, 1080);
        await page.RouteAsync("https://arvidsonfoto.se/**", route => route.AbortAsync());
        await page.GotoAsync($"{app.BaseUrl}/bilder/faglar/dagrovfaglar");
        var request = await page.RunAndWaitForRequestAsync(
            () => page.Locator(".has-category-tooltip[data-category-id='10']").DispatchEventAsync("mouseenter"),
            request => request.Url.Contains("/api/image/getoneimagefromcategory/", StringComparison.Ordinal));
        var parent = await page.Locator("meta[name='arvidsonfoto-traceparent']").GetAttributeAsync("content");
        Assert.False(string.IsNullOrWhiteSpace(parent));
        Assert.Equal(parent, request.Headers["traceparent"]);
        Assert.Equal("tooltip.hover", request.Headers["x-arvidsonfoto-operation"]);
        var response = await request.ResponseAsync();
        Assert.NotNull(response);
        Assert.Equal(200, response.Status);
        Assert.Null(request.RedirectedFrom);
        Assert.Contains("Dagrovfåglar", await page.Locator("main").InnerTextAsync(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task FetchHelper_PreservesHeadersWithoutLeakingPageContextToOtherOrigin()
    {
        var page = await _browser!.NewPageAsync();
        await page.RouteAsync("https://arvidsonfoto.se/**", route => route.AbortAsync());
        await page.GotoAsync($"{app.BaseUrl}/bilder/faglar/dagrovfaglar");
        var parent = await page.Locator("meta[name='arvidsonfoto-traceparent']").GetAttributeAsync("content");
        Assert.False(string.IsNullOrWhiteSpace(parent));
        await page.RouteAsync("**/api/trace-test", route => route.FulfillAsync(new RouteFulfillOptions
        {
            Status = 200,
            Body = "{}",
            ContentType = "application/json",
            Headers = new Dictionary<string, string> { ["Access-Control-Allow-Origin"] = "*" }
        }));

        var local = await page.RunAndWaitForRequestAsync(
            () => page.EvaluateAsync("() => window.ArvidsonFotoTelemetry.fetch('/api/trace-test', { headers: { Accept: 'application/json', 'Cache-Control': 'max-age=3600' }, cache: 'no-store' }, 'gallery.next-page').then(r => r.json())"),
            request => request.Url == app.BaseUrl + "/api/trace-test");
        Assert.Equal(parent, local.Headers["traceparent"]);
        Assert.Equal("application/json", local.Headers["accept"]);
        Assert.Equal("max-age=3600", local.Headers["cache-control"]);
        Assert.Equal("gallery.next-page", local.Headers["x-arvidsonfoto-operation"]);

        var external = await page.RunAndWaitForRequestAsync(
            () => page.EvaluateAsync("() => window.ArvidsonFotoTelemetry.fetch('https://other.example/api/trace-test', undefined, 'tooltip.hover').then(r => r.json())"),
            request => request.Url == "https://other.example/api/trace-test");
        Assert.False(external.Headers.ContainsKey("traceparent"));
        Assert.False(external.Headers.ContainsKey("x-arvidsonfoto-operation"));
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
