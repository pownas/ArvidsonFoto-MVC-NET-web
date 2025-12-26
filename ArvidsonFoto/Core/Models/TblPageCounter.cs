namespace ArvidsonFoto.Core.Models;

/// <summary>
/// Represents a page counter used to track page views and related metadata.
/// </summary>
/// <remarks>This class is typically used to store and manage information about page views, including the number
/// of views,  the page name, associated category, and other metadata such as whether the page contains pictures or the
/// last  time it was viewed. It is designed to be used in scenarios where tracking page activity is required.</remarks>
public class TblPageCounter()
{
    /// <summary> Internt Databas-ID för sid-räknaren, används som primärnyckel. </summary>
    /// <remarks>This is an IDENTITY column - do not set manually, let database generate it</remarks>
    public int Id { get; set; }

    /// <summary> Antal sidvisningar </summary>
    public int PageViews { get; set; } = 0;

    /// <summary> Sidans namn </summary>
    public string PageName { get; set; } = string.Empty;

    /// <summary> Categori Id som uppdateras </summary>
    public int CategoryId { get; set; } = 0;

    /// <summary> Kollar om det är från /Bilder/ eller inte </summary>
    public bool PicturePage { get; set; }

    /// <summary> Innehåller månaden som sidan visades, exempel: "2021-03" </summary>
    public string MonthViewed { get; set; } = string.Empty;

    /// <summary> Datum när sidan senast visades </summary>
    public DateTime LastShowDate { get; set; }
}
