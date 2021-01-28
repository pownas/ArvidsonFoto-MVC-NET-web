using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArvidsonFoto.Models;
using ArvidsonFoto.Data;
using Microsoft.Data.SqlClient;

namespace ArvidsonFoto.Data
{
    public class CategoryService : ICategoryService
    {
        // Databas koppling
        private readonly ArvidsonFotoDbContext _entityContext;
        public CategoryService(ArvidsonFotoDbContext context)
        {
            _entityContext = context;
        }

        public bool AddCategory(TblMenu category)
        {
            bool success = false;
            try {
                _entityContext.TblMenus.Add(category);
                _entityContext.SaveChanges();
                success = true;
            }
            catch(Exception ex)
            {
                success = false;
                throw new Exception("Fel vid skapande av kategori. Felmeddelande: " + ex.Message);
            }
            return success;
        }

        public int GetLastId()
        {
            int highestID = -1;
            highestID = _entityContext.TblMenus.OrderBy(c => c.MenuId).LastOrDefault().MenuId;
            return highestID;
        }

        public TblMenu GetByName(string categoryName)
        {
            TblMenu category = new TblMenu();
            category = _entityContext.TblMenus.FirstOrDefault(c => c.MenuText.Equals(categoryName));
            return category;
        }

        public TblMenu GetById(int? id)
        {
            TblMenu category = _entityContext.TblMenus.FirstOrDefault(c => c.MenuId.Equals(id));
            return category;
        }

        public List<TblMenu> GetAll()
        {
            List<TblMenu> categories;
            categories = _entityContext.TblMenus.ToList();
            return categories;
        }

        public List<TblMenu> GetSubsList(int categoryID)
        {
            List<TblMenu> categories;
            categories = _entityContext.TblMenus.Where(c => c.MenuMainId.Equals(categoryID)).ToList();
            return categories;
        }

        public string GetNameById(int? id)
        {
            string categoryName = "";
            TblMenu category = _entityContext.TblMenus.FirstOrDefault(c => c.MenuId.Equals(id));
            if (category is not null)
                categoryName = category.MenuText;

            return categoryName;
        }

        public int GetIdByName(string categoryName)
        {
            int menuID = -1;
            TblMenu category = _entityContext.TblMenus.FirstOrDefault(c => c.MenuText.Equals(categoryName));
            if (category is not null)
                menuID = category.MenuId;

            return menuID;
        }
    }
}
