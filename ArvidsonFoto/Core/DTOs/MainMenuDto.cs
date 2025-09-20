namespace ArvidsonFoto.Core.DTOs;

/// <summary>
/// Represents a data transfer object for a main menu category, including its unique URL and display title.
/// </summary>
/// <remarks>This class is typically used to encapsulate information about a menu category for display purposes or
/// for navigation within an application.</remarks>
public class MainMenuDto()
{
    /// <summary> Meny-kategorins unika länk-id </summary>
    public string MenuUrl { get; set; } = "/404-NotFound";

    /// <summary> Meny-kategorins namn </summary>
    public string MenuDisplayName { get; set; } = "404 - Not Found";

    ///// <summary> Meny-kategorins unika id </summary>
    //public int MenuId { get; set; } = -1;

    ///// <summary> Meny-kategorins förälders id </summary>
    //public int? ParentId { get; set; } = -1;

    /// <summary> Om det finns underkategorier </summary>
    public bool HasSubCategories
    {
        get => SubCategoryCount > 0; // Return true if there are subcategories, otherwise false.
    }

    ///// <summary> Lista med underkategoriernas id:n </summary>
    //public IList<int> SubCategories { get; set; } = new List<int>();

    /// <summary> Antal underkategorier </summary>
    public int SubCategoryCount { get; set; } = 0;

    /// <summary> Sorting URL för kategorin, används endast för att sortera länkarna korrekt, så att åäö hamnar rätt i sorteringen </summary>
    public string SortingUrlWithAao { get; set; } = "/Ej återfunnen/";
}
