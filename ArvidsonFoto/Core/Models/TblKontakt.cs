namespace ArvidsonFoto.Core.Models;

/// <summary>
/// Database backup of contact form submissions for when email sending fails
/// </summary>
public class TblKontakt
{
    /// <summary>Primary key</summary>
    public int Id { get; set; }

    /// <summary>Submission timestamp</summary>
    public DateTime SubmitDate { get; set; }

    /// <summary>Contact name</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Contact email address</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Subject line</summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>Message content</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Source page (Kontakta or Kop_av_bilder)</summary>
    public string SourcePage { get; set; } = string.Empty;

    /// <summary>Whether email was successfully sent</summary>
    public bool EmailSent { get; set; }

    /// <summary>Error message if email sending failed</summary>
    public string? ErrorMessage { get; set; }
}
