using System.ComponentModel.DataAnnotations;

namespace ArvidsonFoto.Core.DTOs;

/// <summary>
/// Data Transfer Object (DTO), Input model for uploading image input functionality.
/// </summary>
/// <remarks>
/// Contains all the necessary fields for uploading and managing image metadata including 
/// taxonomic classifications, dates, descriptions, and validation requirements.
/// </remarks>
public class UploadImageInputDto
{
    /// <summary>Bildens unika identifierare</summary>
    public int ImageId { get; set; }

    /// <summary>Huvudfamilj ID för taxonomisk klassificering</summary>
    public int? ImageHuvudfamilj { get; set; }

    /// <summary>Huvudfamilj namn för taxonomisk klassificering</summary>
    public string? ImageHuvudfamiljNamn { get; set; } = "";

    /// <summary>Familj ID för taxonomisk klassificering</summary>
    public int? ImageFamilj { get; set; }

    /// <summary>Familj namn för taxonomisk klassificering</summary>
    public string? ImageFamiljNamn { get; set; } = "";

    /// <summary>Art ID för taxonomisk klassificering</summary>
    public int ImageArt { get; set; }

    /// <summary>Art namn för taxonomisk klassificering</summary>
    public string? ImageArtNamn { get; set; } = "";

    /// <summary>Filnamn för bilden</summary>
    [Display(Name = nameof(ValidationResource.AdminImageFilenameLabel), ResourceType = typeof(ValidationResource))]
    [Required(ErrorMessageResourceName = nameof(ValidationResource.AdminImageFilenameRequired), ErrorMessageResourceType = typeof(ValidationResource))]
    [MinLength(2, ErrorMessageResourceName = nameof(ValidationResource.AdminImageFilenameTooShort), ErrorMessageResourceType = typeof(ValidationResource))]
    [DataType(DataType.Text)]
    public string? ImageUrl { get; set; } = "";

    /// <summary>
    /// Fullständig källsökväg för bilden
    /// </summary>
    /// <remarks>
    /// Dock utan ".thumb.jpg" eller ".jpg", på slutet
    /// </remarks>
    public string ImageUrlFullSrc { get; set; } = "";

    /// <summary>Datum då bilden togs</summary>
    [Display(Name = nameof(ValidationResource.AdminImageDateLabel), ResourceType = typeof(ValidationResource))]
    [Required(ErrorMessageResourceName = nameof(ValidationResource.AdminImageDateRequired), ErrorMessageResourceType = typeof(ValidationResource))]
    [DataType(DataType.Date)]
    public DateTime ImageDate { get; set; }

    /// <summary>Beskrivning av bilden</summary>
    [Display(Name = nameof(ValidationResource.AdminImageDescriptionLabel), ResourceType = typeof(ValidationResource))]
    [DataType(DataType.Text)]
    [MaxLength(150, ErrorMessageResourceName = nameof(ValidationResource.AdminImageDescriptionTooLong), ErrorMessageResourceType = typeof(ValidationResource))]
    public string? ImageDescription { get; set; } = "";

    [Display(Name = nameof(ValidationResource.AdminImageDescriptionEnLabel), ResourceType = typeof(ValidationResource))]
    [MaxLength(150)]
    public string? ImageDescriptionEn { get; set; }

    /// <summary>Indikerar om bilden har skapats framgångsrikt</summary>
    public bool ImageCreated { get; set; }

    public bool IsGameSelectable { get; set; } = true;

    /// <summary>Datum och tid när bilden senast uppdaterades</summary>
    public DateTime ImageUpdate { get; set; } = DateTime.Now;

    /// <summary>
    /// Skapar en tom UploadImageInputModel med alla required fields initialiserade
    /// </summary>
    /// <returns>En ny tom UploadImageInputModel</returns>
    public static UploadImageInputDto CreateEmpty()
    {
        return new UploadImageInputDto
        {
            ImageHuvudfamilj = null,
            ImageHuvudfamiljNamn = string.Empty,
            ImageFamilj = null,
            ImageFamiljNamn = string.Empty,
            ImageArt = -1,
            ImageArtNamn = string.Empty,
            ImageUrl = string.Empty,
            ImageUrlFullSrc = string.Empty,
            ImageDate = DateTime.Now,
            ImageDescription = string.Empty,
            ImageCreated = false,
            ImageUpdate = DateTime.Now
        };
    }
}