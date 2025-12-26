using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.Models;
using Serilog;

namespace ArvidsonFoto.Services;

/// <summary>
/// Legacy page counter service - migrated to use Core namespace
/// </summary>
public class PageCounterService : IPageCounterService
{
    private readonly ArvidsonFotoCoreDbContext _entityContext;
    
    public PageCounterService(ArvidsonFotoCoreDbContext context)
    {
        _entityContext = context;
    }

    public void AddPageCount(string pageName)
    {
        string monthViewed = DateTime.Now.ToString("yyyy-MM");
        List<TblPageCounter> tblPageCounters = _entityContext.TblPageCounter
                                                             .Where(p => p.PicturePage == false && p.MonthViewed == monthViewed)
                                                             .ToList();
        try
        {
            bool notExist = true;
            foreach (var item in tblPageCounters)
            {
                if (item.PageName != null && item.PageName.Equals(pageName) && item.MonthViewed != null && item.MonthViewed.Equals(monthViewed))
                {
                    notExist = false;
                    item.PageViews = item.PageViews + 1;
                    item.LastShowDate = DateTime.Now;
                }
            }

            if (notExist)
            {
                TblPageCounter pageCounter = new TblPageCounter()
                {
                    MonthViewed = monthViewed,
                    PageName = pageName,
                    PageViews = 1,
                    CategoryId = 0,
                    PicturePage = false,
                    LastShowDate = DateTime.Now
                };
                _entityContext.TblPageCounter.Add(pageCounter);
            }

            _entityContext.SaveChangesAsync().Wait();
        }
        catch (Exception ex)
        {
            Log.Error("Error while updating PageCounter for the Page: " + pageName + ". Error-message: " + ex.Message);
        }
    }

    public void AddCategoryCount(int categoryId, string pageName)
    {
        string monthViewed = DateTime.Now.ToString("yyyy-MM");
        
        try
        {
            // Check if the record already exists
            var existingCounter = _entityContext.TblPageCounter
                .FirstOrDefault(p => p.PicturePage == true 
                                  && p.CategoryId == categoryId 
                                  && p.MonthViewed == monthViewed);
            
            if (existingCounter != null)
            {
                // Update existing record
                existingCounter.PageViews = existingCounter.PageViews + 1;
                existingCounter.LastShowDate = DateTime.Now;
            }
            else
            {
                // Create new record
                TblPageCounter pageCounter = new TblPageCounter()
                {
                    MonthViewed = monthViewed,
                    PageName = pageName,
                    PageViews = 1,
                    CategoryId = categoryId,
                    PicturePage = true,
                    LastShowDate = DateTime.Now
                };
                _entityContext.TblPageCounter.Add(pageCounter);
            }

            _entityContext.SaveChangesAsync().Wait();
        }
        catch (Exception ex)
        {
            Log.Error("Error while updating PageCounter for the CategoryId: " + categoryId + ". Error-message: " + ex.Message);
        }
    }

    public List<TblPageCounter> GetMonthCount(string yearMonth, bool picturePage)
    {
        return _entityContext.TblPageCounter
                             .Where(p => p.MonthViewed == yearMonth && p.PicturePage == picturePage)
                             .OrderByDescending(p => p.PageViews)
                             .ToList();
    }

    public List<TblPageCounter> GetAllPageCountsGroupedByPageCount()
    {
        List<TblPageCounter> listToReturn = new List<TblPageCounter>();
        var listOfPages = _entityContext.TblPageCounter
                                        .Where(p => p.PageName != null)
                                        .Select(p => p.PageName)
                                        .Distinct()
                                        .ToList();

        for (int i = 0; i < listOfPages.Count; i++)
        {
            var pageName = listOfPages[i];
            TblPageCounter aCountedPage = new TblPageCounter()
            {
                Id = i + 1,
                PageName = pageName,
                PageViews = _entityContext.TblPageCounter.Where(p => p.PageName == pageName).Sum(p => p.PageViews),
                LastShowDate = _entityContext.TblPageCounter.Where(p => p.PageName == pageName).Max(p => p.LastShowDate)
            };

            listToReturn.Add(aCountedPage);
        }

        return listToReturn.OrderByDescending(p => p.PageViews).ToList();
    }

    public List<TblPageCounter> GetTop20CategoryCountsGroupedByPageCount()
    {
        throw new NotImplementedException();
    }

    public Dictionary<string, int> GetMonthlyPageViewsChart(int monthsBack, bool picturePage)
    {
        var result = new Dictionary<string, int>();
        var currentDate = DateTime.Now;

        for (int i = monthsBack - 1; i >= 0; i--)
        {
            var targetDate = currentDate.AddMonths(-i);
            var yearMonth = targetDate.ToString("yyyy-MM");
            
            var monthlyViews = _entityContext.TblPageCounter
                                            .Where(p => p.MonthViewed == yearMonth && p.PicturePage == picturePage)
                                            .Sum(p => p.PageViews);
            
            result[yearMonth] = monthlyViews;
        }

        return result;
    }
}