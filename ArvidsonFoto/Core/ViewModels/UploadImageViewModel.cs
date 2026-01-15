using ArvidsonFoto.Core.DTOs;

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
    public CategoryDto SelectedCategory { get; set; } = CategoryDto.CreateEmpty();

    /// <summary>Lista över tillgängliga underkategorier</summary>
    public List<CategoryDto> SubCategories { get; set; } = [];

    /// <summary>Bilduppladdningsmodell med metadata</summary>
    public UploadImageInputDto ImageInputModel { get; set; } = UploadImageInputDto.CreateEmpty();

    /// <summary>Aktuell URL för sidan</summary>
    public string CurrentUrl { get; set; } = string.Empty;
}
