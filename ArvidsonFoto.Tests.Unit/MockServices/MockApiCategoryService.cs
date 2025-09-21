using ArvidsonFoto.Core.ApiResponses;
using ArvidsonFoto.Core.DTOs;
using ArvidsonFoto.Core.Interfaces;

namespace ArvidsonFoto.Tests.Unit.MockServices;

/// <summary>
/// Mock implementation of IApiCategoryService for unit testing
/// Uses in-memory test data without external dependencies
/// </summary>
public class MockApiCategoryService : IApiCategoryService
{
    private readonly List<CategoryDto> _testCategories;

    public MockApiCategoryService()
    {
        // Create a fresh copy for each instance to avoid test interference
        _testCategories = new List<CategoryDto>
        {
            new CategoryDto
            {
                CategoryId = 1,
                Name = "Fåglar",
                UrlCategoryPath = "faglar",
                UrlCategoryPathFull = "faglar",
                UrlCategory = "bilder/faglar",
                UrlImage = "B57W4725",
                ParentCategoryId = null,
                ImageCount = 5,
                DateUpdated = DateTime.UtcNow
            },
            new CategoryDto
            {
                CategoryId = 10,
                Name = "Tättingar",
                UrlCategoryPath = "tattingar",
                UrlCategoryPathFull = "faglar/tattingar",
                UrlCategory = "bilder/faglar/tattingar",
                UrlImage = "B57W4725",
                ParentCategoryId = 1,
                ImageCount = 3,
                DateUpdated = DateTime.UtcNow
            },
            new CategoryDto
            {
                CategoryId = 12,
                Name = "Mesar",
                UrlCategoryPath = "mesar",
                UrlCategoryPathFull = "faglar/tattingar/mesar",
                UrlCategory = "bilder/faglar/tattingar/mesar",
                UrlImage = "B57W4725",
                ParentCategoryId = 10,
                ImageCount = 2,
                DateUpdated = DateTime.UtcNow
            },
            new CategoryDto
            {
                CategoryId = 13,
                Name = "Blåmes",
                UrlCategoryPath = "blames",
                UrlCategoryPathFull = "faglar/tattingar/mesar/blames",
                UrlCategory = "bilder/faglar/tattingar/mesar/blames",
                UrlImage = "B57W4725",
                ParentCategoryId = 12,
                ImageCount = 1,
                DateUpdated = DateTime.UtcNow
            },
            new CategoryDto
            {
                CategoryId = 2,
                Name = "Däggdjur",
                UrlCategoryPath = "daggdjur",
                UrlCategoryPathFull = "daggdjur",
                UrlCategory = "bilder/daggdjur",
                UrlImage = "B59W4837",
                ParentCategoryId = null,
                ImageCount = 1,
                DateUpdated = DateTime.UtcNow
            }
        };
    }

    public bool AddCategory(CategoryDto category)
    {
        if (category?.Name == null) return false;
        
        var newId = _testCategories.Max(c => c.CategoryId ?? 0) + 1;
        category.CategoryId = newId;
        _testCategories.Add(category);
        return true;
    }

    public int GetLastId()
    {
        return _testCategories.Max(c => c.CategoryId ?? 0);
    }

    public CategoryDto GetByName(string categoryName)
    {
        if (string.IsNullOrEmpty(categoryName))
            return CreateNotFoundCategory();

        var category = _testCategories.FirstOrDefault(c => 
            string.Equals(c.Name, categoryName, StringComparison.OrdinalIgnoreCase));
        
        return category ?? CreateNotFoundCategory();
    }

    public CategoryDto GetById(int? id)
    {
        if (id == null || id <= 0)
            return CreateNotFoundCategory();

        var category = _testCategories.FirstOrDefault(c => c.CategoryId == id);
        return category ?? CreateNotFoundCategory();
    }

    public List<CategoryDto> GetAll()
    {
        return new List<CategoryDto>(_testCategories);
    }

    public List<CategoryDto> GetChildrenByParentId(int categoryID)
    {
        return _testCategories
            .Where(c => c.ParentCategoryId == categoryID)
            .ToList();
    }

    public string GetNameById(int? id)
    {
        if (id == null || id <= 0) return "Not found";
        
        var category = _testCategories.FirstOrDefault(c => c.CategoryId == id);
        return category?.Name ?? "Not found";
    }

