# Säkerhetsnoteringar

Detta dokument beskriver några skyddsmekanismer som finns i källkoden. Det är inte en säkerhetscertifiering, fullständig granskning eller garanti för att applikationen saknar sårbarheter.

## Databasfrågor

Använd Entity Framework Core och parametriserade LINQ-frågor för databasåtkomst. Om rå SQL behövs ska parametrar användas för data; bygg aldrig SQL genom att sammanfoga användarindata. En ORM minskar risken för SQL-injektion i korrekt skrivna frågor, men ersätter inte granskning av varje databasåtkomst.

## Indatamönster

`ArvidsonFoto/Security/InputValidationMiddleware.cs` innehåller en heuristisk kontroll av query-parametrar och routningsvärden för vissa SQL-liknande mönster. Träffar avvisas med HTTP 400. Detta är ett extra filter, inte en SQL-parser eller primärt skydd mot injektion: det kan missa nya eller obfuskerade mönster och kan även ge falska positiva resultat. Det granskar inte request bodies eller alla typer av indata.

Middleware-loggar för närvarande namn på misstänkt parameter, värde och fjärr-IP. Begränsa åtkomst och lagringstid för loggar och undvik att lägga personuppgifter eller hemligheter i URL-parametrar. Ändringar av loggningsbeteendet bör samordnas med middleware och tester.

## HTTP- och formulärskydd

- Antiförfalskningstoken används på de formuläråtgärder där attributet `ValidateAntiForgeryToken` är angivet; ASP.NET Core aktiverar inte automatiskt detta för alla POST-anrop.
- Data annotations och model binding kan validera modeller där de används och där valideringsresultat hanteras. De sanerar inte HTML och ASP.NET Core MVC har inte klassisk automatisk request validation.
- Koda utdata i rätt kontext och validera indata för dess användningsområde; behandla inte användarskickad HTML som betrodd.

## Työskentelystä

- Lägg hemligheter i User Secrets (utveckling) eller godkänd hemlighetshanterare/miljökonfiguration (drift), aldrig i källkod eller versionshanterade appsettings.
- Behåll autentisering/auktorisering på skyddade adminåtgärder och använd POST samt antiförfalskningstoken för tillståndsändringar.
- Lägg till tester för validering och auktorisering när skyddat beteende ändras.
- Granska beroenden och säkerhetsråd separat; en lyckad byggning eller ett tidigare skanningsresultat är inte bevis på aktuell säkerhet.

Relevanta tester finns i `ArvidsonFoto.Tests.Unit/Security/InputValidationMiddlewareTests.cs`. Se även [loggningsstrategin](LOGGING_STRATEGY.md) och den historiska [implementationssammanfattningen](IMPLEMENTATION_SUMMARY.md).
