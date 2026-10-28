using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.Interfaces;
using ArvidsonFoto.Core.Models;
using System.Globalization;

namespace ArvidsonFoto.Core.Services;

/// <summary>
/// Service for managing page counters and tracking page views.
/// </summary>
/// <remarks>
/// This service implements page view tracking functionality using the Core database context
/// and Core models. It provides methods for adding page counts, retrieving statistics,
/// and generating monthly page view charts.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="PageCounterService"/> class.
/// </remarks>
/// <param name="context">The Core database context</param>
public class PageCounterService(ArvidsonFotoCoreDbContext context) : IPageCounterService
{
    private readonly ArvidsonFotoCoreDbContext _context = context;

    /// <summary>
    /// Räknar upp en sidvisning och sätter datum till att sidan nu besöks.
    /// </summary>
    /// <param name="pageName">Namn på sidan som ska uppdateras</param>
    public void AddPageCount(string pageName)
    {
        string monthViewed = DateTime.Now.ToString("yyyy-MM", CultureInfo.InvariantCulture);

        try
        {
            // Check if the record already exists
            var existingCounter = _context.TblPageCounter
                .FirstOrDefault(p => !p.PicturePage && p.PageName == pageName
                                  && p.MonthViewed == monthViewed);

            if (existingCounter != null)
            {
                // Update existing record
                existingCounter.PageViews += 1;
                existingCounter.LastShowDate = DateTime.Now;
            }
            else
            {
                // Create new record - DO NOT set Id, let database generate it
                TblPageCounter pageCounter = new()
                {
                    MonthViewed = monthViewed,
                    PageName = pageName,
                    PageViews = 1,
                    CategoryId = 0,
                    PicturePage = false,
                    LastShowDate = DateTime.Now
                };
                _ = _context.TblPageCounter.Add(pageCounter);
            }

            _ = _context.SaveChanges();
        }
        catch (Exception ex)
        {
            Log.Error($"Error while updating PageCounter for the Page: {pageName}. Error-message: {ex.Message}");
        }
    }

    /// <summary>
    /// Räknar upp kategorins sidvisare och sätter datum till att sidan nu besöks.
    /// </summary>
    /// <param name="categoryId">Kategori Id som ska uppdateras</param>
    /// <param name="pageName">Namn på kategorin som ska uppdateras</param>
    public void AddCategoryCount(int categoryId, string pageName)
    {
        string monthViewed = DateTime.Now.ToString("yyyy-MM", CultureInfo.InvariantCulture);

        try
        {
            // Check if the record already exists
            var existingCounter = _context.TblPageCounter
                .FirstOrDefault(p => p.PicturePage && p.CategoryId == categoryId
                                  && p.MonthViewed == monthViewed);

            if (existingCounter != null)
            {
                // Update existing record
                existingCounter.PageViews += 1;
                existingCounter.LastShowDate = DateTime.Now;
            }
            else
            {
                // Create new record - DO NOT set Id, let database generate it
                TblPageCounter pageCounter = new()
                {
                    MonthViewed = monthViewed,
                    PageName = pageName,
                    PageViews = 1,
                    CategoryId = categoryId,
                    PicturePage = true,
                    LastShowDate = DateTime.Now
                };
                _ = _context.TblPageCounter.Add(pageCounter);
            }

            _ = _context.SaveChanges();
        }
        catch (Exception ex)
        {
            Log.Error($"Error while updating PageCounter for the CategoryId: {categoryId}. Error-message: {ex.Message}");
        }
    }

    /// <summary>
    /// Hämtar månadens sidvisningar.
    /// </summary>
    /// <param name="yearMonth">Månad i formatet "yyyy-MM", exempel: "2021-02"</param>
    /// <param name="picturePage">true == en bild-kategori, false == en sida</param>
    /// <returns>Lista med sidvisningar för den angivna månaden</returns>
    public List<TblPageCounter> GetMonthCount(string yearMonth, bool picturePage)
    {
        return [.. _context.TblPageCounter
            .Where(p => p.MonthViewed == yearMonth && p.PicturePage == picturePage)
            .OrderByDescending(p => p.PageViews)];
    }

    /// <summary>
    /// Hämtar alla sidvisningar per sida och kategori, grupperade och summerade.
    /// </summary>
    /// <returns>Lista med alla sidvisningar grupperade per sida</returns>
    public List<TblPageCounter> GetAllPageCountsGroupedByPageCount()
    {
        var groupedCounters = _context.TblPageCounter
            .GroupBy(p => p.PageName)
            .Select(g => new TblPageCounter
            {
                Id = g.First().Id,
                PageName = g.Key,
                PageViews = g.Sum(p => p.PageViews),
                LastShowDate = g.Max(p => p.LastShowDate),
                CategoryId = g.First().CategoryId,
                PicturePage = g.First().PicturePage,
                MonthViewed = g.OrderByDescending(p => p.LastShowDate).First().MonthViewed
            })
            .OrderByDescending(p => p.PageViews)
            .ToList();

        return groupedCounters;
    }

