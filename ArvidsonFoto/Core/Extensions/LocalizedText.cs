using System.Globalization;

namespace ArvidsonFoto.Core.Extensions;

public static class LocalizedText
{
    public static bool IsEnglish => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "en";

    public static string Select(string? swedish, string? english) =>
        IsEnglish && !string.IsNullOrWhiteSpace(english) ? english : swedish ?? string.Empty;
}
