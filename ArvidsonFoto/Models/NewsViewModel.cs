namespace ArvidsonFoto.Models;

public class NewsViewModel
{
    public List<TblNews> NewsList { get; set; } = new List<TblNews>();
    public TblNews? SelectedNews { get; set; }
    public bool ShowAdminControls { get; set; } = false;
}