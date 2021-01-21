using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArvidsonFoto.Data;
using ArvidsonFoto.Models;

namespace ArvidsonFoto.Controllers
{
    public class SenastController : Controller
    {
        private IImageService _imageService;

        public BilderController(ArvidsonFotoDbContext context)
        {
            _imageService = new ImageService(context);
        }

        [Route("[controller]/{sortOrder}")]
        public IActionResult Index(string sortOrder, int? sida)
        {
            GalleryViewModel viewModel = new GalleryViewModel();
            int pageSize = 48;

            if (sida is null || sida < 1)
                sida = 1;

            viewModel.CurrentPage = (int)sida - 1;

            if (sortOrder.Equals("Per kategori"))
            {
                ViewData["Title"] = "Per kategori";
                viewModel.AllImagesList = _imageService.GetAllImagesList()
            }
            else if (sortOrder.Equals("Uppladdad"))
            {
                ViewData["Title"] = "Uppladdad";
            }
            else if (sortOrder.Equals("Fotograferad"))
            {
                ViewData["Title"] = "Fotograferad";
            }
            else
            {
                return RedirectToAction("Index", new { sortOrder = "Fotograferad" });
            }

            return View(viewModel);
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", new { sortOrder = "Fotograferad" });
        }
    }
}
