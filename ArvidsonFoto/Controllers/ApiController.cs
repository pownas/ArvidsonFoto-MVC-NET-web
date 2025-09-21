//namespace ArvidsonFoto.Controllers;

///// <summary>
///// API för ArvidsonFoto bildgalleri med hierarkiska kategorier
///// </summary>
//[ApiController]
//[Route("api")]
//[Produces("application/json")]
//public class ApiController(ArvidsonFotoCoreDbContext context) : ControllerBase
//{
//    private readonly string _imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Bilder");
//    private readonly IImageService _imageService = new ImageService(context);
//    private readonly ICategoryService _categoryService = new CategoryService(context);

//    /// <summary>
//    /// Hämtar alla bildfilnamn från bildkatalogen
//    /// </summary>
//    /// <returns>Lista med bildfilnamn</returns>
//    /// <response code="200">Returnerar listan med bilder</response>
//    /// <response code="404">Bildkatalogen hittades inte</response>
//    [HttpGet("images")]
//    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    public ActionResult<IEnumerable<string>> GetImages()
//    {
//        if (!Directory.Exists(_imagesPath))
//            return NotFound("Images directory not found.");

//        var images = Directory.GetFiles(_imagesPath)
//            .Select(Path.GetFileName)
//            .Where(f => f != null && (
//                f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
//                f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
//                f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
//                f.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)))
//            .ToList();

//        return Ok(images);
//    }

//    /// <summary>
//    /// Hämtar bilder från en huvudkategori (första nivån)
//    /// </summary>
//    /// <param name="subLevel1">Huvudkategorins namn</param>
//    /// <param name="page">Sidnummer för paginering (standard: 1)</param>
//    /// <param name="pageSize">Antal bilder per sida (standard: 48)</param>
//    /// <returns>Bilder från den angivna kategorin</returns>
//    /// <response code="200">Returnerar bilder från kategorin</response>
//    /// <response code="404">Kategorin hittades inte</response>
//    [HttpGet("bilder/{subLevel1}")]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    public ActionResult<object> GetImagesByCategory(string subLevel1, int page = 1, int pageSize = 48)
//    {
//        return GetImagesFromCategory(subLevel1, null, null, null, page, pageSize);
//    }

//    /// <summary>
//    /// Hämtar bilder från en underkategori (andra nivån)
//    /// </summary>
//    /// <param name="subLevel1">Huvudkategorins namn</param>
//    /// <param name="subLevel2">Underkategorins namn</param>
//    /// <param name="page">Sidnummer för paginering (standard: 1)</param>
//    /// <param name="pageSize">Antal bilder per sida (standard: 48)</param>
//    /// <returns>Bilder från den angivna underkategorin</returns>
//    /// <response code="200">Returnerar bilder från underkategorin</response>
//    /// <response code="404">Underkategorin hittades inte</response>
//    [HttpGet("bilder/{subLevel1}/{subLevel2}")]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    public ActionResult<object> GetImagesBySubCategory2(string subLevel1, string subLevel2, int page = 1, int pageSize = 48)
//    {
//        return GetImagesFromCategory(subLevel1, subLevel2, null, null, page, pageSize);
//    }

//    /// <summary>
//    /// Hämtar bilder från en tredje nivå-kategori
//    /// </summary>
//    /// <param name="subLevel1">Huvudkategorins namn</param>
//    /// <param name="subLevel2">Underkategorins namn</param>
//    /// <param name="subLevel3">Tredje nivåns kategorins namn</param>
//    /// <param name="page">Sidnummer för paginering (standard: 1)</param>
//    /// <param name="pageSize">Antal bilder per sida (standard: 48)</param>
//    /// <returns>Bilder från den angivna tredje nivå-kategorin</returns>
//    /// <response code="200">Returnerar bilder från tredje nivå-kategorin</response>
//    /// <response code="404">Tredje nivå-kategorin hittades inte</response>
//    [HttpGet("bilder/{subLevel1}/{subLevel2}/{subLevel3}")]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    public ActionResult<object> GetImagesBySubCategory3(string subLevel1, string subLevel2, string subLevel3, int page = 1, int pageSize = 48)
//    {
//        return GetImagesFromCategory(subLevel1, subLevel2, subLevel3, null, page, pageSize);
//    }

