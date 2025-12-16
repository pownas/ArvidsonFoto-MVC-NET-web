using AngleSharp;
using AngleSharp.Html.Dom;

namespace ArvidsonFoto.Tests.Integration.Helpers;

/// <summary>
/// Helper class for parsing and interacting with HTML responses in integration tests.
/// </summary>
public static class HtmlHelpers
{
    /// <summary>
    /// Parses an HTTP response message into an HTML document.
    /// </summary>
    public static async Task<IHtmlDocument> GetDocumentAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var context = BrowsingContext.New(Configuration.Default);
        var document = await context.OpenAsync(req => req.Content(content));
        return (IHtmlDocument)document;
    }

    /// <summary>
    /// Extracts the anti-forgery token from a form.
    /// </summary>
    public static string? GetAntiForgeryToken(IHtmlDocument document)
    {
        var tokenInput = document.QuerySelector("input[name='__RequestVerificationToken']");
        return tokenInput?.GetAttribute("value");
    }

    /// <summary>
    /// Creates form data from a dictionary including anti-forgery token.
    /// </summary>
    public static Dictionary<string, string> CreateFormData(
        IHtmlDocument document,
        Dictionary<string, string> formFields)
    {
        var token = GetAntiForgeryToken(document);
        if (token != null)
        {
            formFields["__RequestVerificationToken"] = token;
        }

        return formFields;
    }

    /// <summary>
    /// Gets the value of a specific element by its ID.
    /// </summary>
    public static string? GetElementValue(IHtmlDocument document, string elementId)
    {
        var element = document.QuerySelector($"#{elementId}") as IHtmlInputElement;
        return element?.Value;
    }

    /// <summary>
    /// Checks if an element with specific text exists.
    /// </summary>
    public static bool ContainsText(IHtmlDocument document, string text)
    {
        return document.Body?.TextContent.Contains(text, StringComparison.OrdinalIgnoreCase) ?? false;
    }

    /// <summary>
    /// Gets all alert messages from the page.
    /// </summary>
    public static List<string> GetAlertMessages(IHtmlDocument document)
    {
        var alerts = document.QuerySelectorAll(".alert");
        return alerts
            .Select(a => a.TextContent.Trim())
            .Where(text => !string.IsNullOrWhiteSpace(text))
            .ToList();
    }

    /// <summary>
    /// Checks if a success alert is displayed.
    /// </summary>
    public static bool HasSuccessAlert(IHtmlDocument document)
    {
        return document.QuerySelector(".alert-success") != null;
    }

    /// <summary>
    /// Checks if an error alert is displayed.
    /// </summary>
    public static bool HasErrorAlert(IHtmlDocument document)
    {
        return document.QuerySelector(".alert-danger") != null;
    }
}
