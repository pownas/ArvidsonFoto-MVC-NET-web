using ArvidsonFoto.Areas.Identity.Data;
using ArvidsonFoto.Data;
using ArvidsonFoto.Models;
using ArvidsonFoto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;

namespace ArvidsonFoto.Controllers;

[Authorize]
public class UploadAdminController(ArvidsonFotoDbContext context, UserManager<ArvidsonFotoUser> userManager) : Controller
{
    internal IImageService _imageService = new ImageService(context);
    internal ICategoryService _categoryService = new CategoryService(context);
    internal IGuestBookService _guestBookService = new GuestBookService(context);
    internal readonly UserManager<ArvidsonFotoUser> _userManager = userManager;

    public IActionResult Index()
    {
        ViewData["Title"] = "Upload Admin";
        return View();
    }

    [Route("/[controller]/NyBild")]
    [Route("/[controller]/NyBild/{subLevel1}")]
    [Route("/[controller]/NyBild/{subLevel1}/{subLevel2}")]
    [Route("/[controller]/NyBild/{subLevel1}/{subLevel2}/{subLevel3}")]
    [Route("/[controller]/NyBild/{subLevel1}/{subLevel2}/{subLevel3}/{subLevel4}")]
    public IActionResult NyBild(string subLevel1, string subLevel2, string subLevel3, string subLevel4, UploadImageInputModel inputModel)
    {
        ViewData["Title"] = "Länka till ny bild";
        UploadImageViewModel viewModel = new UploadImageViewModel();
        viewModel.ImageInputModel = inputModel;

        if (subLevel4 is not null)
        {
            viewModel.SelectedCategory = _categoryService.GetByName(subLevel4);
            viewModel.SubCategories = _categoryService.GetSubsList(_categoryService.GetByName(subLevel4).MenuId).OrderBy(c => c.MenuText).ToList();
            viewModel.CurrentUrl = "./UploadAdmin/NyBild/" + subLevel1 + "/" + subLevel2 + "/" + subLevel3 + "/" + subLevel4;
        }
        else if (subLevel3 is not null)
        {
            viewModel.SelectedCategory = _categoryService.GetByName(subLevel3);
            viewModel.SubCategories = _categoryService.GetSubsList(_categoryService.GetByName(subLevel3).MenuId).OrderBy(c => c.MenuText).ToList();
            viewModel.CurrentUrl = "./UploadAdmin/NyBild/" + subLevel1 + "/" + subLevel2 + "/" + subLevel3;
        }
        else if (subLevel2 is not null)
        {
            viewModel.SelectedCategory = _categoryService.GetByName(subLevel2);
            viewModel.SubCategories = _categoryService.GetSubsList(_categoryService.GetByName(subLevel2).MenuId).OrderBy(c => c.MenuText).ToList();
            viewModel.CurrentUrl = "./UploadAdmin/NyBild/" + subLevel1 + "/" + subLevel2;
        }
        else if (subLevel1 is not null)
        {
            viewModel.SelectedCategory = _categoryService.GetByName(subLevel1);
            viewModel.SubCategories = _categoryService.GetSubsList(_categoryService.GetByName(subLevel1).MenuId).OrderBy(c => c.MenuText).ToList();
            viewModel.CurrentUrl = "./UploadAdmin/NyBild/" + subLevel1;
        }
        else
        {
            viewModel.SubCategories = _categoryService.GetSubsList(0); //Hämtar top-level menyn
            viewModel.CurrentUrl = "./UploadAdmin/NyBild";
        }

        return View(viewModel);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult CreateImageLink(UploadImageInputModel model)
    {
        model.ImageCreated = false;

        if (ModelState.IsValid)
        {
            if (model.ImageHuvudfamilj.Equals(0))
                model.ImageHuvudfamilj = null;

            if (model.ImageFamilj.Equals(0))
                model.ImageFamilj = null;

            if (model.ImageHuvudfamilj.Equals(1)) //ID för Fåglar = 1
                model.ImageHuvudfamilj = null;

            TblImage newImage = new TblImage()
            {
                ImageHuvudfamilj = model.ImageHuvudfamilj,
                ImageFamilj = model.ImageFamilj,
                ImageArt = model.ImageArt,
                ImageId = _imageService.GetImageLastId() + 1,
                ImageUpdate = DateTime.Now,
                ImageDate = model.ImageDate,
                ImageDescription = model.ImageDescription,
                ImageUrl = model.ImageUrl
            };

            model.ImageCreated = _imageService.AddImage(newImage); //Om allt OK...
        }
        if (model.ImageCreated)
            return RedirectToAction("NyBild", new { model.ImageCreated, model.ImageArt, model.ImageUrl });
        else
            return RedirectToAction("NyBild", model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult EditImageLink(UploadImageInputModel model)
    {
        if (ModelState.IsValid)
        {
            if (_imageService.UpdateImage(model)) //Lägg in skapandet här, och då sätt som true/false..
                return RedirectToAction("RedigeraBilder", new { DisplayMessage = "OkImgEdit", imgId = model.ImageArt });
        }

        return RedirectToAction("RedigeraBilder", model);
    }

    public IActionResult NyKategori(UploadNewCategoryModel inputModel)
    {
        ViewData["Title"] = "Länka till ny kategori";
        return View(inputModel);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult CreateCategory(UploadNewCategoryModel inputModel)
    {
        inputModel.CategoryCreated = false;

        if (ModelState.IsValid)
        {
            TblMenu newCategory = new TblMenu()
            {
                MenuText = inputModel.MenuText,
                MenuId = _categoryService.GetLastId() + 1,
                MenuMainId = inputModel.MainMenuId
            };

            if (_categoryService.AddCategory(newCategory))
            {
                inputModel.CategoryCreated = true; //Om allt OK...
                inputModel.MainMenuId = null;
            }
        }

        return RedirectToAction("NyKategori", inputModel);
    }

    public IActionResult RedigeraBilder(string DisplayMessage, string imgId, int? sida)
    {
        ViewData["Title"] = "Redigera bland bilderna";

        int imagesPerPage = 25;
        if (sida is null || sida < 1)
            sida = 1;

        List<TblImage> displayTblImages = new List<TblImage>();

        UploadEditImagesViewModel viewModel = new UploadEditImagesViewModel()
        {
            AllImagesList = _imageService.GetAll().OrderByDescending(i => i.ImageId).ToList(),
            CurrentPage = (int)sida,
            CurrentUrl = "./UploadAdmin/RedigeraBilder"
        };
        viewModel.TotalPages = (int)Math.Ceiling(viewModel.AllImagesList.Count() / (decimal)imagesPerPage);
        displayTblImages = viewModel.AllImagesList
                                    .Skip((viewModel.CurrentPage - 1) * imagesPerPage)
                                    .Take(imagesPerPage)
                                    .ToList();
        viewModel.DisplayImagesList = new List<UploadImageInputModel>();

        if (string.IsNullOrWhiteSpace(DisplayMessage) && string.IsNullOrWhiteSpace(imgId))
        {
            viewModel.DisplayMessage = "";
        }
        else
        {
            viewModel.DisplayMessage = DisplayMessage;
            viewModel.UpdatedId = Convert.ToInt32(imgId);
        }

        foreach (var item in displayTblImages)
        {
            DateTime imgDate = item.ImageDate ?? new DateTime(1900, 01, 01);

            UploadImageInputModel inputModel = new UploadImageInputModel()
            {
                ImageId = item.ImageId,
                ImageHuvudfamilj = item.ImageHuvudfamilj,
                ImageHuvudfamiljNamn = _categoryService.GetNameById(item.ImageHuvudfamilj),
                ImageFamilj = item.ImageFamilj,
                ImageFamiljNamn = _categoryService.GetNameById(item.ImageFamilj),
                ImageArt = item.ImageArt,
                ImageArtNamn = _categoryService.GetNameById(item.ImageArt),
                ImageDate = imgDate,
                ImageUpdate = item.ImageUpdate,
                ImageDescription = item.ImageDescription,
                ImageUrl = item.ImageUrl
            };

            inputModel.ImageUrlFullSrc = "https://arvidsonfoto.se/Bilder";
            if (inputModel.ImageHuvudfamilj is not null)
                inputModel.ImageUrlFullSrc += "/" + inputModel.ImageHuvudfamiljNamn;
            if (inputModel.ImageFamilj is not null)
                inputModel.ImageUrlFullSrc += "/" + inputModel.ImageFamiljNamn;

            inputModel.ImageUrlFullSrc += "/" + inputModel.ImageArtNamn + "/" + inputModel.ImageUrl;

            viewModel.DisplayImagesList.Add(inputModel);
        }

        return View(viewModel);
    }

    public IActionResult HanteraGB(string DisplayMessage, string gbId)
    {
        ViewData["Title"] = "Hantera gästboken";
        UploadGbViewModel viewModel = new UploadGbViewModel();
        if (string.IsNullOrWhiteSpace(DisplayMessage) && string.IsNullOrWhiteSpace(gbId))
        {
            viewModel.DisplayMessage = "";
        }
        else
        {
            viewModel.DisplayMessage = DisplayMessage;
            viewModel.UpdatedId = gbId;
        }
        return View(viewModel);
    }

    public IActionResult MarkGbPostAsRead(int gbId)
    {
        if (User?.Identity?.IsAuthenticated is true)
        {
            if (_guestBookService.ReadGbPost(gbId))
            {
                return RedirectToAction("HanteraGB", new { DisplayMessage = "OkGbRead", gbId = gbId });
            }
        }
        return RedirectToAction("HanteraGB", new { DisplayMessage = "ErrorGbRead", gbId = gbId });
    }

    public IActionResult DeleteGbPost(int gbId)
    {
        if (User?.Identity?.IsAuthenticated is true)
        {
            if (_guestBookService.DeleteGbPost(gbId))
            {
                return RedirectToAction("HanteraGB", new { DisplayMessage = "OkGbDelete", gbId = gbId });
            }
        }
        return RedirectToAction("HanteraGB", new { DisplayMessage = "ErrorGbDelete", gbId = gbId });
    }

    public IActionResult DeleteImage(int imgId)
    {
        if (User?.Identity?.IsAuthenticated is true)
        {
            if (_imageService.DeleteImgId(imgId))
            {
                return RedirectToAction("RedigeraBilder", new { DisplayMessage = "OkImgDelete", imgId = imgId });
            }
        }
        return RedirectToAction("RedigeraBilder", new { DisplayMessage = "ErrorImgDelete", imgId = imgId });
    }

    public IActionResult Statistik()
    {
        ViewData["Title"] = "Hemsidans statistik";
        // Hämta sidvisningar per månad (senaste 12 månader)
        var monthlyPageViews = _pageCounterService.GetMonthlyPageViews(12);
        ViewBag.MonthlyPageViews = monthlyPageViews;
        return View();
    }

    /// <summary>Laddar sidan "Läs loggboken" , men heter visa för att slippa ä i Läs</summary>
    /// <param name="datum">Format: ÅÅÅÅMMDD (t.ex: 20210126)</param>
    public async Task<IActionResult> VisaLoggbokenAsync(DateTime datum)
    {
        ViewData["Title"] = "Läser loggboken för: " + datum.ToString("yyyy-MM-dd dddd");

        AppLogReader logReader = new AppLogReader();
        string appLogFile = "appLog" + datum.ToString("yyyyMMdd") + ".txt";

        UploadLogReaderViewModel viewModel = new UploadLogReaderViewModel();
        viewModel.ExistingLogFiles = logReader.ExistingLogFiles();
        viewModel.LogBook = logReader.ReadData(appLogFile);
        viewModel.DateShown = datum;

        ArvidsonFotoUser user = await _userManager.GetUserAsync(User) ?? new();
        viewModel.ShowAllLogs = user.ShowAllLogs;

        return View(viewModel);
    }

    /// <summary>
    /// Funktion som aktiverar/inaktiverar alla Loggar. Kanske dock borde ligga i AccountManagement annars...
    /// </summary>
    [Route("[controller]/ToggleShowAllLogs")]
    public async Task<IActionResult> ToggleShowAllLogs(string date)
    {
        if (string.IsNullOrWhiteSpace(date))
            date = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

        ArvidsonFotoUser user = await _userManager.GetUserAsync(User) ?? new();
        if (user.ShowAllLogs)
            user.ShowAllLogs = false;
        else
            user.ShowAllLogs = true;

        await _userManager.UpdateAsync(user);
        return RedirectToAction("VisaLoggboken", new { datum = date });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}