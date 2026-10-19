namespace ArvidsonFoto.Core.DTOs;

/// <summary>
/// Data Transfer Object (DTO) for Categories  (DB model: <see cref="TblMenu"/>).
/// </summary>
/// <remarks>
/// This DTO contains properties that represent the metadata of a category, such as ID, name,
/// URL, description, and last updated date.
/// </remarks>
public class CategoryDto()
{
    /// <summary> Kategori ID </summary>
    /// <remarks> Används för att identifiera vilken kategori bilden tillhör, t.ex. 13 för Blåmes. </remarks>
    /// <example>13</example>
    public int? CategoryId { get; set; } = -1; // Default value when not set

    /// <summary>
    /// Namnet på kategorin
    /// </summary>
    public string NameSv { get; set; } = string.Empty;
    public string? NameEn { get; set; } = string.Empty;

    /// <summary> En bild sökväg för den senast fotograferade bilden i denna kategorin </summary>
    /// <remarks> Exempel: "bilder/faglar/masar-trutar-tarnor/trana/AP2D1201" </remarks>
    /// <example>bilder/faglar/masar-trutar-tarnor/trana/AP2D1201</example>
    public string? UrlImage { get; set; } = string.Empty;

    /// <summary> Url för kategori </summary>
    /// <remarks> Exempel: "https://arvidsonfoto.se/bilder/insekter/fjarilar/amiral" </remarks>
    /// <example>https://arvidsonfoto.se/bilder/insekter/fjarilar/amiral</example>
    public string? UrlCategorySv { get; set; } = string.Empty;
    public string? UrlCategoryEn { get; set; } = string.Empty;

    /// <summary> Kategori sökväg </summary>
    /// <remarks> Används för att bygga upp sökvägar i applikationen. 
    /// Exempel: "masar-trutar-tarnor" </remarks>
    /// <example>masar-trutar-tarnor</example>
    public string? UrlCategoryPathSv { get; set; } = string.Empty;
    public string? UrlCategoryPathEn { get; set; } = string.Empty;

    /// <summary> Fullständig kategori sökväg </summary>
    /// <remarks> Används för att bygga upp fullständiga sökvägar i applikationen.
    /// Exempel: "faglar/masar-trutar-tarnor/trana/underkategorin" </remarks>
    /// <example>faglar/masar-trutar-tarnor/trana/underkategorin</example>
    public string? UrlCategoryPathFullSv { get; set; } = string.Empty;
    public string? UrlCategoryPathFullEn { get; set; } = string.Empty;

    /// <summary> Datum och tid när kateorin uppdaterades senast </summary>'
    /// <remarks> Datum och tid i ISO 8601 format, t.ex. "2021-11-23T16:21:00" </remarks>
    /// <example>2021-11-23T16:21:00</example>
    public DateTime? DateUpdated { get; set; }

    /// <summary> Huvudkategori ID (Parent ID) </summary>
    /// <remarks> Används för att gruppera underkategorier under en huvudkategori. Null för huvudkategorier. </remarks>
    /// <example>12</example>
    public int? ParentCategoryId { get; set; }

    public bool IsGameSelectable { get; set; } = true;

    /// <summary> Antal bilder i kategorin </summary>
    /// <remarks> Används för att visa antal bilder i kategorin. </remarks>
    /// <example>25</example>
    public int ImageCount { get; set; }

    /// <summary>
    /// Skapar en tom CategoryDto med alla properties initialiserade
    /// </summary>
    /// <returns>En ny tom <see cref="CategoryDto"/></returns>
    public static CategoryDto CreateEmpty()
    {
        return new CategoryDto
        {
            CategoryId = null,
            NameSv = string.Empty,
            NameEn = string.Empty,
            UrlImage = string.Empty,
            UrlCategorySv = string.Empty,
            UrlCategoryEn = string.Empty,
            UrlCategoryPathSv = string.Empty,
            UrlCategoryPathEn = string.Empty,
            UrlCategoryPathFullSv = string.Empty,
            UrlCategoryPathFullEn = string.Empty,
            DateUpdated = null,
            ParentCategoryId = null,
            ImageCount = 0
        };
    }
}