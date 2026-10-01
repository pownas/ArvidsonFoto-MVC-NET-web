using System.ComponentModel.DataAnnotations;
using ArvidsonFoto.Core;

namespace ArvidsonFoto.Core.DTOs;

/// <summary>
/// Data Transfer Object (DTO) for contact input form model submissions.
/// </summary>
/// <remarks>
/// Contains all the necessary fields for a contact form including validation requirements,
/// submission tracking, and display state management.
/// </remarks>
public class ContactFormInputDto
{
    /// <summary>Koden som behövs för att kunna skicka formuläret</summary>
    [Required(ErrorMessageResourceName = nameof(ValidationResource.CodeRequired), ErrorMessageResourceType = typeof(ValidationResource))]
    [RegularExpression(@"^(3568)$", ErrorMessageResourceName = nameof(ValidationResource.CodeInvalid), ErrorMessageResourceType = typeof(ValidationResource))]
    public required string Code { get; set; }

    /// <summary>E-postadress för kontakten</summary>
    [Required(ErrorMessageResourceName = nameof(ValidationResource.EmailRequired), ErrorMessageResourceType = typeof(ValidationResource))]
    [EmailAddress(ErrorMessageResourceName = nameof(ValidationResource.EmailInvalid), ErrorMessageResourceType = typeof(ValidationResource))]
    [MaxLength(150, ErrorMessageResourceName = nameof(ValidationResource.EmailTooLong), ErrorMessageResourceType = typeof(ValidationResource))]
    public required string Email { get; set; }

    /// <summary>Namn på personen som skickar formuläret</summary>
    [Required(ErrorMessageResourceName = nameof(ValidationResource.NameRequired), ErrorMessageResourceType = typeof(ValidationResource))]
    [MaxLength(50, ErrorMessageResourceName = nameof(ValidationResource.NameTooLong), ErrorMessageResourceType = typeof(ValidationResource))]
    public required string Name { get; set; }

    /// <summary>Rubrik för meddelandet</summary>
    [Required(ErrorMessageResourceName = nameof(ValidationResource.SubjectRequired), ErrorMessageResourceType = typeof(ValidationResource))]
    [StringLength(50, ErrorMessageResourceName = nameof(ValidationResource.SubjectTooLong), ErrorMessageResourceType = typeof(ValidationResource))]
    public required string Subject { get; set; }

    /// <summary>Huvudinnehåll i meddelandet</summary>
    [Required(ErrorMessageResourceName = nameof(ValidationResource.MessageRequired), ErrorMessageResourceType = typeof(ValidationResource))]
    [StringLength(2000, ErrorMessageResourceName = nameof(ValidationResource.MessageTooLong), ErrorMessageResourceType = typeof(ValidationResource))]
    public required string Message { get; set; }

    /// <summary>Placeholder-text för meddelandefältet</summary>
    public string MessagePlaceholder { get; set; } = "";

    /// <summary>Datum och tid när formuläret skickades</summary>
    public DateTime FormSubmitDate { get; set; } = DateTime.Now;

    /// <summary>Indikerar om e-post har skickats framgångsrikt</summary>
    public bool DisplayEmailSent { get; set; }

    /// <summary>Indikerar om det uppstod ett fel vid skickning</summary>
    public bool DisplayErrorSending { get; set; }

    /// <summary>URL för sidan att återvända till efter formulärinlämning</summary>
    public string ReturnPageUrl { get; set; } = "";

    /// <summary>
    /// Skapar en tom ContactFormDto med alla required fields initialiserade
    /// </summary>
    /// <returns>En ny tom ContactFormDto</returns>
    public static ContactFormInputDto CreateEmpty()
    {
        return new ContactFormInputDto
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