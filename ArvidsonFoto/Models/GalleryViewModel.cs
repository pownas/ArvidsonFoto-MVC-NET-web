namespace ArvidsonFoto.Models;

public class GalleryViewModel
{
    public List<TblImage> DisplayImagesList { get; set; }

    public List<TblImage> AllImagesList { get; set; }

    public TblMenu SelectedCategory { get; set; }

    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public string CurrentUrl { get; set; }
    public string ErrorMessage { get; set; }
}