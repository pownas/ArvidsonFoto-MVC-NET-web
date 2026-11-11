using System.Diagnostics;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ArvidsonFoto.Core.Diagnostics;

internal static partial class AppTelemetry
{
    internal const string ActivitySourceName = "ArvidsonFoto";

    internal static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    [LoggerMessage(Level = LogLevel.Debug, Message = "MVC {ResultName} returned {StatusCode} in {ElapsedMs} ms")]
    internal static partial void LogMvcResult(ILogger logger, string resultName, int statusCode, double elapsedMs);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Ignoring invalid image trace context for {ImagePath}")]
    internal static partial void LogInvalidImageContext(ILogger logger, string imagePath);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Image {ImagePath} returned {StatusCode}, {ContentLength} bytes, {ContentType}")]
    internal static partial void LogImageResponse(ILogger logger, string imagePath, int statusCode, long? contentLength, string? contentType);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Categories loaded: cache hit {CacheHit}, {Count} categories")]
    internal static partial void LogCategories(ILogger logger, bool cacheHit, int count);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Loaded {Count} images for category {CategoryId}, page {Page}, page size {PageSize}")]
    internal static partial void LogImagePage(ILogger logger, int count, int categoryId, int page, int pageSize);

    [LoggerMessage(Level = LogLevel.Error, Message = "Error loading image page for category {CategoryId}")]
    internal static partial void LogImagePageError(ILogger logger, Exception exception, int categoryId);

    internal static void RecordError(Activity? activity, Exception exception)
    {
        activity?.SetStatus(ActivityStatusCode.Error);
        activity?.SetTag("error.type", exception.GetType().FullName);
        activity?.AddEvent(new ActivityEvent("exception", tags: new ActivityTagsCollection
        {
            { "exception.type", exception.GetType().FullName },
            { "exception.stacktrace", exception.StackTrace }
        }));
    }
}
