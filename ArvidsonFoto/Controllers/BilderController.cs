using ArvidsonFoto.Core.DTOs;
using ArvidsonFoto.Core.Interfaces;
using ArvidsonFoto.Core.ViewModels;
using ArvidsonFoto.Services;
using ArvidsonFoto.Views.Shared;

namespace ArvidsonFoto.Controllers;

public class BilderController(
    IApiImageService imageService,
    IApiCategoryService categoryService,
    IPageCounterService pageCounterService) : Controller
{
    private readonly IApiImageService _imageService = imageService;
    private readonly IApiCategoryService _categoryService = categoryService;
    private readonly IPageCounterService _pageCounterService = pageCounterService;

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
            var selectedCategory = _categoryService.GetByName(subLevel4);
            if (selectedCategory == null || selectedCategory.CategoryId == null || selectedCategory.CategoryId == -1)
            {
                Log.Warning($"Invalid category requested: /Bilder/{subLevel1}/{subLevel2}/{subLevel3}/{subLevel4}");
                return NotFound();
            }
            viewModel.SelectedCategory = selectedCategory;
            var images = _imageService.GetImagesByCategoryID(_categoryService.GetIdByName(subLevel4));
            viewModel.AllImagesList = images.OrderByDescending(i => i.ImageId).ThenByDescending(i => i.DateImageTaken).ToList();
            viewModel.CurrentUrl = "/Bilder/" + subLevel1 + "/" + subLevel2 + "/" + subLevel3 + "/" + subLevel4;
        }
        else if (subLevel3 is not null)
        {
            var selectedCategory = _categoryService.GetByName(subLevel3);
            if (selectedCategory == null || selectedCategory.CategoryId == null || selectedCategory.CategoryId == -1)
            {
                Log.Warning($"Invalid category requested: /Bilder/{subLevel1}/{subLevel2}/{subLevel3}");
                return NotFound();
            }
            viewModel.SelectedCategory = selectedCategory;
            var images = _imageService.GetImagesByCategoryID(_categoryService.GetIdByName(subLevel3));
            viewModel.AllImagesList = images.OrderByDescending(i => i.ImageId).ThenByDescending(i => i.DateImageTaken).ToList();
            viewModel.CurrentUrl = "/Bilder/" + subLevel1 + "/" + subLevel2 + "/" + subLevel3;
        }
        else if (subLevel2 is not null)
        {
            var selectedCategory = _categoryService.GetByName(subLevel2);
            if (selectedCategory == null || selectedCategory.CategoryId == null || selectedCategory.CategoryId == -1)
            {
                Log.Warning($"Invalid category requested: /Bilder/{subLevel1}/{subLevel2}");
                return NotFound();
            }
            viewModel.SelectedCategory = selectedCategory;
            var images = _imageService.GetImagesByCategoryID(_categoryService.GetIdByName(subLevel2));
            viewModel.AllImagesList = images.OrderByDescending(i => i.ImageId).ThenByDescending(i => i.DateImageTaken).ToList();
            viewModel.CurrentUrl = "/Bilder/" + subLevel1 + "/" + subLevel2;
        }
        else if (subLevel1 is not null)
        {
            var selectedCategory = _categoryService.GetByName(subLevel1);
            if (selectedCategory == null || selectedCategory.CategoryId == null || selectedCategory.CategoryId == -1)
            {
                Log.Warning($"Invalid category requested: /Bilder/{subLevel1}");
                return NotFound();
            }
            viewModel.SelectedCategory = selectedCategory;
            var images = _imageService.GetImagesByCategoryID(_categoryService.GetIdByName(subLevel1));
            viewModel.AllImagesList = images.OrderByDescending(i => i.ImageId).ThenByDescending(i => i.DateImageTaken).ToList();
            viewModel.CurrentUrl = "/Bilder/" + subLevel1;
        }

        if (subLevel5ImageName is not null)
        {
            Log.Fatal($"User navigated to strange URL: /Bilder/{subLevel1}/{subLevel2}/{subLevel3}/{subLevel4}/{subLevel5ImageName}");
        }

        if (User?.Identity?.IsAuthenticated is false && viewModel.SelectedCategory != null && viewModel.SelectedCategory.CategoryId.HasValue && viewModel.SelectedCategory.Name != null)
        {
            _pageCounterService.AddPageCount("Bilder");
            _pageCounterService.AddCategoryCount(viewModel.SelectedCategory.CategoryId.Value, viewModel.SelectedCategory.Name);
        }

        if (viewModel.AllImagesList == null)
        {
            viewModel.AllImagesList = new List<ImageDto>();
        }

        viewModel.DisplayImagesList = viewModel.AllImagesList
            .Skip(viewModel.CurrentPage * pageSize)
            .Take(pageSize)
            .ToList();
        viewModel.TotalPages = (int)Math.Ceiling(viewModel.AllImagesList.Count() / (decimal)pageSize);
        viewModel.CurrentPage = (int)sida;

        return View(viewModel);
    }

    [Route("/Bilder/")]
    [Route("/gallery.asp")]
    [Route("/showimagecategory.asp")]
    public IActionResult Bilder(int? ID)
    {
        var url = Url.ActionContext.HttpContext ?? null;
        string visitedUrl = HttpRequestExtensions.GetRawUrl(url) ?? "";

        if (ID is not null && ID > 0 && ID < _categoryService.GetLastId())
        {
            string redirectUrl = "/Bilder/" + _categoryService.GetNameById(ID);
            Log.Fatal($"Redirect from page: {visitedUrl}, to page: {redirectUrl}");
            return RedirectPermanent(redirectUrl);
        }
        return Redirect("./Senast/Fotograferad");
    }

    [Route("/search")]
    public IActionResult Search(string s)
    {
        if (User?.Identity?.IsAuthenticated is false)
            _pageCounterService.AddPageCount("search");

        GalleryViewModel viewModel = new GalleryViewModel();
        
        ViewBag.SearchQuery = s ?? "";
        
        if (s is null)
        {
            ViewData["Title"] = "Sök bland bild-kategorierna";
        }
        else
        {
            Log.Information("En användare sökte efter: '" + s + "'");
            ViewData["Title"] = "Söker efter: " + s;
            s = s.Trim();
            s = s.Replace("+", " ");
            List<CategoryDto> allCategories = _categoryService.GetAll().OrderBy(c => c.Name).ToList();
            List<ImageDto> listOfFirstSearchedImages = new List<ImageDto>();
            foreach (var category in allCategories)
            {
                if (category.Name != null && category.Name.ToUpper().Contains(s.ToUpper()) && category.CategoryId.HasValue)
                {
                    var imageDto = _imageService.GetOneImageFromCategory(category.CategoryId.Value);
                    listOfFirstSearchedImages.Add(imageDto);
                }
            }
            viewModel.DisplayImagesList = listOfFirstSearchedImages;
            viewModel.SelectedCategory = CategoryDto.CreateEmpty();
            viewModel.SelectedCategory.Name = "SearchFor: " + s;
            viewModel.SelectedCategory.UrlCategoryPath = "/Search";
            
            if (listOfFirstSearchedImages.Count == 0)
                Log.Warning("Hittade inget vid sökning: '" + s + "'");
        }
        return View(viewModel);
    }
}
