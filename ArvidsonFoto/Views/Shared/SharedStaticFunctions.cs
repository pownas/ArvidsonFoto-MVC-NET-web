using Microsoft.AspNetCore.Http.Features;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ArvidsonFoto.Views.Shared;

public static class SharedStaticFunctions
{
    public static int GetWeekNumber(DateTime date)
    {
        //Code from: https://docs.microsoft.com/en-us/dotnet/api/system.globalization.calendar.getweekofyear?view=net-5.0

        // Gets the Calendar instance associated with a CultureInfo.
        CultureInfo myCI = new CultureInfo("sv-SE");
        Calendar myCal = myCI.Calendar;

        // Gets the DTFI properties required by GetWeekOfYear.
        CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
        DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;

        // Displays the number of the current week relative to the beginning of the year.
        //Console.WriteLine("The CalendarWeekRule used for the en-US culture is {0}.", myCWR);
        //Console.WriteLine("The FirstDayOfWeek used for the en-US culture is {0}.", myFirstDOW);
        //Console.WriteLine("Therefore, the current week is Week {0} of the current year.", myCal.GetWeekOfYear(DateTime.Now, myCWR, myFirstDOW));

        // Displays the total number of weeks in the current year.
        //DateTime LastDay = new System.DateTime(DateTime.Now.Year, 12, 31);
        //Console.WriteLine("There are {0} weeks in the current year ({1}).", myCal.GetWeekOfYear(LastDay, myCWR, myFirstDOW), DateTime.Now.Year);

        return myCal.GetWeekOfYear(date, myCWR, myFirstDOW);
    }

    /// <summary>
    /// Decodes garbled or URL-encoded Swedish characters back to their proper Unicode form.
    /// </summary>
    /// <remarks>
    /// Handles multiple encoding variants that may appear when Swedish text is incorrectly
    /// decoded, including Mojibake (e.g. "Ã¥"), HTML named references (e.g. "&amp;#xE5;"),
    /// and double-URL-encoded sequences (e.g. "%C3%83%C2%B6").
    /// Supported characters: å, ä, ö, Å, Ä, Ö.
    /// </remarks>
    /// <param name="replaceAAO">The string that may contain garbled Swedish characters.</param>
    /// <returns>The string with all recognised garbled sequences replaced by correct Swedish characters.</returns>
    public static string ReplaceAAO(string replaceAAO)
    {
        replaceAAO = replaceAAO.Replace("Ã¥", "å");
        replaceAAO = replaceAAO.Replace("&#xE5;", "å");
        replaceAAO = replaceAAO.Replace("Ã¤", "ä");
        replaceAAO = replaceAAO.Replace("&#xE4;", "ä");
        replaceAAO = replaceAAO.Replace("Ã¶", "ö");
        replaceAAO = replaceAAO.Replace("%C3%83%C2%B6", "ö");
        replaceAAO = replaceAAO.Replace("&#xF6;", "ö");
        replaceAAO = replaceAAO.Replace("Ã…", "Å");
        replaceAAO = replaceAAO.Replace("Ã„", "Ä");
        replaceAAO = replaceAAO.Replace("Ã–", "Ö");

        return replaceAAO;
    }

    public static string ReplaceUrlText(string? urlText)
    {
        if (urlText == null) return string.Empty;
        urlText = urlText.Replace(" ", "%20");

        return urlText;
    }

    /// <summary>
    /// Converts a display name to a URL-safe segment (slug) by stripping diacritics and
    /// replacing spaces with hyphens.
    /// </summary>
    /// <remarks>
    /// Uses Unicode NFD normalization to decompose accented characters into their base letter
    /// plus a combining diacritical mark, then discards all combining marks. This covers the
    /// full range of Latin accented characters used in Swedish and common loanwords, including:
    /// <list type="bullet">
    ///   <item>Swedish: å/Å→a/A, ä/Ä→a/A, ö/Ö→o/O</item>
    ///   <item>Acute/grave: é/è→e, á/à→a, í/ì→i, ó/ò→o, ú/ù→u</item>
    ///   <item>Circumflex: ê→e, â→a, î→i, ô→o, û→u</item>
    ///   <item>Diaeresis: ë→e, ï→i, ü→u, ÿ→y</item>
    ///   <item>Other: ñ→n, ç→c</item>
    /// </list>
    /// Spaces are replaced with hyphens to match the convention used for <c>menu_URLtext</c>
    /// in the database (e.g. "Turkos blåvinge" → "Turkos-blavinge", "café" → "cafe").
    /// Any remaining characters that are not alphanumeric or hyphens are removed, and consecutive
    /// hyphens are collapsed into a single hyphen.
    /// </remarks>
    /// <param name="displayName">The display name to convert, e.g. a category name.</param>
    /// <returns>A URL-safe segment string, or an empty string if <paramref name="displayName"/> is null or empty.</returns>
    public static string ToUrlSegment(string? displayName)
    {
        if (string.IsNullOrEmpty(displayName)) return string.Empty;

        // NFD decomposes each accented character into base letter + combining diacritical mark(s).
        // Filtering out NonSpacingMark characters then removes all diacritics (å→a, é→e, ü→u, etc.).
        var withoutDiacritics = new string(
            displayName.Normalize(NormalizationForm.FormD)
                       .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                       .ToArray());

        var segment = withoutDiacritics.Replace(" ", "-");

        // Remove any remaining characters that are not alphanumeric or hyphens
        segment = Regex.Replace(segment, @"[^a-zA-Z0-9\-]", string.Empty);

        // Collapse consecutive hyphens and trim leading/trailing hyphens
        segment = Regex.Replace(segment, @"-{2,}", "-").Trim('-');

        return segment;
    }
}

public static class HttpRequestExtensions
{
    public static string? GetRawUrl(this HttpContext httpContext)
    {
        var requestFeature = httpContext.Features.Get<IHttpRequestFeature>();
        return requestFeature?.RawTarget.ToString();
    }
}

////Original-Code above from: https://stackoverflow.com/a/38747631/14036841
//public static class HttpRequestExtensions
//{
//    public static Uri GetRawUrl(this HttpRequest request)
//    {
//        var httpContext = request.HttpContext;
//        var requestFeature = httpContext.Features.Get<IHttpRequestFeature>();
//        return new Uri(requestFeature.RawTarget);
//    }
//}
