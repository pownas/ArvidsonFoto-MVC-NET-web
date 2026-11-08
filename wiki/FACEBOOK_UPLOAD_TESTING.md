# Testa Facebook-uppladdningen

Följ först konfigurations- och tokeninstruktionerna i [Facebook-uppladdning från admin](FACEBOOK_UPLOAD.md). Använd bara en testbild och en sida där du har tillstånd att publicera.

## Automatiserat test

Från lösningens rot:

```bash
dotnet test ArvidsonFoto.Tests.Unit/ArvidsonFoto.Tests.Unit.csproj --filter "FullyQualifiedName~FacebookServiceTests"
```

Testerna använder en mockad HTTP-klient. De verifierar inte nätverksåtkomst, Meta-behörigheter eller publicering på en riktig sida.

## Manuellt test mot Meta

1. Kontrollera att `Facebook:PageId` och `Facebook:PageAccessToken` finns i webbplatsens aktuella miljö och att Page-tokenen är giltig i Metas Access Token Debugger.
2. Logga in som administratör, öppna UploadAdmin och välj **Dela på Facebook**.
3. Välj en bild och skriv ett tydligt testmeddelande. Klicka **Publicera på Facebook** en gång.
4. Bekräfta att sidan visar framgång och att länken öppnar inlägget på rätt Facebook-sida med rätt bild och text.
5. Testa publicering med flera bilder separat, om albumflödet används. Bekräfta att alla valda bilder finns i samma inlägg.
6. Ta bort testinläggen vid behov.

## Vid fel

- **Facebook är inte konfigurerat:** kontrollera båda konfigurationsnycklarna och att produktionsmiljön har laddat hemligheterna.
- **Publiceringen misslyckas:** kontrollera Graph API-fel i serverloggen. Använd Access Token Debugger för tokenstatus och behörigheter; verifiera även sidans `CREATE_CONTENT`-uppgift och att tokenen hör till konfigurerat Page ID.
- **Bilden går inte att ladda:** kontrollera att dess exakta HTTPS-URL är offentligt tillgänglig utan inloggning.
- Följ felsökningstabellen och säkerhetsråden i [huvuddokumentationen](FACEBOOK_UPLOAD.md). Dela aldrig Page-tokenen i loggar, ärenden eller supportmeddelanden.
