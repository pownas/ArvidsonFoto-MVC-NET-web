# Gästbok (Guestbook) Unit Tests

## Översikt

Dessa enhetstester säkerställer att gästboksfunktionaliteten i `InfoController` fungerar korrekt och inte går sönder vid framtida ändringar. Testerna fokuserar särskilt på att förhindra att POST-endpointen `/Info/PostToGb` slutar fungera.

## Test-täckning

### 🔐 Routing och Attribut-tester (3 tester)

Dessa tester säkerställer att rätt attribut är konfigurerade på `PostToGb`-metoden:

- **PostToGb_HasHttpPostAttribute**: Verifierar att metoden har `[HttpPost]`-attributet
- **PostToGb_HasRouteAttribute**: Verifierar att explicit route `"Info/PostToGb"` är definierad
- **PostToGb_HasValidateAntiForgeryTokenAttribute**: Verifierar CSRF-skydd

**Varför?** Dessa tester förhindrar att routing går sönder (404-fel) vid framtida refaktorering.

### ✅ Funktionalitetstester (8 tester)

Tester som verifierar att gästboksinlägg skapas och hanteras korrekt:

- **PostToGb_WithValidModel_CreatesGuestbookEntry**: Verifierar att giltiga inlägg skapas
- **PostToGb_WithValidModel_SetsDisplayPublishedTrue**: Verifierar framgångsmeddelande
- **PostToGb_WithInvalidCode_DoesNotCreateEntry**: Verifierar att ogiltiga koder stoppas
- **PostToGb_StripsHttpsFromHomepage**: Verifierar att `https://` tas bort från hemsidor
- **PostToGb_LimitsHomepageToThreeLevels**: Verifierar URL-djupsbegränsning
- **PostToGb_WithEmptyName_UsesAnonymous**: Verifierar att tomma namn är tillåtna
- **PostToGb_WithEmptyHomepage_CreatesEntryWithoutHomepage**: Verifierar optional homepage
- **PostToGb_GeneratesIncrementalGbId**: Verifierar ID-generering

### 🎨 View-tester (3 tester)

Tester för `Gastbok`-action som visar formuläret:

- **Gastbok_ReturnsViewResult**: Verifierar att vyn returneras
- **Gastbok_InitializesModelWhenEmpty**: Verifierar att tom modell initieras
- **Gastbok_PreservesModelState_WhenProvidedWithData**: Verifierar att tillstånd bevaras

### 📝 Valideringstester (7 tester)

Tester som verifierar `GuestbookInputModel`-validering:

- **GuestbookInputModel_RequiresCode**: Kod är obligatoriskt
- **GuestbookInputModel_RequiresMessage**: Meddelande är obligatoriskt
- **GuestbookInputModel_AcceptsValidHomepageWithoutProtocol**: `example.com` är giltigt
- **GuestbookInputModel_AcceptsValidHomepageWithProtocol**: `https://example.com` är giltigt
- **GuestbookInputModel_RejectsInvalidCode**: Fel kod (inte 3568) avvisas
- **GuestbookInputModel_EnforcesMaxLengths**: Max-längder följs (namn max 50 tecken)

### 🔗 Integrationstester (1 test)

- **PostToGb_FullWorkflow_Success**: End-to-end test av hela flödet

## Köra testerna

### Kör alla gästbokstester

```bash
dotnet test ArvidsonFoto.Tests.Unit/ArvidsonFoto.Tests.Unit.csproj --filter "FullyQualifiedName~InfoControllerTests"
```

### Kör specifikt test

```bash
dotnet test ArvidsonFoto.Tests.Unit/ArvidsonFoto.Tests.Unit.csproj --filter "FullyQualifiedName~PostToGb_HasRouteAttribute"
```

### Kör alla tester i projektet

```bash
dotnet test ArvidsonFoto.Tests.Unit/ArvidsonFoto.Tests.Unit.csproj
```

## Mock-klasser

Testerna använder följande mock-klasser (inga externa dependencies som Moq):

- **MockGuestBookService**: Simulerar gästboksdatabasen i minnet
- **MockTempDataProvider**: Simulerar TempData för RedirectToAction
- **MockImageService**: Simulerar bildservice
- **MockCategoryService**: Simulerar kategoriservice  
- **MockPageCounterService**: Simulerar räknarservice

## Testresultat

```
Test Run Successful.
Total tests: 21
     Passed: 21
 Total time: ~1.3s
```

## Varför dessa tester är viktiga

### Problem som förhindras:

1. **404-fel vid POST**: Route-testerna fångar om `[Route("Info/PostToGb")]` tas bort
2. **Valideringsfel**: Säkerställer att URL:er med/utan `https://` fungerar
3. **Dataförlust**: Verifierar att data sparas korrekt
4. **Säkerhetsbrister**: Kontrollerar CSRF-skydd

### Regressionsskydd:

Om någon framtida ändring bryter gästboken kommer testerna att misslyckas omedelbart, vilket förhindrar att buggar når produktion.

## Exempel på förhindrade buggar

**Före testerna:**
- ❌ POST till `/Info/PostToGb` gav 404-fel
- ❌ URL:er utan `https://` avvisades

**Efter testerna:**
- ✅ Routing-tester hade fångat 404-problemet direkt
- ✅ Validerings-tester säkerställer flexibel URL-hantering

## Framtida utökning

Förslag på ytterligare tester:

- [ ] XSS-skydd i gästboksmeddelanden
- [ ] SQL-injection försök
- [ ] Samtidiga POST:ar (concurrency)
- [ ] Rate-limiting av gästboksinlägg
- [ ] E-postvalidering edge-cases

## Underhåll

Vid ändringar i gästboksfunktionaliteten:

1. Uppdatera först testerna för nytt beteende
2. Implementera ändringarna
3. Verifiera att alla tester passerar
4. Dokumentera nya test-scenarier här

---

**Skapad**: 2025-01-15  
**Test Framework**: xUnit v2.9.3  
**.NET Version**: .NET 10  
**Total testtid**: ~1.3 sekunder
