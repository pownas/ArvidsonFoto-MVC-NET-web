using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArvidsonFoto.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class ShowRecoveryCodesModel : PageModel
{
    [TempData]
    public string[]? RecoveryCodes { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public IActionResult OnGet()
    {
        return RecoveryCodes == null || RecoveryCodes.Length == 0 ? RedirectToPage("./TwoFactorAuthentication") : Page();
    }
}