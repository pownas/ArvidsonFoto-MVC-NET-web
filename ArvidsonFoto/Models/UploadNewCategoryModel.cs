using System;
using System.ComponentModel.DataAnnotations;

namespace ArvidsonFoto.Models
{
    public class UploadNewCategoryModel
    {
        [Required(ErrorMessage = "Ange ett kategorinamn")]
        public string MenuText { get; set; }

        [Required(ErrorMessage = "Välj en kategori att koppla den nya till")]
        public int? MainMenuId { get; set; }

        public bool CategoryCreated { get; set; }
    }
}