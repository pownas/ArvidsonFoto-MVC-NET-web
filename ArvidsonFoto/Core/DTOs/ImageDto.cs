namespace ArvidsonFoto.Core.DTOs;

/// <summary>
/// Data Transfer Object (DTO) for images (DB model: <see cref="TblImage"/>).
/// </summary>
/// <remarks>
/// It contains properties that represent the image's metadata, such as name, URL, date taken, and description.
/// </remarks>
public class ImageDto()
{
    /// <summary> Namn på kategori </summary>
    /// <remarks> Exempel: "Amiral" </remarks>
    /// <example>Amiral</example>
    public string Name { get; set; } = string.Empty;

    /// <summary> Kategorinamn för bilden </summary>
    /// <remarks> Namnet på kategorin som bilden tillhör. Exempel: "Fasan", "Blommor", "Hackspettar" </remarks>
    /// <example>Fasan</example>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary> Url för kategori </summary>
    /// <remarks> Exempel: "https://arvidsonfoto.se/bilder/insekter/fjarilar/amiral" </remarks>
    /// <example>https://arvidsonfoto.se/bilder/insekter/fjarilar/amiral</example>
    public string UrlCategory { get; set; } = string.Empty;

    /// <summary> Bildens filnamn / sökväg </summary>
    /// <remarks> Exempel: "bilder/landskap/gotland/AP2D1201" </remarks>
    /// <example>bilder/landskap/gotland/AP2D1201</example>
    public string UrlImage { get; set; } = string.Empty;

    /// <summary> Datum och tid när bilden togs </summary>'
    /// <remarks> Datum och tid i ISO 8601 format, t.ex. "2021-11-22T16:21:00" </remarks>
    /// <example>2021-11-22T16:21:00</example>
    public DateTime? DateImageTaken { get; set; } = null;

    /// <summary> Datum och tid när bilden laddades upp </summary>'
    /// <remarks> Datum och tid i ISO 8601 format, t.ex. "2021-11-23T16:21:00" </remarks>
    /// <example>2021-11-23T16:21:00</example>
    public DateTime? DateUploaded { get; set; } = null;

    /// <summary> Bildens beskrivning </summary>
    /// <remarks> Beskrivning av bilden, kan vara tom sträng. </remarks>
    /// <example>Hane, beskrivning av amiral...</example>
    public string? Description { get; set; } = string.Empty;

    /// <summary> Bildens unika ID </summary>
    /// <remarks> Används för att identifiera bilden i databasen, t.ex. 8014. </remarks>
    /// <example>8014</example>
    public int ImageId { get; set; } = -1; // Default value when not set

    /// <summary> Bildens kategori ID </summary>
    /// <remarks> Används för att identifiera vilken kategori bilden tillhör, t.ex. 13 för Blåmes. </remarks>
    /// <example>13</example>
    public int CategoryId { get; set; } = -1; // Default value when not set

    /// <summary>
    /// Skapar en tom ImageDto med alla properties initialiserade
    /// </summary>
    /// <returns>En ny tom ImageDto</returns>
    public static ImageDto CreateEmpty()
    {
        return new ImageDto
        {
            Name = string.Empty,
            CategoryName = string.Empty,
            UrlCategory = string.Empty,
            UrlImage = string.Empty,
            DateImageTaken = null,
            DateUploaded = null,
            Description = string.Empty,
            ImageId = -1,
            CategoryId = -1
        };
    }
}
