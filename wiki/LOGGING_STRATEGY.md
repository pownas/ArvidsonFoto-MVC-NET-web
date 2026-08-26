# Loggningsstrategi för ArvidsonFoto

## Översikt

Detta dokument beskriver loggningsstrategin för ArvidsonFoto-applikationen, inklusive vilka loggningsnivåer som ska användas i olika miljöer och situationer.

## Loggningsnivåer

### Trace (mest detaljerad)
**Användning:** Extremt detaljerad information för djup felsökning  
**När:** Aldrig i produktion, endast vid specifik felsökning  
**Exempel:** Enskilda loop-iterationer, varje databas-query parameter

### Debug
**Användning:** Detaljerad information användbar för utveckling och felsökning  
**När:** Development-miljö, tillfälligt i staging för felsökning  
**Exempel:**
```csharp
Log.Debug("Category path retrieved from cache: {CategoryPath}", categoryPath);
Log.Debug("Bulk loading {Count} category paths", categoryIds.Count);
```

### Information
**Användning:** Generell informationsflöde och viktiga affärshändelser  
**När:** Alla miljöer, för att spåra viktiga händelser  
**Exempel:**
```csharp
Log.Information("User submitted guestbook entry with ID: {GbId}", gbPost.GbId);
Log.Information("Contact form saved to database with ID: {ContactId}", savedContactId);
Log.Information("Starting web application in {Environment} mode", environment);
```

### Warning
**Användning:** Ovanliga eller potentiellt problematiska händelser  
**När:** Alla miljöer  
**Exempel:**
```csharp
Log.Warning("Could not find category: '{CategoryName}'", categoryName);
Log.Warning("Failed to send GB-post. Incorrect Code input: '{Code}'", inputModel.Code);
Log.Warning("Potential SQL injection attempt detected in parameter '{Key}' from IP {IpAddress}", param.Key, ipAddress);
```

### Error
**Användning:** Fel som påverkar specifik operation men inte hela applikationen  
**När:** Alla miljöer  
**Exempel:**
```csharp
Log.Error("Error in GetImageById: {Id} {Message}", imageId, ex.Message);
Log.Error("Failed to save contact form to database: {Error}", dbEx.Message);
```

### Fatal (mest allvarlig)
**Användning:** Kritiska fel som kräver omedelbar uppmärksamhet  
**När:** Alla miljöer  
**Exempel:**
```csharp
Log.Fatal("Application terminated unexpectedly", ex);
Log.Fatal("Database connection failed on startup", ex);
```

## Miljöspecifika Konfigurationer

### Production (`appsettings.Production.json`)

**Filosofi:** Minimal loggning för prestanda och diskutrymme

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Warning",
      "Microsoft.EntityFrameworkCore": "Error",
      "ArvidsonFoto": "Information",
      "ArvidsonFoto.Controllers": "Warning",
      "ArvidsonFoto.Core.Services": "Warning"
    }
  }
}
```

**Vad loggas:**
- ✅ Säkerhetsvarningar (SQL injection attempts)
- ✅ Kritiska fel
- ✅ Viktiga affärshändelser (gästbok, kontaktformulär)
- ❌ Debug-information
- ❌ Routine operations
- ❌ Cache hits/misses

### Development (`appsettings.Development.json`)

**Filosofi:** Verbose loggning för felsökning

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Information",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information",
      "ArvidsonFoto": "Debug"
    }
  }
}
```

**Vad loggas:**
- ✅ Allt från Production
- ✅ Debug-information
- ✅ Database queries (SQL)
- ✅ Cache operations
- ✅ Detailed flow information

### Default (`appsettings.json`)

**Filosofi:** Balanserad loggning för ospecificerade miljöer

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "ArvidsonFoto": "Information"
    }
  }
}
```

## Kodexempel och Riktlinjer

### ✅ BRA: Använd Debug för rutin-operationer

```csharp
// Cachning
Log.Debug("All categories retrieved from cache ({Count} categories)", cachedCategories.Count);

// Routine operations
Log.Debug("Image path built: {Path}", imagePath);

// Performance metrics
Log.Debug("GetCategoryPathsBulk completed in {ElapsedMs}ms", elapsed);
```

### ✅ BRA: Använd Information för affärshändelser

```csharp
// User actions
Log.Information("User submitted guestbook entry: {Name}", postToPublish.GbName);

// System state changes
Log.Information("Category cache cleared - {Reason}", reason);

// Application lifecycle
Log.Information("Starting web application in {Environment} mode", environment);
```

### ✅ BRA: Använd Warning för ovanliga situationer

```csharp
// Not found scenarios
Log.Warning("Could not find category with ID: {CategoryId}", categoryId);

