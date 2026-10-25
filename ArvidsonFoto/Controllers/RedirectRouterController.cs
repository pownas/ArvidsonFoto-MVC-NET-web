using ArvidsonFoto.Views.Shared;
using ArvidsonFoto.Core.Extensions;
using ArvidsonFoto.Core.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArvidsonFoto.Controllers;

public class RedirectRouterController() : Controller
{
    public static void ConfigureLowercaseUrlFallback(WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            if (context.Features.Get<IStatusCodeReExecuteFeature>() is not null)
            {
                await next();
                return;
            }

            var endpoint = context.GetEndpoint();
            var isPageEndpoint = endpoint?.Metadata.GetMetadata<PageActionDescriptor>() is not null;
            var controllerAction = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
            if (!isPageEndpoint && controllerAction is null)
            {
                await next();
                return;
            }

            context.Response.OnStarting(() =>
            {
                var location = context.Response.Headers.Location.ToString();
                if (location.Length > 0)
                {
                    context.Response.Headers.Location = LowercaseLocalLocationPath(location, context.Request.Host);
                }

                return Task.CompletedTask;
            });

            var path = context.Request.Path.Value;
            if (string.IsNullOrEmpty(path))
            {
                await next();
                return;
            }

            var canonicalPath = path.ToLowerInvariant();
            if (controllerAction?.ControllerName == "Bilder" && controllerAction.ActionName == "Index")
            {
                var isEnglishGalleryPath = path.StartsWith("/images/", StringComparison.OrdinalIgnoreCase);
                var categories = context.RequestServices.GetRequiredService<IApiCategoryService>().GetAll();
                canonicalPath = LocalizedRoutes.Switch(path, isEnglishGalleryPath, categories);
            }
            else if (controllerAction?.ControllerName == "ImageApi" &&
                     controllerAction.ActionName == "GetImagesByCategoryPath" &&
                     (path.StartsWith("/api/bilder/", StringComparison.OrdinalIgnoreCase) ||
                      path.StartsWith("/api/images/", StringComparison.OrdinalIgnoreCase)))
            {
                var galleryPath = path[4..];
                var isEnglishGalleryPath = galleryPath.StartsWith("/images/", StringComparison.OrdinalIgnoreCase);
                var categories = context.RequestServices.GetRequiredService<IApiCategoryService>().GetAll();
                canonicalPath = "/api" + LocalizedRoutes.Switch(galleryPath, isEnglishGalleryPath, categories);
            }

            if (!string.Equals(path, canonicalPath, StringComparison.Ordinal))
            {
                context.Response.StatusCode = StatusCodes.Status308PermanentRedirect;
                context.Response.Headers.Location = string.Concat(
                    context.Request.PathBase.Value,
                    canonicalPath,
                    context.Request.QueryString.Value);
                return;
            }

            await next();
        });
    }

    private static string LowercaseLocalLocationPath(string location, HostString requestHost)
    {
        if (location.StartsWith("/", StringComparison.Ordinal))
        {
            var relativePathEnd = location.IndexOfAny(['?', '#']);
            if (relativePathEnd < 0)
            {
                relativePathEnd = location.Length;
            }

            return string.Concat(location[..relativePathEnd].ToLowerInvariant(), location[relativePathEnd..]);
        }

        if (Uri.TryCreate(location, UriKind.Absolute, out var absoluteUri))
        {
            if (!absoluteUri.Host.Equals(requestHost.Host, StringComparison.OrdinalIgnoreCase))
            {
                return location;
            }

            var authorityEnd = location.IndexOf("//", StringComparison.Ordinal) + 2;
            var pathStart = location.IndexOf('/', authorityEnd);
            if (pathStart < authorityEnd)
            {
                return location;
            }

            var suffixStart = location.IndexOfAny(['?', '#'], pathStart);
            if (suffixStart < 0)
            {
                suffixStart = location.Length;
            }

            return string.Concat(location[..pathStart], location[pathStart..suffixStart].ToLowerInvariant(), location[suffixStart..]);
        }

        return location;
    }

    [Route("/Index.html")]
    [Route("/Default.asp")]
    [Route("/Default.aspx")]
    public RedirectToActionResult RedirectToHomeIndex()
    {
        //Kommentera på/av för att aktivera/inaktivera loggning av denna redirect:
        var url = Url.ActionContext.HttpContext;
        string? visitedUrl = HttpRequestExtensions.GetRawUrl(url);
        Log.Fatal($"Redirect from page: {visitedUrl} , To startpage: /");

        return RedirectToActionPermanent(actionName: nameof(HomeController.Index), controllerName: nameof(HomeController).Replace("Controller", ""));
    }

    [Route("/Sk")]
    [Route("/Sök")]
    [Route("/sok")]
    [Route("/Sökning")]
    [Route("/Searching")]
    [Route("/Search.asp")]
    public RedirectToActionResult RedirectToBilderSearch()
    {
        ////Kommentera på/av för att aktivera/inaktivera loggning av denna redirect:
        //var url = Url.ActionContext.HttpContext;
        //string? visitedUrl = HttpRequestExtensions.GetRawUrl(url);
        //Log.Fatal($"Redirect from page: {visitedUrl} , To page: /Sök");

        return RedirectToActionPermanent(actionName: nameof(BilderController.Search), controllerName: nameof(BilderController).Replace("Controller", ""));
    }

    [Route("/Latest.asp")]
    [Route("/Bild_kalender.asp")]
    [Route("/Bilder/Senast")]
    [Route("/Bilder/Senast/{sortering}")]
    [Route("/Senast")]
    [Route("/Senast/Index.html")]
    [Route("/Senast/Default.asp")]
    public RedirectToActionResult RedirectToSenast(string sortering)
    {
        ////Kommentera på/av för att aktivera/inaktivera loggning av denna redirect:
        //var url = Url.ActionContext.HttpContext;
        //string? visitedUrl = HttpRequestExtensions.GetRawUrl(url);
        //Log.Fatal($"Redirect from page: {visitedUrl} , To page: /Senast/Fotograferad");

        return RedirectToActionPermanent(actionName: nameof(SenastController.Index), controllerName: nameof(SenastController).Replace("Controller", ""), routeValues: new { sortOrder = "Fotograferad" });
    }

    [Route("/Info.asp")]
    public RedirectToActionResult RedirectToInfoIndex()
    {
        //Kommentera på/av för att aktivera/inaktivera loggning av denna redirect:
        var url = Url.ActionContext.HttpContext;
        string? visitedUrl = HttpRequestExtensions.GetRawUrl(url);
        Log.Fatal($"Redirect from page: {visitedUrl} , To page: /Info");

        return RedirectToActionPermanent(actionName: nameof(InfoController.Index), controllerName: nameof(InfoController).Replace("Controller", ""));
    }

    [Route("/Info/GB")]
    [Route("/Info/Gstbok")]
    [Route("/Info/Gästbok")]
    //[Route("/GB.asp")] //Tar bort denna routern för att slippa gamla spammers. 
    public RedirectToActionResult RedirectToInfoGastbok()
    {
        //Kommentera på/av för att aktivera/inaktivera loggning av denna redirect:
        var url = Url.ActionContext.HttpContext;
        string? visitedUrl = HttpRequestExtensions.GetRawUrl(url);
        Log.Fatal($"Redirect from page: {visitedUrl} , To page: /Info/Gastbok");

        return RedirectToActionPermanent(actionName: nameof(InfoController.Gastbok), controllerName: nameof(InfoController).Replace("Controller", ""));
    }

    [Route("/Kontakta.asp")]
    [Route("/Kontakta")]
    [Route("/Info/Contact")]
    public RedirectToActionResult RedirectToInfoKontakta()
    {
        //Kommentera på/av för att aktivera/inaktivera loggning av denna redirect:
        var url = Url.ActionContext.HttpContext;
        string? visitedUrl = HttpRequestExtensions.GetRawUrl(url);
        Log.Fatal($"Redirect from page: {visitedUrl} , To page: /Info/Kontakta");

        return RedirectToActionPermanent(actionName: nameof(InfoController.Kontakta), controllerName: nameof(InfoController).Replace("Controller", ""));
    }

    [Route("/Kop.asp")]
    [Route("/Kop_av_bilder")]
    [Route("/Köp_av_bilder")]
    [Route("/Info/Kp_av_bilder")]
    [Route("/Info/Köp_av_bilder")]
    public RedirectToActionResult RedirectToInfoKop_av_bilder()
    {
        //Kommentera på/av för att aktivera/inaktivera loggning av denna redirect:
        var url = Url.ActionContext.HttpContext;
        string? visitedUrl = HttpRequestExtensions.GetRawUrl(url);
        Log.Fatal($"Redirect from page: {visitedUrl} , To page: /Info/Kop_av_bilder");

        return RedirectToActionPermanent(actionName: nameof(InfoController.Kop_av_bilder), controllerName: nameof(InfoController).Replace("Controller", ""));
    }

    [Route("/Om.asp")]
    [Route("/Om")]
    public RedirectToActionResult RedirectToInfoOm_mig()
    {
        //Kommentera på/av för att aktivera/inaktivera loggning av denna redirect:
        var url = Url.ActionContext.HttpContext;
        string? visitedUrl = HttpRequestExtensions.GetRawUrl(url);
        Log.Fatal($"Redirect from page: {visitedUrl} , To page: /Info/Om_mig");

        return RedirectToActionPermanent(actionName: nameof(InfoController.Om_mig), controllerName: nameof(InfoController).Replace("Controller", ""));
    }

    [Route("/Sitemap.txt")]
    [Route("/Sitemap.asp")]
    [Route("/Sidkarta")]
    public RedirectToActionResult RedirectToInfoSidkarta()
    {
        //Kommentera på/av för att aktivera/inaktivera loggning av denna redirect:
        var url = Url.ActionContext.HttpContext;
        string? visitedUrl = HttpRequestExtensions.GetRawUrl(url);
        Log.Fatal($"Redirect from page: {visitedUrl} , To page: /Info/Sidkarta");

        return RedirectToActionPermanent(actionName: nameof(InfoController.Sidkarta), controllerName: nameof(InfoController).Replace("Controller", ""));
    }

    [Route("/Copyright.asp")]
    [Route("/Copyright")]
    public RedirectToActionResult RedirectToInfoCopyright()
    {
        //Kommentera på/av för att aktivera/inaktivera loggning av denna redirect:
        var url = Url.ActionContext.HttpContext;
        string? visitedUrl = HttpRequestExtensions.GetRawUrl(url);
        Log.Fatal($"Redirect from page: {visitedUrl} , To page: /Info/Copyright");

        return RedirectToActionPermanent(actionName: nameof(InfoController.Copyright), controllerName: nameof(InfoController).Replace("Controller", ""));
    }
}