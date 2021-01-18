using System;
using System.ComponentModel.DataAnnotations; //Lägger till [Required] och [MaxLength(n)] på de olika fälten, så de matchar mot databasen

namespace ArvidsonFoto.Models
{
    public class Category //tbl_menu
    {
        public int menu_ID { get; set; }

        [Required]
        public int menu_mainID { get; set; }

        [Required]
        [MaxLength(50)]
        public string menu_text { get; set; }

        [Required]
        [MaxLength(50)]
        public string menu_URLtext { get; set; }

        public DateTime menu_lastshowdate { get; set; }

        public int menu_pagecounter { get; set; }
    }
}