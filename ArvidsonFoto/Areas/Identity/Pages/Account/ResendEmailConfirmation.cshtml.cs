using ArvidsonFoto.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

namespace ArvidsonFoto.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class ResendEmailConfirmationModel(UserManager<ArvidsonFotoUser> userManager, IEmailSender emailSender) : PageModel
{
    private readonly UserManager<ArvidsonFotoUser> _userManager = userManager;
    private readonly IEmailSender _emailSender = emailSender;

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.FindByEmailAsync(Input.Email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, IdentityResource.Get("Identity.Manage.Email.VerificationEmailSentPleaseCheckYourEmail"));
            return Page();
        }

        var userId = await _userManager.GetUserIdAsync(user);
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Page(
            "/Account/ConfirmEmail",
            pageHandler: null,
            values: new { userId = userId, code = code },
            protocol: Request.Scheme);
        await _emailSender.SendEmailAsync(
            Input.Email,
            IdentityResource.Get("Identity.ExternalLogin.ConfirmYourEmail"),
            string.Format(System.Globalization.CultureInfo.CurrentCulture, IdentityResource.Get("Identity.ExternalLogin.PleaseConfirmYourAccountByAHereA"), HtmlEncoder.Default.Encode(callbackUrl!)));

        ModelState.AddModelError(string.Empty, IdentityResource.Get("Identity.Manage.Email.VerificationEmailSentPleaseCheckYourEmail"));
        return Page();
    }
}