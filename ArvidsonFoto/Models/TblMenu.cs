#nullable disable
namespace ArvidsonFoto.Models;

/// <summary>
/// Tabellen Menu (databas record)
/// </summary>
public partial class TblMenu()
{
    /// <summary>
    /// Tabell id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Meny id - Kategori id (ex. 2 = Duvor)
    /// </summary>
    public int MenuId { get; set; }

    /// <summary>
    /// Huvudmeny id (ex. 1 = Fåglar)
    /// </summary>
    /// <remarks>
    /// Vad tillhör kategorin i toppmenyn?
    /// </remarks>
    public int? MenuMainId { get; set; }

    /// <summary>
    /// Meny text (namn på kategorin, ex. "Duvor")
    /// </summary>
    /// <remarks>
    /// Här kan ÅÄÖ användas. T.ex. "Trädkrypare"
    /// </remarks>
    public string MenuText { get; set; }

    /// <summary>
    /// Meny text (namn på kategorin, ex. "Doves") på engelska
    /// </summary>
    public string MenuTextEn { get; set; }

    /// <summary>
    /// Url text (namn på kategorin när den anropas via en Url, använd INTE ÅÄÖ, ex. "Duvor")
    /// </summary>
    /// <remarks>
    /// Här ska INTE ÅÄÖ användas. T.ex. "Tradkrypare"
    /// </remarks>
    public string MenuUrltext { get; set; }

    /// <summary>
    /// Url text (namn på kategorin när den anropas via en Url) på engelska
    /// </summary>
    public string MenuUrltextEn { get; set; }
    //public string MenuEngtext { get; set; }
}