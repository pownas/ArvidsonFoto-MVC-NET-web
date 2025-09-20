using System.ComponentModel.DataAnnotations;

namespace ArvidsonFoto.Core.DTOs;

/// <summary>
/// Data Transfer Object (DTO) for guestbook entries.
/// </summary>
/// <remarks>
/// Contains all the necessary fields for a guestbook submission including validation requirements,
/// submission tracking, and display state management.
/// </remarks>
public class GuestbookInputDto
{
    /// <summary>Koden som behövs för att kunna skicka formuläret</summary>
    [Required(ErrorMessage = "Vänligen fyll i en kod")] //Standard meddelande: "The Code field is required."
    [RegularExpression(@"^(3568)$", ErrorMessage = "Fel kod angiven")]
    public required string Code { get; set; }

    /// <summary>Namn på personen som skriver i gästboken</summary>
    [MaxLength(50, ErrorMessage = "För långt namn (max 50 tecken)")] //Använder MaxLength
    public required string Name { get; set; }

    /// <summary>E-postadress för gästboksinlägget (valfritt)</summary>
    [EmailAddress(ErrorMessage = "Du har inte angivit en korrekt e-postadress. Ange en korrekt eller skippa epost helt")]
    [MaxLength(150, ErrorMessage = "Du har angivit en för lång epost-adress. Max 150 tecken")]
    public required string Email { get; set; } = "";

    /// <summary>Hemsida för personen som skriver i gästboken (valfritt)</summary>
    [StringLength(250, ErrorMessage = "För lång rubrik (max 250 tecken)")] //StringLength
    [Url(ErrorMessage = "Du har inte angivit en korrekt URL. Ange korrekt eller skippa detta helt")]
    public required string Homepage { get; set; } = "";

    /// <summary>Huvudinnehåll i gästboksinlägget</summary>
    [Required(ErrorMessage = "Ange ett meddelande")]
    [StringLength(2000, ErrorMessage = "För långt meddelande (max 2000 tecken)")] //StringLength
    public required string Message { get; set; }

    /// <summary>Datum och tid när formuläret skickades</summary>
    public DateTime FormSubmitDate { get; set; }

    /// <summary>Indikerar om inlägget har publicerats framgångsrikt</summary>
    public bool DisplayPublished { get; set; }
    
    /// <summary>Indikerar om det uppstod ett fel vid publicering</summary>
    public bool DisplayErrorPublish { get; set; }
}