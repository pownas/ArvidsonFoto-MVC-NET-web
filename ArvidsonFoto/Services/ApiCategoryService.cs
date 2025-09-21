using ArvidsonFoto.Core.Extensions;
using ArvidsonFoto.Core.ApiResponses;
using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.DTOs;
using ArvidsonFoto.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using System.Collections.Concurrent;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace ArvidsonFoto.Services;

/// <summary>
/// Provides functionality for managing categories, including adding, retrieving, updating, and deleting categories.
/// </summary>
/// <remarks>
/// The <see cref="ApiCategoryService"/> class interacts with the database to perform operations on category data.
/// It includes methods for managing category hierarchies, creating category paths, and handling category operations.
/// Enhanced with caching for improved performance when serving navigation menus.
/// </remarks>
/// <param name="logger">Logging for the service.</param>
/// <param name="dbContext">The database context used to interact with the database.</param>
/// <param name="memoryCache">The memory cache instance for caching category data.</param>
public class ApiCategoryService(ILogger<ApiCategoryService> logger, ArvidsonFotoCoreDbContext dbContext, IMemoryCache memoryCache) : IApiCategoryService
{
    /// <summary> Databas koppling: <see cref="ArvidsonFotoCoreDbContext"/> </summary>
    private readonly ArvidsonFotoCoreDbContext _entityContext = dbContext;

    /// <summary> Memory cache for improved performance </summary>
    private readonly IMemoryCache _cache = memoryCache;

    /// <summary> Cache expiration time for different types of data </summary>
    private static readonly TimeSpan _shortCacheExpiry = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan _longCacheExpiry = TimeSpan.FromHours(2);

    /// <summary> Cache keys for different data types </summary>
    private const string ALL_CATEGORIES_CACHE_KEY = "Api_AllCategories";
    private const string MAIN_CATEGORIES_CACHE_KEY = "Api_MainCategories";
    private const string MAIN_MENU_CACHE_KEY = "Api_MainMenu";

    /// <summary> Värde när meny-kategorin inte hittats </summary>
    private static CategoryDto DefaultCategoryNotFound { get; } = new CategoryDto().CreateNotFoundCategoryDto();

    /// <summary> Simple in-memory cache for category paths to improve performance </summary>
    private static readonly ConcurrentDictionary<int, string> _categoryPathCache = new();

    /// <summary> Cache expiry time for category paths (5 minutes) </summary>
    private static DateTime _cacheLastUpdated = DateTime.MinValue;
    private static readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Adds a new category to the database.
    /// </summary>
    /// <param name="category">The category to add.</param>
    /// <returns>True if the category was successfully added, false otherwise.</returns>
    /// <exception cref="Exception">Thrown when an error occurs during category creation.</exception>
    public bool AddCategory(CategoryDto category)
    {
        bool success = false;
        try
        {
            var tblMenu = category.ToTblMenu();
            _entityContext.TblMenus.Add(tblMenu);
            _entityContext.SaveChanges();
            success = true;
        }
        catch (Exception ex)
        {
            success = false;
            throw new Exception("Fel vid skapande av kategori. Felmeddelande: " + ex.Message);
        }
        return success;
    }

    /// <summary>
    /// Gets the ID of the last added category.
    /// </summary>
    /// <returns>The ID of the last category, or -1 if no categories exist or an error occurs.</returns>
    public int GetLastId()
    {
        try
        {
            var lastCategory = _entityContext.TblMenus.OrderByDescending(c => c.MenuCategoryId).FirstOrDefault();
            return lastCategory?.MenuCategoryId ?? -1;
        }
        catch (Exception ex)
        {
            Log.Error("GetLastId failed: {Message}", ex.Message);
            return -1;
        }
    }

    /// <summary>
    /// Gets a category by its name.
    /// </summary>
    /// <param name="categoryName">The name of the category to retrieve.</param>
    /// <returns>The category DTO if found, otherwise a default "not found" category.</returns>
    public CategoryDto GetByName(string categoryName)
    {
        var category = _entityContext.TblMenus.Where(c => c.MenuDisplayName != null && c.MenuDisplayName.Equals(categoryName)).FirstOrDefault();
        if (category is null)
        {
            Log.Warning("Could not find category: '" + categoryName + "'");
            return DefaultCategoryNotFound;
        }

        var categoryPath = GetCategoryPathForImage(category.MenuCategoryId ?? -1);
        var lastImageFilename = GetLastImageFilename(category.MenuCategoryId ?? -1);
        var categoryImageCount = dbContext.TblImages.Where(x => x.ImageCategoryId == category.MenuCategoryId).Count();
        return category.ToCategoryDto(categoryPath, lastImageFilename, categoryImageCount);
    }

