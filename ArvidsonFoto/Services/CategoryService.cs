using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.Models;

namespace ArvidsonFoto.Services;

/// <summary>
/// Legacy category service - migrated to use Core namespace
/// </summary>
/// <remarks>
/// Consider migrating to ApiCategoryService for new functionality
/// </remarks>
public class CategoryService : ICategoryService
{
    // Databas koppling - uppdaterad till Core.Data
    private readonly ArvidsonFotoCoreDbContext _entityContext;
    
    public CategoryService(ArvidsonFotoCoreDbContext context)
    {
        _entityContext = context;
    }

    public bool AddCategory(TblMenu category)
    {
        bool success = false;
        try
        {
            _entityContext.TblMenus.Add(category);
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

    public int GetLastId()
    {
        try
        {
            var lastCategory = _entityContext.TblMenus
                .OrderByDescending(c => c.MenuCategoryId)
                .FirstOrDefault();
            return lastCategory?.MenuCategoryId ?? -1;
        }
        catch
        {
            return -1;
        }
    }

    public TblMenu GetByName(string categoryName)
    {
        TblMenu? category = _entityContext.TblMenus
            .FirstOrDefault(c => c.MenuDisplayName != null && c.MenuDisplayName.Equals(categoryName));
        
        if (category is null)
        {
            Log.Warning("Could not find category: '" + categoryName + "'");
            return new TblMenu(); // Return empty instead of null
        }
        return category;
    }

    public TblMenu GetById(int? id)
    {
        TblMenu? category = _entityContext.TblMenus
            .FirstOrDefault(c => c.MenuCategoryId.Equals(id));
        
        if (category is null)
        {
            Log.Information("Could not find id with number: " + id);
            return new TblMenu(); // Return empty instead of null
        }
        return category;
    }

    public List<TblMenu> GetAll()
    {
        return _entityContext.TblMenus.ToList();
    }

    public List<TblMenu> GetSubsList(int categoryID)
    {
        return _entityContext.TblMenus
            .Where(c => c.MenuParentCategoryId.Equals(categoryID))
            .ToList();
    }

    public string GetNameById(int? id)
    {
        if (id == null || id <= 0)
            return "";

        var category = _entityContext.TblMenus
            .FirstOrDefault(c => c.MenuCategoryId.Equals(id));
        
        return category?.MenuDisplayName ?? "";
    }

    public int GetIdByName(string categoryName)
    {
        if (string.IsNullOrEmpty(categoryName))
            return -1;

        var category = _entityContext.TblMenus
            .FirstOrDefault(c => c.MenuDisplayName != null && c.MenuDisplayName.Equals(categoryName));
        
        return category?.MenuCategoryId ?? -1;
    }
}