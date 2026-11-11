# 01.01-service-defaults: Uppdatera ServiceDefaults- och OpenTelemetry-paket

## Objective
Uppdatera aktiva stabila NuGet-beroenden i `ArvidsonFoto.ServiceDefaults` enligt den enhetliga assessment-versionen.

## Research findings
- `get_project_dependencies` bekräftade att `ArvidsonFoto.ServiceDefaults.csproj` använder CPM och att alla sju aktuella paket definieras centralt i root `Directory.Packages.props`; projektreferenserna ska därför inte ändras.
- Verifierade nuvarande värden: Microsoft.Extensions.Http.Resilience 10.9.0; Microsoft.Extensions.ServiceDiscovery 10.9.0; OpenTelemetry.Exporter.OpenTelemetryProtocol 1.18.0; OpenTelemetry.Extensions.Hosting 1.18.0; OpenTelemetry.Instrumentation.AspNetCore, Http och Runtime 1.18.0.
- Målversionerna enligt lösningsassessment: Http.Resilience/ServiceDiscovery 10.10.0; exporter/hosting 1.19.1; instrumentation 1.19.0. API-diffarna rapporterade inga source-breaking ändringar.
- Projektet ligger längst ned i den bekräftade projektberoendeordningen (före webbappen); en riktad build med restore validerar just denna deluppgift. Vanliga NuGet-källor används utan konfigurationsändring.

## Scope
Centrala versioner i `Directory.Packages.props`: `Microsoft.Extensions.Http.Resilience` 10.9.0→10.10.0, `Microsoft.Extensions.ServiceDiscovery` 10.9.0→10.10.0, `OpenTelemetry.Exporter.OpenTelemetryProtocol` 1.18.0→1.19.1, `OpenTelemetry.Extensions.Hosting` 1.18.0→1.19.1 samt `OpenTelemetry.Instrumentation.AspNetCore`, `.Http` och `.Runtime` 1.18.0→1.19.0. Ingen API-brytning rapporterades för dessa uppdateringar.

## Steps
1. Ändra endast dessa `PackageVersion`-poster i root `Directory.Packages.props`.
2. Återställ och bygg ServiceDefaults samt dess beroenden; åtgärda eventuella fel och alla warnings.

**Done when**: De sju versionerna är uppdaterade centralt och ServiceDefaults bygger utan fel eller warnings.