    /// <summary>
    /// Gets a category by its ID.
    /// </summary>
    /// <param name="id">The ID of the category to retrieve.</param>
    /// <returns>The category DTO if found, otherwise a default "not found" category.</returns>
    public CategoryDto GetById(int? id)
    {
        if (id == null || id <= 0)
        {
            Log.Information("Invalid category id: {Id}", id);
            return DefaultCategoryNotFound;
        }

        try
        {
            var category = _entityContext.TblMenus
                .FirstOrDefault(c => c.MenuCategoryId == id.Value);

            if (category == null)
            {
                Log.Information("Could not find category with id: {Id}", id);
                return DefaultCategoryNotFound;
            }

            var categoryPath = GetCategoryPathForImage(category.MenuCategoryId ?? -1);
            var lastImageFilename = GetLastImageFilename(category.MenuCategoryId ?? -1);
            var categoryImageCount = dbContext.TblImages.Where(x => x.ImageCategoryId == category.MenuCategoryId).Count();
            return category.ToCategoryDto(categoryPath, lastImageFilename, categoryImageCount);
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetById: {Message}", ex.Message);
            return DefaultCategoryNotFound;
        }
    }

    /// <summary>
    /// Gets all categories from the database with caching support.
    /// </summary>
    /// <returns>A list of all categories as DTOs.</returns>
    public List<CategoryDto> GetAll()
    {
        // Try to get from cache first
        if (_cache.TryGetValue(ALL_CATEGORIES_CACHE_KEY, out List<CategoryDto>? cachedCategories) && cachedCategories != null)
        {
            Log.Debug("All categories retrieved from cache ({Count} categories)", cachedCategories.Count);
            return cachedCategories;
        }

        var categories = _entityContext.TblMenus.ToList();

        // Get category paths for all categories
        var categoryDtos = new List<CategoryDto>();
        foreach (var category in categories)
        {
            var categoryPath = GetCategoryPathForImage(category.MenuCategoryId ?? -1);
            var lastImageFilename = GetLastImageFilename(category.MenuCategoryId ?? -1);

            categoryDtos.Add(category.ToCategoryDto(categoryPath, lastImageFilename));
        }

        // Cache the results
        _cache.Set(ALL_CATEGORIES_CACHE_KEY, categoryDtos, _shortCacheExpiry);
        Log.Debug("All categories cached ({Count} categories)", categoryDtos.Count);

        return categoryDtos;
    }

    /// <summary>
    /// Gets all child categories for a given parent category ID.
    /// </summary>
    /// <param name="categoryID">The ID of the parent category.</param>
    /// <returns>A list of child categories as DTOs.</returns>
    public List<CategoryDto> GetChildrenByParentId(int categoryID)
    {
        try
        {
            var categories = _entityContext.TblMenus
                .Where(c => c.MenuParentCategoryId == categoryID)
                .OrderBy(c => c.MenuDisplayName)
                .ToList();

            // Get category paths for all categories
            var categoryDtos = new List<CategoryDto>();
            foreach (var category in categories)
            {
                var categoryPath = GetCategoryPathForImage(category.MenuCategoryId ?? -1);
                var lastImageFilename = GetLastImageFilename(category.MenuCategoryId ?? -1);

                categoryDtos.Add(category.ToCategoryDto(categoryPath, lastImageFilename));
            }

            return categoryDtos;
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetChildrenByParentId: {Message}", ex.Message);
            return new List<CategoryDto>();
        }
    }

    /// <summary>
    /// Gets the name of a category by its ID.
    /// </summary>
    /// <param name="id">The ID of the category.</param>
    /// <returns>The name of the category, or "Not found" if the category does not exist.</returns>
    public string GetNameById(int? id)
    {
        if (id == null || id <= 0)
        {
            Log.Information("Invalid category id for GetNameById: {Id}", id);
            return "Not found";
        }

        try
        {
            var category = _entityContext.TblMenus
                .FirstOrDefault(c => c.MenuCategoryId == id.Value);

            if (category == null)
            {
                Log.Information("Could not find category name for id: {Id}", id);
                return "Not found";
            }

            return category.MenuDisplayName ?? "Not found";
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetNameById: {Message}", ex.Message);
            return "Not found";
        }
    }

