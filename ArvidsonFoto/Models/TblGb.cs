using System;
using System.Collections.Generic;

#nullable disable

namespace ArvidsonFoto.Models
{
    public partial class TblGb
    {
        public int Id { get; set; }
        public int GbId { get; set; }
        public string GbName { get; set; }
        public string GbEmail { get; set; }
        public string GbHomepage { get; set; }
        public string GbText { get; set; }
        public DateTime? GbDate { get; set; }
        public bool? GbReadPost { get; set; }
        public string GbIp { get; set; }
    }
}
