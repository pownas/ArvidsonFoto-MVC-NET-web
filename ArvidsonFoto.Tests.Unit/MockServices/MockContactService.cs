using ArvidsonFoto.Core.Models;
using ArvidsonFoto.Services;

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

    public bool SaveContactSubmission(TblKontakt kontakt)
    {
        if (kontakt == null)
            return false;

        kontakt.Id = _nextId++;
        _mockContactSubmissions.Add(kontakt);
        return true;
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
}
