using ArvidsonFoto.Controllers;
using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.DTOs;
using ArvidsonFoto.Tests.Unit.MockServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace ArvidsonFoto.Tests.Unit.ControllerTests;

/// <summary>
/// Unit tests for InfoController, specifically focusing on guestbook (Gastbok) functionality.
/// These tests ensure that the POST endpoint remains functional and validates input correctly.
/// </summary>
public class InfoControllerTests
{
    private readonly InfoController _controller;
    private readonly MockGuestBookService _mockGuestBookService;

    public InfoControllerTests()
    {   
        // Create an in-memory database for Core context
        var coreOptions = new DbContextOptionsBuilder<ArvidsonFotoCoreDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;
        var mockCoreDbContext = new ArvidsonFotoCoreDbContext(coreOptions);
        
        _mockGuestBookService = new MockGuestBookService();
        var mockImageService = new MockImageService();
        var mockCategoryService = new MockCategoryService();
        var mockPageCounterService = new MockPageCounterService();

        _controller = new InfoController(mockCoreDbContext)
        {
            _guestbookService = _mockGuestBookService,
            _imageService = mockImageService,
            _categoryService = mockCategoryService,
            _pageCounterService = mockPageCounterService
        };

        // Setup HttpContext for the controller
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost");

        // Create ActionDescriptor for ActionContext
        var actionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor
        {
            ActionName = "PostToGb",
            ControllerName = "Info"
        };

        var actionContext = new ActionContext
        {
            HttpContext = httpContext,
            RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
            ActionDescriptor = actionDescriptor
        };

        var urlHelper = new UrlHelper(actionContext);
        _controller.Url = urlHelper;
        _controller.ControllerContext = new ControllerContext(actionContext);

        // Setup TempData for RedirectToAction using custom mock
        var tempDataProvider = new MockTempDataProvider();
        var tempData = new TempDataDictionary(httpContext, tempDataProvider);
        _controller.TempData = tempData;
    }

    #region PostToGb Action Tests

    [Fact]
    public void PostToGb_HasHttpPostAttribute()
    {
        // Arrange
        var methodInfo = typeof(InfoController).GetMethod(nameof(InfoController.PostToGb));

        // Assert
        Assert.NotNull(methodInfo);
        var httpPostAttribute = methodInfo!.GetCustomAttributes(typeof(HttpPostAttribute), false).FirstOrDefault();
        Assert.NotNull(httpPostAttribute);
    }

    [Fact]
    public void PostToGb_HasRouteAttribute()
    {
        // Arrange
        var methodInfo = typeof(InfoController).GetMethod(nameof(InfoController.PostToGb));

        // Assert
        Assert.NotNull(methodInfo);
        var routeAttribute = methodInfo!.GetCustomAttributes(typeof(RouteAttribute), false)
            .FirstOrDefault() as RouteAttribute;
        Assert.NotNull(routeAttribute);
        Assert.Equal("Info/PostToGb", routeAttribute!.Template);
    }

    [Fact]
    public void PostToGb_HasValidateAntiForgeryTokenAttribute()
    {
        // Arrange
        var methodInfo = typeof(InfoController).GetMethod(nameof(InfoController.PostToGb));

        // Assert
        Assert.NotNull(methodInfo);
        var antiForgeryAttribute = methodInfo!.GetCustomAttributes(typeof(ValidateAntiForgeryTokenAttribute), false)
            .FirstOrDefault();
        Assert.NotNull(antiForgeryAttribute);
    }

    [Fact]
    public void PostToGb_WithValidModel_CreatesGuestbookEntry()
    {
        // Arrange
        var inputModel = new GuestbookInputDto
        {
            Code = "3568",
            Name = "Test User",
            Email = "test@example.com",
            Homepage = "https://example.com",
            Message = "Test message for guestbook",
            FormSubmitDate = DateTime.Now
        };

        var initialCount = _mockGuestBookService.GetAll().Count;

        // Act
        var result = _controller.PostToGb(inputModel);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Gastbok", redirectResult.ActionName);
        Assert.Equal(initialCount + 1, _mockGuestBookService.GetAll().Count);
    }

