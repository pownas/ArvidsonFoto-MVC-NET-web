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
            return false;
        }

        public int GetCategoryLastId()
        {
            int highestID = -1;
            highestID = _entityContext.TblMenus.LastOrDefault().MenuId;
            //using (var conn = new SqlConnection(_sqlConn.Config))
            //{
            //    var sql = @"SELECT MAX(menu_ID) FROM tbl_menu";
            //    highestID = await conn.QuerySingleAsync<int>(sql);
            //}
            return highestID;
        }


        public List<TblMenu> GetAllCategoriesList()
        {
            List<TblMenu> categories;
            categories = _entityContext.Set<TblMenu>().ToList();
            //using (var conn = new SqlConnection(_sqlConn.Config))
            //{
            //    categories = await conn.QueryAsync<TblMenu>(@"SELECT menu_ID, menu_mainID, menu_text, menu_URLtext FROM tbl_menu ORDER BY menu_mainID, menu_text");
            //}
            return categories;
        }

        public IEnumerable<TblMenu> GetCategorySubsList(int categoryID)
        {
            IEnumerable<TblMenu> categories;
            categories = _entityContext.Set<TblMenu>().Where(c => c.MenuMainId == categoryID).ToList();
            //using (var conn = new SqlConnection(_sqlConn.Config))
            //{
            //    categories = await conn.QueryAsync<TblMenu>(@"SELECT menu_ID, menu_mainID, menu_text, menu_URLtext FROM tbl_menu WHERE menu_mainID = " + categoryID + " ORDER BY menu_mainID, menu_text");
            //}
            return categories;
        }

        public int GetCategoryIdByName(string categoryName)
        {
            int menuID = -1;
            //using (var conn = new SqlConnection(_sqlConn.Config))
            //{
            //    var sql = @"SELECT menu_ID FROM tbl_menu WHERE menu_text = '" + categoryName + "'";
            //    menuID = await conn.QuerySingleAsync<int>(sql);
            //}
            return menuID;
        }
    }
}
