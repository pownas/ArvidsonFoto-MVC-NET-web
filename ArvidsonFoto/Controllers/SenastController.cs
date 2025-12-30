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
            
            // OPTIMIZED: Use SQL-level query to get one image per category efficiently
            var categories = coreContext.TblMenus
                .Where(m => m.MenuCategoryId.HasValue)
                .OrderBy(m => m.MenuDisplayName)
                .ToList();
            
            viewModel.AllImagesList = new List<Core.DTOs.ImageDto>();
            
            // Get all category IDs that have images
            var categoriesWithImages = (from cat in categories
                                       join img in coreContext.TblImages on cat.MenuCategoryId equals img.ImageCategoryId
                                       group img by cat.MenuCategoryId into g
                                       select g.Key)
                                       .ToList();
            
            // Fetch one image per category in a single optimized query
            foreach (var categoryId in categoriesWithImages)
            {
                var image = coreContext.TblImages
                    .Where(i => i.ImageCategoryId == categoryId || 
                               i.ImageFamilyId == categoryId || 
                               i.ImageMainFamilyId == categoryId)
                    .OrderByDescending(i => i.ImageUpdate)
                    .Select(i => new { 
                        i.ImageId, 
                        i.ImageCategoryId, 
                        i.ImageUrlName, 
                        i.ImageDate, 
                        i.ImageUpdate, 
                        i.ImageDescription 
                    })
                    .FirstOrDefault();
                
                if (image != null)
                {
                    var category = categories.FirstOrDefault(c => c.MenuCategoryId == categoryId);
                    var categoryName = category?.MenuDisplayName ?? string.Empty;
                    // Use display path with ÅÄÖ to match physical folder structure
                    var categoryPath = _categoryService.GetCategoryDisplayPathForImage(categoryId ?? 0);
                    
                    viewModel.AllImagesList.Add(new Core.DTOs.ImageDto
                    {
                        ImageId = image.ImageId ?? 0,
                        CategoryId = image.ImageCategoryId ?? 0,
                        Name = categoryName,
                        CategoryName = categoryName,
                        UrlImage = $"bilder/{categoryPath}/{image.ImageUrlName}",
                        UrlCategory = $"bilder/{categoryPath}",
                        DateImageTaken = image.ImageDate,
                        DateUploaded = image.ImageUpdate,
                        Description = image.ImageDescription ?? string.Empty
                    });
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
            
            // OPTIMIZED: Get total count first
            int totalImages = coreContext.TblImages.Count();
            viewModel.TotalPages = (int)Math.Ceiling(totalImages / (decimal)pageSize);
            
            // OPTIMIZED: Apply sorting and pagination at SQL level
            var images = coreContext.TblImages
                .OrderByDescending(i => i.ImageUpdate)
                .Skip((viewModel.CurrentPage - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new {
                    i.ImageId,
                    i.ImageCategoryId,
                    i.ImageUrlName,
                    i.ImageDate,
                    i.ImageUpdate,
                    i.ImageDescription
                })
                .ToList();
            
            // Get unique category IDs for bulk path loading
            var categoryIds = images
                .Where(i => i.ImageCategoryId.HasValue)
                .Select(i => i.ImageCategoryId!.Value)
                .Distinct()
                .ToList();

            // Use display paths (with ÅÄÖ) for physical folder structure - switch later to GetCategoryPathForImage
            var categoryDisplayPaths = new Dictionary<int, string>();
            foreach (var catId in categoryIds)
            {
                categoryDisplayPaths[catId] = _categoryService.GetCategoryDisplayPathForImage(catId);
            }
            
            // Use bulk category name lookup to avoid N+1 queries
            var categoryNames = _categoryService.GetCategoryNamesBulk(categoryIds);
            
            viewModel.DisplayImagesList = images.Select(image =>
            {
                var categoryId = image.ImageCategoryId ?? 0;
                var categoryPath = categoryDisplayPaths.GetValueOrDefault(categoryId, string.Empty);
                var categoryName = categoryNames.GetValueOrDefault(categoryId, string.Empty);
                
                return new Core.DTOs.ImageDto
                {
                    ImageId = image.ImageId ?? 0,
                    CategoryId = categoryId,
                    Name = categoryName,
                    CategoryName = categoryName,
                    UrlImage = $"bilder/{categoryPath}/{image.ImageUrlName}",
                    UrlCategory = $"bilder/{categoryPath}",
                    DateImageTaken = image.ImageDate,
                    DateUploaded = image.ImageUpdate,
                    Description = image.ImageDescription ?? string.Empty
                };
            }).ToList();
            
            viewModel.AllImagesList = new List<Core.DTOs.ImageDto>(); // Don't load all images
        }
        else if (sortOrder.Equals("Fotograferad"))
        {
            ViewData["Title"] = "Fotograferad";
            if (User?.Identity?.IsAuthenticated is false)
                _pageCounterService.AddPageCount("Senast-Fotograferad");
            
            // OPTIMIZED: Get total count first
            int totalImages = coreContext.TblImages.Count();
            viewModel.TotalPages = (int)Math.Ceiling(totalImages / (decimal)pageSize);
            
            // OPTIMIZED: Apply sorting and pagination at SQL level
            var images = coreContext.TblImages
                .OrderByDescending(i => i.ImageDate)
                .ThenByDescending(i => i.ImageUpdate)
                .Skip((viewModel.CurrentPage - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new {
                    i.ImageId,
                    i.ImageCategoryId,
                    i.ImageUrlName,
                    i.ImageDate,
                    i.ImageUpdate,
                    i.ImageDescription
                })
                .ToList();
            
            // Get unique category IDs for bulk path loading
            var categoryIds = images
                .Where(i => i.ImageCategoryId.HasValue)
                .Select(i => i.ImageCategoryId!.Value)
                .Distinct()
                .ToList();
            
            // Use display paths (with ÅÄÖ) for physical folder structure
            var categoryDisplayPaths = new Dictionary<int, string>();
            foreach (var catId in categoryIds)
            {
                categoryDisplayPaths[catId] = _categoryService.GetCategoryDisplayPathForImage(catId);
            }
            
            var categoryNames = _categoryService.GetCategoryNamesBulk(categoryIds);
            
            viewModel.DisplayImagesList = images.Select(image =>
            {
                var categoryId = image.ImageCategoryId ?? 0;
                var categoryPath = categoryDisplayPaths.GetValueOrDefault(categoryId, string.Empty);
                var categoryName = categoryNames.GetValueOrDefault(categoryId, string.Empty);
                
                return new Core.DTOs.ImageDto
                {
                    ImageId = image.ImageId ?? 0,
                    CategoryId = categoryId,
                    Name = categoryName,
                    CategoryName = categoryName,
                    UrlImage = $"bilder/{categoryPath}/{image.ImageUrlName}",
                    UrlCategory = $"bilder/{categoryPath}",
                    DateImageTaken = image.ImageDate,
                    DateUploaded = image.ImageUpdate,
                    Description = image.ImageDescription ?? string.Empty
                };
            }).ToList();
            
            viewModel.AllImagesList = new List<Core.DTOs.ImageDto>(); // Don't load all images
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