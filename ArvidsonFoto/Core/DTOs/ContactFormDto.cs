using System.ComponentModel.DataAnnotations;

namespace ArvidsonFoto.Core.DTOs;

/// <summary>
/// Data Transfer Object (DTO) for contact form submissions.
/// </summary>
/// <remarks>
/// Contains all the necessary fields for a contact form including validation requirements,
/// submission tracking, and display state management.
/// </remarks>
public class ContactFormDto
{
    /// <summary>Koden som behövs för att kunna skicka formuläret</summary>
    [Required(ErrorMessage = "Vänligen fyll i en kod")] //Standard meddelande: "The Code field is required."
    [RegularExpression(@"^(3568)$", ErrorMessage = "Fel kod angiven")]
    public required string Code { get; set; }

    /// <summary>E-postadress för kontakten</summary>
    [Required(ErrorMessage = "Ange din e-postadress")]
    [EmailAddress(ErrorMessage = "Du har inte angivit en korrekt e-postadress")]
    [MaxLength(150, ErrorMessage = "Du har angivit en för lång epost-adress. Max 150 tecken")]
    public required string Email { get; set; }

    /// <summary>Namn på personen som skickar formuläret</summary>
    [Required(ErrorMessage = "Ange ditt namn")]
    [MaxLength(50, ErrorMessage = "För långt namn (max 50 tecken)")] //Använder MaxLength
    public required string Name { get; set; }

    /// <summary>Rubrik för meddelandet</summary>
    [Required(ErrorMessage = "Ange en rubrik")]
    [StringLength(50, ErrorMessage = "För lång rubrik (max 50 tecken)")] //StringLength
    public required string Subject { get; set; }

    /// <summary>Huvudinnehåll i meddelandet</summary>
    [Required(ErrorMessage = "Ange ett meddelande")]
    [StringLength(2000, ErrorMessage = "För långt meddelande (max 2000 tecken)")] //StringLength
    public required string Message { get; set; }

    /// <summary>Placeholder-text för meddelandefältet</summary>
    public required string MessagePlaceholder { get; set; } = "";

    /// <summary>Datum och tid när formuläret skickades</summary>
    public DateTime FormSubmitDate { get; set; }

    /// <summary>Indikerar om e-post har skickats framgångsrikt</summary>
    public bool DisplayEmailSent { get; set; }
    
    /// <summary>Indikerar om det uppstod ett fel vid skickning</summary>
    public bool DisplayErrorSending { get; set; }
    
    /// <summary>URL för sidan att återvända till efter formulärinlämning</summary>
    public required string ReturnPageUrl { get; set; } = "";
}