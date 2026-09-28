# 01.02-web-application: Uppdatera webbappens aktiva paket

## Objective
Uppdatera aktiva stabila paket som används av `ArvidsonFoto` enligt assessmentens enhetliga versioner.

## Scope
Centrala versioner i `Directory.Packages.props`: `JavaScriptEngineSwitcher.V8` 3.34.1→3.35.2, `MailKit` och `MimeKit` 4.17.0→4.18.1, `Scalar.AspNetCore` 2.17.1→2.17.10 och `Serilog` 4.3.1→4.4.0. Lämna resterande redan senaste paket oförändrade samt behåll ASP.NET Core- och EF Core-paketen på `11.0.0-rc.1.26425.128`, enligt användarens beslut. API-diffen för paket som uppdateras innehåller inga breaking changes.

## Steps
1. Ändra endast de fem valda `PackageVersion`-posterna i root `Directory.Packages.props`.
2. Återställ och bygg `ArvidsonFoto` samt beroenden; åtgärda eventuella fel och alla warnings.

**Done when**: De fem versionerna är uppdaterade centralt; RC1-versionerna är oförändrade; webbappen bygger utan fel eller warnings.
