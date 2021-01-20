using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArvidsonFoto.Models;

namespace ArvidsonFoto.Data
{
    public interface ICategoryService
    {
        bool SetCategoryInsert(TblMenu category);
        
        int GetCategoryLastId();
        
        int GetCategoryIdByName(string categoryName);

        string GetNameById(int? id);

        List<TblMenu> GetAllCategoriesList();
        
        List<TblMenu> GetCategorySubsList(int categoryID);
    }
}