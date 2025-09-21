namespace ArvidsonFoto.Core.ViewModels;

/// <summary>
/// ViewModel for uploading guestbook entries functionality.
/// </summary>
/// <remarks>
/// Contains tracking information and display messages for guestbook upload operations.
/// </remarks>
public class UploadGbViewModel
{
    /// <summary>ID för den senast uppdaterade gästboksposten</summary>
    public string UpdatedId { get; set; } = string.Empty;
    
    /// <summary>Meddelande att visa för användarfeedback</summary>
    public string DisplayMessage { get; set; } = string.Empty;
}