using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.Models;
using ArvidsonFoto.Core.Services;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ArvidsonFoto.Tests.Unit.ServiceTests;

public class PageCounterServiceStatisticsTests
{
    [Fact]
    public void MonthlyChartIncludesEmptyMonthsAndRangeTotalsGroupPagesAndCategories()
    {
        using var db = new ArvidsonFotoCoreDbContext(new DbContextOptionsBuilder<ArvidsonFotoCoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        db.TblPageCounter.AddRange(
            Counter("Startsidan", 4, "2025-01"),
            Counter("Startsidan", 6, "2025-02"),
            Counter("Info", 3, "2025-02"),
            Counter("Fåglar", 2, "2025-01", categoryId: 10, picturePage: true),
            Counter("Fåglar", 8, "2025-02", categoryId: 10, picturePage: true),
            Counter("Däggdjur", 1, "2025-01", categoryId: 20, picturePage: true),
            Counter("Kategori: Fåglar", 3, "2025-01", picturePage: true),
            Counter("Kategori: Älg", 4, "2025-01", picturePage: true));
        db.SaveChanges();

        var service = new PageCounterService(db);
        var months = service.GetMonthlyPageViewsChart(new DateTime(2025, 1, 1), new DateTime(2025, 3, 1), false);
        var pages = service.GetPageCountsInPeriod(new DateTime(2025, 1, 1), new DateTime(2025, 3, 1), false);
        var categories = service.GetPageCountsInPeriod(new DateTime(2025, 1, 1), new DateTime(2025, 3, 1), true);

        Assert.Equal(new Dictionary<string, int>
        {
            ["2025-01"] = 4,
            ["2025-02"] = 9,
            ["2025-03"] = 0
        }, months);
        Assert.Equal(10, Assert.Single(pages, page => page.PageName == "Startsidan").PageViews);
        Assert.Equal(3, Assert.Single(pages, page => page.PageName == "Info").PageViews);
        Assert.Equal(10, Assert.Single(categories, category => category.CategoryId == 10).PageViews);
        Assert.Equal(1, Assert.Single(categories, category => category.CategoryId == 20).PageViews);
        Assert.Equal(3, Assert.Single(categories, category => category.PageName == "Kategori: Fåglar").PageViews);
        Assert.Equal(4, Assert.Single(categories, category => category.PageName == "Kategori: Älg").PageViews);
        Assert.Equal(4, categories.Count);
        Assert.Empty(service.GetPageCountsInPeriod(new DateTime(2025, 2, 1), new DateTime(2025, 1, 1), false));
    }

    [Fact]
    public void MonthlyChartReturnsEmptyResultForReversedRange()
    {
        using var db = new ArvidsonFotoCoreDbContext(new DbContextOptionsBuilder<ArvidsonFotoCoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

        Assert.Empty(new PageCounterService(db).GetMonthlyPageViewsChart(
            new DateTime(2025, 2, 1), new DateTime(2025, 1, 1), false));
    }

    private static TblPageCounter Counter(string pageName, int views, string month, int categoryId = 0, bool picturePage = false) =>
        new()
        {
            PageName = pageName,
            PageViews = views,
            MonthViewed = month,
            CategoryId = categoryId,
            PicturePage = picturePage,
            LastShowDate = DateTime.ParseExact($"{month}-01", "yyyy-MM-dd", CultureInfo.InvariantCulture)
        };
}
