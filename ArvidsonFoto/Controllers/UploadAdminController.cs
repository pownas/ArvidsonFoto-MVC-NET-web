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
using System.Net;
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
    IPageCounterService pageCounterService,
    IHttpClientFactory? httpClientFactory = null) : Controller
{
    internal IApiImageService _imageService = new ApiImageService(imageLogger, coreContext, configuration, new ApiCategoryService(categoryLogger, coreContext, memoryCache));
    internal IApiCategoryService _categoryService = new ApiCategoryService(categoryLogger, coreContext, memoryCache);
    internal IGuestBookService _guestBookService = new GuestBookService(coreContext);
    internal readonly UserManager<ArvidsonFotoUser> _userManager = userManager;
    internal readonly IFacebookService _facebookService = facebookService;

    public IActionResult Index()
    {
        ViewData["Title"] = localizer["UploadAdmin.Controller.UploadAdmin"];
        var currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var summary = BuildAdminActionSummary();
        StatisticsViewModel statistics;
        try
        {
            statistics = BuildStatisticsViewModel("6m", currentMonth.AddMonths(-5), currentMonth, false);
        }
        catch
        {
            statistics = new StatisticsViewModel();
            summary.StatisticsUnavailable = true;
        }

        ViewData["AdminActionSummary"] = summary;
        return View(statistics);
    }

    private AdminActionSummaryViewModel BuildAdminActionSummary()
    {
        var summary = new AdminActionSummaryViewModel();

        void Add(string titleKey, string linkKey, string url, Func<int> count)
        {
            try
            {
                var value = count();
                if (value > 0)
                {
                    summary.Items.Add(new AdminActionItem(titleKey, linkKey, value, url));
                }
            }
            catch
            {
                summary.DataUnavailable = true;
            }
        }

        Add("UploadAdmin.Dashboard.UnreadGuestbook", "UploadAdmin.Dashboard.Open", "/UploadAdmin/HanteraGB?status=unread",
            () => coreContext.TblGbs.Count(post => post.GbReadPost != true));
        Add("UploadAdmin.Dashboard.MissingImageTranslations", "UploadAdmin.Dashboard.Open",
            "/TranslationReview?table=images&pendingOnly=true&missingOnly=true",
            () => coreContext.TblImages.Count(image =>
                image.ImageDescription != null && image.ImageDescription.Trim() != "" &&
                (image.ImageDescriptionEn == null || image.ImageDescriptionEn.Trim() == "")));
        Add("UploadAdmin.Dashboard.PendingCategoryTranslations", "UploadAdmin.Dashboard.Open",
            "/TranslationReview?table=menu&pendingOnly=true",
            () => coreContext.TblMenus.Count(menu => menu.MenuCategoryId != null && !menu.MenuDisplayNameEnReviewed) +
                coreContext.TblMenus.Count(menu => menu.MenuCategoryId != null && !menu.MenuUrlSegmentEnReviewed));

        try
        {
            var logReader = new AppLogReaderService();
            var latestLogDate = logReader.ExistingLogFiles()
                .Select(value => DateTime.TryParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) ? date : (DateTime?)null)
                .Where(date => date.HasValue)
                .Max();
            if (latestLogDate.HasValue)
            {
                var errors = logReader.ReadData($"appLog{latestLogDate.Value:yyyyMMdd}.txt")
                    .Count(line => line.Contains("[ERR]", StringComparison.Ordinal));
                if (errors > 0)
                {
                    summary.Items.Add(new AdminActionItem(
                        "UploadAdmin.Dashboard.RecentErrors",
                        "UploadAdmin.Dashboard.Open",
                        errors,
                        $"/UploadAdmin/VisaLoggboken?datum={latestLogDate.Value:yyyy-MM-dd}&level=error"));
                }
            }
        }
        catch
        {
            summary.DataUnavailable = true;
        }

        return summary;
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
            var categoryPath = _categoryService.GetCategoryDisplayPathForImage(selectedCategoryId);
            viewModel.ImagePreviewBaseUrl = $"https://arvidsonfoto.se/{ImageMappingExtensions.ToPhysicalImageUrl(categoryPath, string.Empty).TrimEnd('/')}";

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

    internal static string? BuildFacebookImageUrl(string categoryPath, string? imageFileName)
    {
        if (string.IsNullOrWhiteSpace(imageFileName) ||
            imageFileName is "." or ".." ||
            imageFileName.Contains('/') ||
            imageFileName.Contains('\\'))
        {
            return null;
        }

        return $"https://arvidsonfoto.se/{ImageMappingExtensions.ToPhysicalImageUrl(categoryPath, imageFileName)}";
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateImageLink(UploadImageInputDto model)
    {
        if (ModelState.IsValid)
        {
            if (!await IsPublishedImageAsync(model))
            {
                TempData["ImageCreated"] = false;
                TempData["ErrorMessage"] = localizer["UploadAdmin.NyBild.ImageNotPublished"].Value;
                return RedirectToImageCategory(model.ImageArt);
            }

            var selectedCategory = _categoryService.GetById(model.ImageArt)!;
            model.ImageFamilj = selectedCategory.ParentCategoryId;
            model.ImageHuvudfamilj = selectedCategory.ParentCategoryId is int parentId
                ? _categoryService.GetById(parentId)?.ParentCategoryId
                : null;

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

                return RedirectToImageCategory(model.ImageArt);
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
        return RedirectToImageCategory(model.ImageArt);
    }

    private async Task<bool> IsPublishedImageAsync(UploadImageInputDto model)
    {
        var imageName = model.ImageUrl?.Trim();
        if (string.IsNullOrWhiteSpace(imageName) ||
            imageName.Length > 120 ||
            imageName is "." or ".." ||
            imageName.Contains('/') ||
            imageName.Contains('\\') ||
            imageName.Any(char.IsControl) ||
            model.ImageArt <= 0 ||
            httpClientFactory is null)
        {
            return false;
        }

        var selectedCategory = _categoryService.GetById(model.ImageArt);
        if (selectedCategory?.CategoryId != model.ImageArt || _categoryService.GetSubsList(model.ImageArt).Count > 0)
        {
            return false;
        }

        var categoryPath = _categoryService.GetCategoryDisplayPathForImage(model.ImageArt);
        if (string.IsNullOrWhiteSpace(categoryPath))
        {
            return false;
        }

        var path = ImageMappingExtensions.ToPhysicalImageUrl(categoryPath, imageName);
        var client = httpClientFactory.CreateClient("ImagePublication");
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        try
        {
            foreach (var suffix in new[] { ".jpg", ".thumb.jpg" })
            {
                using var request = new HttpRequestMessage(HttpMethod.Head, new Uri($"https://arvidsonfoto.se/{path}{suffix}"));
                using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, timeout.Token);
                if (response.StatusCode is HttpStatusCode.MethodNotAllowed or HttpStatusCode.NotImplemented)
                {
                    using var getRequest = new HttpRequestMessage(HttpMethod.Get, request.RequestUri);
                    using var getResponse = await client.SendAsync(getRequest, HttpCompletionOption.ResponseHeadersRead, timeout.Token);
                    if (!getResponse.IsSuccessStatusCode)
                    {
                        return false;
                    }
                }
                else if (!response.IsSuccessStatusCode)
                {
                    return false;
                }
            }
        }
        catch (HttpRequestException)
        {
            return false;
        }
        catch (OperationCanceledException)
        {
            return false;
        }

        return true;
    }

    private RedirectToActionResult RedirectToImageCategory(int categoryId)
    {
        var category = _categoryService.GetById(categoryId);
        if (category is null)
        {
            return RedirectToAction(nameof(NyBild));
        }

        var path = CultureInfo.CurrentUICulture.Name.StartsWith("en", StringComparison.OrdinalIgnoreCase)
            ? string.IsNullOrWhiteSpace(category.UrlCategoryPathFullEn) ? category.UrlCategoryPathFullSv : category.UrlCategoryPathFullEn
            : category.UrlCategoryPathFullSv;
        var segments = path?.Split('/', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
        return RedirectToAction(nameof(NyBild), new
        {
            subLevel1 = segments.ElementAtOrDefault(0),
            subLevel2 = segments.ElementAtOrDefault(1),
            subLevel3 = segments.ElementAtOrDefault(2),
            subLevel4 = segments.ElementAtOrDefault(3)
        });
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
                    return RedirectToAction(nameof(RedigeraBilder), new
                    {
                        DisplayMessage = "OkImgEdit",
                        imgId = model.ImageId,
                        sida = model.ReturnPage,
                        search = model.ReturnSearch,
                        categoryId = model.ReturnCategoryId,
                        dateFrom = model.ReturnDateFrom,
                        dateTo = model.ReturnDateTo,
                        missingSwedishDescription = model.ReturnMissingSwedishDescription,
                        missingEnglishDescription = model.ReturnMissingEnglishDescription
                    });
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
            var validationMessage = localizer["Admin.InvalidUrlSegments"];
            ModelState.AddModelError(nameof(input.UrlSegmentSv), validationMessage);
            if (!string.IsNullOrWhiteSpace(input.UrlSegmentEn))
            {
                ModelState.AddModelError(nameof(input.UrlSegmentEn), validationMessage);
            }
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

    public IActionResult RedigeraBilder(
        string DisplayMessage,
        string imgId,
        int? sida,
        string? search,
        int? categoryId,
        DateTime? dateFrom,
        DateTime? dateTo,
        bool missingSwedishDescription = false,
        bool missingEnglishDescription = false)
    {
        ViewData["Title"] = localizer["Admin.EditImagesTitle"];

        int imagesPerPage = 25;
        if (sida is null or < 1)
        {
            sida = 1;
        }

        search = search?.Trim();
        if (search?.Length > 100)
        {
            return BadRequest();
        }

        var imageQuery = coreContext.TblImages.AsNoTracking();
        if (!string.IsNullOrEmpty(search))
        {
            imageQuery = imageQuery.Where(image =>
                (image.ImageUrlName ?? "").Contains(search) ||
                (image.ImageDescription ?? "").Contains(search) ||
                (image.ImageDescriptionEn ?? "").Contains(search));
        }

        if (categoryId is > 0)
        {
            imageQuery = imageQuery.Where(image => image.ImageCategoryId == categoryId);
        }

        if (dateFrom.HasValue)
        {
            imageQuery = imageQuery.Where(image => image.ImageDate >= dateFrom.Value.Date);
        }

        if (dateTo.HasValue)
        {
            imageQuery = dateTo.Value.Date == DateTime.MaxValue.Date
                ? imageQuery.Where(image => image.ImageDate <= dateTo.Value.Date)
                : imageQuery.Where(image => image.ImageDate < dateTo.Value.Date.AddDays(1));
        }

        if (missingSwedishDescription)
        {
            imageQuery = imageQuery.Where(image => image.ImageDescription == null ||
                image.ImageDescription.Trim() == "" ||
                image.ImageDescription.Trim() == "saknas");
        }

        if (missingEnglishDescription)
        {
            imageQuery = imageQuery.Where(image => image.ImageDescriptionEn == null ||
                image.ImageDescriptionEn.Trim() == "" ||
                image.ImageDescriptionEn.Trim() == "missing");
        }

        var resultCount = imageQuery.Count();
        UploadEditImagesViewModel viewModel = new()
        {
            CurrentPage = (int)sida,
            CurrentUrl = "/UploadAdmin/RedigeraBilder",
            TotalPages = Math.Max(1, (int)Math.Ceiling(resultCount / (decimal)imagesPerPage)),
            DisplayImagesList = [],
            SearchQuery = search,
            CategoryId = categoryId,
            DateFrom = dateFrom,
            DateTo = dateTo,
            MissingSwedishDescription = missingSwedishDescription,
            MissingEnglishDescription = missingEnglishDescription,
            ResultCount = resultCount,
            Categories = [.. _categoryService.GetAll().OrderBy(category => LocalizedText.Select(category.NameSv, category.NameEn))]
        };
        viewModel.CurrentPage = Math.Min(viewModel.CurrentPage, viewModel.TotalPages);

        var displayTblImages = imageQuery
                                    .OrderByDescending(i => i.ImageId)
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
            viewModel.UpdatedId = Convert.ToInt32(imgId, CultureInfo.InvariantCulture);
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
            inputModel.ReturnPage = viewModel.CurrentPage;
            inputModel.ReturnSearch = viewModel.SearchQuery;
            inputModel.ReturnCategoryId = viewModel.CategoryId;
            inputModel.ReturnDateFrom = viewModel.DateFrom;
            inputModel.ReturnDateTo = viewModel.DateTo;
            inputModel.ReturnMissingSwedishDescription = viewModel.MissingSwedishDescription;
            inputModel.ReturnMissingEnglishDescription = viewModel.MissingEnglishDescription;

            var categoryPath = _categoryService.GetCategoryDisplayPathForImage(inputModel.ImageArt);
            inputModel.ImageUrlFullSrc = $"https://arvidsonfoto.se/{ImageMappingExtensions.ToPhysicalImageUrl(categoryPath, inputModel.ImageUrl)}";

            viewModel.DisplayImagesList.Add(inputModel);
        }

        return View(viewModel);
    }

    private static bool IsMissingDescription(string? description, string placeholder) =>
        string.Equals(description?.Trim(), placeholder, StringComparison.OrdinalIgnoreCase);

    public IActionResult HanteraGB(string DisplayMessage, string gbId, string status = "all", string sort = "newest")
    {
        ViewData["Title"] = localizer["Admin.GuestbookTitle"];
        status = status is "unread" or "read" ? status : "all";
        sort = sort == "oldest" ? "oldest" : "newest";
        var posts = _guestBookService.GetAll().AsEnumerable();
        if (status == "unread")
        {
            posts = posts.Where(post => post.GbReadPost is not true);
        }
        else if (status == "read")
        {
            posts = posts.Where(post => post.GbReadPost is true);
        }

        UploadGbViewModel viewModel = new()
        {
            StatusFilter = status,
            SortOrder = sort,
            Posts = [.. (sort == "oldest"
                ? posts.OrderBy(post => post.GbDate)
                : posts.OrderByDescending(post => post.GbDate))]
        };
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

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult MarkGbPostAsRead(int gbId, string status = "all", string sort = "newest")
    {
        if (User?.Identity?.IsAuthenticated is true)
        {
            if (_guestBookService.ReadGbPost(gbId))
            {
                return RedirectToAction("HanteraGB", new { DisplayMessage = "OkGbRead", gbId, status, sort });
            }
        }
        return RedirectToAction("HanteraGB", new { DisplayMessage = "ErrorGbRead", gbId, status, sort });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult DeleteGbPost(int gbId, string status = "all", string sort = "newest")
    {
        if (User?.Identity?.IsAuthenticated is true)
        {
            if (_guestBookService.DeleteGbPost(gbId))
            {
                return RedirectToAction("HanteraGB", new { DisplayMessage = "OkGbDelete", gbId, status, sort });
            }
        }
        return RedirectToAction("HanteraGB", new { DisplayMessage = "ErrorGbDelete", gbId, status, sort });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult DeleteImage(
        int imgId,
        int? sida = null,
        string? search = null,
        int? categoryId = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        bool missingSwedishDescription = false,
        bool missingEnglishDescription = false)
    {
        if (User?.Identity?.IsAuthenticated is true)
        {
            if (_imageService.DeleteImgId(imgId))
            {
                return RedirectToAction(nameof(RedigeraBilder), new { DisplayMessage = "OkImgDelete", imgId, sida, search, categoryId, dateFrom, dateTo, missingSwedishDescription, missingEnglishDescription });
            }
        }
        return RedirectToAction(nameof(RedigeraBilder), new { DisplayMessage = "ErrorImgDelete", imgId, sida, search, categoryId, dateFrom, dateTo, missingSwedishDescription, missingEnglishDescription });
    }

    public IActionResult Statistik(string? period, string? fromMonth, string? toMonth, bool compare = false)
    {
        ViewData["Title"] = localizer["Admin.StatisticsTitle"];
        return View(CreateStatisticsViewModel(period, fromMonth, toMonth, compare));
    }

    [HttpGet]
    public IActionResult ExportStatisticsCsv(string? period, string? fromMonth, string? toMonth, bool compare = false)
    {
        var model = CreateStatisticsViewModel(period, fromMonth, toMonth, compare);
        var rows = new List<string>
        {
            CsvRow(localizer["UploadAdmin.Statistics.CsvMetric"].Value, localizer["UploadAdmin.Statistics.CsvName"].Value,
                localizer["UploadAdmin.Statistics.CsvValue"].Value, localizer["UploadAdmin.Statistics.CsvLastVisited"].Value,
                localizer["UploadAdmin.Statistics.CsvPeriodStart"].Value, localizer["UploadAdmin.Statistics.CsvPeriodEnd"].Value),
            CsvRow(localizer["UploadAdmin.Statistics.CsvPeriod"].Value, model.Period, "", "",
                model.StartDate.ToString("yyyy-MM", CultureInfo.InvariantCulture),
                model.EndDate.ToString("yyyy-MM", CultureInfo.InvariantCulture))
        };

        void AddRows(string metric, IEnumerable<Core.Models.TblPageCounter> values, bool allTime = false)
        {
            rows.AddRange(values.Select(value => CsvRow(
                metric, value.PageName ?? "", value.PageViews.ToString(CultureInfo.InvariantCulture),
                value.LastShowDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                allTime ? "" : model.StartDate.ToString("yyyy-MM", CultureInfo.InvariantCulture),
                allTime ? "" : model.EndDate.ToString("yyyy-MM", CultureInfo.InvariantCulture))));
        }

        AddRows(localizer["Admin.Views"].Value, model.PageViews);
        AddRows(localizer["Admin.CategoryViews"].Value, model.CategoryViews.Take(10));
        AddRows(localizer["UploadAdmin.Statistik.AllTimePageViews"].Value, model.AllTimePageViews, allTime: true);
        rows.AddRange(model.MonthlyPageViews.Select(month => CsvRow(
            localizer["UploadAdmin.Statistics.CsvMonthlyPageViews"].Value,
            month.Key, month.Value.ToString(CultureInfo.InvariantCulture), "",
            model.StartDate.ToString("yyyy-MM", CultureInfo.InvariantCulture),
            model.EndDate.ToString("yyyy-MM", CultureInfo.InvariantCulture))));
        rows.AddRange(model.MonthlyCategoryViews.Select(month => CsvRow(
            localizer["UploadAdmin.Statistics.CsvMonthlyCategoryViews"].Value,
            month.Key, month.Value.ToString(CultureInfo.InvariantCulture), "",
            model.StartDate.ToString("yyyy-MM", CultureInfo.InvariantCulture),
            model.EndDate.ToString("yyyy-MM", CultureInfo.InvariantCulture))));
        if (compare)
        {
            rows.AddRange(model.PreviousYearPageViews.Select(month => CsvRow(
                localizer["UploadAdmin.Statistics.PreviousYearPageViews"].Value,
                month.Key, month.Value.ToString(CultureInfo.InvariantCulture), "",
                model.StartDate.ToString("yyyy-MM", CultureInfo.InvariantCulture),
                model.EndDate.ToString("yyyy-MM", CultureInfo.InvariantCulture))));
            rows.AddRange(model.PreviousYearCategoryViews.Select(month => CsvRow(
                localizer["UploadAdmin.Statistics.PreviousYearCategoryViews"].Value,
                month.Key, month.Value.ToString(CultureInfo.InvariantCulture), "",
                model.StartDate.ToString("yyyy-MM", CultureInfo.InvariantCulture),
                model.EndDate.ToString("yyyy-MM", CultureInfo.InvariantCulture))));
        }

        var csv = string.Join("\r\n", rows) + "\r\n";
        var encoding = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
        var content = encoding.GetPreamble().Concat(encoding.GetBytes(csv)).ToArray();
        return File(content,
            "text/csv; charset=utf-8", $"arvidsonfoto-statistik-{model.StartDate:yyyy-MM}-{model.EndDate:yyyy-MM}.csv");
    }

    private static string CsvRow(params string[] values) =>
        string.Join(",", values.Select(value =>
        {
            var firstNonWhitespace = value.FirstOrDefault(character => !char.IsWhiteSpace(character));
            var safeValue = firstNonWhitespace is '=' or '+' or '-' or '@' ? "'" + value : value;
            return $"\"{safeValue.Replace("\"", "\"\"")}\"";
        }));

    private StatisticsViewModel CreateStatisticsViewModel(string? period, string? fromMonth, string? toMonth, bool compare)
    {
        var currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        period = period?.ToLowerInvariant();

        DateTime startDate;
        DateTime endDate = currentMonth;
        switch (period)
        {
            case "month":
                startDate = currentMonth;
                break;
            case "3m":
                startDate = currentMonth.AddMonths(-2);
                break;
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

        return BuildStatisticsViewModel(period ?? "6m", startDate, endDate, compare);
    }

    private StatisticsViewModel BuildStatisticsViewModel(string period, DateTime startDate, DateTime endDate, bool compare)
    {
        var viewModel = new StatisticsViewModel
        {
            Period = period,
            StartDate = startDate,
            EndDate = endDate,
            CompareWithPreviousYear = compare,
            IsCurrentMonthIncomplete = endDate >= new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
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

        return viewModel;
    }

    /// <summary>Laddar sidan "Läs loggboken" , men heter visa för att slippa ä i Läs</summary>
    /// <param name="datum">Format: ÅÅÅÅMMDD (t.ex: 20210126)</param>
    public async Task<IActionResult> VisaLoggbokenAsync(DateTime datum, string? level = "all")
    {
        ViewData["Title"] = localizer["Admin.LogTitle", datum];

        AppLogReaderService logReader = new();
        string appLogFile = $"appLog{datum:yyyyMMdd}.txt";

        var viewModel = await BuildLogbookViewModel(logReader, appLogFile, datum, null, level);
        return View(viewModel);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SearchLogbook(DateTime datum, string? search, string? level = "all")
    {
        if (search?.Length > 100)
        {
            return BadRequest();
        }

        ViewData["Title"] = localizer["Admin.LogTitle", datum];
        var logReader = new AppLogReaderService();
        var viewModel = await BuildLogbookViewModel(logReader, $"appLog{datum:yyyyMMdd}.txt", datum, search, level);
        return View("VisaLoggboken", viewModel);
    }

    private async Task<UploadLogReaderViewModel> BuildLogbookViewModel(
        AppLogReaderService logReader, string logFile, DateTime date, string? search, string? level)
    {
        var user = await _userManager.GetUserAsync(User) ?? new ArvidsonFotoUser();
        var levelFilter = level is "error" or "warning" or "info" ? level : "all";
        var lines = logReader.ReadData(logFile);
        if (!user.ShowAllLogs)
        {
            lines = [.. lines.Where(IsSignificantLogLine)];
        }

        if (levelFilter != "all")
        {
            var levelMarker = levelFilter switch
            {
                "error" => "[ERR]",
                "warning" => "[WRN]",
                _ => "[INF]"
            };
            lines = [.. lines.Where(line => line.Contains(levelMarker, StringComparison.Ordinal))];
        }

        search = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        if (search is not null)
        {
            lines = [.. lines.Where(line => line.Contains(search, StringComparison.OrdinalIgnoreCase))];
        }

        return new UploadLogReaderViewModel
        {
            ExistingLogFiles = logReader.ExistingLogFiles(),
            LogBook = lines,
            DateShown = date,
            ShowAllLogs = user.ShowAllLogs,
            SearchQuery = search,
            LevelFilter = levelFilter,
            FilteredCount = lines.Count
        };
    }

    private static bool IsSignificantLogLine(string line) =>
        !line.Contains("/images/gallery", StringComparison.Ordinal) &&
        !line.Contains("/img/gallery", StringComparison.Ordinal) &&
        !line.Contains("to page: /Senast/Fotograferad", StringComparison.Ordinal) &&
        !line.Contains("Redirect from page: ", StringComparison.Ordinal) &&
        !line.Contains("/bilder/", StringComparison.Ordinal) &&
        !line.Contains("/GB.asp", StringComparison.Ordinal) &&
        !line.Contains("/code/", StringComparison.Ordinal) &&
        !line.Contains("/apple-", StringComparison.Ordinal) &&
        !line.Contains("/wp-", StringComparison.Ordinal);

    /// <summary>
    /// Funktion som aktiverar/inaktiverar alla Loggar. Kanske dock borde ligga i AccountManagement annars...
    /// </summary>
    [Route("[controller]/ToggleShowAllLogs")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleShowAllLogs(string date)
    {
        if (string.IsNullOrWhiteSpace(date))
        {
            date = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
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
                if (image.ImageCategoryId is not int categoryId ||
                    categoryId <= 0 ||
                    _categoryService.GetById(categoryId).CategoryId != categoryId)
                {
                    return RedirectToAction("FacebookUpload", new { DisplayMessage = "FacebookError" });
                }

                var categoryPath = _categoryService.GetCategoryDisplayPathForImage(categoryId);
                var imageUrl = BuildFacebookImageUrl(categoryPath, image.ImageUrlName);
                if (imageUrl is null)
                {
                    return RedirectToAction("FacebookUpload", new { DisplayMessage = "FacebookError" });
                }

                imageUrls.Add(imageUrl);
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