using System.ComponentModel.DataAnnotations;

namespace ArvidsonFoto.Core.DTOs;

/// <summary>
/// Data Transfer Object (DTO), InputModel för att skapa Facebook-inlägg
/// </summary>
public class UploadFacebookInputDto
{
    /// <summary>
    /// Lista med valda bild-ID:n (1-10 bilder)
    /// </summary>
    [Required(ErrorMessageResourceName = nameof(ValidationResource.AdminFacebookImageRequired), ErrorMessageResourceType = typeof(ValidationResource))]
    [MinLength(1, ErrorMessageResourceName = nameof(ValidationResource.AdminFacebookImageRequired), ErrorMessageResourceType = typeof(ValidationResource))]
    [MaxLength(10, ErrorMessageResourceName = nameof(ValidationResource.AdminFacebookImagesTooMany), ErrorMessageResourceType = typeof(ValidationResource))]
    public List<int> SelectedImageIds { get; set; } = new();

    /// <summary>
    /// Text som ska följa med inlägget
    /// </summary>
    [Required(ErrorMessageResourceName = nameof(ValidationResource.AdminFacebookMessageRequired), ErrorMessageResourceType = typeof(ValidationResource))]
    [DataType(DataType.MultilineText)]
    [MaxLength(5000, ErrorMessageResourceName = nameof(ValidationResource.AdminFacebookMessageTooLong), ErrorMessageResourceType = typeof(ValidationResource))]
    [Display(Name = nameof(ValidationResource.AdminFacebookMessageLabel), ResourceType = typeof(ValidationResource))]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Skapar en tom FacebookUploadInputDto med alla properties initialiserade
    /// </summary>
    /// <returns>En ny tom FacebookUploadInputDto</returns>
    public static UploadFacebookInputDto CreateEmpty()
    {
        return new UploadFacebookInputDto
        {
            SelectedImageIds = new List<int>(),
            Message = string.Empty
        };
    }
}