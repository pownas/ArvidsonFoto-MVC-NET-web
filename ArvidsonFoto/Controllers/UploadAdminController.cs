using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArvidsonFoto.Data;
using ArvidsonFoto.Models;

namespace ArvidsonFoto.Controllers
{
    [Authorize]
    public class UploadAdminController : Controller
    {

        private IImageService _imageService;
        private ICategoryService _categoryService;

        public UploadAdminController(ArvidsonFotoDbContext context)
        {
            _imageService = new ImageService(context);
            _categoryService = new CategoryService(context);
        }
        
        public IActionResult Index()
        {
            ViewData["Title"] = "Upload Admin";
            return View();
        }

        [Route("/[controller]/NyBild")]
        [Route("/[controller]/NyBild/{subLevel1}")]
        [Route("/[controller]/NyBild/{subLevel1}/{subLevel2}")]
        [Route("/[controller]/NyBild/{subLevel1}/{subLevel2}/{subLevel3}")]
        [Route("/[controller]/NyBild/{subLevel1}/{subLevel2}/{subLevel3}/{subLevel4}")]
        public IActionResult NyBild(string subLevel1, string subLevel2, string subLevel3, string subLevel4)
        {
            ViewData["Title"] = "Länka till ny bild";

            UploadImageViewModel viewModel = new UploadImageViewModel();

            if (subLevel4 is not null)
            {
                viewModel.SelectedCategory = _categoryService.GetByName(subLevel4);
                viewModel.SubCategories = _categoryService.GetSubsList(_categoryService.GetByName(subLevel4).MenuId);
                viewModel.CurrentUrl = "./UploadAdmin/NyBild/" + subLevel1 + "/" + subLevel2 + "/" + subLevel3 + "/" + subLevel4;
            }
            else if (subLevel3 is not null)
            {
                viewModel.SelectedCategory = _categoryService.GetByName(subLevel3);
                viewModel.SubCategories = _categoryService.GetSubsList(_categoryService.GetByName(subLevel3).MenuId);
                viewModel.CurrentUrl = "./UploadAdmin/NyBild/" + subLevel1 + "/" + subLevel2 + "/" + subLevel3;
            }
            else if (subLevel2 is not null)
            {
                viewModel.SelectedCategory = _categoryService.GetByName(subLevel2);
                viewModel.SubCategories = _categoryService.GetSubsList(_categoryService.GetByName(subLevel2).MenuId);
                viewModel.CurrentUrl = "./UploadAdmin/NyBild/" + subLevel1 + "/" + subLevel2;
            }
            else if (subLevel1 is not null)
            {
                viewModel.SelectedCategory = _categoryService.GetByName(subLevel1);
                viewModel.SubCategories = _categoryService.GetSubsList(_categoryService.GetByName(subLevel1).MenuId);
                viewModel.CurrentUrl = "./UploadAdmin/NyBild/" + subLevel1;
            }
            else
            {
                viewModel.SubCategories = _categoryService.GetSubsList(0); //Hämtar top-level menyn
                viewModel.CurrentUrl = "./UploadAdmin/NyBild";
            }

            return View(viewModel);
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
