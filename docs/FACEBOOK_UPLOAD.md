# Facebook-uppladdningsfunktion

## Översikt
Facebook-uppladdningsfunktionen gör det möjligt för administratörer att enkelt dela nyuppladdade bilder direkt till företagets Facebook-sida från UploadAdmin-gränssnittet.

## Funktioner
- Välj 1-10 bilder från de 25 senast uppladdade bilderna
- Skriv en anpassad text som följer med bilderna
- Ladda upp direkt till Facebook-sidan via Facebook Graph API
- Visuell feedback för val av bilder
- Validering och felhantering
- Länk till det skapade Facebook-inlägget efter publicering

## Konfiguration

### 1. Skapa Facebook App
1. Gå till [Facebook for Developers](https://developers.facebook.com/)
2. Skapa en ny app eller använd en befintlig
3. Lägg till "Pages" och "Publish" behörigheter

### 2. Hämta Access Token
1. I din Facebook App, gå till Tools → Graph API Explorer
2. Välj din Facebook-sida i dropdown-menyn
3. Lägg till följande behörigheter:
   - `pages_manage_posts`
   - `pages_read_engagement`
4. Generera en Page Access Token
5. Spara denna token (den är endast synlig en gång)

### 3. Konfigurera applikationen
Lägg till följande i `appsettings.json` eller User Secrets:

```json
{
  "Facebook": {
    "PageAccessToken": "DIN_PAGE_ACCESS_TOKEN",
    "PageId": "DIN_FACEBOOK_SIDA_ID"
  }
}
```

**Hitta ditt PageId:**
- Gå till din Facebook-sida
- Klicka på "About" (Om)
- Scrolla ner till "Page ID" eller hitta det i URL:en

**Säkerhetsvarning:** Lagra **ALDRIG** tokens i versionshantering. Använd User Secrets i utveckling och miljövariabler i produktion.

#### Använda User Secrets (Development)
```bash
cd ArvidsonFoto
dotnet user-secrets set "Facebook:PageAccessToken" "DIN_TOKEN"
dotnet user-secrets set "Facebook:PageId" "DIN_SIDA_ID"
```

#### Använda Miljövariabler (Production)
```bash
export Facebook__PageAccessToken="DIN_TOKEN"
export Facebook__PageId="DIN_SIDA_ID"
```

## Användning

### För administratörer
1. Logga in på UploadAdmin
2. Klicka på "Dela på Facebook" i navigeringsmenyn
3. Välj 1-10 bilder genom att klicka på dem (de markeras med blå kant)
4. Skriv en text som ska följa med bilderna
5. Klicka på "Publicera på Facebook"
6. Efter publicering får du en länk till det skapade Facebook-inlägget

### Felmeddelanden
- **Facebook är inte konfigurerat**: Kontakta administratören för att konfigurera Facebook-integreringen
- **Inga bilder valda**: Välj minst en bild
- **Fel vid publicering**: Kontrollera att Facebook-konfigurationen är korrekt och att token är giltig

## Teknisk dokumentation

### Arkitektur
Funktionen består av:
- **FacebookService**: Service för kommunikation med Facebook Graph API
- **IFacebookService**: Interface för dependency injection
- **FacebookUploadViewModel**: ViewModel för vyn
- **FacebookUploadInputModel**: InputModel för formuläret
- **FacebookUpload.cshtml**: Razor-vy med bildval och formulär
- **UploadAdminController**: Controller-actions för GET och POST

### API-anrop
Tjänsten använder Facebook Graph API v19.0:
- **Enkel bild**: `POST /{page-id}/photos` - Laddar upp en bild direkt
- **Flera bilder**: 
  1. `POST /{page-id}/photos` (med `published=false`) - Laddar upp varje bild opublicerad
  2. `POST /{page-id}/feed` (med `attached_media`) - Skapar inlägg med alla bilder

### Felhantering
- Validering av konfiguration innan API-anrop
- Validering av bildantal (1-10)
- Logging av fel med Serilog
- Användarvänliga felmeddelanden
- Hantering av partiella misslyckanden vid multi-foto-uppladdning

### Säkerhet
- Endast autentiserade administratörer har åtkomst (via `[Authorize]` attribut)
- CSRF-skydd via `[ValidateAntiForgeryToken]`
- Tokens lagras säkert via Configuration (inte i kod)
- Validering av användarinput
- CodeQL-säkerhetsskannad

## Testning

### Unit Tests
Projektet innehåller omfattande unit tests för FacebookService:
```bash
dotnet test --filter "FullyQualifiedName~FacebookServiceTests"
```

Tester täcker:
- Konfigurationsvalidering
- Hantering av tomma bildlistor
- Lyckade uppladdningar (enkel och multi-bild)
- Felhantering vid API-fel

### Manuell testning
1. Konfigurera Facebook-integration enligt ovan
2. Starta applikationen
3. Logga in som administratör
4. Navigera till "Dela på Facebook"
5. Testa att välja bilder och publicera

## Begränsningar
- Maximalt 10 bilder per inlägg (Facebook API-begränsning)
- Kräver giltig Page Access Token
- Bilder måste vara tillgängliga via publika URL:er
- Page Access Token måste förnyas periodiskt (beroende på appkonfiguration)

## Felsökning

### "Facebook är inte konfigurerat"
- Kontrollera att `Facebook:PageAccessToken` och `Facebook:PageId` är korrekt konfigurerade
- Verifiera att värdena inte är tomma strängar

### "Fel vid publicering"
- Kontrollera att Page Access Token är giltig
- Verifiera att appen har rätt behörigheter (`pages_manage_posts`)
- Kontrollera att bildernas URL:er är tillgängliga publikt
- Granska loggar för mer detaljerad felinformation

### Token har löpt ut
- Generera en ny Page Access Token enligt steg 2 ovan
- Uppdatera konfigurationen med den nya token

## Framtida förbättringar
- Stöd för att schemalägga inlägg
- Förhandsvisning av inlägget innan publicering
- Stöd för att lägga till hashtags automatiskt
- Koppla till Facebook Insights för statistik
- Stöd för att publicera till Instagram samtidigt

## Support
För frågor eller problem, kontakta utvecklingsteamet eller skapa en issue i projektet.
