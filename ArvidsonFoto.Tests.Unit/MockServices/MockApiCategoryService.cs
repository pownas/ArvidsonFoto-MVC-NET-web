using ArvidsonFoto.Core.ApiResponses;
using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.DTOs;
using ArvidsonFoto.Core.Interfaces;

namespace ArvidsonFoto.Tests.Unit.MockServices;

/// <summary>
/// Mock implementation of IApiCategoryService for unit testing
/// Uses data from ArvidsonFotoCoreDbSeeder to match production data structure
/// </summary>
public class MockApiCategoryService : IApiCategoryService
{
    private readonly List<CategoryDto> _testCategories;

    public MockApiCategoryService()
    {
        // Convert ArvidsonFotoCoreDbSeeder data to CategoryDto format
        _testCategories = ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_MenuCategories.Select(menu => new CategoryDto
        {
            CategoryId = menu.MenuCategoryId ?? 0,
            NameSv = menu.MenuDisplayName ?? string.Empty,
            NameEn = menu.MenuDisplayNameEn,
            UrlCategoryPathSv = menu.MenuUrlSegment ?? $"category-{menu.MenuCategoryId ?? 0}",
            UrlCategoryPathEn = menu.MenuUrlSegmentEn,
            UrlCategoryPathFullSv = BuildCategoryPath(menu.MenuCategoryId ?? 0),
            UrlCategorySv = $"bilder/{menu.MenuUrlSegment ?? $"category-{menu.MenuCategoryId ?? 0}"}",
            UrlCategoryPathFullEn = BuildEnglishCategoryPath(menu.MenuCategoryId ?? 0),
            UrlCategoryEn = $"images/{BuildEnglishCategoryPath(menu.MenuCategoryId ?? 0)}",
            UrlImage = GetDefaultImageForCategory(menu.MenuCategoryId ?? 0),
            ParentCategoryId = menu.MenuParentCategoryId == 0 ? null : menu.MenuParentCategoryId,
            ImageCount = GetImageCountForCategory(menu.MenuCategoryId ?? 0),
            DateUpdated = menu.MenuDateUpdated ?? DateTime.UtcNow
        }).ToList();
    }

    private static string BuildCategoryPath(int menuId)
    {
        var menu = ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_MenuCategories.FirstOrDefault(m => m.MenuCategoryId == menuId);
        if (menu == null)
        {
            return string.Empty;
        }

        var pathParts = new List<string>();
        var currentMenu = menu;

        while (currentMenu != null)
        {
            // Skip "Fåglar" category (ID = 1) as it's not a physical folder
            if (currentMenu.MenuCategoryId != 1 && !string.IsNullOrWhiteSpace(currentMenu.MenuUrlSegment))
            {
                pathParts.Insert(0, currentMenu.MenuUrlSegment ?? $"category-{currentMenu.MenuCategoryId ?? 0}");
            }

            if (currentMenu.MenuParentCategoryId is 0 or null)
            {
                break;
            }

            currentMenu = ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_MenuCategories.FirstOrDefault(m => m.MenuCategoryId == currentMenu.MenuParentCategoryId);
        }

        return string.Join("/", pathParts);
    }

    private static string BuildEnglishCategoryPath(int menuId)
    {
        var menu = ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_MenuCategories.FirstOrDefault(item => item.MenuCategoryId == menuId);
        if (menu == null)
        {
            return string.Empty;
        }

        var parentPath = menu.MenuParentCategoryId is 0 or null
            ? string.Empty
            : BuildEnglishCategoryPath(menu.MenuParentCategoryId.Value);
        var segment = menu.MenuCategoryId == 1 ? string.Empty : menu.MenuUrlSegmentEn ?? menu.MenuUrlSegment;
        return string.Join("/", new[] { parentPath, segment }.Where(value => !string.IsNullOrWhiteSpace(value)));
    }

    private static string GetDefaultImageForCategory(int menuId)
    {
        // Try to find an image for this category
        var image = ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_Image
            .FirstOrDefault(i => i.ImageCategoryId == menuId || i.ImageFamilyId == menuId || i.ImageMainFamilyId == menuId);

        return image?.ImageUrlName ?? "default-image";
    }

    private static int GetImageCountForCategory(int menuId)
    {
        return ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_Image
            .Count(i => i.ImageCategoryId == menuId || i.ImageFamilyId == menuId || i.ImageMainFamilyId == menuId);
    }

