using System;
using ArvidsonFoto.Models;
using System.Collections.Generic;

namespace ArvidsonFoto.Services
{
    public interface ICategoryService
    {
        bool AddCategory(TblMenu category);
        
        int GetLastId();
        
        int GetIdByName(string categoryName);

        string GetNameById(int? id);

        public TblMenu GetById(int? id);

        TblMenu GetByName(string categoryName);

        List<TblMenu> GetAll();
        
        List<TblMenu> GetSubsList(int categoryID);
    }
}