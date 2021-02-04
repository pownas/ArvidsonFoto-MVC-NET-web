using System.Collections.Generic;
using ArvidsonFoto.Models;

namespace ArvidsonFoto.Services
{
    public interface IPageCounterService
    {
        void AddPageCount(string pageName);

        List<TblPageCounter> GetMonthCount(string yearMonth);

        List<TblPageCounter> GetAllGroupedByPageCount();
    }
}