using System.Diagnostics;
using ArvidsonFoto.Core.ApiResponses;
using ArvidsonFoto.Core.DTOs;
using ArvidsonFoto.Core.Extensions;
using ArvidsonFoto.Core.ViewModels;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ArvidsonFoto.Core.Diagnostics;

internal sealed class MvcTraceFilter(ILogger<MvcTraceFilter> logger) : IAsyncActionFilter, IAsyncResultFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
        using var activity = AppTelemetry.ActivitySource.StartActivity(
            $"mvc.action {descriptor?.ControllerName}.{descriptor?.ActionName}");
        activity?.SetTag("mvc.controller", descriptor?.ControllerName);
        activity?.SetTag("mvc.action", descriptor?.ActionName);
        activity?.SetTag("url.path", context.HttpContext.Request.Path.Value);
        if (context.ActionArguments.TryGetValue("categoryId", out var categoryId) && categoryId is int requestedCategoryId)
        {
            activity?.SetTag("arvidsonfoto.category.requested_id", requestedCategoryId);
        }
        try
        {
            var executed = await next();
            activity?.SetTag("mvc.result.type", executed.Result?.GetType().Name);
            AddResultDetails(activity, executed.Result);
            AddResultDetails(context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity, executed.Result);
            if (executed.Exception is not null && !executed.ExceptionHandled)
            {
                AppTelemetry.RecordError(activity, executed.Exception);
            }
        }
        catch (Exception exception)
        {
            AppTelemetry.RecordError(activity, exception);
            throw;
        }
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var view = context.Result as ViewResult;
        var resultName = view is not null
            ? $"mvc.view {view.ViewName ?? context.RouteData.Values["action"]?.ToString()}"
            : $"mvc.result {context.Result.GetType().Name}";
        using var activity = AppTelemetry.ActivitySource.StartActivity(resultName);
        var started = Stopwatch.GetTimestamp();
        activity?.SetTag("mvc.result.type", context.Result.GetType().Name);
        activity?.SetTag("mvc.view", view?.ViewName ?? (view is not null ? context.RouteData.Values["action"]?.ToString() : null));
        AddResultDetails(activity, context.Result);
        try
        {
            var executed = await next();
            if (executed.Exception is not null && !executed.ExceptionHandled)
            {
                AppTelemetry.RecordError(activity, executed.Exception);
            }
        }
        catch (Exception exception)
        {
            AppTelemetry.RecordError(activity, exception);
            throw;
        }
        finally
        {
            activity?.SetTag("http.response.status_code", context.HttpContext.Response.StatusCode);
            activity?.SetTag("http.response.header.content-type", context.HttpContext.Response.ContentType);
            if (logger.IsEnabled(LogLevel.Debug))
            {
                var elapsedMs = Stopwatch.GetElapsedTime(started).TotalMilliseconds;
                AppTelemetry.LogMvcResult(logger, activity?.DisplayName ?? resultName, context.HttpContext.Response.StatusCode,
                    elapsedMs);
            }
        }
    }

    private static void AddResultDetails(Activity? activity, IActionResult? result)
    {
        if (activity is null)
        {
            return;
        }

        string? summary = null;
        if (result is ViewResult { Model: GalleryViewModel gallery } && gallery.SelectedCategory is { CategoryId: > 0 } category)
        {
            activity.SetTag("arvidsonfoto.category.id", category.CategoryId);
            activity.SetTag("arvidsonfoto.category.name_sv", category.NameSv);
            activity.SetTag("arvidsonfoto.category.name_en", category.NameEn);
            activity.SetTag("arvidsonfoto.page.number", gallery.CurrentPage);
            activity.SetTag("arvidsonfoto.page.size", gallery.PageSize);
            activity.SetTag("arvidsonfoto.page.total", gallery.TotalPages);
            activity.SetTag("arvidsonfoto.image.total", gallery.TotalImageCount);
            activity.SetTag("arvidsonfoto.result.count", gallery.DisplayImagesList?.Count ?? 0);
            summary = $"{LocalizedText.Select(category.NameSv, category.NameEn)}, category {category.CategoryId}, page {gallery.CurrentPage} ({gallery.DisplayImagesList?.Count ?? 0}/{gallery.TotalImageCount} images)";
        }
        else if (result is ObjectResult { Value: ImageDto image })
        {
            activity.SetTag("arvidsonfoto.category.id", image.CategoryId);
            activity.SetTag("arvidsonfoto.image.id", image.ImageId);
            activity.SetTag("arvidsonfoto.image.path", image.UrlImage);
            activity.SetTag("arvidsonfoto.result.found", image.ImageId > 0);
            summary = $"category {image.CategoryId}, image {image.ImageId}: {image.UrlImage?.Split('/').LastOrDefault()}";
        }
        else if (result is ObjectResult { Value: ImageListResponse images })
        {
            activity.SetTag("arvidsonfoto.category.id", images.CategoryId);
            activity.SetTag("arvidsonfoto.category.name_sv", images.CategoryNameSv);
            activity.SetTag("arvidsonfoto.category.name_en", images.CategoryNameEn);
            activity.SetTag("arvidsonfoto.page.number", images.CurrentPage);
            activity.SetTag("arvidsonfoto.page.size", images.PageSize);
            activity.SetTag("arvidsonfoto.image.total", images.ImageCategoryTotalCount);
            activity.SetTag("arvidsonfoto.result.count", images.ImageResultCount);
            summary = $"{LocalizedText.Select(images.CategoryNameSv, images.CategoryNameEn)}, category {images.CategoryId}, page {images.CurrentPage} ({images.ImageResultCount}/{images.ImageCategoryTotalCount} images)";
        }

        if (summary is not null)
        {
            activity.SetTag("arvidsonfoto.result.summary", summary);
            if (activity.Kind != ActivityKind.Server)
            {
                activity.DisplayName = $"{activity.DisplayName} [{summary}]";
            }
        }
    }
}