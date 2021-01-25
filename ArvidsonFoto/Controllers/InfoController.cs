using ArvidsonFoto.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Serilog;
using ArvidsonFoto.Data;

namespace ArvidsonFoto.Controllers
{
    public class InfoController : Controller
    {
        private IImageService _imageService;
        private ICategoryService _categoryService;

        public InfoController(ArvidsonFotoDbContext context)
        {
            _imageService = new ImageService(context);
            _categoryService = new CategoryService(context);
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Info";
            return View();
        }

        public IActionResult Gästbok()
        {
            ViewData["Title"] = "Gästbok";
            return View();
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
                    var fromName = "Kontaktsidan - ArvidsonFoto.se";
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(contactFormModel.Name, contactFormModel.Email));
                    //message.To.Add(new MailboxAddress(fromName, "torbjorn_arvidson@hotmail.com"));
                    message.To.Add(new MailboxAddress(fromName, svc_smtpadress));
                    message.Bcc.Add(new MailboxAddress(fromName, "jonas@arvidsonfoto.se"));
                    message.Subject = "Kontakt - " + contactFormModel.Subject + " - Arvidsonfoto.se";

                    message.Body = new TextPart("plain")
                    {
                        Text = contactFormModel.Message
                    };

                    using (var client = new SmtpClient())
                    {
                        client.Connect(svc_mailServer, 587, SecureSocketOptions.StartTls); //Kräver @using MailKit.Security;
                        //client.Connect(svc_mailServer, 465, true); //Alternativ anslutning, mindre säker...
                        //client.Connect(svc_mailServer, 587, false); //Alternativ anslutning, mindre säker...

                        // Note: only needed if the SMTP server requires authentication
                        client.Authenticate(svc_smtpadress, svc_smtppwd);
                        Log.Information("message: " + message);
                        client.Send(message);
                        client.Disconnect(true);
                    }

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
            else
            {
                return RedirectToAction("Kontakta");
            }
        }

        public IActionResult Kontakta(ContactFormModel contactFormModel)
        {
            ViewData["Title"] = "Kontaktinformation";

            if (contactFormModel.FormSubmitDate < new DateTime(2000,01,01))
            {
                contactFormModel = new ContactFormModel() {
                    FormSubmitDate = DateTime.UtcNow,
                    MessagePlaceholder = "Meddelande \n(Skriv gärna vad ni önskar kontakt om)", // \n = newLine
                    DisplayEmailSent = false,
                    DisplayErrorSending = false
                };
            }

            return View(contactFormModel);
        }

        public IActionResult Köp_av_bilder(ContactFormModel contactFormModel, string imgId)
        {
            ViewData["Title"] = "Köp av bilder";
            if (contactFormModel.FormSubmitDate < new DateTime(2000, 01, 01))
            {
                contactFormModel = new ContactFormModel()
                {
                    FormSubmitDate = DateTime.UtcNow,
                    MessagePlaceholder = "Meddelande\n(Skriv gärna bildnamn på de bilderna ni är intresserade av)", // \n = newLine
                    DisplayEmailSent = false,
                    DisplayErrorSending = false
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
            return View();
        }

        public IActionResult Sidkarta()
        {
            ViewData["Title"] = "Sidkarta";
            return View();
        }

        public IActionResult Copyright()
        {
            ViewData["Title"] = "Copyright";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
