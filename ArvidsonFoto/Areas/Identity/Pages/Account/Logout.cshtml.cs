using ArvidsonFoto.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArvidsonFoto.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class LogoutModel(SignInManager<ArvidsonFotoUser> signInManager) : PageModel
{
    private readonly SignInManager<ArvidsonFotoUser> _signInManager = signInManager;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPost(string? returnUrl = null)
    {
        Log.Information("User: " + User?.Identity?.Name + ", logging out.");
        await _signInManager.SignOutAsync();
        Log.Information("User logged out successfully.");
        return returnUrl != null ? LocalRedirect(returnUrl) : RedirectToPage();
    }
}