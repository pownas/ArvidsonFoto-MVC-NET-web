using ArvidsonFoto.Core.Interfaces;
using ArvidsonFoto.Core.ViewModels;
using ArvidsonFoto.Views.Shared;
using System.Diagnostics;

namespace ArvidsonFoto.Controllers;

/// <summary>
/// Controller for handling home page and general site functionality.
/// </summary>
public class HomeController : Controller
{
    private readonly IPageCounterService _pageCounterService;
    private readonly IApiImageService _imageService;

    /// <summary>
    /// Initializes a new instance of the <see cref="HomeController"/> class.
    /// </summary>
    /// <param name="pageCounterService">The page counter service for tracking page views</param>
    /// <param name="imageService">The image service for fetching images</param>
    public HomeController(IPageCounterService pageCounterService, IApiImageService imageService)
    {
        _pageCounterService = pageCounterService;
        _imageService = imageService;
    }

    public IActionResult Index()
    {
        ViewData["Title"] = "Startsidan";
        if (User?.Identity?.IsAuthenticated is false)
            _pageCounterService.AddPageCount("Startsidan");
        
        var viewModel = new GalleryViewModel
        {
            DisplayImagesList = _imageService.GetRandomNumberOfImages(12)
        };
        
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