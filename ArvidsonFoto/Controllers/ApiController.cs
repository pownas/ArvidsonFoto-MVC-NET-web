namespace ArvidsonFoto.Controllers;

[ApiController]
[Route("api")]
public class ApiController(ArvidsonFotoDbContext context) : ControllerBase
{
    private readonly string _imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Bilder");
    private readonly IImageService _imageService = new ImageService(context);
    private readonly ICategoryService _categoryService = new CategoryService(context);

    // GET: api/images
    [HttpGet("images")]
    public ActionResult<IEnumerable<string>> GetImages()
    {
        if (!Directory.Exists(_imagesPath))
            return NotFound("Images directory not found.");

        var images = Directory.GetFiles(_imagesPath)
            .Select(Path.GetFileName)
            .Where(f => f != null && (
                f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                f.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)))
            .ToList();

        return Ok(images);
    }

    // GET: api/bilder/{subLevel1}
    [HttpGet("bilder/{subLevel1}")]
    public ActionResult<object> GetImagesByCategory(string subLevel1, int page = 1, int pageSize = 48)
    {
        return GetImagesFromCategory(subLevel1, null, null, null, page, pageSize);
    }

    // GET: api/bilder/{subLevel1}/{subLevel2}
    [HttpGet("bilder/{subLevel1}/{subLevel2}")]
    public ActionResult<object> GetImagesBySubCategory2(string subLevel1, string subLevel2, int page = 1, int pageSize = 48)
    {
        return GetImagesFromCategory(subLevel1, subLevel2, null, null, page, pageSize);
    }

    // GET: api/bilder/{subLevel1}/{subLevel2}/{subLevel3}
    [HttpGet("bilder/{subLevel1}/{subLevel2}/{subLevel3}")]
    public ActionResult<object> GetImagesBySubCategory3(string subLevel1, string subLevel2, string subLevel3, int page = 1, int pageSize = 48)
    {
        return GetImagesFromCategory(subLevel1, subLevel2, subLevel3, null, page, pageSize);
    }

    // GET: api/bilder/{subLevel1}/{subLevel2}/{subLevel3}/{subLevel4}
    [HttpGet("bilder/{subLevel1}/{subLevel2}/{subLevel3}/{subLevel4}")]
    public ActionResult<object> GetImagesBySubCategory4(string subLevel1, string subLevel2, string subLevel3, string subLevel4, int page = 1, int pageSize = 48)
    {
        return GetImagesFromCategory(subLevel1, subLevel2, subLevel3, subLevel4, page, pageSize);
    }

    // GET: api/categories - Get all available categories
    [HttpGet("categories")]
    public ActionResult<object> GetCategories()
    {
        try
        {
            var categories = _categoryService.GetAll().OrderBy(c => c.MenuText).Select(c => new
            {
                id = c.MenuId,
                name = c.MenuText,
                urlName = c.MenuUrltext,
                mainId = c.MenuMainId
            }).ToList();

            return Ok(new { categories, total = categories.Count });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve categories", message = ex.Message });
        }
    }

    // Helper method to get images from category (mirrors BilderController logic)
    private ActionResult<object> GetImagesFromCategory(string subLevel1, string subLevel2, string subLevel3, string subLevel4, int page, int pageSize)
    {
        try
        {
            // Apply the same string replacement logic as BilderController
            if (subLevel4 is not null)
                subLevel4 = SharedStaticFunctions.ReplaceAAO(subLevel4);
            if (subLevel3 is not null)
                subLevel3 = SharedStaticFunctions.ReplaceAAO(subLevel3);
            if (subLevel2 is not null)
                subLevel2 = SharedStaticFunctions.ReplaceAAO(subLevel2);
            if (subLevel1 is not null)
                subLevel1 = SharedStaticFunctions.ReplaceAAO(subLevel1);

            // Determine which level to use (same logic as BilderController)
            string categoryName;
            string currentUrl;

            if (subLevel4 is not null)
            {
                categoryName = subLevel4;
                currentUrl = $"/api/bilder/{subLevel1}/{subLevel2}/{subLevel3}/{subLevel4}";
            }
            else if (subLevel3 is not null)
            {
                categoryName = subLevel3;
                currentUrl = $"/api/bilder/{subLevel1}/{subLevel2}/{subLevel3}";
            }
            else if (subLevel2 is not null)
            {
                categoryName = subLevel2;
                currentUrl = $"/api/bilder/{subLevel1}/{subLevel2}";
            }
            else
            {
                categoryName = subLevel1;
                currentUrl = $"/api/bilder/{subLevel1}";
            }

            // Get category and images
            var category = _categoryService.GetByName(categoryName);
            if (category == null)
            {
                return NotFound(new { error = "Category not found", categoryName });
            }

            var allImages = _imageService.GetAllImagesByCategoryID(_categoryService.GetIdByName(categoryName))
                .OrderByDescending(i => i.ImageId)
                .OrderByDescending(i => i.ImageDate)
                .ToList();

            // Apply pagination
            var totalImages = allImages.Count;
            var totalPages = (int)Math.Ceiling(totalImages / (decimal)pageSize);
            var currentPage = Math.Max(1, page);
            var skip = (currentPage - 1) * pageSize;

            var pagedImages = allImages.Skip(skip).Take(pageSize).Select(img => new
            {
                id = img.ImageId,
                url = img.ImageUrl,
                description = img.ImageDescription,
                date = img.ImageDate,
                imageUrl = $"/api/image/{img.ImageUrl}",
                thumbnailUrl = $"/api/image/{img.ImageUrl}" // You might want to add thumbnail logic
            }).ToList();

            return Ok(new
            {
                category = new
                {
                    id = category.MenuId,
                    name = category.MenuText,
                    urlName = category.MenuUrltext
                },
                images = pagedImages,
                pagination = new
                {
                    currentPage,
                    totalPages,
                    totalImages,
                    pageSize,
                    hasNextPage = currentPage < totalPages,
                    hasPreviousPage = currentPage > 1
                },
                url = currentUrl
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve images", message = ex.Message });
        }
    }

    // GET: api/search?s={searchTerm}
    [HttpGet("search")]
    public ActionResult<object> Search(string s)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return Ok(new 
                { 
                    searchTerm = "",
                    categories = new List<object>(),
                    total = 0,
                    message = "No search term provided"
                });
            }

            // Apply same search logic as BilderController
            s = s.Trim().Replace("+", " ");
            
            var allCategories = _categoryService.GetAll().OrderBy(c => c.MenuText).ToList();
            var matchingCategories = new List<object>();

            foreach (var category in allCategories)
            {
                if (category.MenuText.ToUpper().Contains(s.ToUpper()))
                {
                    var firstImage = _imageService.GetOneImageFromCategory(category.MenuId);
                    if (firstImage != null)
                    {
                        matchingCategories.Add(new
                        {
                            category = new
                            {
                                id = category.MenuId,
                                name = category.MenuText,
                                urlName = category.MenuUrltext,
                                mainId = category.MenuMainId
                            },
                            firstImage = new
                            {
                                id = firstImage.ImageId,
                                url = firstImage.ImageUrl,
                                description = firstImage.ImageDescription,
                                date = firstImage.ImageDate,
                                imageUrl = $"/api/image/{firstImage.ImageUrl}"
                            }
                        });
                    }
                }
            }

            return Ok(new
            {
                searchTerm = s,
                categories = matchingCategories,
                total = matchingCategories.Count
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Search failed", message = ex.Message });
        }
    }

    // GET: api/image/{filename}
    [HttpGet("image/{filename}")]
    public IActionResult GetImage(string filename)
    {
        var filePath = Path.Combine(_imagesPath, filename);

        if (!System.IO.File.Exists(filePath))
            return NotFound();

        var ext = Path.GetExtension(filename).ToLowerInvariant();
        var contentType = ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            _ => "application/octet-stream"
        };

        var bytes = System.IO.File.ReadAllBytes(filePath);
        return File(bytes, contentType);
    }
}