//    // GET: api/bilder/{subLevel1}/{subLevel2}/{subLevel3}/{subLevel4}
//    [HttpGet("bilder/{subLevel1}/{subLevel2}/{subLevel3}/{subLevel4}")]
//    public ActionResult<object> GetImagesBySubCategory4(string subLevel1, string subLevel2, string subLevel3, string subLevel4, int page = 1, int pageSize = 48)
//    {
//        return GetImagesFromCategory(subLevel1, subLevel2, subLevel3, subLevel4, page, pageSize);
//    }

//    // GET: api/categories - Get all available categories
//    [HttpGet("categories")]
//    public ActionResult<object> GetCategories()
//    {
//        try
//        {
//            var categories = _categoryService.GetAll().OrderBy(c => c.MenuText).Select(c => new
//            {
//                id = c.MenuId,
//                name = c.MenuText,
//                urlName = c.MenuUrltext,
//                mainId = c.MenuMainId
//            }).ToList();

//            return Ok(new { categories, total = categories.Count });
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, new { error = "Failed to retrieve categories", message = ex.Message });
//        }
//    }

//    // Helper method to get images from category (mirrors BilderController logic)
//    private ActionResult<object> GetImagesFromCategory(string subLevel1, string subLevel2, string subLevel3, string subLevel4, int page, int pageSize)
//    {
//        try
//        {
//            // Apply the same string replacement logic as BilderController
//            if (subLevel4 is not null)
//                subLevel4 = SharedStaticFunctions.ReplaceAAO(subLevel4);
//            if (subLevel3 is not null)
//                subLevel3 = SharedStaticFunctions.ReplaceAAO(subLevel3);
//            if (subLevel2 is not null)
//                subLevel2 = SharedStaticFunctions.ReplaceAAO(subLevel2);
//            if (subLevel1 is not null)
//                subLevel1 = SharedStaticFunctions.ReplaceAAO(subLevel1);

//            // Determine which level to use (same logic as BilderController)
//            string categoryName;
//            string currentUrl;

//            if (subLevel4 is not null)
//            {
//                categoryName = subLevel4;
//                currentUrl = $"/api/bilder/{subLevel1}/{subLevel2}/{subLevel3}/{subLevel4}";
//            }
//            else if (subLevel3 is not null)
//            {
//                categoryName = subLevel3;
//                currentUrl = $"/api/bilder/{subLevel1}/{subLevel2}/{subLevel3}";
//            }
//            else if (subLevel2 is not null)
//            {
//                categoryName = subLevel2;
//                currentUrl = $"/api/bilder/{subLevel1}/{subLevel2}";
//            }
//            else
//            {
//                categoryName = subLevel1;
//                currentUrl = $"/api/bilder/{subLevel1}";
//            }

//            // Get category and images
//            var category = _categoryService.GetByName(categoryName);
//            if (category == null)
//            {
//                return NotFound(new { error = "Category not found", categoryName });
//            }

//            var allImages = _imageService.GetAllImagesByCategoryID(_categoryService.GetIdByName(categoryName))
//                .OrderByDescending(i => i.ImageId)
//                .OrderByDescending(i => i.ImageDate)
//                .ToList();

//            // Apply pagination
//            var totalImages = allImages.Count;
//            var totalPages = (int)Math.Ceiling(totalImages / (decimal)pageSize);
//            var currentPage = Math.Max(1, page);
//            var skip = (currentPage - 1) * pageSize;

//            var pagedImages = allImages.Skip(skip).Take(pageSize).Select(img => new
//            {
//                id = img.ImageId,
//                url = img.ImageUrl,
//                title = img.ImageArt,
//                description = img.ImageDescription,
//                date = img.ImageDate,
//                imageUrl = $"/api/image/{img.ImageUrl}",
//                thumbnailUrl = $"/api/image/{img.ImageUrl}" // You might want to add thumbnail logic
//            }).ToList();

