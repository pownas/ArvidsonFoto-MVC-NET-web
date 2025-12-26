using ArvidsonFoto.Core.DTOs;
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

    public bool SaveContactSubmission(TblKontakt kontakt)
    {
        if (kontakt == null)
            return false;

        kontakt.Id = _nextId++;
        _mockContactSubmissions.Add(kontakt);
        return true;
    }

    public Task<bool> SendContactMessageAsync(ContactFormDto contactForm)
    {
        if (contactForm == null || string.IsNullOrWhiteSpace(contactForm.Email))
            return Task.FromResult(false);

        // Validate CAPTCHA
        if (contactForm.Code != "3568")
            return Task.FromResult(false);

        var kontakt = new TblKontakt
        {
            Id = _nextId++,
            Name = contactForm.Name,
            Email = contactForm.Email,
            Subject = contactForm.Subject,
            Message = contactForm.Message,
            SourcePage = contactForm.ReturnPageUrl,
            SubmitDate = contactForm.FormSubmitDate,
            EmailSent = true,
            ErrorMessage = string.Empty
        };

        _mockContactSubmissions.Add(kontakt);
        return Task.FromResult(true);
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
