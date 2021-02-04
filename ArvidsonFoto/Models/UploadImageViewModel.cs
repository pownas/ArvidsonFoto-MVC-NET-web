using System;
using System.Collections.Generic;

namespace ArvidsonFoto.Models
{
    public class UploadImageViewModel
    {
        public TblMenu SelectedCategory { get; set; }

        public List<TblMenu> SubCategories { get; set; }

        public UploadImageInputModel ImageInputModel { get; set; }

        public string CurrentUrl { get; set; }
    }
}
