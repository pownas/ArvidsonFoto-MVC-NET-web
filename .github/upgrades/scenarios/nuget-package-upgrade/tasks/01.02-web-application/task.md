# 01.02-web-application: Uppdatera webbappens aktiva paket

## Warning remediation (user approved)
- The first clean non-incremental web-app build completed but reported 302 warnings; the saved log has 300 unique warning diagnostics across 56 C# files (each diagnostic was emitted twice in the log). Leading unique counts: CA1848 116, CA1873 44, IDE0028 32, CA1805 18, CA1305 11, IDE0011 7, IDE0052 7, IDE0060 6, IDE0270 6 and IDE0059 5.
- The user explicitly requested fixing build and style warnings and reducing the count below 50. Do not suppress warnings or change analyzer settings to hide them.
- The .NET 11 SDK contains `DotnetTools/dotnet-format/dotnet-format.dll`; the `dotnet format` shim was unavailable from the SDK resolver, so invoke the SDK tool DLL directly to apply supported analyzer fixes, then fix remaining diagnostics in source and verify by a clean build.

## Objective
Uppdatera aktiva stabila paket som används av `ArvidsonFoto` enligt assessmentens enhetliga versioner och åtgärda warnings till färre än 50 utan att undertrycka dem.

## Research findings
- `get_project_dependencies` bekräftade CPM i `ArvidsonFoto.csproj`; paketversionerna kommer från root `Directory.Packages.props`, och projektet refererar `ArvidsonFoto.ServiceDefaults`.
- Verifierade nuvarande och valda versioner: JavaScriptEngineSwitcher.V8 3.34.1→3.35.2; MailKit 4.17.0→4.18.1; MimeKit 4.17.0→4.18.1; Scalar.AspNetCore 2.17.1→2.17.10; Serilog 4.3.1→4.4.0.
- Bedömningen rapporterade inga breaking changes för dessa fem valda målversioner. ASP.NET Core- och EF Core-referenser är uttryckligen kvar på `11.0.0-rc.1.26425.128` och ska inte ändras.
- Alla ändringar hör hemma i `Directory.Packages.props`; projektets versionlösa PackageReference-rader förblir oförändrade. `dotnet build ArvidsonFoto.csproj` ska återställa paket automatiskt från de vanliga NuGet-källorna.

## Scope
Centrala versioner i `Directory.Packages.props`: `JavaScriptEngineSwitcher.V8` 3.34.1→3.35.2, `MailKit` och `MimeKit` 4.17.0→4.18.1, `Scalar.AspNetCore` 2.17.1→2.17.10 och `Serilog` 4.3.1→4.4.0. Lämna resterande redan senaste paket oförändrade samt behåll ASP.NET Core- och EF Core-paketen på `11.0.0-rc.1.26425.128`, enligt användarens beslut. API-diffen för paket som uppdateras innehåller inga breaking changes.

## Steps
1. Ändra endast de fem valda `PackageVersion`-posterna i root `Directory.Packages.props`.
2. Återställ och bygg `ArvidsonFoto` samt beroenden; åtgärda eventuella fel och alla warnings.

**Done when**: De fem versionerna är uppdaterade centralt; RC1-versionerna är oförändrade; webbappen bygger utan fel eller warnings.
