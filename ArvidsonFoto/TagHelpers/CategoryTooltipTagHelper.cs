using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ArvidsonFoto.TagHelpers;

/// <summary>
/// Tag helper for adding category image tooltips to links.
/// Usage: <a category-tooltip="1" href="/Bilder/Faglar">Fåglar</a>
/// </summary>
[HtmlTargetElement("a", Attributes = CategoryIdAttributeName)]
public class CategoryTooltipTagHelper : TagHelper
{
    private const string CategoryIdAttributeName = "category-tooltip";
    
    /// <summary>
    /// The category ID to fetch image for
    /// </summary>
    [HtmlAttributeName(CategoryIdAttributeName)]
    public int CategoryId { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        // Add the class for JavaScript to identify tooltip-enabled links
        var existingClass = output.Attributes["class"]?.Value?.ToString() ?? "";
        var newClass = string.IsNullOrEmpty(existingClass) 
            ? "has-category-tooltip" 
            : $"{existingClass} has-category-tooltip";
        
        output.Attributes.SetAttribute("class", newClass);
        
        // Add data attribute with category ID
        output.Attributes.SetAttribute("data-category-id", CategoryId.ToString());
        
        // Remove the custom attribute so it doesn't appear in final HTML
        output.Attributes.RemoveAll(CategoryIdAttributeName);
    }
}
