using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.Interfaces;
using ArvidsonFoto.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ArvidsonFoto.Core.Services;

/// <summary>
/// Service for managing page counters and tracking page views.
/// </summary>
/// <remarks>
/// This service implements page view tracking functionality using the Core database context
/// and Core models. It provides methods for adding page counts, retrieving statistics,
/// and generating monthly page view charts.
/// </remarks>
public class ApiPageCounterService : IApiPageCounterService
{
    private readonly ArvidsonFotoCoreDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiPageCounterService"/> class.
    /// </summary>
    /// <param name="context">The Core database context</param>
    public ApiPageCounterService(ArvidsonFotoCoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Räknar upp en sidvisning och sätter datum till att sidan nu besöks.
    /// </summary>
    /// <param name="pageName">Namn på sidan som ska uppdateras</param>
    public void AddPageCount(string pageName)
    {
        string monthViewed = DateTime.Now.ToString("yyyy-MM");
        
        var pageCounter = _context.TblPageCounter
            .FirstOrDefault(p => p.PicturePage == false 
                              && p.MonthViewed == monthViewed 
                              && p.PageName == pageName);

        try
        {
            if (pageCounter != null)
            {
                // Update existing counter
                pageCounter.PageViews += 1;
                pageCounter.LastShowDate = DateTime.Now;
            }
            else
            {
                // Create new counter
                var newCounter = new TblPageCounter
                {
                    MonthViewed = monthViewed,
                    PageName = pageName,
                    PageViews = 1,
                    CategoryId = 0,
                    PicturePage = false,
                    LastShowDate = DateTime.Now
                };
                _context.TblPageCounter.Add(newCounter);
            }

            _context.SaveChanges();
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
        string monthViewed = DateTime.Now.ToString("yyyy-MM");

        var pageCounter = _context.TblPageCounter
            .FirstOrDefault(p => p.PicturePage == true 
                              && p.MonthViewed == monthViewed 
                              && p.CategoryId == categoryId);

        try
        {
            if (pageCounter != null)
            {
                // Update existing counter
                pageCounter.PageViews += 1;
                pageCounter.LastShowDate = DateTime.Now;
            }
            else
            {
                // Create new counter
                var newCounter = new TblPageCounter
                {
                    MonthViewed = monthViewed,
                    PageName = pageName,
                    PageViews = 1,
                    CategoryId = categoryId,
                    PicturePage = true,
                    LastShowDate = DateTime.Now
                };
                _context.TblPageCounter.Add(newCounter);
            }

            _context.SaveChanges();
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
        return _context.TblPageCounter
            .Where(p => p.MonthViewed == yearMonth && p.PicturePage == picturePage)
            .OrderByDescending(p => p.PageViews)
            .ToList();
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
            .Where(p => p.PicturePage == true)
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
        return _context.TblPageCounter
            .OrderByDescending(p => p.LastShowDate)
            .ToList();
    }

    /// <summary>
    /// Hämtar sidvisningar för de senaste månaderna grupperat per månad.
    /// </summary>
    /// <param name="monthsBack">Antal månader bakåt att hämta data för</param>
    /// <param name="picturePage">true == en bild-kategori, false == en sida</param>
    /// <returns>Dictionary med månad som nyckel och totala sidvisningar som värde</returns>
    public Dictionary<string, int> GetMonthlyPageViewsChart(int monthsBack, bool picturePage)
    {
        var result = new Dictionary<string, int>();
        var currentDate = DateTime.Now;

        for (int i = monthsBack - 1; i >= 0; i--)
        {
            var targetDate = currentDate.AddMonths(-i);
            var yearMonth = targetDate.ToString("yyyy-MM");

            var monthlyViews = _context.TblPageCounter
                .Where(p => p.MonthViewed == yearMonth && p.PicturePage == picturePage)
                .Sum(p => p.PageViews);

            result[yearMonth] = monthlyViews;
        }

        return result;
    }
}
