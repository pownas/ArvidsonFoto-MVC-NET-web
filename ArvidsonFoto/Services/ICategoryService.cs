using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArvidsonFoto.Models;

namespace ArvidsonFoto.Data
{
    public interface ICategoryService
    {
        bool SetCategoryInsert(TblMenu category);
        
        int GetLastId();
        
        int GetIdByName(string categoryName);

        string GetNameById(int? id);

        TblMenu GetByName(string categoryName);

        List<TblMenu> GetAll();
        
        List<TblMenu> GetSubsList(int categoryID);
    }
}