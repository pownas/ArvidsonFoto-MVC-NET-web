using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Models;
using ArvidsonFoto.Services;

namespace ArvidsonFoto.Tests.Unit.MockServices;

/// <summary>
/// Mock implementation of IPageCounterService for unit testing
/// Uses data from ArvidsonFotoCoreDbSeeder to match production data structure
/// </summary>
public class MockPageCounterService : IPageCounterService
{
    public void AddPageCount(string pageName) { }
    public void AddCategoryCount(int categoryId, string pageName) { }

    public List<TblPageCounter> GetMonthCount(string yearMonth, bool picturePage) => 
        ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_PageCounters
            .Where(pc => pc.MonthViewed == yearMonth && pc.PicturePage == picturePage)
            .Select(coreCounter => new TblPageCounter
            {
                Id = coreCounter.Id,
                CategoryId = coreCounter.CategoryId,
                PicturePage = coreCounter.PicturePage,
                PageName = coreCounter.PageName ?? string.Empty,
                MonthViewed = coreCounter.MonthViewed ?? string.Empty,
                LastShowDate = coreCounter.LastShowDate,
                PageViews = coreCounter.PageViews
            })
            .ToList();
    
    public List<TblPageCounter> GetAllPageCountsGroupedByPageCount() =>
        ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_PageCounters
            .GroupBy(pc => pc.PageName)
            .Select(g => g.OrderByDescending(pc => pc.LastShowDate).First())
            .Select(coreCounter => new TblPageCounter
            {
                Id = coreCounter.Id,
                CategoryId = coreCounter.CategoryId,
                PicturePage = coreCounter.PicturePage,
                PageName = coreCounter.PageName ?? string.Empty,
                MonthViewed = coreCounter.MonthViewed ?? string.Empty,
                LastShowDate = coreCounter.LastShowDate,
                PageViews = coreCounter.PageViews
            })
            .OrderByDescending(pc => pc.PageViews)
            .ToList();
    
    public List<TblPageCounter> GetTop20CategoryCountsGroupedByPageCount() =>
        ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_PageCounters
            .Where(pc => pc.PicturePage == true)
            .GroupBy(pc => pc.PageName)
            .Select(g => g.OrderByDescending(pc => pc.LastShowDate).First())
            .Select(coreCounter => new TblPageCounter
            {
                Id = coreCounter.Id,
                CategoryId = coreCounter.CategoryId,
                PicturePage = coreCounter.PicturePage,
                PageName = coreCounter.PageName ?? string.Empty,
                MonthViewed = coreCounter.MonthViewed ?? string.Empty,
                LastShowDate = coreCounter.LastShowDate,
                PageViews = coreCounter.PageViews
            })
            .OrderByDescending(pc => pc.PageViews)
            .Take(20)
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
}
