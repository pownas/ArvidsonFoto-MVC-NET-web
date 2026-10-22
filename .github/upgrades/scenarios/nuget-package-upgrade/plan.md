# NuGet Package Upgrade Plan

## Overview

**Target**: Uppdatera alla tillämpliga SBOM-markerade paket, lösa rapporterade versionskonflikter och åtgärda sårbarheter utan att lämna .NET 11 RC1 när exakt RC2-version finns.
**Scope**: Åtta projekt i `ArvidsonFoto-MVC-NET-web.slnx`, central pakethantering via `Directory.Packages.props` samt nödvändiga versionsdeklarationer i projektfiler.

## Tasks

### 01-update-nuget-packages: Uppdatera aktiva NuGet-paket

Använd `wiki/sbom-report.md` som versionskälla för alla aktiva `Uppdatering`-poster och de nio rapporterade konflikterna. Uppdatera direktberoenden och befintliga transitive pins enligt CPM; uppdatera direkta projekt-/SDK-referenser där CPM inte styr dem. Uppgradera samtliga tillämpliga .NET 11 RC1-paket till exakt `11.0.0-rc.2.26504.105`, utan nedgradering till 10.0.12. Remediera MessagePack-sårbarheten med SBOM:ens fasta version. Ändra inte poster som inte finns i den aktiva grafen om inte rapporten kräver det, och undvik onödiga projektändringar.

Validera genom restore, lösningsbygge, testkörning och NuGet-sårbarhetsskanning. Åtgärda relevanta kompatibilitetsproblem och varningar utan att undertrycka dem. Rapportera på svenska om en uppdatering inte kan göras säkert, inklusive eventuell kvarstående Pkcs-sårbarhet.

**Done when**: Alla aktiva SBOM-uppdateringar och konflikter har hanterats, .NET 11 RC1-referenser har flyttats till exakt RC2 där den finns, MessagePack ligger utanför den sårbara versionen, inga accepterade paket har oavsiktligt nedgraderats, lösningen bygger och tester passerar; kvarvarande säkerhetsfynd och varningar redovisas.
