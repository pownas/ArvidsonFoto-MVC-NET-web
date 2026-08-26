using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.Interfaces;
using ArvidsonFoto.Core.Models;
using ArvidsonFoto.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace ArvidsonFoto.Tests.Integration.Controllers;

[TestClass]
public class NewsIntegrationTests
{
    private static ArvidsonFotoWebApplicationFactory? _factory;
    private static HttpClient? _client;

    private int _newsId;
    private string _expectedImageUrl = string.Empty;
    private string _expectedThumbnailUrl = string.Empty;
    private string _expectedDescription = string.Empty;

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

    [TestInitialize]
    public void TestInitialize()
    {
        using var scope = _factory!.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ArvidsonFotoCoreDbContext>();
        var categoryService = scope.ServiceProvider.GetRequiredService<IApiCategoryService>();

        db.TblNews.RemoveRange(db.TblNews);

        var image = db.TblImages.First(img => img.ImageId.HasValue && img.ImageCategoryId.HasValue && !string.IsNullOrWhiteSpace(img.ImageUrlName));
        var categoryPath = categoryService.GetCategoryPathForImage(image.ImageCategoryId!.Value);

        _newsId = 9001;
        _expectedImageUrl = $"https://arvidsonfoto.se/bilder/{categoryPath}/{image.ImageUrlName}";
        _expectedThumbnailUrl = $"{_expectedImageUrl}.thumb.jpg";
        _expectedDescription = image.ImageDescription ?? "Testbild";

        db.TblNews.Add(new TblNews
        {
            NewsId = _newsId,
            NewsTitle = "Integrationstest för nyhet",
            NewsContent = "<p>Det här är en <strong>testartikel</strong> för nyhetsvyn.</p>",
            NewsAuthor = "Integration Test",
            NewsSummary = "Kort sammanfattning för testkortet.",
            NewsPublished = true,
            NewsCreated = DateTime.UtcNow,
            NewsUpdated = DateTime.UtcNow,
            NewsImageId = image.ImageId
        });

        db.SaveChanges();
    }

    [TestMethod]
    public async Task GetNyheter_RendersClickableCardWithThumbnail()
    {
        var response = await _client!.GetAsync("/Nyheter");
        var document = await HtmlHelpers.GetDocumentAsync(response);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsNotNull(document.QuerySelector($"a.news-card-link[href='/Nyheter/{_newsId}']"), "News card link should be present");

        var image = document.QuerySelector($"a.news-card-link[href='/Nyheter/{_newsId}'] img.news-card-image") as IHtmlImageElement;
        Assert.IsNotNull(image, "Thumbnail image should be rendered in the card");
        Assert.AreEqual(_expectedThumbnailUrl, image.Source);
    }

    [TestMethod]
    public async Task GetArticle_RendersSelectedCoverImage()
    {
        var response = await _client!.GetAsync($"/Nyheter/{_newsId}");
        var document = await HtmlHelpers.GetDocumentAsync(response);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var image = document.QuerySelector(".news-article-image") as IHtmlImageElement;
        Assert.IsNotNull(image, "Selected cover image should be rendered in the article");
        Assert.AreEqual(_expectedImageUrl, image.Source);
        Assert.IsTrue(document.Body?.TextContent.Contains(_expectedDescription) ?? false);
    }
}
