using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArvidsonFoto.Controllers
{
    public class SenastController : Controller
    {
        [Route("[controller]/{sortOrder}")]
        public IActionResult Index(string sortOrder)
        {
            if (sortOrder.Equals("Per kategori"))
            {
                ViewData["Title"] = "Senast - Per kategori";
            }
            else if (sortOrder.Equals("Uppladdad"))
            {
                ViewData["Title"] = "Senast - Uppladdad";
            }
            else if (sortOrder.Equals("Fotograferad"))
            {
                ViewData["Title"] = "Senast - Fotograferad";
            }
            else
            {
                return RedirectToAction("Index", new { sortOrder = "Fotograferad" });
            }

            return View();
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", new { sortOrder = "Fotograferad" });
        }
    }
}
