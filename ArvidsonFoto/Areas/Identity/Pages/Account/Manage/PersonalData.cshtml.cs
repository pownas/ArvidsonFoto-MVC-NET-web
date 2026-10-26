using ArvidsonFoto.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArvidsonFoto.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class PersonalDataModel(
        UserManager<ArvidsonFotoUser> userManager,
        ILogger<PersonalDataModel> logger) : PageModel
    {
        private readonly UserManager<ArvidsonFotoUser> _userManager = userManager;
        private readonly ILogger<PersonalDataModel> _logger = logger;

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            return user == null
                ? NotFound(string.Format(System.Globalization.CultureInfo.CurrentCulture, IdentityResource.Get("Identity.ConfirmEmail.UnableToLoadUserWithID0"), _userManager.GetUserId(User)))
                : Page();
        }
    }
}