//            return Ok(new
//            {
//                category = new
//                {
//                    id = category.MenuId,
//                    name = category.MenuText,
//                    urlName = category.MenuUrltext
//                },
//                images = pagedImages,
//                pagination = new
//                {
//                    currentPage,
//                    totalPages,
//                    totalImages,
//                    pageSize,
//                    hasNextPage = currentPage < totalPages,
//                    hasPreviousPage = currentPage > 1
//                },
//                url = currentUrl
//            });
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, new { error = "Failed to retrieve images", message = ex.Message });
//        }
//    }

//    // GET: api/search?s={searchTerm}
//    [HttpGet("search")]
//    public ActionResult<object> Search(string s)
//    {
//        try
//        {
//            if (string.IsNullOrWhiteSpace(s))
//            {
//                return Ok(new 
//                { 
//                    searchTerm = "",
//                    categories = new List<object>(),
//                    total = 0,
//                    message = "No search term provided"
//                });
//            }

//            // Apply same search logic as BilderController
//            s = s.Trim().Replace("+", " ");
            
//            var allCategories = _categoryService.GetAll().OrderBy(c => c.MenuText).ToList();
//            var matchingCategories = new List<object>();

//            foreach (var category in allCategories)
//            {
//                if (category.MenuText.ToUpper().Contains(s.ToUpper()))
//                {
//                    var firstImage = _imageService.GetOneImageFromCategory(category.MenuId);
//                    if (firstImage != null)
//                    {
//                        matchingCategories.Add(new
//                        {
//                            category = new
//                            {
//                                id = category.MenuId,
//                                name = category.MenuText,
//                                urlName = category.MenuUrltext,
//                                mainId = category.MenuMainId
//                            },
//                            firstImage = new
//                            {
//                                id = firstImage.ImageId,
//                                url = firstImage.ImageUrl,
//                                description = firstImage.ImageDescription,
//                                date = firstImage.ImageDate,
//                                imageUrl = $"/api/image/{firstImage.ImageUrl}"
//                            }
//                        });
//                    }
//                }
//            }

//            return Ok(new
//            {
//                searchTerm = s,
//                categories = matchingCategories,
//                total = matchingCategories.Count
//            });
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, new { error = "Search failed", message = ex.Message });
//        }
//    }

//    /// <summary>
//    /// Hämtar en bild baserat på database-ID med korrekt hierarkisk sökväg
//    /// </summary>
//    /// <param name="imageId">Bildens ID i databasen</param>
//    /// <param name="thumbnail">True för att hämta miniatyrbild (standard: false)</param>
//    /// <returns>Bildfilen</returns>
//    /// <response code="200">Returnerar bildfilen</response>
//    /// <response code="404">Bilden hittades inte</response>
//    /// <response code="500">Serverfel</response>
//    [HttpGet("image/{imageId:int}")]
//    [Produces("image/jpeg", "image/png", "image/gif", "image/webp")]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//    public IActionResult GetImageById(int imageId, bool thumbnail = false)
//    {
//        try
//        {
//            // Get image from database
//            var images = _imageService.GetAll();
//            var image = images.FirstOrDefault(i => i.ImageId == imageId);
            
//            if (image == null)
//                return NotFound(new { error = "Image not found", imageId });

//            // Build the hierarchical path just like in _Gallery.cshtml
//            var imagePath = BuildImagePath(image, thumbnail);
            
//            if (!System.IO.File.Exists(imagePath))
//                return NotFound(new { error = "Image file not found", path = imagePath });

//            return ServeImageFile(imagePath);
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, new { error = "Failed to retrieve image", message = ex.Message });
//        }
//    }

