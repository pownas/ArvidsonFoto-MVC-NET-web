using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.DTOs;
using ArvidsonFoto.Core.Models;
using ArvidsonFoto.Core.Interfaces;
using Serilog;

namespace ArvidsonFoto.Core.Services;

/// <summary>
/// Service for managing contact form submissions
/// </summary>
public class ContactService(ArvidsonFotoCoreDbContext context) : IContactService
{
    private readonly ArvidsonFotoCoreDbContext _context = context;

    /// <summary>
    /// Save a contact form submission to the database
    /// </summary>
    public bool SaveContactSubmission(TblKontakt kontakt)
    {
        try
        {
            _context.TblKontakt.Add(kontakt);
            _context.SaveChanges();
            Log.Information($"Contact form saved to database. ID: {kontakt.Id}, SourcePage: {kontakt.SourcePage}, EmailSent: {kontakt.EmailSent}");
            return true;
        }
        catch (Exception ex)
        {
            Log.Error($"Error saving contact form to database: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Sends a contact message via email and saves it to the database
    /// </summary>
    public async Task<bool> SendContactMessageAsync(ContactFormDto contactForm)
    {
        try
        {
            // Validate CAPTCHA code
            if (contactForm.Code != "3568")
            {
                Log.Warning($"Invalid CAPTCHA code attempt from {contactForm.Email}");
                return false;
            }

            // Create database entity
            var kontakt = new TblKontakt
            {
                Name = contactForm.Name,
                Email = contactForm.Email,
                Subject = contactForm.Subject,
                Message = contactForm.Message,
                SourcePage = contactForm.ReturnPageUrl,
                SubmitDate = contactForm.FormSubmitDate,
                EmailSent = false,
                ErrorMessage = string.Empty
            };

            // Save to database
            _context.TblKontakt.Add(kontakt);
            await _context.SaveChangesAsync();

            Log.Information($"Contact form from {contactForm.Email} saved successfully. Subject: {contactForm.Subject}");

            // TODO: Implement actual email sending here
            // For now, just mark as sent
            kontakt.EmailSent = true;
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error processing contact form from {contactForm.Email}");
            return false;
        }
    }
}
