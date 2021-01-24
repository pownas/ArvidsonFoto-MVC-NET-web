using System;
using System.ComponentModel.DataAnnotations;

namespace ArvidsonFoto.Data
{
    public class ContactFormModel
    {
        /// <summary>Koden som behövs för att kunna skicka formuläret</summary>
        [Required(ErrorMessage = "Vänligen fyll i en kod")] //Standard meddelande: "The Code field is required."
        [RegularExpression(@"^(3568)$", ErrorMessage = "Fel kod angiven")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Ange din e-postadress")]
        [EmailAddress(ErrorMessage = "Du har inte angivit en korrekt e-postadress")]
        [MaxLength(100)] //Kan lägga till komma (100, ErrorMessage = "Fel angiven e-post")
        public string Email { get; set; }

        [Required(ErrorMessage = "Ange ditt namn")]
        [MaxLength(50, ErrorMessage = "För långt namn (max 50 tecken)")] //Använder MaxLength
        public string Name { get; set; }

        [Required(ErrorMessage = "Ange en rubrik")]
        [StringLength(50, ErrorMessage = "För lång rubrik (max 50 tecken)")] //StringLength
        public string Subject { get; set; }

        [Required(ErrorMessage = "Ange ett meddelande")]
        [StringLength(2000, ErrorMessage = "För långt meddelande (max 2000 tecken)")] //StringLength
        public string MessageBody { get; set; }

        public DateTime FormSubmit { get; set; }

        public bool DisplayEmailSent { get; set; }
        public bool DisplayErrorSending { get; set; }
    }
}