    /// <summary>
    /// Gets the ID of a category by its name.
    /// </summary>
    /// <param name="categoryName">The name of the category to search for.</param>
    /// <returns>The ID of the category, or -1 if not found.</returns>
    public int GetIdByName(string categoryName)
    {
        if (string.IsNullOrEmpty(categoryName))
        {
            Log.Information("Empty category name provided to GetIdByName");
            return -1;
        }

        try
        {
            // First try exact match on URL segment (slug)
            var category = _entityContext.TblMenus
                .FirstOrDefault(c => c.MenuUrlSegment!.ToLower() == categoryName.ToLower());

            // If not found, try display name
            if (category == null)
            {
                category = _entityContext.TblMenus
                    .FirstOrDefault(c => c.MenuDisplayName!.ToLower() == categoryName.ToLower());
            }

            if (category == null)
            {
                Log.Information("Could not find category id for name: {Name}", categoryName);
                return -1;
            }

            return category.MenuCategoryId ?? -1;
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetIdByName: {Message}", ex.Message);
            return -1;
        }
    }

    /// <summary>
    /// Updates an existing category in the database.
    /// </summary>
    /// <param name="updatedCategory">The updated category information.</param>
    /// <returns>True if the update was successful, false otherwise.</returns>
    public bool UpdateCategory(CategoryDto updatedCategory)
    {
        bool success = false;
        try
        {
            var categoryToEdit = _entityContext.TblMenus.FirstOrDefault(c => c.MenuCategoryId == updatedCategory.CategoryId);
            if (categoryToEdit != null)
            {
                categoryToEdit.MenuDisplayName = updatedCategory.Name;
                categoryToEdit.MenuUrlSegment = updatedCategory.UrlCategoryPath;
                // Note: Parent category ID would need to be part of CategoryDto to update it

                _entityContext.TblMenus.Update(categoryToEdit);
                _entityContext.SaveChanges();
                success = true;
            }
        }
        catch (Exception ex)
        {
            success = false;
            throw new Exception("Fel vid uppdatering av kategori. Felmeddelande: " + ex.Message);
        }
        return success;
    }

    /// <summary>
    /// Deletes a category from the database by its ID.
    /// </summary>
    /// <param name="id">The ID of the category to delete.</param>
    /// <returns>True if the deletion was successful, false otherwise.</returns>
    /// <exception cref="Exception">Thrown when an error occurs during deletion.</exception>
    public bool DeleteCategory(int? id)
    {
        bool success = false;
        try
        {
            var categoryToDelete = _entityContext.TblMenus.FirstOrDefault(c => c.MenuCategoryId == id);
            if (categoryToDelete != null)
            {
                _entityContext.TblMenus.Remove(categoryToDelete);
                _entityContext.SaveChanges();
                success = true;
            }
        }
        catch (Exception ex)
        {
            success = false;
            throw new Exception("Fel vid borttagning av kategori. Felmeddelande: " + ex.Message);
        }
        return success;
    }

    /// <summary>
    /// Deletes a category from the database using a category DTO.
    /// </summary>
    /// <param name="category">The category DTO to delete.</param>
    /// <returns>True if the deletion was successful, false otherwise.</returns>
    public bool DeleteCategory(CategoryDto category)
    {
        return DeleteCategory(category.CategoryId);
    }

    /// <summary>
    /// Retrieves the URL for a given category ID.
    /// </summary>
    /// <param name="id">Category id</param>
    /// <returns>Example: //// </returns>
    public string GetCategoryUrl(int? id)
    {
        // Validate the id to ensure it is a positive integer
        if (id == null || id <= 0)
        {
            Log.Warning("Invalid category ID provided: {CategoryId}. Must be a positive integer.", id);
            return $"{(int)HttpStatusCode.BadRequest}-{HttpStatusCode.BadRequest}. Must be a positive integer: {id}";
        }

        // Build the URL segments by traversing up the parent chain in-memory
        var segments = new List<string>();
        var currentId = id;

        // Start with the current category and traverse up to the root category
        while (currentId != null && currentId > 0)
        {
            // Fetch the current category and its parent category from the database
            var category = _entityContext.TblMenus
                .Where(c => c.MenuCategoryId == currentId)
                .Select(c => new { c.MenuParentCategoryId, c.MenuUrlSegment, c.Id })
                .FirstOrDefault();

            // If the category is not found, return an empty string
            if (category == null)
            {
                Log.Warning("Could not find category or parent with ID: {CategoryId}", currentId);
                return $"{(int)HttpStatusCode.NotFound}-{HttpStatusCode.NotFound}. Could not find category or parent with ID: {currentId}";
            }

            // Insert the URL segment at the beginning of the list
            if (!string.IsNullOrWhiteSpace(category.MenuUrlSegment))
                segments.Insert(0, category.MenuUrlSegment);

            // Move to the parent category
            currentId = category.MenuParentCategoryId;
        }

        // If no segments were collected, return an empty string
        if (segments.Count == 0)
            return $"{(int)HttpStatusCode.NotFound}-{HttpStatusCode.NotFound}. Could not find category or parent with ID: {id}";

        var url = "/" + string.Join("/", segments).Trim().ToLowerInvariant();
        return url;
    }

