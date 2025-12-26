using ArvidsonFoto.Core.DTOs;

namespace ArvidsonFoto.Core.ViewModels;

/// <summary>
/// ViewModel for uploading and editing images functionality.
/// </summary>
/// <remarks>
/// Contains paginated image lists, navigation information, and display state management
/// for the image editing interface.
/// </remarks>
public class UploadEditImagesViewModel
{
    /// <summary>Lista med bilder som ska visas på nuvarande sida för redigering</summary>
    public List<UploadImageInputDto> DisplayImagesList { get; set; } = [];

    /// <summary>Totalt antal sidor i bildredigeraren</summary>
    public int TotalPages { get; set; } = -1;

    /// <summary>Aktuell sida som visas</summary>
    public int CurrentPage { get; set; } = -1;
    
    /// <summary>Aktuell URL för sidan</summary>
    public string CurrentUrl { get; set; } = string.Empty;
    
    /// <summary>Meddelande att visa för användarfeedback</summary>
    public string DisplayMessage { get; set; } = string.Empty;
    
    /// <summary>ID för den senast uppdaterade bilden</summary>
    public int? UpdatedId { get; set; } = null;
}