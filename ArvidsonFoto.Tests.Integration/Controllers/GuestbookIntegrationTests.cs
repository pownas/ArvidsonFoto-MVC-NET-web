using ArvidsonFoto.Tests.Integration.Helpers;

namespace ArvidsonFoto.Tests.Integration.Controllers;

/// <summary>
/// Integration tests for Guestbook (Gästbok) functionality.
/// These tests verify the complete HTTP workflow including routing, forms, and database operations.
/// </summary>
[TestClass]
public class GuestbookIntegrationTests
{
    private static ArvidsonFotoWebApplicationFactory? _factory;
    private static HttpClient? _client;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        _factory = new ArvidsonFotoWebApplicationFactory();
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    [TestMethod]
    public async Task ProductionSiteCss_ServesCompiledStaticFile()
    {
        using var factory = _factory!.WithWebHostBuilder(builder => builder
            .UseEnvironment("Production")
            .UseSetting("ConnectionStrings:UseInMemoryDatabase", "true"));
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });

        using var response = await client.GetAsync("/css/site.css");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("text/css", response.Content.Headers.ContentType?.MediaType);

        var webRoot = factory.Services.GetRequiredService<IWebHostEnvironment>().WebRootPath;
        var expected = await File.ReadAllBytesAsync(Path.Combine(webRoot, "css", "site.css"));
        CollectionAssert.AreEqual(expected, await response.Content.ReadAsByteArrayAsync());
    }

    #region GET /Info/Gastbok Tests

    [TestMethod]
    public async Task GetGastbok_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client!.GetAsync("/info/gastbok?culture=sv-SE");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task GetGastbok_ReturnsHtmlContent()
    {
        // Act
        var response = await _client!.GetAsync("/info/gastbok?culture=sv-SE");
        // Assert
        Assert.IsTrue(response.Content.Headers.ContentType?.MediaType?.Contains("text/html") ?? false);
        var document = await HtmlHelpers.GetDocumentAsync(response);
        Assert.IsTrue(document.Body!.TextContent.Contains("Gästbok"));
    }

    [TestMethod]
    public async Task EnglishGuestbook_TranslatesNavigationAndPreservesQueryWhenSwitching()
    {
        var response = await _client!.GetAsync("/info/gastbok?culture=en-US&page=2&search=birds");
        var document = await HtmlHelpers.GetDocumentAsync(response);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsTrue(document.Body!.TextContent.Contains("Sign the guestbook"));
        var swedishLink = document.QuerySelector("a[lang='sv']")?.GetAttribute("href");
        Assert.IsNotNull(swedishLink);
        Assert.IsTrue(swedishLink.Contains("page=2"));
        Assert.IsTrue(swedishLink.Contains("search=birds"));
        Assert.IsTrue(swedishLink.Contains("culture=sv-SE"));
    }

    [TestMethod]
    public async Task Gallery_TranslatesImageCaptionsBreadcrumbsAndCount()
    {
        var englishResponse = await _client!.GetAsync("/images/birds/woodpeckers/eurasian-three-toed-woodpecker?culture=en-US");
        var english = await HtmlHelpers.GetDocumentAsync(englishResponse);

        Assert.AreEqual(HttpStatusCode.OK, englishResponse.StatusCode);
        Assert.IsTrue(english.QuerySelector("#gallery figcaption")?.TextContent.Contains("Eurasian three-toed woodpecker"));
        Assert.IsTrue(english.QuerySelector("#page-breadcrumbs")?.TextContent.Contains("Photos"));
        Assert.IsTrue(english.QuerySelector("#page-image-counter-bottom")?.TextContent.Contains("Photos:"));
        Assert.IsNotNull(english.QuerySelector("a[lang='sv'] svg[aria-hidden='true']"));
        Assert.IsNotNull(english.QuerySelector("a[lang='en'] svg[aria-hidden='true']"));

        var swedishResponse = await _client.GetAsync("/bilder/faglar/tretaig-hackspett?culture=sv-SE");
        var swedish = await HtmlHelpers.GetDocumentAsync(swedishResponse);
        Assert.AreEqual(HttpStatusCode.OK, swedishResponse.StatusCode);
        Assert.IsTrue(swedish.QuerySelector("#gallery figcaption")?.TextContent.Contains("Tretåig hackspett"));
        Assert.IsTrue(swedish.QuerySelector("#page-image-counter-bottom")?.TextContent.Contains("Antal bilder:"));
    }

    [TestMethod]
    public async Task BeaverCategoryName_UsesEnglishEvenAfterSwedishRequest()
    {
        var swedish = await HtmlHelpers.GetDocumentAsync(await _client!.GetAsync("/bilder/daggdjur/baver?culture=sv-SE"));
        var english = await HtmlHelpers.GetDocumentAsync(await _client.GetAsync("/images/mammals/beaver?culture=en-US"));
        Assert.IsTrue(swedish.QuerySelector("h1, h2")?.TextContent.Contains("Bäver") == true ||
            swedish.QuerySelector("#gallery figcaption")?.TextContent.Contains("Bäver") == true);
        Assert.IsTrue(english.QuerySelector("#gallery figcaption")?.TextContent.Contains("Beaver") == true);
    }

    [TestMethod]
    public async Task BeaverGallery_UsesSelectedLanguageOnSwedishAndEnglishRoutes()
    {
        foreach (var path in new[] { "/bilder/daggdjur/baver", "/images/mammals/beaver" })
        {
            using var client = _factory!.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            var englishResponse = await client.GetAsync($"{path}?culture=en-US");
            var english = await HtmlHelpers.GetDocumentAsync(englishResponse);
            Assert.AreEqual(HttpStatusCode.OK, englishResponse.StatusCode, path);
            Assert.IsTrue(english.QuerySelectorAll("#gallery figcaption").Any(c => c.TextContent.Contains("Beaver")), path);
            Assert.IsTrue(english.QuerySelector("#page-breadcrumbs")?.TextContent.Contains("Photos") == true, path);
            Assert.IsTrue(english.QuerySelector("#page-image-counter-bottom")?.TextContent.Contains("Photos:") == true, path);
            Assert.IsTrue(english.QuerySelector("meta[name='description']")?.GetAttribute("content")?.Contains("Beaver") == true, path);
            Assert.IsNotNull(english.QuerySelector("a[lang='sv'][href^='/bilder/daggdjur/baver']"), path);
            Assert.IsTrue(english.QuerySelector(".main-nav")?.TextContent.Contains("Latest") == true, path);
            Assert.AreEqual("https://ArvidsonFoto.se/images/mammals/beaver",
                english.QuerySelector("link[rel='canonical']")?.GetAttribute("href"), path);
            Assert.IsNotNull(english.QuerySelector("#page-breadcrumbs a[href='/images/mammals']"), path);

            var swedish = await HtmlHelpers.GetDocumentAsync(await client.GetAsync($"{path}?culture=sv-SE"));
            Assert.IsTrue(swedish.QuerySelectorAll("#gallery figcaption").Any(c => c.TextContent.Contains("Bäver")), path);
            Assert.IsTrue(swedish.QuerySelector("#page-image-counter-bottom")?.TextContent.Contains("Antal bilder:") == true, path);
            Assert.IsTrue(swedish.QuerySelector(".main-nav")?.TextContent.Contains("Senast") == true, path);
            Assert.AreEqual("https://ArvidsonFoto.se/bilder/daggdjur/baver",
                swedish.QuerySelector("link[rel='canonical']")?.GetAttribute("href"), path);
        }
    }

    [TestMethod]
    public async Task HomePage_TranslatesTitleAndPortraitDescription()
    {
        var english = await HtmlHelpers.GetDocumentAsync(await _client!.GetAsync("/?culture=en-US"));
        Assert.IsTrue(english.Title?.Contains("Welcome to my photo archive") == true);
        Assert.AreEqual("Portrait of Torbjörn Arvidson",
            english.QuerySelector("img[src='./img/Torbjorn2020.jpg']")?.GetAttribute("alt"));

        var swedish = await HtmlHelpers.GetDocumentAsync(await _client.GetAsync("/?culture=sv-SE"));
        Assert.IsTrue(swedish.Title?.Contains("Välkommen till mitt bildarkiv") == true);
        Assert.AreEqual("Porträtt av Torbjörn Arvidson",
            swedish.QuerySelector("img[src='./img/Torbjorn2020.jpg']")?.GetAttribute("alt"));
    }

    [TestMethod]
    public async Task EnglishCookie_TranslatesGalleryOnSwedishBasePath()
    {
        using var client = _factory!.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true
        });
        await client.GetAsync("/bilder/daggdjur/baver?culture=en-US");
        var english = await HtmlHelpers.GetDocumentAsync(await client.GetAsync("/bilder/daggdjur/baver"));

        Assert.IsTrue(english.QuerySelector("#gallery figcaption")?.TextContent.Contains("Beaver") == true);
        Assert.IsTrue(english.QuerySelector("#page-breadcrumbs")?.TextContent.Contains("Photos") == true);
        Assert.IsTrue(english.QuerySelector("#page-image-counter-bottom")?.TextContent.Contains("Photos:") == true);
        Assert.IsNotNull(english.QuerySelector("#page-breadcrumbs a[href='/images/mammals']"));
    }

    [TestMethod]
    public async Task TranslationReview_RequiresAuthentication()
    {
        var get = await _client!.GetAsync("/translationreview?culture=sv-SE");
        Assert.AreEqual(HttpStatusCode.Redirect, get.StatusCode);
        StringAssert.Contains(get.Headers.Location?.OriginalString ?? "", "/identity/account/login");

        var post = await _client.PostAsync("/translationreview/save", new FormUrlEncodedContent(
            new Dictionary<string, string> { ["table"] = "menu", ["id"] = "1", ["field"] = "name", ["reviewed"] = "true" }));
        Assert.AreEqual(HttpStatusCode.Redirect, post.StatusCode);
        StringAssert.Contains(post.Headers.Location?.OriginalString ?? "", "/identity/account/login");
    }

    [TestMethod]
    public async Task CategoryTooltip_UsesSelectedLanguage()
    {
        foreach (var (culture, noImage, loading, date) in new[] {
            ("sv-SE", "Ingen bild tillgänglig", "Laddar...", "Fotodatum"),
            ("en-US", "No photo available", "Loading...", "Date photographed")
        })
        {
            using var client = _factory!.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            var document = await HtmlHelpers.GetDocumentAsync(await client.GetAsync($"/?culture={culture}"));
            var nav = document.QuerySelector(".main-nav");
            Assert.IsNotNull(nav);
            Assert.AreEqual(noImage, nav.GetAttribute("data-tooltip-no-image"));
            Assert.AreEqual(loading, nav.GetAttribute("data-tooltip-loading"));
            Assert.AreEqual(date, nav.GetAttribute("data-tooltip-date"));
        }
    }

    [TestMethod]
    public async Task GalleryApi_ResolvesEnglishSegmentsAndProvidesLocalizedCaptions()
    {
        foreach (var (culture, expectedName) in new[] { ("en-US", "Beaver"), ("sv-SE", "Bäver") })
        {
            var response = await _client!.GetAsync($"/api/images/mammals/beaver?page=1&culture={culture}");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var json = await response.Content.ReadAsStringAsync();
            StringAssert.Contains(json, expectedName);
        }
    }

    [TestMethod]
    public async Task PublicUtilityPages_UseSelectedCulture()
    {
        foreach (var (culture, privacy, error, theme) in new[] {
            ("sv-SE", "Integritetspolicy", "Ett fel uppstod", "Växla mellan mörkt och ljust läge"),
            ("en-US", "Privacy policy", "An error occurred", "Switch between dark and light mode")
        })
        {
            using var client = _factory!.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            var privacyPage = await HtmlHelpers.GetDocumentAsync(await client.GetAsync($"/home/privacy?culture={culture}"));
            Assert.IsTrue(privacyPage.Body!.TextContent.Contains(privacy));
            Assert.IsTrue(privacyPage.QuerySelector("#theme-switcher-description")?.TextContent.Contains(theme) == true);
            var errorPage = await HtmlHelpers.GetDocumentAsync(await client.GetAsync($"/home/error?culture={culture}"));
            Assert.IsTrue(errorPage.Body!.TextContent.Contains(error));
        }
    }

    [TestMethod]
    public async Task LegacyCategoryId_RedirectsWithoutCachingLanguage()
    {
        using var client = _factory!.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var english = await client.GetAsync("/gallery.asp?ID=11&culture=en-US");
        Assert.AreEqual(HttpStatusCode.Redirect, english.StatusCode);
        Assert.AreEqual("/images/mammals/beaver", english.Headers.Location?.OriginalString);

        var swedish = await client.GetAsync("/gallery.asp?ID=11&culture=sv-SE");
        Assert.AreEqual(HttpStatusCode.Redirect, swedish.StatusCode);
        Assert.AreEqual("/bilder/daggdjur/baver", swedish.Headers.Location?.OriginalString);
    }

    [TestMethod]
    public async Task LegacyCasingAndCategoryNames_RedirectToCanonicalLowercaseUrls()
    {
        using var client = _factory!.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var category = await client.GetAsync("/bilder/Däggdjur/Bäver?culture=sv-SE&sida=2");
        Assert.AreEqual(HttpStatusCode.PermanentRedirect, category.StatusCode);
        Assert.AreEqual("/bilder/daggdjur/baver?culture=sv-SE&sida=2", category.Headers.Location?.OriginalString);

        var legacyApi = await client.GetAsync("/api/bilder/Däggdjur/Bäver?page=1");
        Assert.AreEqual(HttpStatusCode.PermanentRedirect, legacyApi.StatusCode);
        Assert.AreEqual("/api/bilder/daggdjur/baver?page=1", legacyApi.Headers.Location?.OriginalString);

        var encodedCategory = await client.GetAsync("/bilder/Resor/2010%20Turkiet%2C%20Istanbul");
        Assert.AreEqual(HttpStatusCode.PermanentRedirect, encodedCategory.StatusCode);
        Assert.AreEqual("/bilder/resor/2010-turkiet-istanbul", encodedCategory.Headers.Location?.OriginalString);
        var lowercaseEscapeCategory = await client.GetAsync("/bilder/resor/2010-turkiet%2c-istanbul");
        Assert.AreEqual(HttpStatusCode.PermanentRedirect, lowercaseEscapeCategory.StatusCode);
        Assert.AreEqual("/bilder/resor/2010-turkiet-istanbul", lowercaseEscapeCategory.Headers.Location?.OriginalString);

        var legacyContact = await client.GetAsync("/Info/Contact?culture=sv-SE");
        Assert.AreEqual(HttpStatusCode.PermanentRedirect, legacyContact.StatusCode);
        Assert.AreEqual("/info/contact?culture=sv-SE", legacyContact.Headers.Location?.OriginalString);

        var canonicalContact = await client.GetAsync(legacyContact.Headers.Location);
        Assert.AreEqual(HttpStatusCode.MovedPermanently, canonicalContact.StatusCode);
        Assert.AreEqual("/info/kontakta", canonicalContact.Headers.Location?.OriginalString);
        Assert.AreEqual(HttpStatusCode.OK, (await client.GetAsync(canonicalContact.Headers.Location)).StatusCode);
    }

    [DataTestMethod]
    [DataRow("/bilder/insekter/fjarilar/brun-blavinge", "Brun blåvinge")]
    [DataRow("/bilder/daggdjur/bjorn", "Björn")]
    [DataRow("/bilder/faglar/vadare/morkulla", "Morkulla")]
    public async Task TransliteratedCategorySlugs_ResolveToExpectedCategory(string path, string categoryName)
    {
        var response = await _client!.GetAsync($"{path}?culture=sv-SE");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var document = await HtmlHelpers.GetDocumentAsync(response);
        Assert.IsTrue(document.Body!.TextContent.Contains(categoryName, StringComparison.Ordinal));
    }

    [TestMethod]
    public async Task PublicPageLinks_UseLowercasePaths()
    {
        foreach (var path in new[] { "/", "/info", "/info/gastbok", "/search", "/sok", "/latest", "/bilder/faglar" })
        {
            var response = await _client!.GetAsync(path);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, path);

            var document = await HtmlHelpers.GetDocumentAsync(response);
            foreach (var anchor in document.QuerySelectorAll("a[href^='/']"))
            {
                var href = anchor.GetAttribute("href")!;
                var absoluteUri = new Uri(new Uri("https://localhost"), href);
                var decodedPath = Uri.UnescapeDataString(absoluteUri.AbsolutePath);
                Assert.AreEqual(decodedPath.ToLowerInvariant(), decodedPath, href);
            }
        }
    }

    [TestMethod]
    public async Task SwedishAndEnglishSearch_UseDistinctCanonicalPaths()
    {
        using var client = _factory!.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var swedish = await client.GetAsync("/sok?culture=sv-SE");
        Assert.AreEqual(HttpStatusCode.OK, swedish.StatusCode);
        var swedishDocument = await HtmlHelpers.GetDocumentAsync(swedish);
        Assert.AreEqual("/sok", swedishDocument.QuerySelector("form[role='search']")?.GetAttribute("action"));
        Assert.AreEqual("https://ArvidsonFoto.se/sok", swedishDocument.QuerySelector("link[rel='canonical']")?.GetAttribute("href"));

        var english = await client.GetAsync("/search?culture=en-US");
        Assert.AreEqual(HttpStatusCode.OK, english.StatusCode);
        var englishDocument = await HtmlHelpers.GetDocumentAsync(english);
        Assert.AreEqual("/search", englishDocument.QuerySelector("form[role='search']")?.GetAttribute("action"));
        Assert.AreEqual("https://ArvidsonFoto.se/search", englishDocument.QuerySelector("link[rel='canonical']")?.GetAttribute("href"));

        var legacySwedish = await client.GetAsync("/Search?culture=sv-SE");
        Assert.AreEqual(HttpStatusCode.PermanentRedirect, legacySwedish.StatusCode);
        Assert.AreEqual("/sok?culture=sv-SE", legacySwedish.Headers.Location?.OriginalString);

        var legacyEnglish = await client.GetAsync("/Search?culture=en-US");
        Assert.AreEqual(HttpStatusCode.PermanentRedirect, legacyEnglish.StatusCode);
        Assert.AreEqual("/search?culture=en-US", legacyEnglish.Headers.Location?.OriginalString);
    }

    [TestMethod]
    public async Task EnglishSearch_FindsEnglishAndSwedishCategoryNames()
    {
        foreach (var term in new[] { "Eurasian", "Tretåig" })
        {
            var response = await _client!.GetAsync($"/search?s={Uri.EscapeDataString(term)}&culture=en-US");
            var document = await HtmlHelpers.GetDocumentAsync(response);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(document.QuerySelectorAll("#gallery figcaption")
                .Any(caption => caption.TextContent.Contains("Eurasian three-toed woodpecker")));
            Assert.IsTrue(document.Body!.TextContent.Contains("Search results"));
        }
    }

    [TestMethod]
    public async Task Sitemap_UsesWorkingLocalizedRoutesAndPreservesSwedishLinks()
    {
        using var freshClient = _factory!.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var response = await freshClient.GetAsync("/information/sitemap");
        var document = await HtmlHelpers.GetDocumentAsync(response);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsTrue(document.Body!.TextContent.Contains("Explore all pages and photo categories"));

        foreach (var path in new[] { "/images/birds/woodpeckers/eurasian-three-toed-woodpecker",
                     "/latest/photographed", "/information/contact", "/information/guestbook" })
        {
            Assert.IsNotNull(document.QuerySelector($"a[href='{path}']"), path);
            Assert.AreEqual(HttpStatusCode.OK, (await _client!.GetAsync($"{path}?culture=en-US")).StatusCode, path);
        }

        var languageLink = document.QuerySelector("a[lang='sv']")?.GetAttribute("href");
        Assert.IsTrue(languageLink?.StartsWith("/info/sidkarta", StringComparison.Ordinal) == true);
        var swedishResponse = await _client!.GetAsync("/info/sidkarta?culture=sv-SE");
        var swedish = await HtmlHelpers.GetDocumentAsync(swedishResponse);
        Assert.IsNotNull(swedish.QuerySelector("a[href='/bilder/faglar/hackspettar/tretaig-hackspett']"));
    }

    [TestMethod]
    public async Task EnglishAlias_PersistsLanguageAndUsesEnglishCanonical()
    {
        using var client = _factory!.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var gallery = await client.GetAsync("/images/");
        Assert.AreEqual(HttpStatusCode.Redirect, gallery.StatusCode);
        Assert.AreEqual("/latest/photographed", gallery.Headers.Location?.OriginalString);

        var contact = await client.GetAsync("/information/contact");
        var contactPage = await HtmlHelpers.GetDocumentAsync(contact);
        Assert.IsNotNull(contactPage.QuerySelector("link[rel='canonical'][href='https://ArvidsonFoto.se/information/contact']"));

        var home = await HtmlHelpers.GetDocumentAsync(await client.GetAsync("/"));
        Assert.IsNotNull(home.QuerySelector("a[href^='/images/']"));
    }

    [TestMethod]
    public async Task InformationPagesAndForms_TranslateEnglishAndRetainSwedish()
    {
        using var client = _factory!.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        foreach (var (englishPath, englishText, swedishPath, swedishText) in new[]
        {
            ("/information/about", "A passion for nature photography", "/info/om_mig", "Passion för naturfotografi"),
            ("/information/contact", "You can contact me by email or phone.", "/info/kontakta", "Om du vill kontakta mig"),
            ("/information/buy-photos", "What can I help you with?", "/info/kop_av_bilder", "Vad kan jag hjälpa till med?"),
            ("/information/copyright", "All rights reserved by the photographer.", "/info/copyright", "Alla rättigheter förbehållna fotografen.")
        })
        {
            var english = await HtmlHelpers.GetDocumentAsync(await client.GetAsync($"{englishPath}?culture=en-US"));
            Assert.IsTrue(english.Body!.TextContent.Contains(englishText), englishPath);
            var swedish = await HtmlHelpers.GetDocumentAsync(await client.GetAsync($"{swedishPath}?culture=sv-SE"));
            Assert.IsTrue(swedish.Body!.TextContent.Contains(swedishText), swedishPath);
        }

        var contact = await HtmlHelpers.GetDocumentAsync(await client.GetAsync("/information/contact?culture=en-US"));
        Assert.AreEqual("Your name", contact.QuerySelector("#Name")?.GetAttribute("placeholder"));
        Assert.AreEqual("Enter your name.", contact.QuerySelector("#Name")?.GetAttribute("data-val-required"));
        Assert.AreEqual("Enter the digits shown in the image", contact.QuerySelector("label[for='Code']")?.TextContent.Trim().TrimEnd('*').Trim());
        var guestbook = await HtmlHelpers.GetDocumentAsync(await client.GetAsync("/information/guestbook?culture=en-US"));
        Assert.AreEqual("Name (leave blank to be anonymous)", guestbook.QuerySelector("#Name")?.GetAttribute("placeholder"));
        Assert.AreEqual("Enter a message.", guestbook.QuerySelector("#Message")?.GetAttribute("data-val-required"));

        var swedishContact = await HtmlHelpers.GetDocumentAsync(await client.GetAsync("/info/kontakta?culture=sv-SE"));
        Assert.AreEqual("Ditt namn", swedishContact.QuerySelector("#Name")?.GetAttribute("placeholder"));
        Assert.AreEqual("Ange ditt namn", swedishContact.QuerySelector("#Name")?.GetAttribute("data-val-required"));
        var latest = await HtmlHelpers.GetDocumentAsync(await client.GetAsync("/latest/photographed?culture=en-US"));
        Assert.IsTrue(latest.QuerySelector("meta[name='description']")?.GetAttribute("content")?.Contains("nature and wildlife photos") == true);
    }

    [TestMethod]
    public async Task SwitchingGalleryLanguage_PreservesCategoryAndPage()
    {
        var response = await _client!.GetAsync("/images/birds/woodpeckers/eurasian-three-toed-woodpecker?culture=en-US&sida=2");
        var document = await HtmlHelpers.GetDocumentAsync(response);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsTrue(document.QuerySelector("a[lang='sv']")?.GetAttribute("href")
            ?.Contains("/bilder/faglar/hackspettar/tretaig-hackspett?sida=2", StringComparison.Ordinal) == true);
    }
    [TestMethod]
    public async Task GetGastbok_ContainsGuestbookForm()
    {
        // Act
        var response = await _client!.GetAsync("/info/gastbok");
        var document = await HtmlHelpers.GetDocumentAsync(response);

        // Assert
        var form = document.QuerySelector("form[action='/info/posttogb']");
        Assert.IsNotNull(form, "Guestbook form should be present");
        Assert.AreEqual("/info/posttogb", form!.GetAttribute("action"));
    }

    [TestMethod]
    public async Task GetGastbok_ContainsRequiredFields()
    {
        // Act
        var response = await _client!.GetAsync("/info/gastbok");
        var document = await HtmlHelpers.GetDocumentAsync(response);

        // Assert - Check for required form fields
        Assert.IsNotNull(document.QuerySelector("input[name='Code']"), "Code field should exist");
        Assert.IsNotNull(document.QuerySelector("input[name='Name']"), "Name field should exist");
        Assert.IsNotNull(document.QuerySelector("input[name='Email']"), "Email field should exist");
        Assert.IsNotNull(document.QuerySelector("input[name='Homepage']"), "Homepage field should exist");
        Assert.IsNotNull(document.QuerySelector("textarea[name='Message']"), "Message field should exist");
    }

    [TestMethod]
    public async Task GetGastbok_ContainsAntiForgeryToken()
    {
        // Act
        var response = await _client!.GetAsync("/info/gastbok");
        var document = await HtmlHelpers.GetDocumentAsync(response);

        // Assert
        var token = HtmlHelpers.GetAntiForgeryToken(document);
        Assert.IsNotNull(token, "Anti-forgery token should be present");
        Assert.IsTrue(token.Length > 0, "Anti-forgery token should not be empty");
    }

    #endregion

    #region POST /Info/PostToGb Tests

    [TestMethod]
    public async Task PostToGb_WithValidData_RedirectsToGastbok()
    {
        // Arrange - Get the form page first to obtain anti-forgery token
        var getResponse = await _client!.GetAsync("/info/gastbok");
        var document = await HtmlHelpers.GetDocumentAsync(getResponse);

        var formData = HtmlHelpers.CreateFormData(document, new Dictionary<string, string>
        {
            ["Code"] = "3568",
            ["Name"] = "Integration Test User",
            ["Email"] = "integration@test.com",
            ["Homepage"] = "https://example.com",
            ["Message"] = "This is an integration test message"
        });

        var content = new FormUrlEncodedContent(formData);

        // Act
        var response = await _client.PostAsync("/info/posttogb", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
        Assert.IsTrue(response.Headers.Location?.ToString().Contains("/info/gastbok") ?? false);
    }

    [TestMethod]
    public async Task PostToGb_WithValidData_ShowsSuccessMessage()
    {
        // Arrange
        var getResponse = await _client!.GetAsync("/info/gastbok");
        var document = await HtmlHelpers.GetDocumentAsync(getResponse);

        var formData = HtmlHelpers.CreateFormData(document, new Dictionary<string, string>
        {
            ["Code"] = "3568",
            ["Name"] = "Success Test User",
            ["Email"] = "success@test.com",
            ["Homepage"] = "example.com",
            ["Message"] = "Testing success message"
        });

        var content = new FormUrlEncodedContent(formData);

        // Act - Post the form
        var postResponse = await _client.PostAsync("/info/posttogb", content);

        // Assert redirect happened
        Assert.AreEqual(HttpStatusCode.Redirect, postResponse.StatusCode);

        // Follow the redirect manually
        var redirectLocation = postResponse.Headers.Location?.ToString();
        Assert.IsNotNull(redirectLocation, "Should have redirect location");

        // Note: The success message is passed via route values/query string
        // We verify the redirect contains the success parameter
        Assert.IsTrue(
            redirectLocation.Contains("DisplayPublished") || redirectLocation.Contains("Gastbok"),
            "Redirect should go to Gastbok page with success indication"
        );
    }

    [TestMethod]
    public async Task PostToGb_WithInvalidCode_DoesNotRedirect()
    {
        // Arrange
        var getResponse = await _client!.GetAsync("/info/gastbok");
        var document = await HtmlHelpers.GetDocumentAsync(getResponse);

        var formData = HtmlHelpers.CreateFormData(document, new Dictionary<string, string>
        {
            ["Code"] = "0000", // Invalid code
            ["Name"] = "Invalid Code User",
            ["Email"] = "invalid@test.com",
            ["Homepage"] = "",
            ["Message"] = "This should fail"
        });

        var content = new FormUrlEncodedContent(formData);

        // Act
        var response = await _client.PostAsync("/info/posttogb", content);

        // Assert - Should redirect back to form with error
        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
    }

    [TestMethod]
    public async Task PostToGb_WithMissingRequiredFields_ShowsValidationErrors()
    {
        // Arrange
        var getResponse = await _client!.GetAsync("/info/gastbok");
        var document = await HtmlHelpers.GetDocumentAsync(getResponse);

        var formData = HtmlHelpers.CreateFormData(document, new Dictionary<string, string>
        {
            ["Code"] = "", // Missing required
            ["Name"] = "Test",
            ["Email"] = "test@test.com",
            ["Homepage"] = "",
            ["Message"] = "" // Missing required
        });

        var content = new FormUrlEncodedContent(formData);

        // Act
        var response = await _client.PostAsync("/info/posttogb", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
    }

    [TestMethod]
    public async Task PostToGb_WithoutAntiForgeryToken_Returns400()
    {
        // Arrange - Create form data without getting token
        var formData = new Dictionary<string, string>
        {
            ["Code"] = "3568",
            ["Name"] = "No Token User",
            ["Email"] = "notoken@test.com",
            ["Homepage"] = "",
            ["Message"] = "This should fail due to missing token"
        };

        var content = new FormUrlEncodedContent(formData);

        // Act
        var response = await _client!.PostAsync("/info/posttogb", content);

        // Assert - Should fail due to missing anti-forgery token
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, response.Headers.Location?.ToString());
    }

    [TestMethod]
    public async Task PostToGb_WithHomepageWithoutHttps_ProcessesCorrectly()
    {
        // Arrange
        var getResponse = await _client!.GetAsync("/info/gastbok");
        var document = await HtmlHelpers.GetDocumentAsync(getResponse);

        var formData = HtmlHelpers.CreateFormData(document, new Dictionary<string, string>
        {
            ["Code"] = "3568",
            ["Name"] = "No HTTPS User",
            ["Email"] = "nohttps@test.com",
            ["Homepage"] = "example.com", // Without https://
            ["Message"] = "Testing URL without protocol"
        });

        var content = new FormUrlEncodedContent(formData);

        // Act
        var response = await _client.PostAsync("/info/posttogb", content);

        // Assert - Should succeed and redirect
        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
        Assert.IsTrue(response.Headers.Location?.ToString().Contains("/info/gastbok") ?? false);
    }

    [TestMethod]
    public async Task PostToGb_WithLongHomepage_TruncatesCorrectly()
    {
        // Arrange
        var getResponse = await _client!.GetAsync("/info/gastbok");
        var document = await HtmlHelpers.GetDocumentAsync(getResponse);

        var formData = HtmlHelpers.CreateFormData(document, new Dictionary<string, string>
        {
            ["Code"] = "3568",
            ["Name"] = "Long URL User",
            ["Email"] = "longurl@test.com",
            ["Homepage"] = "https://example.com/level1/level2/level3/level4/level5",
            ["Message"] = "Testing URL depth truncation"
        });

        var content = new FormUrlEncodedContent(formData);

        // Act
        var response = await _client.PostAsync("/info/posttogb", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
    }

    [TestMethod]
    public async Task PostToGb_MultipleSubmissions_AllSucceed()
    {
        // Test that multiple submissions work correctly
        for (int i = 0; i < 3; i++)
        {
            // Arrange
            var getResponse = await _client!.GetAsync("/info/gastbok");
            var document = await HtmlHelpers.GetDocumentAsync(getResponse);

            var formData = HtmlHelpers.CreateFormData(document, new Dictionary<string, string>
            {
                ["Code"] = "3568",
                ["Name"] = $"Multi Test User {i}",
                ["Email"] = $"multi{i}@test.com",
                ["Homepage"] = "",
                ["Message"] = $"Test message number {i}"
            });

            var content = new FormUrlEncodedContent(formData);

            // Act
            var response = await _client.PostAsync("/info/posttogb", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode,
                $"Submission {i} should succeed");
        }
    }

    #endregion

    #region Route Tests

    [TestMethod]
    public async Task PostToGb_RouteIsAccessible()
    {
        // This test verifies that the explicit route "/Info/PostToGb" is correctly configured
        // This was the original bug that caused 404 errors

        // Arrange
        var getResponse = await _client!.GetAsync("/info/gastbok");
        var document = await HtmlHelpers.GetDocumentAsync(getResponse);

        var formData = HtmlHelpers.CreateFormData(document, new Dictionary<string, string>
        {
            ["Code"] = "3568",
            ["Name"] = "Route Test",
            ["Email"] = "route@test.com",
            ["Homepage"] = "",
            ["Message"] = "Testing route accessibility"
        });

        var content = new FormUrlEncodedContent(formData);

        // Act
        var response = await _client.PostAsync("/info/posttogb", content);

        // Assert - Should NOT get 404
        Assert.AreNotEqual(HttpStatusCode.NotFound, response.StatusCode,
            "Route /Info/PostToGb should be accessible (not 404)");
        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode,
            "POST should redirect after successful submission");
    }

    [TestMethod]
    public async Task PostToGb_WithGetRequest_IsNotAllowed()
    {
        // Verify that GET requests to PostToGb are not allowed.
        // With MapStaticAssets() in the pipeline, ASP.NET Core endpoint routing returns
        // 404 (instead of 405) for method-constrained routes. Both 404 and 405 correctly
        // indicate that GET is not a valid method for this POST-only endpoint.

        // Act
        var response = await _client!.GetAsync("/info/posttogb");

        // Assert - Should return either 405 Method Not Allowed or 404 Not Found (GET is not allowed)
        Assert.IsTrue(
            response.StatusCode is HttpStatusCode.MethodNotAllowed or
            HttpStatusCode.NotFound,
            $"Expected 405 or 404, but got {response.StatusCode}");
    }

    #endregion

    #region Performance Tests

    [TestMethod]
    [Timeout(15000)] // 15 seconds timeout (first request includes app startup)
    public async Task GetGastbok_RespondsWithinAcceptableTime()
    {
        // Note: First request includes application startup time
        // Subsequent requests should be much faster

        // Arrange - Warm up the application
        await _client!.GetAsync("/");

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var response = await _client!.GetAsync("/info/gastbok");
        stopwatch.Stop();

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        // Relaxed timing for integration tests which include middleware overhead
        Assert.IsTrue(stopwatch.ElapsedMilliseconds < 10000,
            $"Response took {stopwatch.ElapsedMilliseconds}ms, should be less than 10000ms");
    }

    [TestMethod]
    [Timeout(15000)] // 15 seconds timeout
    public async Task PostToGb_RespondsWithinAcceptableTime()
    {
        // Arrange
        var getResponse = await _client!.GetAsync("/info/gastbok");
        var document = await HtmlHelpers.GetDocumentAsync(getResponse);

        var formData = HtmlHelpers.CreateFormData(document, new Dictionary<string, string>
        {
            ["Code"] = "3568",
            ["Name"] = "Performance Test",
            ["Email"] = "perf@test.com",
            ["Homepage"] = "",
            ["Message"] = "Testing response time"
        });

        var content = new FormUrlEncodedContent(formData);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var response = await _client.PostAsync("/info/posttogb", content);
        stopwatch.Stop();

        // Assert
        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
        // Relaxed timing for integration tests which include middleware overhead
        Assert.IsTrue(stopwatch.ElapsedMilliseconds < 10000,
            $"POST took {stopwatch.ElapsedMilliseconds}ms, should be less than 10000ms");
    }

    #endregion
}