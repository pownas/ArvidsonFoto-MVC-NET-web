# Test- och verifieringsguide

## Bygg och automatiska tester

Kör från lösningens rot med SDK-versionen i `global.json`:

```powershell
dotnet restore
dotnet build
dotnet test ArvidsonFoto.Tests.Unit/ArvidsonFoto.Tests.Unit.csproj
dotnet test ArvidsonFoto.Tests.Integration/ArvidsonFoto.Tests.Integration.csproj
```

End-to-end-testerna finns i `ArvidsonFoto.Tests.E2E`; de kan kräva att rätt Playwright-webbläsare är installerad. Se testprojektens konfiguration innan du tolkar ett fel som ett applikationsfel. Gamla testantal och tidigare körresultat i andra dokument är historiska.

## Manuell kontroll av kategoricachen

1. Starta `dotnet run --project ArvidsonFoto.AppHost` och öppna webbplatsen från Aspire Dashboard.
2. I webbläsarens utvecklarverktyg, kontrollera att `categoryCache.js` laddar kategorier och att localStorage-nyckeln `arvidsonfoto_categories` skapas.
3. Anropa `CategoryCache.getCacheStats()` i konsolen för aktuell cachestatus. Exakt antal poster varierar med seed-/databasinnehållet.
4. Kontrollera Network-fliken för `GET /api/category/AllLightweight` vid cachemiss. Efterföljande laddningar kan använda localStorage.
5. Hovra eller fokusera en kategorilänk och kontrollera att `GET /api/image/GetOneImageFromCategory/{id}` hämtar popupbilden vid behov.
6. Uppdatera en kategori i admin och verifiera servercachebeteendet. Klientcachen i andra webbläsare får inte en omedelbar push-invalidering; dess TTL/version gäller.

## API, loggar och traces

- API-exempel och aktuella rutter finns i [API_ENDPOINTS.md](API_ENDPOINTS.md).
- Öppna Aspire Dashboard → **Traces** och följ en sid- eller API-förfrågan genom dess spans.
- Vid SQL Server används databas-provider/instrumentation enligt den aktiva konfigurationen. In-memory-testdatabasen ger inga SQL-kommandospans.

## Prestandamätning

Äldre dokument innehåller uppskattade queryantal och laddningstider från 2025-12-30; de är inte aktuella benchmarkresultat. Mät en reproducerbar cold-start och warm-cache-situation i den miljö som ska optimeras. Anteckna databas/provider, datamängd, cacheläge, antal samtidiga användare och mätverktyg innan resultaten jämförs. Sätt inte acceptanskriterier utifrån historiska uppskattningar.
