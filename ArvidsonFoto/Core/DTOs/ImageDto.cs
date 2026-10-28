namespace ArvidsonFoto.Core.DTOs;

/// <summary>
/// Data Transfer Object (DTO) for images (DB model: <see cref="TblImage"/>).
/// </summary>
/// <remarks>
/// It contains properties that represent the image's metadata, such as name, URL, date taken, and description.
/// </remarks>
public class ImageDto()
{
    /// <summary> Kategorinamn för bilden på svenska </summary>
    /// <remarks> Namnet på kategorin som bilden tillhör. Exempel: "Fasan", "Blommor", "Hackspettar" </remarks>
    /// <example>Fasan</example>
    public string CategoryNameSv { get; set; } = string.Empty;

    public string? CategoryNameEn { get; set; } = string.Empty;

    /// <summary> Url för kategori </summary>
    /// <remarks> Exempel: "https://arvidsonfoto.se/bilder/insekter/fjarilar/amiral" </remarks>
    /// <example>https://arvidsonfoto.se/bilder/insekter/fjarilar/amiral</example>
    public string UrlCategorySv { get; set; } = string.Empty;

    public string UrlCategoryEn { get; set; } = string.Empty;

    /// <summary> Bildens filnamn / sökväg </summary>
    /// <remarks> Exempel: "bilder/landskap/gotland/AP2D1201" </remarks>
    /// <example>bilder/landskap/gotland/AP2D1201</example>
    public string UrlImage { get; set; } = string.Empty;

    /// <summary> Datum och tid när bilden togs </summary>'
    /// <remarks> Datum och tid i ISO 8601 format, t.ex. "2021-11-22T16:21:00" </remarks>
    /// <example>2021-11-22T16:21:00</example>
    public DateTime? DateImageTaken { get; set; }

    /// <summary> Datum och tid när bilden laddades upp </summary>'
    /// <remarks> Datum och tid i ISO 8601 format, t.ex. "2021-11-23T16:21:00" </remarks>
    /// <example>2021-11-23T16:21:00</example>
    public DateTime? DateUploaded { get; set; } = null;

    public string? DescriptionSv { get; set; } = string.Empty;
    public string? DescriptionEn { get; set; } = string.Empty;

    /// <summary> Bildens unika ID </summary>
    /// <remarks> Används för att identifiera bilden i databasen, t.ex. 8014. </remarks>
    /// <example>8014</example>
    public int ImageId { get; set; } = -1; // Default value when not set

    /// <summary> Bildens kategori ID </summary>
    /// <remarks> Används för att identifiera vilken kategori bilden tillhör, t.ex. 13 för Blåmes. </remarks>
    /// <example>13</example>
    public int CategoryId { get; set; } = -1; // Default value when not set

    public bool IsGameSelectable { get; set; } = true;
    public bool IsCategoryGameSelectable { get; set; }

    /// <summary>
    /// Skapar en tom ImageDto med alla properties initialiserade
    /// </summary>
    /// <returns>En ny tom ImageDto</returns>
    public static ImageDto CreateEmpty()
    {
        return new ImageDto
        {
            CategoryNameSv = string.Empty,
            CategoryNameEn = string.Empty,
            UrlCategorySv = string.Empty,
            UrlCategoryEn = string.Empty,
            UrlImage = string.Empty,
            DateImageTaken = null,
            DateUploaded = null,
            DescriptionSv = string.Empty,
            DescriptionEn = string.Empty,
            ImageId = -1,
            CategoryId = -1
        };
    }
}