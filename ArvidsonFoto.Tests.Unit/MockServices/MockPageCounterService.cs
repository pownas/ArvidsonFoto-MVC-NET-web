using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.Interfaces;
using ArvidsonFoto.Core.Models;
using System.Globalization;

namespace ArvidsonFoto.Tests.Unit.MockServices;

/// <summary>
/// Mock implementation of IPageCounterService for unit testing
/// Uses Core.Models.TblPageCounter from ArvidsonFotoCoreDbSeeder
/// </summary>
public class MockPageCounterService : IPageCounterService
{
    public void AddPageCount(string pageName) { }
    public void AddCategoryCount(int categoryId, string pageName) { }

    public List<TblPageCounter> GetMonthCount(string yearMonth, bool picturePage) =>
        ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_PageCounters
            .Where(pc => pc.MonthViewed == yearMonth && pc.PicturePage == picturePage)
            .ToList();

    public List<TblPageCounter> GetAllPageCountsGroupedByPageCount() =>
        ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_PageCounters
            .GroupBy(pc => pc.PageName)
            .Select(g => g.OrderByDescending(pc => pc.LastShowDate).First())
            .OrderByDescending(pc => pc.PageViews)
            .ToList();

    public List<TblPageCounter> GetTop20CategoryCountsGroupedByPageCount() =>
        ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_PageCounters
            .Where(pc => pc.PicturePage == true)
            .GroupBy(pc => pc.PageName)
            .Select(g => g.OrderByDescending(pc => pc.LastShowDate).First())
            .OrderByDescending(pc => pc.PageViews)
            .Take(20)
            .ToList();

    public List<TblPageCounter> GetAllPageCounts() =>
        ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_PageCounters
            .OrderByDescending(pc => pc.LastShowDate)
            .ToList();

    public Dictionary<string, int> GetMonthlyPageViewsChart(int monthsBack, bool picturePage)
    {
        var result = new Dictionary<string, int>();
        var now = DateTime.Now;

        for (int i = 0; i < monthsBack; i++)
        {
            var targetDate = now.AddMonths(-i);
            var yearMonth = $"{targetDate:yyyy-MM}";

            var views = ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_PageCounters
                .Where(pc => pc.MonthViewed == yearMonth && pc.PicturePage == picturePage)
                .Sum(pc => pc.PageViews);

            result[yearMonth] = views;
        }

        return result;
    }

    public Dictionary<string, int> GetMonthlyPageViewsChart(DateTime startDate, DateTime endDate, bool picturePage)
    {
        var result = new Dictionary<string, int>();
        var firstMonth = new DateTime(startDate.Year, startDate.Month, 1);
        var lastMonth = new DateTime(endDate.Year, endDate.Month, 1);
        for (var month = firstMonth; month <= lastMonth; month = month.AddMonths(1))
        {
            var key = month.ToString("yyyy-MM", CultureInfo.InvariantCulture);
            result[key] = ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_PageCounters
                .Where(pc => pc.MonthViewed == key && pc.PicturePage == picturePage)
                .Sum(pc => pc.PageViews);
        }

        return result;
    }

    public List<TblPageCounter> GetPageCountsInPeriod(DateTime startDate, DateTime endDate, bool picturePage) =>
        ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_PageCounters
            .Where(pc => pc.PicturePage == picturePage
                && string.Compare(pc.MonthViewed, startDate.ToString("yyyy-MM", CultureInfo.InvariantCulture), StringComparison.Ordinal) >= 0
                && string.Compare(pc.MonthViewed, endDate.ToString("yyyy-MM", CultureInfo.InvariantCulture), StringComparison.Ordinal) <= 0)
            .GroupBy(pc => picturePage ? pc.CategoryId.ToString(CultureInfo.InvariantCulture) : pc.PageName)
            .Select(g =>
            {
                var latest = g.OrderByDescending(pc => pc.LastShowDate).First();
                return new TblPageCounter
                {
                    Id = latest.Id,
                    PageName = latest.PageName,
                    CategoryId = latest.CategoryId,
                    PicturePage = latest.PicturePage,
                    MonthViewed = latest.MonthViewed,
                    LastShowDate = latest.LastShowDate,
                    PageViews = g.Sum(pc => pc.PageViews)
                };
            })
            .OrderByDescending(pc => pc.PageViews)
            .ToList();
}