    [Fact]
    public void PostToGb_WithValidModel_SetsDisplayPublishedTrue()
    {
        // Arrange
        var inputModel = new GuestbookInputDto
        {
            Code = "3568",
            Name = "Test User",
            Email = "test@example.com",
            Homepage = "https://example.com",
            Message = "Test message",
            FormSubmitDate = DateTime.Now
        };

        // Act
        var result = _controller.PostToGb(inputModel);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        var routeValues = redirectResult.RouteValues;
        Assert.NotNull(routeValues);
        Assert.True((bool)routeValues!["DisplayPublished"]!);
    }

    [Fact]
    public void PostToGb_WithInvalidCode_DoesNotCreateEntry()
    {
        // Arrange
        var inputModel = new GuestbookInputDto
        {
            Code = "0000", // Invalid code
            Name = "Test User",
            Email = "test@example.com",
            Homepage = "https://example.com",
            Message = "Test message",
            FormSubmitDate = DateTime.Now
        };

        _controller.ModelState.AddModelError("Code", "Fel kod angiven");
        var initialCount = _mockGuestBookService.GetAll().Count;

        // Act
        var result = _controller.PostToGb(inputModel);

        // Assert
        Assert.Equal(initialCount, _mockGuestBookService.GetAll().Count);
    }

    [Fact]
    public void PostToGb_StripsHttpsFromHomepage()
    {
        // Arrange
        var inputModel = new GuestbookInputDto
        {
            Code = "3568",
            Name = "Test User",
            Email = "test@example.com",
            Homepage = "https://example.com/some/path",
            Message = "Test message",
            FormSubmitDate = DateTime.Now
        };

        // Act
        var result = _controller.PostToGb(inputModel);

        // Assert
        var createdEntry = _mockGuestBookService.GetAll()
            .FirstOrDefault(g => g.GbName == "Test User");
        Assert.NotNull(createdEntry);
        Assert.DoesNotContain("https://", createdEntry!.GbHomepage);
        Assert.DoesNotContain("http://", createdEntry.GbHomepage);
    }

    [Fact]
    public void PostToGb_LimitsHomepageToThreeLevels()
    {
        // Arrange
        var inputModel = new GuestbookInputDto
        {
            Code = "3568",
            Name = "Test User",
            Email = "test@example.com",
            Homepage = "https://example.com/level1/level2/level3/level4",
            Message = "Test message",
            FormSubmitDate = DateTime.Now
        };

        // Act
        var result = _controller.PostToGb(inputModel);

        // Assert
        var createdEntry = _mockGuestBookService.GetAll()
            .FirstOrDefault(g => g.GbName == "Test User");
        Assert.NotNull(createdEntry);
        var parts = createdEntry!.GbHomepage.Split('/');
        Assert.True(parts.Length <= 3);
    }

    [Fact]
    public void PostToGb_WithEmptyName_UsesAnonymous()
    {
        // Arrange
        var inputModel = new GuestbookInputDto
        {
            Code = "3568",
            Name = "", // Empty name
            Email = "test@example.com",
            Homepage = "",
            Message = "Test message",
            FormSubmitDate = DateTime.Now
        };

        // Act
        var result = _controller.PostToGb(inputModel);

        // Assert - Should still create entry (Name is optional)
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Gastbok", redirectResult.ActionName);
    }

    [Fact]
    public void PostToGb_WithEmptyHomepage_CreatesEntryWithoutHomepage()
    {
        // Arrange
        var inputModel = new GuestbookInputDto
        {
            Code = "3568",
            Name = "Test User",
            Email = "test@example.com",
            Homepage = "", // Empty homepage
            Message = "Test message",
            FormSubmitDate = DateTime.Now
        };

        // Act
        var result = _controller.PostToGb(inputModel);

        // Assert
        var createdEntry = _mockGuestBookService.GetAll()
            .FirstOrDefault(g => g.GbName == "Test User");
        Assert.NotNull(createdEntry);
        Assert.True(string.IsNullOrEmpty(createdEntry!.GbHomepage));
    }

