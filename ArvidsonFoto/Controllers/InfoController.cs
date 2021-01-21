using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArvidsonFoto.Controllers
{
    public class InfoController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Info";
            return View();
        }

        public IActionResult Köp_av_bilder()
        {
            ViewData["Title"] = "Info - Köp av bilder";
            return View();
        }

        public IActionResult Gästbok()
        {
            ViewData["Title"] = "Info - Gästbok";
            return View();
        }

        public IActionResult Kontakta()
        {
            ViewData["Title"] = "Info - Kontaktinformation";
            return View();
        }

        public IActionResult Om_mig()
        {
            ViewData["Title"] = "Info - Om mig, Torbjörn Arvidson";
            return View();
        }

        public IActionResult Sidkarta()
        {
            ViewData["Title"] = "Info - Sidkarta";
            return View();
        }

        public IActionResult Copyright()
        {
            ViewData["Title"] = "Info - Copyright";
            return View();
        }
    }
}
