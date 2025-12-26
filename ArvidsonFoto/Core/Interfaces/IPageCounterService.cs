using ArvidsonFoto.Core.Models;

namespace ArvidsonFoto.Core.Interfaces;

/// <summary>
/// Legacy interface for page counter services - migrated to use Core namespace
/// </summary>
public interface IPageCounterService
{
    /// <summary> Räknar upp en sidvisning och sätter datum till att sidan nu besöks. </summary>
    /// <param name="pageName">Namn på kategorin som ska uppdateras.</param>
    void AddPageCount(string pageName);

    /// <summary> Räknar upp kategorins sidvisare och sätter datum till att sidan nu besöks. </summary>
    /// <param name="categoryId">Kategori Id som ska uppdateras.</param>
    /// <param name="pageName">Namn på kategorin som ska uppdateras.</param>
    void AddCategoryCount(int categoryId, string pageName);

    /// <summary> En funktion som hämtar månadens sidvisningar. Exempel på input: "2021-02" </summary>
    /// <param name="yearMonth">Tar en DateTime likt "2021-02", exempel: DateTime.Now.ToString("yyyy-MM")</param>
    /// <param name="picturePage">true == en bild-kategori , false == en sida</param>
    /// <returns></returns>
    List<TblPageCounter> GetMonthCount(string yearMonth, bool picturePage);

    /// <summary> Hämtar alla Sidvisningar per sida och Kategori... </summary>
    /// <returns></returns>
    List<TblPageCounter> GetAllPageCountsGroupedByPageCount();

    /// <summary> Hämtar de 20st mest besökta bild-Kategorierna. </summary>
    /// <returns></returns>
    List<TblPageCounter> GetTop20CategoryCountsGroupedByPageCount();

    /// <summary> Hämtar sidvisningar för de senaste månaderna grupperat per månad. </summary>
    /// <param name="monthsBack">Antal månader bakåt att hämta data för</param>
    /// <param name="picturePage">true == en bild-kategori , false == en sida</param>
    /// <returns>Dictionary med månad som nyckel och totala sidvisningar som värde</returns>
    Dictionary<string, int> GetMonthlyPageViewsChart(int monthsBack, bool picturePage);
}