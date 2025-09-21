using ArvidsonFoto.Core.DTOs;

namespace ArvidsonFoto.Core.ApiResponses;

/// <summary> Response för en lista av bilder inom en efterfrågad kategori. </summary>
public class ImageListResponse
{
    /// <summary> Kategori Id </summary>
    public int CategoryId { get; set; } = -1;

    /// <summary> Kategori Namn </summary>
    public string CategoryName { get; set; }

    /// <summary> Kategori URL med gamla formatet som innehåller åäö </summary>
    public string CategoryUrlWithAAO { get; set; }

    /// <summary> Kategori Url </summary>
    public string CategoryUrl { get; set; }

    /// <summary> Datum för senaste fotograferade bilden i denna kategori. </summary>
    public DateTime? DateLastPhotographedImage { get; set; }

    /// <summary> Datum för senaste uppladdade bilden i denna kategori. </summary>
    public DateTime? DateLastUploadedImage { get; set; }

    /// <summary> Antal bilder i denna kategori </summary>
    public int ImageCategoryTotalCount { get; set; } = -1;

    /// <summary> Totalt antal bilder i denna kategori. </summary>
    public int ImageResultCount { get; set; }

    /// <summary> Lista av bilder i denna kategori. </summary>
    public List<ImageDto> Images { get; set; }


    /// <summary> Querystring: Filter för sortering av resultatet </summary>
    /// <remarks> Exempel: "uploaded" (datum när bilden laddades upp), "imagetaken" (datum när bilden togs) or "categoryname" (namn för kategorin) </remarks>
    public string QuerySortBy { get; set; }

    /// <summary> Querystring: Filter för sorteringsordning av resultatet (asc, desc) </summary>
    /// <remarks> asc (alphabetically ascending order) or desc (reverse alphabetically descending order) </remarks>
    public string QuerySortOrder { get; set; }

    /// <summary> Querystring: Filter för att begränsa antal resultat via limit (0 = obegränsat resultat, 48 är standard) </summary>
    public int QueryLimit { get; set; }
}
