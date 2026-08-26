# Migreringslogg: .NET 10 → .NET 11 (preview.7)

**Datum**: 2026-08-26  
**Version**: v3.10.11 → v3.11.0  
**Status**: ✅ Slutförd

## Krav

- .NET 11 SDK `11.0.100-preview.7.26381.103`
- Installera från: https://dotnet.microsoft.com/download/dotnet/11.0

## Ändrade filer (Sammanfattning)

| Fil | Ändring |
|-----|---------|
| `global.json` | SDK `10.0.300` → `11.0.100-preview.7.26381.103`, MSTest.Sdk `4.2.3` → `4.3.3` |
| `Directory.Build.props` | TFM `net10.0` → `net11.0`, Version `3.10.11` → `3.11.0` |
| `Directory.Packages.props` | Alla `10.0.9` Microsoft-paket → `11.0.0-preview.7.26381.103` |
| `ArvidsonFoto.AppHost/ArvidsonFoto.AppHost.csproj` | Aspire.AppHost.Sdk `13.4.3` → `13.5.3` |
| `.config/dotnet-tools.json` | dotnet-ef `10.0.9` → `11.0.0-preview.7.26381.103` |
| `.github/workflows/dotnet.yml` | .NET 10 → .NET 11 preview |

## Paketlyft

### Microsoft-paket till `11.0.0-preview.7.26381.103`

