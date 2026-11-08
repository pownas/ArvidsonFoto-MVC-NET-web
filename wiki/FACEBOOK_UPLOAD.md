# Facebook-uppladdning från admin

## Vad funktionen gör

En inloggad administratör öppnar **Dela på Facebook** i UploadAdmin, väljer 1–10 bilder bland de 60 senaste, skriver en text och publicerar till den konfigurerade Facebook-sidan. Bilderna måste kunna hämtas anonymt över HTTPS från `https://arvidsonfoto.se`. Facebook-tjänsten använder Graph API v27.0:

- En bild: `POST /{page-id}/photos` med bildens publika `url` och texten som `caption`.
- Flera bilder: en `POST /{page-id}/photos` per bild med `published=false`, följt av `POST /{page-id}/feed` med `attached_media`.

Tjänsten behöver bara en Page ID och en Page access token. Den skickar tokenen i POST-innehållet och hemligheter ska inte finnas i webbläsaren, loggar, källkod eller dokumentation.

## Förutsättningar i Meta

1. På [Meta for Developers](https://developers.facebook.com/) skapar eller väljer du en app och aktiverar Facebook Pages API/Facebook Login för appen.
2. Använd ett Facebook-konto som har en roll på sidan och siduppgiften **CREATE_CONTENT**. Kontot måste kunna skapa innehåll på den aktuella sidan.
3. Bevilja `pages_manage_posts`, som krävs för att skapa Page-inlägg. Metas aktuella Pages API-flöde för att hämta Page ID och token via Graph API Explorer använder även `pages_show_list`, `pages_manage_metadata` och `pages_manage_read_engagement` för `GET /me/accounts`. Dessa extra behörigheter behövs för det tokenhämtningsflödet, inte för webbplatsens publiceringsanrop. Bevilja inte äldre `manage_pages` eller `publish_pages`.
4. Om appen ska användas av personer som inte har en approll måste appen vara publicerad och ha nödvändig avancerad behörighet/App Review enligt Metas krav. En app i utvecklingsläge kan bara användas av appens administratörer, utvecklare och testare. Meta kan även kräva företagsverifiering.

Meta kan ändra behörighets- och granskningskrav. Kontrollera alltid appens aktuella status, behörigheter och krav i Meta for Developers innan produktionssättning.

## Skapa och kontrollera Page access token

Webbplatsen använder en **Page access token**, inte app secret, app access token eller en kortlivad User access token.

1. Öppna [Graph API Explorer](https://developers.facebook.com/tools/explorer/) och välj webbplatsens Meta-app.
2. Välj de Pages-behörigheter som behövs ovan och generera en User access token för ett konto med rätt sidroll.
3. Följ Metas aktuella flöde för att byta den kortlivade User access token mot en långlivad User access token. Den långlivade användartokenen gäller normalt omkring 60 dagar.
4. Med den långlivade användartokenen anropar du `GET /me/accounts` och väljer rätt sida i svaret. Notera sidans `id` och dess `access_token`. Det är sidans token som ska användas i webbplatsen. Kontrollera även att sidan visar uppgiften `CREATE_CONTENT`.
5. Klistra in Page access token i [Access Token Debugger](https://developers.facebook.com/tools/debug/accesstoken). Kontrollera att app, sida och `pages_manage_posts` är rätt och att tokenen inte redan har upphört eller återkallats. Klistra aldrig in tokenen i en issue, logg, chatt eller dokumentation.

En långlivad Page access token visas normalt utan ett fast utgångsdatum, men är inte garanterat permanent. Den kan sluta fungera när behörigheter återkallas, sidåtkomst eller appstatus ändras, användaren tar bort appåtkomst eller Meta återkallar tokenen. Appen förnyar inte token automatiskt. Kontrollera den regelbundet och skapa en ny via stegen ovan när den återkallats eller inte längre fungerar. Efter tokenbyte uppdaterar du den hemliga konfigurationen och startar om/laddar om applikationen.

## Konfigurera webbplatsen

Applikationen läser `Facebook:PageId` och `Facebook:PageAccessToken`. App ID och App Secret används inte av uppladdningstjänsten och ska inte läggas in i dess konfiguration.

Lokal utveckling (kör från projektet `ArvidsonFoto`):

```bash
dotnet user-secrets set "Facebook:PageId" "<PAGE_ID>"
dotnet user-secrets set "Facebook:PageAccessToken" "<PAGE_ACCESS_TOKEN>"
```

I produktion sätts motsvarande miljövariabler eller hemligheter i plattformens secret manager:

```text
Facebook__PageId=<PAGE_ID>
Facebook__PageAccessToken=<PAGE_ACCESS_TOKEN>
```

Ersätt värdena lokalt; committa aldrig dessa värden till `appsettings*.json`, versionshantering, ärenden eller dokumentation. Begränsa åtkomst till hemligheterna till applikationens driftidentitet och behöriga administratörer. Rotera en exponerad token omedelbart.

Administratören kan öppna sidan via UploadAdmin-menyn. Om konfiguration saknas visas **Facebook är inte konfigurerat**. Efter publicering visas en länk till Facebook-inlägget.

## Verifiera uppladdningen

En riktig publicering kräver en egen Meta-app, en Page access token med tillräcklig behörighet och en sida som kontot får publicera till. Dessa hemligheter och sidåtkomst finns inte i repo; därför ska unit-testresultat inte beskrivas som en verifierad livepublicering.

1. Kontrollera att Page ID och Page access token är satta i driftmiljön och att tokenen passerar Access Token Debugger.
2. Kontrollera att den valda sidan är den avsedda och att appen/tokenen har `pages_manage_posts` samt siduppgiften `CREATE_CONTENT`.
3. Logga in i webbplatsens admin och öppna **Dela på Facebook**.
4. Välj först en bild, skriv ett kort testmeddelande och publicera. Bekräfta att framgångslänken öppnar det nya inlägget på rätt sida och att bilden/texten stämmer.
5. Om albumpublicering behövs, gör ett separat test med två bilder och kontrollera att båda visas i samma inlägg.
6. Ta bort testinläggen vid behov. Testa inte med riktiga kundbilder utan godkännande.

De automatiserade testerna körs från lösningens rot:

```bash
dotnet test ArvidsonFoto.Tests.Unit/ArvidsonFoto.Tests.Unit.csproj --filter "FullyQualifiedName~FacebookServiceTests"
```

Testerna simulerar Graph API-svar och validerar inte Meta-appens behörigheter eller en verklig publicering.

## Felsökning

Admin får ett användarvänligt fel vid misslyckad publicering. Serverloggen innehåller HTTP-status och Facebooks Graph API-felsvar; använd inte loggar för att dela tokenvärden. Kontrollera:

| Symptom | Kontroll |
| --- | --- |
| **Facebook är inte konfigurerat** | Kontrollera att både `Facebook:PageId` och `Facebook:PageAccessToken` finns i samma miljö/process som webbplatsen. Kontrollera stavning, secret injection och omstart. |
| OAuth-fel, utgången/ogiltig token eller fel app | Granska tokenen i Access Token Debugger. Skapa om Page access token från rätt app och långlivad användartoken. Kontrollera att rätt app valdes i Graph API Explorer. |
| Behörighetsfel eller sidan kan inte publicera | Kontrollera `pages_manage_posts`, appens behörighetsstatus, sidans `CREATE_CONTENT`-uppgift och att sidåtkomsten inte återkallats. |
| Graph API säger att sidan inte hittas | Kontrollera att konfigurerat `PageId` hör till den token som används. Hämta ID och token tillsammans från `/me/accounts`. |
| Facebook kan inte hämta bilden | Öppna bildens exakta HTTPS-URL i ett privat webbläsarfönster. Den måste fungera utan inloggning och vara tillgänglig för Facebooks servrar; kontrollera TLS, omdirigeringar, brandvägg och bildformat. |
| Enbildsuppladdning lyckas men länken fungerar inte | Kontrollera att svaret innehåller `post_id` och att det öppnade inlägget är synligt för målgruppen. Ett foto-ID är inte samma sak som ett inläggs-ID. |
| Nätverksfel, begränsning eller serverfel | Kontrollera driftens utgående HTTPS-anslutning och Meta-status. Försök igen senare vid tillfälliga fel; undvik täta upprepade publiceringsförsök eftersom de kan skapa dubbletter. |

Använd Metas [Graph API-felsökningsguide](https://developers.facebook.com/docs/graph-api/using-graph-api/error-handling/) och [Access Token Debugger](https://developers.facebook.com/tools/debug/accesstoken/) för felkod och tokenstatus. Dela aldrig en fullständig token i en supportbegäran.

## Källor

- [Meta Pages API: Getting Started](https://developers.facebook.com/documentation/pages-api/getting-started/)
- [Meta Pages API: Posts](https://developers.facebook.com/documentation/pages-api/posts/)
- [Meta: Long-lived access tokens](https://developers.facebook.com/documentation/facebook-login/guides/access-tokens/get-long-lived/)
- [Meta: Graph API error handling](https://developers.facebook.com/docs/graph-api/using-graph-api/error-handling/)
