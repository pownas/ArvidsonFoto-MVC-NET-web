using System.ComponentModel.DataAnnotations;

namespace ArvidsonFoto.Core.DTOs;

/// <summary>
/// Data Transfer Object (DTO) for contact form submissions.
/// </summary>
/// <remarks>
/// Contains all the necessary fields for a contact form including validation requirements,
/// submission tracking, and display state management.
/// Note: Uses mutable properties (set instead of init) to support Blazor form binding.
/// </remarks>
public class ContactFormDto
{
    /// <summary>Koden som behövs för att kunna skicka formuläret</summary>
    [Required(ErrorMessage = "Vänligen fyll i en kod")]
    [RegularExpression(@"^(3568)$", ErrorMessage = "Fel kod angiven")]
    public string Code { get; set; } = string.Empty;

    /// <summary>E-postadress för kontakten</summary>
    [Required(ErrorMessage = "Ange din e-postadress")]
    [EmailAddress(ErrorMessage = "Du har inte angivit en korrekt e-postadress")]
    [MaxLength(150, ErrorMessage = "Du har angivit en för lång epost-adress. Max 150 tecken")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Namn på personen som skickar formuläret</summary>
    [Required(ErrorMessage = "Ange ditt namn")]
    [MaxLength(50, ErrorMessage = "För långt namn (max 50 tecken)")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Rubrik för meddelandet</summary>
    [Required(ErrorMessage = "Ange en rubrik")]
    [StringLength(50, ErrorMessage = "För lång rubrik (max 50 tecken)")]
    public string Subject { get; set; } = string.Empty;

    /// <summary>Huvudinnehåll i meddelandet</summary>
    [Required(ErrorMessage = "Ange ett meddelande")]
    [StringLength(2000, ErrorMessage = "För långt meddelande (max 2000 tecken)")]
    public string Message { get; set; } = string.Empty;

    /// <summary>Placeholder-text för meddelandefältet</summary>
    public string MessagePlaceholder { get; set; } = string.Empty;

    /// <summary>Datum och tid när formuläret skickades</summary>
    public DateTime FormSubmitDate { get; set; } = DateTime.Now;

    /// <summary>Indikerar om e-post har skickats framgångsrikt</summary>
    public bool DisplayEmailSent { get; set; }
    
    /// <summary>Indikerar om det uppstod ett fel vid skickning</summary>
    public bool DisplayErrorSending { get; set; }
    
    /// <summary>URL för sidan att återvända till efter formulärinlämning</summary>
    public string ReturnPageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Skapar en tom ContactFormDto med alla fields initialiserade
    /// </summary>
    /// <returns>En ny tom ContactFormDto</returns>
    public static ContactFormDto CreateEmpty()
    {
        return new ContactFormDto
        {
            Code = string.Empty,
            Email = string.Empty,
            Name = string.Empty,
            Subject = string.Empty,
            Message = string.Empty,
            MessagePlaceholder = string.Empty,
            FormSubmitDate = DateTime.Now,
            DisplayEmailSent = false,
            DisplayErrorSending = false,
            ReturnPageUrl = string.Empty
        };
    }
}