namespace ArvidsonFoto.Core.ViewModels;

/// <summary>
/// ViewModel for uploading images functionality.
/// </summary>
/// <remarks>
/// Contains category selection, subcategory management, and image input model
/// for the image upload process.
/// </remarks>
public class UploadImageViewModel
{
    /// <summary>Vald kategori för bilduppladdning</summary>
    public TblMenu SelectedCategory { get; set; } = new TblMenu();

    /// <summary>Lista över tillgängliga underkategorier</summary>
    public List<TblMenu> SubCategories { get; set; } = new List<TblMenu>();

    /// <summary>Bilduppladdningsmodell med metadata</summary>
    public UploadImageInputDto ImageInputModel { get; set; } = new UploadImageInputDto
    {
        ImageHuvudfamiljNamn = "",
        ImageFamiljNamn = "",
        ImageArtNamn = "",
        ImageUrl = "",
        ImageUrlFullSrc = "",
        ImageDescription = ""
    };

    /// <summary>Aktuell URL för sidan</summary>
    public string CurrentUrl { get; set; } = string.Empty;
}