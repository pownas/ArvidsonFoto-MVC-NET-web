using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArvidsonFoto.Models;

namespace ArvidsonFoto.Data
{
    public interface ICategoryService
    {
        Task<bool> SetCategoryInsert(Category category);
        Task<int> GetCategoryLastId();
        Task<int> GetCategoryIdByName(string categoryName);
        Task<IEnumerable<Category>> GetAllCategoriesList();
        Task<IEnumerable<Category>> GetCategorySubsList(int categoryID);
    }
}
