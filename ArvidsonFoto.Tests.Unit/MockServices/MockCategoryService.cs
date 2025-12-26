using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.Models;
using ArvidsonFoto.Services;

namespace ArvidsonFoto.Tests.Unit.MockServices;

/// <summary>
/// Mock implementation of ICategoryService using Core namespace
/// </summary>
public class MockCategoryService : ICategoryService
{
    public bool AddCategory(TblMenu category) => true;
    
    public int GetLastId() => 
        ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_MenuCategories.Max(x => x.MenuCategoryId ?? 0);
    
    public int GetIdByName(string categoryName) =>
        ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_MenuCategories
            .FirstOrDefault(c => c.MenuDisplayName != null && c.MenuDisplayName.Equals(categoryName))?.MenuCategoryId ?? -1;

    public string GetNameById(int? id) => 
        ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_MenuCategories
            .FirstOrDefault(c => c.MenuCategoryId.Equals(id))?.MenuDisplayName ?? "Not found";
    
    public TblMenu GetById(int? id)
    {
        var coreMenu = ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_MenuCategories
            .FirstOrDefault(c => c.MenuCategoryId.Equals(id));
        
        if (coreMenu == null) 
            return new TblMenu();
        
        return coreMenu;
    }

    public TblMenu GetByName(string categoryName)
    {
        var coreMenu = ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_MenuCategories
            .FirstOrDefault(c => c.MenuDisplayName != null && c.MenuDisplayName.Equals(categoryName));
        
        if (coreMenu == null)
        {
            return new TblMenu(); // Return empty instance
        }
        
        return coreMenu;
    }
    
    public List<TblMenu> GetAll() => 
        ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_MenuCategories.ToList();
    
    public List<TblMenu> GetSubsList(int categoryID) => 
        ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_MenuCategories
            .Where(x => x.MenuParentCategoryId.Equals(categoryID))
            .ToList();
}
