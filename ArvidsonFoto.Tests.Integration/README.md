# Integration Tests - ArvidsonFoto

## Översikt

Detta projekt innehåller **integration tests** för ArvidsonFoto-applikationen med fokus på end-to-end testning av HTTP-förfrågningar, routing, formulär och databasoperationer.

## Teknisk Stack

- **.NET 10**
- **MSTest** (Test Framework)
- **WebApplicationFactory** (ASP.NET Core Test Server)
- **AngleSharp** (HTML Parsing)
- **In-Memory Database** (EF Core)

## Skillnad mot Unit Tests

| Aspekt | Unit Tests | Integration Tests |
|--------|------------|-------------------|
| **Scope** | Enskilda klasser/metoder | Hela HTTP-pipeline |
| **Server** | Ingen | Riktig ASP.NET Core test-server |
| **Database** | Mock services | In-memory EF Core database |
| **HTTP** | Nej | Ja (GET, POST, redirects) |
| **HTML** | Nej | Ja (parsing, formulär) |
| **Routing** | Nej | Ja (verifierar faktiska routes) |
| **Anti-forgery** | Nej | Ja (CSRF tokens) |
| **Speed** | Snabbt (~1s) | Långsammare (~3-5s) |

## 📊 Teststatistik

### Gästbok Integration Tests (20 tester)

| Kategori | Antal | Beskrivning |
|----------|-------|-------------|
| **GET Tests** | 5 | Hämta gästbokssida |
| **POST Tests** | 9 | Skicka gästboksinlägg |
| **Route Tests** | 2 | Verifiera routing |
| **Performance** | 2 | Svarstider |
| **Validation** | 2 | Formulärvalidering |

## 🚀 Kör testerna

### Alla integration tests
```bash
dotnet test ArvidsonFoto.Tests.Integration/ArvidsonFoto.Tests.Integration.csproj
```

### Specifik testklass
```bash
dotnet test --filter "FullyQualifiedName~GuestbookIntegrationTests"
```

### Specifikt test
```bash
dotnet test --filter "PostToGb_WithValidData_RedirectsToGastbok"
```

### Med verbose output
```bash
dotnet test --logger "console;verbosity=detailed"
```

## 📁 Projektstruktur

```
ArvidsonFoto.Tests.Integration/
├── ArvidsonFoto.Tests.Integration.csproj
├── ArvidsonFotoWebApplicationFactory.cs      # Custom WebApplicationFactory
├── Controllers/
│   └── GuestbookIntegrationTests.cs          # Gästbok integration tests
├── Helpers/
│   └── HtmlHelpers.cs                        # HTML parsing utilities
└── README.md
```

## 🧪 Test Exempel

### 1. GET Request Test
```csharp
[TestMethod]
public async Task GetGastbok_ReturnsSuccessStatusCode()
{
    // Act
    var response = await _client.GetAsync("/Info/Gastbok");

    // Assert
    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
}
```

### 2. POST med Formulär
```csharp
[TestMethod]
public async Task PostToGb_WithValidData_RedirectsToGastbok()
{
    // Arrange - Hämta formulär och anti-forgery token
    var getResponse = await _client.GetAsync("/Info/Gastbok");
    var document = await HtmlHelpers.GetDocumentAsync(getResponse);

    var formData = HtmlHelpers.CreateFormData(document, new Dictionary<string, string>
    {
        ["Code"] = "3568",
        ["Name"] = "Test User",
        ["Email"] = "test@example.com",
        ["Message"] = "Test message"
    });

    // Act - Posta formulär
    var response = await _client.PostAsync("/Info/PostToGb", 
        new FormUrlEncodedContent(formData));

    // Assert
    Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
}
```

### 3. HTML Parsing
```csharp
[TestMethod]
public async Task GetGastbok_ContainsGuestbookForm()
{
    // Act
    var response = await _client.GetAsync("/Info/Gastbok");
    var document = await HtmlHelpers.GetDocumentAsync(response);

    // Assert
    var form = document.QuerySelector("form[action*='PostToGb']");
    Assert.IsNotNull(form);
}
```

## 🛠️ WebApplicationFactory

`ArvidsonFotoWebApplicationFactory` konfigurerar test-servern:

### Funktioner
- ✅ In-memory database (isolerad för varje test)
- ✅ Seed test data automatiskt
- ✅ Ingen påverkan på riktig databas
- ✅ Snabb setup och teardown

### Användning
```csharp
[ClassInitialize]
public static void ClassInitialize(TestContext context)
{
    _factory = new ArvidsonFotoWebApplicationFactory();
    _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
    {
        AllowAutoRedirect = false // För att testa redirects manuellt
    });
}
```

## 🔧 HtmlHelpers

Utility-klass för HTML-parsing:

### Metoder
- `GetDocumentAsync()` - Parsa HTTP response till HTML dokument
- `GetAntiForgeryToken()` - Extrahera CSRF token från formulär
- `CreateFormData()` - Skapa formulärdata med token
- `ContainsText()` - Kolla om text finns på sidan
- `HasSuccessAlert()` - Kolla om success-meddelande visas
- `HasErrorAlert()` - Kolla om fel-meddelande visas

### Exempel
```csharp
var document = await HtmlHelpers.GetDocumentAsync(response);
var token = HtmlHelpers.GetAntiForgeryToken(document);
bool hasSuccess = HtmlHelpers.HasSuccessAlert(document);
```

## ✅ Vad testerna verifierar

