namespace ArvidsonFoto.Models;

/// <summary>
/// Sidvisningsräknare (Tabellen PageCounter databas record)
/// </summary>
/// <remarks>
/// för att hålla koll på hur många gånger en sida har visats.
/// </remarks>
public class TblPageCounter()
{
    public int Id { get; set; }

    /// <summary> Antal sidvisningar </summary>
    public int PageViews { get; set; }

    /// <summary> Sidans namn </summary>
    public string PageName { get; set; }

    /// <summary> Categori Id som uppdateras </summary>
    public int CategoryId { get; set; }

    /// <summary> Kollar om det är från /Bilder/ eller inte </summary>
    public bool PicturePage { get; set; }

    /// <summary> Innehåller månaden som sidan visades, exempel: "2021-03" </summary>
    public string MonthViewed { get; set; }

    /// <summary> Datum när sidan senast visades </summary>
    public DateTime LastShowDate { get; set; }
}