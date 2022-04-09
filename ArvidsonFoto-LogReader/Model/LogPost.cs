namespace ArvidsonFoto_LogReader.Model;

public class LogPost
{
    public string IPadress { get; set; }
    public string Date { get; set; } // public DateTime Date { get; set; }
    public string HTTPmethod { get; set; }
    public string UrlPath { get; set; }
    public string ErrorCode { get; set; }
    public string TimeToLoad { get; set; }
    public string RequestFrom { get; set; } //????
    public string Browser { get; set; }
}