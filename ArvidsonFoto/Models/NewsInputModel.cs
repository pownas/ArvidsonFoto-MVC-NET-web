using System.ComponentModel.DataAnnotations;

namespace ArvidsonFoto.Models;

public class NewsInputModel
{
    public int Id { get; set; } = -1;
    public int NewsId { get; set; } = -1;

    [Required(ErrorMessage = "Ange en titel för nyhetsartikeln")]
    [MaxLength(200, ErrorMessage = "Titeln får vara max 200 tecken")]
    public string NewsTitle { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ange innehållet för nyhetsartikeln")]
    public string NewsContent { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ange författare")]
    [MaxLength(100, ErrorMessage = "Författarnamnet får vara max 100 tecken")]
    public string NewsAuthor { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Sammanfattningen får vara max 500 tecken")]
    public string NewsSummary { get; set; } = string.Empty;

    public bool NewsPublished { get; set; } = false;

    public bool NewsCreated { get; set; } = false;
    public bool NewsUpdated { get; set; } = false;
    public bool DisplayErrorPublish { get; set; } = false;
}