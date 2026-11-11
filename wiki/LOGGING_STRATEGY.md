# Loggningsstrategi för ArvidsonFoto

## Nuvarande konfiguration

Applikationen konfigurerar Serilog från appsettings. Loggar skrivs till `logs/appLog.txt` med daglig rotation och bevaras enligt inställningen i `Program.cs`; i Development skrivs de även till konsolen. Loggnivåer och namespace-overrides skiljer sig mellan Development och Production och anges i `appsettings.Development.json` respektive `appsettings.Production.json`.

Aspire ServiceDefaults kan dessutom exportera strukturerade loggar till Aspire Dashboard/OTLP beroende på miljö och exporterinställning. Se [OpenTelemetry tracing](OPENTELEMETRY_TRACING.md).

## Riktlinjer

- Använd strukturerade loggar med parametrar, och välj nivå efter händelsens betydelse.
- Logga inte lösenord, tokens, formulärinnehåll eller annan onödig persondata.
- `InputValidationMiddleware` loggar för närvarande misstänkt query-/routevärde och fjärr-IP. Begränsa åtkomst och retention för dessa loggar; ändringar kräver samordning med middleware och tester.
- Använd `Information` för betydelsefulla driftshändelser, `Warning` för avvikande men hanterade situationer och `Error` när en operation misslyckas. `Debug` är främst för utveckling/felsökning.
- Verifiera aktiva nivåer i appsettings i stället för att anta att exempel eller äldre rapporter beskriver produktionsbeteendet.

## Förslag till konvention

```csharp
Log.Information("Contact submission saved with ID {ContactId}", contactId);
Log.Warning("Could not find category with ID {CategoryId}", categoryId);
Log.Error(exception, "Failed to save contact submission");
```

Loggexempel ska använda icke-känsliga identifierare och undvika att skriva användarens meddelande eller credentials.

---

**Senast uppdaterad:** 2025-12-30  
**Version:** 1.0  
**Ansvarig:** Development Team
