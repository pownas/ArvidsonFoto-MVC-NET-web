![Last commit](https://img.shields.io/github/last-commit/pownas/ArvidsonFoto-MVC-NET-web?style=flat-square&cacheSeconds=86400)

# ArvidsonFoto-MVC .NET web
ArvidsonFoto är en ASP.NET Core MVC-webbplats för bildgallerier, information och administration. Lösningen riktar sig mot .NET 11 och SDK-versionen anges i `global.json`.

## Release notes och versionshistorik
Se [ändringsloggen (CHANGELOG) och release notes](CHANGELOG.md) för versionshistorik.
  
  
## Instruktion för att starta webbsidan lokalt

### Starta med .NET Aspire AppHost 🚀

Aspire AppHost startar webbappen i två utvecklingsprofiler och ger en dashboard för loggar, traces och metrics. Standardkonfigurationen använder en föränderlig in-memory-databas; AppHost startar inte SQL Server eller Docker.

```bash
# Kör från projektroten
dotnet run --project ArvidsonFoto.AppHost
```

Se [ArvidsonFoto.AppHost/README.md](ArvidsonFoto.AppHost/README.md) för mer detaljer.

### Lokalt testkonto

Vid start i `Development` skapas ett bekräftat Identity-konto automatiskt om det saknas. Öppna inloggningssidan från webbplatsens endpoint i Aspire Dashboard och logga in med:

- E-post: `test@example.com`
- Lösenord: `LocalTest123!`

Kontot skapas bara i `Development` när `DatabaseSettings:SeedTestAccount` är aktiverat. Det är ett lokalt testkonto, inte produktionsuppgifter; byt aldrig till detta lösenord i en driftsmiljö.

#### Detaljerade traces och bildhämtning

I Aspire visar MVC-spans controller/action, resultattyp och rendering av Razor-vyn. Galleriets controller visar kategori-id, sidnummer, sidstorlek och antal bilder. `category.list cache (antal)` respektive `category.list database (antal)` visar direkt om kategorierna kom från cache eller databas. Öppna ett span för att se attribut och exception-events; Structured Logs visar också MVC-resultat och cacheinformation när Debug-loggning är aktiverad. HTTP-spans visar sökväg, innehållstyp och tillgänglig svarsstorlek. Inga formulärvärden, cookies eller extra queryvärden läggs till av denna detaljloggning.

I Development bäddar layouten även in sidans W3C-context i `arvidsonfoto-traceparent`. `pageTrace.js` använder detta för de publika API-anropen från kategoripopup, kategoricache och ljuslådans nästa sida. Anropen blir barn till sidans request-span och behåller samma trace-id, även efter att HTML-svaret har skickats. Vanliga `traceparent`-headers används utan att ändra API-URL:er, cacheinställningar eller själva bildladdningen. Context skickas bara till samma origin och aldrig till externa bildservrar. Befintlig explicit `traceparent` bevaras. Funktionen kan stängas av med `Telemetry:CorrelateBrowserRequests: false` och är avstängd i Production.

`arvidsonfoto.browser.operation` skiljer `tooltip.prefetch` (automatisk förladdning), `tooltip.hover`, `category.cache-load` och `gallery.next-page`. HTTP-/MVC-spans visar dessutom kategorinamn, kategori-id, sidnummer, bildantal och, för enskilda bilder, bild-id och filnamn. `category.get`, `category.children`, `image.count` och `image.latest` förtydligar databas- och kategoriarbetet under respektive anrop. Browser Link (`/_vs/browserLink`) och Hot Reload (`/_framework/aspnetcore-browser-refresh.js`) filtreras bort från inkommande HTTP-traces i Development; sätt `Telemetry:IncludeDevelopmentToolTraces: true` om dessa behöver felsökas. Verktygen i sig stängs inte av.

En hover/förladdning som startar på **föregående sida** tillhör fortfarande den sidans trace. Den flyttas inte felaktigt till den sida man sedan navigerar till. Inte heller cacheträffar som undviker ett nätverksanrop skapar ett nytt server-span.

HTTP-anrop under `/js/` och `/lib/` filtreras bort från traces i alla miljöer. JavaScript-filer och biblioteksresurser serveras och cachelagras precis som tidigare; API-anropen som skripten gör spåras fortfarande. Sätt `Telemetry:IncludeJavaScriptTraces: true` om dessa filhämtningar behöver felsökas. Redan exporterade traces i Aspire ligger kvar tills de rensas eller löper ut.

I **Development** lägger en Tag Helper till `af_traceparent` på lokala bilders `src` och ljuslådans bildlänkar (`href` med `data-gallery`). Bildmiddleware exporterar då ett `image.fetch /sökväg`-span med **samma trace-id och sidans request-span som parent**. Det separata, okorrelerade hosting-spannet exporteras inte för dessa anrop. Även sent laddade lokala bilder kan därför tillhöra sidans trace. Ogiltiga tracevärden loggas och behandlas som vanliga oberoende anrop.

Detta kräver inget JavaScript och ändrar inte hur bilden visas. Sätt `Telemetry:CorrelateImageRequests` till `false` för att stänga av kopplingen. Funktionen är alltid avstängd i Production: ett tracevärde i bildens URL skapar en ny cache-nyckel per sidladdning i Development. Bilder som redan finns i webbläsarens cache ger inget nytt server-span. Bilder från en annan server, inklusive galleriets foton på `https://arvidsonfoto.se`, lämnas oförändrade och kan **inte** få samma server-trace-id genom den lokala appen. För dem krävs separat webbläsarinstrumentering eller stöd för trace-propagation på bildservern.

### Databas och migrationer

Lokal utveckling använder `ConnectionStrings:UseInMemoryDatabase=true` i `appsettings.Development.json`. För en beständig SQL Server-databas krävs en giltig `ConnectionStrings:DefaultConnection` och tillämpade migrationer. Se [databaskonfigurationen](wiki/DatabaseConfiguration.md) innan du ändrar databas eller kör migrationer. SDK-verktyg återställs från verktygsmanifestet med `dotnet tool restore`.

Engelska innehållskolumner, språkval, fallback och migrations-/rollback-steg beskrivs i [inventeringen](wiki/EnglishContentInventory.md).

Inloggade administratörer kan granska databasöversättningar på `/TranslationReview`: välj kategorier, bilder eller gästbok och jämför varje svenskt fält med dess engelska motsvarighet. Vyn visar antalet ogranskade fält, kan filtrera fram dem och paginerar posterna. Spara en ändrad översättning först; markera den därefter som granskad. Saknade engelska värden kan inte markeras som granskade. Om svensk eller engelsk text senare ändras nollställs dess granskningsstatus automatiskt. Migrationen `AddTranslationReviewStatus` skapar fem separata bit-kolumner som börjar på `false`, även för tidigare översättningar. RESX-översättningar omfattas inte av databaskontrollen och publik svensk fallback påverkas inte.

Redaktörer kan söka efter kategorier och arter på `/UploadAdmin/RedigeraKategori` och ändra svenska och engelska namn samt URL-segment. Nya kategorier får motsvarande fält på `/UploadAdmin/NyKategori`; bildens engelska beskrivning kan ändras under `/UploadAdmin/RedigeraBilder`. Bilder har ingen egen rubrik: visat namn kommer från kategorin/arten. Svenska texter används som fallback när engelska saknas. Seed-översättningar påverkar bara nya databaser; översätt befintligt innehåll separat via admin efter granskning. Var försiktig med att ändra publicerade URL-segment: gamla direktlänkar får ännu ingen automatisk omdirigering.

Publika engelska länkar använder `/images`, `/latest` och `/information` med engelska sidnamn och kategorisegment. Svenska `/bilder`, `/Senast` och `/Info` fungerar fortfarande; språkväxlaren byter också URL och behåller queryparametrar. Den synliga webbkartan visar länkar på valt språk och `wwwroot/sitemap.xml` listar fasta sidlänkar på båda språken. Sökvägar till faktiska bildfiler i `/bilder` är oförändrade.

Statiska översättningar finns i `ArvidsonFoto/Resources/*Resource.{sv-SE,en-US}.resx`, uppdelade i gemensam navigation/formuläretiketter samt `Home`, `Gallery`, `Search`, `Latest`, `Info`, `Site`, `Identity`, `UploadAdmin`, `Dev` och `Validation`. Nycklar anger sida, del och användning, till exempel `Gallery.Metadata.Description`, `Gallery.Counter.Images` och `Contact.Form.Placeholder.Name`. Lägg till samma nyckel i båda språkfilerna och använd motsvarande `IStringLocalizer<TResource>` i vy eller controller. Formulärvalidering har dessutom en neutral svensk `ValidationResource.resx` för validering utanför HTTP-förfrågningar. Galleriets JavaScript och kategoripopupen får sina statiska texter via lokaliserade data-attribut från vyerna, inte hårdkodade strängar. Översatt databasinnehåll och URL-segment lagras **inte** i resursfilerna: deras valfria `En`-fält faller tillbaka till svenska via `LocalizedText.Select`.
  
  
## EF Core-verktyg

Återställ verktyg från repositoryts manifest med `dotnet tool restore`. Se [databaskonfigurationen](wiki/DatabaseConfiguration.md) och [språkfältsinventeringen](wiki/EnglishContentInventory.md) innan du kör migrationer.

## .NET SDK

`global.json` kräver SDK `11.0.100` och tillåter prerelease-SDK:er. Installera den SDK-version som projektet anger innan du kör `dotnet restore`, `dotnet build` eller `dotnet run`. NuGet-versioner hanteras centralt i `Directory.Packages.props`; ändra inte enskilda paketversioner utanför den etablerade versionshanteringen.

## Installera developer-certifikat
Om du får felmeddelande när du kör webbsidan lokalt, som säger:  
```
Unable to configure HTTPS endpoint. No server certificate was specified, and the default developer certificate could not be found.
```
Då behöver du installera ett developer-certifikat med kommandot:  
```ps
# Installera och lita på ASP.NET Core HTTPS development-certifikatet
dotnet dev-certs https --trust
```

Använd inte `dotnet dev-certs https --clean` som rutinåtgärd; det tar bort befintliga utvecklingscertifikat.


## Modernisering och utveckling

Äldre moderniserings- och migreringsdokument är historiska ögonblicksbilder, inte aktuell installationsvägledning. Börja med den här README-filen och de aktuella guiderna nedan.

### Dokumentation

- 📖 [ASPIRE.md](wiki/ASPIRE.md) - Aktuell Aspire-konfiguration
- 📖 [DatabaseConfiguration.md](wiki/DatabaseConfiguration.md) - Databas och lokal testdata
- 📖 [TESTING_GUIDE.md](wiki/TESTING_GUIDE.md) - Aktuella bygg-, test- och manuella kontroller
- 📖 [API_ENDPOINTS.md](wiki/API_ENDPOINTS.md) - Publika API-exempel
- 📖 [MODERNIZATION.md](wiki/MODERNIZATION.md) - Historiska moderniseringsförslag
