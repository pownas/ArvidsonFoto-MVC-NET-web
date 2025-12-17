using ArvidsonFoto.Models;

namespace ArvidsonFoto.Services;

/// <summary>
/// Service interface for managing contact form submissions
/// </summary>
public interface IContactService
{
    /// <summary>
    /// Save a contact form submission to the database
    /// </summary>
    /// <param name="kontakt">Contact form data to save</param>
    /// <returns>True if successful, false otherwise</returns>
    bool SaveContactSubmission(TblKontakt kontakt);
}
