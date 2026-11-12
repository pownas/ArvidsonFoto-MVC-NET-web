# Körlogg för taxonomimigrering

Kopiera den här mallen för varje test- eller produktionsmigrering. Spara
förhandsvisningarna från SQL, filflyttens CSV-plan och loggen tillsammans.

## Körinformation

- Miljö: (test / produktion)
- Datum och tid:
- Ansvarig:
- SQL Server-instans:
- Databas:
- Källdump:
- Arbetskopia av dump:
- Databassäkerhetskopia före applicering:
- Plats för säkerhetskopia av bildträdet:
- Skriptversioner / commit:

## Granskningsresultat

- Resultat från `00-review-taxonomy.sql` före migrering:
- Resultat från `00-review-taxonomy.sql` efter SQL-applicering:
- Taxonomi- och bildtextmappningar granskade av:
- Olösta bildtexter godkända eller mappade:
- Föräldralösa `image_art`-rader:
- Ogiltiga `menu_URLtext`-värden:
- Befintliga däggdjurs-ID:n bekräftade oförändrade:
- Engelska namn som behöver översättning:

## SQL-migrering

- Förhandsvisning sparad här:
- `@ApplyChanges` i körd fil:
- Databassäkerhetskopia verifierad:
- Nya artkategorier skapade:
- `image_art`-rader uppdaterade:
- Transaktion slutförd:
- SQL-fel eller avvikelser:

## Filsystemsmigrering

- `PicturesRoot` före migrering:
- `PicturesRoot` efter migrering (förväntat gemena `bilder`):
- `bilder\faglar\`-sökväg verifierad:
- CSV-flyttplan:
- `-Apply` använt:
- `-AllowIncomplete` använt:
- Antal flyttade filer:
- Antal flyttade miniatyrbilder:
- Antal `AlreadyInPlace`:
- Saknade filer:
- Dubbla filnamn/tilldelningar:
- Ogiltiga meny-URL-sökvägar:
- `NotInDatabase`-filer lämnade orörda:
- Övriga avvikelser:

## Efterkontroll

- Kategori- och bildantal jämförda:
- Förhandsvisning av filorganiseraren körd igen:
- Webbplatsens sökvägskonvention verifierad:
- Provbilder och sidor kontrollerade:
- Kvarvarande problem / uppföljning:
- Slutligt godkännande av:
