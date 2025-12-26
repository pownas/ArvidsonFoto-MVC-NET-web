# Detaljerad Migreringsplan för ArvidsonFoto

**Datum**: 2025-01-20  
**Version**: v3.10.3 → v4.0.0  
**Status**: 📋 Planering

---

## Innehållsförteckning

1. [Översikt](#översikt)
2. [Fasindelning](#fasindelning)
3. [Fas 1: Nullable Warnings & Required Members](#fas-1-nullable-warnings--required-members)
4. [Fas 2: Records & DTOs](#fas-2-records--dtos)
5. [Fas 3: Primary Constructors & DI](#fas-3-primary-constructors--di)
6. [Fas 4: Model Consolidation](#fas-4-model-consolidation)
7. [Fas 5: Global Usings Expansion](#fas-5-global-usings-expansion)
8. [Fas 6: LoggerMessage Source Generators](#fas-6-loggermessage-source-generators)
9. [Fas 7: Advanced C# Features](#fas-7-advanced-c-features)
10. [Testning & Validering](#testning--validering)
11. [Riskanalys](#riskanalys)
12. [Rollback-strategi](#rollback-strategi)

---

## Översikt

Denna migreringsplan beskriver hur ArvidsonFoto-applikationen ska moderniseras från .NET 10 med traditionella mönster till modern C# 14 med senaste best practices.

### Mål

- ✅ Eliminera alla nullable warnings (~50 warnings)
- ✅ Modernisera datamodeller till records
- ✅ Implementera primary constructors
- ✅ Konsolidera duplicerade modeller
- ✅ Expandera global usings
- ✅ Implementera LoggerMessage source generators
- ✅ Använda collection expressions och pattern matching

### Totalt estimerad tid: 20-30 timmar

### Viktiga principer

1. **Inkrementell migration** - En fas i taget
2. **Testning mellan faser** - Alltid validera före nästa steg
3. **Git commits per fas** - Enkel rollback om något går fel
4. **Dokumentation** - Uppdatera README och kommentarer

---

## Fasindelning

| Fas | Namn | Tid | Risk | Prioritet |
|-----|------|-----|------|-----------|
| 1 | Nullable Warnings & Required Members | 4-6h | Låg | Hög ⭐⭐⭐ |
| 2 | Records & DTOs | 4-6h | Medel | Hög ⭐⭐⭐ |
| 3 | Primary Constructors & DI | 2-3h | Låg | Medel ⭐⭐ |
| 4 | Model Consolidation | 4-6h | Hög | Hög ⭐⭐⭐ |
| 5 | Global Usings Expansion | 1-2h | Låg | Medel ⭐⭐ |
| 6 | LoggerMessage Source Generators | 2-3h | Medel | Medel ⭐⭐ |
| 7 | Advanced C# Features | 2-3h | Låg | Låg ⭐ |

---

## Fas 1: Nullable Warnings & Required Members

**Mål**: Eliminera alla nullable warnings och introducera `required` keyword  
**Tid**: 4-6 timmar  
**Risk**: Låg  
**Commit**: `feat: eliminate nullable warnings and add required members`

### 1.1 ViewModels

#### UploadImageViewModel.cs

**Före:**
```csharp
public class UploadImageViewModel
{
    public CategoryDto SelectedCategory { get; set; } = CategoryDto.CreateEmpty();
    public List<CategoryDto> SubCategories { get; set; } = [];
    public UploadImageInputDto ImageInputModel { get; set; } = UploadImageInputDto.CreateEmpty();
    public string CurrentUrl { get; set; } = string.Empty;
}
```

**Efter:**
```csharp
public class UploadImageViewModel
{
    public required CategoryDto SelectedCategory { get; init; } = CategoryDto.CreateEmpty();
    public required List<CategoryDto> SubCategories { get; init; } = [];
    public required UploadImageInputDto ImageInputModel { get; init; } = UploadImageInputDto.CreateEmpty();
    public required string CurrentUrl { get; init; } = string.Empty;
}
```

#### UploadImageInputModel.cs

**Före:**
```csharp
public class UploadImageInputModel
{
    public int ImageId { get; set; }
    public int? ImageHuvudfamilj { get; set; }
    public required string ImageHuvudfamiljNamn { get; set; } = "";
    // ... fler properties
}
```

**Efter:**
```csharp
public class UploadImageInputModel
{
    public required int ImageId { get; init; }
    public int? ImageHuvudfamilj { get; init; }
    public required string ImageHuvudfamiljNamn { get; init; } = "";
    // ... fler properties med required och init
}
```

#### Filer att uppdatera:

1. ✅ `ArvidsonFoto/Core/ViewModels/ErrorViewModel.cs`
2. ✅ `ArvidsonFoto/Core/ViewModels/GalleryViewModel.cs`
3. ✅ `ArvidsonFoto/Core/ViewModels/UploadEditImagesViewModel.cs`
4. ✅ `ArvidsonFoto/Core/ViewModels/UploadGbViewModel.cs`
5. ✅ `ArvidsonFoto/Core/ViewModels/UploadImageInputModel.cs`
6. ✅ `ArvidsonFoto/Core/ViewModels/UploadImageViewModel.cs`
7. ✅ `ArvidsonFoto/Core/ViewModels/UploadLogReaderViewModel.cs`
8. ✅ `ArvidsonFoto/Core/ViewModels/UploadNewCategoryModel.cs`
9. ✅ `ArvidsonFoto/Core/ViewModels/FacebookUploadViewModel.cs`

### 1.2 DTOs

#### Filer att uppdatera:

1. ✅ `ArvidsonFoto/Core/DTOs/CategoryDto.cs`
2. ✅ `ArvidsonFoto/Core/DTOs/ContactFormDto.cs`
3. ✅ `ArvidsonFoto/Core/DTOs/FacebookUploadInputDto.cs`
4. ✅ `ArvidsonFoto/Core/DTOs/GuestbookInputDto.cs`
5. ✅ `ArvidsonFoto/Core/DTOs/ImageDto.cs`
6. ✅ `ArvidsonFoto/Core/DTOs/MainMenuDto.cs`
7. ✅ `ArvidsonFoto/Core/DTOs/UploadImageInputDto.cs`
8. ✅ `ArvidsonFoto/Core/DTOs/UploadNewCategoryDto.cs`

### 1.3 Validering

```bash
# Bygg projektet
dotnet build

# Kontrollera warnings
dotnet build 2>&1 | grep -i "warning CS8618"

# Förväntat resultat: 0 warnings
```

### 1.4 Testning

```bash
# Kör alla tester
dotnet test

# Förväntat resultat: Alla tester passerar
```

---

## Fas 2: Records & DTOs

**Mål**: Konvertera DTOs och ViewModels till records  
**Tid**: 4-6 timmar  
**Risk**: Medel  
**Commit**: `feat: convert DTOs and ViewModels to records`

### 2.1 DTOs → Records

#### CategoryDto.cs

**Före:**
```csharp
public class CategoryDto
{
    public int? CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string UrlImage { get; set; } = string.Empty;
    // ... fler properties
}
```

**Efter:**
```csharp
public record CategoryDto
{
    public required int? CategoryId { get; init; }
    public required string Name { get; init; } = string.Empty;
    public required string UrlImage { get; init; } = string.Empty;
    // ... fler properties med required och init
    
    public static CategoryDto CreateEmpty() => new()
    {
        CategoryId = null,
        Name = string.Empty,
        UrlImage = string.Empty,
        // ... default values
    };
}
```

#### ImageDto.cs

**Före:**
```csharp
public class ImageDto
{
    public int ImageId { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    // ... fler properties
}
```

**Efter:**
```csharp
public record ImageDto
{
    public required int ImageId { get; init; }
    public required int CategoryId { get; init; }
    public required string Name { get; init; } = string.Empty;
    // ... fler properties med required och init
}
```

### 2.2 ViewModels → Records

#### ErrorViewModel.cs

**Före:**
```csharp
public class ErrorViewModel
{
    public string RequestId { get; set; } = string.Empty;
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    public string VisitedUrl { get; set; } = string.Empty;
}
```

**Efter:**
```csharp
public record ErrorViewModel
{
    public required string RequestId { get; init; } = string.Empty;
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    public required string VisitedUrl { get; init; } = string.Empty;
}
```

### 2.3 Filer att konvertera till records

#### DTOs:
1. ✅ `CategoryDto.cs`
2. ✅ `ContactFormDto.cs`
3. ✅ `FacebookUploadInputDto.cs`
4. ✅ `GuestbookInputDto.cs`
5. ✅ `ImageDto.cs`
6. ✅ `MainMenuDto.cs`
7. ✅ `UploadImageInputDto.cs`
8. ✅ `UploadNewCategoryDto.cs`

#### ViewModels:
1. ✅ `ErrorViewModel.cs`
2. ✅ `GalleryViewModel.cs`
3. ✅ `FacebookUploadViewModel.cs`
4. ⚠️ `UploadImageViewModel.cs` (komplicerad med mutable state)
5. ⚠️ `UploadEditImagesViewModel.cs` (komplicerad med mutable state)
6. ⚠️ `UploadImageInputModel.cs` (används för form binding)

**OBS**: ViewModels med `[BindProperty]` eller som används för POST-formulär bör behålla klasser med mutable state.

### 2.4 Uppdatera användning

När records har skapats, uppdatera kod som använder objektinitializers:

**Före:**
```csharp
var viewModel = new GalleryViewModel
{
    DisplayImagesList = images,
    SelectedCategory = category
};
```

**Efter:**
```csharp
var viewModel = new GalleryViewModel
{
    DisplayImagesList = images,
    SelectedCategory = category,
    AllImagesList = [], // Required properties måste sättas
    TotalPages = -1,
    CurrentPage = -1,
    CurrentUrl = string.Empty,
    ErrorMessage = string.Empty
};
```

### 2.5 Validering

```bash
# Bygg projektet
dotnet build

# Kör alla tester
dotnet test

# Förväntat resultat: Alla tester passerar
```

---

## Fas 3: Primary Constructors & DI

**Mål**: Modernisera services med primary constructors  
**Tid**: 2-3 timmar  
**Risk**: Låg  
**Commit**: `feat: implement primary constructors in services`

### 3.1 Services

#### ApiImageService.cs

**Före:**
```csharp
public class ApiImageService : IApiImageService
{
    private readonly ILogger<ApiImageService> _logger;
    private readonly ArvidsonFotoCoreDbContext _coreContext;
    private readonly IConfiguration _configuration;
    private readonly IApiCategoryService _categoryService;
    
    public ApiImageService(
        ILogger<ApiImageService> logger,
        ArvidsonFotoCoreDbContext coreContext,
        IConfiguration configuration,
        IApiCategoryService categoryService)
    {
        _logger = logger;
        _coreContext = coreContext;
        _configuration = configuration;
        _categoryService = categoryService;
    }
    
    public List<ImageDto> GetRandomNumberOfImages(int count)
    {
        return _coreContext.TblImages
            .OrderBy(x => Guid.NewGuid())
            .Take(count)
            .ToList();
    }
}
```

**Efter:**
```csharp
public class ApiImageService(
    ILogger<ApiImageService> logger,
    ArvidsonFotoCoreDbContext coreContext,
    IConfiguration configuration,
    IApiCategoryService categoryService) : IApiImageService
{
    public List<ImageDto> GetRandomNumberOfImages(int count)
    {
        return coreContext.TblImages
            .OrderBy(x => Guid.NewGuid())
            .Take(count)
            .ToList();
    }
}
```

#### ApiCategoryService.cs

**Före:**
```csharp
public class ApiCategoryService : IApiCategoryService
{
    private readonly ILogger<ApiCategoryService> _logger;
    private readonly ArvidsonFotoCoreDbContext _coreContext;
    private readonly IMemoryCache _memoryCache;
    
    public ApiCategoryService(
        ILogger<ApiCategoryService> logger,
        ArvidsonFotoCoreDbContext coreContext,
        IMemoryCache memoryCache)
    {
        _logger = logger;
        _coreContext = coreContext;
        _memoryCache = memoryCache;
    }
}
```

**Efter:**
```csharp
public class ApiCategoryService(
    ILogger<ApiCategoryService> logger,
    ArvidsonFotoCoreDbContext coreContext,
    IMemoryCache memoryCache) : IApiCategoryService
{
    // Direct parameter usage instead of fields
}
```

### 3.2 Controllers

#### HomeController.cs

**Före:**
```csharp
public class HomeController : Controller
{
    private readonly IPageCounterService _pageCounterService;
    private readonly IApiImageService _imageService;

    public HomeController(IPageCounterService pageCounterService, IApiImageService imageService)
    {
        _pageCounterService = pageCounterService;
        _imageService = imageService;
    }
    
    public IActionResult Index()
    {
        ViewData["Title"] = "Startsidan";
        if (User?.Identity?.IsAuthenticated is false)
            _pageCounterService.AddPageCount("Startsidan");
        
        var viewModel = new GalleryViewModel
        {
            DisplayImagesList = _imageService.GetRandomNumberOfImages(12)
        };
        
        return View(viewModel);
    }
}
```

**Efter:**
```csharp
public class HomeController(
    IPageCounterService pageCounterService,
    IApiImageService imageService) : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = "Startsidan";
        if (User?.Identity?.IsAuthenticated is false)
            pageCounterService.AddPageCount("Startsidan");
        
        var viewModel = new GalleryViewModel
        {
            DisplayImagesList = imageService.GetRandomNumberOfImages(12),
            // ... required properties
        };
        
        return View(viewModel);
    }
}
```

### 3.3 Filer att uppdatera

#### Services:
1. ✅ `ApiCategoryService.cs`
2. ✅ `ApiImageService.cs`
3. ✅ `ContactService.cs`
4. ✅ `FacebookService.cs`
5. ✅ `GuestBookService.cs`
6. ✅ `PageCounterService.cs`
7. ✅ `DatabaseInitializationService.cs`

#### Controllers:
1. ✅ `BilderController.cs`
2. ✅ `DevController.cs`
3. ✅ `HomeController.cs`
4. ✅ `InfoController.cs`
5. ✅ `RedirectRouterController.cs`
6. ✅ `SenastController.cs`
7. ✅ `UploadAdminController.cs`
8. ✅ `CategoryApiController.cs`
9. ✅ `ImageApiController.cs`

### 3.4 Validering

```bash
# Bygg projektet
dotnet build

# Kör alla tester
dotnet test

# Förväntat resultat: Alla tester passerar
```

---

## Fas 4: Model Consolidation

**Mål**: Eliminera duplicerade modeller mellan `ArvidsonFoto.Models` och `ArvidsonFoto.Core.Models`  
**Tid**: 4-6 timmar  
**Risk**: Hög  
**Commit**: `feat: consolidate duplicate models`

### 4.1 Problemanalys

**Nuvarande situation:**

```
ArvidsonFoto.Core.Models:
- TblImage (partial class, databas-mappning)
- TblMenu (partial class, databas-mappning)
- TblGb (partial class, databas-mappning)
- TblPageCounter

ArvidsonFoto.Models: (om den finns)
- TblImage (duplicerad?)
- TblMenu (duplicerad?)
- TblGb (duplicerad?)
```

**Detta förhindrar att vi kan lägga till `global using ArvidsonFoto.Models;` i GlobalUsings.cs**

### 4.2 Strategi

#### Option 1: Byt namn på Core.Models (Rekommenderad)

**Före:**
```
ArvidsonFoto.Core.Models.TblImage
ArvidsonFoto.Models.TblImage (duplicerad)
```

**Efter:**
```
ArvidsonFoto.Core.Entities.TblImage (databas-entitet)
ArvidsonFoto.Models.TblImage (borttagen eller konsoliderad)
```

#### Option 2: Konsolidera till Core.Models

**Förenkling:**
- Flytta all business logic till `ArvidsonFoto.Core.Models`
- Ta bort `ArvidsonFoto.Models`-mappen helt
- Uppdatera alla `using`-statements

### 4.3 Implementation (Option 1 - Rekommenderad)

#### Steg 1: Skapa Entities namespace

```bash
mkdir -p ArvidsonFoto/Core/Entities
```

#### Steg 2: Flytta modeller

```csharp
// Flytta från:
ArvidsonFoto/Core/Models/TblImage.cs
ArvidsonFoto/Core/Models/TblMenu.cs
ArvidsonFoto/Core/Models/TblGb.cs
ArvidsonFoto/Core/Models/TblPageCounter.cs
ArvidsonFoto/Core/Models/TblKontakt.cs

// Till:
ArvidsonFoto/Core/Entities/TblImage.cs
ArvidsonFoto/Core/Entities/TblMenu.cs
ArvidsonFoto/Core/Entities/TblGb.cs
ArvidsonFoto/Core/Entities/TblPageCounter.cs
ArvidsonFoto/Core/Entities/TblKontakt.cs
```

**Namespace-ändring:**
```csharp
// Före
namespace ArvidsonFoto.Core.Models;

// Efter
namespace ArvidsonFoto.Core.Entities;
```

#### Steg 3: Uppdatera DbContext

```csharp
// ArvidsonFotoCoreDbContext.cs

using ArvidsonFoto.Core.Entities; // Ändrat från .Models

public partial class ArvidsonFotoCoreDbContext : DbContext
{
    public virtual DbSet<TblGb> TblGbs { get; set; } = null!;
    public virtual DbSet<TblImage> TblImages { get; set; } = null!;
    public virtual DbSet<TblMenu> TblMenus { get; set; } = null!;
    public virtual DbSet<TblPageCounter> TblPageCounter { get; set; } = null!;
    public virtual DbSet<TblKontakt> TblKontakt { get; set; } = null!;
}
```

#### Steg 4: Uppdatera alla using-statements

**Find & Replace:**
```
Find:    using ArvidsonFoto.Core.Models;
Replace: using ArvidsonFoto.Core.Entities;
```

**Filer att uppdatera** (estimerade):
- Services (~7 filer)
- Controllers (~9 filer)
- Extensions (~3 filer)
- DbContext (~3 filer)
- Migrations (~alla migrationsfiler)

#### Steg 5: Ta bort duplicerade modeller

```bash
# Om ArvidsonFoto.Models finns med duplicerade filer
rm -rf ArvidsonFoto/Models/TblImage.cs
rm -rf ArvidsonFoto/Models/TblMenu.cs
rm -rf ArvidsonFoto/Models/TblGb.cs
```

### 4.4 Validering

```bash
# Bygg projektet
dotnet build

# Kontrollera att inga fel finns
# Förväntat resultat: Build succeeded, 0 errors

# Kör alla tester
dotnet test

# Förväntat resultat: Alla tester passerar
```

### 4.5 Risker

| Risk | Sannolikhet | Påverkan | Mitigering |
|------|-------------|----------|-----------|
| Missat using-statement | Medel | Hög | Bygg hela lösningen efter varje ändring |
| Migration-problem | Låg | Hög | EF Core migrationer behöver inte ändras (samma table names) |
| Test-fel | Medel | Medel | Kör tester kontinuerligt |

---

## Fas 5: Global Usings Expansion

**Mål**: Expandera GlobalUsings.cs med fler namespaces  
**Tid**: 1-2 timmar  
**Risk**: Låg  
**Commit**: `feat: expand global usings`

### 5.1 Nuvarande GlobalUsings.cs

```csharp
// Controllers
global using ArvidsonFoto.Controllers;

// ASP.NET Core
global using Microsoft.AspNetCore.Mvc;

// Logging
global using Serilog;
global using Serilog.Events;

// Common annotations
global using System.ComponentModel.DataAnnotations.Schema;
```

### 5.2 Efter Fas 4 (Model Consolidation)

```csharp
// Controllers
global using ArvidsonFoto.Controllers;

// ASP.NET Core
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Identity;

// Entity Framework
global using Microsoft.EntityFrameworkCore;

// Data & Entities
global using ArvidsonFoto.Core.Data;
global using ArvidsonFoto.Core.Entities;  // Nytt efter Fas 4!
global using ArvidsonFoto.Core.DTOs;
global using ArvidsonFoto.Core.ViewModels;
global using ArvidsonFoto.Core.Interfaces;

// Logging
global using Serilog;
global using Serilog.Events;

// Common System
global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;

// Extensions
global using ArvidsonFoto.Core.Extensions;

namespace ArvidsonFoto;
```

### 5.3 Cleanup av using-statements

Efter global usings expansion, ta bort redundanta using-statements från filer:

```bash
# PowerShell script för att hitta filer med redundanta usings
Get-ChildItem -Path "ArvidsonFoto" -Filter "*.cs" -Recurse | 
    Select-String -Pattern "using Microsoft.AspNetCore.Mvc;" |
    Select-Object -ExpandProperty Path -Unique
```

**Manuell cleanup av filer:**
- Controllers (~9 filer)
- Services (~7 filer)
- Pages (~50+ Razor Pages)

### 5.4 Validering

```bash
# Bygg projektet
dotnet build

# Kör alla tester
dotnet test

# Förväntat resultat: Alla tester passerar
```

---

## Fas 6: LoggerMessage Source Generators

**Mål**: Implementera LoggerMessage source generators för bättre performance  
**Tid**: 2-3 timmar  
**Risk**: Medel  
**Commit**: `feat: implement LoggerMessage source generators`

### 6.1 Skapa LogMessages-klass

```csharp
// ArvidsonFoto/Core/Logging/LogMessages.cs

namespace ArvidsonFoto.Core.Logging;

/// <summary>
/// High-performance logging using source generators
/// </summary>
public static partial class LogMessages
{
    // Security
    
    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Warning,
        Message = "Potential SQL injection attempt detected in query parameter '{ParameterKey}' from IP {IpAddress}")]
    public static partial void LogSqlInjectionAttempt(
        this ILogger logger,
        string parameterKey,
        string ipAddress);
    
    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Warning,
        Message = "Potential XSS attempt detected in field '{FieldName}' from IP {IpAddress}")]
    public static partial void LogXssAttempt(
        this ILogger logger,
        string fieldName,
        string ipAddress);
    
    // Image Service
    
    [LoggerMessage(
        EventId = 2001,
        Level = LogLevel.Information,
        Message = "Loading {ImageCount} random images")]
    public static partial void LogLoadingRandomImages(
        this ILogger logger,
        int imageCount);
    
    [LoggerMessage(
        EventId = 2002,
        Level = LogLevel.Error,
        Message = "Failed to load image with ID {ImageId}")]
    public static partial void LogImageLoadFailed(
        this ILogger logger,
        int imageId,
        Exception exception);
    
    // Category Service
    
    [LoggerMessage(
        EventId = 3001,
        Level = LogLevel.Information,
        Message = "Loading category '{CategoryName}' (ID: {CategoryId})")]
    public static partial void LogLoadingCategory(
        this ILogger logger,
        string categoryName,
        int categoryId);
    
    [LoggerMessage(
        EventId = 3002,
        Level = LogLevel.Warning,
        Message = "Category '{CategoryName}' not found")]
    public static partial void LogCategoryNotFound(
        this ILogger logger,
        string categoryName);
    
    // Contact Service
    
    [LoggerMessage(
        EventId = 4001,
        Level = LogLevel.Information,
        Message = "Contact form submitted from {Email}")]
    public static partial void LogContactFormSubmitted(
        this ILogger logger,
        string email);
    
    [LoggerMessage(
        EventId = 4002,
        Level = LogLevel.Error,
        Message = "Failed to send contact email to {Email}")]
    public static partial void LogContactEmailFailed(
        this ILogger logger,
        string email,
        Exception exception);
    
    // Facebook Service
    
    [LoggerMessage(
        EventId = 5001,
        Level = LogLevel.Information,
        Message = "Creating Facebook post with {ImageCount} images")]
    public static partial void LogCreatingFacebookPost(
        this ILogger logger,
        int imageCount);
    
    [LoggerMessage(
        EventId = 5002,
        Level = LogLevel.Error,
        Message = "Failed to create Facebook post")]
    public static partial void LogFacebookPostFailed(
        this ILogger logger,
        Exception exception);
    
    // Guestbook Service
    
    [LoggerMessage(
        EventId = 6001,
        Level = LogLevel.Information,
        Message = "New guestbook entry from {Name} ({Email})")]
    public static partial void LogGuestbookEntry(
        this ILogger logger,
        string name,
        string email);
    
    // Page Counter Service
    
    [LoggerMessage(
        EventId = 7001,
        Level = LogLevel.Debug,
        Message = "Incrementing page counter for '{PageName}'")]
    public static partial void LogPageCounterIncrement(
        this ILogger logger,
        string pageName);
    
    // Application Lifecycle
    
    [LoggerMessage(
        EventId = 9001,
        Level = LogLevel.Information,
        Message = "Application starting in {Environment} mode")]
    public static partial void LogApplicationStarting(
        this ILogger logger,
        string environment);
    
    [LoggerMessage(
        EventId = 9002,
        Level = LogLevel.Information,
        Message = "Application shutting down gracefully")]
    public static partial void LogApplicationShuttingDown(
        this ILogger logger);
    
    [LoggerMessage(
        EventId = 9999,
        Level = LogLevel.Critical,
        Message = "Unhandled exception occurred")]
    public static partial void LogUnhandledException(
        this ILogger logger,
        Exception exception);
}
```

### 6.2 Uppdatera GlobalUsings.cs

```csharp
// Lägg till
global using ArvidsonFoto.Core.Logging;
```

### 6.3 Ersätt befintlig logging

#### InputValidationMiddleware.cs

**Före:**
```csharp
_logger.LogWarning(
    "Potential SQL injection attempt detected in query parameter '{Key}' from IP {IpAddress}",
    param.Key,
    context.Connection.RemoteIpAddress);
```

**Efter:**
```csharp
_logger.LogSqlInjectionAttempt(
    param.Key,
    context.Connection.RemoteIpAddress?.ToString() ?? "unknown");
```

#### ApiImageService.cs

**Före:**
```csharp
_logger.LogInformation("Loading {Count} random images", count);
```

**Efter:**
```csharp
logger.LogLoadingRandomImages(count);
```

### 6.4 Filer att uppdatera

1. ✅ `InputValidationMiddleware.cs`
2. ✅ `ApiImageService.cs`
3. ✅ `ApiCategoryService.cs`
4. ✅ `ContactService.cs`
5. ✅ `FacebookService.cs`
6. ✅ `GuestBookService.cs`
7. ✅ `PageCounterService.cs`
8. ✅ `Program.cs` (application lifecycle)

### 6.5 Validering

```bash
# Bygg projektet
dotnet build

# Kör alla tester
dotnet test

# Förväntat resultat: Alla tester passerar

# Performance benchmark (optional)
dotnet run --configuration Release
```

---

## Fas 7: Advanced C# Features

**Mål**: Använda collection expressions, pattern matching och andra moderna features  
**Tid**: 2-3 timmar  
**Risk**: Låg  
**Commit**: `feat: implement advanced C# 14 features`

### 7.1 Collection Expressions

**Före:**
```csharp
var origins = new[] { "https://localhost:5001", "http://localhost:5000" };
var emptyList = new List<string>();
```

**Efter:**
```csharp
var origins = ["https://localhost:5001", "http://localhost:5000"];
var emptyList = List<string>.Empty;
```

### 7.2 Pattern Matching

#### HomeController.cs

**Före:**
```csharp
public IActionResult Index()
{
    ViewData["Title"] = "Startsidan";
    if (User?.Identity?.IsAuthenticated is false)
        pageCounterService.AddPageCount("Startsidan");
    
    var viewModel = new GalleryViewModel
    {
        DisplayImagesList = imageService.GetRandomNumberOfImages(12)
    };
    
    return View(viewModel);
}
```

**Efter:**
```csharp
public IActionResult Index()
{
    ViewData["Title"] = "Startsidan";
    if (User?.Identity is { IsAuthenticated: false })
        pageCounterService.AddPageCount("Startsidan");
    
    var viewModel = new GalleryViewModel
    {
        DisplayImagesList = imageService.GetRandomNumberOfImages(12),
        // ... required properties
    };
    
    return View(viewModel);
}
```

#### BilderController.cs

**Före:**
```csharp
if (image == null)
{
    return NotFound();
}
return View(image);
```

**Efter:**
```csharp
return image switch
{
    null => NotFound(),
    _ => View(image)
};
```

### 7.3 Switch Expressions

**Före:**
```csharp
string GetCategoryPath(int? categoryId)
{
    if (categoryId == null)
        return string.Empty;
    
    if (categoryId == 1)
        return "faglar";
    
    if (categoryId == 2)
        return "daggdjur";
    
    return "ovriga";
}
```

**Efter:**
```csharp
string GetCategoryPath(int? categoryId) => categoryId switch
{
    null => string.Empty,
    1 => "faglar",
    2 => "daggdjur",
    _ => "ovriga"
};
```

### 7.4 List Patterns

**Före:**
```csharp
if (images.Count > 0)
{
    var firstImage = images[0];
    ProcessImage(firstImage);
}
```

**Efter:**
```csharp
if (images is [var firstImage, ..])
{
    ProcessImage(firstImage);
}
```

### 7.5 Validering

```bash
# Bygg projektet
dotnet build

# Kör alla tester
dotnet test

# Förväntat resultat: Alla tester passerar
```

---

## Testning & Validering

### Test-strategi per fas

#### Unit Tests

```bash
# Kör unit tests
dotnet test ArvidsonFoto.Tests.Unit

# Förväntat resultat: Alla tester passerar
```

#### Integration Tests

```bash
# Kör integration tests
dotnet test ArvidsonFoto.Tests.Integration

# Förväntat resultat: Alla tester passerar
```

#### E2E Tests

```bash
# Kör E2E tests
dotnet test ArvidsonFoto.Tests.E2E

# Förväntat resultat: Alla tester passerar
```

### Build Validation

```bash
# Bygg hela lösningen
dotnet build

# Kör alla tester
dotnet test

# Kontrollera warnings
dotnet build 2>&1 | grep -i "warning"

# Förväntat resultat:
# - Build succeeded
# - 0 Error(s)
# - 0 Warning(s) (efter Fas 1)
# - All tests pass
```

### Code Coverage

```bash
# Kör code coverage
dotnet test --collect:"XPlat Code Coverage"

# Generera rapport
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:./coverage-report

# Förväntat resultat: >80% coverage
```

---

## Riskanalys

### Högrisk-områden

| Område | Risk | Mitigering |
|--------|------|-----------|
| Model Consolidation | Hög | Testa varje steg, ha backup |
| Records i ViewModels | Medel | Kontrollera model binding noga |
| Primary Constructors | Låg | Bygg och testa kontinuerligt |

### Kritiska filer

Dessa filer är kritiska och kräver extra noggrannhet:

1. `ArvidsonFotoCoreDbContext.cs` - Databas-kontext
2. `Program.cs` - Application startup
3. `TblImage.cs`, `TblMenu.cs`, `TblGb.cs` - Core entities
4. `ApiImageService.cs`, `ApiCategoryService.cs` - Core services

### Break-glass procedur

Om något går fel under en fas:

```bash
# 1. Stoppa omedelbart
# 2. Kör tester för att verifiera problemet
dotnet test

# 3. Kontrollera Git status
git status
git diff

# 4. Om nödvändigt, rollback
git checkout .
git clean -fd

# 5. Återgå till senast fungerande commit
git log --oneline
git reset --hard <commit-hash>
```

---

## Rollback-strategi

### Git Workflow

```bash
# Före varje fas
git checkout -b feature/migration-phase-N
git commit -m "feat: start phase N - <description>"

# Efter fas completion
git add .
git commit -m "feat: complete phase N - <description>"

# Test och validera
dotnet build
dotnet test

# Om allt fungerar
git checkout main
git merge feature/migration-phase-N

# Om något går fel
git checkout main
# feature/migration-phase-N finns kvar för felsökning
```

### Phase-specific rollback

#### Fas 1 (Nullable Warnings)

```bash
# Rollback nullable changes
git checkout main
git branch -D feature/migration-phase-1
```

#### Fas 4 (Model Consolidation)

**Kritiskt!** Spara backup före:

```bash
# Backup
tar -czf backup-before-phase4-$(date +%Y%m%d).tar.gz ArvidsonFoto/Core/

# Efter fas 4, om problem uppstår
tar -xzf backup-before-phase4-YYYYMMDD.tar.gz
```

---

## Checklista per fas

### Fas 1: Nullable Warnings ✅

- [ ] Uppdatera alla ViewModels med `required` och `init`
- [ ] Uppdatera alla DTOs med `required` och `init`
- [ ] Bygg projektet utan warnings
- [ ] Kör unit tests
- [ ] Kör integration tests
- [ ] Git commit

### Fas 2: Records ✅

- [ ] Konvertera DTOs till records
- [ ] Konvertera ViewModels till records (där lämpligt)
- [ ] Uppdatera `CreateEmpty()` methods
- [ ] Bygg projektet
- [ ] Kör alla tester
- [ ] Git commit

### Fas 3: Primary Constructors ✅

- [ ] Uppdatera alla Services
- [ ] Uppdatera alla Controllers
- [ ] Uppdatera Middleware
- [ ] Bygg projektet
- [ ] Kör alla tester
- [ ] Git commit

### Fas 4: Model Consolidation ✅

- [ ] **BACKUP!** Spara backup av Core-mappen
- [ ] Skapa Entities-namespace
- [ ] Flytta modeller till Entities
- [ ] Uppdatera DbContext
- [ ] Find & Replace using-statements
- [ ] Ta bort duplicerade modeller
- [ ] Bygg projektet
- [ ] Kör alla tester (viktigt!)
- [ ] Kör E2E tests
- [ ] Git commit

### Fas 5: Global Usings ✅

- [ ] Uppdatera GlobalUsings.cs
- [ ] Cleanup redundanta using-statements
- [ ] Bygg projektet
- [ ] Kör alla tester
- [ ] Git commit

### Fas 6: LoggerMessage ✅

- [ ] Skapa LogMessages-klass
- [ ] Implementera source generators
- [ ] Ersätt befintlig logging
- [ ] Uppdatera GlobalUsings.cs
- [ ] Bygg projektet
- [ ] Kör alla tester
- [ ] Git commit

### Fas 7: Advanced Features ✅

- [ ] Implementera collection expressions
- [ ] Förbättra pattern matching
- [ ] Använd switch expressions
- [ ] Bygg projektet
- [ ] Kör alla tester
- [ ] Git commit

---

## Tidsplan

### Vecka 1: Förberedelse & Fas 1-2

| Dag | Aktivitet | Tid |
|-----|-----------|-----|
| Mån | Läs igenom plan, förbered miljö | 1h |
| Tis | Fas 1: Nullable Warnings (del 1) | 3h |
| Ons | Fas 1: Nullable Warnings (del 2) | 3h |
| Tor | Fas 2: Records (DTOs) | 3h |
| Fre | Fas 2: Records (ViewModels) | 3h |

### Vecka 2: Fas 3-5

| Dag | Aktivitet | Tid |
|-----|-----------|-----|
| Mån | Fas 3: Primary Constructors | 3h |
| Tis | Fas 4: Model Consolidation (prep) | 2h |
| Ons | Fas 4: Model Consolidation (impl) | 4h |
| Tor | Fas 5: Global Usings Expansion | 2h |
| Fre | Testing & Validation | 2h |

### Vecka 3: Fas 6-7 & Final

| Dag | Aktivitet | Tid |
|-----|-----------|-----|
| Mån | Fas 6: LoggerMessage (impl) | 3h |
| Tis | Fas 7: Advanced Features | 3h |
| Ons | Final testing | 2h |
| Tor | Documentation update | 2h |
| Fre | Release prep & deployment | 2h |

---

## Success Criteria

### Tekniska mål

- ✅ 0 nullable warnings
- ✅ 0 build errors
- ✅ All tests passing
- ✅ Code coverage >80%
- ✅ Performance maintained or improved

### Kodkvalitet

- ✅ Modern C# 14 features används
- ✅ Records för DTOs och ViewModels
- ✅ Primary constructors i Services
- ✅ LoggerMessage source generators
- ✅ Global usings expanderade

### Dokumentation

- ✅ README uppdaterad
- ✅ MIGRATION_SUMMARY uppdaterad
- ✅ MODERNIZATION uppdaterad
- ✅ Code comments uppdaterade

---

## Post-Migration

### Version Bump

```bash
# Uppdatera version i .csproj
<Version>4.0.0</Version>
<AssemblyVersion>4.0.0.0</AssemblyVersion>
<FileVersion>4.0.0.0</FileVersion>
```

### Release Notes

Skapa `docs/RELEASE_NOTES_v4.0.0.md`:

```markdown
# Release Notes v4.0.0

## Major Changes

- ✅ Eliminated all nullable warnings
- ✅ Modernized to C# 14 with records and primary constructors
- ✅ Consolidated duplicate models
- ✅ Implemented LoggerMessage source generators
- ✅ Expanded global usings

## Breaking Changes

None - All changes are internal improvements.

## Performance Improvements

- LoggerMessage source generators: 10-20% faster logging
- Records: Reduced memory allocations

## Migration Guide

This is an internal modernization. No migration needed for consumers.
```

### Git Tagging

```bash
git tag -a v4.0.0 -m "Major modernization to C# 14"
git push origin v4.0.0
```

---

## Resurser

### Microsoft Dokumentation

- [Records](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record)
- [Primary Constructors](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-12#primary-constructors)
- [Required Members](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/required)
- [LoggerMessage](https://learn.microsoft.com/en-us/dotnet/core/extensions/logger-message-generator)
- [Collection Expressions](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/collection-expressions)

### Interna dokument

- [MODERNIZATION.md](MODERNIZATION.md) - Ursprungliga rekommendationer
- [MIGRATION_SUMMARY.md](MIGRATION_SUMMARY.md) - Program.cs migration
- [ASPIRE.md](ASPIRE.md) - .NET Aspire integration

---

**Slutnotering**: Denna migreringsplan är omfattande men genomförbar. Varje fas är oberoende och kan rullas tillbaka om problem uppstår. Totalt beräknas migreringen ta 20-30 timmar fördelat över 2-3 veckor.

**Status**: 📋 Redo att påbörja  
**Senast uppdaterad**: 2025-01-20  
**Version**: 1.0
