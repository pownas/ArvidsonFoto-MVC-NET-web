using ArvidsonFoto.Core.DTOs;
using ArvidsonFoto.Core.Models;

namespace ArvidsonFoto.Core.Extensions;

/// <summary>
/// Extension methods for mapping between TblMenu and CategoryDto
/// </summary>
public static class CategoryMappingExtensions
{
    /// <summary>
    /// Converts a TblMenu to a CategoryDto
    /// </summary>
    /// <param name="menu">The TblMenu to convert</param>
    /// <param name="categoryPath">The full path for the category, used to generate the UrlCategoryPathFull</param>
    /// <param name="lastImageFilename">Filename of the last image in the category, used to generate the UrlImage</param>
    /// <param name="baseUrl">Optional base URL for URL generation</param>
    /// <returns>A CategoryDto representation of the TblMenu</returns>
    public static CategoryDto ToCategoryDto(this TblMenu menu, string categoryPath, string lastImageFilename, string baseUrl = "")
    {
        if (menu == null)
            return new CategoryDto();

        var categoryUrl = string.IsNullOrEmpty(baseUrl) 
            ? $"bilder/{categoryPath}" 
            : $"{baseUrl}/bilder/{categoryPath}";

        return new CategoryDto
        {
            CategoryId = menu.MenuCategoryId,
            Name = menu.MenuDisplayName ?? string.Empty,
            UrlImage = string.IsNullOrEmpty(lastImageFilename) ? string.Empty : $"{lastImageFilename}",
            UrlCategory = categoryUrl,
            UrlCategoryPath = menu.MenuUrlSegment ?? string.Empty,
            UrlCategoryPathFull = categoryPath ?? menu.MenuUrlSegment ?? string.Empty, // This might need more complex logic for full paths
            DateUpdated = DateTime.UtcNow, // TblMenu doesn't have a date field, using current time
            Description = string.Empty, // TblMenu doesn't have a description field
            ParentCategoryId = menu.MenuParentCategoryId
        };
    }

    /// <summary>
    /// Converts a CategoryDto back to TblMenu for database operations
    /// </summary>
    /// <param name="categoryDto">The CategoryDto to convert</param>
    /// <returns>A TblMenu representation of the CategoryDto</returns>
    public static TblMenu ToTblMenu(this CategoryDto categoryDto)
    {
        if (categoryDto == null)
            return new TblMenu();

        return new TblMenu
        {
            MenuCategoryId = categoryDto.CategoryId,
            MenuDisplayName = categoryDto.Name,
            MenuUrlSegment = categoryDto.UrlCategoryPath,
            MenuParentCategoryId = categoryDto.ParentCategoryId
        };
    }

    /// <summary>
    /// Creates a default "not found" CategoryDto
    /// </summary>
    /// <returns>A CategoryDto representing a not found category</returns>
    public static CategoryDto CreateNotFoundCategoryDto(this CategoryDto categoryDto)
    {
        return new CategoryDto
        {
            CategoryId = -1,
            Name = "Not found",
            UrlCategory = "bilder/404-NotFound",
            UrlCategoryPath = "404-NotFound",
            UrlCategoryPathFull = "404-NotFound",
            DateUpdated = DateTime.UtcNow,
            Description = "Category not found"
        };
    }
}
