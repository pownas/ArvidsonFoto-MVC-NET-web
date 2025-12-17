using ArvidsonFoto.Data;
using ArvidsonFoto.Models;
using Serilog;

namespace ArvidsonFoto.Services;

/// <summary>
/// Service for managing contact form submissions
/// </summary>
public class ContactService(ArvidsonFotoDbContext context) : IContactService
{
    private readonly ArvidsonFotoDbContext _context = context;

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
}
