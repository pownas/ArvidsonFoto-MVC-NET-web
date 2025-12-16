using Microsoft.AspNetCore.Mvc;

namespace ArvidsonFoto.ViewComponents;

/// <summary>
/// View component for rendering category links with image tooltips.
/// Usage: @await Component.InvokeAsync("CategoryTooltip", new { categoryId = 1, categoryName = "Fåglar", url = "/Bilder/Faglar" })
/// </summary>
public class CategoryTooltipViewComponent : ViewComponent
{
    /// <summary>
    /// Renders a link with category tooltip functionality
    /// </summary>
    /// <param name="categoryId">The category ID for fetching images</param>
    /// <param name="categoryName">Display name of the category</param>
    /// <param name="url">URL the link should navigate to</param>
    /// <param name="cssClass">Additional CSS classes (optional)</param>
    /// <returns>View result with the link element</returns>
    public IViewComponentResult Invoke(int categoryId, string categoryName, string url, string? cssClass = null)
    {
        var model = new CategoryTooltipViewModel
        {
            CategoryId = categoryId,
            CategoryName = categoryName,
            Url = url,
            CssClass = cssClass
        };
        
        return View(model);
    }
}

/// <summary>
/// View model for the CategoryTooltip view component
/// </summary>
public class CategoryTooltipViewModel
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? CssClass { get; set; }
}
