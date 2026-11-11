namespace ArvidsonFoto.Core.Extensions;

public static class CategoryIconHelper
{
    public static string GetIconClass(bool hasSubcategories, bool isRoot = false) =>
        hasSubcategories
            ? isRoot ? "bi-folder2-open" : "bi-folder"
            : "bi-tag";
}