    /// <summary>
    /// Retrieves the sorting URL for a given category ID.
    /// </summary>
    /// <param name="id">Category id</param>
    /// <returns>Example: //// </returns>
    private string GetSortingUrl(int? id)
    {
        // Validate the id to ensure it is a positive integer
        if (id == null || id <= 0)
        {
            Log.Warning("Invalid category ID provided: {CategoryId}. Must be a positive integer.", id);
            return $"{(int)HttpStatusCode.BadRequest}-{HttpStatusCode.BadRequest}. Must be a positive integer: {id}";
        }

        // Build the URL segments by traversing up the parent chain in-memory
        var segments = new List<string>();
        var currentId = id;

        // Start with the current category and traverse up to the root category
        while (currentId != null && currentId > 0)
        {
            // Fetch the current category and its parent category from the database
            var category = _entityContext.TblMenus
                .Where(c => c.MenuCategoryId == currentId)
                .Select(c => new { c.MenuParentCategoryId, c.MenuDisplayName, c.Id })
                .FirstOrDefault();

            // If the category is not found, return an empty string
            if (category == null)
            {
                Log.Warning("Could not find category or parent with ID: {CategoryId}", currentId);
                return $"{(int)HttpStatusCode.NotFound}-{HttpStatusCode.NotFound}. Could not find category or parent with ID: {currentId}";
            }

            // Insert the URL segment at the beginning of the list
            if (!string.IsNullOrWhiteSpace(category.MenuDisplayName))
                segments.Insert(0, category.MenuDisplayName);

            // Move to the parent category
            currentId = category.MenuParentCategoryId;
        }

        // If no segments were collected, return an empty string
        if (segments.Count == 0)
            return $"{(int)HttpStatusCode.NotFound}-{HttpStatusCode.NotFound}. Could not find category or parent with ID: {id}";

        var url = "/" + string.Join("/", segments).Trim().ToLowerInvariant();
        return url;
    }

    /// <summary>
    /// Gets the main menu structure with categories organized hierarchically, with enhanced caching.
    /// </summary>
    /// <returns>A main menu response containing the structured menu data.</returns>
    public MainMenuResponse GetMainMenu()
    {
        // Try to get from cache first
        if (_cache.TryGetValue(MAIN_MENU_CACHE_KEY, out MainMenuResponse? cachedMainMenu) && cachedMainMenu != null)
        {
            Log.Debug("Main menu retrieved from cache");
            return cachedMainMenu;
        }

        var mainMenu = new MainMenuResponse();
        var categories = _entityContext.TblMenus.ToList();

        if (categories == null || categories.Count == 0)
        {
            Log.Warning("No categories found in the database.");
            return mainMenu; // Return an empty dictionary if no categories are found
        }

        // Iterate through the categories and populate the main menu dictionary
        for (int i = 0; i < categories.Count; i++)
        {
            // Ensure that the MenuUrlSegment and MenuDisplayName are not null or empty before adding to the dictionary
            if (!string.IsNullOrWhiteSpace(categories[i].MenuUrlSegment) && !string.IsNullOrWhiteSpace(categories[i].MenuDisplayName))
            {
                // Add the dto to the response
                mainMenu.MainMenu.Add(new MainMenuDto
                {
                    MenuUrl = GetCategoryUrl(categories[i].MenuCategoryId),
                    MenuDisplayName = categories[i].MenuDisplayName ?? string.Empty,
                    //MenuId = categories[i].MenuCategoryId,
                    //ParentId = categories[i].MenuParentCategoryId,
                    SubCategoryCount = _entityContext.TblMenus.Count(c => c.MenuParentCategoryId == categories[i].MenuCategoryId),
                    //SubCategories = _entityContext.TblMenus
                    //    .Where(c => c.MenuParentCategoryId == categories[i].MenuCategoryId)
                    //    .Select(c => c.MenuCategoryId).ToList(),
                    SortingUrlWithAao = GetSortingUrl(categories[i].MenuCategoryId),
                });
            }
        }

        // Sort the MainMenu by URLs (keys) before returning
        mainMenu.MainMenu = mainMenu.MainMenu.OrderBy(x => x.SortingUrlWithAao).ToList();

        // Cache the main menu with longer expiration
        _cache.Set(MAIN_MENU_CACHE_KEY, mainMenu, _longCacheExpiry);
        Log.Debug("Main menu cached with {Count} items", mainMenu.MainMenu.Count);

        return mainMenu;
    }

