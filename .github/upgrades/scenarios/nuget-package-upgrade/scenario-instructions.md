# NuGet Package Upgrade

## Preferences
- **Flow Mode**: Automatic
- **Commit Strategy**: After Each Task
- **Scope**: All NuGet packages in the solution, including direct references and centrally pinned transitive dependencies
- **Version Policy**: Latest stable versions compatible with the existing .NET 11 target; do not include prerelease package versions
- **Report Language**: Swedish
- **.NET 11 RC1 packages**: Keep the existing `11.0.0-rc.1.26425.128` references unchanged rather than downgrading to stable 10.0.12.
- **Warnings**: Resolve build and code-style warnings without suppressing them; target fewer than 30 warnings.

## Source Control
- **Source Branch**: copilot/add-english-columns-to-database-again
- **Working Branch**: copilot/add-english-columns-to-database-again
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge); source and working branch are the same.

## Decisions
- Keep the existing `copilot/add-english-columns-to-database-again` branch as the working branch; it was already active and the working tree was clean.
- Use only NuGet sources provided by the repository's normal NuGet configuration; do not add or switch feeds.
- Retain the current ASP.NET Core, Entity Framework Core, and ILLink RC1 packages because the assessed stable version would be a downgrade from 11 RC1 to 10.0.12, with breaking API diffs.
- Upgrade active packages to assessment versions: AngleSharp 1.8.2; coverlet.collector 10.1.0; JavaScriptEngineSwitcher.V8 3.35.2; MailKit/MimeKit 4.18.1; MessagePack 3.1.10; Microsoft.Extensions.Http.Resilience/Microsoft.Extensions.ServiceDiscovery 10.10.0; Microsoft.NET.Test.Sdk 18.10.1; Microsoft.Playwright.NUnit 1.63.0; MSTest.TestAdapter/MSTest.TestFramework 4.4.1; OpenTelemetry.Exporter.OpenTelemetryProtocol/OpenTelemetry.Extensions.Hosting 1.19.1; OpenTelemetry.Instrumentation.AspNetCore/Http/Runtime 1.19.0; Scalar.AspNetCore 2.17.10; Serilog 4.4.0; xunit.v3 4.0.1.
- Leave package entries reported as not referenced unchanged; no latest compatible version was resolved for those entries.
