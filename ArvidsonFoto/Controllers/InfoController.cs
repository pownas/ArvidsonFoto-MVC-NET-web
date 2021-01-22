using ArvidsonFoto.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            ViewData["Title"] = "Köp av bilder";
            return View();
        }

        public IActionResult Gästbok()
        {
            ViewData["Title"] = "Gästbok";
            return View();
        }

        public IActionResult Kontakta()
        {
            ViewData["Title"] = "Kontaktinformation";
            return View();
        }

        public IActionResult Om_mig()
        {
            ViewData["Title"] = "Om mig, Torbjörn Arvidson";
            return View();
        }

        public IActionResult Sidkarta()
        {
            ViewData["Title"] = "Sidkarta";
            return View();
        }

        public IActionResult Copyright()
        {
            ViewData["Title"] = "Copyright";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
