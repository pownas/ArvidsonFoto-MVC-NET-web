using ArvidsonFoto.Core.DTOs;
using ArvidsonFoto.Core.Models;

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
    public TblMenu SelectedCategory { get; set; } = new();

    /// <summary>Lista över tillgängliga underkategorier</summary>
    public List<TblMenu> SubCategories { get; set; } = [];

    /// <summary>Bilduppladdningsmodell med metadata</summary>
    public UploadImageInputDto ImageInputModel { get; set; } = new()
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
