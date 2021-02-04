using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ArvidsonFoto.Data;
using ArvidsonFoto.Models;
using ArvidsonFoto.Services;
using Microsoft.AspNetCore.Mvc;

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
            _pageCounterService.AddPageCount("Startsidan");
            var viewModel = new GalleryViewModel();
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
