# Databasmigreringar

De två SQL-skripten här är genererade från projektets EF Core-migreringar och beskriver applikationens aktuella databasschema:

1. `01-Core.idempotent.sql` – applikationens kärntabeller, inklusive kontaktformulär, översättningar och spelurval.
2. `02-Identity.idempotent.sql` – ASP.NET Identity-tabeller, inklusive passkeys.

Kärnmigreringen `20261020222500_CreateTblKontaktIfMissing` ser till att `tbl_kontakt`, dess index och standardvärden finns. Den skapar tabellen om den saknas och lämnar befintliga kontaktdata orörda. Dess `Down`-migrering tar avsiktligt inte bort tabellen.

Kör dem mot samma valda SQL Server-databas, i ordningen ovan. Skripten skapar inte databasen eller SQL Server-användaren. Ta en backup före körning i en befintlig miljö.

## Viktigt innan uppgradering

Skripten är idempotenta i EF Core-betydelsen: varje migrering körs bara om dess `MigrationId` saknas i `dbo.__EFMigrationsHistory`. De jämför inte tabeller och kolumner mot databasen och reparerar inte ett schema som skapats manuellt.

Kontrollera därför att databasen har korrekt migreringshistorik för sitt schema innan skripten körs. Om tabellerna skapats från SQL-filerna i `sql-db-script` men motsvarande migreringar inte registrerats i `__EFMigrationsHistory`, kör inte skripten direkt. Den äldre migreringen `20250714143852_UpdateDatabaseSchema` tar bort seed-rader med ID 1–20 och ändrar flera kolumner till `NOT NULL`; äldre migreringar tar också bort kolumner. Utan korrekt historik kan det leda till dataförlust eller körfel. Stäm av databasen och historiken först, eller prova på en kopia.

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
