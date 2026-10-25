using ArvidsonFoto.Core.DTOs;
using System.Text;

namespace ArvidsonFoto.Core.Extensions;

public static class LocalizedRoutes
{
    public static bool IsEnglishPath(string? path)
    {
        return path is not null && (path.Equals("/search", StringComparison.OrdinalIgnoreCase) ||
            new[] { "/images", "/api/images", "/latest", "/information" }
                .Any(prefix => path.Equals(prefix, StringComparison.OrdinalIgnoreCase) ||
                    path.StartsWith(prefix + "/", StringComparison.OrdinalIgnoreCase)));
    }

    public static bool IsSwedishApiPath(string? path)
    {
        return path is not null && (path.Equals("/api/bilder", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/api/bilder/", StringComparison.OrdinalIgnoreCase));
    }

    public static string Gallery => LocalizedText.IsEnglish ? "/images" : "/bilder";
    public static string Search => LocalizedText.IsEnglish ? "/search" : "/sok";

    public static string NormalizePathCasing(string path)
    {
        var normalized = new StringBuilder(path.Length);
        for (var index = 0; index < path.Length; index++)
        {
            if (path[index] == '%' && index + 2 < path.Length &&
                Uri.IsHexDigit(path[index + 1]) && Uri.IsHexDigit(path[index + 2]))
            {
                normalized.Append('%');
                normalized.Append(char.ToUpperInvariant(path[index + 1]));
                normalized.Append(char.ToUpperInvariant(path[index + 2]));
                index += 2;
            }
            else
            {
                normalized.Append(char.ToLowerInvariant(path[index]));
            }
        }

        return normalized.ToString();
    }

    private static readonly (string Swedish, string English)[] Pages =
    [
        ("/senast/per kategori", "/latest/by-category"),
        ("/senast/fotograferad", "/latest/photographed"),
        ("/senast/uppladdad", "/latest/uploaded"),
        ("/senast", "/latest"),
        ("/info/kop_av_bilder", "/information/buy-photos"),
        ("/info/gastbok", "/information/guestbook"),
        ("/info/kontakta", "/information/contact"),
        ("/info/om_mig", "/information/about"),
        ("/info/sidkarta", "/information/sitemap"),
        ("/info/copyright", "/information/copyright"),
        ("/info", "/information")
    ];

    public static string Page(string swedish)
    {
        var (Swedish, English) = Pages.FirstOrDefault(p => p.Swedish.Equals(swedish, StringComparison.OrdinalIgnoreCase));
        return NormalizePathCasing(LocalizedText.IsEnglish && English is not null ? English : swedish);
    }

    public static string Category(params CategoryDto[] categories)
    {
        return NormalizePathCasing(Gallery + "/" + string.Join("/", categories.Select(CategorySegment)));
    }

    public static string CategorySegment(CategoryDto category)
    {
        var segment = LocalizedText.IsEnglish
            ? string.IsNullOrWhiteSpace(category.UrlCategoryPathEn) ? category.UrlCategoryPathSv ?? category.NameSv : category.UrlCategoryPathEn
            : category.UrlCategoryPathSv ?? category.NameSv;
        return Uri.EscapeDataString(UrlSlug.ToUrlSegment(segment));
    }

    public static CategoryDto? ResolveCategoryPath(IEnumerable<string?> pathSegments, IReadOnlyCollection<CategoryDto> categories)
    {
        CategoryDto? resolvedCategory = null;
        int? parentCategoryId = null;

        foreach (var pathSegment in pathSegments.Where(segment => !string.IsNullOrWhiteSpace(segment)))
        {
            var decodedSegment = Uri.UnescapeDataString(pathSegment!);
            if (resolvedCategory is null &&
                (decodedSegment.Equals("bilder", StringComparison.OrdinalIgnoreCase) ||
                 decodedSegment.Equals("images", StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            var candidates = categories.Where(category => parentCategoryId is null
                ? category.ParentCategoryId is null or 0
                : category.ParentCategoryId == parentCategoryId);

            resolvedCategory = candidates.FirstOrDefault(category => CategoryMatchesSegment(category, decodedSegment, useSlugNormalization: false))
                ?? candidates.FirstOrDefault(category => CategoryMatchesSegment(category, decodedSegment, useSlugNormalization: true));

            if (resolvedCategory?.CategoryId is not int categoryId)
            {
                return null;
            }

            parentCategoryId = categoryId;
        }

        return resolvedCategory;
    }

    public static string CategoryForId(int id, IReadOnlyCollection<CategoryDto> categories)
    {
        var path = new List<CategoryDto>();
        var visited = new HashSet<int>();
        while (id > 0 && visited.Add(id))
        {
            var category = categories.FirstOrDefault(c => c.CategoryId == id);
            if (category is null)
            {
                break;
            }

            path.Add(category);
            id = category.ParentCategoryId ?? 0;
        }
        path.Reverse();
        return path.Count == 0 ? Gallery : Category([.. path]);
    }

    public static string Switch(string path, bool english, IReadOnlyCollection<CategoryDto> categories)
    {
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0)
        {
            return "/";
        }

        if (segments[0].Equals("bilder", StringComparison.OrdinalIgnoreCase) ||
            segments[0].Equals("images", StringComparison.OrdinalIgnoreCase))
        {
            var result = new List<string> { english ? "images" : "bilder" };
            int? parent = null;
            foreach (var segment in segments.Skip(1))
            {
                var decoded = Uri.UnescapeDataString(segment);
                var candidates = categories.Where(c =>
                    (parent is null ? c.ParentCategoryId is null or 0 : c.ParentCategoryId == parent));
                var category = candidates.FirstOrDefault(c => CategoryMatchesSegment(c, decoded, useSlugNormalization: false))
                    ?? candidates.FirstOrDefault(c => CategoryMatchesSegment(c, decoded, useSlugNormalization: true));
                if (category is null)
                {
                    result.Add(Uri.EscapeDataString(UrlSlug.ToUrlSegment(decoded)));
                    parent = null;
                }
                else
                {
                    var categorySegment = english
                        ? (string.IsNullOrWhiteSpace(category.UrlCategoryPathEn) ? category.UrlCategoryPathSv ?? decoded : category.UrlCategoryPathEn)
                        : category.UrlCategoryPathSv ?? decoded;
                    result.Add(Uri.EscapeDataString(UrlSlug.ToUrlSegment(categorySegment)));
                    parent = category.CategoryId;
                }
            }
            return NormalizePathCasing("/" + string.Join("/", result));
        }

        var unescaped = Uri.UnescapeDataString(path).TrimEnd('/');
        var (Swedish, English) = Pages.FirstOrDefault(p => string.Equals(p.Swedish, unescaped, StringComparison.OrdinalIgnoreCase) ||
                                              string.Equals(p.English, unescaped, StringComparison.OrdinalIgnoreCase));
        return NormalizePathCasing(Swedish is not null ? english ? English : Swedish : path);
    }

    private static bool CategoryMatchesSegment(CategoryDto category, string segment, bool useSlugNormalization)
    {
        var values = new[] { category.UrlCategoryPathSv, category.UrlCategoryPathEn, category.NameSv, category.NameEn };
        return values.Any(value => !string.IsNullOrWhiteSpace(value) &&
            (useSlugNormalization
                ? UrlSlug.ToUrlSegment(value).Equals(UrlSlug.ToUrlSegment(segment), StringComparison.OrdinalIgnoreCase)
                : value.Equals(segment, StringComparison.OrdinalIgnoreCase)));
    }
}
