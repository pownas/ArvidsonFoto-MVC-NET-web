# NuGet Package Upgrade

## Preferences
- **Flow Mode**: Automatic
- **Commit Strategy**: After Each Task
- **Scope**: All NuGet packages in the solution, including direct references and centrally pinned transitive dependencies
- **Version Policy**: Use the versions identified by the generated SBOM for applicable updates; use the exact .NET 11 RC2 version `11.0.0-rc.2.26504.105` for applicable framework packages; use stable versions for other packages and do not introduce unrelated prereleases
- **Report Language**: Swedish
- **.NET 11 packages**: Upgrade existing `11.0.0-rc.1.26425.128` references to `11.0.0-rc.2.26504.105` where available and compatible with `net11.0`; never downgrade these packages to stable 10.0.12. Preserve RC1 only if RC2 does not exist for that package and report why.
- **Warnings**: Resolve build and code-style warnings without suppressing them; target fewer than 30 warnings.

## Source Control
- **Source Branch**: copilot/add-english-columns-to-database-again
- **Working Branch**: copilot/add-english-columns-to-database-again
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge); source and working branch are the same.

## Decisions
- Keep the existing `copilot/add-english-columns-to-database-again` branch as the working branch; it was already active and the working tree was clean.
- Use only NuGet sources provided by the repository's normal NuGet configuration; do not add or switch feeds.
- User expanded the upgrade scope to all applicable `Uppdatering`, `Konflikt`, and vulnerability entries in `wiki/sbom-report.md` and requests build/test validation; prefer updating direct parents over unnecessary transitive pins.
- NuGet confirmed `11.0.0-rc.2.26504.105` for representative ASP.NET Core, EF Core, Microsoft.Extensions, and cryptography packages targeting `net11.0`; use this exact target for applicable RC1 packages.
- Remediate MessagePack by moving to a version at or above 3.1.9 according to the SBOM's fixed release; update `System.Security.Cryptography.Pkcs` to RC2 to avoid leaving the reported advisory at RC1, and report any residual advisory after validation.
- The SBOM dated 2026-10-18 supersedes prior assessment versions and package decisions for this execution.