//    /// <summary>
//    /// Hämtar en bild baserat på filnamn (URL) med korrekt hierarkisk sökväg
//    /// </summary>
//    /// <param name="imageUrl">Bildens filnamn (utan filändelse)</param>
//    /// <param name="thumbnail">True för att hämta miniatyrbild (standard: false)</param>
//    /// <returns>Bildfilen</returns>
//    /// <response code="200">Returnerar bildfilen</response>
//    /// <response code="404">Bilden hittades inte</response>
//    /// <response code="500">Serverfel</response>
//    [HttpGet("image/by-url/{imageUrl}")]
//    [Produces("image/jpeg", "image/png", "image/gif", "image/webp")]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//    public IActionResult GetImageByUrl(string imageUrl, bool thumbnail = false)
//    {
//        try
//        {
//            // Find image by URL in database
//            var images = _imageService.GetAll();
//            var image = images.FirstOrDefault(i => i.ImageUrl.Equals(imageUrl, StringComparison.OrdinalIgnoreCase));
            
//            if (image == null)
//                return NotFound(new { error = "Image not found", imageUrl });

//            // Build the hierarchical path
//            var imagePath = BuildImagePath(image, thumbnail);
            
//            if (!System.IO.File.Exists(imagePath))
//                return NotFound(new { error = "Image file not found", path = imagePath });

//            return ServeImageFile(imagePath);
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, new { error = "Failed to retrieve image", message = ex.Message });
//        }
//    }

//    // GET: api/image/{filename} - Legacy method for direct file access
//    [HttpGet("image/{filename}")]
//    public IActionResult GetImage(string filename)
//    {
//        // Check if this is a request for a database image by removing extensions
//        var baseFilename = Path.GetFileNameWithoutExtension(filename);
//        var isThumbnail = filename.Contains(".thumb.");

//        // Try to find image in database first
//        var images = _imageService.GetAll();
//        var image = images.FirstOrDefault(i => i.ImageUrl.Equals(baseFilename, StringComparison.OrdinalIgnoreCase));
        
//        if (image != null)
//        {
//            // Found in database, use hierarchical path
//            var imagePath = BuildImagePath(image, isThumbnail);
            
//            if (System.IO.File.Exists(imagePath))
//                return ServeImageFile(imagePath);
//        }

//        // Fallback to direct file access in wwwroot/Bilder
//        var fallbackPath = Path.Combine(_imagesPath, filename);
        
//        if (!System.IO.File.Exists(fallbackPath))
//            return NotFound(new { error = "Image file not found", filename });

//        return ServeImageFile(fallbackPath);
//    }

//    // Helper method to build image path like in _Gallery.cshtml
//    private string BuildImagePath(TblImage image, bool thumbnail = false)
//    {
//        // Start with base images path
//        var pathParts = new List<string> { _imagesPath };

//        // Add hierarchical category structure
//        if (image.ImageHuvudfamilj.HasValue)
//        {
//            var huvudfamilj = _categoryService.GetNameById(image.ImageHuvudfamilj.Value);
//            if (!string.IsNullOrEmpty(huvudfamilj))
//                pathParts.Add(huvudfamilj);
//        }

//        if (image.ImageFamilj.HasValue)
//        {
//            var familj = _categoryService.GetNameById(image.ImageFamilj.Value);
//            if (!string.IsNullOrEmpty(familj))
//                pathParts.Add(familj);
//        }

//        // Add art (species) category
//        var art = _categoryService.GetNameById(image.ImageArt);
//        if (!string.IsNullOrEmpty(art))
//            pathParts.Add(art);

//        // Add filename with appropriate extension
//        var filename = thumbnail ? 
//            $"{image.ImageUrl}.thumb.jpg" : 
//            $"{image.ImageUrl}.jpg";
//        pathParts.Add(filename);

//        return Path.Combine(pathParts.ToArray());
//    }

//    // Helper method to serve image files with proper content type
//    // Helper method to serve image files with proper content type
//    private IActionResult ServeImageFile(string filePath)
//    {
//        var ext = Path.GetExtension(filePath).ToLowerInvariant();
//        var contentType = ext switch
//        {
//            ".jpg" or ".jpeg" => "image/jpeg",
//            ".png" => "image/png",
//            ".gif" => "image/gif",
//            ".webp" => "image/webp",
//            _ => "application/octet-stream"
//        };

//        var bytes = System.IO.File.ReadAllBytes(filePath);
//        return File(bytes, contentType);
//    }
//}