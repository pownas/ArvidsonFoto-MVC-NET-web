# 01.05-transitive-conflicts: Lös SBOM-konflikter och verifiera transitive uppdateringar

## Objective
Efter att direkta parent packages har uppdaterats, stäm av den aktiva lösningsgrafen mot SBOM:s nio versionskonflikter och övriga transitiva uppdateringar. Lägg bara till eller ändra centrala transitive pins där uppdaterade föräldrapaket inte räcker.

## Scope
Root `Directory.Packages.props` med `CentralPackageTransitivePinningEnabled=true`; SBOM-konflikterna gäller Microsoft.Extensions.Configuration.CommandLine/EnvironmentVariables/UserSecrets, Microsoft.Extensions.Hosting, Microsoft.Extensions.Logging.Console/Debug/EventLog/EventSource samt System.Diagnostics.EventLog. Hantera aktiva transitive update entries för Aspire, Azure, SQL Client, Microsoft.Extensions, Microsoft.IdentityModel, testing, Polly, telemetry, diagnostics och System packages utifrån rapporten. Move relevant RC1 packages to exact `11.0.0-rc.2.26504.105`. Rör inte StreamJsonRpc från 2.25.29 till rapportens lägre 2.22.23 eller andra paket i riktning bakåt.

## Steps
1. Restore efter direkta uppdateringar och granska resolved direct/transitive package versions.
2. Lägg till minsta nödvändiga CPM pins för återstående SBOM-uppdateringar/konflikter.
3. Kör full lösningsbuild, samtliga tester och NuGet-sårbarhetsskanning.

**Done when**: De nio konflikterna är samordnade där det är kompatibelt, aktiva SBOM-transitiva uppdateringar är applicerade, inga oavsiktliga nedgraderingar finns, full build/test passerar och säkerhetsresultaten rapporteras.
