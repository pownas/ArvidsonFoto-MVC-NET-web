namespace ArvidsonFoto.Core.ViewModels;

/// <summary>
/// ViewModel for log reader functionality.
/// </summary>
/// <remarks>
/// Contains information about existing log files, log content, and display options
/// for the log reading interface.
/// </remarks>
public class UploadLogReaderViewModel
{
    /// <summary>Lista över befintliga loggfiler</summary>
    public List<string> ExistingLogFiles { get; set; } = [];

    /// <summary>Innehåll från loggfiler</summary>
    public List<string> LogBook { get; set; } = [];

    /// <summary>Indikerar om alla loggar ska visas</summary>
    public bool ShowAllLogs { get; set; }

    /// <summary>Datum för vilken dag som visas</summary>
    public DateTime DateShown { get; set; } = DateTime.Now;
}