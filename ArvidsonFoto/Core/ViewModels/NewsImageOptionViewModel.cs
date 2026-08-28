namespace ArvidsonFoto.Core.ViewModels;

public class NewsImageOptionViewModel
{
    public int ImageId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? ImageDate { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
}
