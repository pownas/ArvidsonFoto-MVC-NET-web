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
        private IGuestBookService _guestBookService;

        public UploadAdminController(ArvidsonFotoDbContext context)
        {
            _imageService = new ImageService(context);
            _categoryService = new CategoryService(context);
            _guestBookService = new GuestBookService(context);
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

        public IActionResult RedigeraBilder(string DisplayMessage, string imgId, int? sida)
        {
            ViewData["Title"] = "Redigera bland bilderna";

            int imagesPerPage = 25;
            if (sida is null || sida < 1)
                sida = 1;

            List<TblImage> displayTblImages = new List<TblImage>();

            UploadEditImagesViewModel viewModel = new UploadEditImagesViewModel()
            {
                AllImagesList = _imageService.GetAll().OrderByDescending(i => i.ImageId).ToList(),
                CurrentPage = (int)sida,
                CurrentUrl = "./UploadAdmin/RedigeraBilder"
            };
            viewModel.TotalPages = (int)Math.Ceiling(viewModel.AllImagesList.Count() / (decimal)imagesPerPage);
            displayTblImages = viewModel.AllImagesList
                                        .Skip(viewModel.CurrentPage * imagesPerPage)
                                        .Take(imagesPerPage)
                                        .ToList();
            viewModel.DisplayImagesList = new List<UploadImageInputModel>();

            if (string.IsNullOrWhiteSpace(DisplayMessage) && string.IsNullOrWhiteSpace(imgId))
            {
                viewModel.DisplayMessage = "OK";
            }
            else if (DisplayMessage.Equals("OKdeleted"))
            {
                viewModel.DisplayMessage = "OkDeleted";
                viewModel.UpdatedId = imgId;
            }
            else
            {
                viewModel.DisplayMessage = "ErrorDelete";
                viewModel.UpdatedId = imgId;
            }

            foreach (var item in displayTblImages)
            {
                DateTime imgDate = item.ImageDate ?? new DateTime(1900, 01, 01);

                UploadImageInputModel inputModel = new UploadImageInputModel()
                {
                    ImageId = item.ImageId,
                    ImageHuvudfamilj = item.ImageHuvudfamilj,
                    ImageHuvudfamiljNamn = _categoryService.GetNameById(item.ImageHuvudfamilj),
                    ImageFamilj = item.ImageFamilj,
                    ImageFamiljNamn = _categoryService.GetNameById(item.ImageFamilj),
                    ImageArt = item.ImageArt,
                    ImageArtNamn = _categoryService.GetNameById(item.ImageArt),
                    ImageDate = imgDate,
                    ImageUpdate = item.ImageUpdate,
                    ImageDescription = item.ImageDescription,
                    ImageUrl = item.ImageUrl
                };

                inputModel.ImageUrlFullSrc = "https://arvidsonfoto.se/Bilder";
                if (inputModel.ImageHuvudfamilj is not null)
                    inputModel.ImageUrlFullSrc += "/" + inputModel.ImageHuvudfamiljNamn;
                if (inputModel.ImageFamilj is not null)
                    inputModel.ImageUrlFullSrc += "/" + inputModel.ImageFamiljNamn;

                inputModel.ImageUrlFullSrc += "/" + inputModel.ImageArtNamn + "/" + inputModel.ImageUrl;

                viewModel.DisplayImagesList.Add(inputModel);
            }
            
            return View(viewModel);
        }

        public IActionResult HanteraGB(string DisplayMessage, string gbId)
        {
            ViewData["Title"] = "Hantera gästboken";
            UploadGbViewModel viewModel = new UploadGbViewModel();
            if (string.IsNullOrWhiteSpace(DisplayMessage) && string.IsNullOrWhiteSpace(gbId))
            {
                viewModel.Error = false;
            }
            else if(DisplayMessage.Equals("OK"))
            {
                viewModel.Error = false;
                viewModel.UpdatedId = gbId;
            }
            else
            {
                viewModel.Error = true;
                viewModel.UpdatedId = gbId;
            }
            return View(viewModel);
        }

        public IActionResult DeleteGbPost(int gbId)
        {
            if (User.Identity.IsAuthenticated)
            {
                if(_guestBookService.DeleteGbPost(gbId))
                { 
                    return RedirectToAction("HanteraGB", new { DisplayMessage = "OK", gbId = gbId });
                }
            }
            return RedirectToAction("HanteraGB", new { DisplayMessage = "Error", gbId = gbId });
        }

        public IActionResult DeleteImage(int imgId)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (_imageService.DeleteImgId(imgId))
                {
                    return RedirectToAction("RedigeraBilder", new { DisplayMessage = "OKdeleted", imgId = imgId });
                }
            }
            return RedirectToAction("RedigeraBilder", new { DisplayMessage = "Error", imgId = imgId });
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
            string appLogFile = "appLog"+ datum.ToString("yyyyMMdd") + ".txt";
            
            UploadLogReaderViewModel viewModel = new UploadLogReaderViewModel();
            viewModel.ExistingLogFiles = logReader.ExistingLogFiles();
            viewModel.LogBook = logReader.ReadData(appLogFile);
            viewModel.DateShown = datum;

            return View(viewModel);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
