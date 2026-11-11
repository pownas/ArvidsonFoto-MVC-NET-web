# 01.02-web-application: Uppdatera webbappens aktiva paket och .NET 11 RC2

## Scope Inventory
- **Projects affected**: `ArvidsonFoto` and its referenced `ArvidsonFoto.ServiceDefaults`; shared package declarations are in root `Directory.Packages.props`.
- **Distinct concerns**: update active ASP.NET Core/EF Core references to the selected RC2 release; resolve the cross-project .NET package conflicts through CPM; keep application package versions aligned with the SBOM; reduce existing code-style/build warnings without suppressions.
- **Change signals**: the SBOM reports 9 dependency version conflicts between `10.0.12` and `.NET 11 RC1`, plus two active vulnerabilities. `System.Security.Cryptography.Pkcs` is pinned at RC1 and both listed advisory intervals end at RC1. `MessagePack` is a separate AppHost/test concern and will be handled in task 01.03.
- **Skill matches**: `managing-package-references` requires CPM discovery before edits; `building-projects` requires restore-enabled builds and warnings review.

## Research findings
- `ArvidsonFoto.csproj` uses CPM, and the active direct packages are versionless references resolved from root `Directory.Packages.props`; this project references `ArvidsonFoto.ServiceDefaults`.
- Confirmed active app references currently at `11.0.0-rc.1.26425.128`: Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore, Microsoft.AspNetCore.Identity.EntityFrameworkCore, Microsoft.AspNetCore.Identity.UI, Microsoft.AspNetCore.OpenApi, Microsoft.EntityFrameworkCore.Design, Microsoft.EntityFrameworkCore.InMemory, Microsoft.EntityFrameworkCore.SqlServer, Microsoft.EntityFrameworkCore.Tools. The central transitive pins also include RC1 Microsoft.Extensions and cryptography packages.
- CPM is enabled with `CentralPackageTransitivePinningEnabled=true`. Make version changes centrally and do not add `Version` attributes to project `PackageReference` elements.
- NuGet confirms exact `11.0.0-rc.2.26504.105` is available for `net11.0` for representative ASP.NET Core, EF Core, Microsoft.Extensions, `System.Security.Cryptography.Pkcs`, `System.Security.Cryptography.ProtectedData`, Microsoft.Bcl.AsyncInterfaces, and `Microsoft.AspNetCore.Mvc.Testing` packages.
- The nine SBOM conflicts concern Configuration.CommandLine, Configuration.EnvironmentVariables, Configuration.UserSecrets, Hosting, Logging.Console, Logging.Debug, Logging.EventLog, Logging.EventSource, and System.Diagnostics.EventLog. Align them to the exact RC2 version only after confirming resolved package graph compatibility.
- The app packages JavaScriptEngineSwitcher.V8 3.35.2, MailKit/MimeKit 4.18.1, Scalar.AspNetCore 2.17.10, and Serilog 4.4.0 already match the SBOM; do not change them based on the older assessment.
- The SBOM flags `System.Security.Cryptography.Pkcs` RC1 with advisories GHSA-846f-f764-76mx and GHSA-2fmv-rv2c-59x7, whose affected ranges end at RC1. RC2 is the user-selected target; verify the resulting graph with `dotnet list package --vulnerable --include-transitive`.
- A prior clean non-incremental app build recorded 302 warning lines / 300 unique diagnostics across 56 C# files. Project instructions require fewer than 50 warnings without suppressing them; the scenario preference is stricter (fewer than 30). Rebuild to establish current baseline and fix actual warnings.

## Steps
1. Update applicable RC1 framework package pins and the 9 conflict pins in root `Directory.Packages.props` to exactly `11.0.0-rc.2.26504.105`, without downgrading to 10.0.12.
2. Confirm active web package versions remain aligned to the SBOM; avoid unrelated changes.
3. Restore/build `ArvidsonFoto`, resolve compiler/warning issues without suppressions, and verify warning count below the configured threshold.
4. Record build, vulnerability and test evidence before completing this subtask.

**Done when**: the active application package graph resolves on the exact RC2 version where available, web package versions agree with the SBOM, the app builds without errors and has fewer than 30 warnings, and no RC1 Pkcs advisory remains in the restored graph.
