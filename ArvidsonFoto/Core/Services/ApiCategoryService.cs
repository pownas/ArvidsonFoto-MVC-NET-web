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

namespace ArvidsonFoto.Core.Services;

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
            logger.LogError("Error adding category: {Message}", ex.Message);
            throw new Exception("Fel vid skapande av kategori. Felmeddelande: " + ex.Message);
        }
        return success;
    }

    public int GetLastId()
    {
        try
        {
            var lastCategory = _entityContext.TblMenus.OrderByDescending(c => c.MenuCategoryId).FirstOrDefault();
            return lastCategory?.MenuCategoryId ?? -1;
        }
        catch (Exception ex)
        {
            logger.LogError("Error getting last category ID: {Message}", ex.Message);
            Log.Error("GetLastId failed: {Message}", ex.Message);
            return -1;
        }
    }

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

    public List<CategoryDto> GetAll()
    {
        if (_cache.TryGetValue(ALL_CATEGORIES_CACHE_KEY, out List<CategoryDto>? cachedCategories) && cachedCategories != null)
        {
            Log.Debug("All categories retrieved from cache ({Count} categories)", cachedCategories.Count);
            return cachedCategories;
        }

        var categories = _entityContext.TblMenus.ToList();
        var categoryDtos = new List<CategoryDto>();
        foreach (var category in categories)
        {
            var categoryPath = GetCategoryPathForImage(category.MenuCategoryId ?? -1);
            var lastImageFilename = GetLastImageFilename(category.MenuCategoryId ?? -1);
            categoryDtos.Add(category.ToCategoryDto(categoryPath, lastImageFilename));
        }

        _cache.Set(ALL_CATEGORIES_CACHE_KEY, categoryDtos, _shortCacheExpiry);
        Log.Debug("All categories cached ({Count} categories)", categoryDtos.Count);
        return categoryDtos;
    }

    public List<CategoryDto> GetChildrenByParentId(int categoryID)
    {
        try
        {
            var categories = _entityContext.TblMenus
                .Where(c => c.MenuParentCategoryId == categoryID)
                .OrderBy(c => c.MenuDisplayName)
                .ToList();

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

    public string GetNameById(int? id)
    {
        if (id == null || id <= 0)
        {
            Log.Information("Invalid category id for GetNameById: {Id}", id);
            return "Not found";
        }

        try
        {
            var category = _entityContext.TblMenus.FirstOrDefault(c => c.MenuCategoryId == id.Value);
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

    public int GetIdByName(string categoryName)
    {
        if (string.IsNullOrEmpty(categoryName))
        {
            Log.Information("Empty category name provided to GetIdByName");
            return -1;
        }

        try
        {
            var category = _entityContext.TblMenus.FirstOrDefault(c => c.MenuUrlSegment!.ToLower() == categoryName.ToLower());
            if (category == null)
                category = _entityContext.TblMenus.FirstOrDefault(c => c.MenuDisplayName!.ToLower() == categoryName.ToLower());
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

    public bool DeleteCategory(CategoryDto category)
    {
        return DeleteCategory(category.CategoryId);
    }

    public string GetCategoryUrl(int? id)
    {
        if (id == null || id <= 0)
        {
            Log.Warning("Invalid category ID provided: {CategoryId}. Must be a positive integer.", id);
            return $"{(int)HttpStatusCode.BadRequest}-{HttpStatusCode.BadRequest}. Must be a positive integer: {id}";
        }

        var segments = new List<string>();
        var currentId = id;

        while (currentId != null && currentId > 0)
        {
            var category = _entityContext.TblMenus
                .Where(c => c.MenuCategoryId == currentId)
                .Select(c => new { c.MenuParentCategoryId, c.MenuUrlSegment, c.Id })
                .FirstOrDefault();

            if (category == null)
            {
                Log.Warning("Could not find category or parent with ID: {CategoryId}", currentId);
                return $"{(int)HttpStatusCode.NotFound}-{HttpStatusCode.NotFound}. Could not find category or parent with ID: {currentId}";
            }

            if (!string.IsNullOrWhiteSpace(category.MenuUrlSegment))
                segments.Insert(0, category.MenuUrlSegment);

            currentId = category.MenuParentCategoryId;
        }

        if (segments.Count == 0)
            return $"{(int)HttpStatusCode.NotFound}-{HttpStatusCode.NotFound}. Could not find category or parent with ID: {id}";

        var url = "/" + string.Join("/", segments).Trim().ToLowerInvariant();
        return url;
    }

    private string GetSortingUrl(int? id)
    {
        if (id == null || id <= 0)
        {
            Log.Warning("Invalid category ID provided: {CategoryId}. Must be a positive integer.", id);
            return $"{(int)HttpStatusCode.BadRequest}-{HttpStatusCode.BadRequest}. Must be a positive integer: {id}";
        }

        var segments = new List<string>();
        var currentId = id;

        while (currentId != null && currentId > 0)
        {
            var category = _entityContext.TblMenus
                .Where(c => c.MenuCategoryId == currentId)
                .Select(c => new { c.MenuParentCategoryId, c.MenuDisplayName, c.Id })
                .FirstOrDefault();

            if (category == null)
            {
                Log.Warning("Could not find category or parent with ID: {CategoryId}", currentId);
                return $"{(int)HttpStatusCode.NotFound}-{HttpStatusCode.NotFound}. Could not find category or parent with ID: {currentId}";
            }

            if (!string.IsNullOrWhiteSpace(category.MenuDisplayName))
                segments.Insert(0, category.MenuDisplayName);

            currentId = category.MenuParentCategoryId;
        }

        if (segments.Count == 0)
            return $"{(int)HttpStatusCode.NotFound}-{HttpStatusCode.NotFound}. Could not find category or parent with ID: {id}";

        var url = "/" + string.Join("/", segments).Trim().ToLowerInvariant();
        return url;
    }

    public MainMenuResponse GetMainMenu()
    {
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
            return mainMenu;
        }

        for (int i = 0; i < categories.Count; i++)
        {
            if (!string.IsNullOrWhiteSpace(categories[i].MenuUrlSegment) && !string.IsNullOrWhiteSpace(categories[i].MenuDisplayName))
            {
                mainMenu.MainMenu.Add(new MainMenuDto
                {
                    MenuUrl = GetCategoryUrl(categories[i].MenuCategoryId),
                    MenuDisplayName = categories[i].MenuDisplayName ?? string.Empty,
                    SubCategoryCount = _entityContext.TblMenus.Count(c => c.MenuParentCategoryId == categories[i].MenuCategoryId),
                    SortingUrlWithAao = GetSortingUrl(categories[i].MenuCategoryId),
                });
            }
        }

        mainMenu.MainMenu = mainMenu.MainMenu.OrderBy(x => x.SortingUrlWithAao).ToList();
        _cache.Set(MAIN_MENU_CACHE_KEY, mainMenu, _longCacheExpiry);
        Log.Debug("Main menu cached with {Count} items", mainMenu.MainMenu.Count);

        return mainMenu;
    }

    public void ClearCache()
    {
        _cache.Remove(ALL_CATEGORIES_CACHE_KEY);
        _cache.Remove(MAIN_CATEGORIES_CACHE_KEY);
        _cache.Remove(MAIN_MENU_CACHE_KEY);
        _categoryPathCache.Clear();
        _cacheLastUpdated = DateTime.MinValue;
        Log.Information("All category caches cleared");
    }

    public List<CategoryDto> GetMainCategories()
    {
        if (_cache.TryGetValue(MAIN_CATEGORIES_CACHE_KEY, out List<CategoryDto>? cachedMainCategories) && cachedMainCategories != null)
        {
            Log.Debug("Main categories retrieved from cache ({Count} categories)", cachedMainCategories.Count);
            return cachedMainCategories;
        }

        try
        {
            var testCategories = new List<CategoryDto>
            {
                new CategoryDto { CategoryId = 1, Name = "Däggdjur", UrlCategoryPath = "daggdjur", UrlCategoryPathFull = "daggdjur" },
                new CategoryDto { CategoryId = 2, Name = "Fåglar", UrlCategoryPath = "faglar", UrlCategoryPathFull = "faglar" },
                new CategoryDto { CategoryId = 3, Name = "Insekter", UrlCategoryPath = "insekter", UrlCategoryPathFull = "insekter" },
                new CategoryDto { CategoryId = 4, Name = "Landskap", UrlCategoryPath = "landskap", UrlCategoryPathFull = "landskap" },
                new CategoryDto { CategoryId = 5, Name = "Växter", UrlCategoryPath = "vaxter", UrlCategoryPathFull = "vaxter" }
            };

            _cache.Set(MAIN_CATEGORIES_CACHE_KEY, testCategories, _longCacheExpiry);
            Log.Debug("Main categories cached ({Count} categories)", testCategories.Count);
            return testCategories;
        }
        catch (Exception ex)
        {
            Log.Error("GetMainCategories failed: {Message}", ex.Message);
            return new List<CategoryDto>();
        }
    }

    public Task<List<CategoryDto>> GetAllCategoriesAsync() => Task.FromResult(GetAll());
    public Task<bool> CreateCategoryAsync(CategoryDto category) => Task.FromResult(AddCategory(category));
    public List<CategoryDto> GetSubsList(int parentId) => GetChildrenByParentId(parentId);
    public List<CategoryDto> GetAllCategories() => GetAll();

    public int GetAllSubCategoriesCounted()
    {
        return _entityContext.TblMenus.Count(m => m.MenuParentCategoryId.HasValue && m.MenuParentCategoryId > 0);
    }

    public CategoryDto GetByUrlSegment(string urlSegment)
    {
        if (string.IsNullOrEmpty(urlSegment))
        {
            Log.Information("Empty URL segment provided to GetByUrlSegment");
            return DefaultCategoryNotFound;
        }

        try
        {
            var category = _entityContext.TblMenus.FirstOrDefault(c => c.MenuUrlSegment != null && c.MenuUrlSegment.ToLower() == urlSegment.ToLower());
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

    public int GetIdByUrlSegment(string urlSegment)
    {
        if (string.IsNullOrEmpty(urlSegment))
        {
            Log.Information("Empty URL segment provided to GetIdByUrlSegment");
            return -1;
        }

        try
        {
            var category = _entityContext.TblMenus.FirstOrDefault(c => c.MenuUrlSegment!.ToLower() == urlSegment.ToLower());
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

    public CategoryDto GetByUrlSegmentWithFallback(string urlSegment)
    {
        if (string.IsNullOrEmpty(urlSegment))
        {
            Log.Information("Empty URL segment provided to GetByUrlSegmentWithFallback");
            return DefaultCategoryNotFound;
        }

        try
        {
            var category = _entityContext.TblMenus.FirstOrDefault(c => c.MenuUrlSegment!.ToLower() == urlSegment.ToLower());
            if (category != null)
            {
                var categoryPath = GetCategoryPathForImage(category.MenuCategoryId ?? -1);
                var lastImageFilename = GetLastImageFilename(category.MenuCategoryId ?? -1);
                var categoryImageCount = dbContext.TblImages.Where(x => x.ImageCategoryId == category.MenuCategoryId).Count();
                return category.ToCategoryDto(categoryPath, lastImageFilename, categoryImageCount);
            }

            if (int.TryParse(urlSegment, out int categoryId))
            {
                category = _entityContext.TblMenus.FirstOrDefault(c => c.MenuCategoryId == categoryId);
                if (category != null)
                {
                    Log.Information("Found category by ID fallback: {Id}", categoryId);
                    var categoryPath = GetCategoryPathForImage(category.MenuCategoryId ?? -1);
                    var lastImageFilename = GetLastImageFilename(category.MenuCategoryId ?? -1);
                    var categoryImageCount = dbContext.TblImages.Where(x => x.ImageCategoryId == category.MenuCategoryId).Count();
                    return category.ToCategoryDto(categoryPath, lastImageFilename, categoryImageCount);
                }
            }

            category = _entityContext.TblMenus.FirstOrDefault(c => c.MenuDisplayName!.ToLower() == urlSegment.ToLower());
            if (category != null)
            {
                Log.Information("Found category by display name fallback: {Name}", urlSegment);
                var categoryPath = GetCategoryPathForImage(category.MenuCategoryId ?? -1);
                var lastImageFilename = GetLastImageFilename(category.MenuCategoryId ?? -1);
                var categoryImageCount = dbContext.TblImages.Where(x => x.ImageCategoryId == category.MenuCategoryId).Count();
                return category.ToCategoryDto(categoryPath, lastImageFilename, categoryImageCount);
            }

            category = _entityContext.TblMenus.FirstOrDefault(c => c.MenuUrlSegment!.ToLower().Contains(urlSegment.ToLower()) || c.MenuDisplayName!.ToLower().Contains(urlSegment.ToLower()));
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

        var lastImage = _entityContext.TblImages
                            .Where(i => i.ImageCategoryId == categoryId || i.ImageFamilyId == categoryId || i.ImageMainFamilyId == categoryId)
                            .OrderByDescending(i => i.ImageUpdate)
                            .Select(i => i.ImageUrlName)
                            .FirstOrDefault() ?? "";

        return lastImage;
    }

    public string GetCategoryPathForImage(int categoryId)
    {
        if (categoryId <= 0)
            return string.Empty;

        var segments = new List<string>();
        var currentId = categoryId;

        while (currentId > 0)
        {
            var category = _entityContext.TblMenus
                .Where(c => c.MenuCategoryId == currentId)
                .Select(c => new { c.MenuParentCategoryId, c.MenuUrlSegment })
                .FirstOrDefault();

            if (category == null)
                break;

            if (!string.IsNullOrWhiteSpace(category.MenuUrlSegment))
                segments.Insert(0, category.MenuUrlSegment);

            currentId = category.MenuParentCategoryId.GetValueOrDefault();
        }

        return segments.Count > 0 ? string.Join("/", segments).ToLowerInvariant() : string.Empty;
    }

    public Dictionary<int, string> GetCategoryPathsBulk(List<int> categoryIds)
    {
        var result = new Dictionary<int, string>();

        if (!categoryIds.Any())
            return result;

        try
        {
            var now = DateTime.UtcNow;
            var needsCacheRefresh = now - _cacheLastUpdated > _cacheExpiry;
            var uncachedIds = needsCacheRefresh ? categoryIds.ToList() : categoryIds.Where(id => !_categoryPathCache.ContainsKey(id)).ToList();

            if (uncachedIds.Any() || needsCacheRefresh)
            {
                var allCategories = _entityContext.TblMenus
                    .Where(c => c.MenuCategoryId.HasValue)
                    .Select(c => new { Id = c.MenuCategoryId!.Value, ParentId = c.MenuParentCategoryId, UrlSegment = c.MenuUrlSegment })
                    .ToList();

                var categoryLookup = allCategories.ToDictionary(c => c.Id, c => (c.ParentId, c.UrlSegment));

                if (needsCacheRefresh)
                {
                    _categoryPathCache.Clear();
                    foreach (var category in allCategories)
                    {
                        BuildAndCacheCategoryPath(category.Id, categoryLookup);
                    }
                    _cacheLastUpdated = now;
                }
                else
                {
                    foreach (var categoryId in uncachedIds)
                    {
                        BuildAndCacheCategoryPath(categoryId, categoryLookup);
                    }
                }
            }

            foreach (var categoryId in categoryIds)
            {
                result[categoryId] = _categoryPathCache.GetValueOrDefault(categoryId, string.Empty);
            }
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetCategoryPathsBulk: {Message}", ex.Message);
            foreach (var categoryId in categoryIds)
            {
                result[categoryId] = string.Empty;
            }
        }

        return result;
    }

    private void BuildAndCacheCategoryPath(int categoryId, Dictionary<int, (int? ParentId, string? UrlSegment)> categoryLookup)
    {
        if (categoryId <= 0)
        {
            _categoryPathCache[categoryId] = string.Empty;
            return;
        }

        var segments = new List<string>();
        var currentId = categoryId;

        while (currentId > 0 && categoryLookup.TryGetValue(currentId, out var category))
        {
            if (!string.IsNullOrWhiteSpace(category.UrlSegment))
                segments.Insert(0, category.UrlSegment);

            currentId = category.ParentId.GetValueOrDefault();
        }

        var path = segments.Count > 0 ? string.Join("/", segments).ToLowerInvariant() : string.Empty;
        _categoryPathCache[categoryId] = path;
    }
}
