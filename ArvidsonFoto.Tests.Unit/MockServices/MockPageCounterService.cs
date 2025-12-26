using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.Models;
using ArvidsonFoto.Services;

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
                .Sum(pc => pc.PageViews); // PageViews is int, not int?
            
            result[yearMonth] = views;
        }
        
        return result;
    }
}
