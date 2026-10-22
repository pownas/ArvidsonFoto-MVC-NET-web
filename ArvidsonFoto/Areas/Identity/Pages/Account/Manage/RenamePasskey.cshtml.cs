using ArvidsonFoto.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Buffers.Text;
using System.ComponentModel.DataAnnotations;

namespace ArvidsonFoto.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class RenamePasskeyModel(
    UserManager<ArvidsonFotoUser> userManager,
    ILogger<RenamePasskeyModel> logger) : PageModel
{
    private readonly UserManager<ArvidsonFotoUser> _userManager = userManager;
    private readonly ILogger<RenamePasskeyModel> _logger = logger;

    public UserPasskeyInfo? Passkey { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return NotFound(string.Format(System.Globalization.CultureInfo.CurrentCulture, IdentityResource.Get("Identity.ConfirmEmail.UnableToLoadUserWithID0"), _userManager.GetUserId(User)));
        }

        byte[] credentialId;
        try
        {
            credentialId = Base64Url.DecodeFromChars(id);
        }
        catch (FormatException)
        {
            TempData["StatusMessage"] = IdentityResource.Get("Identity.Manage.Passkeys.ErrorTheSpecifiedPasskeyIDHadInvalidFormat");
            return RedirectToPage("./Passkeys");
        }

        Passkey = await _userManager.GetPasskeyAsync(user, credentialId);
        if (Passkey is null)
        {
            TempData["StatusMessage"] = IdentityResource.Get("Identity.Manage.RenamePasskey.ErrorTheSpecifiedPasskeyCouldNotBeFound");
            return RedirectToPage("./Passkeys");
        }

        Input.Name = Passkey.Name ?? string.Empty;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return NotFound(string.Format(System.Globalization.CultureInfo.CurrentCulture, IdentityResource.Get("Identity.ConfirmEmail.UnableToLoadUserWithID0"), _userManager.GetUserId(User)));
        }

        byte[] credentialId;
        try
        {
            credentialId = Base64Url.DecodeFromChars(id);
        }
        catch (FormatException)
        {
            TempData["StatusMessage"] = IdentityResource.Get("Identity.Manage.Passkeys.ErrorTheSpecifiedPasskeyIDHadInvalidFormat");
            return RedirectToPage("./Passkeys");
        }

        var passkey = await _userManager.GetPasskeyAsync(user, credentialId);
        if (passkey is null)
        {
            TempData["StatusMessage"] = IdentityResource.Get("Identity.Manage.RenamePasskey.ErrorTheSpecifiedPasskeyCouldNotBeFound");
            return RedirectToPage("./Passkeys");
        }

        passkey.Name = Input.Name;
        var result = await _userManager.AddOrUpdatePasskeyAsync(user, passkey);
        if (!result.Succeeded)
        {
            TempData["StatusMessage"] = IdentityResource.Get("Identity.Manage.RenamePasskey.ErrorThePasskeyCouldNotBeUpdated");
            return RedirectToPage("./Passkeys");
        }

        _logger.LogInformation("User '{UserId}' renamed a passkey.", _userManager.GetUserId(User));
        TempData["StatusMessage"] = IdentityResource.Get("Identity.Manage.RenamePasskey.PasskeyUpdatedSuccessfully");
        return RedirectToPage("./Passkeys");
    }

    public sealed class InputModel
    {
        [Required]
        [StringLength(200, ErrorMessageResourceName = nameof(IdentityResource.PasskeyNameLength), ErrorMessageResourceType = typeof(IdentityResource))]
        [Display(Name = nameof(IdentityResource.PasskeyName), ResourceType = typeof(IdentityResource))]
        public string Name { get; set; } = "";
    }
}