### HTTP & Routing
- ✅ `/Info/Gastbok` är tillgänglig (GET)
- ✅ `/Info/PostToGb` är tillgänglig (POST)
- ✅ `/Info/PostToGb` returnerar 405 vid GET
- ✅ Korrekt redirect efter POST
- ✅ 404-buggen är fixad (explicit route test)

### Formulär & Validering
- ✅ Alla fält finns (Code, Name, Email, Homepage, Message)
- ✅ Anti-forgery token finns och fungerar
- ✅ Validering av obligatoriska fält (Code, Message)
- ✅ Ogiltiga koder avvisas
- ✅ POST utan token ger 400 Bad Request

### Data Processing
- ✅ Giltiga inlägg skapas i databasen
- ✅ Success-meddelande visas efter POST
- ✅ Hemsidor utan `https://` accepteras
- ✅ Långa URL:er trunkeras korrekt
- ✅ Flera submissions fungerar

### Performance
- ✅ GET svarar inom 5 sekunder
- ✅ POST svarar inom 5 sekunder

## 🔒 Säkerhet som testas

1. **CSRF Protection**: Anti-forgery token krävs
2. **Method Validation**: Endast POST tillåts på `/Info/PostToGb`
3. **Input Validation**: Obligatoriska fält verifieras
4. **Code Validation**: Endast korrekt kod (3568) accepteras

## 🐛 Buggar som förhindras

### Original Bug: 404 vid POST
```
❌ Error 404 - Page not found
   Requested URL: ArvidsonFoto.se/Info/PostToGb
```

**Test som fångar detta:**
```csharp
[TestMethod]
public async Task PostToGb_RouteIsAccessible()
{
    var response = await _client.PostAsync("/Info/PostToGb", content);
    
    Assert.AreNotEqual(HttpStatusCode.NotFound, response.StatusCode, 
        "Route /Info/PostToGb should be accessible (not 404)");
}
```

### Andra förhindrade buggar
- ✅ Saknad anti-forgery validation
- ✅ Felaktig HTTP-metod på endpoint
- ✅ Validering av hemsidor
- ✅ Redirect-loopar

## 📈 CI/CD Integration

### GitHub Actions
```yaml
- name: Run Integration Tests
  run: dotnet test ArvidsonFoto.Tests.Integration/ArvidsonFoto.Tests.Integration.csproj --logger "trx;LogFileName=integration-tests.trx"

- name: Upload Test Results
  uses: actions/upload-artifact@v3
  if: always()
  with:
    name: integration-test-results
    path: '**/integration-tests.trx'
```

## 🔄 Jämförelse med Unit Tests

| Test Scenario | Unit Test | Integration Test |
|---------------|-----------|------------------|
| Routing | ❌ Kan ej testa | ✅ Testar faktisk route |
| Controller | ✅ Mock dependencies | ✅ Riktiga dependencies |
| Databas | ✅ Mock service | ✅ In-memory EF Core |
| HTTP | ❌ Ingen HTTP | ✅ Riktig HTTP request/response |
| Forms | ❌ Ingen HTML | ✅ Parsar faktisk HTML |
| Anti-forgery | ❌ Svårt att testa | ✅ Testar CSRF token |
| E2E Flow | ❌ Nej | ✅ Ja |

## 🎯 När använda vilket?

### Unit Tests (xUnit) - ArvidsonFoto.Tests.Unit
- ✅ Testa business logic
- ✅ Validering av modeller
- ✅ Service-lager
- ✅ Snabba tester (~1s för 105 tester)
- ✅ Isolerade komponenter

### Integration Tests (MSTest) - ArvidsonFoto.Tests.Integration
- ✅ End-to-end scenarios
- ✅ HTTP routing
- ✅ Formulär med anti-forgery tokens
- ✅ Redirect-flöden
- ✅ Databas-integration
- ✅ Real-world användning (~3-5s för 20 tester)

## 💡 Best Practices

### 1. Använd ClassInitialize/ClassCleanup
```csharp
[ClassInitialize]
public static void ClassInitialize(TestContext context)
{
    _factory = new ArvidsonFotoWebApplicationFactory();
    _client = _factory.CreateClient();
}

[ClassCleanup]
public static void ClassCleanup()
{
    _client?.Dispose();
    _factory?.Dispose();
}
```

### 2. Hämta alltid anti-forgery token
```csharp
var getResponse = await _client.GetAsync("/Info/Gastbok");
var document = await HtmlHelpers.GetDocumentAsync(getResponse);
var formData = HtmlHelpers.CreateFormData(document, fields);
```

### 3. Disable auto-redirect för explicit testning
```csharp
_client = _factory.CreateClient(new WebApplicationFactoryClientOptions
{
    AllowAutoRedirect = false
});
```

### 4. Testa både success och failure cases
```csharp
[TestMethod]
public async Task PostToGb_WithValidData_Succeeds() { }

[TestMethod]
public async Task PostToGb_WithInvalidCode_Fails() { }
```

## 🚦 Test Coverage

Dessa integration tests kompletterar unit tests för 100% coverage av:
- HTTP routing
- Form submission
- Anti-forgery validation
- Redirect behavior
- Database operations
- Success/error messaging

## 📚 Referenser

- [ASP.NET Core Integration Tests (Official)](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-10.0&pivots=mstest)
- [WebApplicationFactory Documentation](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.testing.webapplicationfactory-1)
- [AngleSharp Documentation](https://anglesharp.github.io/)
- [MSTest Documentation](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-mstest)

---

**Skapad**: 2025-01-15  
**Framework**: MSTest v3.8.3  
**.NET Version**: 10  
**Status**: ✅ Alla tester implementerade och körklara
