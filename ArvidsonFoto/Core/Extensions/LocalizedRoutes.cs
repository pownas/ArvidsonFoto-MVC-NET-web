using ArvidsonFoto.Core.DTOs;

namespace ArvidsonFoto.Core.Extensions;

public static class LocalizedRoutes
{
    public static string Gallery => LocalizedText.IsEnglish ? "/images" : "/Bilder";
    public static string Search => LocalizedText.IsEnglish ? "/search" : "/Search";

    private static readonly (string Swedish, string English)[] Pages =
    [
        ("/Senast/Per kategori", "/latest/by-category"),
        ("/Senast/Fotograferad", "/latest/photographed"),
        ("/Senast/Uppladdad", "/latest/uploaded"),
        ("/Senast", "/latest"),
        ("/Info/Kop_av_bilder", "/information/buy-photos"),
        ("/Info/Gastbok", "/information/guestbook"),
        ("/Info/Kontakta", "/information/contact"),
        ("/Info/Om_mig", "/information/about"),
        ("/Info/Sidkarta", "/information/sitemap"),
        ("/Info/Copyright", "/information/copyright"),
        ("/Info", "/information")
    ];

    public static string Page(string swedish)
    {
        var page = Pages.FirstOrDefault(p => p.Swedish.Equals(swedish, StringComparison.OrdinalIgnoreCase));
        return LocalizedText.IsEnglish && page.English is not null ? page.English : swedish;
    }

    public static string Category(params CategoryDto[] categories) =>
        Gallery + "/" + string.Join("/", categories.Select(c => Uri.EscapeDataString(
            LocalizedText.IsEnglish ? c.DisplayUrlCategoryPath : c.UrlCategoryPath ?? c.NameSv ?? string.Empty)));

    public static string CategoryForId(int id, IReadOnlyCollection<CategoryDto> categories)
    {
        var path = new List<CategoryDto>();
        var visited = new HashSet<int>();
        while (id > 0 && visited.Add(id))
        {
            var category = categories.FirstOrDefault(c => c.CategoryId == id);
            if (category is null)
                break;
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
            return "/";

        if (segments[0].Equals("Bilder", StringComparison.OrdinalIgnoreCase) ||
            segments[0].Equals("images", StringComparison.OrdinalIgnoreCase))
        {
            var result = new List<string> { english ? "images" : "Bilder" };
            int? parent = null;
            foreach (var segment in segments.Skip(1))
            {
                var decoded = Uri.UnescapeDataString(segment);
                var category = categories.FirstOrDefault(c =>
                    (parent is null ? c.ParentCategoryId is null or 0 : c.ParentCategoryId == parent) &&
                    (string.Equals(c.UrlCategoryPath, decoded, StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(c.UrlCategoryPathEn, decoded, StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(c.NameEn, decoded, StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(c.NameSv, decoded, StringComparison.OrdinalIgnoreCase)));
                if (category is null)
                {
                    result.Add(segment);
                    parent = null;
                }
                else
                {
                    result.Add(Uri.EscapeDataString(english
                        ? (string.IsNullOrWhiteSpace(category.UrlCategoryPathEn) ? category.UrlCategoryPath ?? decoded : category.UrlCategoryPathEn)
                        : category.UrlCategoryPath ?? decoded));
                    parent = category.CategoryId;
                }
            }
            return "/" + string.Join("/", result);
        }

        var unescaped = Uri.UnescapeDataString(path).TrimEnd('/');
        var match = Pages.FirstOrDefault(p => string.Equals(p.Swedish, unescaped, StringComparison.OrdinalIgnoreCase) ||
                                              string.Equals(p.English, unescaped, StringComparison.OrdinalIgnoreCase));
        return match.Swedish is not null ? english ? match.English : match.Swedish : path;
    }
}
