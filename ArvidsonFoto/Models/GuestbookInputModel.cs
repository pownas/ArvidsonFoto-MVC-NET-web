using System;
using System.ComponentModel.DataAnnotations;

namespace ArvidsonFoto.Models
{
    public class GuestbookInputModel
    {
        /// <summary>Koden som behövs för att kunna skicka formuläret</summary>
        [Required(ErrorMessage = "Vänligen fyll i en kod")] //Standard meddelande: "The Code field is required."
        [RegularExpression(@"^(3568)$", ErrorMessage = "Fel kod angiven")]
        public string Code { get; set; }

        [MaxLength(50, ErrorMessage = "För långt namn (max 50 tecken)")] //Använder MaxLength
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "Du har inte angivit en korrekt e-postadress. Ange en korrekt eller skippa epost helt")]
        [MaxLength(150, ErrorMessage = "Du har angivit en för lång epost-adress. Max 150 tecken")]
        public string Email { get; set; }

        [StringLength(250, ErrorMessage = "För lång rubrik (max 250 tecken)")] //StringLength
        [Url(ErrorMessage = "Du har inte angivit en korrekt URL. Ange korrekt eller skippa detta helt")]
        public string Homepage { get; set; }

        [Required(ErrorMessage = "Ange ett meddelande")]
        [StringLength(2000, ErrorMessage = "För långt meddelande (max 2000 tecken)")] //StringLength
        public string Message { get; set; }

        public DateTime FormSubmitDate { get; set; }

        public bool DisplayPublished { get; set; }
        public bool DisplayErrorPublish { get; set; }
    }
}