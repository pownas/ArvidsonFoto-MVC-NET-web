using System;
using System.Collections.Generic;

#nullable disable

namespace ArvidsonFoto.Models
{
    public partial class TblAdmin
    {
        public int AdminId { get; set; }
        public string AdminUser { get; set; }
        public string AdminPass { get; set; }
        public string AdminName { get; set; }
        public string AdminMail { get; set; }
        public DateTime? AdminLastonline { get; set; }
    }
}
