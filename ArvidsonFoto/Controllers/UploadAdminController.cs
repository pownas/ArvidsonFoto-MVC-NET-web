using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArvidsonFoto.Data;
using ArvidsonFoto.Models;
using ArvidsonFoto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
        public IActionResult NyBild(string subLevel1, string subLevel2, string subLevel3, string subLevel4, UploadImageInputModel inputModel)
        {
            ViewData["Title"] = "Länka till ny bild";
            UploadImageViewModel viewModel = new UploadImageViewModel();
                viewModel.ImageInputModel = inputModel;

            if (subLevel4 is not null)
            {
                viewModel.SelectedCategory = _categoryService.GetByName(subLevel4);
                viewModel.SubCategories = _categoryService.GetSubsList(_categoryService.GetByName(subLevel4).MenuId).OrderBy(c => c.MenuText).ToList();
                viewModel.CurrentUrl = "./UploadAdmin/NyBild/" + subLevel1 + "/" + subLevel2 + "/" + subLevel3 + "/" + subLevel4;
            }
            else if (subLevel3 is not null)
            {
                viewModel.SelectedCategory = _categoryService.GetByName(subLevel3);
                viewModel.SubCategories = _categoryService.GetSubsList(_categoryService.GetByName(subLevel3).MenuId).OrderBy(c => c.MenuText).ToList();
                viewModel.CurrentUrl = "./UploadAdmin/NyBild/" + subLevel1 + "/" + subLevel2 + "/" + subLevel3;
            }
            else if (subLevel2 is not null)
            {
                viewModel.SelectedCategory = _categoryService.GetByName(subLevel2);
                viewModel.SubCategories = _categoryService.GetSubsList(_categoryService.GetByName(subLevel2).MenuId).OrderBy(c => c.MenuText).ToList();
                viewModel.CurrentUrl = "./UploadAdmin/NyBild/" + subLevel1 + "/" + subLevel2;
            }
            else if (subLevel1 is not null)
            {
                viewModel.SelectedCategory = _categoryService.GetByName(subLevel1);
                viewModel.SubCategories = _categoryService.GetSubsList(_categoryService.GetByName(subLevel1).MenuId).OrderBy(c => c.MenuText).ToList();
                viewModel.CurrentUrl = "./UploadAdmin/NyBild/" + subLevel1;
            }
            else
            {
                viewModel.SubCategories = _categoryService.GetSubsList(0); //Hämtar top-level menyn
                viewModel.CurrentUrl = "./UploadAdmin/NyBild";
            }

            return View(viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CreateImageLink(UploadImageInputModel model)
        {
            model.ImageCreated = false;

            if (ModelState.IsValid)
            {
                if (model.ImageFamilj.Equals(0))
                    model.ImageFamilj = null;
                
                if (model.ImageHuvudfamilj.Equals(0))
                    model.ImageHuvudfamilj = null;

                TblImage newImage = new TblImage()
                {
                    ImageHuvudfamilj = model.ImageHuvudfamilj,
                    ImageFamilj = model.ImageFamilj,
                    ImageArt = model.ImageArt,
                    ImageId = _imageService.GetImageLastId() + 1,
                    ImageUpdate = DateTime.Now,
                    ImageDate = model.ImageDate,
                    ImageDescription = model.ImageDescription,
                    ImageUrl = model.ImageUrl
                };

                if (_imageService.AddImage(newImage))
                {
                    model.ImageCreated = true; //Om allt OK...
                }
            }
            if (model.ImageCreated)
                return RedirectToAction("NyBild", new { model.ImageCreated, model.ImageArt, model.ImageUrl });
            else
                return RedirectToAction("NyBild", model);
        }

        public IActionResult NyKategori(UploadNewCategoryModel inputModel)
        {
            ViewData["Title"] = "Länka till ny kategori";
            return View(inputModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CreateCategory(UploadNewCategoryModel inputModel)
        {
            inputModel.CategoryCreated = false;

            if (ModelState.IsValid)
            {
                TblMenu newCategory = new TblMenu() {
                    MenuText = inputModel.MenuText,
                    MenuId = _categoryService.GetLastId() + 1,
                    MenuMainId = inputModel.MainMenuId,
                    MenuLastshowdate = DateTime.Now,
                    MenuPagecounter = 0
                };

                if (_categoryService.AddCategory(newCategory))
                {
                    inputModel.CategoryCreated = true; //Om allt OK...
                    inputModel.MainMenuId = null;
                }
            }

            return RedirectToAction("NyKategori", inputModel);
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

        /// <summary>Laddar sidan "Läs loggboken" , men heter visa för att slippa ä i Läs</summary>
        /// <param name="datum">Format: ÅÅÅÅMMDD (t.ex: 20210126)</param>
        public IActionResult VisaLoggboken(DateTime datum)
        {
            ViewData["Title"] = "Läser loggboken för: "+datum.ToString("yyyy-MM-dd dddd");

            AppLogReader logReader = new AppLogReader();
            List<string> logBook = new List<string>();
            string appLogFile = "appLog"+ datum.ToString("yyyyMMdd") + ".txt";
            logBook = logReader.ReadData(appLogFile);

            return View(logBook);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
