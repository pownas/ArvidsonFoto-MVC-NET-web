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
            //HttpRequestFeature rawerUrl = this.IHttpRequestFeature;
            //PropertyInfo rawUrl = rawerUrl.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).Single(pi => pi.Name == "RawTarget");
            //object url = rawUrl.GetValue(rawerUrl, null);
            var url = Url.ActionContext.HttpContext;

            string currentUrl = HttpRequestExtensions.GetRawUrl(url);

            //foreach (var item in url.Where(uri => uri.Key.Equals("RawTarget")))
            //foreach (var item in url)
            //{
            //    if (item.Key.Equals("IHttpResponseFeature"))
            //    {    //currentUrl = item.Value.ToString();
            //        currentUrl = item.ToString();
            //        currentUrl2 = item.Value.ToString();
            //    }
            //    if (item.Value.Equals("IHttpResponseFeature"))
            //    {
            //        //currentUrl = item.Value.ToString();
            //        currentUrl = item.ToString();
            //        currentUrl2 = item.Value.ToString();
            //    }
            //    if (item.Value.GetType().Equals("RawTarget"))
            //    {
            //        currentUrl = item.ToString();
            //        currentUrl2 = item.Key.ToString();
            //        //currentUrl = item.Key.ToString();
            //    }
            //    if (item.Equals("RawTarget"))
            //    {
            //        currentUrl = item.ToString();
            //        currentUrl2 = item.Value.ToString();
            //    }
            //}

            Log.Warning("Navigation error to page: " + currentUrl);

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}