    public int GetIdByName(string categoryName)
    {
        if (string.IsNullOrEmpty(categoryName)) return -1;
        
        var category = _testCategories.FirstOrDefault(c => 
            string.Equals(c.Name, categoryName, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(c.UrlCategoryPath, categoryName, StringComparison.OrdinalIgnoreCase));
        
        return category?.CategoryId ?? -1;
    }

    public bool UpdateCategory(CategoryDto updatedCategory)
    {
        if (updatedCategory?.CategoryId == null) return false;
        
        var existingCategory = _testCategories.FirstOrDefault(c => c.CategoryId == updatedCategory.CategoryId);
        if (existingCategory == null) return false;
        
        existingCategory.Name = updatedCategory.Name;
        existingCategory.UrlCategoryPath = updatedCategory.UrlCategoryPath;
        existingCategory.DateUpdated = DateTime.UtcNow;
        return true;
    }

    public bool DeleteCategory(int? id)
    {
        if (id == null || id <= 0) return false;
        
        var category = _testCategories.FirstOrDefault(c => c.CategoryId == id);
        if (category == null) return false;
        
        _testCategories.Remove(category);
        return true;
    }

    public bool DeleteCategory(CategoryDto category)
    {
        return DeleteCategory(category?.CategoryId);
    }

    public string GetCategoryUrl(int? id)
    {
        if (id == null || id <= 0) return string.Empty;
        
        var category = _testCategories.FirstOrDefault(c => c.CategoryId == id);
        return category?.UrlCategory ?? string.Empty;
    }

    public MainMenuResponse GetMainMenu()
    {
        var mainCategories = _testCategories.Where(c => c.ParentCategoryId == null).ToList();
        var mainMenu = new MainMenuResponse();
        
        foreach (var category in mainCategories)
        {
            var subCategoryCount = _testCategories.Count(c => c.ParentCategoryId == category.CategoryId);
            mainMenu.MainMenu.Add(new MainMenuDto
            {
                MenuUrl = category.UrlCategory ?? string.Empty,
                MenuDisplayName = category.Name ?? string.Empty,
                SubCategoryCount = subCategoryCount,
                SortingUrlWithAao = category.UrlCategoryPathFull ?? string.Empty
            });
        }
        
        return mainMenu;
    }

    public void ClearCache()
    {
        // No-op for testing
    }

    public List<CategoryDto> GetMainCategories()
    {
        return _testCategories.Where(c => c.ParentCategoryId == null).ToList();
    }

    public Task<List<CategoryDto>> GetAllCategoriesAsync()
    {
        return Task.FromResult(GetAll());
    }

    public Task<bool> CreateCategoryAsync(CategoryDto category)
    {
        return Task.FromResult(AddCategory(category));
    }

    public List<CategoryDto> GetSubsList(int parentId)
    {
        return GetChildrenByParentId(parentId);
    }

    public List<CategoryDto> GetAllCategories()
    {
        return GetAll();
    }

    public int GetAllSubCategoriesCounted()
    {
        return _testCategories.Count(c => c.ParentCategoryId != null);
    }

    public CategoryDto GetByUrlSegment(string urlSegment)
    {
        if (string.IsNullOrEmpty(urlSegment))
            return CreateNotFoundCategory();

        var category = _testCategories.FirstOrDefault(c => 
            string.Equals(c.UrlCategoryPath, urlSegment, StringComparison.OrdinalIgnoreCase));
        
        return category ?? CreateNotFoundCategory();
    }

    public int GetIdByUrlSegment(string urlSegment)
    {
        if (string.IsNullOrEmpty(urlSegment)) return -1;
        
        var category = _testCategories.FirstOrDefault(c => 
            string.Equals(c.UrlCategoryPath, urlSegment, StringComparison.OrdinalIgnoreCase));
        
        return category?.CategoryId ?? -1;
    }

    public CategoryDto GetByUrlSegmentWithFallback(string urlSegment)
    {
        var category = GetByUrlSegment(urlSegment);
        if (category.CategoryId != -1) return category;
        
        // Try by ID if numeric
        if (int.TryParse(urlSegment, out int id))
        {
            category = GetById(id);
            if (category.CategoryId != -1) return category;
        }
        
        // Try by name
        category = GetByName(urlSegment);
        return category;
    }

    public string GetCategoryPathForImage(int categoryId)
    {
        if (categoryId <= 0) return string.Empty;
        
        var category = _testCategories.FirstOrDefault(c => c.CategoryId == categoryId);
        return category?.UrlCategoryPathFull ?? string.Empty;
    }

    public Dictionary<int, string> GetCategoryPathsBulk(List<int> categoryIds)
    {
        var result = new Dictionary<int, string>();
        foreach (var id in categoryIds)
        {
            result[id] = GetCategoryPathForImage(id);
        }
        return result;
    }

    private static CategoryDto CreateNotFoundCategory()
    {
        return new CategoryDto
        {
            CategoryId = -1,
            Name = "Not found",
            UrlCategory = "bilder/404-NotFound",
            UrlCategoryPath = "404-NotFound",
            UrlCategoryPathFull = "404-NotFound",
            DateUpdated = DateTime.UtcNow
        };
    }
}