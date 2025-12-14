#nullable enable
using ArvidsonFoto.Core.DTOs;

namespace ArvidsonFoto.Core.ComponentState;

/// <summary>
/// Result class for category navigation operations from breadcrumb component
/// </summary>
public class CategoryNavigationResult()
{
    /// <summary>
    /// Indicates whether the category navigation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if navigation failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// The selected category that was navigated to
    /// </summary>
    public CategoryDto? SelectedCategory { get; set; }

    /// <summary>
    /// The parent category of the selected category
    /// </summary>
    public CategoryDto? ParentCategory { get; set; }

    /// <summary>
    /// List of categories for breadcrumb navigation
    /// </summary>
    public List<CategoryDto>? BreadcrumbCategories { get; set; }

    /// <summary>
    /// The resolved category ID
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// The suggested page title based on the navigation
    /// </summary>
    public string? PageTitle { get; set; }
}
