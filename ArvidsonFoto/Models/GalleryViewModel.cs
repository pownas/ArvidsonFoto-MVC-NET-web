using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArvidsonFoto.Models
{
    public class GalleryViewModel
    {
        public List<TblImage> ImagesList { get; set; }

        public TblMenu SelectedCategory { get; set; }

        public string ErrorMessage { get; set; }
    }
}