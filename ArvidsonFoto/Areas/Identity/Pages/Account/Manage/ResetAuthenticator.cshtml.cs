using ArvidsonFoto.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArvidsonFoto.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class ResetAuthenticatorModel(
    UserManager<ArvidsonFotoUser> userManager,
    SignInManager<ArvidsonFotoUser> signInManager,
    ILogger<ResetAuthenticatorModel> logger) : PageModel
{
    private readonly UserManager<ArvidsonFotoUser> _userManager = userManager;
    private readonly SignInManager<ArvidsonFotoUser> _signInManager = signInManager;
    private readonly ILogger<ResetAuthenticatorModel> _logger = logger;

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGet()
    {
        var user = await _userManager.GetUserAsync(User);
        return user == null
            ? NotFound(string.Format(System.Globalization.CultureInfo.CurrentCulture, IdentityResource.Get("Identity.ConfirmEmail.UnableToLoadUserWithID0"), _userManager.GetUserId(User)))
            : Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(string.Format(System.Globalization.CultureInfo.CurrentCulture, IdentityResource.Get("Identity.ConfirmEmail.UnableToLoadUserWithID0"), _userManager.GetUserId(User)));
        }

        _ = await _userManager.SetTwoFactorEnabledAsync(user, false);
        _ = await _userManager.ResetAuthenticatorKeyAsync(user);
        _logger.LogInformation("User with ID '{UserId}' has reset their authentication app key.", user.Id);

        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = IdentityResource.Get("Identity.Manage.ResetAuthenticator.YourAuthenticatorAppKeyHasBeenNewKey");

        return RedirectToPage("./EnableAuthenticator");
    }
}