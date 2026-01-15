using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.Interfaces;
using ArvidsonFoto.Core.ViewModels;
using ArvidsonFoto.Core.Services;

namespace ArvidsonFoto.Controllers;

public class NewsController(ArvidsonFotoCoreDbContext coreContext, INewsService newsService, IPageCounterService pageCounterService) : Controller
{
    internal INewsService _newsService = newsService;
    internal IPageCounterService _pageCounterService = pageCounterService;

    [Route("[controller]")]
    public IActionResult Index()
    {
        ViewData["Title"] = "Nyheter";
        if (User?.Identity?.IsAuthenticated is false)
            _pageCounterService.AddPageCount("Nyheter");

        var viewModel = new NewsViewModel
        {
            NewsList = _newsService.GetPublished()
        };

        return View(viewModel);
    }

    [Route("Nyheter/{newsId}")]
    public IActionResult Article(int newsId)
    {
        var news = _newsService.GetByNewsId(newsId);
        if (news == null || !news.NewsPublished)
        {
            return RedirectToAction("Index");
        }

        ViewData["Title"] = news.NewsTitle;
        if (User?.Identity?.IsAuthenticated is false)
            _pageCounterService.AddPageCount($"Nyheter-{news.NewsTitle}");

        var viewModel = new NewsViewModel
        {
            SelectedNews = news
        };

        return View(viewModel);
    }
}