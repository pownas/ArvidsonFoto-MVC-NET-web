using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.Interfaces;
using ArvidsonFoto.Core.Services;
using ArvidsonFoto.Core.ViewModels;
using ArvidsonFoto.Views.Shared;
using Microsoft.Extensions.Caching.Memory;

namespace ArvidsonFoto.Controllers;

public class SenastController(
    ArvidsonFotoCoreDbContext coreContext, 
    ILogger<ApiImageService> imageLogger,
    ILogger<ApiCategoryService> categoryLogger,
    IConfiguration configuration,
    IMemoryCache memoryCache) : Controller
{
    internal IApiImageService _imageService = new ApiImageService(imageLogger, coreContext, configuration, new ApiCategoryService(categoryLogger, coreContext, memoryCache));
    internal IApiCategoryService _categoryService = new ApiCategoryService(categoryLogger, coreContext, memoryCache);
    internal IPageCounterService _pageCounterService = new PageCounterService(coreContext);

    [Route("[controller]/{sortOrder}")]
    public IActionResult Index(string sortOrder, int? sida)
    {
        GalleryViewModel viewModel = new GalleryViewModel();
        int pageSize = 48;

        if (sida is null || sida < 1)
            sida = 1;

        if (sortOrder is null)
            sortOrder = "Fotograferad";

        viewModel.CurrentPage = (int)sida;

        if (sortOrder.Equals("Per kategori"))
        {
            ViewData["Title"] = "Per kategori";
            if (User?.Identity?.IsAuthenticated is false)
                _pageCounterService.AddPageCount("Senast-Per kategori");
            
            var categories = _categoryService.GetAll().OrderBy(c => c.Name).ToList();
            viewModel.AllImagesList = new List<Core.DTOs.ImageDto>();
            
            foreach (var category in categories)
            {
                if (category.CategoryId.HasValue)
                {
                    var imageDto = _imageService.GetOneImageFromCategory(category.CategoryId.Value);
                    imageDto.Name = category.Name ?? string.Empty;
                    imageDto.CategoryName = category.Name ?? string.Empty;
                    viewModel.AllImagesList.Add(imageDto);
                }
            }
            
            viewModel.TotalPages = (int)Math.Ceiling(viewModel.AllImagesList.Count / (decimal)pageSize);
            viewModel.DisplayImagesList = viewModel.AllImagesList
                .Skip((viewModel.CurrentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
        else if (sortOrder.Equals("Uppladdad"))
        {
            ViewData["Title"] = "Uppladdad";
            if (User?.Identity?.IsAuthenticated is false)
                _pageCounterService.AddPageCount("Senast-Uppladdad");
            
            // OPTIMIZED: Use GetLatestImageList with limit for current page
            int totalImages = _imageService.GetCountedAllImages();
            viewModel.TotalPages = (int)Math.Ceiling(totalImages / (decimal)pageSize);
            
            // Calculate how many images to fetch (current page + buffer for sorting)
            int limit = Math.Min(totalImages, pageSize * 10); // Fetch max 10 pages worth for sorting
            var images = _imageService.GetLatestImageList(limit);
            
            // Update each image with its category name
            foreach (var image in images)
            {
                if (image.CategoryId > 0)
                {
                    var categoryName = _categoryService.GetNameById(image.CategoryId);
                    image.Name = categoryName;
                    image.CategoryName = categoryName;
                }
            }
            
            // Sort and paginate
            viewModel.AllImagesList = images;
            viewModel.DisplayImagesList = images
                .Skip((viewModel.CurrentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
        else if (sortOrder.Equals("Fotograferad"))
        {
            ViewData["Title"] = "Fotograferad";
            if (User?.Identity?.IsAuthenticated is false)
                _pageCounterService.AddPageCount("Senast-Fotograferad");
            
            // OPTIMIZED: Get only the images needed (with some buffer for sorting)
            int totalImages = _imageService.GetCountedAllImages();
            viewModel.TotalPages = (int)Math.Ceiling(totalImages / (decimal)pageSize);
            
            // For "Fotograferad", we need to sort by DateImageTaken
            // Fetch enough images to cover current page (with buffer)
            int limit = Math.Min(totalImages, pageSize * 10); // Fetch max 10 pages worth
            var allImages = _imageService.GetAll()
                .OrderByDescending(i => i.DateImageTaken)
                .Take(limit)
                .ToList();
            
            // Update each image with its category name
            foreach (var image in allImages)
            {
                if (image.CategoryId > 0)
                {
                    var categoryName = _categoryService.GetNameById(image.CategoryId);
                    image.Name = categoryName;
                    image.CategoryName = categoryName;
                }
            }
            
            viewModel.AllImagesList = allImages;
            viewModel.DisplayImagesList = allImages
                .Skip((viewModel.CurrentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
        else
        {
            var url = Url.ActionContext.HttpContext;
            string visitedUrl = HttpRequestExtensions.GetRawUrl(url);
            Log.Information($"Redirect from page: {visitedUrl}, to page: /Senast/Fotograferad");

            return RedirectToAction("Index", new { sortOrder = "Fotograferad" });
        }

        viewModel.SelectedCategory = new Core.DTOs.CategoryDto { Name = sortOrder };
        viewModel.CurrentUrl = "/Senast/" + sortOrder;

        return View(viewModel);
    }
}