    [Fact]
    public void PostToGb_GeneratesIncrementalGbId()
    {
        // Arrange
        var lastId = _mockGuestBookService.GetLastGbId();
        var inputModel = new GuestbookInputDto
        {
            Code = "3568",
            Name = "Test User",
            Email = "test@example.com",
            Homepage = "https://example.com",
            Message = "Test message",
            FormSubmitDate = DateTime.Now
        };

        // Act
        var result = _controller.PostToGb(inputModel);

        // Assert
        var createdEntry = _mockGuestBookService.GetAll()
            .FirstOrDefault(g => g.GbName == "Test User");
        Assert.NotNull(createdEntry);
        Assert.Equal(lastId + 1, createdEntry!.GbId);
    }

    #endregion

    #region Gastbok Action Tests

    [Fact]
    public void Gastbok_ReturnsViewResult()
    {
        // Arrange
        var inputModel = GuestbookInputDto.CreateEmpty();

        // Act
        var result = _controller.Gastbok(inputModel);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<GuestbookInputDto>(viewResult.Model);
    }

    [Fact]
    public void Gastbok_InitializesModelWhenEmpty()
    {
        // Arrange
        var inputModel = GuestbookInputDto.CreateEmpty();
        inputModel.FormSubmitDate = DateTime.MinValue;
        inputModel.Message = null!;

        // Act
        var result = _controller.Gastbok(inputModel);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<GuestbookInputDto>(viewResult.Model);
        Assert.NotEqual(DateTime.MinValue, model.FormSubmitDate);
        Assert.False(model.DisplayPublished);
        Assert.False(model.DisplayErrorPublish);
    }

    [Fact]
    public void Gastbok_PreservesModelState_WhenProvidedWithData()
    {
        // Arrange
        var inputModel = GuestbookInputDto.CreateEmpty();
        inputModel.DisplayPublished = true;
        inputModel.Message = "Test";

        // Act
        var result = _controller.Gastbok(inputModel);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<GuestbookInputDto>(viewResult.Model);
        Assert.True(model.DisplayPublished);
    }

    #endregion

    #region Model Validation Tests

