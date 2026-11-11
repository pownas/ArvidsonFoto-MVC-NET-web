# Moderniseringsrekommendationer för ArvidsonFoto

> **Historisk ögonblicksbild:** Genomförandestatus och paketversioner på denna sida avser den tidigare .NET 10-kodbasen och är inte aktuella paketförslag. Kontrollera alltid `Directory.Packages.props` och aktuell kod innan du använder rekommendationerna. Projektet riktar sig nu mot .NET 11; se [README](../ReadMe.md) för nuläget.

Detta dokument innehåller rekommendationer för att modernisera kodbasen ytterligare efter migreringen från Startup.cs till Program.cs.

## Sammanfattning av genomförda ändringar

### ✅ Slutförda moderniseringar (v3.10.2)

1. **Program.cs modernisering**
   - Migrerat från IHostBuilder-mönstret till WebApplicationBuilder
   - Konsoliderat Startup.cs och IdentityHostingStartup.cs i en fil
   - Förbättrad felhantering med try-catch och Serilog
   - Tydlig separation mellan service-konfiguration och middleware-pipeline

2. **Projektstruktur**
   - Använder redan .NET 10
   - ImplicitUsings aktiverat
   - Nullable reference types aktiverat
   - LangVersion satt till latest

## Rekommenderade moderniseringar

### 1. Uppgradera bibliotek och dependencies ⚠️ HÖGT PRIORITERAT

#### Aktuella versioner (att överväga)

| Paket | Nuvarande | Senaste | Rekommendation |
|-------|-----------|---------|----------------|
| JavaScriptEngineSwitcher.V8 | 3.29.1 | 3.29.1 | ✅ Aktuell |
| LigerShark.WebOptimizer.Core | 3.0.477 | 3.0.477 | ✅ Aktuell |
| Serilog | 4.3.1-dev | 4.2.0 (stable) | ⚠️ Överväg stable version |
| Microsoft.* | 10.0.1 | 10.0.1 | ✅ Aktuella |

#### Rekommendationer:

```xml
<!-- Överväg att byta från pre-release till stable för Serilog -->
<PackageReference Include="Serilog" Version="4.2.0" />

<!-- Lägg till Serilog.AspNetCore för bättre integration -->
<PackageReference Include="Serilog.AspNetCore" Version="8.0.3" />
```

### 2. File-scoped namespaces 📁 REKOMMENDERAS STARKT

**Nuvarande:**
```csharp
namespace ArvidsonFoto;

public class Program
{
    // kod
}
```

**Moderniserad (file-scoped):**
```csharp
namespace ArvidsonFoto;

public class Program
{
    // kod
}
```

Projektet använder redan file-scoped namespaces! ✅

### 3. Global usings expansion 🌐 BEGRÄNSAD MÖJLIGHET

**Nuvarande GlobalUsings.cs:**
```csharp
global using ArvidsonFoto.Controllers;
global using Microsoft.AspNetCore.Mvc;
global using Serilog;
global using Serilog.Events;
global using System.ComponentModel.DataAnnotations.Schema;
```

**Förbättrad (redan implementerad):**
```csharp
global using ArvidsonFoto.Controllers;
global using Microsoft.AspNetCore.Mvc;
global using Serilog;
global using Serilog.Events;  // Added for better logging
global using System.ComponentModel.DataAnnotations.Schema;
```

**Varför inte fler?**

Projektet har **dubbletter av modellnamn** mellan `ArvidsonFoto.Models` och `ArvidsonFoto.Core.Models`:
- `TblImage` finns i båda
- `TblMenu` finns i båda  
- `TblGb` finns i båda
- `TblPageCounter` finns i båda

Detta gör att vi inte kan inkludera `ArvidsonFoto.Models`, `ArvidsonFoto.Data` eller `ArvidsonFoto.Core.Interfaces` i global usings utan att orsaka ambiguösa referenser.

**Rekommendation för framtiden:**
1. Byt namn på modeller i `ArvidsonFoto.Core.Models` för att undvika konflikter (t.ex. `CoreTblImage`)
2. Eller konsolidera modellerna till en gemensam plats
3. Efter detta kan global usings expanderas enligt ursprungligt förslag

