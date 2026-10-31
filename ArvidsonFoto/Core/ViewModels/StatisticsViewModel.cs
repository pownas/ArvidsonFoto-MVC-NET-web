using ArvidsonFoto.Core.Models;

namespace ArvidsonFoto.Core.ViewModels;

public class StatisticsViewModel
{
    public string Period { get; set; } = "6m";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool CompareWithPreviousYear { get; set; }
    public Dictionary<string, int> MonthlyPageViews { get; set; } = [];
    public Dictionary<string, int> MonthlyCategoryViews { get; set; } = [];
    public Dictionary<string, int> PreviousYearPageViews { get; set; } = [];
    public Dictionary<string, int> PreviousYearCategoryViews { get; set; } = [];
    public List<TblPageCounter> PageViews { get; set; } = [];
    public List<TblPageCounter> CategoryViews { get; set; } = [];
    public List<TblPageCounter> AllTimePageViews { get; set; } = [];
}
