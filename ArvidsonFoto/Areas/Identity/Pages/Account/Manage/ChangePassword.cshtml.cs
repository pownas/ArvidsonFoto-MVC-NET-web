using ArvidsonFoto.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace ArvidsonFoto.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class ChangePasswordModel(
    UserManager<ArvidsonFotoUser> userManager,
    SignInManager<ArvidsonFotoUser> signInManager,
    ILogger<ChangePasswordModel> logger) : PageModel
{
    private readonly UserManager<ArvidsonFotoUser> _userManager = userManager;
    private readonly SignInManager<ArvidsonFotoUser> _signInManager = signInManager;
    private readonly ILogger<ChangePasswordModel> _logger = logger;

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    [TempData]
    public string? StatusMessage { get; set; }

    public class InputModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = nameof(IdentityResource.CurrentPassword), ResourceType = typeof(IdentityResource))]
        public string OldPassword { get; set; } = null!;

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
            return NotFound(string.Format(System.Globalization.CultureInfo.CurrentCulture, IdentityResource.Get("Identity.ConfirmEmail.UnableToLoadUserWithID0"), _userManager.GetUserId(User)));
        }

        var hasPassword = await _userManager.HasPasswordAsync(user);
        if (!hasPassword)
        {
            return RedirectToPage("./SetPassword");
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
            return NotFound(string.Format(System.Globalization.CultureInfo.CurrentCulture, IdentityResource.Get("Identity.ConfirmEmail.UnableToLoadUserWithID0"), _userManager.GetUserId(User)));
        }

        var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
        if (!changePasswordResult.Succeeded)
        {
            foreach (var error in changePasswordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }

        await _signInManager.RefreshSignInAsync(user);
        _logger.LogInformation("User changed their password successfully.");
        StatusMessage = IdentityResource.Get("Identity.Manage.ChangePassword.YourPasswordHasBeenChanged");

        return RedirectToPage();
    }
}