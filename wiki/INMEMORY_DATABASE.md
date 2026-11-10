# In-memory-databas för utveckling

## Lokal utveckling

I `ArvidsonFoto/appsettings.Development.json` är `ConnectionStrings:UseInMemoryDatabase` satt till `true`. `Program.cs` använder då EF Core InMemory för `ArvidsonFotoCoreDbContext` och Identity-contexten. Codespaces-miljövariabler kan också aktivera in-memory-läget.

Kärnappens testdata kommer från `ArvidsonFotoCoreDbSeeder` och läggs till när databasen är tom. Identity-testkontot hanteras separat; det styrs av `DatabaseSettings:SeedTestAccount`. Testdataantal är inte ett stabilt kontrakt, så kontrollera seedern i stället för äldre siffror i dokumentationen.

Starta lokalt med:

```powershell
dotnet run --project ArvidsonFoto.AppHost
```

In-memory-data är inte beständig och försvinner när en appinstans avslutas. Standardkonfigurationen startar inte SQL Server eller Docker.

## Exempel på aktuella API:er

```text
GET /api/category/AllLightweight
GET /api/image/Latest?count=6
GET /api/bilder/{categoryPath}?page=1&limit=48
GET /api/game/categories
GET /api/game/images?count=20
```

Se [API_ENDPOINTS.md](API_ENDPOINTS.md) för parametrar och svarstyper.

## Använda SQL Server

Sätt `ConnectionStrings:UseInMemoryDatabase` till `false` och ange en giltig `ConnectionStrings:DefaultConnection` i lokal, ej versionshanterad konfiguration. AppHost skickar anslutningssträngen vidare men provisionerar inte SQL Server eller kör migrationer åt dig. Se [DatabaseConfiguration.md](DatabaseConfiguration.md) innan du ändrar databasinställningar.