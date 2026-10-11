using ArvidsonFoto.Areas.Identity.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArvidsonFoto.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class ExternalLoginsModel(
    UserManager<ArvidsonFotoUser> userManager,
    SignInManager<ArvidsonFotoUser> signInManager) : PageModel
{
    private readonly UserManager<ArvidsonFotoUser> _userManager = userManager;
    private readonly SignInManager<ArvidsonFotoUser> _signInManager = signInManager;

    public IList<UserLoginInfo> CurrentLogins { get; set; } = [];

    public IList<AuthenticationScheme> OtherLogins { get; set; } = [];

    public bool ShowRemoveButton { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(string.Format(System.Globalization.CultureInfo.CurrentCulture, IdentityResource.Get("Message.090286fde77f"), _userManager.GetUserId(User)));
        }

        CurrentLogins = await _userManager.GetLoginsAsync(user);
        OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
            .Where(auth => CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
            .ToList();
        ShowRemoveButton = user.PasswordHash != null || CurrentLogins.Count > 1;
        return Page();
    }

    public async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(string.Format(System.Globalization.CultureInfo.CurrentCulture, IdentityResource.Get("Message.090286fde77f"), _userManager.GetUserId(User)));
        }

        var result = await _userManager.RemoveLoginAsync(user, loginProvider, providerKey);
        if (!result.Succeeded)
        {
            StatusMessage = IdentityResource.Get("Message.733f37a8a58f");
            return RedirectToPage();
        }

        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = IdentityResource.Get("Message.806d416d01a5");
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostLinkLoginAsync(string provider)
    {
        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        // Request a redirect to the external login provider to link a login for the current user
        var redirectUrl = Url.Page("./ExternalLogins", pageHandler: "LinkLoginCallback");
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
        return new ChallengeResult(provider, properties);
    }

    public async Task<IActionResult> OnGetLinkLoginCallbackAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(string.Format(System.Globalization.CultureInfo.CurrentCulture, IdentityResource.Get("Message.090286fde77f"), _userManager.GetUserId(User)));
        }

        var info = await _signInManager.GetExternalLoginInfoAsync(user.Id);
        if (info == null)
        {
            throw new InvalidOperationException($"Unexpected error occurred loading external login info for user with ID '{user.Id}'.");
        }

        var result = await _userManager.AddLoginAsync(user, info);
        if (!result.Succeeded)
        {
            StatusMessage = IdentityResource.Get("Message.037378f25df9");
            return RedirectToPage();
        }

        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        StatusMessage = IdentityResource.Get("Message.e7ed04e79aaf");
        return RedirectToPage();
    }
}