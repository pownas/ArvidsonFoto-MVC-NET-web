using ArvidsonFoto.Controllers;
using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.DTOs;
using ArvidsonFoto.Core.Models;
using ArvidsonFoto.Core.Services;
using ArvidsonFoto.Core.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;
using System.Security.Claims;

namespace ArvidsonFoto.Tests.Unit.ControllerTests;

public class UploadAdminWorkflowTests
{
    [Fact]
    public void HanteraGbFiltersUnreadEntriesAndSortsByDate()
    {
        using var db = CreateContext();
        db.TblGbs.AddRange(
            GuestbookPost(1, false, new DateTime(2026, 4, 10)),
            GuestbookPost(2, false, new DateTime(2026, 4, 1)),
            GuestbookPost(3, true, new DateTime(2026, 3, 1)));
        db.SaveChanges();

        var result = Assert.IsType<ViewResult>(CreateController(db).HanteraGB("", "", "unread", "oldest"));
        var model = Assert.IsType<UploadGbViewModel>(result.Model);

        Assert.Collection(
            model.Posts,
            post => Assert.Equal(2, post.GbId),
            post => Assert.Equal(1, post.GbId));
        Assert.Equal("unread", model.StatusFilter);
        Assert.Equal("oldest", model.SortOrder);
    }

    [Fact]
    public void DeleteGbPostRequiresPostAndAntiforgeryAndDeletesWhenCalled()
    {
        using var db = CreateContext();
        db.TblGbs.Add(GuestbookPost(42, false, DateTime.Today));
        db.SaveChanges();
        var controller = CreateController(db);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                    [new Claim(ClaimTypes.Name, "admin")], "test"))
            }
        };

        var action = typeof(UploadAdminController).GetMethod(nameof(UploadAdminController.DeleteGbPost))!;
        Assert.NotNull(action.GetCustomAttributes(typeof(HttpPostAttribute), inherit: true).SingleOrDefault());
        Assert.NotNull(action.GetCustomAttributes(typeof(ValidateAntiForgeryTokenAttribute), inherit: true).SingleOrDefault());

        var result = Assert.IsType<RedirectToActionResult>(controller.DeleteGbPost(42, "unread", "oldest"));

        Assert.Empty(db.TblGbs);
        Assert.Equal("HanteraGB", result.ActionName);
        Assert.Equal("unread", result.RouteValues!["status"]);
        Assert.Equal("oldest", result.RouteValues["sort"]);
    }

    [Fact]
    public async Task CreateImageLinkDoesNotSaveWhenPublishedFilesCannotBeVerified()
    {
        using var db = CreateContext();
        AddLeafCategory(db);
        db.SaveChanges();
        var handler = new StatusHandler(HttpStatusCode.NotFound);
        var controller = CreateController(db, new StubHttpClientFactory(handler));
        SetControllerContext(controller, db);

        var result = Assert.IsType<RedirectToActionResult>(await controller.CreateImageLink(NewImage("missing-file")));

        Assert.Equal("NyBild", result.ActionName);
        Assert.Empty(db.TblImages);
        Assert.Single(handler.Requests);
    }

    [Fact]
    public async Task CreateImageLinkSavesAfterBothImageFilesAreVerified()
    {
        using var db = CreateContext();
        AddLeafCategory(db);
        db.TblImages.Add(new TblImage { ImageId = 10, ImageUrlName = "existing" });
        db.SaveChanges();
        var handler = new StatusHandler(HttpStatusCode.OK);
        var controller = CreateController(db, new StubHttpClientFactory(handler));
        SetControllerContext(controller, db);

        var input = NewImage("published-file");
        input.ImageFamilj = 999;
        input.ImageHuvudfamilj = 999;
        var result = Assert.IsType<RedirectToActionResult>(await controller.CreateImageLink(input));

        var savedImage = db.TblImages.Single(image => image.ImageUrlName == "published-file");
        Assert.Equal(11, savedImage.ImageId);
        Assert.Null(savedImage.ImageFamilyId);
        Assert.Null(savedImage.ImageMainFamilyId);
        Assert.Equal("NyBild", result.ActionName);
        Assert.Equal(2, handler.Requests.Count);
        Assert.Contains(handler.Requests, uri => uri.AbsolutePath.EndsWith("/birds/published-file.jpg", StringComparison.Ordinal));
        Assert.Contains(handler.Requests, uri => uri.AbsolutePath.EndsWith("/birds/published-file.thumb.jpg", StringComparison.Ordinal));
    }

    private static TblGb GuestbookPost(int id, bool isRead, DateTime date) =>
        new() { Id = id, GbId = id, GbReadPost = isRead, GbDate = date, GbText = "Entry" };

    private static void AddLeafCategory(ArvidsonFotoCoreDbContext db) =>
        db.TblMenus.Add(new TblMenu
        {
            MenuCategoryId = 5,
            MenuDisplayName = "birds",
            MenuUrlSegment = "birds"
        });

    private static UploadImageInputDto NewImage(string fileName) =>
        new()
        {
            ImageArt = 5,
            ImageUrl = fileName,
            ImageDate = DateTime.Today,
            ImageDescription = "Description"
        };

    private static ArvidsonFotoCoreDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<ArvidsonFotoCoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static UploadAdminController CreateController(ArvidsonFotoCoreDbContext db, IHttpClientFactory? httpClientFactory = null) =>
        new(
            db,
            null!,
            null!,
            NullLogger<ApiImageService>.Instance,
            NullLogger<ApiCategoryService>.Instance,
            new ConfigurationBuilder().Build(),
            new MemoryCache(new MemoryCacheOptions()),
            new TestLocalizer(),
            new PageCounterService(db),
            httpClientFactory);

    private static void SetControllerContext(UploadAdminController controller, ArvidsonFotoCoreDbContext db)
    {
        var services = new ServiceCollection().AddSingleton(db).BuildServiceProvider();
        var httpContext = new DefaultHttpContext { RequestServices = services };
        var actionContext = new ActionContext(
            httpContext,
            new Microsoft.AspNetCore.Routing.RouteData(),
            new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor());
        controller.Url = new UrlHelper(actionContext);
        controller.ControllerContext = new ControllerContext(actionContext);
        controller.TempData = new TempDataDictionary(httpContext, new TestTempDataProvider());
    }

    private sealed class StubHttpClientFactory(HttpMessageHandler handler) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new(handler, disposeHandler: false);
    }

    private sealed class StatusHandler(HttpStatusCode statusCode) : HttpMessageHandler
    {
        public List<Uri> Requests { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(request.RequestUri!);
            return Task.FromResult(new HttpResponseMessage(statusCode));
        }
    }

    private sealed class TestTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();

        public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
    }

    private sealed class TestLocalizer : IStringLocalizer<UploadAdminResource>
    {
        public LocalizedString this[string name] => new(name, name);

        public LocalizedString this[string name, params object[] arguments] =>
            new(name, string.Format(System.Globalization.CultureInfo.InvariantCulture, name, arguments));

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => [];
    }
}
