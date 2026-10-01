using System.ComponentModel.DataAnnotations;
using ArvidsonFoto.Core;

namespace ArvidsonFoto.Core.DTOs;

/// <summary>
/// Data Transfer Object (DTO) for guestbook entries input model.
/// </summary>
/// <remarks>
/// Contains all the necessary fields for a guestbook submission including validation requirements,
/// submission tracking, and display state management.
/// </remarks>
public class GuestbookFormInputDto
{
    /// <summary>Koden som behövs för att kunna skicka formuläret</summary>
    [Required(ErrorMessageResourceName = nameof(ValidationResource.CodeRequired), ErrorMessageResourceType = typeof(ValidationResource))]
    [RegularExpression(@"^(3568)$", ErrorMessageResourceName = nameof(ValidationResource.CodeInvalid), ErrorMessageResourceType = typeof(ValidationResource))]
    public required string Code { get; set; }

    /// <summary>Namn på personen som skriver i gästboken</summary>
    [MaxLength(50, ErrorMessageResourceName = nameof(ValidationResource.NameTooLong), ErrorMessageResourceType = typeof(ValidationResource))]
    public required string Name { get; set; }

    /// <summary>E-postadress för gästboksinlägget (valfritt)</summary>
    [EmailAddress(ErrorMessageResourceName = nameof(ValidationResource.GuestbookEmailInvalid), ErrorMessageResourceType = typeof(ValidationResource))]
    [MaxLength(150, ErrorMessageResourceName = nameof(ValidationResource.EmailTooLong), ErrorMessageResourceType = typeof(ValidationResource))]
    public required string Email { get; set; } = "";

    /// <summary>Hemsida för personen som skriver i gästboken (valfritt)</summary>
    [StringLength(250, ErrorMessageResourceName = nameof(ValidationResource.WebsiteTooLong), ErrorMessageResourceType = typeof(ValidationResource))]
    [Url(ErrorMessageResourceName = nameof(ValidationResource.WebsiteInvalid), ErrorMessageResourceType = typeof(ValidationResource))]
    public string? Homepage { get; set; } = "";

    /// <summary>Huvudinnehåll i gästboksinlägget</summary>
    [Required(ErrorMessageResourceName = nameof(ValidationResource.MessageRequired), ErrorMessageResourceType = typeof(ValidationResource))]
    [StringLength(2000, ErrorMessageResourceName = nameof(ValidationResource.MessageTooLong), ErrorMessageResourceType = typeof(ValidationResource))]
    public required string Message { get; set; }

    /// <summary>Datum och tid när formuläret skickades</summary>
    public DateTime FormSubmitDate { get; set; }

    /// <summary>Indikerar om inlägget har publicerats framgångsrikt</summary>
    public bool DisplayPublished { get; set; }

    /// <summary>Indikerar om det uppstod ett fel vid publicering</summary>
    public bool DisplayErrorPublish { get; set; }

    /// <summary>
    /// Skapar en tom GuestbookInputDto med alla required fields initialiserade
    /// </summary>
    /// <returns>En ny tom GuestbookInputDto</returns>
    public static GuestbookFormInputDto CreateEmpty()
    {
        return new GuestbookFormInputDto
        {
            Code = string.Empty,
            Name = string.Empty,
            Email = string.Empty,
            Homepage = string.Empty,
            Message = string.Empty,
            FormSubmitDate = DateTime.Now,
            DisplayPublished = false,
            DisplayErrorPublish = false
        };
    }
}