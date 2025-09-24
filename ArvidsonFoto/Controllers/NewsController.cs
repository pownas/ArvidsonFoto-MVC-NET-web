using ArvidsonFoto.Data;
using ArvidsonFoto.Models;
using ArvidsonFoto.Services;

namespace ArvidsonFoto.Controllers;

public class NewsController(ArvidsonFotoDbContext context) : Controller
{
    internal INewsService _newsService = new NewsService(context);
    internal IPageCounterService _pageCounterService = new PageCounterService(context);

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