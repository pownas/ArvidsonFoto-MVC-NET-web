using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.Extensions;
using ArvidsonFoto.Core.Interfaces;
using ArvidsonFoto.Core.Services;
using ArvidsonFoto.Core.ViewModels;
using ArvidsonFoto.Views.Shared;
using Microsoft.Extensions.Caching.Memory;

namespace ArvidsonFoto.Controllers;

internal enum LatestSortOrder
{
    Fotograferad,
    Uppladdad,
    PerKategori
}

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
    [Route("/latest")]
    [Route("/latest/{sortOrder}")]
    public IActionResult Index(string sortOrder, int? sida)
    {
        GalleryViewModel viewModel = new();
        int pageSize = 48;

        if (sida is null or < 1)
        {
            sida = 1;
        }

        var selectedSortOrder = NormalizeSortOrder(sortOrder);
        if (selectedSortOrder is null)
        {
            var url = Url.ActionContext.HttpContext;
            string? visitedUrl = HttpRequestExtensions.GetRawUrl(url);
            Log.Information("Unknown latest sort order {SortOrder} from {Url}; redirecting to photographed.", sortOrder, visitedUrl);
            return RedirectToAction("Index", new { sortOrder = LatestSortOrder.Fotograferad.ToString() });
        }

        var latestSortOrder = selectedSortOrder.Value;
        var sortTitle = latestSortOrder switch
        {
            LatestSortOrder.Uppladdad => "Uppladdad",
            LatestSortOrder.PerKategori => "Per kategori",
            _ => "Fotograferad"
        };
        ViewData["Title"] = sortTitle;

        viewModel.CurrentPage = (int)sida;

        if (latestSortOrder == LatestSortOrder.PerKategori)
        {
            if (User?.Identity?.IsAuthenticated is false)
            {
                _pageCounterService.AddPageCount("senast-per kategori");
            }

            // OPTIMIZED: Use SQL-level query to get one image per category efficiently
            var categories = coreContext.TblMenus
                .Where(m => m.MenuCategoryId.HasValue)
                .OrderBy(m => m.MenuDisplayName)
                .ToList();

            viewModel.AllImagesList = [];

            // Get all category IDs that have images (including family and main family relationships)
            var categoriesWithImages = categories
                .Where(cat => coreContext.TblImages.Any(img =>
                    img.ImageCategoryId == cat.MenuCategoryId ||
                    img.ImageFamilyId == cat.MenuCategoryId ||
                    img.ImageMainFamilyId == cat.MenuCategoryId))
                .Select(cat => cat.MenuCategoryId)
                .ToList();
            var categoryUrlPaths = _categoryService.GetCategoryPathsBulk(
                categoriesWithImages.Where(categoryId => categoryId.HasValue).Select(categoryId => categoryId!.Value).ToList());

            // Fetch one image per category in a single optimized query
            foreach (var categoryId in categoriesWithImages)
            {
                var image = coreContext.TblImages
                    .Where(i => i.ImageCategoryId == categoryId ||
                               i.ImageFamilyId == categoryId ||
                               i.ImageMainFamilyId == categoryId)
                    .OrderByDescending(i => i.ImageUpdate)
                    .Select(i => new
                    {
                        i.ImageId,
                        i.ImageCategoryId,
                        i.ImageUrlName,
                        i.ImageDate,
                        i.ImageUpdate,
                        i.ImageDescription,
                        i.ImageDescriptionEn
                    })
                    .FirstOrDefault();

                if (image != null)
                {
                    var category = categories.FirstOrDefault(c => c.MenuCategoryId == categoryId);
                    var categoryName = category?.MenuDisplayName ?? string.Empty;
                    // Use display path with ÅÄÖ to match physical folder structure
                    var categoryPath = _categoryService.GetCategoryDisplayPathForImage(categoryId ?? 0);
                    var categoryUrlPath = categoryUrlPaths.GetValueOrDefault(categoryId ?? 0, string.Empty);
                    // Use the image's own category path for the actual file location,
                    // which may be in a subcategory folder (e.g. "Dagrovfåglar/Röd glada")
                    var imagePath = _categoryService.GetCategoryDisplayPathForImage(image.ImageCategoryId ?? 0);

                    viewModel.AllImagesList.Add(new Core.DTOs.ImageDto
                    {
                        ImageId = image.ImageId ?? 0,
                        CategoryId = image.ImageCategoryId ?? 0,
                        CategoryNameSv = categoryName,
                        CategoryNameEn = category?.MenuDisplayNameEn,
                        UrlImage = ImageMappingExtensions.ToPhysicalImageUrl(imagePath, image.ImageUrlName),
                        UrlCategorySv = $"bilder/{categoryUrlPath}",
                        DateImageTaken = image.ImageDate,
                        DateUploaded = image.ImageUpdate,
                        DescriptionSv = image.ImageDescription ?? string.Empty,
                        DescriptionEn = image.ImageDescriptionEn
                    });
                }
            }

            viewModel.TotalImageCount = viewModel.AllImagesList.Count;
            viewModel.TotalPages = (int)Math.Ceiling(viewModel.AllImagesList.Count / (decimal)pageSize);
            viewModel.DisplayImagesList = [.. viewModel.AllImagesList
                .Skip((viewModel.CurrentPage - 1) * pageSize)
                .Take(pageSize)];
        }
        else if (latestSortOrder == LatestSortOrder.Uppladdad)
        {
            if (User?.Identity?.IsAuthenticated is false)
            {
                _pageCounterService.AddPageCount("senast-uppladdad");
            }

            // OPTIMIZED: Get total count first
            int totalImages = coreContext.TblImages.Count();
            viewModel.TotalImageCount = totalImages;
            viewModel.TotalPages = (int)Math.Ceiling(totalImages / (decimal)pageSize);

            // OPTIMIZED: Apply sorting and pagination at SQL level
            var images = coreContext.TblImages
                .OrderByDescending(i => i.ImageUpdate)
                .Skip((viewModel.CurrentPage - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new
                {
                    i.ImageId,
                    i.ImageCategoryId,
                    i.ImageUrlName,
                    i.ImageDate,
                    i.ImageUpdate,
                    i.ImageDescription,
                    i.ImageDescriptionEn
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
            var categoryUrlPaths = _categoryService.GetCategoryPathsBulk(categoryIds);

            // Use bulk category name lookup to avoid N+1 queries
            var categoryNames = _categoryService.GetCategoryNamesBulk(categoryIds);

            viewModel.DisplayImagesList = [.. images.Select(image =>
            {
                var categoryId = image.ImageCategoryId ?? 0;
                var categoryPath = categoryDisplayPaths.GetValueOrDefault(categoryId, string.Empty);
                var categoryUrlPath = categoryUrlPaths.GetValueOrDefault(categoryId, string.Empty);
                var categoryName = categoryNames.GetValueOrDefault(categoryId, string.Empty);

                return new Core.DTOs.ImageDto
                {
                    ImageId = image.ImageId ?? 0,
                    CategoryId = categoryId,
                    CategoryNameSv = categoryName,
                    CategoryNameEn = _categoryService.GetById(categoryId).NameEn,
                    UrlImage = ImageMappingExtensions.ToPhysicalImageUrl(categoryPath, image.ImageUrlName),
                    UrlCategorySv = $"bilder/{categoryUrlPath}",
                    DateImageTaken = image.ImageDate,
                    DateUploaded = image.ImageUpdate,
                    DescriptionSv = image.ImageDescription ?? string.Empty,
                    DescriptionEn = image.ImageDescriptionEn
                };
            })];

            viewModel.AllImagesList = []; // Don't load all images
        }
        else
        {
            if (User?.Identity?.IsAuthenticated is false)
            {
                _pageCounterService.AddPageCount("senast-fotograferad");
            }

            // OPTIMIZED: Get total count first
            int totalImages = coreContext.TblImages.Count();
            viewModel.TotalImageCount = totalImages;
            viewModel.TotalPages = (int)Math.Ceiling(totalImages / (decimal)pageSize);

            // OPTIMIZED: Apply sorting and pagination at SQL level
            var images = coreContext.TblImages
                .OrderByDescending(i => i.ImageDate)
                .ThenByDescending(i => i.ImageUpdate)
                .Skip((viewModel.CurrentPage - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new
                {
                    i.ImageId,
                    i.ImageCategoryId,
                    i.ImageUrlName,
                    i.ImageDate,
                    i.ImageUpdate,
                    i.ImageDescription,
                    i.ImageDescriptionEn
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
            var categoryUrlPaths = _categoryService.GetCategoryPathsBulk(categoryIds);

            var categoryNames = _categoryService.GetCategoryNamesBulk(categoryIds);

            viewModel.DisplayImagesList = [.. images.Select(image =>
            {
                var categoryId = image.ImageCategoryId ?? 0;
                var categoryPath = categoryDisplayPaths.GetValueOrDefault(categoryId, string.Empty);
                var categoryUrlPath = categoryUrlPaths.GetValueOrDefault(categoryId, string.Empty);
                var categoryName = categoryNames.GetValueOrDefault(categoryId, string.Empty);

                return new Core.DTOs.ImageDto
                {
                    ImageId = image.ImageId ?? 0,
                    CategoryId = categoryId,
                    CategoryNameSv = categoryName,
                    CategoryNameEn = _categoryService.GetById(categoryId).NameEn,
                    UrlImage = ImageMappingExtensions.ToPhysicalImageUrl(categoryPath, image.ImageUrlName),
                    UrlCategorySv = $"bilder/{categoryUrlPath}",
                    DateImageTaken = image.ImageDate,
                    DateUploaded = image.ImageUpdate,
                    DescriptionSv = image.ImageDescription ?? string.Empty,
                    DescriptionEn = image.ImageDescriptionEn
                };
            })];

            viewModel.AllImagesList = []; // Don't load all images
        }
        viewModel.SelectedCategory = new Core.DTOs.CategoryDto { NameSv = sortTitle };
        viewModel.CurrentUrl = HttpContext?.Request.Path.Value ??
            ("/senast/" + sortTitle);

        return View(viewModel);
    }

    private static LatestSortOrder? NormalizeSortOrder(string? sortOrder)
    {
        return sortOrder?.Trim().ToLowerInvariant() switch
        {
            null or "" => LatestSortOrder.Fotograferad,
            "photographed" or "fotograferad" => LatestSortOrder.Fotograferad,
            "uploaded" or "uppladdad" => LatestSortOrder.Uppladdad,
            "by-category" or "per kategori" or "perkategori" => LatestSortOrder.PerKategori,
            _ => null
        };
    }
}