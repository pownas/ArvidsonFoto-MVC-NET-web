using ArvidsonFoto.Core.DTOs;
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
    /// <returns>True if successful, false otherwise</returns>
    bool SaveContactSubmission(TblKontakt kontakt);

    /// <summary>
    /// Sends a contact message via email and saves it to the database
    /// </summary>
    /// <param name="contactForm">Contact form data from user</param>
    /// <returns>True if email was sent successfully, false otherwise</returns>
    Task<bool> SendContactMessageAsync(ContactFormDto contactForm);
}
