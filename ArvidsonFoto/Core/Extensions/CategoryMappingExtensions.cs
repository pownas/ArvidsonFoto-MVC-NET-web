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
    public static CategoryDto ToCategoryDto(this TblMenu menu, string categoryPath, string lastImageFilename, int? imageCounted = null, string baseUrl = "", string categoryPathEn = "", string categoryPathFullSv = "")
    {
        if (menu == null)
        {
            return new CategoryDto();
        }

        var categoryUrl = string.IsNullOrEmpty(baseUrl)
            ? $"bilder/{categoryPath}"
            : $"{baseUrl}/bilder/{categoryPath}";

        return new CategoryDto
        {
            CategoryId = menu.MenuCategoryId,
            NameSv = menu.MenuDisplayName ?? string.Empty,
            NameEn = menu.MenuDisplayNameEn,
            UrlImage = string.IsNullOrEmpty(lastImageFilename) ? string.Empty : $"{lastImageFilename}",
            UrlCategorySv = categoryUrl,
            UrlCategoryEn = string.IsNullOrEmpty(categoryPathEn) ? string.Empty : $"images/{categoryPathEn}",
            UrlCategoryPathSv = UrlSlug.ToUrlSegment(menu.MenuUrlSegment),
            UrlCategoryPathEn = string.IsNullOrWhiteSpace(menu.MenuUrlSegmentEn) ? null : UrlSlug.ToUrlSegment(menu.MenuUrlSegmentEn),
            UrlCategoryPathFullSv = ToSlugPath(string.IsNullOrEmpty(categoryPathFullSv) ? categoryPath ?? menu.MenuUrlSegment ?? string.Empty : categoryPathFullSv),
            UrlCategoryPathFullEn = ToSlugPath(categoryPathEn),
            DateUpdated = DateTime.UtcNow, // TblMenu doesn't have a date field, using current time
            //Description = string.Empty, // TblMenu doesn't have a description field
            ParentCategoryId = menu.MenuParentCategoryId,
            IsGameSelectable = menu.IsGameSelectable,
            ImageCount = imageCounted ?? -1,
        };
    }

    /// <summary>
    /// Converts a CategoryDto back to TblMenu for database operations
    /// </summary>
    /// <param name="categoryDto">The CategoryDto to convert</param>
    /// <returns>A TblMenu representation of the CategoryDto</returns>
    public static TblMenu ToTblMenu(this CategoryDto categoryDto)
    {
        return categoryDto == null
            ? new TblMenu()
            : new TblMenu
        {
            MenuCategoryId = categoryDto.CategoryId,
            MenuDisplayName = categoryDto.NameSv,
            MenuDisplayNameEn = categoryDto.NameEn,
            MenuUrlSegment = UrlSlug.ToUrlSegment(categoryDto.UrlCategoryPathSv),
            MenuUrlSegmentEn = string.IsNullOrWhiteSpace(categoryDto.UrlCategoryPathEn) ? null : UrlSlug.ToUrlSegment(categoryDto.UrlCategoryPathEn),
            MenuParentCategoryId = categoryDto.ParentCategoryId,
            MenuDateUpdated = categoryDto.DateUpdated,
            IsGameSelectable = categoryDto.IsGameSelectable
        };
    }

    private static string ToSlugPath(string? path)
    {
        return string.Join("/", (path ?? string.Empty)
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Select(UrlSlug.ToUrlSegment));
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
            NameSv = "Not found",
            UrlCategorySv = "bilder/404-NotFound",
            UrlCategoryPathSv = "404-NotFound",
            UrlCategoryPathFullSv = "404-NotFound",
            DateUpdated = DateTime.UtcNow,
            //Description = "Category not found"
        };
    }
}