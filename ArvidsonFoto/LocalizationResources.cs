using System.Globalization;
using System.Resources;

namespace ArvidsonFoto;

public class HomeResource;
public class GalleryResource;
public class SearchResource;
public class InfoResource;
public class LatestResource;
public class SharedResource;

public class ValidationResource
{
    private static readonly ResourceManager Resources = new("ArvidsonFoto.Resources.ValidationResource", typeof(ValidationResource).Assembly);

    private static string Get(string key) => Resources.GetString(key, CultureInfo.CurrentUICulture) ?? key;

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
}