    public bool AddCategory(CategoryDto category)
    {
        if (category == null || string.IsNullOrWhiteSpace(category.NameSv))
        {
            return false;
        }

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
        {
            return CreateNotFoundCategory();
        }

        var category = _testCategories.FirstOrDefault(c =>
            string.Equals(c.NameSv, categoryName, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(c.NameEn, categoryName, StringComparison.OrdinalIgnoreCase));

        return category ?? CreateNotFoundCategory();
    }

    public CategoryDto GetById(int? id)
    {
        if (id is null or <= 0)
        {
            return CreateNotFoundCategory();
        }

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
        if (id is null or <= 0)
        {
            return "Not found";
        }

        var category = _testCategories.FirstOrDefault(c => c.CategoryId == id);
        return category?.NameSv ?? "Not found";
    }

    public int GetIdByName(string categoryName)
    {
        if (string.IsNullOrEmpty(categoryName))
        {
            return -1;
        }

        var category = _testCategories.FirstOrDefault(c =>
            string.Equals(c.NameSv, categoryName, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(c.NameEn, categoryName, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(c.UrlCategoryPathSv, categoryName, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(c.UrlCategoryPathEn, categoryName, StringComparison.OrdinalIgnoreCase));

        return category?.CategoryId ?? -1;
    }

    public bool UpdateCategory(CategoryDto updatedCategory)
    {
        if (updatedCategory?.CategoryId == null)
        {
            return false;
        }

        var existingCategory = _testCategories.FirstOrDefault(c => c.CategoryId == updatedCategory.CategoryId);
        if (existingCategory == null)
        {
            return false;
        }

        existingCategory.NameSv = updatedCategory.NameSv;
        existingCategory.NameEn = updatedCategory.NameEn;
        existingCategory.UrlCategoryPathSv = updatedCategory.UrlCategoryPathSv;
        existingCategory.UrlCategoryPathEn = updatedCategory.UrlCategoryPathEn;
        existingCategory.DateUpdated = DateTime.UtcNow;
        return true;
    }

    public bool DeleteCategory(int? id)
    {
        if (id is null or <= 0)
        {
            return false;
        }

        var category = _testCategories.FirstOrDefault(c => c.CategoryId == id);
        if (category == null)
        {
            return false;
        }

        _testCategories.Remove(category);
        return true;
    }

    public bool DeleteCategory(CategoryDto category)
    {
        return DeleteCategory(category?.CategoryId);
    }

    public string GetCategoryUrl(int? id)
    {
        if (id is null or <= 0)
        {
            return string.Empty;
        }

        var category = _testCategories.FirstOrDefault(c => c.CategoryId == id);
        return category?.UrlCategorySv ?? string.Empty;
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
                MenuUrl = category.UrlCategorySv ?? string.Empty,
                MenuDisplayName = category.NameSv,
                SubCategoryCount = subCategoryCount,
                SortingUrlWithAao = category.UrlCategoryPathFullSv ?? string.Empty
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
        {
            return CreateNotFoundCategory();
        }

        var category = _testCategories.FirstOrDefault(c =>
            string.Equals(c.UrlCategoryPathSv, urlSegment, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(c.UrlCategoryPathEn, urlSegment, StringComparison.OrdinalIgnoreCase));

        return category ?? CreateNotFoundCategory();
    }

    public int GetIdByUrlSegment(string urlSegment)
    {
        if (string.IsNullOrEmpty(urlSegment))
        {
            return -1;
        }

        var category = _testCategories.FirstOrDefault(c =>
            string.Equals(c.UrlCategoryPathSv, urlSegment, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(c.UrlCategoryPathEn, urlSegment, StringComparison.OrdinalIgnoreCase));

        return category?.CategoryId ?? -1;
    }

    public CategoryDto GetByUrlSegmentWithFallback(string urlSegment)
    {
        var category = GetByUrlSegment(urlSegment);
        if (category.CategoryId != -1)
        {
            return category;
        }

        // Try by ID if numeric
        if (int.TryParse(urlSegment, out int id))
        {
            category = GetById(id);
            if (category.CategoryId != -1)
            {
                return category;
            }
        }

        // Try by name
        category = GetByName(urlSegment);
        return category;
    }

    public string GetCategoryPathForImage(int categoryId)
    {
        if (categoryId <= 0)
        {
            return string.Empty;
        }

        var category = _testCategories.FirstOrDefault(c => c.CategoryId == categoryId);
        return category?.UrlCategoryPathFullSv ?? string.Empty;
    }

    public string GetCategoryDisplayPathForImage(int categoryId)
    {
        if (categoryId <= 0)
        {
            return string.Empty;
        }

        // Build display path with ÅÄÖ from category names
        var menu = ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_MenuCategories.FirstOrDefault(m => m.MenuCategoryId == categoryId);
        if (menu == null)
        {
            return string.Empty;
        }

        var pathParts = new List<string>();
        var currentMenu = menu;

        while (currentMenu != null)
        {
            // Skip "Fåglar" category (ID = 1) as it's not a physical folder
            if (currentMenu.MenuCategoryId != 1 && !string.IsNullOrWhiteSpace(currentMenu.MenuDisplayName))
            {
                pathParts.Insert(0, currentMenu.MenuDisplayName);
            }

            if (currentMenu.MenuParentCategoryId is 0 or null)
            {
                break;
            }

            currentMenu = ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_MenuCategories.FirstOrDefault(m => m.MenuCategoryId == currentMenu.MenuParentCategoryId);
        }

        return string.Join("/", pathParts);
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

    public Dictionary<int, string> GetCategoryNamesBulk(List<int> categoryIds)
    {
        var result = new Dictionary<int, string>();
        foreach (var id in categoryIds)
        {
            result[id] = GetNameById(id);
        }
        return result;
    }

    public List<int> GetAllDescendantCategoryIds(int categoryId)
    {
        var result = new List<int>();
        var queue = new Queue<int>();
        queue.Enqueue(categoryId);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var children = _testCategories
                .Where(c => c.ParentCategoryId == current)
                .ToList();
            foreach (var child in children)
            {
                if (child.CategoryId.HasValue)
                {
                    result.Add(child.CategoryId.Value);
                    queue.Enqueue(child.CategoryId.Value);
                }
            }
        }

        return result;
    }

    private static CategoryDto CreateNotFoundCategory()
    {
        return new CategoryDto
        {
            CategoryId = -1,
            NameSv = "Not found",
            UrlCategorySv = "bilder/404-NotFound",
            UrlCategoryPathSv = "404-NotFound",
            UrlCategoryPathFullSv = "404-NotFound",
            DateUpdated = DateTime.UtcNow
        };
    }
}