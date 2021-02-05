using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ArvidsonFoto.Data;
using ArvidsonFoto.Models;
using ArvidsonFoto.Services;
using Microsoft.AspNetCore.Mvc;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Serilog;

namespace ArvidsonFoto.Controllers
{
    public class InfoController : Controller
    {
        private ICategoryService _categoryService;
        private IImageService _imageService;
        private IGuestBookService _guestbookService;
        private IPageCounterService _pageCounterService;

        public InfoController(ArvidsonFotoDbContext context)
        {
            _categoryService = new CategoryService(context);
            _imageService = new ImageService(context);
            _guestbookService = new GuestBookService(context);
            _pageCounterService = new PageCounterService(context);
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Info";
            _pageCounterService.AddPageCount("Info");
            return View();
        }

        public IActionResult Gästbok(GuestbookInputModel inputModel)
        {
            ViewData["Title"] = "Gästbok";
            _pageCounterService.AddPageCount("Gästbok");

            if (inputModel.FormSubmitDate < new DateTime(2000, 01, 01) && inputModel.Message is null)
            {
                inputModel = new GuestbookInputModel()
                {
                    FormSubmitDate = DateTime.Now,
                    DisplayPublished = false,
                    DisplayErrorPublish = false
                };
            }

            return View(inputModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult PostToGb([Bind("Code,Name,Email,Homepage,Message,FormSubmitDate")] GuestbookInputModel inputModel)
        {
            if (ModelState.IsValid)
            {
                inputModel.DisplayErrorPublish = false;
                try
                {
                    Log.Information("User trying to post to Guestbook...");

                    string homePage = "";
                    if(inputModel.Homepage is not null) {
                        homePage = inputModel.Homepage.Replace("https://", "");
                        homePage = homePage.Replace("http://", "");
                        string[] splittedHome = homePage.Split("/");
                        if(splittedHome is not null) {
                            homePage = splittedHome[0];
                            if(splittedHome.Length>1) { homePage += "/" + splittedHome[1]; }
                            if (splittedHome.Length>2) { homePage += "/" + splittedHome[2]; }
                        }
                    }

                    TblGb postToPublish = new TblGb()
                    {
                        GbId = (_guestbookService.GetLastGbId() + 1),
                        GbName = inputModel.Name,
                        GbEmail = inputModel.Email,
                        GbHomepage = homePage,
                        GbText = inputModel.Message,
                        GbDate = DateTime.Now
                    };
                    Log.Information("User GuestBook-post: " + postToPublish.GbId + " Name: " + postToPublish.GbName + " Email: " + postToPublish.GbEmail + " Homepage:" + postToPublish.GbHomepage);
                    Log.Information("GB-Message: \n" + postToPublish.GbText);
                    
                    if (_guestbookService.CreateGBpost(postToPublish)) 
                    {
                        Log.Information("GB-post above, published OK.");
                        inputModel = new GuestbookInputModel();
                        inputModel.DisplayPublished = true;
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
                inputModel.DisplayPublished = false;
            }
            return RedirectToAction("Gästbok", inputModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendMessage([Bind("Code,Email,Name,Subject,Message")] ContactFormModel contactFormModel, string Page)
        {
            //bool isValid = ModelState.IsValid;
            //bool isValid = contactFormModel.Validate(); //UPPDATERA!!!
                                                            //bool isValid = editContext.Validate();
                                                            //Behöver validera alla fält i modellen korrekt, att de uppfyller Required status. 
            if (ModelState.IsValid)
            {
                contactFormModel.DisplayErrorSending = false;

                try
                {
                    //Github-secrets, samt miljö variablar som krävs (Se dokumentation i OneDrive -> ArvidsonFoto -> Miljö-variabler) :
                    var svc_mailServer = Environment.GetEnvironmentVariable("ARVIDSONFOTO_MAILSERVER");
                    var svc_smtpadress = Environment.GetEnvironmentVariable("ARVIDSONFOTO_SMTPADRESS");
                    var svc_smtppwd = Environment.GetEnvironmentVariable("ARVIDSONFOTO_SMTPPWD");

                    Log.Information("User trying to send e-mail...");
                    var fromName = Page + "-ArvidsonFoto.se";
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(contactFormModel.Name, contactFormModel.Email));
                    //message.To.Add(new MailboxAddress(fromName, "torbjorn_arvidson@hotmail.com"));
                    message.To.Add(new MailboxAddress(fromName, svc_smtpadress));
                    message.Bcc.Add(new MailboxAddress(fromName, "jonas@arvidsonfoto.se"));
                    message.Subject = "Arvidsonfoto.se/" +Page+ " - " + contactFormModel.Subject;

                    message.Body = new TextPart("plain")
                    {
                        Text = contactFormModel.Message
                    };

                    Log.Information("User message: " + message);

                    using (var client = new SmtpClient())
                    {
                        client.Connect(svc_mailServer, 587, SecureSocketOptions.StartTls); //Kräver @using MailKit.Security;
                        //client.Connect(svc_mailServer, 465, true); //Alternativ anslutning, mindre säker...
                        //client.Connect(svc_mailServer, 587, false); //Alternativ anslutning, mindre säker...

                        // Note: only needed if the SMTP server requires authentication
                        client.Authenticate(svc_smtpadress, svc_smtppwd);
                        client.Send(message);
                        client.Disconnect(true);
                        Log.Information("Email above, sent OK.");
                    }

                    contactFormModel = new ContactFormModel();
                    contactFormModel.DisplayEmailSent = true;
                }
                catch (Exception e)
                {
                    contactFormModel.DisplayErrorSending = true;
                    contactFormModel.DisplayEmailSent = false;
                    Log.Error("Error sending email. Error-message: " + e.Message);
                }
            }
            else
            {
                contactFormModel.DisplayEmailSent = false;
            }

            if (Page.Equals("Kontakta"))
            {
                return RedirectToAction("Kontakta", contactFormModel);
            }
            else if (Page.Equals("Köp_av_bilder"))
            {
                return RedirectToAction("Köp_av_bilder", contactFormModel);
            }
            else
            {
                return RedirectToAction("Kontakta");
            }
        }

        public IActionResult Kontakta(ContactFormModel contactFormModel)
        {
            ViewData["Title"] = "Kontaktinformation";
            _pageCounterService.AddPageCount("Kontaktinformation");

            if (contactFormModel.FormSubmitDate < new DateTime(2000,01,01) && contactFormModel.Message is null)
            {
                contactFormModel = new ContactFormModel() {
                    FormSubmitDate = DateTime.Now,
                    MessagePlaceholder = "Meddelande \n (Skriv gärna vad ni önskar kontakt om)", // \n = newLine
                    DisplayEmailSent = false,
                    DisplayErrorSending = false,
                    ReturnPageUrl = "Kontakta"
                };
            }

            return View(contactFormModel);
        }

        public IActionResult Köp_av_bilder(ContactFormModel contactFormModel, string imgId)
        {
            ViewData["Title"] = "Köp av bilder";
            _pageCounterService.AddPageCount("Köp av bilder");
            if (contactFormModel.FormSubmitDate < new DateTime(2000, 01, 01) && contactFormModel.Message is null)
            {
                contactFormModel = new ContactFormModel()
                {
                    FormSubmitDate = DateTime.Now,
                    MessagePlaceholder = "Meddelande \n (Skriv gärna bildnamn på de bilderna ni är intresserade av)", // \n = newLine
                    DisplayEmailSent = false,
                    DisplayErrorSending = false,
                    ReturnPageUrl = "Köp_av_bilder"
                };

                if(imgId is not null)
                {
                    try
                    {
                        var image = _imageService.GetById(Convert.ToInt32(imgId));
                        var imageArt = _categoryService.GetNameById(image.ImageArt);

                        contactFormModel.Message = "Hej!\nJag är intresserad av att köpa en bild på: " + imageArt + "\n som har bildnamnet: " + image.ImageUrl + ".jpg";
                    }
                    catch (Exception)
                    {

                    }
                }
            }

            return View(contactFormModel);
        }

        public IActionResult Om_mig()
        {
            ViewData["Title"] = "Om mig, Torbjörn Arvidson";
            _pageCounterService.AddPageCount("Om mig");
            return View();
        }

        public IActionResult Sidkarta()
        {
            ViewData["Title"] = "Sidkarta";
            _pageCounterService.AddPageCount("Sidkarta");
            return View();
        }

        public IActionResult Copyright()
        {
            ViewData["Title"] = "Copyright";
            _pageCounterService.AddPageCount("Copyright");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
