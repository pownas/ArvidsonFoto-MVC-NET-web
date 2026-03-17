using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using ArvidsonFoto.Areas.Identity.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ArvidsonFoto.Views.Shared;

namespace ArvidsonFoto.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly UserManager<ArvidsonFotoUser> _userManager;
    private readonly SignInManager<ArvidsonFotoUser> _signInManager;

    public LoginModel(SignInManager<ArvidsonFotoUser> signInManager,
        UserManager<ArvidsonFotoUser> userManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    [BindProperty]
    public PasskeyLoginInputModel PasskeyInput { get; set; } = null!;

    public IList<AuthenticationScheme> ExternalLogins { get; set; } = [];

    public string? ReturnUrl { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-postadress")]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Lösenord")]
        public string Password { get; set; } = null!;

        [Display(Name = "Kom ihåg inloggningen?")]
        public bool RememberMe { get; set; }
    }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        returnUrl ??= Url.Content("~/");


        var url = Url.ActionContext.HttpContext;
        string? visitedUrl = HttpRequestExtensions.GetRawUrl(url);

        Log.Warning($"A user visited UploadAdmin-login page via URL: {visitedUrl}.");

        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        // Handle passkey sign-in
        if (!string.IsNullOrEmpty(PasskeyInput?.Error))
        {
            ModelState.AddModelError(string.Empty, $"Passkey-fel: {PasskeyInput.Error}");
            return Page();
        }

        if (!string.IsNullOrEmpty(PasskeyInput?.CredentialJson))
        {
            var passkeyResult = await _signInManager.PasskeySignInAsync(PasskeyInput.CredentialJson);
            if (passkeyResult.Succeeded)
            {
                Log.Information("User signed in with a passkey.");
                return LocalRedirect(returnUrl);
            }
            if (passkeyResult.IsLockedOut)
            {
                Log.Warning("User account locked out (passkey sign-in).");
                return RedirectToPage("./Lockout");
            }

            ModelState.AddModelError(string.Empty, "Ogiltig passkey-inloggning.");
            return Page();
        }

        // Handle password sign-in
        if (ModelState.IsValid)
        {
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                Log.Information("User: " + Input.Email + ", logged in.");
                return LocalRedirect(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
            }
            if (result.IsLockedOut)
            {
                Log.Warning("User account locked out.");
                return RedirectToPage("./Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }
        }

        // If we got this far, something failed, redisplay form
        return Page();
    }
}

public class PasskeyLoginInputModel
{
    public string? CredentialJson { get; set; }
    public string? Error { get; set; }
}
