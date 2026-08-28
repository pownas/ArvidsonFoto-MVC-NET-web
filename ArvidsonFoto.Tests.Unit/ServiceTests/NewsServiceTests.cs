using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.Models;
using ArvidsonFoto.Core.Services;
using ArvidsonFoto.Tests.Unit.MockServices;
using Microsoft.EntityFrameworkCore;

namespace ArvidsonFoto.Tests.Unit.ServiceTests;

public class NewsServiceTests
{
    [Fact]
    public void GetPublished_PopulatesExcerptAndSelectedImageUrls()
    {
        var options = new DbContextOptionsBuilder<ArvidsonFotoCoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new ArvidsonFotoCoreDbContext(options);

        context.TblImages.Add(new TblImage
        {
            Id = 1,
            ImageId = 1234,
            ImageCategoryId = 13,
            ImageUrlName = "blue-tit",
            ImageDescription = "Blåmes på gren"
        });

        context.TblNews.Add(new TblNews
        {
            Id = 1,
            NewsId = 1,
            NewsTitle = "Ny artikel",
            NewsContent = "<p>En <strong>rik</strong> nyhetstext med HTML.</p>",
            NewsAuthor = "Test",
            NewsPublished = true,
            NewsImageId = 1234
        });

        context.SaveChanges();

        var categoryService = new MockApiCategoryService();
        var service = new NewsService(context, categoryService);

        var news = Assert.Single(service.GetPublished());
        var expectedImageUrl = $"https://arvidsonfoto.se/bilder/{categoryService.GetCategoryPathForImage(13)}/blue-tit";

        Assert.Equal("En rik nyhetstext med HTML.", news.NewsExcerpt);
        Assert.Equal("Blåmes på gren", news.NewsImageAlt);
        Assert.Equal("Blåmes på gren", news.NewsImageDescription);
        Assert.Equal(expectedImageUrl, news.NewsImageUrl);
        Assert.Equal($"{expectedImageUrl}.thumb.jpg", news.NewsImageThumbnailUrl);
    }
}
