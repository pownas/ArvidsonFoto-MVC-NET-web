using ArvidsonFoto.Core.DTOs;
using ArvidsonFoto.Core.Interfaces;
using ArvidsonFoto.Core.ViewModels;
using ArvidsonFoto.Views.Shared;
using System.Web;

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

        viewModel.CurrentPage = (int)sida;

        // URL decode and normalize the incoming parameters
        if (subLevel4 is not null)
        {
            subLevel4 = Uri.UnescapeDataString(subLevel4);
            subLevel4 = SharedStaticFunctions.ReplaceAAO(subLevel4);
        }
        if (subLevel3 is not null)
        {
            subLevel3 = Uri.UnescapeDataString(subLevel3);
            subLevel3 = SharedStaticFunctions.ReplaceAAO(subLevel3);
        }
        if (subLevel2 is not null)
        {
            subLevel2 = Uri.UnescapeDataString(subLevel2);
            subLevel2 = SharedStaticFunctions.ReplaceAAO(subLevel2);
        }
        if (subLevel1 is not null)
        {
            subLevel1 = Uri.UnescapeDataString(subLevel1);
            subLevel1 = SharedStaticFunctions.ReplaceAAO(subLevel1);
        }

        // Determine which category to load
        string categoryName = null;
        string currentUrl = null;
        
        if (subLevel4 is not null)
        {
            categoryName = subLevel4;
            currentUrl = "/Bilder/" + subLevel1 + "/" + subLevel2 + "/" + subLevel3 + "/" + subLevel4;
        }
        else if (subLevel3 is not null)
        {
            categoryName = subLevel3;
            currentUrl = "/Bilder/" + subLevel1 + "/" + subLevel2 + "/" + subLevel3;
        }
        else if (subLevel2 is not null)
        {
            categoryName = subLevel2;
            currentUrl = "/Bilder/" + subLevel1 + "/" + subLevel2;
        }
        else if (subLevel1 is not null)
        {
            categoryName = subLevel1;
            currentUrl = "/Bilder/" + subLevel1;
        }

        if (categoryName != null)
        {
            // Try to find category by name (case-insensitive via GetByName)
            var selectedCategory = _categoryService.GetByName(categoryName);
            
            // If not found by display name, try by URL segment with fallback
            if (selectedCategory == null || selectedCategory.CategoryId == null || selectedCategory.CategoryId == -1)
            {
                selectedCategory = _categoryService.GetByUrlSegmentWithFallback(categoryName);
            }
            
            if (selectedCategory == null || selectedCategory.CategoryId == null || selectedCategory.CategoryId == -1)
            {
                Log.Warning($"Invalid category requested: {currentUrl} (categoryName: {categoryName})");
                return NotFound();
            }
            
            viewModel.SelectedCategory = selectedCategory;
            viewModel.CurrentUrl = currentUrl;
            
            // OPTIMIZED: Use count method instead of loading all images into memory
            var totalImageCount = _imageService.GetCountedCategoryId(selectedCategory.CategoryId.Value);
            
            // Calculate pagination
            viewModel.TotalPages = (int)Math.Ceiling(totalImageCount / (decimal)pageSize);
            
            // OPTIMIZED: Get only the images for the current page with SQL-level sorting and pagination
            viewModel.DisplayImagesList = _imageService.GetImagesByCategoryIDPaginated(
                selectedCategory.CategoryId.Value, 
                viewModel.CurrentPage, 
                pageSize);
                
            // Set AllImagesList to empty list to save memory (we don't need all images in memory)
            viewModel.AllImagesList = new List<ImageDto>();
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

        if (viewModel.DisplayImagesList == null)
        {
            viewModel.DisplayImagesList = new List<ImageDto>();
        }

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
        ViewBag.SearchPerformed = !string.IsNullOrWhiteSpace(s); // Track if a search was actually performed
        
        if (string.IsNullOrWhiteSpace(s))
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
