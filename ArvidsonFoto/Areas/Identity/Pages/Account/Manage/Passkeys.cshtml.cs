using ArvidsonFoto.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Buffers.Text;

namespace ArvidsonFoto.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class PasskeysModel(
    UserManager<ArvidsonFotoUser> userManager,
    SignInManager<ArvidsonFotoUser> signInManager,
    ILogger<PasskeysModel> logger) : PageModel
{
    private const int MaxPasskeyCount = 100;

    private readonly UserManager<ArvidsonFotoUser> _userManager = userManager;
    private readonly SignInManager<ArvidsonFotoUser> _signInManager = signInManager;
    private readonly ILogger<PasskeysModel> _logger = logger;

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
            return NotFound(string.Format(System.Globalization.CultureInfo.CurrentCulture, IdentityResource.Get("Message.090286fde77f"), _userManager.GetUserId(User)));
        }

        CurrentPasskeys = await _userManager.GetPasskeysAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostAddPasskeyAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return NotFound(string.Format(System.Globalization.CultureInfo.CurrentCulture, IdentityResource.Get("Message.090286fde77f"), _userManager.GetUserId(User)));
        }

        CurrentPasskeys = await _userManager.GetPasskeysAsync(user);

        if (!string.IsNullOrEmpty(Input.Error))
        {
            StatusMessage = string.Format(System.Globalization.CultureInfo.CurrentCulture, IdentityResource.Get("Message.43f78154e1d7"), Input.Error);
            return RedirectToPage();
        }

        if (string.IsNullOrEmpty(Input.CredentialJson))
        {
            StatusMessage = IdentityResource.Get("Message.e6dc1288f905");
            return RedirectToPage();
        }

        if (CurrentPasskeys.Count >= MaxPasskeyCount)
        {
            StatusMessage = IdentityResource.Get("Message.4d206b16b187");
            return RedirectToPage();
        }

        var attestationResult = await _signInManager.PerformPasskeyAttestationAsync(Input.CredentialJson);
        if (!attestationResult.Succeeded)
        {
            StatusMessage = string.Format(System.Globalization.CultureInfo.CurrentCulture, IdentityResource.Get("Message.b5a5968995d4"), attestationResult.Failure?.Message);
            return RedirectToPage();
        }

        var addPasskeyResult = await _userManager.AddOrUpdatePasskeyAsync(user, attestationResult.Passkey);
        if (!addPasskeyResult.Succeeded)
        {
            StatusMessage = IdentityResource.Get("Message.b95a62d553a4");
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
            return NotFound(string.Format(System.Globalization.CultureInfo.CurrentCulture, IdentityResource.Get("Message.090286fde77f"), _userManager.GetUserId(User)));
        }

        byte[] credentialIdBytes;
        try
        {
            credentialIdBytes = Base64Url.DecodeFromChars(credentialId);
        }
        catch (FormatException)
        {
            StatusMessage = IdentityResource.Get("Message.ef1195449972");
            return RedirectToPage();
        }

        var result = await _userManager.RemovePasskeyAsync(user, credentialIdBytes);
        if (!result.Succeeded)
        {
            StatusMessage = IdentityResource.Get("Message.d95342f59e6b");
            return RedirectToPage();
        }

        _logger.LogInformation("User '{UserId}' deleted a passkey.", _userManager.GetUserId(User));
        StatusMessage = IdentityResource.Get("Message.3259cdf0ea26");
        return RedirectToPage();
    }
}

public class PasskeyInputModel
{
    public string? CredentialJson { get; set; }
    public string? Error { get; set; }
}