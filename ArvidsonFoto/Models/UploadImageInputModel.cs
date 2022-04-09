using System.ComponentModel.DataAnnotations;
namespace ArvidsonFoto.Models;

public class UploadImageInputModel
{
    public int ImageId { get; set; }
    public int? ImageHuvudfamilj { get; set; }
    public string ImageHuvudfamiljNamn { get; set; }
    public int? ImageFamilj { get; set; }
    public string ImageFamiljNamn { get; set; }
    public int ImageArt { get; set; }
    public string ImageArtNamn { get; set; }

    [Display(Name = "Filnamn")]
    [Required(ErrorMessage = "Ange filnamn")]
    [DataType(DataType.Text)]
    public string ImageUrl { get; set; }

    /// <summary>
    /// Dock utan ".thumb.jpg" eller ".jpg", på slutet
    /// </summary>
    public string ImageUrlFullSrc { get; set; }

    [Display(Name = "Fotodatum")]
    [Required(ErrorMessage = "Ange fotodatum")]
    [DataType(DataType.Date)]
    public DateTime ImageDate { get; set; }

    [Display(Name = "Beskrivning")]
    [DataType(DataType.Text)]
    [MaxLength(150, ErrorMessage = "Du får max ange 150-tecken i detta fältet")]
    public string ImageDescription { get; set; }

    public bool ImageCreated { get; set; }

    public DateTime ImageUpdate { get; set; }
}