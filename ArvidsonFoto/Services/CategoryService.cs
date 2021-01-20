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

        public bool SetCategoryInsert(TblMenu category)
        {
            //using (var conn = new SqlConnection(_entityContext.Database))
            //{
            //    var parameters = new DynamicParameters();
            //    parameters.Add("menu_ID", category.menu_ID, DbType.Int32);
            //    parameters.Add("menu_mainID", category.menu_mainID, DbType.Int32);
            //    parameters.Add("menu_text", category.menu_text, DbType.String);
            //    parameters.Add("menu_URLtext", category.menu_URLtext, DbType.String);
            //    parameters.Add("menu_pagecounter", category.menu_pagecounter, DbType.Int32);

            //    // Raw SQL method
            //    const string query = @"INSERT INTO tbl_menu (menu_ID, menu_mainID, menu_text, menu_URLtext, menu_pagecounter) VALUES (@menu_ID, @menu_mainID, @menu_text, @menu_URLtext, @menu_pagecounter)";
            //    await conn.ExecuteAsync(query, new { category.menu_ID, category.menu_mainID, category.menu_text, category.menu_URLtext, category.menu_pagecounter }, commandType: CommandType.Text);
            //}
            //return true;
            throw new NotImplementedException();
        }

        public int GetCategoryLastId()
        {
            int highestID = -1;
            highestID = _entityContext.TblMenus.LastOrDefault().MenuId;
            return highestID;
        }


        public List<TblMenu> GetAllCategoriesList()
        {
            List<TblMenu> categories;
            categories = _entityContext.TblMenus.ToList();
            return categories;
        }

        public List<TblMenu> GetCategorySubsList(int categoryID)
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

        public int GetCategoryIdByName(string categoryName)
        {
            int menuID = -1;
            //using (var conn = new SqlConnection(_sqlConn.Config))
            //{
            //    var sql = @"SELECT menu_ID FROM tbl_menu WHERE menu_text = '" + categoryName + "'";
            //    menuID = await conn.QuerySingleAsync<int>(sql);
            //}
            throw new NotImplementedException();
            return menuID;
        }
    }
}
