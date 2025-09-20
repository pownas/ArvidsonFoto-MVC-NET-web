namespace ArvidsonFoto.Core.ViewModels;

/// <summary>
/// ViewModel for gallery display functionality.
/// </summary>
/// <remarks>
/// Contains all the necessary data for displaying image galleries including pagination,
/// category information, and image collections.
/// </remarks>
public class GalleryViewModel
{
    /// <summary>Lista med bilder som ska visas på nuvarande sida</summary>
    public List<ImageDto> DisplayImagesList { get; set; } = new List<ImageDto>();

    /// <summary>Fullständig lista med alla bilder i kategorin</summary>
    public List<ImageDto> AllImagesList { get; set; } = new List<ImageDto>();

    /// <summary>Vald kategori för galleriet</summary>
    public CategoryDto SelectedCategory { get; set; } = new CategoryDto();

    /// <summary>Totalt antal sidor i galleriet</summary>
    public int TotalPages { get; set; }
    
    /// <summary>Aktuell sida som visas</summary>
    public int CurrentPage { get; set; }
    
    /// <summary>Aktuell URL för sidan</summary>
    public string CurrentUrl { get; set; } = string.Empty;
    
    /// <summary>Felmeddelande att visa om något går fel</summary>
    public string ErrorMessage { get; set; } = string.Empty;
}