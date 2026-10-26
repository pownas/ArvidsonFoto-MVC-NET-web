using System.Globalization;
using ArvidsonFoto.Core.DTOs;
using ArvidsonFoto.Core.ViewModels;

namespace ArvidsonFoto.Tests.Unit.ServiceTests;

public class AdminValidationLocalizationTests
{
    [Theory]
    [InlineData("sv-SE", "Ange filnamn", "Filnamn måste vara minst 2 tecken...", "Du får max ange 150-tecken i detta fältet", "Ange fotodatum")]
    [InlineData("en-US", "Enter a filename.", "Filename must be at least 2 characters long.", "Description must not exceed 150 characters.", "Enter the photo date.")]
    [InlineData("", "Ange filnamn", "Filnamn måste vara minst 2 tecken...", "Du får max ange 150-tecken i detta fältet", "Ange fotodatum")]
    public void ImageValidation_UsesCurrentUiCulture(string culture, string required, string tooShort, string tooLong, string dateRequired)
    {
        WithCulture(culture, () =>
        {
            var input = UploadImageInputDto.CreateEmpty();
            input.ImageDate = DateTime.Today;
            AssertError(input, nameof(input.ImageUrl), required);

            input.ImageUrl = "a";
            input.ImageDescription = new string('a', 151);
            AssertError(input, nameof(input.ImageUrl), tooShort);
            AssertError(input, nameof(input.ImageDescription), tooLong);

            var date = typeof(UploadImageInputDto).GetProperty(nameof(input.ImageDate))!
                .GetCustomAttributes(typeof(RequiredAttribute), false).Cast<RequiredAttribute>().Single();
            Assert.Equal(dateRequired, date.GetValidationResult(null, new ValidationContext(input))!.ErrorMessage);
        });
    }

    [Theory]
    [InlineData("sv-SE", "Välj minst en bild", "Du kan välja max 10 bilder", "Skriv ett meddelande", "Meddelandet får max vara 5000 tecken")]
    [InlineData("en-US", "Select at least one image.", "You can select up to 10 images.", "Write a message.", "Message must not exceed 5000 characters.")]
    [InlineData("", "Välj minst en bild", "Du kan välja max 10 bilder", "Skriv ett meddelande", "Meddelandet får max vara 5000 tecken")]
    public void FacebookValidation_UsesCurrentUiCulture(string culture, string imageRequired, string imagesTooMany, string messageRequired, string messageTooLong)
    {
        WithCulture(culture, () =>
        {
            var input = UploadFacebookInputDto.CreateEmpty();
            AssertError(input, nameof(input.SelectedImageIds), imageRequired);
            AssertError(input, nameof(input.Message), messageRequired);

            input.SelectedImageIds = Enumerable.Range(1, 11).ToList();
            input.Message = new string('a', 5001);
            AssertError(input, nameof(input.SelectedImageIds), imagesTooMany);
            AssertError(input, nameof(input.Message), messageTooLong);
        });
    }

    [Theory]
    [InlineData("sv-SE", "Ange ett kategorinamn", "Välj en kategori att koppla den nya till")]
    [InlineData("en-US", "Enter a category name.", "Select a category to attach the new one to.")]
    [InlineData("", "Ange ett kategorinamn", "Välj en kategori att koppla den nya till")]
    public void CategoryValidation_UsesCurrentUiCulture(string culture, string nameRequired, string parentRequired)
    {
        WithCulture(culture, () =>
        {
            var input = UploadNewCategoryInputDto.CreateEmpty();
            AssertError(input, nameof(input.MenuText), nameRequired);
            AssertError(input, nameof(input.MainMenuId), parentRequired);

            var model = new UploadNewCategoryModel { MenuText = "", MainMenuId = null };
            AssertError(model, nameof(model.MenuText), nameRequired);
            AssertError(model, nameof(model.MainMenuId), parentRequired);
        });
    }

    [Theory]
    [InlineData("sv-SE", "Filnamn", "Fotodatum", "Beskrivning", "Description (English)", "Meddelande")]
    [InlineData("en-US", "Filename", "Photo date", "Description", "Description (English)", "Message")]
    [InlineData("", "Filnamn", "Fotodatum", "Beskrivning", "Description (English)", "Meddelande")]
    public void AdminFormLabels_UseCurrentUiCulture(string culture, string filename, string date, string description, string descriptionEn, string message)
    {
        WithCulture(culture, () =>
        {
            AssertDisplayName<UploadImageInputDto>(nameof(UploadImageInputDto.ImageUrl), filename);
            AssertDisplayName<UploadImageInputDto>(nameof(UploadImageInputDto.ImageDate), date);
            AssertDisplayName<UploadImageInputDto>(nameof(UploadImageInputDto.ImageDescription), description);
            AssertDisplayName<UploadImageInputDto>(nameof(UploadImageInputDto.ImageDescriptionEn), descriptionEn);
            AssertDisplayName<UploadFacebookInputDto>(nameof(UploadFacebookInputDto.Message), message);
        });
    }

    private static void AssertDisplayName<T>(string property, string expected)
    {
        var attribute = typeof(T).GetProperty(property)!
            .GetCustomAttributes(typeof(DisplayAttribute), false).Cast<DisplayAttribute>().Single();
        Assert.Equal(expected, attribute.GetName());
    }

    private static void AssertError(object model, string property, string expected)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, true);
        Assert.Contains(results, result => result.MemberNames.Contains(property) && result.ErrorMessage == expected);
    }

    private static void WithCulture(string culture, Action test)
    {
        var previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = culture.Length == 0 ? CultureInfo.InvariantCulture : CultureInfo.GetCultureInfo(culture);
            test();
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }
}
