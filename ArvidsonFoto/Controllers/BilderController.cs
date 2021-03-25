using System;
using System.Collections.Generic;
//using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ArvidsonFoto.Data;
using ArvidsonFoto.Models;
using ArvidsonFoto.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ArvidsonFoto.Controllers
{
    public class BilderController : Controller
    {
        private IImageService _imageService;
        private ICategoryService _categoryService;
        private IPageCounterService _pageCounterService;

        public BilderController(ArvidsonFotoDbContext context)
        {
            _imageService = new ImageService(context);
            _categoryService = new CategoryService(context);
            _pageCounterService = new PageCounterService(context);
        }

        [Route("/[controller]/{subLevel1}")]
        [Route("/[controller]/{subLevel1}/{subLevel2}")]
        [Route("/[controller]/{subLevel1}/{subLevel2}/{subLevel3}")]
        [Route("/[controller]/{subLevel1}/{subLevel2}/{subLevel3}/{subLevel4}")]
        [Route("/[controller]/{subLevel1}/{subLevel2}/{subLevel3}/{subLevel4}/{subLevel5ImageName}")]
        public IActionResult Index(string subLevel1, string subLevel2, string subLevel3, string subLevel4, string subLevel5ImageName, int? sida)
        {
            GalleryViewModel viewModel = new GalleryViewModel();
            int pageSize = 48;

            if (sida is null || sida < 1)
                sida = 1;

            viewModel.CurrentPage = (int)sida - 1;

            if (subLevel4 is not null)
                subLevel4 = SharedStaticFunctions.ReplaceAAO(subLevel4);
            if (subLevel3 is not null)
                subLevel3 = SharedStaticFunctions.ReplaceAAO(subLevel3);
            if (subLevel2 is not null)
                subLevel2 = SharedStaticFunctions.ReplaceAAO(subLevel2);
            if (subLevel1 is not null)
                subLevel1 = SharedStaticFunctions.ReplaceAAO(subLevel1);


            if (subLevel4 is not null)
            {
                viewModel.SelectedCategory = _categoryService.GetByName(subLevel4);
                viewModel.AllImagesList = _imageService.GetAllImagesByCategoryID(_categoryService.GetIdByName(subLevel4)).OrderByDescending(i => i.ImageId).OrderByDescending(i => i.ImageDate).ToList();
                viewModel.CurrentUrl = "/Bilder/" + subLevel1 + "/" + subLevel2 + "/" + subLevel3 + "/" + subLevel4;
            }
            else if (subLevel3 is not null)
            {
                viewModel.SelectedCategory = _categoryService.GetByName(subLevel3);
                viewModel.AllImagesList = _imageService.GetAllImagesByCategoryID(_categoryService.GetIdByName(subLevel3)).OrderByDescending(i => i.ImageId).OrderByDescending(i => i.ImageDate).ToList();
                viewModel.CurrentUrl = "/Bilder/" + subLevel1 + "/" + subLevel2 + "/" + subLevel3;
            }
            else if (subLevel2 is not null)
            {
                viewModel.SelectedCategory = _categoryService.GetByName(subLevel2);
                viewModel.AllImagesList = _imageService.GetAllImagesByCategoryID(_categoryService.GetIdByName(subLevel2)).OrderByDescending(i => i.ImageId).OrderByDescending(i => i.ImageDate).ToList();
                viewModel.CurrentUrl = "/Bilder/" + subLevel1 + "/" + subLevel2;
            }
            else if (subLevel1 is not null)
            {
                viewModel.SelectedCategory = _categoryService.GetByName(subLevel1);
                viewModel.AllImagesList = _imageService.GetAllImagesByCategoryID(_categoryService.GetIdByName(subLevel1)).OrderByDescending(i => i.ImageId).OrderByDescending(i => i.ImageDate).ToList();
                viewModel.CurrentUrl = "/Bilder/" + subLevel1;
            }

            if (subLevel5ImageName is not null)
            {
                Log.Fatal($"User navigated to strange URL: /Bilder/{subLevel1}/{subLevel2}/{subLevel3}/{subLevel4}/{subLevel5ImageName}");
            }

            if(User.Identity.IsAuthenticated is false)
            {   //Räkar upp sidvisningar om användaren inte är inloggad: 
                _pageCounterService.AddPageCount("Bilder"); //Räkar upp att sidan "Bilder" besöks. 
                _pageCounterService.AddCategoryCount(viewModel.SelectedCategory.Id, viewModel.SelectedCategory.MenuText); //Räknar upp kategorins sidvisare och sätter datum till att sidan nu besöks.
                //_categoryService.AddPageCount(viewModel.SelectedCategory); //Förra räknaren...
            }

            viewModel.DisplayImagesList = viewModel.AllImagesList.Skip(viewModel.CurrentPage * pageSize).Take(pageSize).OrderByDescending(i => i.ImageId).OrderByDescending(i => i.ImageDate).ToList();
            viewModel.TotalPages = (int)Math.Ceiling(viewModel.AllImagesList.Count() / (decimal)pageSize);
            viewModel.CurrentPage = (int)sida;

            return View(viewModel);
        }

        [Route("/Bilder/")]
        [Route("/gallery.asp")]
        [Route("/showimagecategory.asp")]
        public IActionResult Bilder(int? ID)
        {
            var url = Url.ActionContext.HttpContext;
            string visitedUrl = HttpRequestExtensions.GetRawUrl(url);

            if (ID is not null && ID > 0 && ID < _categoryService.GetLastId())
            {
                string redirectUrl = "/Bilder/" + _categoryService.GetNameById(ID);
                Log.Fatal($"Redirect from page: {visitedUrl}, to page: {redirectUrl}");
                return RedirectPermanent(redirectUrl);
            }
            //Log.Fatal($"Redirect from page: {visitedUrl}, to page: /Senast/Fotograferad");
            return Redirect("./Senast/Fotograferad");
        }

        [Route("/Sök")]
        public IActionResult Sök(string s)
        {
            if (User.Identity.IsAuthenticated is false)
                _pageCounterService.AddPageCount("Sök");

            GalleryViewModel viewModel = new GalleryViewModel();
            if (s is null) //Besöker sidan utan att skrivit in någon sökning
            {
                ViewData["Title"] = "Sök bland bild-kategorierna";
            }
            else //Annars, om man skickar med en söksträng likt: /Sök?s=SöktText
            {
                Log.Information("En användare sökte efter: '"+s+"'"); //Borde logga i databas eller separat sök-fil... 
                ViewData["Title"] = "Söker efter: " + s;
                s = s.Trim(); // tar bort blankspace i början och slutet. Använd annars TrimEnd/TrimStart. 
                s = s.Replace("+", " ");
                List<TblMenu> allCategories = _categoryService.GetAll().OrderBy(c => c.MenuText).ToList();
                List<TblImage> listOfFirstSearchedImages = new List<TblImage>();
                foreach (var category in allCategories)
                {
                    if (category.MenuText.ToUpper().Contains(s.ToUpper()))
                        listOfFirstSearchedImages.Add(_imageService.GetOneImageFromCategory(category.MenuId));
                }
                viewModel.DisplayImagesList = listOfFirstSearchedImages;
                viewModel.SelectedCategory = new TblMenu() { MenuText = "SearchFor: " + s, MenuUrltext = "/Search" }; //För att _Gallery.cshtml , inte ska tolka detta som startsidan.
                if (listOfFirstSearchedImages.Count == 0)
                    Log.Warning("Hittade inget vid sökning: '"+s+"'"); //Borde logga i databas och då sätta ett "found" värde till false.
            }
            return View(viewModel);
        }
    }
}
