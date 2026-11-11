using System.Diagnostics;

namespace ArvidsonFoto.Core.Diagnostics;

internal sealed class ImageTraceMiddleware(RequestDelegate next, ImageTraceContext traceContext,
    ILogger<ImageTraceMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (!traceContext.TryGetParent(context.Request, out var parent))
        {
            if (ImageTraceContext.IsImagePath(context.Request.Path.Value)
                && context.Request.Query.ContainsKey(ImageTraceContext.QueryParameter))
            {
                AppTelemetry.LogInvalidImageContext(logger, context.Request.Path.ToString());
            }

            await next(context);
            return;
        }

        // Native img requests cannot send traceparent headers. Export this server span instead
        // of the unrelated hosting span, using the page's context carried in the image URL.
        using var activity = AppTelemetry.ActivitySource.StartActivity(
            $"image.fetch {context.Request.Path}", ActivityKind.Server, parent);
        activity?.SetTag("http.request.method", context.Request.Method);
        activity?.SetTag("url.path", context.Request.Path.Value);
        activity?.SetTag("arvidsonfoto.image.correlated", true);
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            AppTelemetry.RecordError(activity, exception);
            throw;
        }
        finally
        {
            activity?.SetTag("http.response.status_code", context.Response.StatusCode);
            activity?.SetTag("http.response.body.size", context.Response.ContentLength);
            activity?.SetTag("http.response.header.content-type", context.Response.ContentType);
            if (context.Response.StatusCode >= 500)
            {
                activity?.SetStatus(ActivityStatusCode.Error);
            }

            if (logger.IsEnabled(LogLevel.Debug))
            {
                var imagePath = context.Request.Path.ToString();
                AppTelemetry.LogImageResponse(logger, imagePath, context.Response.StatusCode,
                    context.Response.ContentLength, context.Response.ContentType);
            }
        }
    }
}