**Fördelar efter konsolidering:**
- Mindre repetitiv kod i varje fil
- Tydligare fokus på affärslogik
- Lättare att läsa och underhålla

### 4. Records för DTOs och Models 📦 HÖG PRIORITET

Många modeller i projektet skulle kunna använda records för immutabilitet och kortare syntax.

**Exempel - nuvarande kod:**
```csharp
public class TblImage
{
    public int Id { get; set; }
    public string ImageUrl { get; set; }
    public string ImageDescription { get; set; }
    // ... fler properties
}
```

**Moderniserad med record (om read-only):**
```csharp
public record TblImageDto(
    int Id,
    string ImageUrl,
    string ImageDescription,
    // ... fler properties
);
```

**Eller record med init-only properties (mer flexibelt):**
```csharp
public record TblImage
{
    public required int Id { get; init; }
    public required string ImageUrl { get; init; }
    public required string ImageDescription { get; init; }
    // ... fler properties
}
```

**Områden att applicera:**
- ViewModels (UploadImageViewModel, UploadNewCategoryModel, etc.)
- DTOs för API-kommunikation
- Konfigurationsobjekt

### 5. Required members och init properties 🔒 HÖG PRIORITET

Projektet har många nullable warnings. Använd `required` keyword för att göra properties obligatoriska.

**Nuvarande (med warnings):**
```csharp
public class UploadImageViewModel
{
    public string SelectedCategory { get; set; }  // CS8618 warning
    public List<string> SubCategories { get; set; }  // CS8618 warning
}
```

**Moderniserad:**
```csharp
public class UploadImageViewModel
{
    public required string SelectedCategory { get; init; }
    public required List<string> SubCategories { get; init; }
}
```

**Fördelar:**
- Eliminerar nullable warnings
- Tydligare kontrakt för vad som krävs
- Init-only properties förhindrar mutation efter skapande

### 6. Primary constructors för services 🏗️ MEDEL PRIORITET

.NET 10 stödjer primary constructors för alla klasser.

**Nuvarande:**
```csharp
public class ImageService : IImageService
{
    private readonly ArvidsonFotoDbContext _context;
    
    public ImageService(ArvidsonFotoDbContext context)
    {
        _context = context;
    }
}
```

**Moderniserad:**
```csharp
public class ImageService(ArvidsonFotoDbContext context) : IImageService
{
    private readonly ArvidsonFotoDbContext _context = context;
}
```

**Eller ännu bättre (direktanvändning):**
```csharp
public class ImageService(ArvidsonFotoDbContext context) : IImageService
{
    public async Task<TblImage?> GetImageByIdAsync(int id)
    {
        return await context.TblImages.FindAsync(id);
    }
}
```

### 7. Pattern matching improvements 🎯 LÅGT-MEDEL PRIORITET

**Nuvarande:**
```csharp
if (image == null)
{
    return NotFound();
}
return View(image);
```

**Moderniserad:**
```csharp
return image switch
{
    null => NotFound(),
    _ => View(image)
};
```

**Eller med property pattern:**
```csharp
if (image is { IsVisible: true, IsDeleted: false })
{
    return View(image);
}
```

### 8. Collection expressions 📊 .NET 10 FEATURE

```csharp
// Nuvarande
var origins = new[] { "https://localhost:5001", "http://localhost:5000" };

// Moderniserad
var origins = ["https://localhost:5001", "http://localhost:5000"];
```

### 9. Async/await modernisering ⚡ MEDEL PRIORITET

**Kontrollera att alla I/O-operationer är async:**

```csharp
// Dåligt
public TblImage GetImageById(int id)
{
    return _context.TblImages.Find(id);
}

// Bra
public async Task<TblImage?> GetImageByIdAsync(int id)
{
    return await _context.TblImages.FindAsync(id);
}
```

### 10. Minimal APIs för enklare endpoints 🚀 VALFRITT

Nuvarande projektet använder Controllers. För enklare API-endpoints, överväg Minimal APIs.

**Exempel:**
```csharp
// I Program.cs ConfigureMiddleware-metoden
app.MapGet("/api/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.MapGet("/api/images/latest/{count:int}", async (
    int count,
    IImageService imageService) =>
{
    var images = await imageService.GetLatestImagesAsync(count);
    return Results.Ok(images);
});
```

