using System.Buffers.Text;
using System.ComponentModel.DataAnnotations;
using ArvidsonFoto.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArvidsonFoto.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class PasskeysModel : PageModel
{
    private const int MaxPasskeyCount = 100;

    private readonly UserManager<ArvidsonFotoUser> _userManager;
    private readonly SignInManager<ArvidsonFotoUser> _signInManager;
    private readonly ILogger<PasskeysModel> _logger;

    public PasskeysModel(
        UserManager<ArvidsonFotoUser> userManager,
        SignInManager<ArvidsonFotoUser> signInManager,
        ILogger<PasskeysModel> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    public IList<UserPasskeyInfo> CurrentPasskeys { get; set; } = [];

    [TempData]
    public string? StatusMessage { get; set; }

    [BindProperty]
    public PasskeyInputModel Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        CurrentPasskeys = await _userManager.GetPasskeysAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostAddPasskeyAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        CurrentPasskeys = await _userManager.GetPasskeysAsync(user);

        if (!string.IsNullOrEmpty(Input.Error))
        {
            StatusMessage = $"Error: {Input.Error}";
            return RedirectToPage();
        }

        if (string.IsNullOrEmpty(Input.CredentialJson))
        {
            StatusMessage = "Error: The browser did not provide a passkey.";
            return RedirectToPage();
        }

        if (CurrentPasskeys.Count >= MaxPasskeyCount)
        {
            StatusMessage = "Error: You have reached the maximum number of allowed passkeys.";
            return RedirectToPage();
        }

        var attestationResult = await _signInManager.PerformPasskeyAttestationAsync(Input.CredentialJson);
        if (!attestationResult.Succeeded)
        {
            StatusMessage = $"Error: Could not add the passkey: {attestationResult.Failure?.Message}";
            return RedirectToPage();
        }

        var addPasskeyResult = await _userManager.AddOrUpdatePasskeyAsync(user, attestationResult.Passkey);
        if (!addPasskeyResult.Succeeded)
        {
            StatusMessage = "Error: The passkey could not be added to your account.";
            return RedirectToPage();
        }

        _logger.LogInformation("User '{UserId}' successfully added a new passkey.", _userManager.GetUserId(User));

        // Redirect to rename so the user can give the passkey a friendly name
        var credentialIdBase64Url = Base64Url.EncodeToString(attestationResult.Passkey.CredentialId);
        return RedirectToPage("./RenamePasskey", new { id = credentialIdBase64Url });
    }

    public async Task<IActionResult> OnPostDeletePasskeyAsync(string credentialId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        byte[] credentialIdBytes;
        try
        {
            credentialIdBytes = Base64Url.DecodeFromChars(credentialId);
        }
        catch (FormatException)
        {
            StatusMessage = "Error: The specified passkey ID had an invalid format.";
            return RedirectToPage();
        }

        var result = await _userManager.RemovePasskeyAsync(user, credentialIdBytes);
        if (!result.Succeeded)
        {
            StatusMessage = "Error: The passkey could not be deleted.";
            return RedirectToPage();
        }

        _logger.LogInformation("User '{UserId}' deleted a passkey.", _userManager.GetUserId(User));
        StatusMessage = "Passkey deleted successfully.";
        return RedirectToPage();
    }
}

public class PasskeyInputModel
{
    public string? CredentialJson { get; set; }
    public string? Error { get; set; }
}
