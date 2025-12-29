namespace ArvidsonFoto.Core.Configuration;

/// <summary>
/// SMTP configuration settings for sending emails
/// </summary>
public class SmtpSettings
{
    /// <summary>SMTP server address</summary>
    public string Server { get; set; } = string.Empty;

    /// <summary>SMTP server port (default: 587)</summary>
    public int Port { get; set; } = 587;

    /// <summary>Sender email address</summary>
    public string SenderEmail { get; set; } = string.Empty;

    /// <summary>Sender email password</summary>
    public string SenderPassword { get; set; } = string.Empty;

    /// <summary>Enable SSL/TLS encryption</summary>
    public bool EnableSsl { get; set; } = true;

    /// <summary>
    /// Validates that all required settings are configured
    /// </summary>
    public bool IsConfigured()
    {
        return !string.IsNullOrWhiteSpace(Server) &&
               !string.IsNullOrWhiteSpace(SenderEmail) &&
               !string.IsNullOrWhiteSpace(SenderPassword) &&
               Port > 0;
    }
}
