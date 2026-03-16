using System.Buffers.Text;
using System.ComponentModel.DataAnnotations;
using ArvidsonFoto.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArvidsonFoto.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class RenamePasskeyModel : PageModel
{
    private readonly UserManager<ArvidsonFotoUser> _userManager;
    private readonly ILogger<RenamePasskeyModel> _logger;

    public RenamePasskeyModel(
        UserManager<ArvidsonFotoUser> userManager,
        ILogger<RenamePasskeyModel> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

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
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        byte[] credentialId;
        try
        {
            credentialId = Base64Url.DecodeFromChars(id);
        }
        catch (FormatException)
        {
            TempData["StatusMessage"] = "Error: The specified passkey ID had an invalid format.";
            return RedirectToPage("./Passkeys");
        }

        Passkey = await _userManager.GetPasskeyAsync(user, credentialId);
        if (Passkey is null)
        {
            TempData["StatusMessage"] = "Error: The specified passkey could not be found.";
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
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        byte[] credentialId;
        try
        {
            credentialId = Base64Url.DecodeFromChars(id);
        }
        catch (FormatException)
        {
            TempData["StatusMessage"] = "Error: The specified passkey ID had an invalid format.";
            return RedirectToPage("./Passkeys");
        }

        var passkey = await _userManager.GetPasskeyAsync(user, credentialId);
        if (passkey is null)
        {
            TempData["StatusMessage"] = "Error: The specified passkey could not be found.";
            return RedirectToPage("./Passkeys");
        }

        passkey.Name = Input.Name;
        var result = await _userManager.AddOrUpdatePasskeyAsync(user, passkey);
        if (!result.Succeeded)
        {
            TempData["StatusMessage"] = "Error: The passkey could not be updated.";
            return RedirectToPage("./Passkeys");
        }

        _logger.LogInformation("User '{UserId}' renamed a passkey.", _userManager.GetUserId(User));
        TempData["StatusMessage"] = "Passkey updated successfully.";
        return RedirectToPage("./Passkeys");
    }

    public sealed class InputModel
    {
        [Required]
        [StringLength(200, ErrorMessage = "Passkey-namn får inte vara längre än {1} tecken.")]
        [Display(Name = "Passkey-namn")]
        public string Name { get; set; } = "";
    }
}
