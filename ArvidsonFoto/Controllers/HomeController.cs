using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.Interfaces;
using ArvidsonFoto.Core.Services;
using ArvidsonFoto.Core.ViewModels;
using ArvidsonFoto.Views.Shared;
using System.Diagnostics;

namespace ArvidsonFoto.Controllers;

/// <summary>
/// Controller for handling home page and general site functionality.
/// </summary>
public class HomeController : Controller
{
    private readonly IApiPageCounterService _pageCounterService;

    /// <summary>
    /// Initializes a new instance of the <see cref="HomeController"/> class.
    /// </summary>
    /// <param name="coreContext">The Core database context</param>
    public HomeController(ArvidsonFotoCoreDbContext coreContext)
    {
        _pageCounterService = new ApiPageCounterService(coreContext);
    }

    public IActionResult Index()
    {
        ViewData["Title"] = "Startsidan";
        if (User?.Identity?.IsAuthenticated is false)
            _pageCounterService.AddPageCount("Startsidan");
        var viewModel = new GalleryViewModel();
        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        ViewData["Title"] = "Privacy Policy";
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var viewModel = new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };

        var url = Url.ActionContext.HttpContext;
        viewModel.VisitedUrl = HttpRequestExtensions.GetRawUrl(url);

        if (LogErrorUrlPost(viewModel.VisitedUrl))
            Log.Fatal("Navigation error to page: " + viewModel.VisitedUrl);

        if (viewModel.VisitedUrl is null)
            ViewData["Title"] = "Error";
        else if (viewModel.VisitedUrl.ToLower().StartsWith("/images/gallery"))
            ViewData["Title"] = "Error 301 - Old image Url";
        else
            ViewData["Title"] = "Error 404 - Page not found";

        return View(viewModel);
    }

    private bool LogErrorUrlPost(string visitedUrl)
    {
        bool logThisPost = true;

        if (visitedUrl.StartsWith("/images/gallery"))
            logThisPost = false;

        return logThisPost;
    }
}