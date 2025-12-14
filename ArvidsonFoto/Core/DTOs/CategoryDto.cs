#nullable enable
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

    /// <summary> Namn på kategori </summary>
    /// <remarks> Exempel: "Amiral" </remarks>
    /// <example>Amiral</example>
    public string? Name { get; set; } = string.Empty;

    /// <summary> En bild sökväg för den senast fotograferade bilden i denna kategorin </summary>
    /// <remarks> Exempel: "bilder/faglar/masar-trutar-tarnor/trana/AP2D1201" </remarks>
    /// <example>bilder/faglar/masar-trutar-tarnor/trana/AP2D1201</example>
    public string? UrlImage { get; set; } = string.Empty;

    /// <summary> Url för kategori </summary>
    /// <remarks> Exempel: "https://arvidsonfoto.se/bilder/insekter/fjarilar/amiral" </remarks>
    /// <example>https://arvidsonfoto.se/bilder/insekter/fjarilar/amiral</example>
    public string? UrlCategory { get; set; } = string.Empty;

    /// <summary> Kategori sökväg </summary>
    /// <remarks> Används för att bygga upp sökvägar i applikationen. 
    /// Exempel: "masar-trutar-tarnor" </remarks>
    /// <example>masar-trutar-tarnor</example>
    public string? UrlCategoryPath { get; set; } = string.Empty;

    /// <summary> Fullständig kategori sökväg </summary>
    /// <remarks> Används för att bygga upp fullständiga sökvägar i applikationen.
    /// Exempel: "faglar/masar-trutar-tarnor/trana/underkategorin" </remarks>
    /// <example>faglar/masar-trutar-tarnor/trana/underkategorin</example>
    public string? UrlCategoryPathFull { get; set; } = string.Empty;

    /// <summary> Datum och tid när kateorin uppdaterades senast </summary>'
    /// <remarks> Datum och tid i ISO 8601 format, t.ex. "2021-11-23T16:21:00" </remarks>
    /// <example>2021-11-23T16:21:00</example>
    public DateTime? DateUpdated { get; set; } = null;

    ///// <summary> Kategori beskrivning </summary>
    ///// <remarks> Beskrivning av kategorin, kan vara tom sträng. </remarks>
    ///// <example>Djupare beskrivning av amiral...</example>
    //public string? Description { get; set; } = string.Empty;

    /// <summary> Huvudkategori ID (Parent ID) </summary>
    /// <remarks> Används för att gruppera underkategorier under en huvudkategori. Null för huvudkategorier. </remarks>
    /// <example>12</example>
    public int? ParentCategoryId { get; set; } = null;

    /// <summary> Antal bilder i kategorin </summary>
    /// <remarks> Används för att visa antal bilder i kategorin. </remarks>
    /// <example>25</example>
    public int ImageCount { get; set; } = 0;
}
