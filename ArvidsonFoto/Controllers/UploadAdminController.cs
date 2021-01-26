using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArvidsonFoto.Controllers
{
    [Authorize]
    public class UploadAdminController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Upload Admin";
            return View();
        }

        public IActionResult NyBild()
        {
            ViewData["Title"] = "Länka till ny bild";
            return View();
        }

        public IActionResult NyKategori()
        {
            ViewData["Title"] = "Länka till ny kategori";
            return View();
        }

        public IActionResult RedigeraBilder()
        {
            ViewData["Title"] = "Redigera bland bilderna";
            return View();
        }

        public IActionResult HanteraGB()
        {
            ViewData["Title"] = "Hantera gästboken";
            return View();
        }

        public IActionResult Statistik()
        {
            ViewData["Title"] = "Hemsidans statistik";
            return View();
        }

        public IActionResult VisaLoggboken()
        {
            ViewData["Title"] = "Läs loggboken";
            return View();
        }

    }
}
