using ArvidsonFoto.Core.Interfaces;
using ArvidsonFoto.Core.Models;

namespace ArvidsonFoto.Tests.Unit.MockServices;

/// <summary>
/// Mock implementation of IContactService for unit testing.
/// </summary>
public class MockContactService : IContactService
{
    private readonly List<TblKontakt> _mockContactSubmissions;
    private int _nextId;

    public MockContactService()
    {
        _mockContactSubmissions = new List<TblKontakt>();
        _nextId = 1;
    }

    public int SaveContactSubmission(TblKontakt kontakt)
    {
        if (kontakt == null)
            throw new ArgumentNullException(nameof(kontakt));

        kontakt.Id = _nextId++;
        _mockContactSubmissions.Add(kontakt);
        return kontakt.Id;
    }

    public bool UpdateEmailStatus(int contactId, bool emailSent, string? errorMessage)
    {
        var kontakt = _mockContactSubmissions.FirstOrDefault(k => k.Id == contactId);
        if (kontakt == null)
            return false;

        kontakt.EmailSent = emailSent;
        kontakt.ErrorMessage = errorMessage;
        return true;
    }

    public TblKontakt? GetContactSubmission(int contactId)
    {
        return _mockContactSubmissions.FirstOrDefault(k => k.Id == contactId);
    }

    public IEnumerable<TblKontakt> GetFailedEmailSubmissions()
    {
        return _mockContactSubmissions
            .Where(k => k.EmailSent == false)
            .OrderByDescending(k => k.SubmitDate)
            .ToList();
    }

    // Helper methods for testing (not part of IContactService interface)
    internal List<TblKontakt> GetAll()
    {
        return _mockContactSubmissions.OrderByDescending(k => k.SubmitDate).ToList();
    }

    internal TblKontakt? GetById(int id)
    {
        return _mockContactSubmissions.FirstOrDefault(k => k.Id == id);
    }

    internal void Clear()
    {
        _mockContactSubmissions.Clear();
        _nextId = 1;
    }
}
