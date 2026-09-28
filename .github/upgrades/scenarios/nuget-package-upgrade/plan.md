# NuGet Package Upgrade Plan

## Overview

**Target**: Update active NuGet dependencies to the latest stable versions assessed as compatible with the solution's `net11.0` target while preserving the existing .NET 11 RC1 package set.
**Scope**: Eight projects in the solution using Central Package Management.

## Tasks

### 01-update-nuget-packages: Uppdatera aktiva NuGet-paket

Uppdatera centralt hanterade paketversioner i `Directory.Packages.props` enligt de föreslagna stabila versionerna i `assessment.md`. Behåll ASP.NET Core-, Entity Framework Core- och ILLink-paket som redan ligger på `11.0.0-rc.1.26425.128`; den stabila bedömningen föreslog i stället en 10.0.12-downgrade, vilken användaren uttryckligen valt bort. Lämna paketposter som bedömningen inte kunde koppla till ett scoped projekt oförändrade. Använd endast projektets normala NuGet-källor.

Bedömningen identifierade inga API-brott för de aktiva paket som ska uppdateras. Bekräfta detta genom restore, fullständig lösningsbuild och tester; åtgärda eventuella fel och varningar som uppstår.

**Done when**: Alla aktiva paket med en nyare bedömd stabil version är uppdaterade centralt; valda .NET 11 RC1-paket och oidentifierade/inaktiva centralposter är oförändrade; restore, lösningsbuild och tester lyckas utan kvarvarande build warnings.
