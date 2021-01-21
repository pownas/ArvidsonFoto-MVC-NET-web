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
        public IActionResult Index(string subLevel1, string subLevel2, string subLevel3, string subLevel4, int? sida)
        {
            GalleryViewModel viewModel = new GalleryViewModel();
            int pageSize = 48;

            if (sida is null || sida < 1)
                sida = 1;

            viewModel.CurrentPage = (int)sida - 1;

            if (subLevel4 is not null)
            {
                viewModel.SelectedCategory = _categoryService.GetByName(subLevel4);
                viewModel.AllImagesList = _imageService.GetAllImagesByCategoryID(_categoryService.GetIdByName(subLevel4));
                viewModel.CurrentUrl = "./Bilder/" + subLevel1 + "/" + subLevel2 + "/" + subLevel3 + "/" + subLevel4;
            }
            else if (subLevel3 is not null)
            {
                viewModel.SelectedCategory = _categoryService.GetByName(subLevel3);
                viewModel.AllImagesList = _imageService.GetAllImagesByCategoryID(_categoryService.GetIdByName(subLevel3));
                viewModel.CurrentUrl = "./Bilder/" + subLevel1 + "/" + subLevel2 + "/" + subLevel3;
            }
            else if (subLevel2 is not null)
            {
                viewModel.SelectedCategory = _categoryService.GetByName(subLevel2);
                viewModel.AllImagesList = _imageService.GetAllImagesByCategoryID(_categoryService.GetIdByName(subLevel2));
                viewModel.CurrentUrl = "./Bilder/" + subLevel1 + "/" + subLevel2;
            }
            else if (subLevel1 is not null)
            {
                viewModel.SelectedCategory = _categoryService.GetByName(subLevel1);
                viewModel.AllImagesList = _imageService.GetAllImagesByCategoryID(_categoryService.GetIdByName(subLevel1));
                viewModel.CurrentUrl = "./Bilder/" + subLevel1;
            }

            viewModel.DisplayImagesList = viewModel.AllImagesList.Skip(viewModel.CurrentPage * pageSize).Take(pageSize).OrderByDescending(i => i.ImageUpdate).ToList();
            viewModel.TotalPages = (int)Math.Ceiling(viewModel.AllImagesList.Count() / (decimal)pageSize);
            viewModel.CurrentPage = (int)sida;

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
