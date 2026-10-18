using System.Globalization;
using System.Resources;

namespace ArvidsonFoto;

public class HomeResource;
public class GalleryResource;
public class SearchResource;
public class InfoResource;
public class LatestResource;
public class SharedResource;
public class SiteResource;

public class ValidationResource
{
    private static readonly ResourceManager Resources = new("ArvidsonFoto.Resources.ValidationResource", typeof(ValidationResource).Assembly);

    private static string Get(string key)
    {
        return Resources.GetString(key, CultureInfo.CurrentUICulture) ?? key;
    }

    public static string CodeRequired => Get("Form.Validation.CodeRequired");
    public static string CodeInvalid => Get("Form.Validation.CodeInvalid");
    public static string EmailRequired => Get("Contact.Form.Validation.EmailRequired");
    public static string EmailInvalid => Get("Contact.Form.Validation.EmailInvalid");
    public static string GuestbookEmailInvalid => Get("Guestbook.Form.Validation.EmailInvalid");
    public static string EmailTooLong => Get("Form.Validation.EmailTooLong");
    public static string NameRequired => Get("Contact.Form.Validation.NameRequired");
    public static string NameTooLong => Get("Form.Validation.NameTooLong");
    public static string SubjectRequired => Get("Contact.Form.Validation.SubjectRequired");
    public static string SubjectTooLong => Get("Contact.Form.Validation.SubjectTooLong");
    public static string WebsiteTooLong => Get("Guestbook.Form.Validation.WebsiteTooLong");
    public static string WebsiteInvalid => Get("Guestbook.Form.Validation.WebsiteInvalid");
    public static string MessageRequired => Get("Form.Validation.MessageRequired");
    public static string MessageTooLong => Get("Form.Validation.MessageTooLong");
    public static string AdminImageFilenameRequired => Get("Admin.Image.Validation.FilenameRequired");
    public static string AdminImageFilenameTooShort => Get("Admin.Image.Validation.FilenameTooShort");
    public static string AdminImageDateRequired => Get("Admin.Image.Validation.DateRequired");
    public static string AdminImageDescriptionTooLong => Get("Admin.Image.Validation.DescriptionTooLong");
    public static string AdminFacebookImageRequired => Get("Admin.Facebook.Validation.ImageRequired");
    public static string AdminFacebookImagesTooMany => Get("Admin.Facebook.Validation.ImagesTooMany");
    public static string AdminFacebookMessageRequired => Get("Admin.Facebook.Validation.MessageRequired");
    public static string AdminFacebookMessageTooLong => Get("Admin.Facebook.Validation.MessageTooLong");
    public static string AdminCategoryNameRequired => Get("Admin.Category.Validation.NameRequired");
    public static string AdminCategoryParentRequired => Get("Admin.Category.Validation.ParentRequired");
    public static string AdminImageFilenameLabel => Get("Admin.Image.Form.Label.Filename");
    public static string AdminImageDateLabel => Get("Admin.Image.Form.Label.Date");
    public static string AdminImageDescriptionLabel => Get("Admin.Image.Form.Label.Description");
    public static string AdminImageDescriptionEnLabel => Get("Admin.Image.Form.Label.DescriptionEn");
    public static string AdminFacebookMessageLabel => Get("Admin.Facebook.Form.Label.Message");
}
