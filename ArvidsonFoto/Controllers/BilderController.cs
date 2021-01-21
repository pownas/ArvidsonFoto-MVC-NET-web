using ArvidsonFoto.Data;
using ArvidsonFoto.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArvidsonFoto.Controllers
{
    public class BilderController : Controller
    {
        private IImageService _imageService;
        private ICategoryService _categoryService;

        public BilderController(ArvidsonFotoDbContext context)
        {
            _imageService = new ImageService(context);
            _categoryService = new CategoryService(context);
        }

        [Route("/[controller]/{subLevel1}")]
        [Route("/[controller]/{subLevel1}/{subLevel2}")]
        [Route("/[controller]/{subLevel1}/{subLevel2}/{subLevel3}")]
        [Route("/[controller]/{subLevel1}/{subLevel2}/{subLevel3}/{subLevel4}")]
        public IActionResult Index(string subLevel1, string subLevel2, string subLevel3, string subLevel4)
        {
            GalleryViewModel viewModel = new GalleryViewModel();

            if (subLevel4 is not null)
            {
                viewModel.SelectedCategory = _categoryService.GetByName(subLevel4);
                viewModel.ImagesList = _imageService.GetAllImagesByCategoryID(_categoryService.GetIdByName(subLevel4));
            }
            else if (subLevel3 is not null)
            {
                viewModel.SelectedCategory = _categoryService.GetByName(subLevel3);
                viewModel.ImagesList = _imageService.GetAllImagesByCategoryID(_categoryService.GetIdByName(subLevel3));
            }
            else if (subLevel2 is not null)
            {
                viewModel.SelectedCategory = _categoryService.GetByName(subLevel2);
                viewModel.ImagesList = _imageService.GetAllImagesByCategoryID(_categoryService.GetIdByName(subLevel2));
            }
            else if (subLevel1 is not null)
            {
                viewModel.SelectedCategory = _categoryService.GetByName(subLevel1);
                viewModel.ImagesList = _imageService.GetAllImagesByCategoryID(_categoryService.GetIdByName(subLevel1));
            }
            return View(viewModel);
        }

        [Route("/Sök")]
        public IActionResult Sök()
        {
            ViewData["Title"] = "Sök efter bilder";
            GalleryViewModel viewModel = new GalleryViewModel();

            return View(viewModel);
        }

        [Route("/Sök/{s}")]
        public IActionResult Sök(string s)
        {
            ViewData["Title"] = "Söfer efter: " + s;
            GalleryViewModel viewModel = new GalleryViewModel();

            return View(viewModel);
        }
    }
}
