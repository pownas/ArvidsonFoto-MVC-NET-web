namespace ArvidsonFoto.Core.Models;

/// <summary>
/// Represents a guestbook entry in the database.
/// </summary>
/// <remarks>
/// Contains all the information needed to store and display guestbook entries
/// including visitor information, content, and metadata.
/// </remarks>
public partial class TblGb()
{
    /// <summary>Internt databas-ID för gästboksinlägg, används som primärnyckel</summary>
    public int Id { get; set; } = -1;
    
    /// <summary>Gästboksinlägg ID, används som unik identifierare</summary>
    public int GbId { get; set; } = -1;

    /// <summary>Namn på personen som skrev inlägget</summary>
    public string GbName { get; set; } = string.Empty;
    
    /// <summary>Namn på personen som skrev inlägget (engelska)</summary>
    public string GbNameEn { get; set; } = string.Empty;
    
    /// <summary>E-postadress för personen som skrev inlägget</summary>
    public string GbEmail { get; set; } = string.Empty;
    
    /// <summary>Hemsida för personen som skrev inlägget</summary>
    public string GbHomepage { get; set; } = string.Empty;
    
    /// <summary>Innehåll i gästboksinlägget</summary>
    public string GbText { get; set; } = string.Empty;

    /// <summary>Innehåll i gästboksinlägget (engelska)</summary>
    public string GbTextEn { get; set; } = string.Empty;

    /// <summary>Datum när inlägget skapades</summary>
    public DateTime? GbDate { get; set; } = DateTime.Now;
    
    /// <summary>Indikerar om inlägget har lästs/hanterats</summary>
    public bool? GbReadPost { get; set; }
    
    /// <summary>IP-adress för personen som skrev inlägget</summary>
    public string GbIp { get; set; } = string.Empty;
}
