# 01-update-nuget-packages: Uppdatera aktiva NuGet-paket

## Objective

Uppdatera aktiva NuGet-paket enligt assessmentens stabila, enhetliga versioner, samtidigt som nuvarande .NET 11 RC1-paket och oidentifierade centralposter lämnas orörda. Lösningen använder central package management; versionsändringar görs i `Directory.Packages.props` och ska inte dupliceras i projektfiler.

## Scope Inventory

- **Projects affected**: `ArvidsonFoto.ServiceDefaults`, `ArvidsonFoto`, `ArvidsonFoto.AppHost`, `ArvidsonFoto.Tests.Unit`, `ArvidsonFoto.Tests.Integration`, `ArvidsonFoto.Tests.E2E`; `.SolutionItems.csproj` contains the ILLink package but that RC1 version is intentionally retained. `SbomScanner` has no package references in the dependency inventory.
- **Distinct concerns**: central version updates for service defaults/telemetry, the web application, and host/test tooling; compatibility validation via restore, full solution build, and tests.
- **Change signals**: the quick assessment found 20 active packages with a newer stable recommendation and no version divergence. The API diffs for packages selected for update report no breaking changes. Breaking diffs for EF Core and OpenAPI refer to the rejected stable 10.0.12 downgrade; the user chose to retain all existing ASP.NET Core/EF Core/ILLink `11.0.0-rc.1.26425.128` packages.
- **Package management**: CPM is enabled with `CentralPackageTransitivePinningEnabled=true`; all planned versions are in root `Directory.Packages.props`. Package references are versionless in project files, except references for which assessment reports no newer version.
- **ServiceDefaults updates**: `Microsoft.Extensions.Http.Resilience` 10.9.0→10.10.0; `Microsoft.Extensions.ServiceDiscovery` 10.9.0→10.10.0; `OpenTelemetry.Exporter.OpenTelemetryProtocol` and `OpenTelemetry.Extensions.Hosting` 1.18.0→1.19.1; the three `OpenTelemetry.Instrumentation.*` packages 1.18.0→1.19.0.
- **Web application updates**: `JavaScriptEngineSwitcher.V8` 3.34.1→3.35.2; `MailKit` and `MimeKit` 4.17.0→4.18.1; `Scalar.AspNetCore` 2.17.1→2.17.10; `Serilog` 4.3.1→4.4.0.
- **Host/test updates**: `MessagePack` 3.1.8→3.1.10; `AngleSharp` 1.5.1→1.8.2; `coverlet.collector` 10.0.1→10.1.0; `Microsoft.NET.Test.Sdk` 18.9.0→18.10.1; `Microsoft.Playwright.NUnit` 1.62.0→1.63.0; `MSTest.TestAdapter` and `MSTest.TestFramework` 4.3.3→4.4.1; `xunit.v3` 4.0.0→4.0.1.
- **Test projects**: `ArvidsonFoto.Tests.Unit`, `ArvidsonFoto.Tests.Integration`, and `ArvidsonFoto.Tests.E2E` were identified as test projects. Solution dependency order is ServiceDefaults → ArvidsonFoto → test projects/AppHost.
- **Unresolved/inactive package entries**: assessment marked 34 centrally pinned entries as not referenced by scoped projects. Leave them untouched and report them as not version-verified.
- **Feeds**: no alternate source will be configured; restore uses only the existing NuGet configuration.

## Done when

All active packages with a newer assessed stable version are centrally updated; chosen .NET 11 RC1 packages and inactive central pins remain unchanged; restore, full solution build, and test projects succeed without unresolved build warnings.
