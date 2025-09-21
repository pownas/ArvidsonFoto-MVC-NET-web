using ArvidsonFoto.Core.ApiResponses;
using ArvidsonFoto.Core.Attributes;
using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.DTOs;
using ArvidsonFoto.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ArvidsonFoto.Controllers.ApiControllers;

/// <summary>
/// Provides endpoints for managing images
/// </summary>
/// <remarks>This controller includes both public and secured endpoints. Secured endpoints require authentication
/// in production environments,  while public endpoints are accessible without authentication. Debug mode bypasses
/// authentication for secured endpoints.</remarks>
/// <inheritdoc cref="ImageApiController"/>
[ApiController]
[Route("api/image")]
[Route("api/bild")]
public class ImageApiController(ILogger<ImageApiController> logger,
    IApiImageService imageService,
    ArvidsonFotoCoreDbContext entityContext, //TODO: Bör bygga bort denna och enbart använda IApiImageService
    IApiCategoryService categoryService,
    IConfiguration configuration) : ControllerBase
{
    // TODO: Att använda senare om vi vill spara eller läsa bilder från filsystemet
    private readonly string _imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Bilder");

    /// <summary>
    /// Adds a new image to the system.
    /// </summary>
    /// <remarks>In production environments, this endpoint requires authentication and authorization. 
    /// In debug mode, authentication is bypassed.</remarks>
    /// <param name="image">The image entity to be added. Must contain valid data for all required fields.</param>
    /// <returns><see langword="true"/> if the image was successfully added; otherwise, <see langword="false"/>.</returns>
#if !DEBUG  // This endpoint is secured and requires authentication in production, but can be accessed without authentication in debug mode
    [Authorize]
#endif
    [SwaggerSecurityRequirement("cookieAuth")] // This will add the lock icon for this endpoint
    //[SwaggerOperation(Summary = "Secured endpoint", Description = "Requires authentication")]
    [HttpPost("AddImage")]
    public bool AddImage(ImageDto image)
    {
        logger.LogInformation("Image - AddImage called.");
        return imageService.AddImage(image);
    }

    /// <summary>
    /// Deletes an image identified by the specified ID.
    /// </summary>
    /// <remarks>This endpoint requires authentication in production environments. 
    /// In debug mode, authentication is not enforced.</remarks>
    /// <param name="imgId">The unique identifier of the image to delete. Must be a valid, existing image ID.</param>
    /// <returns><see langword="true"/> if the image was successfully deleted; otherwise, <see langword="false"/>. </returns>
    /// <exception cref="NotImplementedException">This method is not yet implemented.</exception>
#if !DEBUG  // This endpoint is secured and requires authentication in production, but can be accessed without authentication in debug mode
    [Authorize]
#endif
    [SwaggerSecurityRequirement("cookieAuth")] // This will add the lock icon for this endpoint
    //[SwaggerOperation(Summary = "Secured endpoint", Description = "Requires authentication")]
    [HttpDelete("DeleteImgId/{imgId}")]
    public bool DeleteImgId(int imgId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Retrieves a list of all images stored in the system.
    /// </summary>
    /// <remarks>This endpoint requires authentication in production environments and uses an API key for access. 
    /// In debug mode, authentication is not enforced. 
    /// The returned list contains all images available  in the database.</remarks>
    /// <returns>A list of <see cref="ImageDto"/> objects representing all images in the system.</returns>
#if !DEBUG  // This endpoint is secured and requires authentication in production, but can be accessed without authentication in debug mode
    [Authorize]
#endif
    [HttpGet("GetAll")]
    //[SwaggerOperation(Summary = "Secured endpoint - api_key", Description = "Requires authentication with api_key")]
    [SwaggerSecurityRequirement("api_key")] // This will add the lock icon for this endpoint
    public List<ImageDto> GetAll()
    {
        logger.LogInformation("Image - GetAll called.");
        return imageService.GetAll();
    }

    /// <summary>
    /// Retrieves a list of images associated with the specified category ID.
    /// </summary>
    /// <remarks>This is a public endpoint that does not require authentication. Use this method to fetch all
    /// images linked to a specific category.</remarks>
    /// <param name="categoryId">The unique identifier of the category for which images are to be retrieved. Must be a valid category ID.</param>
    /// <param name="sortBy">uploaded (when image was uploaded), imagetaken (when the image was taken) or categoryname (name of the category)</param>
    /// <param name="sortOrder">asc (alphabetically ascending order) or desc (reverse alphabetically descending order)</param>
    /// <param name="limit">number of images to get</param>
    /// <returns>A list of <see cref="ImageDto"/> objects representing the images in the specified category. Returns an empty
    /// list if no images are found for the given category.</returns>
    [AllowAnonymous]
    //[SwaggerOperation(Summary = "Public endpoint", Description = "No authentication required")]
    [HttpGet("ByCategoryId/{categoryId}")]
    public List<ImageDto> GetImagesByCategoryID(int categoryId = 13, [FromQuery] string sortBy = "uploaded", [FromQuery]string sortOrder = "asc", [FromQuery]int limit = 48)
    {
        logger.LogInformation("Image - GetAllImagesByCategoryID called.");
        return imageService.GetImagesByCategoryID(categoryId);
    }

    private string GetCategoryPath(int categoryId)
    {
        try
        {
            if (categoryId <= 0)
            {
                logger.LogInformation("Invalid category ID provided to GetCategoryPath: {CategoryId}", categoryId);
                return string.Empty;
            }
            
            var category = categoryService.GetById(categoryId);
            
            if (category == null || category.CategoryId <= 0)
                return string.Empty;
                
            var path = category.UrlCategoryPath?.ToLower() ?? string.Empty;
            
            // Get parent categories if any
            if (category.ParentCategoryId.HasValue && category.ParentCategoryId.Value > 0)
            {
                var parent = categoryService.GetById(category.ParentCategoryId.Value);
                if (parent != null && parent.CategoryId > 0)
                {
                    path = (parent.UrlCategoryPath?.ToLower() ?? "") + "/" + path;
                    
                    // Get grandparent if any
                    if (parent.ParentCategoryId.HasValue && parent.ParentCategoryId.Value > 0)
                    {
                        var grandparent = categoryService.GetById(parent.ParentCategoryId.Value);
                        if (grandparent != null && grandparent.CategoryId > 0)
                        {
                            path = (grandparent.UrlCategoryPath?.ToLower() ?? "") + "/" + path;
                        }
                    }
                }
            }
            
            return path + "/";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetCategoryPath for categoryId {CategoryId}", categoryId);
            return string.Empty;
        }
    }

    /// <summary>
    /// Retrieves an image record by its unique identifier.
    /// </summary>
    /// <remarks>This is a public endpoint that does not require authentication.</remarks>
    /// <param name="imageId">The unique identifier of the image to retrieve. Must be a positive integer.</param>
    /// <returns>The <see cref="ImageDto"/> object corresponding to the specified <paramref name="imageId"/>. Returns <see
    /// langword="null"/> if no image with the given identifier exists.</returns>
    [AllowAnonymous]
    //[SwaggerOperation(Summary = "Public endpoint", Description = "No authentication required")]
    [HttpGet("{imageId}")]
    [HttpGet("GetById/{imageId}")]
    public ImageDto GetById(int imageId = 2)
    {
        logger.LogInformation($"Image - GetById called with id: {imageId}.");
        return imageService.GetById(imageId);
    }

    /// <summary>
    /// Retrieves the ID of the most recently added image.
    /// </summary>
    /// <remarks>This method calls the underlying image service to fetch the last image ID.  Ensure that the
    /// image service is properly configured and the database contains image records.</remarks>
    /// <returns>The ID of the last image added to the system. Returns 0 if no images exist.</returns>
    [HttpGet("GetImageLastId")]
    public int GetImageLastId()
    {
        logger.LogInformation("Image - GetImageLastId called.");
        return imageService.GetImageLastId();
    }

    /// <summary>
    /// Retrieves a single image from the specified category.
    /// </summary>
    /// <remarks>This is a public endpoint that does not require authentication. Use this method to fetch one
    /// image from a specific category for display or processing.</remarks>
    /// <param name="categoryId">The ID of the category from which to retrieve an image. Must be a valid category ID.</param>
    /// <returns>An instance of <see cref="ImageDto"/> representing the image retrieved from the specified category. If no image
    /// is found, the method may return null.</returns>
    [AllowAnonymous]
    //[SwaggerOperation(Summary = "Public endpoint", Description = "No authentication required")]
    [HttpGet("GetOneImageFromCategory/{categoryId}")]
    public ImageDto GetOneImageFromCategory(int categoryId= 13)
    {
        logger.LogInformation("Image - GetOneImageFromCategory called.");
        return imageService.GetOneImageFromCategory(categoryId);
    }

    /// <summary>
    /// Retrieves a specified number of random images from the database.
    /// </summary>
    /// <remarks>This is a public endpoint that does not require authentication. Use this method to fetch a
    /// random subset of images for display or processing.</remarks>
    /// <param name="count">The number of random images to retrieve. Must be a positive integer.</param>
    /// <returns>A list of <see cref="ImageDto"/> objects representing the randomly selected images. If no images are available,
    /// the list will be empty.</returns>
    [AllowAnonymous]
    //[SwaggerOperation(Summary = "Public endpoint", Description = "No authentication required")]
    [HttpGet("GetRandomNumberOfImages/{count}")]
    public List<ImageDto> GetRandomNumberOfImages(int count = 3)
    {
        logger.LogInformation("Image - GetRandomNumberOfImages called.");
        return imageService.GetRandomNumberOfImages(count);
    }

    /// <summary>
    /// Retrieves the latest images from the database.
    /// </summary>
    /// <remarks>This is a public endpoint that does not require authentication. Use this method to fetch the
    /// most recently uploaded images for display on the home page or recent images section.</remarks>
    /// <param name="count">The number of latest images to retrieve. Must be a positive integer. Default is 6.</param>
    /// <returns>A list of <see cref="ImageDto"/> objects representing the latest images ordered by upload date. If no images are available,
    /// the list will be empty.</returns>
    [AllowAnonymous]
    [HttpGet("Latest")]
    [ProducesResponseType<List<ImageDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public ActionResult<List<ImageDto>> GetLatestImages([FromQuery] int count = 6)
    {
        try
        {
            if (count <= 0 || count > 100)
            {
                return Problem(
                    title: "Invalid Parameter",
                    detail: "Count must be between 1 and 100.",
                    statusCode: StatusCodes.Status400BadRequest,
                    type: "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                );
            }

            logger.LogInformation("Image - GetLatestImages called with count: {Count}", count);
            var result = imageService.GetLatestImageList(count);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving latest images with count: {Count}", count);
            // Let the global exception handler deal with it
            throw;
        }
    }

    /// <summary>
    /// Updates the properties of an existing image in the database.
    /// </summary>
    /// <remarks>This method requires authentication in production environments. It validates the input and
    /// updates the image's properties if the image exists in the database.</remarks>
    /// <param name="image">The data transfer object containing the updated properties of the image. The <see
    /// cref="UploadImageInputDto.ImageId"/> must be greater than 0.</param>
    /// <returns><see langword="true"/> if the image was successfully updated; otherwise, <see langword="false"/> if the input is
    /// invalid or the image does not exist.</returns>
#if !DEBUG  // This endpoint is secured and requires authentication in production, but can be accessed without authentication in debug mode
    [Authorize]
#endif
    //[SwaggerOperation(Summary = "Secured endpoint", Description = "Requires authentication")]
    [SwaggerSecurityRequirement("cookieAuth")] // This will add the lock icon for this endpoint
    [HttpPut("UpdateImage")]
    public bool UpdateImage(UploadImageInputDto image)
    {
        // Validate the input
        if (image == null || image.ImageId <= 0)
        {
            return false; // Invalid input
        }

        // Find the existing image in the database
        var existingImage = entityContext.TblImages.Find(image.ImageId);
        if (existingImage == null)
        {
            return false; // Image not found
        }

        // Update the image properties
        existingImage.ImageMainFamilyId = image.ImageHuvudfamilj;
        existingImage.ImageFamilyId = image.ImageFamilj;
        //existingImage.ImageHuvudfamiljNamn = image.ImageHuvudfamiljNamn;
        //existingImage.ImageFamiljNamn = image.ImageFamiljNamn;

        // Save changes to the database
        entityContext.SaveChanges();

        return true; // Update successful
    }

    /// <summary>
    /// Retrieves a list of images associated with a category path of multiple levels.
    /// </summary>
    /// <remarks>
    /// This endpoint handles multi-level category paths like "Faglar/Vadare/Pipare/Kustpipare".
    /// It will find the most specific category in the path and return its images.
    /// </remarks>
    /// <param name="categoryPath">The path with multiple category segments (e.g., "Faglar/Vadare/Pipare/Kustpipare")</param>
    /// <param name="sortBy">uploaded (when image was uploaded), imagetaken (when the image was taken) or categoryname (name of the category)</param>
    /// <param name="sortOrder">asc (alphabetically ascending order) or desc (reverse alphabetically descending order)</param>
    /// <param name="limit">number of images to get</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>A <see cref="ImageListResponse"/> with a list of <see cref="ImageDto"/> objects representing the images in the specified category path</returns>
    [AllowAnonymous]
    [HttpGet("/api/Bilder/{*categoryPath}")]
    [HttpGet("Bilder/{*categoryPath}")]
    [HttpGet("ByPath/{*categoryPath}")]
    [HttpGet("ByPath/Bilder/{*categoryPath}")]
    [ProducesResponseType<ImageListResponse>(StatusCodes.Status200OK)]
    public IActionResult GetImagesByCategoryPath(
        string categoryPath, 
        [FromQuery] string sortBy = "uploaded", 
        [FromQuery] string sortOrder = "desc", 
        [FromQuery] int limit = 48,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Check for cancellation early
            cancellationToken.ThrowIfCancellationRequested();
            
            if (string.IsNullOrEmpty(categoryPath))
            {
                return BadRequest("Category path cannot be empty");
            }
            // Decode the URL-encoded path
            categoryPath = Uri.UnescapeDataString(categoryPath);
            
            logger.LogInformation("Image - GetImagesByCategoryPath called with path: {CategoryPath}", categoryPath);
            
            // Split the path into segments
            string[] segments = categoryPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            
            if (segments.Length == 0)
            {
                return BadRequest("Invalid category path format");
            }

            // Start with the first segment and traverse the category hierarchy
            int? currentCategoryId = null;
            var matchingChildCategory = (CategoryDto)null;

            foreach (var segment in segments)
            {
                // Check for cancellation during path resolution
                cancellationToken.ThrowIfCancellationRequested();
                
                // If this is the first segment, find the main category
                if (currentCategoryId == null)
                {
                    currentCategoryId = categoryService.GetIdByName(segment);
                    if (currentCategoryId <= 0)
                    {
                        return NotFound($"Category '{segment}' not found");
                    }
                }
                else
                {
                    // Find the child category under the current parent
                    var children = categoryService.GetChildrenByParentId(currentCategoryId.Value);
                    matchingChildCategory = children.FirstOrDefault(c => 
                        c.UrlCategoryPath!.Equals(segment, StringComparison.OrdinalIgnoreCase) || 
                        c.Name!.Equals(segment, StringComparison.OrdinalIgnoreCase));
                    
                    if (matchingChildCategory == null)
                    {
                        return NotFound($"Category '{segment}' not found under parent category");
                    }
                    
                    currentCategoryId = matchingChildCategory.CategoryId;
                }
            }
            
            // Check for cancellation before expensive image loading
            cancellationToken.ThrowIfCancellationRequested();
            
            // Get images for the final category ID
            if (currentCategoryId.HasValue && currentCategoryId.Value > 0)
            {
                var images = imageService.GetImagesByCategoryID(currentCategoryId.Value);
                
                // Apply sorting and limiting
                var sortedImages = ApplySortingAndLimit(images, sortBy, sortOrder, limit);

                var totalCategoryImageCount = imageService.GetCountedCategoryId(currentCategoryId.Value);

                var response = new ImageListResponse
                {
                    CategoryId = currentCategoryId.Value,
                    CategoryName = $"{matchingChildCategory?.Name ?? "Unknown"}",
                    CategoryUrl =  $"{matchingChildCategory?.UrlCategoryPathFull ?? "Unknown"}",
                    CategoryUrlWithAAO = Uri.EscapeDataString(categoryPath),
                    ImageCategoryTotalCount = totalCategoryImageCount,
                    ImageResultCount = sortedImages.Count,
                    Images = sortedImages,
                    QueryLimit = limit,
                    QuerySortBy = sortBy,
                    QuerySortOrder = sortOrder,
                };
                return Ok(response);
            }
            
            return NotFound("Category path could not be resolved to a valid category");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing category path: {CategoryPath}", categoryPath);
            return StatusCode(500, "An error occurred while processing the request");
        }
    }

    /// <summary>
    /// Applies sorting and limit to the list of images
    /// </summary>
    private List<ImageDto> ApplySortingAndLimit(List<ImageDto> images, string sortBy, string sortOrder, int limit)
    {
        // Apply sorting
        IOrderedEnumerable<ImageDto> sortedImages;
        
        switch (sortBy.ToLower())
        {
            case "imagetaken":
                sortedImages = sortOrder.ToLower() == "asc" 
                    ? images.OrderBy(i => i.DateImageTaken) 
                    : images.OrderByDescending(i => i.DateImageTaken);
                break;
                
            case "categoryname":
                sortedImages = sortOrder.ToLower() == "asc" 
                    ? images.OrderBy(i => i.UrlCategory) 
                    : images.OrderByDescending(i => i.UrlCategory);
                break;
                
            case "uploaded":
            default:
                sortedImages = sortOrder.ToLower() == "asc" 
                    ? images.OrderBy(i => i.DateUploaded) 
                    : images.OrderByDescending(i => i.DateUploaded);
                break;
        }
        
        // if limit = 0, then no limit
        if (limit == 0)
            return sortedImages.ToList();

        // else retur with Applied limit
        return sortedImages.Take(limit).ToList();
    }
}
