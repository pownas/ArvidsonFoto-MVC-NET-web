using ArvidsonFoto.Core.Models;

namespace ArvidsonFoto.Core.Interfaces;

/// <summary>
/// Service interface for managing contact form submissions
/// </summary>
public interface IContactService
{
    /// <summary>
    /// Save a contact form submission to the database
    /// </summary>
    /// <param name="kontakt">Contact form data to save</param>
    /// <returns>The ID of the saved contact record</returns>
    int SaveContactSubmission(TblKontakt kontakt);

    /// <summary>
    /// Update the email status for a contact submission
    /// </summary>
    /// <param name="contactId">The ID of the contact submission</param>
    /// <param name="emailSent">True if email was sent successfully, false otherwise</param>
    /// <param name="errorMessage">Error message if email sending failed</param>
    /// <returns>True if the status was updated successfully, false otherwise</returns>
    bool UpdateEmailStatus(int contactId, bool emailSent, string? errorMessage);

    /// <summary>
    /// Get a contact submission by ID
    /// </summary>
    /// <param name="contactId">The ID of the contact submission</param>
    /// <returns>The contact submission if found, null otherwise</returns>
    TblKontakt? GetContactSubmission(int contactId);

    /// <summary>
    /// Get all failed email submissions (for retry/follow-up)
    /// </summary>
    /// <returns>A list of contact submissions with failed emails</returns>
    IEnumerable<TblKontakt> GetFailedEmailSubmissions();
}
