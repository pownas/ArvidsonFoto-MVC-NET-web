# Ändringslogg

Alla noterade ändringar för projektets publicerade versioner. Formatet följer
[Keep a Changelog](https://keepachangelog.com/sv/) och versioneringen följer
[Semantic Versioning](https://semver.org/lang/sv/). Releaser är publicerade
på [GitHub - ArvidsonFoto-MVC-NET-web Releases](https://github.com/pownas/ArvidsonFoto-MVC-NET-web/releases).

## [Unreleased](https://github.com/pownas/ArvidsonFoto-MVC-NET-web/compare/v4.11.0...HEAD)


## [v4.11.0](https://github.com/pownas/ArvidsonFoto-MVC-NET-web/compare/v3.11.2...v4.11.0) - 2026-11-11

### Added
- Engelskt innehåll, språkval och svensk reservtext i menyer, galleri, gästbok och API.
- Granskningsflöde för databasöversättningar samt engelska innehållsfält och URL-segment.
- Förhandsvisningar av kategorier när pekaren hålls över kategorier i sidkartan.
- Sammanhängande OpenTelemetry-spårning för sidvisningar, API-anrop och lokala bilder.
- Kategorins spelbarhet i API-svar för slumpmässiga bilder.

### Changed
- Förbättrade UploadAdmin-flöden, återkoppling och formulär för bilduppladdning.
- Uppdaterade NuGet-paket och låsfiler. .NET 11-, ASP.NET Core- och EF Core-paket är fortsatt låsta till projektets RC2-version.
- Uppgraderade MailKit och MimeKit till 5.x.

### Fixed
- Bildvisaren återgår till rätt gallerisida och använder rätt bild-URL:er efter sidväxling.
- Bildunderskrifter och adminvyer hanterar tomma värden tydligare.
- Anpassade e-postanrop till MailKit 5:s nya API.

## [v4.11.0-rc.3](https://github.com/pownas/ArvidsonFoto-MVC-NET-web/compare/v4.11.0-rc.2...v4.11.0-rc.3) - 2026-11-08

### Added
- Förhandsvisning av kategorier vid hovring i sidkartan.
- Dokumentation av prioriterade design- och adminförbättringar.

### Changed
- Förbättrade adminens arbetsflöden och återkoppling.

## [v4.11.0-rc.2](https://github.com/pownas/ArvidsonFoto-MVC-NET-web/compare/v4.11.0-rc.1...v4.11.0-rc.2) - 2026-10-31

### Added
- Kategorins spelbarhet i svar från slumpbilds-API:et.
- Sammanhängande OpenTelemetry-spårning.
- Förslag och dokumentation för statistikvy med valbar period och årsjämförelse.

### Changed
- Förbättrade UploadAdmin, filtrerade bort tomma bildtexter och gjorde formuläret för ny bild kompaktare.
- Visar en taggikon för kategorier utan underkategorier.
- Uppgraderade .NET 11 SDK till RC3.

### Fixed
- Bildvisaren återgår till aktuell sida när den stängs.
- Rättade bildvisarens URL:er efter sidväxling.

## [v4.11.0-rc.1](https://github.com/pownas/ArvidsonFoto-MVC-NET-web/compare/v3.11.2...v4.11.0-rc.1) - 2026-10-26

### Added
- Valfria engelska fält för gästboksinlägg, menynamn, URL-segment och bildbeskrivningar.
- Språkval via sidhuvud, URL eller cookie, med svensk text som reserv när engelsk text saknas.
- Granskning av databasöversättningar.
- Databasmigration, seeddata och dokumentation för engelskt innehåll.

## [v3.11.2](https://github.com/pownas/ArvidsonFoto-MVC-NET-web/compare/v3.11.1...v3.11.2) - 2026-10-25

### Changed
- Uppgraderade lösningen till .NET 11 RC2 och anpassade CI- och testkonfigurationen.
- Reducerade byggvarningar genom riktade justeringar.

### Added
- Automatisk start av webbappen vid Playwright-tester.

## [v3.11.1](https://github.com/pownas/ArvidsonFoto-MVC-NET-web/compare/v3.10.11...v3.11.1) - 2026-09-13

### Added
- SBOM-skanner för att inventera projektets beroenden.

### Changed
- Uppgraderade lösningen till .NET 11 Preview 7.

## [v3.10.11](https://github.com/pownas/ArvidsonFoto-MVC-NET-web/compare/v3.10.10...v3.10.11) - 2026-06-13

### Added
- SBOM-skannern kan generera en Markdown-rapport.

### Changed
- Uppdaterade NuGet-paketen.

## [v3.10.10](https://github.com/pownas/ArvidsonFoto-MVC-NET-web/compare/v3.10.8...v3.10.10) - 2026-06-13

### Added
- Open Graph-, Twitter Card- och metabeskrivningar för publika sidor.
- Oändlig rullning i galleriet och tydligare sid- och bildräknare på Senast-sidorna.
- Visning av centrala konfigurationsvärden i Aspire Dashboard.

### Changed
- Förbättrade galleri-, miniatyrbilds- och ljuslådepresentationen, inklusive mobil- och liggande vyer.
- Flyttade projektdokumentation från `docs` till `wiki`.
- Delade upp testkörningen i CI i separata steg för enhets- och integrationstester.

### Fixed
- Rättade visning och antal bilder för huvudkategorier och kategorier utan egna bilder.
- Rättade saknade bildräknare, sidinformation, miniatyrbilder och NULL-värden vid skapande av kategorier.
- Rättade ett HTTP 500-fel på sidan för hantering av passkeys.
- Åtgärdade nullable-varningar i lösningen.

## [v3.10.8](https://github.com/pownas/ArvidsonFoto-MVC-NET-web/compare/v3.10.7...v3.10.8) - 2026-03-16

### Added
- Stöd för passkeys enligt ASP.NET Core Identity.
- Cache-busting och modernare hantering av statiska webbassets för .NET 10.

### Changed
- Flyttade dokumentation till `wiki` och tog bort det oanvända LogReader-projektet.

### Fixed
- Visar saknad fotodag i stället för standarddatumet 1900-01-01.
- Förbättrade bildtextens bredd och centrering i miniatyrbilder.
- Rättade Aspire-startfel för IDE-körningar.
- Tog bort en ogiltig sökvägsloggfil som störde Git på Linux och macOS.

## [v3.10.7](https://github.com/pownas/ArvidsonFoto-MVC-NET-web/compare/v3.10.6...v3.10.7) - 2026-01-09

### Changed
- Förbättrade bildsökning, uppladdningsflöde, metadata och loggnivåer.
- Normaliserade URL:er och gjorde kategorisökning och routning mer robust.
- Anpassade DTO:er för nullbara bildbeskrivningar och bild-URL:er.
- Delade upp GitHub Actions-testflödet i tydligare steg.

### Fixed
- Rättade validering i admin, fallback för bild-URL:er och sökresultatens visning.

## [v3.10.6](https://github.com/pownas/ArvidsonFoto-MVC-NET-web/compare/v3.10.5...v3.10.6) - 2025-12-30

### Fixed
- Rättade bildinmatning i kontaktformuläret.
- Rättade inkonsekvent filtrering av bilder per kategori.

## [v3.10.5](https://github.com/pownas/ArvidsonFoto-MVC-NET-web/compare/v3.10.3...v3.10.5) - 2025-12-30

### Added
- Facebook-inlägg från UploadAdmin med val av bild.
- Databasbackup och förbättrad återkoppling i kontaktformuläret.
- Aspire-upplägg med flera instanser, utvecklarportal och Scalar-dokumentation.
- SQL-stöd för kontaktformuläret.

### Changed
- Moderniserade startsidan och informationssidorna; förbättrade mobilmenyn.
- Flyttade modeller och tjänster till Core och minskade databasbelastningen i galleriet.
- Moderniserade startkonfigurationen till `Program.cs` och lade till .NET Aspire 13.
- Förbättrade lokal utvecklingsmiljö och tester för kontaktformuläret.

### Fixed
- Rättade teckenkodning för svenska och tog bort Å, Ä och Ö ur URL-segment.
- Rättade publicering av gästboksinlägg och visning av kategorinamn i mobilmenyn.

## [v3.10.3](https://github.com/pownas/ArvidsonFoto-MVC-NET-web/compare/v3.10.2...v3.10.3) - 2025-12-14

### Fixed
- Rättade sökfunktionen och popup-/popover-beteendet.

## [v3.10.2](https://github.com/pownas/ArvidsonFoto-MVC-NET-web/compare/v3.10.1...v3.10.2) - 2025-12-14

### Added
- Bildförhandsvisningar vid hovring över underkategorier.
- Diagram över sex månaders sidvisningar i UploadAdmin.
- Mörkt läge med sparat val och formulär anpassade för Bootstrap 5 och WCAG.
- API-kontrollers för kategorier och bilder.

### Changed
- Uppgraderade till .NET 10 och Bootstrap 5.
- Införde deterministiska byggen och CI-inställningar.

### Security
- Lade till indatavalidering för att förebygga SQL-injektion.

## [v3.10.1](https://github.com/pownas/ArvidsonFoto-MVC-NET-web/releases/tag/v3.10.1) - 2025-12-14

### Added
- Första publicerade releasen av den här projektlinjen, med inloggning, UploadAdmin, bilduppladdning, gästbok, loggbok och sidräkning.
- Felhantering, omdirigeringar, miniatyrbilder, metadata och administrativa funktioner för bilder och gästboksinlägg.
- Testprojekt och API-funktioner för kategorier och bilder.

### Changed
- Moderniserade projektet stegvis från äldre .NET-versioner till .NET 6, 8, 9 och 10.
- Flyttade sidräkning till en tjänst och förbättrade routing, HTML-standard och webbserverloggning.