### 11. Top-level statements 📝 GENOMFÖRT (med modifikation)

Projektet använder redan en modern approach med `Main`-metod men strukturerad för tydlighet. Detta är en bra balans mellan modern syntax och läsbarhet.

### 12. Logging modernisering med LoggerMessage 📋 HÖG PRIORITET

**Nuvarande:**
```csharp
_logger.LogWarning(
    "Potential SQL injection attempt detected in query parameter '{Key}' from IP {IpAddress}",
    param.Key,
    context.Connection.RemoteIpAddress
);
```

**Moderniserad med source generators:**
```csharp
public static partial class LogMessages
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Warning,
        Message = "Potential SQL injection attempt detected in query parameter '{Key}' from IP {IpAddress}")]
    public static partial void LogSqlInjectionAttempt(
        this ILogger logger,
        string key,
        string ipAddress);
}

// Användning
logger.LogSqlInjectionAttempt(param.Key, context.Connection.RemoteIpAddress?.ToString() ?? "unknown");
```

**Fördelar:**
- Bättre performance (kompileras till direkta anrop)
- Type-safety
- Mindre boilerplate

## Implementeringsplan

### Fas 1: Säkerhet och stabilitet (OMEDELBART)
1. ✅ Migrera Startup.cs till Program.cs
2. Åtgärda nullable reference warnings med `required` keyword
3. Utvärdera Serilog-versionen (stable vs pre-release)

### Fas 2: Kodkvalitet (NÄSTA SPRINT)
1. Konvertera ViewModels till records med init properties
2. Expandera global usings
3. Implementera primary constructors i services
4. Implementera LoggerMessage source generators

### Fas 3: Performance och modernitet (FRAMTIDA)
1. Säkerställ att alla I/O-operationer är async
2. Överväg Minimal APIs för nya endpoints
3. Implementera collection expressions där lämpligt
4. Förbättra pattern matching

### Fas 4: Infrastruktur (VID BEHOV)
1. Integrera .NET Aspire för lokal utveckling
2. Lägg till distributed tracing
3. Implementera health checks
4. Överväg Redis för caching

## Uppskattad påverkan

### Kodkvalitet
- **Läsbarhet**: ⬆️⬆️⬆️ (records, init properties, global usings)
- **Underhåll**: ⬆️⬆️ (mindre boilerplate, tydligare kontrakt)
- **Type-safety**: ⬆️⬆️⬆️ (required members, nullable improvements)

### Performance
- **Runtime**: ⬆️ (LoggerMessage source generators)
- **Memory**: ⬆️ (records kan vara mer minneseffektiva)
- **I/O**: ⬆️⬆️ (korrekt async/await användning)

### Developer Experience
- **Lokal utveckling**: ⬆️⬆️⬆️ (.NET Aspire)
- **Debugging**: ⬆️⬆️ (bättre observability)
- **Onboarding**: ⬆️ (modernare kod lättare att lära)

## Risker och överväganden

### Låg risk
- Global usings expansion
- Init properties
- Primary constructors
- Collection expressions

### Medel risk
- Records (kan kräva migration av befintlig kod)
- LoggerMessage source generators (ny pattern)
- Minimal APIs (arkitekturell förändring)

### Hög risk / Stor förändring
- .NET Aspire (kräver Docker, ny infrastruktur)
- Fullständig async/await refactoring (kan påverka många filer)

## Slutsats

Projektet är redan väl positionerat med .NET 10 och modern projektstruktur. De viktigaste moderniseringarna att fokusera på är:

1. **Åtgärda nullable warnings** (hög påverkan, låg risk)
2. **Implementera records och init properties** (hög påverkan, medel risk)
3. **Expandera global usings** (medel påverkan, låg risk)
4. **Lägg till .NET Aspire** (hög påverkan på DX, medel risk)
5. **LoggerMessage source generators** (medel påverkan, låg risk)

Dessa moderniseringar kommer att förbättra kodkvaliteten, utvecklarupplevelsen och underhållbarheten utan att introducera signifikanta risker.

---

**Senast uppdaterad**: 2025-12-15
**Version**: v3.10.2
**Status**: Startup.cs migration slutförd, ytterligare moderniseringar rekommenderade
