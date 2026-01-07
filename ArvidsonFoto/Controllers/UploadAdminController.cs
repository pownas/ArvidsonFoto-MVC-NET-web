using ArvidsonFoto.Areas.Identity.Data;
using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.DTOs;
using ArvidsonFoto.Core.Interfaces;
using ArvidsonFoto.Core.Services;
using ArvidsonFoto.Core.ViewModels;
using ArvidsonFoto.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace ArvidsonFoto.Controllers;

[Authorize]
public class UploadAdminController : Controller
{
    internal IApiImageService _imageService;
    internal IApiCategoryService _categoryService;
    internal IGuestBookService _guestBookService;
    internal readonly UserManager<ArvidsonFotoUser> _userManager;
    internal readonly IFacebookService _facebookService;

    public UploadAdminController(
        ArvidsonFotoCoreDbContext coreContext,
        UserManager<ArvidsonFotoUser> userManager,
        IFacebookService facebookService,
        ILogger<ApiImageService> imageLogger,
        ILogger<ApiCategoryService> categoryLogger,
        IConfiguration configuration,
        IMemoryCache memoryCache)
    {
        _imageService = new ApiImageService(imageLogger, coreContext, configuration, new ApiCategoryService(categoryLogger, coreContext, memoryCache));
        _categoryService = new ApiCategoryService(categoryLogger, coreContext, memoryCache);
        _guestBookService = new GuestBookService(coreContext);
        _userManager = userManager;
        _facebookService = facebookService;
    }

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
    public IActionResult NyBild(string subLevel1, string? subLevel2, string? subLevel3, string? subLevel4, UploadImageInputDto inputModel)
    {
        ViewData["Title"] = "Länka till ny bild";
        UploadImageViewModel viewModel = new UploadImageViewModel();
        viewModel.ImageInputModel = inputModel ?? UploadImageInputDto.CreateEmpty();

        var selectedCategory = CategoryDto.CreateEmpty();
        var subCategories = new List<CategoryDto>();

        if (subLevel4 is not null)
        {
            selectedCategory = _categoryService.GetByName(subLevel4);
            subCategories = _categoryService.GetSubsList(selectedCategory.CategoryId ?? 0);
            viewModel.CurrentUrl = "./UploadAdmin/NyBild/" + subLevel1 + "/" + subLevel2 + "/" + subLevel3 + "/" + subLevel4;
        }
        else if (subLevel3 is not null)
        {
            selectedCategory = _categoryService.GetByName(subLevel3);
            subCategories = _categoryService.GetSubsList(selectedCategory.CategoryId ?? 0);
            viewModel.CurrentUrl = "./UploadAdmin/NyBild/" + subLevel1 + "/" + subLevel2 + "/" + subLevel3;
        }
        else if (subLevel2 is not null)
        {
            selectedCategory = _categoryService.GetByName(subLevel2);
            subCategories = _categoryService.GetSubsList(selectedCategory.CategoryId ?? 0);
            viewModel.CurrentUrl = "./UploadAdmin/NyBild/" + subLevel1 + "/" + subLevel2;
        }
        else if (subLevel1 is not null)
        {
            selectedCategory = _categoryService.GetByName(subLevel1);
            subCategories = _categoryService.GetSubsList(selectedCategory.CategoryId ?? 0);
            viewModel.CurrentUrl = "./UploadAdmin/NyBild/" + subLevel1;
        }
        else
        {
            subCategories = _categoryService.GetSubsList(0);
            viewModel.CurrentUrl = "./UploadAdmin/NyBild";
        }

        viewModel.SelectedCategory = selectedCategory;
        viewModel.SubCategories = subCategories.OrderBy(c => c.Name).ToList();

        return View(viewModel);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult CreateImageLink(UploadImageInputDto model)
    {
        model.ImageCreated = false;

        if (ModelState.IsValid)
        {
            if (model.ImageHuvudfamilj.Equals(0))
                model.ImageHuvudfamilj = null;

            if (model.ImageFamilj.Equals(0))
                model.ImageFamilj = null;

            if (model.ImageHuvudfamilj.Equals(1))
                model.ImageHuvudfamilj = null;

            Core.Models.TblImage newImage = new Core.Models.TblImage
            {
                ImageId = _imageService.GetImageLastId() + 1,
                ImageMainFamilyId = model.ImageHuvudfamilj,
                ImageFamilyId = model.ImageFamilj,
                ImageCategoryId = model.ImageArt,
                ImageUpdate = DateTime.Now,
                ImageDate = model.ImageDate,
                ImageDescription = model.ImageDescription,
                ImageUrlName = model.ImageUrl
            };

            try
            {
                var coreContext = HttpContext.RequestServices.GetRequiredService<ArvidsonFotoCoreDbContext>();
                coreContext.TblImages.Add(newImage);
                coreContext.SaveChanges();
                model.ImageCreated = true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Fel vid skapande av bild");
                model.ImageCreated = false;
            }
        }
        if (model.ImageCreated)
            return RedirectToAction("NyBild", new { model.ImageCreated, model.ImageArt, model.ImageUrl });
        else
            return RedirectToAction("NyBild", model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditImageLink(UploadImageInputDto model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var coreContext = HttpContext.RequestServices.GetRequiredService<ArvidsonFotoCoreDbContext>();
                var existingImage = coreContext.TblImages
                    .Where(i => i.ImageId == model.ImageId)
                    .FirstOrDefault();
                    
                if (existingImage != null)
                {
                    existingImage.ImageUrlName = model.ImageUrl;
                    existingImage.ImageCategoryId = model.ImageArt;
                    existingImage.ImageFamilyId = model.ImageFamilj;
                    existingImage.ImageMainFamilyId = model.ImageHuvudfamilj;
                    existingImage.ImageDate = model.ImageDate;
                    existingImage.ImageDescription = model.ImageDescription;
                    existingImage.ImageUpdate = DateTime.Now;
                    
                    await coreContext.SaveChangesAsync();
                    return RedirectToAction("RedigeraBilder", new { DisplayMessage = "OkImgEdit", imgId = model.ImageArt });
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Fel vid uppdatering av bild");
            }
        }

        return RedirectToAction("RedigeraBilder", model);
    }

    public IActionResult NyKategori(UploadNewCategoryInputDto inputModel)
    {
        ViewData["Title"] = "Länka till ny kategori";
        return View(inputModel ?? UploadNewCategoryInputDto.CreateEmpty());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult CreateCategory(UploadNewCategoryInputDto inputModel)
    {
        inputModel.CategoryCreated = false;

        if (ModelState.IsValid)
        {
            CategoryDto newCategory = new CategoryDto
            {
                Name = inputModel.MenuText,
                CategoryId = _categoryService.GetLastId() + 1,
                ParentCategoryId = inputModel.MainMenuId
            };

            if (_categoryService.AddCategory(newCategory))
            {
                inputModel.CategoryCreated = true;
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

        var coreContext = HttpContext.RequestServices.GetRequiredService<ArvidsonFotoCoreDbContext>();
        var allImages = coreContext.TblImages.OrderByDescending(i => i.ImageId).ToList();

        UploadEditImagesViewModel viewModel = new UploadEditImagesViewModel()
        {
            CurrentPage = (int)sida,
            CurrentUrl = "./UploadAdmin/RedigeraBilder"
        };
        
        viewModel.TotalPages = (int)Math.Ceiling(allImages.Count() / (decimal)imagesPerPage);
        var displayTblImages = allImages
                                    .Skip((viewModel.CurrentPage - 1) * imagesPerPage)
                                    .Take(imagesPerPage)
                                    .ToList();
        viewModel.DisplayImagesList = new List<UploadImageInputDto>();

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

            UploadImageInputDto inputModel = UploadImageInputDto.CreateEmpty();
            inputModel.ImageId = item.ImageId ?? -1;
            inputModel.ImageHuvudfamilj = item.ImageMainFamilyId;
            inputModel.ImageHuvudfamiljNamn = _categoryService.GetNameById(item.ImageMainFamilyId);
            inputModel.ImageFamilj = item.ImageFamilyId;
            inputModel.ImageFamiljNamn = _categoryService.GetNameById(item.ImageFamilyId);
            inputModel.ImageArt = item.ImageCategoryId ?? -1;
            inputModel.ImageArtNamn = _categoryService.GetNameById(item.ImageCategoryId);
            inputModel.ImageDate = imgDate;
            inputModel.ImageUpdate = item.ImageUpdate ?? DateTime.Now;
            inputModel.ImageDescription = item.ImageDescription ?? "Saknas";
            inputModel.ImageUrl = item.ImageUrlName ?? "Saknas";

            // Get category path using the service method
            var categoryPath = _categoryService.GetCategoryPathForImage(inputModel.ImageArt);
            
            // Build the full source URL with the correct path
            inputModel.ImageUrlFullSrc = $"https://arvidsonfoto.se/bilder/{categoryPath}/{inputModel.ImageUrl}";

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

    /// <summary>
    /// Visar sidan för att välja bilder och skapa Facebook-inlägg
    /// </summary>
    public IActionResult FacebookUpload(string? DisplayMessage, string? FacebookPostUrl)
    {
        ViewData["Title"] = "Dela bilder på Facebook";

        var coreContext = HttpContext.RequestServices.GetRequiredService<ArvidsonFotoCoreDbContext>();
        var recentImages = coreContext.TblImages
            .OrderByDescending(i => i.ImageId)
            .Take(60)
            .ToList();

        var viewModel = new FacebookUploadViewModel
        {
            DisplayMessage = DisplayMessage,
            FacebookPostUrl = FacebookPostUrl
        };

        foreach (var item in recentImages)
        {
            DateTime imgDate = item.ImageDate ?? new DateTime(1900, 01, 01);

            var inputModel = UploadImageInputDto.CreateEmpty();
            inputModel.ImageId = item.ImageId ?? -1;
            inputModel.ImageHuvudfamilj = item.ImageMainFamilyId;
            inputModel.ImageHuvudfamiljNamn = _categoryService.GetNameById(item.ImageMainFamilyId);
            inputModel.ImageFamilj = item.ImageFamilyId;
            inputModel.ImageFamiljNamn = _categoryService.GetNameById(item.ImageFamilyId);
            inputModel.ImageArt = item.ImageCategoryId ?? -1;
            inputModel.ImageArtNamn = _categoryService.GetNameById(item.ImageCategoryId);
            inputModel.ImageDate = imgDate;
            inputModel.ImageUpdate = item.ImageUpdate ?? DateTime.Now;
            inputModel.ImageDescription = item.ImageDescription ?? string.Empty;
            inputModel.ImageUrl = item.ImageUrlName ?? string.Empty;

            inputModel.ImageUrlFullSrc = "https://arvidsonfoto.se/Bilder";
            if (inputModel.ImageHuvudfamilj is not null)
                inputModel.ImageUrlFullSrc += "/" + inputModel.ImageHuvudfamiljNamn;
            if (inputModel.ImageFamilj is not null)
                inputModel.ImageUrlFullSrc += "/" + inputModel.ImageFamiljNamn;

            inputModel.ImageUrlFullSrc += "/" + inputModel.ImageArtNamn + "/" + inputModel.ImageUrl;

            viewModel.RecentImages.Add(inputModel);
        }

        return View(viewModel);
    }

    /// <summary>
    /// Skapar Facebook-inlägg med valda bilder
    /// </summary>
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateFacebookPost(UploadFacebookInputDto model)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("FacebookUpload", new { DisplayMessage = "ValidationError" });
        }

        if (!_facebookService.IsConfigured())
        {
            return RedirectToAction("FacebookUpload", new { DisplayMessage = "NotConfigured" });
        }

        try
        {
            var coreContext = HttpContext.RequestServices.GetRequiredService<ArvidsonFotoCoreDbContext>();
            var selectedImages = coreContext.TblImages
                .Where(img => model.SelectedImageIds.Contains(img.ImageId ?? 0))
                .Take(10)
                .ToList();

            if (selectedImages.Count == 0)
            {
                return RedirectToAction("FacebookUpload", new { DisplayMessage = "NoImagesSelected" });
            }

            var imageUrls = new List<string>();
            foreach (var image in selectedImages)
            {
                string imageUrlFullSrc = "https://arvidsonfoto.se/Bilder";
                
                if (image.ImageMainFamilyId is not null)
                {
                    var huvudfamiljNamn = _categoryService.GetNameById(image.ImageMainFamilyId);
                    imageUrlFullSrc += "/" + huvudfamiljNamn;
                }
                
                if (image.ImageFamilyId is not null)
                {
                    var familjNamn = _categoryService.GetNameById(image.ImageFamilyId);
                    imageUrlFullSrc += "/" + familjNamn;
                }
                
                var artNamn = _categoryService.GetNameById(image.ImageCategoryId);
                imageUrlFullSrc += "/" + artNamn + "/" + image.ImageUrlName;
                
                imageUrls.Add(imageUrlFullSrc);
            }

            var postUrl = await _facebookService.CreatePostAsync(imageUrls, model.Message);

            if (!string.IsNullOrEmpty(postUrl))
            {
                return RedirectToAction("FacebookUpload", new { DisplayMessage = "Success", FacebookPostUrl = postUrl });
            }
            else
            {
                return RedirectToAction("FacebookUpload", new { DisplayMessage = "FacebookError" });
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fel vid skapande av Facebook-inlägg från UploadAdmin");
            return RedirectToAction("FacebookUpload", new { DisplayMessage = "Error" });
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}