    /// <summary>
    /// Clears all category-related caches
    /// </summary>
    public void ClearCache()
    {
        _cache.Remove(ALL_CATEGORIES_CACHE_KEY);
        _cache.Remove(MAIN_CATEGORIES_CACHE_KEY);
        _cache.Remove(MAIN_MENU_CACHE_KEY);

        // Clear the category path cache as well
        _categoryPathCache.Clear();
        _cacheLastUpdated = DateTime.MinValue;

        Log.Information("All category caches cleared");
    }

    /// <summary>
    /// Gets all main (root) categories with no parent, with caching support
    /// </summary>
    public List<CategoryDto> GetMainCategories()
    {
        // Try to get from cache first
        if (_cache.TryGetValue(MAIN_CATEGORIES_CACHE_KEY, out List<CategoryDto>? cachedMainCategories) && cachedMainCategories != null)
        {
            Log.Debug("Main categories retrieved from cache ({Count} categories)", cachedMainCategories.Count);
            return cachedMainCategories;
        }

        try
        {
            // Temporary hardcoded test data for navigation menu testing
            // This allows testing the navigation menu functionality without a database connection
            var testCategories = new List<CategoryDto>
            {
                new CategoryDto
                {
                    CategoryId = 1,
                    Name = "Däggdjur",
                    //Description = "Däggdjur bilder",
                    UrlCategoryPath = "daggdjur",
                    UrlCategoryPathFull = "daggdjur"
                },
                new CategoryDto
                {
                    CategoryId = 2,
                    Name = "Fåglar",
                    //Description = "Fåglar bilder",
                    UrlCategoryPath = "faglar",
                    UrlCategoryPathFull = "faglar"
                },
                new CategoryDto
                {
                    CategoryId = 3,
                    Name = "Insekter",
                    //Description = "Insekter bilder",
                    UrlCategoryPath = "insekter",
                    UrlCategoryPathFull = "insekter"
                },
                new CategoryDto
                {
                    CategoryId = 4,
                    Name = "Landskap",
                    //Description = "Landskap bilder",
                    UrlCategoryPath = "landskap",
                    UrlCategoryPathFull = "landskap"
                },
                new CategoryDto
                {
                    CategoryId = 5,
                    Name = "Växter",
                    //Description = "Växter bilder",
                    UrlCategoryPath = "vaxter",
                    UrlCategoryPathFull = "vaxter"
                }
            };

            // Cache the results with longer expiration since main categories change less frequently
            _cache.Set(MAIN_CATEGORIES_CACHE_KEY, testCategories, _longCacheExpiry);
            Log.Debug("Main categories cached ({Count} categories)", testCategories.Count);

            return testCategories;

            // Original database code (disabled for testing)
            /*
            var categories = _entityContext.TblMenus
                .Where(m => m.MenuParentCategoryId == null || m.MenuParentCategoryId <= 0)
                .OrderBy(m => m.MenuDisplayName)
                .ToList();

            // Get category paths for all categories
            var categoryDtos = new List<CategoryDto>();
            foreach (var category in categories)
            {
                var categoryPath = GetCategoryPathForImage(category.MenuCategoryId ?? -1);
                var lastImageFilename = GetLastImageFilename(category.MenuCategoryId ?? -1);

                categoryDtos.Add(category.ToCategoryDto(categoryPath, lastImageFilename));
            }

            // Cache the results
            _cache.Set(MAIN_CATEGORIES_CACHE_KEY, categoryDtos, _longCacheExpiry);
            
            return categoryDtos;
            */
        }
        catch (Exception ex)
        {
            Log.Error("GetMainCategories failed: {Message}", ex.Message);
            return new List<CategoryDto>();
        }
    }

