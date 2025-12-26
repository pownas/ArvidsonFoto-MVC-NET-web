using ArvidsonFoto.Core.Models;

namespace ArvidsonFoto.Core.Interfaces;

/// <summary>
/// Interface for page counter services providing page view tracking functionality.
/// </summary>
/// <remarks>
/// This interface defines methods for tracking page views and category visits,
/// including monthly statistics and most visited page analytics.
/// </remarks>
public interface IPageCounterService
{
    /// <summary> Räknar upp en sidvisning och sätter datum till att sidan nu besöks. </summary>
    /// <param name="pageName">Namn på sidan som ska uppdateras.</param>
    void AddPageCount(string pageName);

    /// <summary> Räknar upp kategorins sidvisare och sätter datum till att sidan nu besöks. </summary>
    /// <param name="categoryId">Kategori Id som ska uppdateras.</param>
    /// <param name="pageName">Namn på kategorin som ska uppdateras.</param>
    void AddCategoryCount(int categoryId, string pageName);

    /// <summary> En funktion som hämtar månadens sidvisningar. Exempel på input: "2021-02" </summary>
    /// <param name="yearMonth">Tar en DateTime likt "2021-02", exempel: DateTime.Now.ToString("yyyy-MM")</param>
    /// <param name="picturePage">true == en bild-kategori , false == en sida</param>
    /// <returns>Lista med sidvisningar för den angivna månaden</returns>
    List<TblPageCounter> GetMonthCount(string yearMonth, bool picturePage);

    /// <summary> Hämtar alla Sidvisningar per sida och Kategori, grupperade och summerade. </summary>
    /// <returns>Lista med alla sidvisningar grupperade per sida</returns>
    List<TblPageCounter> GetAllPageCountsGroupedByPageCount();

    /// <summary> Hämtar de 20st mest besökta bild-Kategorierna. </summary>
    /// <returns>Lista med de 20 mest besökta bild-kategorierna</returns>
    List<TblPageCounter> GetTop20CategoryCountsGroupedByPageCount();
    
    /// <summary> Hämtar alla sidvisningar utan gruppering. </summary>
    /// <returns>Lista med alla sidvisningar</returns>
    List<TblPageCounter> GetAllPageCounts();

    /// <summary> Hämtar sidvisningar för de senaste månaderna grupperat per månad. </summary>
    /// <param name="monthsBack">Antal månader bakåt att hämta data för</param>
    /// <param name="picturePage">true == en bild-kategori , false == en sida</param>
    /// <returns>Dictionary med månad som nyckel och totala sidvisningar som värde</returns>
    Dictionary<string, int> GetMonthlyPageViewsChart(int monthsBack, bool picturePage);
}