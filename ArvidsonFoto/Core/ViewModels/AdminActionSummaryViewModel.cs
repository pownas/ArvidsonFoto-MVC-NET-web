namespace ArvidsonFoto.Core.ViewModels;

public record AdminActionItem(string TitleKey, string LinkKey, int Count, string Url);

public class AdminActionSummaryViewModel
{
    public List<AdminActionItem> Items { get; set; } = [];

    public bool DataUnavailable { get; set; }

    public bool StatisticsUnavailable { get; set; }
}
