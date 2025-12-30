using ArvidsonFoto.Core.DTOs;
using ArvidsonFoto.Core.ViewModels;

namespace ArvidsonFoto.Core.ViewModels;

/// <summary>
/// ViewModel för Facebook-uppladdningssidan
/// </summary>
public class FacebookUploadViewModel
{
    /// <summary>
    /// Lista med de senaste bilderna (max 25)
    /// </summary>
    public List<UploadImageInputModel> RecentImages { get; set; } = new();

    /// <summary>
    /// Meddelande att visa för användaren (success/error)
    /// </summary>
    public string? DisplayMessage { get; set; }

    /// <summary>
    /// URL till det skapade Facebook-inlägget
    /// </summary>
    public string? FacebookPostUrl { get; set; }

    /// <summary>
    /// Inputmodellen för formuläret
    /// </summary>
    public FacebookUploadInputDto InputModel { get; set; } = new();
}
