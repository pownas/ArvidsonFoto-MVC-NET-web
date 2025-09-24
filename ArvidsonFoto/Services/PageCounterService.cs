using ArvidsonFoto.Data;
using ArvidsonFoto.Models;

namespace ArvidsonFoto.Services;

public class PageCounterService : IPageCounterService
    // Returnerar sidvisningar per månad för senaste X månader (default 12)
    public List<(string Month, int PageViews)> GetMonthlyPageViews(int monthsBack = 12)
    {
        var fromDate = DateTime.Now.AddMonths(-monthsBack);
        // MonthViewed format: "yyyy-MM"
        return _entityContext.TblPageCounter
            .Where(p => DateTime.Parse(p.MonthViewed + "-01") >= fromDate)
            .GroupBy(p => p.MonthViewed)
            .Select(g => new { Month = g.Key, PageViews = g.Sum(x => x.PageViews) })
            .OrderBy(g => g.Month)
            .Select(g => (g.Month, g.PageViews))
            .ToList();
    }
{
    private readonly ArvidsonFotoDbContext _entityContext;
    public PageCounterService(ArvidsonFotoDbContext context)
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
                if (item.PageName.Equals(pageName) && item.MonthViewed.Equals(monthViewed))
                {
                    notExist = false;
                    item.PageViews += 1;
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

            _entityContext.SaveChanges();
        }
        catch (Exception ex)
        {
            Log.Error("Error while updating PageCounter for the Page: " + pageName + ". Error-message: " + ex.Message);
        }
    }

    public void AddCategoryCount(int categoryId, string pageName)
    {
        string monthViewed = DateTime.Now.ToString("yyyy-MM");
        List<TblPageCounter> tblPageCounters = _entityContext.TblPageCounter
                                                             .Where(p => p.PicturePage == true && p.MonthViewed == monthViewed)
                                                             .ToList();
        try
        {
            bool notExist = true;
            foreach (var item in tblPageCounters)
            {
                if (item.CategoryId.Equals(categoryId) && item.MonthViewed.Equals(monthViewed))
                {
                    notExist = false;
                    item.PageViews += 1;
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
                    CategoryId = categoryId,
                    PicturePage = true,
                    LastShowDate = DateTime.Now
                };
                _entityContext.TblPageCounter.Add(pageCounter);
            }

            _entityContext.SaveChanges();
        }
        catch (Exception ex)
        {
            Log.Error("Error while updating PageCounter for the CategoryId: " + categoryId + ". Error-message: " + ex.Message);
        }
    }

    public List<TblPageCounter> GetMonthCount(string yearMonth, bool picturePage)
    {
        List<TblPageCounter> listToReturn = new List<TblPageCounter>();
        listToReturn = _entityContext.TblPageCounter
                                     .Where(p => p.MonthViewed.Equals(yearMonth) && p.PicturePage == picturePage)
                                     .OrderByDescending(p => p.PageViews)
                                     .ToList();
        return listToReturn;
    }

    public List<TblPageCounter> GetAllPageCountsGroupedByPageCount()
    {
        List<TblPageCounter> listToReturn = new List<TblPageCounter>();
        var listOfPages = _entityContext.TblPageCounter
                                        //.Where(p => p.PicturePage == false) //För att bara se sidor och inte bild-kategorier...
                                        .Select(p => p.PageName)
                                        .Distinct()
                                        .ToList();

        for (int i = 0; i < listOfPages.Count; i++)
        {
            TblPageCounter aCountedPage = new TblPageCounter()
            {
                Id = i + 1,
                PageName = listOfPages[i],
                PageViews = _entityContext.TblPageCounter.Where(p => p.PageName.Equals(listOfPages[i])).Sum(p => p.PageViews),
                LastShowDate = _entityContext.TblPageCounter.Where(p => p.PageName.Equals(listOfPages[i])).Max(p => p.LastShowDate)
            };

            listToReturn.Add(aCountedPage);
        }

        //Tidigare SQL-frågan som delats upp i PageViews och LastShowDate ovan...
        //var SQLquery = "SELECT SUM(PageCounter_Views) AS PageCounter_Views, PageCounter_Name, MAX(PageCounter_LastShowDate) AS PageCounter_LastShowDate FROM tbl_PageCounter GROUP BY PageCounter_Name ORDER BY PageCounter_Views DESC";
        //var groupedList = _entityContext.TblPageCounter.FromSqlRaw(SQLquery).ToList();

        return listToReturn.OrderByDescending(p => p.PageViews).ToList();
    }

    public List<TblPageCounter> GetTop20CategoryCountsGroupedByPageCount()
    {
        throw new NotImplementedException();
    }
}