    // Extension-compatible methods
    /// <summary>
    /// Gets all categories from the database asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of all categories as DTOs.</returns>
    public Task<List<CategoryDto>> GetAllCategoriesAsync()
    {
        return Task.FromResult(GetAll());
    }

    /// <summary>
    /// Creates a new category asynchronously.
    /// </summary>
    /// <param name="category">The category to create.</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the creation was successful.</returns>
    public Task<bool> CreateCategoryAsync(CategoryDto category)
    {
        return Task.FromResult(AddCategory(category));
    }

    /// <summary>
    /// Gets a list of subcategories for a given parent category ID.
    /// </summary>
    /// <param name="parentId">The ID of the parent category.</param>
    /// <returns>A list of subcategories as DTOs.</returns>
    public List<CategoryDto> GetSubsList(int parentId)
    {
        return GetChildrenByParentId(parentId);
    }

    /// <summary>
    /// Gets all categories from the database.
    /// </summary>
    /// <returns>A list of all categories as DTOs.</returns>
    public List<CategoryDto> GetAllCategories()
    {
        return GetAll();
    }

    /// <summary>
    /// Gets the count of all subcategories (categories with a parent category).
    /// </summary>
    /// <returns>The number of subcategories.</returns>
    public int GetAllSubCategoriesCounted()
    {
        return _entityContext.TblMenus.Count(m => m.MenuParentCategoryId.HasValue && m.MenuParentCategoryId > 0);
    }

    /// <summary>
    /// Retrieves a category by its URL segment.
    /// </summary>
    /// <param name="urlSegment">The URL segment of the category to retrieve. This is case-insensitive.</param>
    /// <returns>The category with the specified URL segment, or a default "not found" category if no match is found.</returns>
    public CategoryDto GetByUrlSegment(string urlSegment)
    {
        if (string.IsNullOrEmpty(urlSegment))
        {
            Log.Information("Empty URL segment provided to GetByUrlSegment");
            return DefaultCategoryNotFound;
        }

        try
        {
            var category = _entityContext.TblMenus
                .FirstOrDefault(c => c.MenuUrlSegment != null && c.MenuUrlSegment.ToLower() == urlSegment.ToLower());

            if (category == null)
            {
                Log.Warning("Could not find category with URL segment: '{UrlSegment}'", urlSegment);
                return DefaultCategoryNotFound;
            }

            var categoryPath = GetCategoryPathForImage(category.MenuCategoryId ?? -1);
            var lastImageFilename = GetLastImageFilename(category.MenuCategoryId ?? -1);
            var categoryImageCount = dbContext.TblImages.Where(x => x.ImageCategoryId == category.MenuCategoryId).Count();

            return category.ToCategoryDto(categoryPath, lastImageFilename, categoryImageCount);
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetByUrlSegment: {Message}", ex.Message);
            return DefaultCategoryNotFound;
        }
    }

    /// <summary>
    /// Gets the ID of a category based on its URL segment.
    /// </summary>
    /// <param name="urlSegment">The URL segment of the category. This is case-insensitive.</param>
    /// <returns>The ID of the category, or -1 if no category with the specified URL segment is found.</returns>
    public int GetIdByUrlSegment(string urlSegment)
    {
        if (string.IsNullOrEmpty(urlSegment))
        {
            Log.Information("Empty URL segment provided to GetIdByUrlSegment");
            return -1;
        }

        try
        {
            var category = _entityContext.TblMenus
                .FirstOrDefault(c => c.MenuUrlSegment!.ToLower() == urlSegment.ToLower());

            if (category == null)
            {
                Log.Information("Could not find category id for URL segment: {UrlSegment}", urlSegment);
                return -1;
            }

            return category.MenuCategoryId ?? -1;
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetIdByUrlSegment: {Message}", ex.Message);
            return -1;
        }
    }

