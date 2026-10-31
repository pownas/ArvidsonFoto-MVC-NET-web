using System.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;

namespace ArvidsonFoto.Core.Diagnostics;

internal sealed class ImageTraceContext(bool enabled)
{
    internal const string QueryParameter = "af_traceparent";

    internal bool Enabled { get; } = enabled;

    internal static bool IsImagePath(string? path) =>
        Path.GetExtension(path)?.ToLowerInvariant() is
            ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp" or ".avif" or ".svg" or ".ico";

    internal bool TryGetParent(HttpRequest request, out ActivityContext parent)
    {
        parent = default;
        return Enabled && HttpMethods.IsGet(request.Method) && IsImagePath(request.Path.Value)
            && request.Query[QueryParameter].Count == 1
            && ActivityContext.TryParse(request.Query[QueryParameter][0], null, isRemote: true, out parent);
    }

    internal string AddToLocalUrl(HttpContext context, string url)
    {
        var requestActivity = context.Features.Get<IHttpActivityFeature>()?.Activity ?? Activity.Current;
        if (!Enabled || requestActivity?.IdFormat != ActivityIdFormat.W3C || !requestActivity.Recorded
            || string.IsNullOrWhiteSpace(url))
        {
            return url;
        }

        // The layout's base element resolves relative image URLs from the application root.
        var origin = new Uri($"{context.Request.Scheme}://{context.Request.Host}{context.Request.PathBase}/");
        var resolvedUrl = new Uri(origin, url.StartsWith("~/", StringComparison.Ordinal) ? url[2..] : url);
        if (resolvedUrl.Scheme != origin.Scheme || resolvedUrl.Authority != origin.Authority
            || !IsImagePath(resolvedUrl.AbsolutePath)
            || QueryHelpers.ParseQuery(resolvedUrl.Query).ContainsKey(QueryParameter))
        {
            return url;
        }

        return QueryHelpers.AddQueryString(url, QueryParameter, requestActivity.Id!);
    }
}