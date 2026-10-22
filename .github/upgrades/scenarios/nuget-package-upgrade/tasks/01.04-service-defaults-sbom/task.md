# 01.04-service-defaults-sbom: Uppdatera ServiceDefaults och telemetriberoenden enligt SBOM

## Objective
Uppdatera ServiceDefaults-paket från de redan slutförda föregående målen till SBOM-versionerna utan att ändra paket som SBOM inte markerar för uppdatering.

## Scope
`ArvidsonFoto.ServiceDefaults` och root `Directory.Packages.props`. SBOM anger Microsoft.Extensions.Http.Resilience och Microsoft.Extensions.ServiceDiscovery 10.11.0 samt OpenTelemetry.Exporter.OpenTelemetryProtocol, OpenTelemetry.Extensions.Hosting och OpenTelemetry.Instrumentation.* 1.20.0. Aktiva relaterade Polly-pins uppdateras enligt rapporten där CPM styr dem. .NET 11 RC1 Microsoft.Extensions-paket flyttas till exakt `11.0.0-rc.2.26504.105`.

## Steps
1. Bekräfta befintliga direkta referenser och aktiva transitive package graph.
2. Uppdatera versionerna centralt till SBOM-målen.
3. Återställ och bygg ServiceDefaults samt beroenden; kör relevanta tester.

**Done when**: Aktiva ServiceDefaults- och telemetry-paket matchar SBOM/RC2-målet och berört projekt bygger utan fel eller kvarvarande warnings.
