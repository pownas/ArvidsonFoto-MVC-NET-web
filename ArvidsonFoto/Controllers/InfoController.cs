using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.DTOs;
using ArvidsonFoto.Core.Interfaces;
using ArvidsonFoto.Core.Models;
using ArvidsonFoto.Core.Services;
using ArvidsonFoto.Core.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ArvidsonFoto.Controllers;

public class InfoController : Controller
{
    private readonly ArvidsonFotoCoreDbContext _coreContext;
    private readonly IConfiguration _configuration;
    private readonly SmtpSettings _smtpSettings;
    internal IApiCategoryService _categoryService;
    internal IApiImageService _imageService;
    internal IGuestBookService _guestbookService;
    internal IPageCounterService _pageCounterService;
    internal IContactService _contactService;

    public InfoController(
        ArvidsonFotoCoreDbContext coreContext,
        IConfiguration configuration,
        IOptions<SmtpSettings> smtpSettings,
        ILogger<ApiImageService>? imageLogger = null,
        ILogger<ApiCategoryService>? categoryLogger = null,
        IMemoryCache? memoryCache = null)
    {
        _coreContext = coreContext;
        _configuration = configuration;
        _smtpSettings = smtpSettings.Value;
        
        // Support both DI and manual instantiation (for tests)
        _categoryService = new ApiCategoryService(
            categoryLogger ?? LoggerFactory.Create(b => b.AddConsole()).CreateLogger<ApiCategoryService>(),
            _coreContext,
            memoryCache ?? new MemoryCache(new MemoryCacheOptions()));
            
        _imageService = new ApiImageService(
            imageLogger ?? LoggerFactory.Create(b => b.AddConsole()).CreateLogger<ApiImageService>(),
            _coreContext,
            configuration ?? new ConfigurationBuilder().Build(),
            _categoryService);
            
        _guestbookService = new GuestBookService(_coreContext);
        _pageCounterService = new PageCounterService(_coreContext);
        _contactService = new ContactService(_coreContext);
    }

    public IActionResult Index()
    {
        ViewData["Title"] = "Info";
        if (User?.Identity?.IsAuthenticated is false)
            _pageCounterService.AddPageCount("Info");
        return View();
    }

