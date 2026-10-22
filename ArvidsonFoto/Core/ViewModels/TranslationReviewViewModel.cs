namespace ArvidsonFoto.Core.ViewModels;

public record TranslationReviewItem(int Id, string Field, string Swedish, string? English, bool Reviewed);

public class TranslationReviewViewModel
{
    public string Table { get; set; } = "menu";
    public bool PendingOnly { get; set; } = true;
    public int Page { get; set; } = 1;
    public int TotalPages { get; set; }
    public int PendingCount { get; set; }
    public int TotalCount { get; set; }
    public int AllPendingCount { get; set; }
    public int AllFieldsCount { get; set; }
    public List<TranslationReviewItem> Items { get; set; } = [];
}