    /// <summary>
    /// Hämtar de 20 mest besökta bild-kategorierna.
    /// </summary>
    /// <returns>Lista med de 20 mest besökta bild-kategorierna</returns>
    public List<TblPageCounter> GetTop20CategoryCountsGroupedByPageCount()
    {
        var groupedCounters = _context.TblPageCounter
            .Where(p => p.PicturePage)
            .GroupBy(p => p.CategoryId)
            .Select(g => new TblPageCounter
            {
                Id = g.First().Id,
                CategoryId = g.Key,
                PageName = g.First().PageName,
                PageViews = g.Sum(p => p.PageViews),
                LastShowDate = g.Max(p => p.LastShowDate),
                PicturePage = true,
                MonthViewed = g.OrderByDescending(p => p.LastShowDate).First().MonthViewed
            })
            .OrderByDescending(p => p.PageViews)
            .Take(20)
            .ToList();

        return groupedCounters;
    }

    /// <summary>
    /// Hämtar alla sidvisningar utan gruppering.
    /// </summary>
    /// <returns>Lista med alla sidvisningar</returns>
    public List<TblPageCounter> GetAllPageCounts()
    {
        return [.. _context.TblPageCounter.OrderByDescending(p => p.LastShowDate)];
    }

    /// <summary>
    /// Hämtar sidvisningar för de senaste månaderna grupperat per månad.
    /// </summary>
    /// <param name="monthsBack">Antal månader bakåt att hämta data för</param>
    /// <param name="picturePage">true == en bild-kategori, false == en sida</param>
    /// <returns>Dictionary med månad som nyckel och totala sidvisningar som värde</returns>
    public Dictionary<string, int> GetMonthlyPageViewsChart(int monthsBack, bool picturePage)
    {
        if (monthsBack <= 0)
        {
            return [];
        }

        var endDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var startDate = endDate.AddMonths(-(monthsBack - 1));
        return GetMonthlyPageViewsChart(startDate, endDate, picturePage);
    }

    public Dictionary<string, int> GetMonthlyPageViewsChart(DateTime startDate, DateTime endDate, bool picturePage)
    {
        var monthKeys = GetMonthKeys(startDate, endDate);
        if (monthKeys.Length == 0)
        {
            return [];
        }

        var monthlyTotals = _context.TblPageCounter
            .Where(p => monthKeys.Contains(p.MonthViewed) && p.PicturePage == picturePage)
            .GroupBy(p => p.MonthViewed)
            .Select(group => new { Month = group.Key, Views = group.Sum(p => p.PageViews) })
            .ToDictionary(group => group.Month, group => group.Views);

        return monthKeys.ToDictionary(month => month, month => monthlyTotals.GetValueOrDefault(month));
    }

    public List<TblPageCounter> GetPageCountsInPeriod(DateTime startDate, DateTime endDate, bool picturePage)
    {
        var monthKeys = GetMonthKeys(startDate, endDate);
        if (monthKeys.Length == 0)
        {
            return [];
        }

        var counters = _context.TblPageCounter
            .Where(p => monthKeys.Contains(p.MonthViewed) && p.PicturePage == picturePage)
            .ToList();
        return counters
            .GroupBy(p => picturePage ? p.CategoryId.ToString(CultureInfo.InvariantCulture) : p.PageName)
            .Select(group =>
            {
                var latest = group.OrderByDescending(p => p.LastShowDate).First();
                return new TblPageCounter
                {
                    Id = latest.Id,
                    PageName = latest.PageName,
                    CategoryId = latest.CategoryId,
                    PicturePage = picturePage,
                    PageViews = group.Sum(p => p.PageViews),
                    LastShowDate = group.Max(p => p.LastShowDate),
                    MonthViewed = latest.MonthViewed
                };
            })
            .OrderByDescending(counter => counter.PageViews)
            .ToList();
    }

    private static string[] GetMonthKeys(DateTime startDate, DateTime endDate)
    {
        var firstMonth = new DateTime(startDate.Year, startDate.Month, 1);
        var lastMonth = new DateTime(endDate.Year, endDate.Month, 1);
        if (lastMonth < firstMonth)
        {
            return [];
        }

        return Enumerable.Range(0, ((lastMonth.Year - firstMonth.Year) * 12) + lastMonth.Month - firstMonth.Month + 1)
            .Select(offset => firstMonth.AddMonths(offset).ToString("yyyy-MM", CultureInfo.InvariantCulture))
            .ToArray();
    }
}