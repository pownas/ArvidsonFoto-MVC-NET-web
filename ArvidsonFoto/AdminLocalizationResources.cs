using System.Globalization;
using System.Resources;

namespace ArvidsonFoto;

public class IdentityResource
{
    private static readonly ResourceManager Resources = new("ArvidsonFoto.Resources.IdentityResource", typeof(IdentityResource).Assembly);

    public static string Get(string key) => Resources.GetString(key, CultureInfo.CurrentUICulture) ?? key;

    public static string Email => Get(nameof(Email));
    public static string Password => Get(nameof(Password));
    public static string ConfirmPassword => Get(nameof(ConfirmPassword));
    public static string RememberMe => Get(nameof(RememberMe));
    public static string TwoFactorCode => Get(nameof(TwoFactorCode));
    public static string RememberMachine => Get(nameof(RememberMachine));
    public static string RecoveryCode => Get(nameof(RecoveryCode));
    public static string CurrentPassword => Get(nameof(CurrentPassword));
    public static string NewPassword => Get(nameof(NewPassword));
    public static string ConfirmNewPassword => Get(nameof(ConfirmNewPassword));
    public static string NewEmail => Get(nameof(NewEmail));
    public static string VerificationCode => Get(nameof(VerificationCode));
    public static string PhoneNumber => Get(nameof(PhoneNumber));
    public static string PasskeyName => Get(nameof(PasskeyName));
    public static string PasswordLength => Get(nameof(PasswordLength));
    public static string PasswordMismatch => Get(nameof(PasswordMismatch));
    public static string NewPasswordMismatch => Get(nameof(NewPasswordMismatch));
    public static string PasskeyNameLength => Get(nameof(PasskeyNameLength));
}
public class UploadAdminResource;
public class DevResource;
