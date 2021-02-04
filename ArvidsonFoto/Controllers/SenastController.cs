using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArvidsonFoto.Data;
using ArvidsonFoto.Models;
using ArvidsonFoto.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ArvidsonFoto.Controllers
{
    public class SenastController : Controller
    {
        private IImageService _imageService;
        private ICategoryService _categoryService;

        public SenastController(ArvidsonFotoDbContext context)
        {
            _imageService = new ImageService(context);
            _categoryService = new CategoryService(context);
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
                List<TblMenu> categories = _categoryService.GetAll().OrderBy(c => c.MenuText).ToList();
                viewModel.AllImagesList = new List<TblImage>();
                foreach (var category in categories)
                {
                    viewModel.AllImagesList.Add(_imageService.GetOneImageFromCategory(category.MenuId));
                }
            }
            else if (sortOrder.Equals("Uppladdad"))
            {
                ViewData["Title"] = "Uppladdad";
                viewModel.AllImagesList = _imageService.GetAll().OrderByDescending(i => i.ImageUpdate).ToList();
            }
            else if (sortOrder.Equals("Fotograferad"))
            {
                ViewData["Title"] = "Fotograferad";
                viewModel.AllImagesList = _imageService.GetAll().OrderByDescending(i => i.ImageDate).ToList();
            }
            else
            {
                return RedirectToAction("Index", new { sortOrder = "Fotograferad" });
            }

            viewModel.SelectedCategory = new TblMenu() { MenuText = sortOrder }; //Lägger till en SelectedCategory, så det inte blir tolkat som startsidan. 
            viewModel.DisplayImagesList = viewModel.AllImagesList.Skip(viewModel.CurrentPage * pageSize).Take(pageSize).OrderByDescending(i => i.ImageUpdate).ToList();
            viewModel.TotalPages = (int)Math.Ceiling(viewModel.AllImagesList.Count() / (decimal)pageSize);
            viewModel.CurrentPage = (int)sida;
            viewModel.CurrentUrl = "./Senast/" + sortOrder;

            return View(viewModel);
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", new { sortOrder = "Fotograferad" });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
