using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArvidsonFoto.Models;

namespace ArvidsonFoto.Data
{
    public interface ICategoryService
    {
        bool SetCategoryInsert(TblMenu category);
        
        int GetCategoryLastId();
        
        int GetCategoryIdByName(string categoryName);
        
        List<TblMenu> GetAllCategoriesList();
        
        IEnumerable<TblMenu> GetCategorySubsList(int categoryID);
    }
}