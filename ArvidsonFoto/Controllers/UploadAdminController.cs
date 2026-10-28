using ArvidsonFoto.Areas.Identity.Data;
using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.DTOs;
using ArvidsonFoto.Core.Extensions;
using ArvidsonFoto.Core.Interfaces;
using ArvidsonFoto.Core.Services;
using ArvidsonFoto.Core.ViewModels;
using ArvidsonFoto.Views.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;

namespace ArvidsonFoto.Controllers;

[Authorize]
public class UploadAdminController(
    ArvidsonFotoCoreDbContext coreContext,
    UserManager<ArvidsonFotoUser> userManager,
    IFacebookService facebookService,
    ILogger<ApiImageService> imageLogger,
    ILogger<ApiCategoryService> categoryLogger,
    IConfiguration configuration,
    IMemoryCache memoryCache,
    IStringLocalizer<UploadAdminResource> localizer,
    IPageCounterService pageCounterService) : Controller
{
    internal IApiImageService _imageService = new ApiImageService(imageLogger, coreContext, configuration, new ApiCategoryService(categoryLogger, coreContext, memoryCache));
    internal IApiCategoryService _categoryService = new ApiCategoryService(categoryLogger, coreContext, memoryCache);
    internal IGuestBookService _guestBookService = new GuestBookService(coreContext);
    internal readonly UserManager<ArvidsonFotoUser> _userManager = userManager;
    internal readonly IFacebookService _facebookService = facebookService;

    public IActionResult Index()
    {
        ViewData["Title"] = localizer["UploadAdmin.Controller.UploadAdmin"];
        return View();
    }

    [Route("/[controller]/NyBild")]
    [Route("/[controller]/NyBild/{subLevel1}")]
    [Route("/[controller]/NyBild/{subLevel1}/{subLevel2}")]
    [Route("/[controller]/NyBild/{subLevel1}/{subLevel2}/{subLevel3}")]
    [Route("/[controller]/NyBild/{subLevel1}/{subLevel2}/{subLevel3}/{subLevel4}")]
    public IActionResult NyBild(string subLevel1, string? subLevel2, string? subLevel3, string? subLevel4)
    {
        ViewData["Title"] = localizer["Admin.ImageTitle"];

        // Rensa ModelState för GET-requests så att validering inte körs automatiskt
        ModelState.Clear();

        UploadImageViewModel viewModel = new()
        {
            ImageInputModel = UploadImageInputDto.CreateEmpty()
        };

        var subCategories = new List<CategoryDto>();
        var categoryPathSegments = new[] { subLevel1, subLevel2, subLevel3, subLevel4 }
            .Where(segment => !string.IsNullOrWhiteSpace(segment))
            .ToArray();

        if (categoryPathSegments.Length > 0)
        {
            var selectedCategory = LocalizedRoutes.ResolveCategoryPath(categoryPathSegments, _categoryService.GetAll());
            if (selectedCategory?.CategoryId is not int selectedCategoryId)
            {
                return NotFound();
            }

            viewModel.SelectedCategory = selectedCategory;
            subCategories = _categoryService.GetSubsList(selectedCategoryId);

            var latestImage = _imageService.GetOneImageFromCategory(
                selectedCategoryId,
                LocalizedText.Select(selectedCategory.NameSv, selectedCategory.NameEn),
                selectedCategory.NameEn ?? string.Empty);
            viewModel.LatestCategoryImageUrl = BuildThumbnailUrl(latestImage);
        }
        else
        {
            subCategories = _categoryService.GetSubsList(0);
        }

        var currentPath = Request.PathBase.Add(Request.Path).Value;
        viewModel.CurrentUrl = string.IsNullOrEmpty(currentPath) ? "/UploadAdmin/NyBild" : currentPath.TrimEnd('/');
        viewModel.SubCategories = [.. subCategories.OrderBy(c => LocalizedText.Select(c.NameSv, c.NameEn))];

        return View(viewModel);
    }

    internal static string? BuildThumbnailUrl(ImageDto image)
    {
        if (image.ImageId <= 0 || string.IsNullOrWhiteSpace(image.UrlImage))
        {
            return null;
        }

        var imagePathSegments = image.UrlImage
            .Split('/')
            .Select(Uri.UnescapeDataString)
            .ToArray();
        if (imagePathSegments.Length < 2 ||
            !string.Equals(imagePathSegments[0], "bilder", StringComparison.OrdinalIgnoreCase) ||
            imagePathSegments.Any(segment =>
                string.IsNullOrWhiteSpace(segment) ||
                segment is "." or ".." ||
                segment.Contains('/') ||
                segment.Contains('\\')))
        {
            return null;
        }

        return "/" +
            string.Join("/", imagePathSegments.Select(Uri.EscapeDataString)) + ".thumb.jpg";
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult CreateImageLink(UploadImageInputDto model)
    {
        if (ModelState.IsValid)
        {
            if (model.ImageHuvudfamilj.Equals(0))
            {
                model.ImageHuvudfamilj = null;
            }

            if (model.ImageFamilj.Equals(0))
            {
                model.ImageFamilj = null;
            }

            if (model.ImageHuvudfamilj.Equals(1))
            {
                model.ImageHuvudfamilj = null;
            }

            Core.Models.TblImage newImage = new()
            {
                ImageId = _imageService.GetImageLastId() + 1,
                ImageMainFamilyId = model.ImageHuvudfamilj,
                ImageFamilyId = model.ImageFamilj,
                ImageCategoryId = model.ImageArt,
                ImageUpdate = DateTime.Now,
                ImageDate = model.ImageDate,
                ImageDescription = model.ImageDescription,
                ImageDescriptionEn = model.ImageDescriptionEn,
                ImageUrlName = model.ImageUrl,
                IsGameSelectable = model.IsGameSelectable
            };

            try
            {
                var coreContext = HttpContext.RequestServices.GetRequiredService<ArvidsonFotoCoreDbContext>();
                _ = coreContext.TblImages.Add(newImage);
                _ = coreContext.SaveChanges();

                // Använd TempData för att visa framgångsmeddelande
                TempData["ImageCreated"] = true;
                TempData["ImageCategoryId"] = model.ImageArt;
                TempData["ImageUrl"] = model.ImageUrl;

                return RedirectToAction("NyBild");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Fel vid skapande av bild");
                TempData["ImageCreated"] = false;
                TempData["ErrorMessage"] = localizer["Admin.ImageCreationError"].Value;
            }
        }

        // Om validering misslyckades, skicka tillbaka till formuläret med felmeddelanden
        TempData["ImageCreated"] = false;
        return RedirectToAction("NyBild");
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
                    existingImage.ImageDescriptionEn = model.ImageDescriptionEn;
                    existingImage.IsGameSelectable = model.IsGameSelectable;
                    existingImage.ImageUpdate = DateTime.Now;

                    _ = await coreContext.SaveChangesAsync();
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
        ViewData["Title"] = localizer["Admin.CategoryTitle"];
        return View(inputModel ?? UploadNewCategoryInputDto.CreateEmpty());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult CreateCategory(UploadNewCategoryInputDto inputModel)
    {
        inputModel.CategoryCreated = false;

        if (ModelState.IsValid)
        {
            var svSegment = string.IsNullOrWhiteSpace(inputModel.MenuUrlSegment)
                ? SharedStaticFunctions.ToUrlSegment(inputModel.MenuText)
                : inputModel.MenuUrlSegment.Trim().ToLowerInvariant();
            var enSegment = string.IsNullOrWhiteSpace(inputModel.MenuUrlSegmentEn) ? null : inputModel.MenuUrlSegmentEn.Trim().ToLowerInvariant();
            if (!ValidCategorySegments(svSegment, enSegment, null))
            {
                ModelState.AddModelError(nameof(inputModel.MenuUrlSegment), localizer["Admin.InvalidUrlSegments"]);
                return View("NyKategori", inputModel);
            }
            CategoryDto newCategory = new()
            {
                NameSv = inputModel.MenuText,
                NameEn = inputModel.MenuTextEn,
                CategoryId = _categoryService.GetLastId() + 1,
                ParentCategoryId = inputModel.MainMenuId,
                UrlCategoryPathSv = svSegment,
                UrlCategoryPathEn = enSegment,
                DateUpdated = DateTime.UtcNow
            };

            if (_categoryService.AddCategory(newCategory))
            {
                _categoryService.ClearCache();
                inputModel.CategoryCreated = true;
                inputModel.MainMenuId = null;
            }
        }

        return View("NyKategori", inputModel);
    }

    public IActionResult RedigeraKategori(int? id, string? search)
    {
        ViewData["Title"] = localizer["Admin.CategoriesTitle"];
        PopulateCategoryEditor(search);
        var selected = id.HasValue ? coreContext.TblMenus.FirstOrDefault(c => c.MenuCategoryId == id) : null;
        return View(new EditCategoryInputDto
        {
            CategoryId = selected?.MenuCategoryId ?? 0,
            NameSv = selected?.MenuDisplayName ?? string.Empty,
            NameEn = selected?.MenuDisplayNameEn,
            UrlSegmentSv = selected?.MenuUrlSegment ?? string.Empty,
            UrlSegmentEn = selected?.MenuUrlSegmentEn,
            IsGameSelectable = selected?.IsGameSelectable ?? true
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult SaveCategory(EditCategoryInputDto input, string? search)
    {
        ViewData["Title"] = localizer["Admin.CategoriesTitle"];
        var category = coreContext.TblMenus.FirstOrDefault(c => c.MenuCategoryId == input.CategoryId);
        if (category == null)
        {
            return NotFound();
        }

        if (!ValidCategorySegments(input.UrlSegmentSv, input.UrlSegmentEn, input.CategoryId))
        {
            ModelState.AddModelError(nameof(input.UrlSegmentSv), localizer["Admin.InvalidUrlSegments"]);
        }

        if (!ModelState.IsValid)
        {
            PopulateCategoryEditor(search);
            return View("RedigeraKategori", input);
        }
        category.MenuDisplayName = input.NameSv.Trim();
        category.MenuDisplayNameEn = string.IsNullOrWhiteSpace(input.NameEn) ? null : input.NameEn.Trim();
        category.MenuUrlSegment = input.UrlSegmentSv.Trim().ToLowerInvariant();
        category.MenuUrlSegmentEn = string.IsNullOrWhiteSpace(input.UrlSegmentEn) ? null : input.UrlSegmentEn.Trim().ToLowerInvariant();
        category.IsGameSelectable = input.IsGameSelectable;
        category.MenuDateUpdated = DateTime.UtcNow;
        _ = coreContext.SaveChanges();
        _categoryService.ClearCache();
        return RedirectToAction(nameof(RedigeraKategori), new { id = input.CategoryId, search });
    }

    private void PopulateCategoryEditor(string? search)
    {
        var categories = coreContext.TblMenus.AsNoTracking().ToList();
        var childrenByParent = categories.ToLookup(category => category.MenuParentCategoryId ?? 0);
        var orderedCategories = new List<(Core.Models.TblMenu Category, int Level)>();
        var visited = new HashSet<int>();

        void AddChildren(int parentId, int level)
        {
            foreach (var child in childrenByParent[parentId]
                .OrderBy(category => LocalizedText.Select(category.MenuDisplayName, category.MenuDisplayNameEn)))
            {
                if (child.MenuCategoryId is not int categoryId || !visited.Add(categoryId))
                {
                    continue;
                }

                orderedCategories.Add((child, level));
                AddChildren(categoryId, level + 1);
            }
        }

        AddChildren(0, 0);
        foreach (var category in categories.OrderBy(item => LocalizedText.Select(item.MenuDisplayName, item.MenuDisplayNameEn)))
        {
            if (category.MenuCategoryId is int categoryId && visited.Add(categoryId))
            {
                orderedCategories.Add((category, 0));
                AddChildren(categoryId, 1);
            }
        }

        ViewData["Categories"] = orderedCategories;
        ViewData["Search"] = search;
    }

    private bool ValidCategorySegments(string? swedish, string? english, int? excludeId)
    {
        return string.IsNullOrWhiteSpace(swedish) || swedish.Length > 50 ||
            !System.Text.RegularExpressions.Regex.IsMatch(swedish, @"^[a-zA-Z0-9-]+$") ||
            (!string.IsNullOrWhiteSpace(english) &&
                (english.Length > 50 || !System.Text.RegularExpressions.Regex.IsMatch(english, @"^[a-zA-Z0-9-]+$"))) ||
            (!string.IsNullOrWhiteSpace(english) && string.Equals(swedish, english, StringComparison.OrdinalIgnoreCase))
            ? false
            : !coreContext.TblMenus.AsEnumerable().Any(c => c.MenuCategoryId != excludeId &&
            new[] { c.MenuUrlSegment, c.MenuUrlSegmentEn }.Any(existing =>
                string.Equals(existing, swedish, StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrWhiteSpace(english) && string.Equals(existing, english, StringComparison.OrdinalIgnoreCase))));
    }

    private string GetOptionalFamilyName(int? categoryId)
    {
        if (categoryId is null or <= 0)
        {
            return string.Empty;
        }

        var name = _categoryService.GetNameById(categoryId);
        return string.IsNullOrWhiteSpace(name) ? string.Empty : name;
    }

    public IActionResult RedigeraBilder(string DisplayMessage, string imgId, int? sida)
    {
        ViewData["Title"] = localizer["Admin.EditImagesTitle"];

        int imagesPerPage = 25;
        if (sida is null or < 1)
        {
            sida = 1;
        }

        var coreContext = HttpContext.RequestServices.GetRequiredService<ArvidsonFotoCoreDbContext>();
        var allImages = coreContext.TblImages.OrderByDescending(i => i.ImageId).ToList();

        UploadEditImagesViewModel viewModel = new()
        {
            CurrentPage = (int)sida,
            CurrentUrl = "./UploadAdmin/RedigeraBilder",
            TotalPages = (int)Math.Ceiling(allImages.Count / (decimal)imagesPerPage),
            DisplayImagesList = []
        };

        var displayTblImages = allImages
                                    .Skip((viewModel.CurrentPage - 1) * imagesPerPage)
                                    .Take(imagesPerPage)
                                    .ToList();

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
            inputModel.ImageHuvudfamiljNamn = GetOptionalFamilyName(item.ImageMainFamilyId);
            inputModel.ImageFamilj = item.ImageFamilyId;
            inputModel.ImageFamiljNamn = GetOptionalFamilyName(item.ImageFamilyId);
            inputModel.ImageArt = item.ImageCategoryId ?? -1;
            inputModel.ImageArtNamn = _categoryService.GetNameById(item.ImageCategoryId);
            inputModel.ImageDate = imgDate;
            inputModel.ImageUpdate = item.ImageUpdate ?? DateTime.Now;
            inputModel.ImageDescription = IsMissingDescription(item.ImageDescription, "saknas")
                ? string.Empty
                : item.ImageDescription ?? string.Empty;
            inputModel.ImageDescriptionEn = IsMissingDescription(item.ImageDescriptionEn, "Missing")
                ? string.Empty
                : item.ImageDescriptionEn;
            inputModel.IsGameSelectable = item.IsGameSelectable;
            inputModel.ImageUrl = item.ImageUrlName ?? localizer["Admin.Missing"];

            var categoryPath = _categoryService.GetCategoryDisplayPathForImage(inputModel.ImageArt);
            inputModel.ImageUrlFullSrc = $"https://arvidsonfoto.se/{ImageMappingExtensions.ToPhysicalImageUrl(categoryPath, inputModel.ImageUrl)}";

            viewModel.DisplayImagesList.Add(inputModel);
        }

        return View(viewModel);
    }

    private static bool IsMissingDescription(string? description, string placeholder) =>
        string.Equals(description?.Trim(), placeholder, StringComparison.OrdinalIgnoreCase);

    public IActionResult HanteraGB(string DisplayMessage, string gbId)
    {
        ViewData["Title"] = localizer["Admin.GuestbookTitle"];
        UploadGbViewModel viewModel = new();
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
                return RedirectToAction("HanteraGB", new { DisplayMessage = "OkGbRead", gbId });
            }
        }
        return RedirectToAction("HanteraGB", new { DisplayMessage = "ErrorGbRead", gbId });
    }

    public IActionResult DeleteGbPost(int gbId)
    {
        if (User?.Identity?.IsAuthenticated is true)
        {
            if (_guestBookService.DeleteGbPost(gbId))
            {
                return RedirectToAction("HanteraGB", new { DisplayMessage = "OkGbDelete", gbId });
            }
        }
        return RedirectToAction("HanteraGB", new { DisplayMessage = "ErrorGbDelete", gbId });
    }

    public IActionResult DeleteImage(int imgId)
    {
        if (User?.Identity?.IsAuthenticated is true)
        {
            if (_imageService.DeleteImgId(imgId))
            {
                return RedirectToAction("RedigeraBilder", new { DisplayMessage = "OkImgDelete", imgId });
            }
        }
        return RedirectToAction("RedigeraBilder", new { DisplayMessage = "ErrorImgDelete", imgId });
    }

    public IActionResult Statistik(string? period, string? fromMonth, string? toMonth, bool compare = false)
    {
        ViewData["Title"] = localizer["Admin.StatisticsTitle"];
        var currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        period = period?.ToLowerInvariant();

        DateTime startDate;
        DateTime endDate = currentMonth;
        switch (period)
        {
            case "12m":
                startDate = currentMonth.AddMonths(-11);
                break;
            case "year":
                startDate = new DateTime(currentMonth.Year, 1, 1);
                break;
            case "custom" when DateTime.TryParseExact(fromMonth, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var from)
                              && DateTime.TryParseExact(toMonth, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var to)
                              && from <= to
                              && from <= currentMonth:
                startDate = new DateTime(from.Year, from.Month, 1);
                endDate = new DateTime(to.Year, to.Month, 1) > currentMonth
                    ? currentMonth
                    : new DateTime(to.Year, to.Month, 1);
                if (startDate > endDate)
                {
                    period = "6m";
                    startDate = currentMonth.AddMonths(-5);
                    endDate = currentMonth;
                }
                break;
            default:
                period = "6m";
                startDate = currentMonth.AddMonths(-5);
                break;
        }

        if (((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month >= 60)
        {
            startDate = endDate.AddMonths(-59);
        }

        var viewModel = new StatisticsViewModel
        {
            Period = period ?? "6m",
            StartDate = startDate,
            EndDate = endDate,
            CompareWithPreviousYear = compare,
            MonthlyPageViews = pageCounterService.GetMonthlyPageViewsChart(startDate, endDate, false),
            MonthlyCategoryViews = pageCounterService.GetMonthlyPageViewsChart(startDate, endDate, true),
            PageViews = pageCounterService.GetPageCountsInPeriod(startDate, endDate, false),
            CategoryViews = pageCounterService.GetPageCountsInPeriod(startDate, endDate, true),
            AllTimePageViews = pageCounterService.GetAllPageCountsGroupedByPageCount()
        };

        if (compare)
        {
            viewModel.PreviousYearPageViews = pageCounterService.GetMonthlyPageViewsChart(startDate.AddYears(-1), endDate.AddYears(-1), false);
            viewModel.PreviousYearCategoryViews = pageCounterService.GetMonthlyPageViewsChart(startDate.AddYears(-1), endDate.AddYears(-1), true);
        }

        return View(viewModel);
    }

    /// <summary>Laddar sidan "Läs loggboken" , men heter visa för att slippa ä i Läs</summary>
    /// <param name="datum">Format: ÅÅÅÅMMDD (t.ex: 20210126)</param>
    public async Task<IActionResult> VisaLoggbokenAsync(DateTime datum)
    {
        ViewData["Title"] = localizer["Admin.LogTitle", datum];

        AppLogReaderService logReader = new();
        string appLogFile = $"appLog{datum:yyyyMMdd}.txt";

        UploadLogReaderViewModel viewModel = new()
        {
            ExistingLogFiles = logReader.ExistingLogFiles(),
            LogBook = logReader.ReadData(appLogFile),
            DateShown = datum
        };

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
        {
            date = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
        }

        ArvidsonFotoUser user = await _userManager.GetUserAsync(User) ?? new();
        user.ShowAllLogs = !user.ShowAllLogs;

        _ = await _userManager.UpdateAsync(user);
        return RedirectToAction("VisaLoggboken", new { datum = date });
    }

    /// <summary>
    /// Visar sidan för att välja bilder och skapa Facebook-inlägg
    /// </summary>
    public IActionResult FacebookUpload(string? DisplayMessage, string? FacebookPostUrl)
    {
        ViewData["Title"] = localizer["Admin.FacebookTitle"];

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
            inputModel.ImageHuvudfamiljNamn = GetOptionalFamilyName(item.ImageMainFamilyId);
            inputModel.ImageFamilj = item.ImageFamilyId;
            inputModel.ImageFamiljNamn = GetOptionalFamilyName(item.ImageFamilyId);
            inputModel.ImageArt = item.ImageCategoryId ?? -1;
            inputModel.ImageArtNamn = _categoryService.GetNameById(item.ImageCategoryId);
            inputModel.ImageDate = imgDate;
            inputModel.ImageUpdate = item.ImageUpdate ?? DateTime.Now;
            inputModel.ImageDescription = item.ImageDescription ?? string.Empty;
            inputModel.ImageDescriptionEn = item.ImageDescriptionEn;
            inputModel.ImageUrl = item.ImageUrlName ?? string.Empty;

            inputModel.ImageUrlFullSrc = "https://arvidsonfoto.se/bilder";
            if (inputModel.ImageHuvudfamilj is not null)
            {
                inputModel.ImageUrlFullSrc += "/" + inputModel.ImageHuvudfamiljNamn;
            }

            if (inputModel.ImageFamilj is not null)
            {
                inputModel.ImageUrlFullSrc += "/" + inputModel.ImageFamiljNamn;
            }

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
                string imageUrlFullSrc = "https://arvidsonfoto.se/bilder";

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