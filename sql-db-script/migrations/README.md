# Databasmigreringar

De två SQL-skripten här är genererade från projektets EF Core-migreringar och beskriver applikationens aktuella databasschema:

1. `01-Core.idempotent.sql` – applikationens kärntabeller, inklusive kontaktformulär, översättningar och spelurval.
2. `02-Identity.idempotent.sql` – ASP.NET Identity-tabeller, inklusive passkeys.
3. `03-Upgrade-Backup-20261020.sql` – databevarande uppgradering för den återställda dumpen `Arvidsonfoto-db-backup-with-data-20261020.sql`.

De två sista kärnmigreringarna kompletterar objekt som finns i den aktuella applikationsmodellen men saknas i den äldre migrationskedjan: `20261020222500_CreateTblKontaktIfMissing` ser till att `tbl_kontakt`, dess index och standardvärden finns, och `20261020222600_AddMenuDateUpdatedIfMissing` lägger till `menu_dateUpdated` om den saknas. De bevarar befintliga data; deras `Down`-migreringar tar avsiktligt inte bort tabellen eller kolumnen.

För en ny, tom databas körs `01-Core.idempotent.sql` och därefter `02-Identity.idempotent.sql` mot samma databas. Skripten skapar inte databasen eller SQL Server-användaren.

För databaskopian i projektet ska den återställda databasen i stället få **endast** `03-Upgrade-Backup-20261020.sql` körd. Kör inte generiska `01`/`02`-skript mot den: dumpens historik-ID:n skiljer sig från EF-kedjan och tabellerna finns redan. `03` kontrollerar att dumpen känns igen, lägger till de saknade modellkolumnerna och Identity-/kontaktindexen, samt lägger in nuvarande EF-migrerings-ID:n efter schemaändringarna. Det raderar inte tabellrader och behåller äldre, omappade kolumner.

Återställ alltid dumpen till en separat testdatabas och ta en backup innan körning. `03` har en transaktion, avbryter om förväntade äldre historikposter/tabeller saknas och avbryter om `tbl_PageCounter` har NULL i namn/månad. Jag har torrkört skriptet mot den lokala återställda kopian i en yttre transaktion och rullat tillbaka allt; den körningen lyckades. Databasen har alltså inte ändrats permanent.

Dumpfilen innehåller användar- och kontaktdata och är därför undantagen från Git via `.gitignore`; den ska inte checkas in.

## Viktigt innan uppgradering

Skripten är idempotenta i EF Core-betydelsen: varje migrering körs bara om dess `MigrationId` saknas i `dbo.__EFMigrationsHistory`. De jämför inte tabeller och kolumner mot databasen och reparerar inte ett schema som skapats manuellt.

Kontrollera att databasen har rätt migreringshistorik innan generiska skript körs. Den äldre migreringen `20250714143852_UpdateDatabaseSchema` tar bort seed-rader med ID 1–20 och ändrar flera kolumner till `NOT NULL`; äldre migreringar tar också bort kolumner. Kör därför inte det generiska `01`-skriptet mot en befintlig databas med äldre/annorlunda historik. För den bifogade dumpen är `03` avsett att baselina det befintliga schemat utan att spela upp de historiska seed-raderingarna.

SQL-filerna på en nivå ovanför denna mapp är en databasschemaexport och avviker på vissa punkter från applikationens EF-modell. De aktuella översättnings- och urvalsflaggorna finns i EF-migreringarna och ingår i `01-Core.idempotent.sql`. EF-migreringarna är källan till dessa genererade skript.

## Återskapa skripten

Kör från repository-roten i PowerShell. Produktionsmiljön väljs för att design-time-kommandot ska använda SQL Server-provider i stället för utvecklingsmiljöns InMemory-provider:

```powershell
$env:ASPNETCORE_ENVIRONMENT = 'Production'

dotnet ef migrations script --idempotent `
  --project .\ArvidsonFoto\ArvidsonFoto.csproj `
  --startup-project .\ArvidsonFoto\ArvidsonFoto.csproj `
  --context ArvidsonFoto.Core.Data.ArvidsonFotoCoreDbContext `
  --output .\sql-db-script\migrations\01-Core.idempotent.sql

dotnet ef migrations script --idempotent `
  --project .\ArvidsonFoto\ArvidsonFoto.csproj `
  --startup-project .\ArvidsonFoto\ArvidsonFoto.csproj `
  --context ArvidsonFoto.Areas.Identity.Data.ArvidsonFotoIdentityContext `
  --output .\sql-db-script\migrations\02-Identity.idempotent.sql
```

Båda kontexterna använder samma `dbo.__EFMigrationsHistory`-tabell men har separata migrationskedjor. Kör båda skripten för att skapa hela applikationsschemat.
