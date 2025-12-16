using System.ComponentModel.DataAnnotations;

namespace ArvidsonFoto.Models;

/// <summary>
/// InputModel för att skapa Facebook-inlägg
/// </summary>
public class FacebookUploadInputModel
{
    /// <summary>
    /// Lista med valda bild-ID:n (1-10 bilder)
    /// </summary>
    [Required(ErrorMessage = "Välj minst en bild")]
    [MinLength(1, ErrorMessage = "Välj minst en bild")]
    [MaxLength(10, ErrorMessage = "Du kan välja max 10 bilder")]
    public List<int> SelectedImageIds { get; set; } = new();

    /// <summary>
    /// Text som ska följa med inlägget
    /// </summary>
    [Required(ErrorMessage = "Skriv ett meddelande")]
    [DataType(DataType.MultilineText)]
    [MaxLength(5000, ErrorMessage = "Meddelandet får max vara 5000 tecken")]
    [Display(Name = "Meddelande")]
    public string Message { get; set; } = string.Empty;
}