    /// <summary>
    /// Retrieves a category by its URL segment with fallback to numeric ID if the segment is numeric.
    /// </summary>
    /// <param name="urlSegment">The URL segment or potentially a numeric ID of the category to retrieve.</param>
    /// <returns>The category found by URL segment or ID, or a default "not found" category if no match is found.</returns>
    public CategoryDto GetByUrlSegmentWithFallback(string urlSegment)
    {
        if (string.IsNullOrEmpty(urlSegment))
        {
            Log.Information("Empty URL segment provided to GetByUrlSegmentWithFallback");
            return DefaultCategoryNotFound;
        }

        try
        {
            // First try to find by URL segment
            var category = _entityContext.TblMenus
                .FirstOrDefault(c => c.MenuUrlSegment!.ToLower() == urlSegment.ToLower());

            // If found, return it
            if (category != null)
            {
                var categoryPath = GetCategoryPathForImage(category.MenuCategoryId ?? -1);
                var lastImageFilename = GetLastImageFilename(category.MenuCategoryId ?? -1);
                var categoryImageCount = dbContext.TblImages.Where(x => x.ImageCategoryId == category.MenuCategoryId).Count();

                return category.ToCategoryDto(categoryPath, lastImageFilename, categoryImageCount);
            }

            // If not found, check if the segment might be a numeric ID
            if (int.TryParse(urlSegment, out int categoryId))
            {
                // Try to find by ID
                category = _entityContext.TblMenus
                    .FirstOrDefault(c => c.MenuCategoryId == categoryId);

                if (category != null)
                {
                    Log.Information("Found category by ID fallback: {Id}", categoryId);
                    var categoryPath = GetCategoryPathForImage(category.MenuCategoryId ?? -1);
                    var lastImageFilename = GetLastImageFilename(category.MenuCategoryId ?? -1);
                    var categoryImageCount = dbContext.TblImages.Where(x => x.ImageCategoryId == category.MenuCategoryId).Count();

                    return category.ToCategoryDto(categoryPath, lastImageFilename, categoryImageCount);
                }
            }

            // If still not found, check if it matches a display name (for legacy support)
            category = _entityContext.TblMenus
                .FirstOrDefault(c => c.MenuDisplayName!.ToLower() == urlSegment.ToLower());

            if (category != null)
            {
                Log.Information("Found category by display name fallback: {Name}", urlSegment);
                var categoryPath = GetCategoryPathForImage(category.MenuCategoryId ?? -1);
                var lastImageFilename = GetLastImageFilename(category.MenuCategoryId ?? -1);
                var categoryImageCount = dbContext.TblImages.Where(x => x.ImageCategoryId == category.MenuCategoryId).Count();

                return category.ToCategoryDto(categoryPath, lastImageFilename, categoryImageCount);
            }

            // If all else fails, try a partial match on URL segment or display name
            category = _entityContext.TblMenus
                .FirstOrDefault(c => c.MenuUrlSegment!.ToLower().Contains(urlSegment.ToLower()) ||
                                     c.MenuDisplayName!.ToLower().Contains(urlSegment.ToLower()));

            if (category != null)
            {
                Log.Information("Found category by partial match: {Segment}", urlSegment);
                var categoryPath = GetCategoryPathForImage(category.MenuCategoryId ?? -1);
                var lastImageFilename = GetLastImageFilename(category.MenuCategoryId ?? -1);
                var categoryImageCount = dbContext.TblImages.Where(x => x.ImageCategoryId == category.MenuCategoryId).Count();

                return category.ToCategoryDto(categoryPath, lastImageFilename, categoryImageCount);
            }

            Log.Warning("Could not find category with URL segment or fallback: '{UrlSegment}'", urlSegment);
            return DefaultCategoryNotFound;
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetByUrlSegmentWithFallback: {Message}", ex.Message);
            return DefaultCategoryNotFound;
        }
    }

    internal string GetLastImageFilename(int categoryId)
    {
        if (categoryId <= 0)
            return string.Empty;

        // Use self-reference to avoid circular dependency
        var lastImage = _entityContext.TblImages
                            .Where(i => i.ImageCategoryId == categoryId || i.ImageFamilyId == categoryId || i.ImageMainFamilyId == categoryId)
                            .OrderByDescending(i => i.ImageUpdate)
                            .Select(i => i.ImageUrlName)
                            .FirstOrDefault() ?? "";

        // Fetch the last image filename for the specified category directly from database
        return lastImage;
    }

