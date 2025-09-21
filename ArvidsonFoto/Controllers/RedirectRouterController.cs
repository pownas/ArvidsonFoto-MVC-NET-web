using ArvidsonFoto.Views.Shared;

namespace ArvidsonFoto.Controllers;

public class RedirectRouterController() : Controller
{
    [Route("/Index.html")]
    [Route("/Default.asp")]
    [Route("/Default.aspx")]
    public RedirectToActionResult RedirectToHomeIndex()
    {
        //Kommentera på/av för att aktivera/inaktivera loggning av denna redirect:
        var url = Url.ActionContext.HttpContext;
        string visitedUrl = HttpRequestExtensions.GetRawUrl(url);
        Log.Fatal($"Redirect from page: {visitedUrl} , To startpage: /");

        return RedirectToActionPermanent(actionName: nameof(HomeController.Index), controllerName: nameof(HomeController).Replace("Controller", ""));
    }

    [Route("/Sk")]
    [Route("/Sok")]
    [Route("/Search")]
    [Route("/Search.asp")]
    public RedirectToActionResult RedirectToBilderSök()
    {
        ////Kommentera på/av för att aktivera/inaktivera loggning av denna redirect:
        //var url = Url.ActionContext.HttpContext;
        //string visitedUrl = HttpRequestExtensions.GetRawUrl(url);
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
        //string visitedUrl = HttpRequestExtensions.GetRawUrl(url);
        //Log.Fatal($"Redirect from page: {visitedUrl} , To page: /Senast/Fotograferad");

        return RedirectToActionPermanent(actionName: nameof(SenastController.Index), controllerName: nameof(SenastController).Replace("Controller", ""), routeValues: new { sortOrder = "Fotograferad" });
    }

    [Route("/Info.asp")]
    public RedirectToActionResult RedirectToInfoIndex()
    {
        //Kommentera på/av för att aktivera/inaktivera loggning av denna redirect:
        var url = Url.ActionContext.HttpContext;
        string visitedUrl = HttpRequestExtensions.GetRawUrl(url);
        Log.Fatal($"Redirect from page: {visitedUrl} , To page: /Info");

        return RedirectToActionPermanent(actionName: nameof(InfoController.Index), controllerName: nameof(InfoController).Replace("Controller", ""));
    }

    [Route("/Info/GB")]
    [Route("/Info/Gstbok")]
    [Route("/Info/Gastbok")]
    //[Route("/GB.asp")] //Tar bort denna routern för att slippa gamla spammers. 
    public RedirectToActionResult RedirectToInfoGästbok()
    {
        //Kommentera på/av för att aktivera/inaktivera loggning av denna redirect:
        var url = Url.ActionContext.HttpContext;
        string visitedUrl = HttpRequestExtensions.GetRawUrl(url);
        Log.Fatal($"Redirect from page: {visitedUrl} , To page: /Info/Gästbok");

        return RedirectToActionPermanent(actionName: nameof(InfoController.Gästbok), controllerName: nameof(InfoController).Replace("Controller", ""));
    }

    [Route("/Kontakta.asp")]
    [Route("/Kontakta")]
    [Route("/Info/Contact")]
    public RedirectToActionResult RedirectToInfoKontakta()
    {
        //Kommentera på/av för att aktivera/inaktivera loggning av denna redirect:
        var url = Url.ActionContext.HttpContext;
        string visitedUrl = HttpRequestExtensions.GetRawUrl(url);
        Log.Fatal($"Redirect from page: {visitedUrl} , To page: /Info/Kontakta");

        return RedirectToActionPermanent(actionName: nameof(InfoController.Kontakta), controllerName: nameof(InfoController).Replace("Controller", ""));
    }

    [Route("/Kop.asp")]
    [Route("/Köp_av_bilder")]
    [Route("/Info/Kp_av_bilder")]
    [Route("/Info/Kop_av_bilder")]
    public RedirectToActionResult RedirectToInfoKöp_av_bilder()
    {
        //Kommentera på/av för att aktivera/inaktivera loggning av denna redirect:
        var url = Url.ActionContext.HttpContext;
        string visitedUrl = HttpRequestExtensions.GetRawUrl(url);
        Log.Fatal($"Redirect from page: {visitedUrl} , To page: /Info/Köp_av_bilder");

        return RedirectToActionPermanent(actionName: nameof(InfoController.Köp_av_bilder), controllerName: nameof(InfoController).Replace("Controller", ""));
    }

    [Route("/Om.asp")]
    [Route("/Om")]
    public RedirectToActionResult RedirectToInfoOm_mig()
    {
        //Kommentera på/av för att aktivera/inaktivera loggning av denna redirect:
        var url = Url.ActionContext.HttpContext;
        string visitedUrl = HttpRequestExtensions.GetRawUrl(url);
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
        string visitedUrl = HttpRequestExtensions.GetRawUrl(url);
        Log.Fatal($"Redirect from page: {visitedUrl} , To page: /Info/Sidkarta");

        return RedirectToActionPermanent(actionName: nameof(InfoController.Sidkarta), controllerName: nameof(InfoController).Replace("Controller", ""));
    }

    [Route("/Copyright.asp")]
    [Route("/Copyright")]
    public RedirectToActionResult RedirectToInfoCopyright()
    {
        //Kommentera på/av för att aktivera/inaktivera loggning av denna redirect:
        var url = Url.ActionContext.HttpContext;
        string visitedUrl = HttpRequestExtensions.GetRawUrl(url);
        Log.Fatal($"Redirect from page: {visitedUrl} , To page: /Info/Copyright");

        return RedirectToActionPermanent(actionName: nameof(InfoController.Copyright), controllerName: nameof(InfoController).Replace("Controller", ""));
    }
}