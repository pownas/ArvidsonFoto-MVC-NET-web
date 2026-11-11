# Databaskonfiguration

## Lokal utveckling

I `ArvidsonFoto/appsettings.Development.json` är `ConnectionStrings:UseInMemoryDatabase` satt till `true`. `Program.cs` väljer in-memory-provider när den inställningen är sann eller när Codespaces-miljövariabler finns. AppHost startar två instanser av webbappen och använder samma utvecklingskonfiguration.

Den gemensamma in-memory-databasen är temporär: innehållet försvinner när respektive appinstans avslutas. Vid uppstart skapas kärnappens testdata om tabellerna är tomma, med data från `ArvidsonFotoCoreDbSeeder`. Identity-testkontot hanteras separat av `ArvidsonFotoIdentityDbSeeder` och styrs av `DatabaseSettings:SeedTestAccount`. Testdata och antal poster kan ändras; se seedern för aktuell uppsättning i stället för att förlita dig på äldre antal i dokumentation.

Starta den vanliga utvecklingsmiljön från lösningens rot:

```powershell
dotnet run --project ArvidsonFoto.AppHost
```

Se [Aspire-guiden](ASPIRE.md) för appinstanser och dashboard.

## Ansluta till SQL Server

För en beständig databas behöver du:

1. Ställa in `ConnectionStrings:UseInMemoryDatabase` till `false`.
2. Ange en giltig `ConnectionStrings:DefaultConnection` i lokal, ej versionshanterad konfiguration.
3. Säkerställa att SQL Server är åtkomlig och att databasschemat har aktuella migrationer.

AppHost läser dessa värden från `ArvidsonFoto`-konfigurationen och skickar anslutningssträngen vidare till appinstanserna när in-memory-läget är avstängt. AppHost skapar inte själv någon SQL Server-container.

Använd User Secrets för lokal utveckling och en lämplig hemlighetshanterare eller miljövariabler i drift. Lägg aldrig lösenord eller produktionsanslutningssträngar i versionshanterade filer.

## Felsökning

- **Ingen testdata:** kontrollera `ASPNETCORE_ENVIRONMENT=Development`, `UseInMemoryDatabase=true` och startloggarna för seedern.
- **Data försvinner efter omstart:** det är förväntat med in-memory-provider.
- **SQL Server-anslutningsfel:** kontrollera `DefaultConnection`, serveråtkomst och att migrationer har tillämpats.
- **Identity-testkonto skapas inte:** kontrollera `DatabaseSettings:SeedTestAccount` och Identity-seederns loggning.

