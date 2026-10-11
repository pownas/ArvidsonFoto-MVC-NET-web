using ArvidsonFoto.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace ArvidsonFoto.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class SetPasswordModel(
    UserManager<ArvidsonFotoUser> userManager,
    SignInManager<ArvidsonFotoUser> signInManager) : PageModel
{
    private readonly UserManager<ArvidsonFotoUser> _userManager = userManager;
    private readonly SignInManager<ArvidsonFotoUser> _signInManager = signInManager;

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    [TempData]
    public string? StatusMessage { get; set; }

    public class InputModel
    {
        [Required]
        [StringLength(100, ErrorMessageResourceName = nameof(IdentityResource.PasswordLength), ErrorMessageResourceType = typeof(IdentityResource), MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = nameof(IdentityResource.NewPassword), ResourceType = typeof(IdentityResource))]
        public string NewPassword { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = nameof(IdentityResource.ConfirmNewPassword), ResourceType = typeof(IdentityResource))]
        [Compare("NewPassword", ErrorMessageResourceName = nameof(IdentityResource.NewPasswordMismatch), ErrorMessageResourceType = typeof(IdentityResource))]
        public string ConfirmPassword { get; set; } = null!;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(string.Format(System.Globalization.CultureInfo.CurrentCulture, IdentityResource.Get("Message.090286fde77f"), _userManager.GetUserId(User)));
        }

        var hasPassword = await _userManager.HasPasswordAsync(user);

        if (hasPassword)
        {
            return RedirectToPage("./ChangePassword");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(string.Format(System.Globalization.CultureInfo.CurrentCulture, IdentityResource.Get("Message.090286fde77f"), _userManager.GetUserId(User)));
        }

        var addPasswordResult = await _userManager.AddPasswordAsync(user, Input.NewPassword);
        if (!addPasswordResult.Succeeded)
        {
            foreach (var error in addPasswordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }

        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = IdentityResource.Get("Message.933540c3072d");

        return RedirectToPage();
    }
}