    public IActionResult Gastbok(GuestbookFormInputDto inputModel)
    {
        ViewData["Title"] = "Gästbok";
        if (User?.Identity?.IsAuthenticated is false)
            _pageCounterService.AddPageCount("Gästbok");

        if (inputModel.FormSubmitDate < new DateTime(2000, 01, 01) && inputModel.Message is null)
        {
            inputModel = GuestbookFormInputDto.CreateEmpty();
        }

        return View(inputModel);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Route("Info/PostToGb")]
    public IActionResult PostToGb([Bind("Code,Name,Email,Homepage,Message,FormSubmitDate")] GuestbookFormInputDto inputModel)
    {
        Log.Information("A user trying to post to the Guestbook...");
        if (ModelState.IsValid)
        {
            inputModel.DisplayErrorPublish = false;
            try
            {
                string? homePage = null;
                if (!string.IsNullOrWhiteSpace(inputModel.Homepage))
                {
                    homePage = inputModel.Homepage.Replace("https://", "");
                    homePage = homePage.Replace("http://", "");
                    string[] splittedHome = homePage.Split("/");
                    if (splittedHome is not null)
                    {
                        homePage = splittedHome[0];
                        if (splittedHome.Length > 1) { homePage += "/" + splittedHome[1]; }
                        if (splittedHome.Length > 2) { homePage += "/" + splittedHome[2]; }
                    }
                }

                TblGb postToPublish = new()
                {
                    GbId = (_guestbookService.GetLastGbId() + 1),
                    GbName = string.IsNullOrWhiteSpace(inputModel.Name) ? null : inputModel.Name,
                    GbEmail = string.IsNullOrWhiteSpace(inputModel.Email) ? null : inputModel.Email,
                    GbHomepage = homePage,
                    GbText = inputModel.Message,
                    GbDate = DateTime.Now
                };
                Log.Fatal("User GuestBook-post: " + postToPublish.GbId + " Name: " + postToPublish.GbName + " Email: " + postToPublish.GbEmail + " Homepage:" + postToPublish.GbHomepage);
                Log.Fatal("GB-Message: \n" + postToPublish.GbText);

                if (_guestbookService.CreateGBpost(postToPublish))
                {
                    Log.Information("GB-post, published OK.");
                    inputModel = new GuestbookFormInputDto
                    {
                        Code = string.Empty,
                        Name = string.Empty,
                        Email = string.Empty,
                        Homepage = string.Empty,
                        Message = string.Empty,
                        FormSubmitDate = DateTime.Now,
                        DisplayPublished = true,
                        DisplayErrorPublish = false
                    };
                }
            }
            catch (Exception e)
            {
                inputModel.DisplayErrorPublish = true;
                inputModel.DisplayPublished = false;
                Log.Error("Error publishing the GB-post. Error-message: " + e.Message);
            }
        }
        else
        {
            Log.Fatal($"Name: '{inputModel.Name}' Email: '{inputModel.Email}' Homepage: '{inputModel.Homepage}'");
            Log.Fatal($"GB-Message:\n {inputModel.Message}");
            Log.Warning($"Failed to send GB-post... Probably an incorrect Code-input: '{inputModel.Code}'.");
            inputModel.DisplayPublished = false;
        }
        return RedirectToAction("Gastbok", inputModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SendMessage([Bind("Code,Email,Name,Subject,Message")] ContactFormInputDto contactFormModel, string Page)
    {
        if (ModelState.IsValid)
        {
            contactFormModel.DisplayErrorSending = false;
            bool emailSent = false;
            string? errorMessage = null;
            int? savedContactId = null;

            // STEP 1: Save to database FIRST (as backup)
            try
            {
                Log.Information("Saving contact form submission to database...");
                
                var kontaktRecord = new TblKontakt
                {
                    SubmitDate = DateTime.Now,
                    Name = contactFormModel.Name,
                    Email = contactFormModel.Email,
                    Subject = contactFormModel.Subject,
                    Message = contactFormModel.Message,
                    SourcePage = Page,
                    EmailSent = false, // Will be updated after email attempt
                    ErrorMessage = null
                };

                savedContactId = _contactService.SaveContactSubmission(kontaktRecord);
                Log.Information("Contact form saved to database with ID: {ContactId}", savedContactId);
            }
            catch (Exception dbEx)
            {
                Log.Error(dbEx, "Failed to save contact form to database");
                errorMessage = "Database error: " + dbEx.Message;
                contactFormModel.DisplayErrorSending = true;
                contactFormModel.DisplayEmailSent = false;
                
                // Early return if we can't even save to database
                TempData["DisplayEmailSent"] = false;
                TempData["DisplayErrorSending"] = true;
                return RedirectToActionBasedOnPage(Page);
            }

            // STEP 2: Try to send email
            try
            {
                // Validate SMTP configuration
                if (!_smtpSettings.IsConfigured())
                {
                    throw new InvalidOperationException("SMTP settings are not properly configured. Check appsettings.json or User Secrets.");
                }

                Log.Information("User trying to send e-mail from {SourcePage}...", Page);
                
                var fromName = Page + "-ArvidsonFoto.se";
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(contactFormModel.Name, contactFormModel.Email));
                message.To.Add(new MailboxAddress(fromName, _smtpSettings.SenderEmail));
                if (!string.IsNullOrWhiteSpace(_smtpSettings.CcEmail))
                {
                    message.Cc.Add(new MailboxAddress(fromName, _smtpSettings.CcEmail));
                }
                message.Bcc.Add(new MailboxAddress(fromName, "jonas@arvidsonfoto.se"));
                message.Subject = "Arvidsonfoto.se/" + Page + " - " + contactFormModel.Subject;

                message.Body = new TextPart("plain")
                {
                    Text = contactFormModel.Message
                };

                Log.Information("Sending email via SMTP server: {SmtpServer}", _smtpSettings.Server);

                using (var client = new SmtpClient())
                {
                    client.Connect(_smtpSettings.Server, _smtpSettings.Port, 
                        _smtpSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
                    client.Authenticate(_smtpSettings.SenderEmail, _smtpSettings.SenderPassword);
                    client.Send(message);
                    client.Disconnect(true);
                    Log.Information("Email sent successfully!");
                    emailSent = true;
                }

                contactFormModel = new ContactFormInputDto()
                {
                    Code = string.Empty,
                    Email = string.Empty,
                    Name = string.Empty,
                    Subject = string.Empty,
                    Message = string.Empty,
                    MessagePlaceholder = string.Empty,
                    DisplayEmailSent = true,
                    FormSubmitDate = DateTime.Now,
                    ReturnPageUrl = Page
                };
            }
            catch (Exception emailEx)
            {
                contactFormModel.DisplayErrorSending = true;
                contactFormModel.DisplayEmailSent = false;
                errorMessage = emailEx.Message;
                Log.Error(emailEx, "Error sending email from contact form");
            }

            // STEP 3: Update database record with email status
            if (savedContactId.HasValue)
            {
                try
                {
                    _contactService.UpdateEmailStatus(savedContactId.Value, emailSent, errorMessage);
                    Log.Information("Updated contact record {ContactId} - EmailSent: {EmailSent}", savedContactId, emailSent);
                }
                catch (Exception updateEx)
                {
                    Log.Error(updateEx, "Failed to update email status for contact ID: {ContactId}", savedContactId);
                }
            }
        }
        else
        {
            contactFormModel.DisplayEmailSent = false;
        }

        // Store display flags in TempData for the redirect
        TempData["DisplayEmailSent"] = contactFormModel.DisplayEmailSent;
        TempData["DisplayErrorSending"] = contactFormModel.DisplayErrorSending;

        return RedirectToActionBasedOnPage(Page);
    }

    /// <summary>
    /// Redirects to the appropriate action based on the source page
    /// </summary>
    private IActionResult RedirectToActionBasedOnPage(string page)
    {
        return page switch
        {
            "Kontakta" => RedirectToAction("Kontakta"),
            "Kop_av_bilder" => RedirectToAction("Kop_av_bilder"),
            _ => RedirectToAction("Kontakta")
        };
    }

    public IActionResult Kontakta(ContactFormInputDto contactFormModel)
    {
        ViewData["Title"] = "Kontaktinformation";
        if (User?.Identity?.IsAuthenticated is false)
            _pageCounterService.AddPageCount("Kontaktinformation");

        // Retrieve display flags from TempData
        if (TempData["DisplayEmailSent"] is bool displayEmailSent)
        {
            contactFormModel.DisplayEmailSent = displayEmailSent;
            contactFormModel.FormSubmitDate = DateTime.Now;
        }
        if (TempData["DisplayErrorSending"] is bool displayErrorSending)
        {
            contactFormModel.DisplayErrorSending = displayErrorSending;
            contactFormModel.FormSubmitDate = DateTime.Now;
        }

        if (contactFormModel.FormSubmitDate < new DateTime(2000, 01, 01) && contactFormModel.Message is null)
        {
            contactFormModel = new ContactFormInputDto()
            {
                Code = string.Empty,
                Email = string.Empty,
                Name = string.Empty,
                Subject = string.Empty,
                Message = string.Empty,
                FormSubmitDate = DateTime.Now,
                MessagePlaceholder = "Meddelande \n (Skriv gärna vad ni önskar kontakt om)", // \n = newLine
                DisplayEmailSent = false,
                DisplayErrorSending = false,
                ReturnPageUrl = "Kontakta"
            };
        }
        else
        {
            contactFormModel.ReturnPageUrl = "Kontakta";
        }

        return View(contactFormModel);
    }

    public IActionResult Kop_av_bilder(ContactFormInputDto contactFormModel, string imgId)
    {
        ViewData["Title"] = "Köp av bilder";
        if (User?.Identity?.IsAuthenticated is false)
            _pageCounterService.AddPageCount("Köp av bilder");
        
        // Retrieve display flags from TempData
        if (TempData["DisplayEmailSent"] is bool displayEmailSent)
        {
            contactFormModel.DisplayEmailSent = displayEmailSent;
            contactFormModel.FormSubmitDate = DateTime.Now;
        }
        if (TempData["DisplayErrorSending"] is bool displayErrorSending)
        {
            contactFormModel.DisplayErrorSending = displayErrorSending;
            contactFormModel.FormSubmitDate = DateTime.Now;
        }
        
        if (contactFormModel.FormSubmitDate < new DateTime(2000, 01, 01) && contactFormModel.Message is null)
        {
            contactFormModel = new ContactFormInputDto()
            {
                Code = string.Empty,
                Email = string.Empty,
                Name = string.Empty,
                Subject = string.Empty,
                Message = string.Empty,
                FormSubmitDate = DateTime.Now,
                MessagePlaceholder = "Meddelande \n (Skriv gärna bildnamn på de bilderna ni är intresserade av)", // \n = newLine
                DisplayEmailSent = false,
                DisplayErrorSending = false,
                ReturnPageUrl = "Kop_av_bilder"
            };

            if (imgId is not null)
            {
                try
                {
                    var image = _imageService.GetById(Convert.ToInt32(imgId));
                    var imageArt = image.Name ?? "okänd kategori";

                    contactFormModel.Message = "Hej!\nJag är intresserad av att köpa en bild på: " + imageArt + "\n som har bildnamnet: " + image.UrlImage.Split('/').Last() + ".jpg";
                }
                catch (Exception)
                {
                    // Ignore errors when trying to pre-fill message
                }
            }
        }
        else
        {
            contactFormModel.ReturnPageUrl = "Kop_av_bilder";
        }

        return View(contactFormModel);
    }

    public IActionResult Om_mig()
    {
        ViewData["Title"] = "Om mig, Torbjörn Arvidson";
        if (User?.Identity?.IsAuthenticated is false)
            _pageCounterService.AddPageCount("Om mig");
        return View();
    }

    public IActionResult Sidkarta()
    {
        ViewData["Title"] = "Sidkarta";
        if (User?.Identity?.IsAuthenticated is false)
            _pageCounterService.AddPageCount("Sidkarta");
        return View();
    }

    public IActionResult Copyright()
    {
        ViewData["Title"] = "Copyright";
        if (User?.Identity?.IsAuthenticated is false)
            _pageCounterService.AddPageCount("Copyright");
        return View();
    }
}