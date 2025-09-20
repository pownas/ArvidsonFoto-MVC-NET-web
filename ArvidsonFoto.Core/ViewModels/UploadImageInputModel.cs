namespace ArvidsonFoto.Core.ViewModels;

/// <summary>
/// ViewModel for uploading image input functionality.
/// </summary>
/// <remarks>
/// Contains all the necessary fields for uploading and managing image metadata including 
/// taxonomic classifications, dates, descriptions, and validation requirements.
/// </remarks>
public class UploadImageInputModel
{
    /// <summary>Bildens unika identifierare</summary>
    public int ImageId { get; set; }
    
    /// <summary>Huvudfamilj ID för taxonomisk klassificering</summary>
    public int? ImageHuvudfamilj { get; set; }
    
    /// <summary>Huvudfamilj namn för taxonomisk klassificering</summary>
    public required string ImageHuvudfamiljNamn { get; set; } = "";
    
    /// <summary>Familj ID för taxonomisk klassificering</summary>
    public int? ImageFamilj { get; set; }
    
    /// <summary>Familj namn för taxonomisk klassificering</summary>
    public required string ImageFamiljNamn { get; set; } = "";
    
    /// <summary>Art ID för taxonomisk klassificering</summary>
    public int ImageArt { get; set; }
    
    /// <summary>Art namn för taxonomisk klassificering</summary>
    public required string ImageArtNamn { get; set; } = "";

    /// <summary>Filnamn för bilden</summary>
    [Display(Name = "Filnamn")]
    [Required(ErrorMessage = "Ange filnamn")]
    [DataType(DataType.Text)]
    public required string ImageUrl { get; set; }

    /// <summary>
    /// Fullständig källsökväg för bilden
    /// </summary>
    /// <remarks>
    /// Dock utan ".thumb.jpg" eller ".jpg", på slutet
    /// </remarks>
    public required string ImageUrlFullSrc { get; set; } = "";

    /// <summary>Datum då bilden togs</summary>
    [Display(Name = "Fotodatum")]
    [Required(ErrorMessage = "Ange fotodatum")]
    [DataType(DataType.Date)]
    public DateTime ImageDate { get; set; }

    /// <summary>Beskrivning av bilden</summary>
    [Display(Name = "Beskrivning")]
    [DataType(DataType.Text)]
    [MaxLength(150, ErrorMessage = "Du får max ange 150-tecken i detta fältet")]
    public required string ImageDescription { get; set; } = "";

    /// <summary>Indikerar om bilden har skapats framgångsrikt</summary>
    public bool ImageCreated { get; set; }

    /// <summary>Datum och tid när bilden senast uppdaterades</summary>
    public DateTime ImageUpdate { get; set; }
}
