using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArvidsonFoto.Models;

namespace ArvidsonFoto.Data
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