// Validation failures
Log.Warning("Invalid category ID provided: {Id}", id);

// Security events
Log.Warning("Potential SQL injection attempt from IP {IpAddress}", ipAddress);
```

### ❌ DÅLIGT: Använd INTE Fatal för routine errors

```csharp
// FÖRE (för aggressivt)
Log.Fatal("Redirect from page: {Url}, to page: /Senast/Fotograferad", url);

// EFTER (korrekt nivå)
Log.Information("Redirect from page: {Url}, to page: /Senast/Fotograferad", url);
```

### ❌ DÅLIGT: Använd INTE Information för routine operations

```csharp
// FÖRE (för verbose)
Log.Information("Category {Id} path retrieved", categoryId);

// EFTER (korrekt nivå)
Log.Debug("Category {Id} path retrieved", categoryId);
```

## Rekommenderade Ändringar per Fil

### ApiCategoryService.cs
- ✅ `GetAll()` - cache hits: Information → Debug
- ✅ `GetById()` - lookups: Information → Debug  
- ✅ `BuildAndCacheCategoryPath()` - operations: Ta bort eller → Debug

### InfoController.cs
- ✅ Guestbook submissions: Information (behåll)
- ✅ Contact form submissions: Information (behåll)
- ✅ Email sent: Information (behåll)
- ✅ Validation errors: Warning (behåll)

### RedirectRouterController.cs
- ❌ Alla `Log.Fatal()` redirects → `Log.Information()` eller ta bort helt
- ✅ Behåll endast för verkligt kritiska fel

### SenastController.cs
- ✅ Redirects: Ta bort eller → Debug
- ✅ Page counts: Ta bort eller → Debug

## Performance Impact

### Production med Warning-nivå
- **Diskutrymme:** ~10-50 MB/dag (beroende på trafik)
- **CPU overhead:** <1%
- **I/O overhead:** Minimal

### Development med Debug-nivå
- **Diskutrymme:** ~100-500 MB/dag
- **CPU overhead:** 1-3%
- **I/O overhead:** Märkbar men acceptabel

## Loggrotation

Serilog konfigureras med daglig rotation:

```csharp
.WriteTo.File("logs\\appLog.txt", rollingInterval: RollingInterval.Day)
```

**Rekommendation för produktion:**
- Behåll loggar i 30 dagar
- Arkivera äldre loggar till Azure Blob Storage eller S3
- Komprimera arkiverade loggar

## Monitoring och Alerts

### Production Alerts (rekommenderas)

**Kritisk Priority:**
- Fatal logs → omedelbar notification
- Error rate > 10/minut → notification inom 5 min

**Hög Priority:**
- Warning rate > 50/minut → notification inom 15 min
- SQL injection attempts → daily summary

**Normal Priority:**
- Daily log summary → email nästa morgon

### Tools (förslag)

- **Local:** Serilog File Sink + manuell granskning
- **Production:** 
  - Azure Application Insights (om Azure)
  - Seq (lokal eller self-hosted)
  - Elastic Stack (ELK)
  - Sentry (för errors)

## Console Logging

### Development
Console logging är aktiverat för enklare debugging:

```csharp
if (isDevelopment)
{
    loggerConfig.WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
}
```

### Production
Console logging är **inaktiverat** för prestanda.

## Structured Logging

Använd alltid structured logging med parametrar:

```csharp
// ✅ BRA
Log.Information("User {UserId} submitted form with {ItemCount} items", userId, items.Count);

// ❌ DÅLIGT
Log.Information($"User {userId} submitted form with {items.Count} items");
```

**Fördelar:**
- Enklare att söka och filtrera
- Bättre prestanda
- Fungerar bättre med log aggregation tools

## Sammanfattning

### Nyckelprinciper

1. **Produktion är tyst** - Endast Warning och högre som standard
2. **Development är verbose** - Debug-nivå för full insikt
3. **Affärshändelser är Information** - Användareaktioner loggas alltid
4. **Routine operations är Debug** - Cache, lookups, etc.
5. **Fatal är verkligen Fatal** - Endast för kritiska systemfel

### Maintenance Checklist

- [ ] Granska loggar varje sprint för att identifiera "chatty" kod
- [ ] Konvertera Information → Debug för routine operations
- [ ] Konvertera Fatal → Information för non-critical events
- [ ] Testa loggvolym i staging innan production deploy
- [ ] Verifiera att inga känsliga data loggas (lösenord, tokens, etc.)

---

**Senast uppdaterad:** 2025-12-30  
**Version:** 1.0  
**Ansvarig:** Development Team
