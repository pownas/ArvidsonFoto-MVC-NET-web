using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ArvidsonFoto.Data;
using ArvidsonFoto.Models;
using ArvidsonFoto.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ArvidsonFoto.Controllers
{
    public class HomeController : Controller
    {
        private IPageCounterService _pageCounterService;

        public HomeController(ArvidsonFotoDbContext context)
        {
            _pageCounterService = new PageCounterService(context);
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Startsidan";
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

            if(LogErrorUrlPost(viewModel.VisitedUrl))
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

            if(visitedUrl.StartsWith("/images/gallery"))
                logThisPost = false;

            return logThisPost;
        }
    }
}