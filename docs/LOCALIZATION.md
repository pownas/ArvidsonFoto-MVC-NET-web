# Lokalisering (Localization) - ArvidsonFoto

Detta projekt stöder nu flerspråkighet med svenska (sv-SE) och engelska (en-US).

## Översikt

Lokaliseringen implementeras med hjälp av ASP.NET Core's inbyggda lokaliseringsstöd med `.resx`-filer och `IViewLocalizer`.

## Konfiguration

### Startup.cs
Lokaliseringen konfigureras i `Startup.cs`:

```csharp
// I ConfigureServices:
services.AddLocalization(options => options.ResourcesPath = "Resources");
services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

// I Configure:
var supportedCultures = new[]
{
    new CultureInfo("sv-SE"),
    new CultureInfo("en-US")
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("sv-SE"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});
```

## Mappstruktur

Resursfiler lagras i `Resources/Views/`-mappen och följer samma struktur som views:

```
ArvidsonFoto/
├── Resources/
│   └── Views/
│       ├── Home/
│       │   ├── Index.sv-SE.resx
│       │   └── Index.en-US.resx
│       ├── Shared/
│       │   ├── _Layout.sv-SE.resx
│       │   └── _Layout.en-US.resx
│       └── Info/
│           ├── Gästbok.sv-SE.resx
│           └── Gästbok.en-US.resx
```

## Användning i Views

I alla views kan du använda `@Localizer["Key"]` för att hämta översatta texter:

```html
<h2>@Localizer["WelcomeTitle"]</h2>
<p>@Localizer["IntroText"]</p>
```

`IViewLocalizer` injiceras automatiskt via `_ViewImports.cshtml`:

```csharp
@inject IViewLocalizer Localizer
```

## Språkväxling

Användare kan växla språk via språkväljaren i navigeringsmenyn. Detta sätter en cookie som ASP.NET Core använder för att bestämma vilket språk som ska visas.

### SetLanguage Action (HomeController.cs)

```csharp
[HttpPost]
public IActionResult SetLanguage(string culture, string returnUrl)
{
    Response.Cookies.Append(
        CookieRequestCultureProvider.DefaultCookieName,
        CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
        new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
    );
    return LocalRedirect(returnUrl);
}
```

## Lägga till nya översättningar

### Steg 1: Skapa resursfiler
Skapa `.resx`-filer för varje språk i rätt mapp under `Resources/Views/`.

### Steg 2: Lägg till översättningar
Lägg till nycklar och värden i `.resx`-filerna:

**Svenska (sv-SE):**
```xml
<data name="MyKey" xml:space="preserve">
  <value>Min svenska text</value>
</data>
```

**Engelska (en-US):**
```xml
<data name="MyKey" xml:space="preserve">
  <value>My English text</value>
</data>
```

### Steg 3: Använd i view
```html
@Localizer["MyKey"]
```

## Befintliga översättningar

### Navigation (_Layout.sv-SE.resx / _Layout.en-US.resx)
- `ToggleNavigation` - Toggle menyns synlighet
- `Latest` - Senast/Latest
- `Photographed` - Fotograferad/Photographed
- `Uploaded` - Uppladdad/Uploaded
- `PerCategory` - Per kategori/Per category
- `Info` - Info
- `BuyImages` - Köp av bilder/Buy images
- `Guestbook` - Gästbok/Guestbook
- `ContactInfo` - Kontaktinformation/Contact information
- `AboutMe` - Om mig/About me
- `Sitemap` - Sidkarta/Sitemap
- `Copyright` - Copyright
- `Search` - Sök/Search
- `Language` - Språk/Language
- `Swedish` - Svenska/Swedish
- `English` - Engelska/English

### Kategorier
- `Mammals` - Däggdjur/Mammals
- `Birds` - Fåglar/Birds
- `Insects` - Insekter/Insects
- `Reptiles` - Kräldjur/Reptiles
- `Landscape` - Landskap/Landscape
- `Travel` - Resor/Travel
- `Plants` - Växter/Plants
- `Seasons` - Årstider/Seasons

### Startsida (Index.sv-SE.resx / Index.en-US.resx)
- `MetaDescription` - Meta beskrivning för SEO
- `WelcomeTitle` - Välkommen titel
- `IntroText` - Introduktionstext
- `BuyHighResText` - Text om köp av bilder
- `ContactMeAt` - Kontakta mig på
- `RandomImages` - Slumpmässiga bilder
- `ImageOf` - Bild av

### Gästbok (Gästbok.sv-SE.resx / Gästbok.en-US.resx)
- `Name`, `Email`, `Homepage`, `Message` - Formulärfält
- `SubmitPost` - Skicka knapp
- `MessageSent` - Bekräftelsemeddelande
- `ErrorSending` - Felmeddelande
- `AnonymousUser` - Anonym användare

## Standardspråk

Standardspråket är svenska (`sv-SE`). Om ingen språkpreferens är satt via cookie, används svenska.

## CSS Styling

Språkväljaren har anpassad styling i `wwwroot/css/site.scss`:

```scss
.language-selector {
    select {
        background-color: #333;
        color: #fff;
        border: 1px solid #555;
        // ... mer styling
    }
}
```

## Framtida förbättringar

- Databaskategorier med flerspråksstöd
- Fler språk (t.ex. tyska, franska)
- Admin-gränssnitt för att hantera översättningar
- Automatisk språkdetektering baserat på webbläsarinställningar

## Teknisk information

- **.resx-filer**: XML-baserade resursfiler som innehåller nycklar och översättningar
- **IViewLocalizer**: ASP.NET Core's interface för lokalisering i views
- **RequestLocalizationOptions**: Konfigurerar vilka kulturer som stöds
- **CookieRequestCultureProvider**: Lagrar användarens språkval i en cookie