    /// <summary>
    /// Category paths for the category of an image, used in the new gallery feature.
    /// </summary>
    /// <param name="categoryId">category id in db</param>
    /// <returns>bilder/faglar/hackspettar/tretaig-hackspett , med /faglar och utan ÅÄÖ (AAO)</returns>
    public string GetCategoryPathForImage(int categoryId)
    {
        if (categoryId <= 0)
            return string.Empty;

        // Build the category path by traversing up the parent chain
        var segments = new List<string>();
        var currentId = categoryId;

        // Start with the current category and traverse up to the root category
        while (currentId > 0)
        {
            // Fetch the current category and its parent category from the database
            var category = _entityContext.TblMenus
                .Where(c => c.MenuCategoryId == currentId)
                .Select(c => new { c.MenuParentCategoryId, c.MenuUrlSegment })
                .FirstOrDefault();

            // If the category is not found, break the loop
            if (category == null)
                break;

            // Insert the URL segment at the beginning of the list
            if (!string.IsNullOrWhiteSpace(category.MenuUrlSegment))
                segments.Insert(0, category.MenuUrlSegment);

            // Move to the parent category, ensuring null safety
            currentId = category.MenuParentCategoryId.GetValueOrDefault();
        }

        return segments.Count > 0 ? string.Join("/", segments).ToLowerInvariant() : string.Empty;
    }

    /// <summary>
    /// Bulk load category paths for multiple category IDs to optimize database queries
    /// </summary>
    /// <param name="categoryIds">List of category IDs to get paths for</param>
    /// <returns>Dictionary mapping category ID to its full path</returns>
    public Dictionary<int, string> GetCategoryPathsBulk(List<int> categoryIds)
    {
        var result = new Dictionary<int, string>();

        if (!categoryIds.Any())
            return result;

        try
        {
            // Check if cache needs refreshing
            var now = DateTime.UtcNow;
            var needsCacheRefresh = now - _cacheLastUpdated > _cacheExpiry;

            // Find which category IDs are not in cache or cache is expired
            var uncachedIds = needsCacheRefresh
                ? categoryIds.ToList()
                : categoryIds.Where(id => !_categoryPathCache.ContainsKey(id)).ToList();

            if (uncachedIds.Any() || needsCacheRefresh)
            {
                // Get all categories that might be needed (including parents) in one query
                var allCategories = _entityContext.TblMenus
                    .Where(c => c.MenuCategoryId.HasValue)
                    .Select(c => new
                    {
                        Id = c.MenuCategoryId!.Value,
                        ParentId = c.MenuParentCategoryId,
                        UrlSegment = c.MenuUrlSegment
                    })
                    .ToList();

                // Create a lookup dictionary for faster access
                var categoryLookup = allCategories.ToDictionary(c => c.Id, c => (c.ParentId, c.UrlSegment));

                // If cache expired, clear it and rebuild for all categories
                if (needsCacheRefresh)
                {
                    _categoryPathCache.Clear();
                    // Build paths for all categories to warm the cache
                    foreach (var category in allCategories)
                    {
                        BuildAndCacheCategoryPath(category.Id, categoryLookup);
                    }
                    _cacheLastUpdated = now;
                }
                else
                {
                    // Build paths only for uncached category IDs
                    foreach (var categoryId in uncachedIds)
                    {
                        BuildAndCacheCategoryPath(categoryId, categoryLookup);
                    }
                }
            }

            // Return cached results for requested category IDs
            foreach (var categoryId in categoryIds)
            {
                result[categoryId] = _categoryPathCache.GetValueOrDefault(categoryId, string.Empty);
            }
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetCategoryPathsBulk: {Message}", ex.Message);
            // Return empty paths for failed lookups
            foreach (var categoryId in categoryIds)
            {
                result[categoryId] = string.Empty;
            }
        }

        return result;
    }

    /// <summary>
    /// Build and cache a category path for a specific category ID
    /// </summary>
    private void BuildAndCacheCategoryPath(int categoryId, Dictionary<int, (int? ParentId, string? UrlSegment)> categoryLookup)
    {
        if (categoryId <= 0)
        {
            _categoryPathCache[categoryId] = string.Empty;
            return;
        }

        var segments = new List<string>();
        var currentId = categoryId;

        // Traverse up the parent chain using in-memory data
        while (currentId > 0 && categoryLookup.TryGetValue(currentId, out var category))
        {
            // Insert the URL segment at the beginning of the list
            if (!string.IsNullOrWhiteSpace(category.UrlSegment))
                segments.Insert(0, category.UrlSegment);

            // Move to the parent category
            currentId = category.ParentId.GetValueOrDefault();
        }

        var path = segments.Count > 0 ? string.Join("/", segments).ToLowerInvariant() : string.Empty;
        _categoryPathCache[categoryId] = path;
    }
}