- `Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore`
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
- `Microsoft.AspNetCore.Identity.UI`
- `Microsoft.AspNetCore.Mvc.Testing`
- `Microsoft.AspNetCore.OpenApi`
- `Microsoft.Bcl.AsyncInterfaces`
- `Microsoft.Bcl.Cryptography`
- `Microsoft.EntityFrameworkCore.InMemory`
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.EntityFrameworkCore.Tools`
- `Microsoft.Extensions.Caching.Memory`
- `Microsoft.Extensions.Configuration`
- `Microsoft.Extensions.Configuration.Abstractions`
- `Microsoft.Extensions.Configuration.Binder`
- `Microsoft.Extensions.Configuration.FileExtensions`
- `Microsoft.Extensions.Configuration.Json`
- `Microsoft.Extensions.DependencyInjection`
- `Microsoft.Extensions.DependencyModel`
- `Microsoft.Extensions.Diagnostics`
- `Microsoft.Extensions.Diagnostics.Abstractions`
- `Microsoft.Extensions.FileProviders.Abstractions`
- `Microsoft.Extensions.Hosting.Abstractions`
- `Microsoft.Extensions.Http`
- `Microsoft.Extensions.Logging`
- `Microsoft.Extensions.Logging.Configuration`
- `System.Security.Cryptography.Pkcs`
- `System.Security.Cryptography.ProtectedData`

### Övriga paketuppdateringar

| Paket | Från | Till |
|-------|------|------|
| `Aspire.Hosting.SqlServer` | 13.4.3 | 13.5.3 |
| `Aspire.Hosting.Testing` | 13.4.3 | 13.5.3 |
| `Azure.Core` | 1.59.0 | 1.62.0 |
| `BouncyCastle.Cryptography` | 2.6.2 | 2.7.0 |
| `MessagePack` | 3.1.7 | 3.1.8 |
| `Microsoft.Data.SqlClient` | 7.0.1 | 7.0.2 |
| `Microsoft.Extensions.Http.Resilience` | 10.7.0 | 10.9.0 |
| `Microsoft.Extensions.ServiceDiscovery` | 10.7.0 | 10.9.0 |
| `Microsoft.Identity.Client` | 4.84.2 | 4.88.0 |
| `Microsoft.IdentityModel.JsonWebTokens` | 8.19.1 | 8.22.0 |
| `Microsoft.NET.Test.Sdk` | 18.6.0 | 18.9.0 |
| `Microsoft.Playwright.NUnit` | 1.60.0 | 1.62.0 |
| `Microsoft.Testing.Extensions.Telemetry` | 2.2.3 | 2.3.3 |
| `Microsoft.Testing.Extensions.TrxReport.Abstractions` | 2.2.3 | 2.3.3 |
| `Microsoft.Testing.Platform` | 2.2.3 | 2.3.3 |
| `Microsoft.Testing.Platform.MSBuild` | 2.2.3 | 2.3.3 |
| `MSTest.TestAdapter` | 4.2.3 | 4.3.3 |
| `MSTest.TestFramework` | 4.2.3 | 4.3.3 |
| `OpenTelemetry.Exporter.OpenTelemetryProtocol` | 1.16.0 | 1.18.0 |
| `OpenTelemetry.Extensions.Hosting` | 1.16.0 | 1.18.0 |
| `OpenTelemetry.Instrumentation.AspNetCore` | 1.15.2 | 1.18.0 |
| `OpenTelemetry.Instrumentation.Http` | 1.15.1 | 1.18.0 |
| `OpenTelemetry.Instrumentation.Runtime` | 1.15.1 | 1.18.0 |
| `Scalar.AspNetCore` | 2.16.3 | 2.17.1 |
| `Serilog.Settings.Configuration` | 10.0.0 | 10.0.1 |
| `System.IdentityModel.Tokens.Jwt` | 8.19.1 | 8.22.0 |
| `xunit.runner.visualstudio` | 3.1.5 | 4.0.0 |
| `xunit.v3` | 3.2.2 | 4.0.0 |

### Oförändrade paket

Följande paket behöll sina versioner (ingen nyare kompatibel version tillgänglig):

- `AngleSharp` 1.5.1 (inget stabilt 1.x utöver beta)
- `coverlet.collector` 10.0.1
- `Humanizer.Core` 3.0.10
- `JavaScriptEngineSwitcher.Extensions.MsDependencyInjection` 3.31.0
- `JavaScriptEngineSwitcher.V8` 3.34.1
- `KubernetesClient` 19.0.2
- `LigerShark.WebOptimizer.Core` 3.0.477
- `LigerShark.WebOptimizer.Sass` 3.0.147
- `MailKit` 4.17.0
- `MimeKit` 4.17.0
- `Newtonsoft.Json` 13.0.4
- `Polly.Core` 8.7.0
- `Polly.Extensions` 8.7.0
- `Serilog` 4.3.1
- `Serilog.Sinks.Console` 6.1.1
- `Serilog.Sinks.File` 7.0.0
- `xunit` 2.9.3

## Kända begränsningar (preview)

- .NET 11 är i preview-läge och kan innehålla inkompatibiliteter som löses i kommande release candidates.
- `LigerShark.WebOptimizer.Core` och `LigerShark.WebOptimizer.Sass` har inga .NET 11-specifika versioner och kör via .NET-kompatibilitetslagret.
- `JavaScriptEngineSwitcher.V8` är inte uppdaterad för .NET 11 men är bakåtkompatibel via `netstandard2.0`-targeting.
- `AngleSharp` har inte stabila 1.x-utgåvor (alla är beta/alpha).
- `Microsoft.Data.SqlClient` 7.0.2 är stabilt och fungerar med .NET 11.
- Preview-SDK:n (`allowPrerelease: true` i `global.json`) krävs för att köra projektet tills .NET 11 officiellt lanseras.

## CI/CD

GitHub Actions-workflödet (`dotnet.yml`) har uppdaterats från `.NET 10` till `.NET 11 preview`:

```yaml
uses: actions/setup-dotnet@v4
with:
  dotnet-version: '11.0.x'
  dotnet-quality: 'preview'
```

## Kodändringar

Inga kodändringar i applikationskoden krävdes för migreringen från .NET 10 till .NET 11 – den är bakåtkompatibel. Alla befintliga API:er (passkeys, EF Core, Identity, OpenAPI, Serilog, etc.) fungerar utan ändringar.
