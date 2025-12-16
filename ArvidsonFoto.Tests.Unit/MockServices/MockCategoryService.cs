using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Models;
using ArvidsonFoto.Services;

namespace ArvidsonFoto.Tests.Unit.MockServices;

public class MockCategoryService : ICategoryService
{
    public bool AddCategory(TblMenu category) => true;
    
    public int GetLastId() => 
        ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_MenuCategories.Max(x => x.MenuCategoryId ?? 0);
    
    public int GetIdByName(string categoryName) =>
        ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_MenuCategories.FirstOrDefault(c => c.MenuDisplayName != null && c.MenuDisplayName.Equals(categoryName))?.MenuCategoryId ?? -1;

    public string GetNameById(int? id) => 
        ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_MenuCategories.FirstOrDefault(c => c.MenuCategoryId.Equals(id))?.MenuDisplayName ?? "Hittas inte i ArvidsonFotoCoreDbSeeder.cs";
    
    public TblMenu GetById(int? id)
    {
        var coreMenu = ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_MenuCategories.FirstOrDefault(c => c.MenuCategoryId.Equals(id));
        if (coreMenu == null) return new TblMenu();
        
        return new TblMenu
        {
            Id = coreMenu.Id ?? 0,
            MenuId = coreMenu.MenuCategoryId ?? 0,
            MenuMainId = coreMenu.MenuParentCategoryId ?? 0,
            MenuText = coreMenu.MenuDisplayName ?? string.Empty,
            MenuUrltext = coreMenu.MenuUrlSegment ?? string.Empty
        };
    }

    public TblMenu GetByName(string categoryName)
    {
        var coreMenu = ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_MenuCategories.FirstOrDefault(c => 
            c.MenuUrlSegment != null && c.MenuUrlSegment.Equals(categoryName));
        
        if (coreMenu == null)
        {
            // Return null to match real CategoryService behavior - controller checks for null
            return null;
        }
        
        return new TblMenu
        {
            Id = coreMenu.Id ?? 0,
            MenuId = coreMenu.MenuCategoryId ?? 0,
            MenuMainId = coreMenu.MenuParentCategoryId ?? 0,
            MenuText = coreMenu.MenuDisplayName ?? string.Empty,
            MenuUrltext = coreMenu.MenuUrlSegment ?? string.Empty
        };
    }
    
    public List<TblMenu> GetAll() => 
        ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_MenuCategories.Select(c => new TblMenu
        {
            Id = c.Id ?? 0,
            MenuId = c.MenuCategoryId ?? 0,
            MenuMainId = c.MenuParentCategoryId ?? 0,
            MenuText = c.MenuDisplayName ?? string.Empty,
            MenuUrltext = c.MenuUrlSegment ?? string.Empty
        }).ToList() ?? new List<TblMenu>();
    
    public List<TblMenu> GetSubsList(int categoryID) => 
        ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_MenuCategories
            .Where(x => x.MenuParentCategoryId.Equals(categoryID))
            .Select(c => new TblMenu
            {
                Id = c.Id ?? 0,
                MenuId = c.MenuCategoryId ?? 0,
                MenuMainId = c.MenuParentCategoryId ?? 0,
                MenuText = c.MenuDisplayName ?? string.Empty,
                MenuUrltext = c.MenuUrlSegment ?? string.Empty
            }).ToList() ?? new List<TblMenu>();
}
