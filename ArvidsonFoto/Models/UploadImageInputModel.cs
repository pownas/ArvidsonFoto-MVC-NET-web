using System;
using System.ComponentModel.DataAnnotations;

namespace ArvidsonFoto.Models
{
    public class UploadImageInputModel
    {
        //public int ImageId { get; set; }
        public int? ImageHuvudfamilj { get; set; }
        public int? ImageFamilj { get; set; }
        public int ImageArt { get; set; }

        [Display(Name= "Filnamn")]
        [Required(ErrorMessage = "Ange filnamn")]
        [DataType(DataType.Text)]
        public string ImageUrl { get; set; }

        [Display(Name = "Fotodatum")]
        [Required(ErrorMessage = "Ange fotodatum")]
        [DataType(DataType.Date)]
        public DateTime ImageDate { get; set; }
        
        [Display(Name = "Kommentar")]
        [DataType(DataType.Text)]
        public string ImageDescription { get; set; }

        public bool ImageCreated { get; set; }

        //public DateTime ImageUpdate { get; set; }
    }
}
