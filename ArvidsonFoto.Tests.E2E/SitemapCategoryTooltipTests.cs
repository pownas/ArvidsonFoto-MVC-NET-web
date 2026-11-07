using Microsoft.Playwright;

namespace ArvidsonFoto.Tests.E2E;

[Collection("E2E")]
public sealed class SitemapCategoryTooltipTests(E2EWebApplicationFactory app) : IAsyncLifetime
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
    public async Task SitemapCategories_ShowPreviewOnlyAfterHover()
    {
        var page = await _browser!.NewPageAsync();
        await page.RouteAsync("https://arvidsonfoto.se/**", route => route.AbortAsync());
        var categoryRequests = 0;
        string? categoryId = null;
        await page.RouteAsync("**/api/image/getoneimagefromcategory/**", async route =>
        {
            if (categoryId is not null && new Uri(route.Request.Url).AbsolutePath.EndsWith("/" + categoryId, StringComparison.Ordinal))
            {
                categoryRequests++;
                await route.FulfillAsync(new RouteFulfillOptions
                {
                    ContentType = "application/json",
                    Body = """{"urlImage":"bilder/test/category-preview","categoryNameSv":"Testkategori","dateImageTaken":"2024-01-02T00:00:00"}"""
                });
                return;
            }

            await route.ContinueAsync();
        });

        await page.GotoAsync($"{app.BaseUrl}/Info/Sidkarta");
        var categoryLinks = page.Locator(".sitemap-category-tree .sitemap-category-link");
        var categoryCount = await categoryLinks.CountAsync();
        Assert.True(categoryCount > 0);
        Assert.Equal(categoryCount, await page.Locator(".sitemap-category-tree .sitemap-category-link.has-category-tooltip[data-category-id][data-category-name]").CountAsync());
        Assert.True(await page.Locator(".sitemap-category-level-1 .has-category-tooltip").CountAsync() > 0);

        var categoryLink = categoryLinks.First;
        categoryId = await categoryLink.GetAttributeAsync("data-category-id");
        Assert.False(string.IsNullOrWhiteSpace(categoryId));

        await page.WaitForTimeoutAsync(2500);
        Assert.Equal(0, categoryRequests);

        await page.RunAndWaitForRequestAsync(
            () => categoryLink.HoverAsync(),
            request => request.Url.Contains($"/api/image/getoneimagefromcategory/{categoryId}?", StringComparison.Ordinal));
        await page.Locator(".category-tooltip-content").WaitForAsync();

        Assert.Equal(1, categoryRequests);
        Assert.Contains(await categoryLink.GetAttributeAsync("data-category-name"), await page.Locator(".category-tooltip-content").InnerTextAsync());
        Assert.Contains("category-preview.thumb.jpg", await page.Locator(".category-tooltip-content img").GetAttributeAsync("src"));
    }
}
