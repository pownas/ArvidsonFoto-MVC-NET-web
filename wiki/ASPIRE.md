# .NET Aspire i ArvidsonFoto

Aspire AppHost finns i lösningen och startar två instanser av samma webbprojekt: webbplatsen och en utvecklingsprofil för admin/API. AppHost startar inte en SQL Server-container. I standardkonfigurationen använder båda instanserna en temporär in-memory-databas.

## Starta lokalt

Förutsättning: installera SDK-versionen som anges i `global.json` (11.0.100).

Från lösningens rot:

```powershell
dotnet run --project ArvidsonFoto.AppHost
```

Öppna Aspire Dashboard från dess endpoint i konsolen eller dashboarden. Standardportarna dokumenteras i [AppHost README](../ArvidsonFoto.AppHost/README.md); använd endpoint-adresserna som Aspire visar om portar har ändrats. Utvecklingsinstansen exponerar Scalar på `/scalar/v1` och OpenAPI JSON på `/openapi/v1.json`.

I Visual Studio väljer du `ArvidsonFoto.AppHost` som startprojekt. Docker behövs inte för standardkonfigurationen.

## Vad ingår

- `ArvidsonFoto.AppHost`: startar webbplatsinstansen och utvecklings-/API-instansen.
- `ArvidsonFoto.ServiceDefaults`: gemensam Aspire-konfiguration, OpenTelemetry och health endpoints.
- `ArvidsonFoto`: ASP.NET Core MVC-applikationen med Razor Pages, API och adminfunktioner.

Traces, metrics och strukturerade loggar kan visas i Aspire Dashboard. Se även [OpenTelemetry-guiden](OPENTELEMETRY_TRACING.md).

## Databas

`ArvidsonFoto/appsettings.Development.json` sätter `ConnectionStrings:UseInMemoryDatabase` till `true`. Seedad testdata skapas lokalt vid uppstart och är inte beständig. För SQL Server:

1. Sätt `ConnectionStrings:UseInMemoryDatabase` till `false` i lokal konfiguration.
2. Ange en giltig `ConnectionStrings:DefaultConnection` via User Secrets, miljövariabel eller annan lokal konfiguration.
3. Säkerställ att nödvändiga databasscheman/migrationer finns innan du startar appen.

Använd aldrig riktiga lösenord eller anslutningsuppgifter i versionshanterade appsettings-filer. Se [databaskonfigurationen](DatabaseConfiguration.md) för detaljer.

## Felsökning

- Om appen inte startar, kontrollera fel och endpoint-adresser i AppHost-konsolen och Aspire Dashboard.
- Om testdata saknas, kontrollera att miljön är `Development` och `UseInMemoryDatabase` är `true`.
- Om SQL Server-anslutning misslyckas, kontrollera `DefaultConnection`, databastillgänglighet och att schemat är uppdaterat.
- Om Scalar eller OpenAPI saknas, kontrollera att du öppnat utvecklingsinstansen; dessa endpoints registreras endast i Development.
