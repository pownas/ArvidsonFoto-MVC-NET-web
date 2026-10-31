using ArvidsonFoto.Controllers;
using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.Interfaces;
using ArvidsonFoto.Core.Models;
using ArvidsonFoto.Core.Services;
using ArvidsonFoto.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArvidsonFoto.Tests.Unit.ControllerTests;

public class UploadAdminStatisticsTests
{
    [Fact]
    public void IndexProvidesStatisticsModelForHomepagePartial()
    {
        using var db = CreateContext();

        var result = Assert.IsType<ViewResult>(CreateController(db).Index());

        Assert.IsType<StatisticsViewModel>(result.Model);
    }

    [Fact]
    public void StatisticsSupportsCurrentMonthAndLastThreeMonths()
    {
        using var db = CreateContext();
        var controller = CreateController(db);
        var currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

        var currentMonthResult = Assert.IsType<StatisticsViewModel>(
            Assert.IsType<ViewResult>(controller.Statistik("month", null, null)).Model);
        var lastThreeMonthsResult = Assert.IsType<StatisticsViewModel>(
            Assert.IsType<ViewResult>(controller.Statistik("3m", null, null)).Model);

        Assert.Equal(currentMonth, currentMonthResult.StartDate);
        Assert.Equal(currentMonth, currentMonthResult.EndDate);
        Assert.Equal(currentMonth.AddMonths(-2), lastThreeMonthsResult.StartDate);
        Assert.Equal(currentMonth, lastThreeMonthsResult.EndDate);
    }

    [Fact]
    public void StatisticsIncludesAllTimeViewsForTheAllTimeChart()
    {
        using var db = CreateContext();
        db.TblPageCounter.AddRange(
            new TblPageCounter { PageName = "Startsidan", PageViews = 12, MonthViewed = "2026-10", LastShowDate = DateTime.Today },
            new TblPageCounter { PageName = "Bilder", PageViews = 7, MonthViewed = "2026-09", LastShowDate = DateTime.Today.AddMonths(-1) });
        db.SaveChanges();

        var viewModel = Assert.IsType<StatisticsViewModel>(
            Assert.IsType<ViewResult>(CreateController(db).Statistik("month", null, null)).Model);

        Assert.Equal(2, viewModel.AllTimePageViews.Count);
        Assert.Equal("Startsidan", viewModel.AllTimePageViews[0].PageName);
        Assert.Equal(12, viewModel.AllTimePageViews[0].PageViews);
    }

    private static ArvidsonFotoCoreDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<ArvidsonFotoCoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static UploadAdminController CreateController(ArvidsonFotoCoreDbContext db) =>
        new(
            db,
            null!,
            null!,
            NullLogger<ApiImageService>.Instance,
            NullLogger<ApiCategoryService>.Instance,
            new ConfigurationBuilder().Build(),
            new MemoryCache(new MemoryCacheOptions()),
            new TestLocalizer(),
            new PageCounterService(db));

    private sealed class TestLocalizer : IStringLocalizer<UploadAdminResource>
    {
        public LocalizedString this[string name] => new(name, name);

        public LocalizedString this[string name, params object[] arguments] =>
            new(name, string.Format(System.Globalization.CultureInfo.InvariantCulture, name, arguments));

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => [];
    }
}