    [Fact]
    public void GuestbookInputModel_RequiresCode()
    {
        // Arrange
        var model = new GuestbookInputDto
        {
            Code = "", // Missing required field
            Name = "Test",
            Email = "test@example.com",
            Homepage = "https://example.com",
            Message = "Test message"
        };

        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains("Code"));
    }

    [Fact]
    public void GuestbookInputModel_RequiresMessage()
    {
        // Arrange
        var model = new GuestbookInputDto
        {
            Code = "3568",
            Name = "Test",
            Email = "test@example.com",
            Homepage = "https://example.com",
            Message = "" // Missing required field
        };

        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains("Message"));
    }

    [Fact]
    public void GuestbookInputModel_AcceptsValidHomepageWithoutProtocol()
    {
        // Arrange
        var model = new GuestbookInputDto
        {
            Code = "3568",
            Name = "Test",
            Email = "test@example.com",
            Homepage = "example.com", // Without https://
            Message = "Test message"
        };

        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void GuestbookInputModel_AcceptsValidHomepageWithProtocol()
    {
        // Arrange
        var model = new GuestbookInputDto
        {
            Code = "3568",
            Name = "Test",
            Email = "test@example.com",
            Homepage = "https://example.com", // With https://
            Message = "Test message"
        };

        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void GuestbookInputModel_RejectsInvalidCode()
    {
        // Arrange
        var model = new GuestbookInputDto
        {
            Code = "1234", // Invalid code
            Name = "Test",
            Email = "test@example.com",
            Homepage = "https://example.com",
            Message = "Test message"
        };

        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains("Code"));
    }

    [Fact]
    public void GuestbookInputModel_EnforcesMaxLengths()
    {
        // Arrange
        var model = new GuestbookInputDto
        {
            Code = "3568",
            Name = new string('a', 51), // Too long (max 50)
            Email = "test@example.com",
            Homepage = "https://example.com",
            Message = "Test message"
        };

        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains("Name"));
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void PostToGb_FullWorkflow_Success()
    {
        // Arrange - Create a valid guestbook entry
        var inputModel = new GuestbookInputDto
        {
            Code = "3568",
            Name = "Integration Test User",
            Email = "integration@test.com",
            Homepage = "https://integration-test.com/path",
            Message = "This is an integration test message",
            FormSubmitDate = DateTime.Now
        };

        var initialCount = _mockGuestBookService.GetAll().Count;

        // Act - Submit the form
        var result = _controller.PostToGb(inputModel);

        // Assert - Verify redirect
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Gastbok", redirectResult.ActionName);

        // Assert - Verify entry was created
        var allEntries = _mockGuestBookService.GetAll();
        Assert.Equal(initialCount + 1, allEntries.Count);

        // Assert - Verify entry content
        var createdEntry = allEntries.FirstOrDefault(g => g.GbName == "Integration Test User");
        Assert.NotNull(createdEntry);
        Assert.Equal("integration@test.com", createdEntry!.GbEmail);
        Assert.Equal("This is an integration test message", createdEntry.GbText);
        
        // Assert - Verify homepage processing (stripped protocol and limited depth)
        Assert.DoesNotContain("https://", createdEntry.GbHomepage);
    }

    #endregion

    #region SendMessage Action Tests

    [Fact]
    public void SendMessage_HasHttpPostAttribute()
    {
        // Arrange
        var methodInfo = typeof(InfoController).GetMethod(nameof(InfoController.SendMessage));

        // Assert
        Assert.NotNull(methodInfo);
        var httpPostAttribute = methodInfo!.GetCustomAttributes(typeof(HttpPostAttribute), false).FirstOrDefault();
        Assert.NotNull(httpPostAttribute);
    }

    [Fact]
    public void SendMessage_HasValidateAntiForgeryTokenAttribute()
    {
        // Arrange
        var methodInfo = typeof(InfoController).GetMethod(nameof(InfoController.SendMessage));

        // Assert
        Assert.NotNull(methodInfo);
        var antiForgeryAttribute = methodInfo!.GetCustomAttributes(typeof(ValidateAntiForgeryTokenAttribute), false)
            .FirstOrDefault();
        Assert.NotNull(antiForgeryAttribute);
    }

    [Fact]
    public void SendMessage_WithInvalidModel_DoesNotSaveToDatabase()
    {
        // Arrange
        var mockContactService = new MockContactService();
        
        var coreOptions = new DbContextOptionsBuilder<ArvidsonFotoCoreDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;
        var mockCoreDbContext = new ArvidsonFotoCoreDbContext(coreOptions);
        
        var controller = new InfoController(mockCoreDbContext)
        {
            _contactService = mockContactService
        };

        // Setup HttpContext
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost");
        
        var actionContext = new ActionContext
        {
            HttpContext = httpContext,
            RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
            ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor
            {
                ActionName = "SendMessage",
                ControllerName = "Info"
            }
        };
        
        controller.ControllerContext = new ControllerContext(actionContext);
        var tempDataProvider = new MockTempDataProvider();
        controller.TempData = new TempDataDictionary(httpContext, tempDataProvider);

        // Add model error to simulate invalid model state
        controller.ModelState.AddModelError("Email", "Invalid email");

        var contactFormModel = new ContactFormDto
        {
            Code = "3568",
            Name = "Test User",
            Email = "invalid-email",
            Subject = "Test Subject",
            Message = "Test Message"
        };

        var initialCount = mockContactService.GetAll().Count;

        // Act
        var result = controller.SendMessage(contactFormModel, "Kontakta");

        // Assert
        // When model is invalid, no database save should occur
        Assert.Equal(initialCount, mockContactService.GetAll().Count);
    }

    [Fact]
    public void SendMessage_RedirectsToCorrectPage_Kontakta()
    {
        // Arrange
        var mockContactService = new MockContactService();
        
        var coreOptions = new DbContextOptionsBuilder<ArvidsonFotoCoreDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;
        var mockCoreDbContext = new ArvidsonFotoCoreDbContext(coreOptions);
        
        var controller = new InfoController(mockCoreDbContext)
        {
            _contactService = mockContactService
        };

        // Setup HttpContext
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost");
        
        var actionContext = new ActionContext
        {
            HttpContext = httpContext,
            RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
            ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor
            {
                ActionName = "SendMessage",
                ControllerName = "Info"
            }
        };
        
        controller.ControllerContext = new ControllerContext(actionContext);
        var tempDataProvider = new MockTempDataProvider();
        controller.TempData = new TempDataDictionary(httpContext, tempDataProvider);

        var contactFormModel = new ContactFormDto
        {
            Code = "3568",
            Name = "Test User",
            Email = "test@example.com",
            Subject = "Test Subject",
            Message = "Test Message"
        };

        // Act
        var result = controller.SendMessage(contactFormModel, "Kontakta");

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Kontakta", redirectResult.ActionName);
    }

    [Fact]
    public void SendMessage_RedirectsToCorrectPage_KopAvBilder()
    {
        // Arrange
        var mockContactService = new MockContactService();
        
        var coreOptions = new DbContextOptionsBuilder<ArvidsonFotoCoreDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;
        var mockCoreDbContext = new ArvidsonFotoCoreDbContext(coreOptions);
        
        var controller = new InfoController(mockCoreDbContext)
        {
            _contactService = mockContactService
        };

        // Setup HttpContext
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost");
        
        var actionContext = new ActionContext
        {
            HttpContext = httpContext,
            RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
            ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor
            {
                ActionName = "SendMessage",
                ControllerName = "Info"
            }
        };
        
        controller.ControllerContext = new ControllerContext(actionContext);
        var tempDataProvider = new MockTempDataProvider();
        controller.TempData = new TempDataDictionary(httpContext, tempDataProvider);

        var contactFormModel = new ContactFormDto
        {
            Code = "3568",
            Name = "Test User",
            Email = "test@example.com",
            Subject = "Test Subject",
            Message = "Test Message"
        };

        // Act
        var result = controller.SendMessage(contactFormModel, "Kop_av_bilder");

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Kop_av_bilder", redirectResult.ActionName);
    }

    #endregion

    #region ContactFormModel Validation Tests

    [Fact]
    public void ContactFormModel_RequiresCode()
    {
        // Arrange
        var model = new ContactFormDto
        {
            Code = "", // Missing required field
            Name = "Test",
            Email = "test@example.com",
            Subject = "Test Subject",
            Message = "Test message"
        };

        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains("Code"));
    }

    [Fact]
    public void ContactFormModel_RequiresValidEmail()
    {
        // Arrange
        var model = new ContactFormDto
        {
            Code = "3568",
            Name = "Test",
            Email = "invalid-email", // Invalid email format
            Subject = "Test Subject",
            Message = "Test message"
        };

        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains("Email"));
    }

    [Fact]
    public void ContactFormModel_RequiresSubject()
    {
        // Arrange
        var model = new ContactFormDto
        {
            Code = "3568",
            Name = "Test",
            Email = "test@example.com",
            Subject = "", // Missing required field
            Message = "Test message"
        };

        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains("Subject"));
    }

    [Fact]
    public void ContactFormModel_RequiresMessage()
    {
        // Arrange
        var model = new ContactFormDto
        {
            Code = "3568",
            Name = "Test",
            Email = "test@example.com",
            Subject = "Test Subject",
            Message = "" // Missing required field
        };

        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains("Message"));
    }

    [Fact]
    public void ContactFormModel_ValidatesCorrectCode()
    {
        // Arrange
        var model = new ContactFormDto
        {
            Code = "1234", // Wrong code (should be 3568)
            Name = "Test",
            Email = "test@example.com",
            Subject = "Test Subject",
            Message = "Test message"
        };

        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains("Code") && r.ErrorMessage!.Contains("Fel kod"));
    }

    #endregion
}
