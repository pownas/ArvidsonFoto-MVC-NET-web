using ArvidsonFoto.Core.Data;
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
    /// <returns>The ID of the saved contact record</returns>
    public int SaveContactSubmission(TblKontakt kontakt)
    {
        try
        {
            _context.TblKontakt.Add(kontakt);
            _context.SaveChanges();
            Log.Information("Contact form saved to database. ID: {ContactId}, SourcePage: {SourcePage}, EmailSent: {EmailSent}",
                kontakt.Id, kontakt.SourcePage, kontakt.EmailSent);
            return kontakt.Id;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving contact form to database");
            throw; // Re-throw to let controller handle
        }
    }

    /// <summary>
    /// Update the email status for a contact submission
    /// </summary>
    public bool UpdateEmailStatus(int contactId, bool emailSent, string? errorMessage)
    {
        try
        {
            var kontakt = _context.TblKontakt.Find(contactId);
            if (kontakt == null)
            {
                Log.Warning("Contact record not found for ID: {ContactId}", contactId);
                return false;
            }

            kontakt.EmailSent = emailSent;
            kontakt.ErrorMessage = errorMessage;
            _context.SaveChanges();

            Log.Information("Updated email status for contact ID: {ContactId} - EmailSent: {EmailSent}",
                contactId, emailSent);
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating email status for contact ID: {ContactId}", contactId);
            return false;
        }
    }

    /// <summary>
    /// Get a contact submission by ID
    /// </summary>
    public TblKontakt? GetContactSubmission(int contactId)
    {
        try
        {
            return _context.TblKontakt.Find(contactId);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error retrieving contact submission ID: {ContactId}", contactId);
            return null;
        }
    }

    /// <summary>
    /// Get all failed email submissions (for retry/follow-up)
    /// </summary>
    public IEnumerable<TblKontakt> GetFailedEmailSubmissions()
    {
        try
        {
            return _context.TblKontakt
                .Where(k => k.EmailSent == false)
                .OrderByDescending(k => k.SubmitDate)
                .ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error retrieving failed email submissions");
            return Enumerable.Empty<TblKontakt>();
        }
    }
}
