namespace ArvidsonFoto.Core.Models;

/// <summary>
/// Represents a news article in the database.
/// </summary>
/// <remarks>
/// Contains all the information needed to store and display news articles
/// including content, metadata, and publishing information.
/// </remarks>
public partial class TblNews()
{
    /// <summary>Internt databas-ID för nyhetsartikel, används som primärnyckel</summary>
    public int Id { get; set; } = -1;
    
    /// <summary>Nyhetsartikel ID, används som unik identifierare</summary>
    public int NewsId { get; set; } = -1;

    /// <summary>Titel på nyhetsartikeln</summary>
    public string NewsTitle { get; set; } = string.Empty;
    
    /// <summary>Innehåll i nyhetsartikeln (HTML-format)</summary>
    public string NewsContent { get; set; } = string.Empty;

    /// <summary>Författare av nyhetsartikeln</summary>
    public string NewsAuthor { get; set; } = string.Empty;

    /// <summary>Datum när artikeln skapades</summary>
    public DateTime? NewsCreated { get; set; } = DateTime.Now;
    
    /// <summary>Datum när artikeln uppdaterades senast</summary>
    public DateTime? NewsUpdated { get; set; } = DateTime.Now;
    
    /// <summary>Indikerar om artikeln är publicerad och synlig</summary>
    public bool NewsPublished { get; set; } = false;

    /// <summary>Kort sammanfattning/ingress för artikeln</summary>
    public string NewsSummary { get; set; } = string.Empty;
}