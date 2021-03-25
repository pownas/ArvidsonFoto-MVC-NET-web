using System;
using System.Collections.Generic;

#nullable disable

namespace ArvidsonFoto.Models
{
    public partial class TblMenu
    {
        public int Id { get; set; }
        public int MenuId { get; set; }
        public int? MenuMainId { get; set; }
        public string MenuText { get; set; }
        public string MenuUrltext { get; set; }
        //public string MenuEngtext { get; set; }

        //public DateTime? MenuLastshowdate { get; set; }
        //public int? MenuPagecounter { get; set; }
    }
}
