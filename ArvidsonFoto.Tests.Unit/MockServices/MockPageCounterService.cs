using ArvidsonFoto.Data;
using ArvidsonFoto.Models;
using ArvidsonFoto.Services;

namespace ArvidsonFoto.Tests.Unit.MockServices;

public class MockPageCounterService : IPageCounterService
{
    public void AddPageCount(string pageName) { }
    public void AddCategoryCount(int categoryId, string pageName) { }

    public List<TblPageCounter> GetMonthCount(string yearMonth, bool picturePage) => 
        DbSeederExtension.DbSeed_Tbl_PageCounter ?? new List<TblPageCounter>();
    
    public List<TblPageCounter> GetAllPageCountsGroupedByPageCount() =>
        DbSeederExtension.DbSeed_Tbl_PageCounter ?? new List<TblPageCounter>();
    
    public List<TblPageCounter> GetTop20CategoryCountsGroupedByPageCount() =>
        DbSeederExtension.DbSeed_Tbl_PageCounter ?? new List<TblPageCounter>();

    public Dictionary<string, int> GetMonthlyPageViewsChart(int monthsBack, bool picturePage) =>
        new Dictionary<string, int>();
}
