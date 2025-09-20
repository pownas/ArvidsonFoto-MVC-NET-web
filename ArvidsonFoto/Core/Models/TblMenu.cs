namespace ArvidsonFoto.Core.Models;

/// <summary>
/// Modell för en meny kategori i databasen
/// </summary>
public partial class TblMenu()
{
    /// <summary> Internt Databas-ID för en en meny kategori, används som primärnyckel. INTE att förväxla med MenuId som är en unik identifierare för varje meny kategori. </summary>
    /// <example>13</example>
    //[JsonPropertyName("Id")] // ColumnName in database
    public int? Id { get; set; }

    /// <summary> Meny kategorins Id, används som unik identifierare (ID) i koden. </summary>
    /// <example>13</example>
    //[JsonPropertyName("menu_ID")] // ColumnName in database
    public int? MenuCategoryId { get; set; }

    /// <summary> Meny kategorins huvudkategori Id (ParentId), används för att gruppera underkategorier. </summary>
    /// <example>12</example>
    //[JsonPropertyName("menu_mainID")] // ColumnName in database
    public int? MenuParentCategoryId { get; set; }

    /// <summary> Svenskt namn för menyn, används för att visa i UI. </summary>
    /// <remarks> Exempel: "Blåmes" </remarks>
    /// <example>Blåmes</example>
    //[JsonPropertyName("menu_text")] // ColumnName in database
    public string? MenuDisplayName { get; set; } = string.Empty;
    
    //public string MenuEngtext { get; set; }

    /// <summary> URL text för menyn, används för att skapa sökvänliga URL:er. </summary>
    /// <remarks> Kallas även för "Slug" eller "UrlSlug", när det är enbart ett segment av en URL. Exempel: "blames" </remarks>
    /// <example>blames</example>
    //[JsonPropertyName("menu_URLtext")] // ColumnName in database
    public string? MenuUrlSegment { get; set; } = string.Empty;

    /// <summary> Datumen då menyn uppdaterades senast. </summary>
    //[JsonPropertyName("menu_dateUpdated")] // ColumnName in database
    public DateTime? MenuDateUpdated { get; set; } = DateTime.UtcNow;
}