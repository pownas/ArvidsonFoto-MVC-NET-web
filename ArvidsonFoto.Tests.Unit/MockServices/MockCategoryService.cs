using ArvidsonFoto.Data;
using ArvidsonFoto.Models;
using ArvidsonFoto.Services;

namespace ArvidsonFoto.Tests.Unit.MockServices;

public class MockCategoryService : ICategoryService
{
    public bool AddCategory(TblMenu category) => true;
    
    public int GetLastId() => 
        DbSeederExtension.DbSeed_Tbl_Menu.Max(x => x.MenuId);
    
    public int GetIdByName(string categoryName) =>
        DbSeederExtension.DbSeed_Tbl_Menu.FirstOrDefault(c => c.MenuText.Equals(categoryName))?.MenuId ?? -1;

    public string GetNameById(int? id) => 
        DbSeederExtension.DbSeed_Tbl_Menu.FirstOrDefault(c => c.MenuId.Equals(id))?.MenuText ?? "Hittas inte i DbSeederExtension.cs";
    
    public TblMenu GetById(int? id) => 
        DbSeederExtension.DbSeed_Tbl_Menu.FirstOrDefault(c => c.MenuId.Equals(id)) ?? new TblMenu();

    public TblMenu GetByName(string categoryName) => 
        DbSeederExtension.DbSeed_Tbl_Menu.FirstOrDefault(c => c.MenuUrltext.Equals(categoryName)) ?? new TblMenu { Id = -1, MenuId = -1, MenuMainId = -1, MenuText = "Hittas inte", MenuUrltext = "NotFound" };
    
    public List<TblMenu> GetAll() => 
        DbSeederExtension.DbSeed_Tbl_Menu ?? new List<TblMenu>();
    
    public List<TblMenu> GetSubsList(int categoryID) => 
        DbSeederExtension.DbSeed_Tbl_Menu.Where(x => x.MenuMainId.Equals(categoryID)).ToList() ?? new List<TblMenu>();
}
