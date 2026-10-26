using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ArvidsonFoto.Core.Extensions;

public static class UrlSlug
{
    public static string ToUrlSegment(string? displayName)
    {
        if (string.IsNullOrEmpty(displayName))
        {
            return string.Empty;
        }

        var withoutDiacritics = new string(
            [.. displayName.Normalize(NormalizationForm.FormD).Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)]);
        var segment = withoutDiacritics.Replace(" ", "-");
        segment = Regex.Replace(segment, @"[^a-zA-Z0-9\-]", string.Empty);
        segment = Regex.Replace(segment, @"-{2,}", "-").Trim('-');

        return segment.ToLowerInvariant();
    }
}
