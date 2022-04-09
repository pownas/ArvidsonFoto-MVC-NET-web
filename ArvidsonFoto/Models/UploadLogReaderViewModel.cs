namespace ArvidsonFoto.Models;

public class UploadLogReaderViewModel
{
    public List<string> ExistingLogFiles { get; set; }

    public List<string> LogBook { get; set; }

    public bool ShowAllLogs { get; set; }

    public DateTime DateShown { get; set; }
}