using ArvidsonFoto.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

namespace ArvidsonFoto.Areas.Identity.Pages.Account.Manage;

[Authorize]
public partial class EmailModel(
    UserManager<ArvidsonFotoUser> userManager,
    SignInManager<ArvidsonFotoUser> signInManager,
    IEmailSender emailSender) : PageModel
{
    private readonly UserManager<ArvidsonFotoUser> _userManager = userManager;
    private readonly SignInManager<ArvidsonFotoUser> _signInManager = signInManager;
    private readonly IEmailSender _emailSender = emailSender;

    public string? Username { get; set; }

    public string? Email { get; set; }

    public bool IsEmailConfirmed { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = nameof(IdentityResource.NewEmail), ResourceType = typeof(IdentityResource))]
        public string NewEmail { get; set; } = null!;
    }

    private async Task LoadAsync(ArvidsonFotoUser user)
    {
        var email = await _userManager.GetEmailAsync(user);
        Email = email;

        Input = new InputModel
        {
            NewEmail = email ?? string.Empty,
        };

        IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(string.Format(System.Globalization.CultureInfo.CurrentCulture, IdentityResource.Get("Message.090286fde77f"), _userManager.GetUserId(User)));
        }

        await LoadAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostChangeEmailAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(string.Format(System.Globalization.CultureInfo.CurrentCulture, IdentityResource.Get("Message.090286fde77f"), _userManager.GetUserId(User)));
        }

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }

        var email = await _userManager.GetEmailAsync(user);
        if (Input.NewEmail != email)
        {
            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmailChange",
                pageHandler: null,
                values: new { userId = userId, email = Input.NewEmail, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                Input.NewEmail,
                IdentityResource.Get("Message.0d1fe19967e2"),
                string.Format(System.Globalization.CultureInfo.CurrentCulture, IdentityResource.Get("Message.2b673777cb25"), HtmlEncoder.Default.Encode(callbackUrl!)));

            StatusMessage = IdentityResource.Get("Message.646b05a0b1b1");
            return RedirectToPage();
        }

        StatusMessage = IdentityResource.Get("Message.94a9c4f2e5e4");
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSendVerificationEmailAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(string.Format(System.Globalization.CultureInfo.CurrentCulture, IdentityResource.Get("Message.090286fde77f"), _userManager.GetUserId(User)));
        }

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }

        var userId = await _userManager.GetUserIdAsync(user);
        var email = await _userManager.GetEmailAsync(user) ?? IdentityResource.Get("Message.2bb2fdbbc3d5");
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Page(
            "/Account/ConfirmEmail",
            pageHandler: null,
            values: new { area = "Identity", userId = userId, code = code },
            protocol: Request.Scheme);
        await _emailSender.SendEmailAsync(
            email: email,
            subject: IdentityResource.Get("Message.0d1fe19967e2"),
            htmlMessage: string.Format(System.Globalization.CultureInfo.CurrentCulture, IdentityResource.Get("Message.2b673777cb25"), HtmlEncoder.Default.Encode(callbackUrl!)));

        StatusMessage = IdentityResource.Get("Message.e70468364917");
        return RedirectToPage();
    }
}