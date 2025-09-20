namespace ArvidsonFoto.Core.ViewModels;

/// <summary>
/// ViewModel for error page display.
/// </summary>
/// <remarks>
/// Contains information about request tracking and visited URLs for error handling.
/// </remarks>
public class ErrorViewModel
{
    /// <summary>Unik identifierare för begäran</summary>
    public string RequestId { get; set; } = string.Empty;

    /// <summary>Indikerar om begäran-ID ska visas</summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    /// <summary>URL som besöktes när felet uppstod</summary>
    public string VisitedUrl { get; set; } = string.Empty;
}