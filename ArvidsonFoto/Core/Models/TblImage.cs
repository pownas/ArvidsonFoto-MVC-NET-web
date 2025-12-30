using System.Text.Json.Serialization;

namespace ArvidsonFoto.Core.Models;

/// <summary> Modell för en bild i databasen </summary>
public partial class TblImage()
{
    /// <summary> Internt Databas-ID för en bild, används som primärnyckel. INTE att förväxla med ImageId som är en unik identifierare för varje bild. </summary>
    [JsonIgnore]
    public int? Id { get; set; } = null;

    /// <summary> Bildens Id, används som unik identifierare (ID) i koden. </summary>
    /// <example>2</example>
    public int? ImageId { get; set; } = null;

    /// <summary> Bildens huvudfamilje-namn / kategori id </summary>
    /// <remarks> Exempel: 10 = Tättingar (som tillhör kategori id: 1 - Fåglar)</remarks>
    /// <example>10</example>
    public int? ImageMainFamilyId { get; set; } = null;

    /// <summary> Bildens familje-namn / kategori id </summary>
    /// <remarks> Exempel: 12 = Mesar </remarks>
    /// <example>12</example>
    public int? ImageFamilyId { get; set; } = null;

    /// <summary> Bildens art-namn / kategori id </summary>
    /// <remarks> Exempel: 13 = Blåmes </remarks>
    /// <example>13</example>
    //[JsonPropertyName("image_art")] // ColumnName in database
    public int? ImageCategoryId { get; set; } = null;

    /// <summary> Bildens filnamn till url:en </summary>
    /// <remarks> Exempel: B57W4725 </remarks>
    /// <example>B57W4725</example>
    public string? ImageUrlName { get; set; } = string.Empty;

    /// <summary> Datum och tid när bilden togs </summary>
    /// <example>2021-11-22T16:21:00</example>
    public DateTime? ImageDate { get; set; } = DateTime.MinValue;

    /// <summary> Bildens beskriving </summary>
    /// <remarks> Beskrivning av bilden, kan vara tom sträng. </remarks>
    /// <example>Hane, beskrivning av blåmes...</example>
    public string? ImageDescription { get; set; } = string.Empty;
    
    /// <summary> Bildens beskrivning (engelska) </summary>
    /// <remarks> Engelsk beskrivning av bilden, kan vara tom sträng. </remarks>
    /// <example>Male, description of blue tit...</example>
    public string? ImageDescriptionEn { get; set; } = string.Empty;

    /// <summary> Datum och tid när bilden laddades upp/uppdaterades senast </summary>
    /// <example>2021-12-03T19:23:42</example>
    public DateTime? ImageUpdate { get; set; } = DateTime.Now;

    /// <summary> Inte något fält i databasen, men kan sättas ett namn, om det är en kategori som sökts fram. </summary>
    [NotMapped]
    public string? Name { get; set; } = string.Empty;
}
