using ArvidsonFoto.Core.Attributes;
using ArvidsonFoto.Core.DTOs;
using ArvidsonFoto.Core.Interfaces;
using ArvidsonFoto.Core.Models;
using ArvidsonFoto.Core.Services;
using Microsoft.AspNetCore.Authorization;

namespace ArvidsonFoto.Controllers.ApiControllers;

/// <summary>
/// Provides endpoints for managing categories
/// </summary>
/// <remarks>The <see cref="CategoryApiController"/> class includes methods for adding, retrieving, and managing
/// categories. Some endpoints require authentication, while others can be accessed anonymously. This controller
/// interacts with the underlying database context and services to perform operations on category data.</remarks>
/// <inheritdoc cref="CategoryApiController"/>
[ApiController]
[Route("api/category")]
[Route("api/kategori")]
[Tags("Categories")]
public class CategoryApiController(ILogger<CategoryApiController> logger,
    IApiCategoryService apiCategoryService) : ControllerBase
{
    /// <summary>
    /// Adds a new category to the system.
    /// </summary>
    /// <remarks>This operation requires authentication. Ensure the provided category object contains all
    /// required fields  and adheres to the expected format.</remarks>
    /// <param name="category">The category to be added. Must not be null and should contain valid data.</param>
    /// <returns><see langword="true"/> if the category was successfully added; otherwise, <see langword="false"/>.</returns>
#if !DEBUG
    [Authorize]
#endif
    //[SwaggerOperation(Summary = "Add a new category", Description = "Requires authentication")]
    [SwaggerSecurityRequirement("cookieAuth")]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [HttpPost("AddCategory")]
    public IActionResult AddCategory(CategoryDto category)
    {
        try
        {
            if (category == null)
            {
                return Problem(
                    title: "Invalid Input",
                    detail: "Category data is required",
                    statusCode: StatusCodes.Status400BadRequest,
                    type: "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                );
            }

            var result = apiCategoryService.AddCategory(category);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding category");
            return Problem(
                title: "Internal Server Error",
                detail: "An error occurred while adding the category",
                statusCode: StatusCodes.Status500InternalServerError,
                type: "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            );
        }
    }

    /// <summary>
    /// Retrieves all categories from the database.
    /// </summary>
    /// <remarks>This method requires authentication using an API key. Ensure that the API key is provided in
    /// the request headers.</remarks>
    /// <returns>A list of <see cref="TblMenu"/> objects representing all categories. The list will be empty if no categories are
    /// found.</returns>
    [HttpGet("GetAll")]
    //[SwaggerOperation(Summary = "Get all categories", Description = "Requires authentication with api_key")]
    [SwaggerSecurityRequirement("api_key")]
    public List<CategoryDto> GetAll()
    {
        logger.LogInformation("Category - GetAll called.");
        return apiCategoryService.GetAll();
    }

    /// <summary>
    /// Tests the multi-level category path resolution without returning images.
    /// </summary>
    /// <remarks>
    /// This endpoint is useful for testing the category path resolution logic.
    /// </remarks>
    /// <param name="categoryPath">The path with multiple category segments (e.g., "Faglar/Vadare/Pipare/Kustpipare")</param>
    /// <returns>Information about the resolved category</returns>
    [AllowAnonymous]
    [HttpGet("Bilder/{*categoryPath}")]
    [HttpGet("ByPath/{*categoryPath}")]
    [HttpGet("ByPath/Bilder/{*categoryPath}")]
    [HttpGet("ByCategoryPath/{*categoryPath}")]
    public IActionResult ByCategoryPath(string categoryPath)
    {
        try
        {
            if (string.IsNullOrEmpty(categoryPath))
            {
                return BadRequest("Category path cannot be empty");
            }
            // Decode the URL-encoded path
            categoryPath = Uri.UnescapeDataString(categoryPath);

            logger.LogInformation("Image - TestCategoryPath called with path: {CategoryPath}", categoryPath);

            // Split the path into segments
            string[] segments = categoryPath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length == 0)
            {
                return BadRequest("Invalid category path format");
            }

            // Start with the first segment and traverse the category hierarchy
            int? currentCategoryId = null;
            var pathInfo = new List<object>();

            foreach (var segment in segments)
            {
                // If this is the first segment, find the main category
                if (currentCategoryId == null)
                {
                    currentCategoryId = apiCategoryService.GetIdByName(segment);
                    if (currentCategoryId <= 0)
                    {
                        return NotFound($"Category '{segment}' not found");
                    }

                    var category = apiCategoryService.GetById(currentCategoryId.Value);
                    pathInfo.Add(new
                    {
                        Level = "Main",
                        Name = category.Name,
                        Id = category.CategoryId,
                        UrlSegment = category.UrlCategoryPath?.ToLower()
                    });
                }
                else
                {
                    // Find the child category under the current parent
                    var children = apiCategoryService.GetChildrenByParentId(currentCategoryId.Value);
                    var matchingChild = children.FirstOrDefault(c =>
                        c.UrlCategoryPath!.Equals(segment, StringComparison.OrdinalIgnoreCase) ||
                        c.Name!.Equals(segment, StringComparison.OrdinalIgnoreCase));

                    if (matchingChild == null)
                    {
                        return NotFound($"Category '{segment}' not found under parent category");
                    }
                    matchingChild.UrlCategoryPath = matchingChild.UrlCategoryPath!.ToLower();


                    currentCategoryId = matchingChild.CategoryId;
                    pathInfo.Add(new
                    {
                        Level = $"Level {pathInfo.Count + 1}",
                        Name = matchingChild.Name,
                        Id = matchingChild.CategoryId,
                        UrlSegment = matchingChild.UrlCategoryPath
                    });
                }
            }

            // Return information about the path resolution
            return Ok(new
            {
                CategoryPath = categoryPath,
                ResolvedSegments = segments.Length,
                FinalCategoryId = currentCategoryId,
                PathResolution = pathInfo
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error testing category path: {CategoryPath}", categoryPath);
            return StatusCode(500, "An error occurred while processing the request");
        }
    }

    /// <summary>
    /// Retrieves a menu category by its name.
    /// </summary>
    /// <remarks>This endpoint does not require authentication and can be accessed anonymously.</remarks>
    /// <param name="categoryName">The name of the category to retrieve. This parameter cannot be null or empty.</param>
    /// <returns>An instance of <see cref="TblMenu"/> representing the menu category with the specified name. Returns <c>null</c>
    /// if no category with the given name exists.</returns>
    [AllowAnonymous]
    //[SwaggerOperation(Summary = "Get category by name", Description = "No authentication required")]
    [HttpGet("GetByName/{categoryName}")]
    public CategoryDto GetByName(string categoryName = "Blåmes")
    {
        logger.LogInformation("Category - GetByName called.");
        return apiCategoryService.GetByName(categoryName);
    }

    /// <summary>
    /// Retrieves all categories from the database (public endpoint).
    /// </summary>
    /// <remarks>This is a public endpoint that does not require authentication. It provides the same functionality
    /// as GetAll but is accessible without API key authentication and includes caching for better performance.</remarks>
    /// <returns>A list of <see cref="CategoryDto"/> objects representing all categories. The list will be empty if no categories are
    /// found.</returns>
    [AllowAnonymous]
    [ProducesResponseType<List<CategoryDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [HttpGet("All")]
    public IActionResult GetAllPublicCached()
    {
        try
        {
            logger.LogInformation("Category - GetAllPublicCached called");
            var allCategories = apiCategoryService.GetAll();
            return Ok(allCategories ?? new List<CategoryDto>());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving all categories");
            return Problem(
                title: "Internal Server Error",
                detail: "An error occurred while retrieving all categories",
                statusCode: StatusCodes.Status500InternalServerError,
                type: "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            );
        }
    }

    /// <summary>
    /// Retrieves a category by its unique identifier.
    /// </summary>
    /// <remarks>This endpoint does not require authentication and can be accessed anonymously.</remarks>
    /// <param name="categoryId">The unique identifier of the category to retrieve. Must be a positive integer.</param>
    /// <returns>An instance of <see cref="TblMenu"/> representing the category with the specified identifier. Returns <see
    /// langword="null"/> if no category with the given identifier exists.</returns>
    [AllowAnonymous]
    //[SwaggerOperation(Summary = "Get category by id", Description = "No authentication required")]
    [ProducesResponseType<CategoryDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [HttpGet("{categoryId}")]
    [HttpGet("GetById/{categoryId}")]
    public IActionResult GetById(int categoryId = 13)
    {
        try
        {
            logger.LogInformation("Category - GetById called with ID: {CategoryId}", categoryId);
            
            if (categoryId <= 0)
            {
                return Problem(
                    title: "Invalid Category ID",
                    detail: "Category ID must be a positive integer",
                    statusCode: StatusCodes.Status400BadRequest,
                    type: "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                );
            }

            var category = apiCategoryService.GetById(categoryId);
            
            if (category == null || category.CategoryId == -1)
            {
                return Problem(
                    title: "Category Not Found",
                    detail: $"No category found with ID {categoryId}",
                    statusCode: StatusCodes.Status404NotFound,
                    type: "https://tools.ietf.org/html/rfc7231#section-6.5.4"
                );
            }

            return Ok(category);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving category with ID: {CategoryId}", categoryId);
            return Problem(
                title: "Internal Server Error",
                detail: "An error occurred while retrieving the category",
                statusCode: StatusCodes.Status500InternalServerError,
                type: "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            );
        }
    }

    /// <summary>
    /// Retrieves a list of subcategories associated with the specified category ID.
    /// </summary>
    /// <remarks>This method does not require authentication and can be accessed anonymously.</remarks>
    /// <param name="categoryId">The unique identifier of the category for which subcategories are to be retrieved. Must be a valid category ID.</param>
    /// <returns>A list of <see cref="CategoryDto"/> objects representing the subcategories of the specified category. Returns an
    /// empty list if no subcategories are found.</returns>
    [AllowAnonymous]
    //[SwaggerOperation(Summary = "Get subcategories by category id", Description = "No authentication required")]
    [ProducesResponseType<List<CategoryDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [HttpGet("GetSubsList/{categoryId}")]
    public IActionResult GetSubsList(int categoryId = 10)
    {
        try
        {
            logger.LogInformation("Category - GetSubsList called with ID: {CategoryId}", categoryId);
            
            if (categoryId <= 0)
            {
                return Problem(
                    title: "Invalid Category ID",
                    detail: "Category ID must be a positive integer",
                    statusCode: StatusCodes.Status400BadRequest,
                    type: "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                );
            }

            var subcategories = apiCategoryService.GetChildrenByParentId(categoryId);
            return Ok(subcategories ?? new List<CategoryDto>());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving subcategories for category ID: {CategoryId}", categoryId);
            return Problem(
                title: "Internal Server Error",
                detail: "An error occurred while retrieving subcategories",
                statusCode: StatusCodes.Status500InternalServerError,
                type: "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            );
        }
    }

    /// <summary>
    /// Retrieves children categories associated with the specified category ID.
    /// </summary>
    /// <remarks>This method is provided for compatibility with the Web project.</remarks>
    /// <param name="categoryId">The unique identifier of the category for which children are to be retrieved.</param>
    /// <returns>A list of <see cref="CategoryDto"/> objects representing the children of the specified category.</returns>
    [AllowAnonymous]
    [ProducesResponseType<List<CategoryDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [HttpGet("Children/{categoryId}")]
    public IActionResult GetChildren(int? categoryId)
    {
        try
        {
            logger.LogInformation("Category - GetChildren called with ID: {CategoryId}", categoryId);
            
            var children = apiCategoryService.GetChildrenByParentId(categoryId ?? 0);
            return Ok(children ?? new List<CategoryDto>());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving children for category ID: {CategoryId}", categoryId);
            return Problem(
                title: "Internal Server Error",
                detail: "An error occurred while retrieving child categories",
                statusCode: StatusCodes.Status500InternalServerError,
                type: "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            );
        }
    }

    /// <summary>
    /// Retrieves the name of a category based on its unique identifier.
    /// </summary>
    /// <remarks>This endpoint does not require authentication and can be accessed anonymously.</remarks>
    /// <param name="categoryId">The unique identifier of the category. Must be a positive integer.</param>
    /// <returns>The name of the category associated with the specified <paramref name="categoryId"/>. Returns an empty string if no
    /// category is found for the given identifier.</returns>
    [AllowAnonymous]
    //[SwaggerOperation(Summary = "Get category name by id", Description = "No authentication required")]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [HttpGet("GetNameById/{categoryId}")]
    public IActionResult GetNameById(int categoryId = 13)
    {
        try
        {
            logger.LogInformation("Category - GetNameById called with ID: {CategoryId}", categoryId);
            
            if (categoryId <= 0)
            {
                return Problem(
                    title: "Invalid Category ID",
                    detail: "Category ID must be a positive integer",
                    statusCode: StatusCodes.Status400BadRequest,
                    type: "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                );
            }

            var categoryName = apiCategoryService.GetNameById(categoryId);

            if (categoryName == "Not found" || string.IsNullOrEmpty(categoryName))
            {
                return Problem(
                    title: "Category Not Found",
                    detail: $"No category found with ID {categoryId}",
                    statusCode: StatusCodes.Status404NotFound,
                    type: "https://tools.ietf.org/html/rfc7231#section-6.5.4"
                );
            }

            return Ok(categoryName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving category name for ID: {CategoryId}", categoryId);
            return Problem(
                title: "Internal Server Error",
                detail: "An error occurred while retrieving the category name",
                statusCode: StatusCodes.Status500InternalServerError,
                type: "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            );
        }
    }

    /// <summary>
    /// Retrieves the unique identifier of a category based on its name.
    /// </summary>
    /// <remarks>This endpoint does not require authentication. It is intended for retrieving category
    /// identifiers  by name in scenarios where the category name is known but the identifier is needed for further
    /// operations.</remarks>
    /// <param name="categoryName">The name of the category for which the identifier is requested. Cannot be null or empty.</param>
    /// <returns>The unique identifier of the category if found; otherwise, 0.</returns>
    [AllowAnonymous]
    //[SwaggerOperation(Summary = "Get category id by name", Description = "No authentication required")]
    [ProducesResponseType<int>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [HttpGet("GetIdByName/{categoryName}")]
    public IActionResult GetIdByName(string categoryName = "Blames")
    {
        try
        {
            logger.LogInformation("Category - GetIdByName called with name: {CategoryName}", categoryName);
            
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                return Problem(
                    title: "Invalid Category Name",
                    detail: "Category name cannot be null or empty",
                    statusCode: StatusCodes.Status400BadRequest,
                    type: "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                );
            }

            var categoryId = apiCategoryService.GetIdByName(categoryName);
            return Ok(categoryId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving category ID for name: {CategoryName}", categoryName);
            return Problem(
                title: "Internal Server Error",
                detail: "An error occurred while retrieving the category ID",
                statusCode: StatusCodes.Status500InternalServerError,
                type: "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            );
        }
    }

    /// <summary>
    /// Retrieves the last category ID from the database.
    /// </summary>
    /// <remarks>This method requires authentication using an API key. It returns the highest category ID
    /// currently stored in the database.</remarks>
    /// <returns>The last category ID as an integer. If no categories exist, the return value may depend on the implementation of
    /// the underlying service.</returns>
    [HttpGet("GetLastId")]
    //[SwaggerOperation(Summary = "Get last category id", Description = "Requires authentication with api_key")]
    [SwaggerSecurityRequirement("api_key")]
    [ProducesResponseType<int>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public IActionResult GetLastId()
    {
        try
        {
            logger.LogInformation("Category - GetLastId called");
            var lastId = apiCategoryService.GetLastId();
            return Ok(lastId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving last category ID");
            return Problem(
                title: "Internal Server Error",
                detail: "An error occurred while retrieving the last category ID",
                statusCode: StatusCodes.Status500InternalServerError,
                type: "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            );
        }
    }

    /// <summary>
    /// Retrieves a list of all main categories.
    /// </summary>
    /// <remarks>This endpoint does not require authentication and can be accessed anonymously.</remarks>
    /// <returns>A list of <see cref="CategoryDto"/> objects representing the main categories.</returns>
    [AllowAnonymous]
    [ProducesResponseType<List<CategoryDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [HttpGet("Main")]
    public IActionResult GetMainCategories()
    {
        try
        {
            logger.LogInformation("Category - GetMainCategories called");
            var mainCategories = apiCategoryService.GetMainCategories();
            return Ok(mainCategories ?? new List<CategoryDto>());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving main categories");
            return Problem(
                title: "Internal Server Error",
                detail: "An error occurred while retrieving main categories",
                statusCode: StatusCodes.Status500InternalServerError,
                type: "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            );
        }
    }

    /// <summary>
    /// Retrieves a category by its URL segment.
    /// </summary>
    /// <remarks>This endpoint does not require authentication and can be accessed anonymously.</remarks>
    /// <param name="urlSegment">The URL segment of the category to retrieve. This parameter is case-insensitive.</param>
    /// <returns>A <see cref="CategoryDto"/> representing the category with the specified URL segment. Returns a default DTO if no category is found.</returns>
    [AllowAnonymous]
    [ProducesResponseType<CategoryDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [HttpGet("ByUrlSegment/{urlSegment}")]
    public IActionResult GetByUrlSegment(string urlSegment)
    {
        try
        {
            logger.LogInformation("Category - GetByUrlSegment called with: {UrlSegment}", urlSegment);
            
            if (string.IsNullOrWhiteSpace(urlSegment))
            {
                return Problem(
                    title: "Invalid URL Segment",
                    detail: "URL segment cannot be null or empty",
                    statusCode: StatusCodes.Status400BadRequest,
                    type: "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                );
            }

            var category = apiCategoryService.GetByUrlSegment(urlSegment);
            return Ok(category);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving category by URL segment: {UrlSegment}", urlSegment);
            return Problem(
                title: "Internal Server Error",
                detail: "An error occurred while retrieving the category",
                statusCode: StatusCodes.Status500InternalServerError,
                type: "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            );
        }
    }

    /// <summary>
    /// Retrieves the unique identifier of a category based on its URL segment.
    /// </summary>
    /// <remarks>This endpoint does not require authentication. It is intended for retrieving category
    /// identifiers by URL segment in scenarios where the segment is known but the identifier is needed for further
    /// operations.</remarks>
    /// <param name="urlSegment">The URL segment of the category for which the identifier is requested. Cannot be null or empty.</param>
    /// <returns>The unique identifier of the category if found; otherwise, -1.</returns>
    [AllowAnonymous]
    [ProducesResponseType<int>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [HttpGet("GetIdByUrlSegment/{urlSegment}")]
    public IActionResult GetIdByUrlSegment(string urlSegment)
    {
        try
        {
            logger.LogInformation("Category - GetIdByUrlSegment called with: {UrlSegment}", urlSegment);
            
            if (string.IsNullOrWhiteSpace(urlSegment))
            {
                return Problem(
                    title: "Invalid URL Segment",
                    detail: "URL segment cannot be null or empty",
                    statusCode: StatusCodes.Status400BadRequest,
                    type: "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                );
            }

            var categoryId = apiCategoryService.GetIdByUrlSegment(urlSegment);
            return Ok(categoryId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving category ID by URL segment: {UrlSegment}", urlSegment);
            return Problem(
                title: "Internal Server Error",
                detail: "An error occurred while retrieving the category ID",
                statusCode: StatusCodes.Status500InternalServerError,
                type: "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            );
        }
    }

    /// <summary>
    /// Retrieves a category by its URL segment with fallback to numeric ID if the segment is numeric.
    /// </summary>
    /// <remarks>This endpoint does not require authentication and can be accessed anonymously. It tries multiple
    /// approaches to find a matching category, providing robust URL handling.</remarks>
    /// <param name="urlSegment">The URL segment or potentially a numeric ID of the category to retrieve.</param>
    /// <returns>A <see cref="CategoryDto"/> representing the category found by URL segment or ID. Returns a default DTO if no category is found.</returns>
    [AllowAnonymous]
    [ProducesResponseType<CategoryDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [HttpGet("ByUrlSegmentWithFallback/{urlSegment}")]
    public IActionResult GetByUrlSegmentWithFallback(string urlSegment)
    {
        try
        {
            logger.LogInformation("Category - GetByUrlSegmentWithFallback called with: {UrlSegment}", urlSegment);
            
            if (string.IsNullOrWhiteSpace(urlSegment))
            {
                return Problem(
                    title: "Invalid URL Segment",
                    detail: "URL segment cannot be null or empty",
                    statusCode: StatusCodes.Status400BadRequest,
                    type: "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                );
            }

            var category = apiCategoryService.GetByUrlSegmentWithFallback(urlSegment);
            return Ok(category);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving category by URL segment with fallback: {UrlSegment}", urlSegment);
            return Problem(
                title: "Internal Server Error",
                detail: "An error occurred while retrieving the category",
                statusCode: StatusCodes.Status500InternalServerError,
                type: "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            );
        }
    }
}
