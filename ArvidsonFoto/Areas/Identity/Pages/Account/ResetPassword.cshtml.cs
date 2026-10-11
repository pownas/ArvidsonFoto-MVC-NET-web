using ArvidsonFoto.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ArvidsonFoto.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class ResetPasswordModel(UserManager<ArvidsonFotoUser> userManager) : PageModel
{
    private readonly UserManager<ArvidsonFotoUser> _userManager = userManager;

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100, ErrorMessageResourceName = nameof(IdentityResource.PasswordLength), ErrorMessageResourceType = typeof(IdentityResource), MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = nameof(IdentityResource.ConfirmPassword), ResourceType = typeof(IdentityResource))]
        [Compare("Password", ErrorMessageResourceName = nameof(IdentityResource.PasswordMismatch), ErrorMessageResourceType = typeof(IdentityResource))]
        public string ConfirmPassword { get; set; } = null!;

        public string Code { get; set; } = null!;
    }

    public IActionResult OnGet(string? code = null)
    {
        if (code == null)
        {
            return BadRequest(IdentityResource.Get("Message.14c54c87ce5e"));
        }
        else
        {
            Input = new InputModel
            {
                Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
            };
            return Page();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.FindByEmailAsync(Input.Email);
        if (user == null)
        {
            // Don't reveal that the user does not exist
            return RedirectToPage("./ResetPasswordConfirmation");
        }

        var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
        if (result.Succeeded)
        {
            return RedirectToPage("./ResetPasswordConfirmation");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return Page();
    }
}