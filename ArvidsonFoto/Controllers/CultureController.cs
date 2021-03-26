using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ArvidsonFoto.Controllers
{
    [Route("[controller]/[action]")]
    public class CultureController : Controller
    {
        public IActionResult SetCulture(string culture, string redirectUri)
        {
            if (culture is not null)
                HttpContext.Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName, 
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture))
                );
            return LocalRedirect(redirectUri);
        }
    }
}