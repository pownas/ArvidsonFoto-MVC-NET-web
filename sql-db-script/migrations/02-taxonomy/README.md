# Migrering av taxonomi och bildmappar

Den här migreringen är förberedd från den kompletta SQL-dumpen. Bildträdet är
ännu inte komplett. Kör därför inte filflytten förrän samtliga bilder har
laddats ned.

## Vad skripten gör

1. `00-review-taxonomy.sql` är skrivskyddat. Det listar befintligt biologiskt
   kategoriträd, bildantal, bildtexter, saknade eller olösta kategori-ID:n,
   svenska mappsluggar och befintliga däggdjurskopplingar.
2. `01-update-species-categories.sql` skapar art- och godkända restkategorier
   i `tbl_menu`. Det ändrar `tbl_images.image_art` endast för exakta
   bildtexter eller uttryckligen angivna alias. Standardläget är förhandsvisning.
3. `02-organize-species-images.ps1` bygger hela mappsökvägen från svenska
   `tbl_menu.menu_URLtext`. Det förhandsvisar filflyttar och flyttar
   databaskopplade original samt matchande `.thumb`-filer endast med
   `-Apply`. Om roten heter `Bilder` döps den efter filflytten om till `bilder`.

Engelska menyvärden gissas inte fram och skrivs inte över. Webbplatsen kan
fortsätta visa `menu_text_en`; mappar och bildsökvägar utgår alltid från
svenska `menu_URLtext`.

## Namngivning av mappar

Varje mappdel kommer från svensk `menu_URLtext`, inte `menu_text_en` eller
valt visningsspråk. Även rotkategorin ingår i sökvägen. Exempel:

```text
bilder\faglar\dagrovfaglar\tornfalk
bilder\daggdjur\baver
bilder\vaxter\blommor\vitsippa
```

Fågelroten har redan `menu_URLtext = faglar`, och filorganiseraren tar med
rotkategorier i alla sökvägar. Fågelbilder planeras alltså under
`bilder\faglar\...`. Vid första körningen anger du den befintliga roten
`Bilder`; med `-Apply` byter skriptet namn på den efter att filflyttarna har
genomförts. Namnbytet görs via en tillfällig syskonmapp eftersom Windows
hanterar namnbyte som endast skiljer på stora och små bokstäver särskilt.
Efteråt används sökvägen till `bilder`.

Filorganiseraren avvisar menyvägar som inte är gemena ASCII-sluggar. Den
använder databasvärdena som de är och inför ingen separat translitterering.
Nya artvärden för `menu_URLtext` skapas med gemener, bindestreck i stället
för blanksteg och translitterering av `å`, `ä`, `ö` och `é`. SQL-migreringen
avbryts om en ny art skulle ge en slug utanför ASCII. Resultatet från
`00-review-taxonomy.sql` visar befintliga sluggar som behöver rättas innan
filflytten.

## Taxonomins omfattning och granskningsprinciper

Backupen innehåller redan art-ID:n för däggdjur och de flesta fåglar,
kräldjur, fjärilar, sländor och träd/buskar. Dessa ID:n behålls. Migreringen
bygger ut breda växt- och insektskategorier, däribland Blommor, Bär och
svampar, Trädgårdsväxter, Skalbaggar och Övriga insekter. Bildtexten
`Blåstång` flyttas från Blommor till en underkategori under Växter.

| Källkategori | Ändring |
| --- | --- |
| Blommor (`303`) | Godkända arter blir underkategorier till Blommor (`303`); `Blåstång` flyttas till Växter (`6`). Blandade blomtexter får restkategorin `Övriga blommor`. |
| Bär och svampar (`337`) | Godkända arter blir underkategorier; `Gul kantarell` normaliseras till `Kantarell`. Blandade eller icke-artbestämda motiv och aktivitetsbilder får `Övriga bär och svampar`. `Kartlav` lämnas för taxonomisk granskning. |
| Trädgårdsväxter (`357`) | Godkända arter blir underkategorier; `Körsbärsträd` och `Äppelblom` kopplas till Träd och buskar (`338`), och `Ros Ingrid Bergman` till `Ros`. Blandade eller generella texter får `Övriga trädgårdsväxter`. |
| Skalbaggar (`37`) | Identifierade skalbaggar och tydliga grupper får underkategorier. Kön utan artbestämning hamnar i `Övriga skalbaggar`. `Eldskinnbagge` flyttas till Övriga insekter (`35`). |
| Övriga insekter (`35`) | Tydliga insektsgrupper blir underkategorier, bland annat Bin, Bromsar, Bärfisar, Flugor, Getingar, Myggor, Myror, Svävflugor, Tvestjärtar och Vårtbitare. |
| Däggdjur (`2`) | Befintliga art-ID:n granskas och behålls; bildkopplingarna ändras inte. |

`01-update-species-categories.sql` är den auktoritativa listan över arter och
bildtextalias. Den återanvänder befintlig kategori för `Körsbärsträd` under
Träd och buskar och skapar `Äppelträd` där endast om den saknas. Ingen blandad
bildtext görs om till en enda art.

Vissa texter anger inte en art eller ens en insekt. Spindlar och fästingar
som ligger under Övriga insekter samt lavar som ligger under Bär och svampar
lämnas därför olösta i stället för att felklassificeras. Granska sådana rader
i det andra resultatset från `00-review-taxonomy.sql`. Om en bild lämnas på en
bred kategori som får underkategorier lägger filorganiseraren den i den breda
kategorins egen mapp.

## Exakt körordning

Följ stegen i ordning. SQL-dumpen är ett körbart SQL-skript, inte en `.bak`-fil.
Den skapar databasen `arvidsonfoto_se_db` och använder ursprungliga
SQL Server-sökvägar `D:\SQL\arvidsonfoto_se_db.mdf` och
`D:\SQL\arvidsonfoto_se_db_log.ldf`. Kör aldrig källdumpen direkt mot
produktion. Stegen nedan skapar en separat arbetskopia för en testdatabas.

### 0. Ange miljövärden

Öppna PowerShell i repository-roten och ange värden som finns i din miljö.
SQL Servers tjänstekonto, inte bara ditt Windows-konto, måste kunna läsa och
skriva i SQL Serverns data- och backupmappar.

```powershell
$server = 'localhost' # Byt till SERVER\INSTANCE för rätt SQL Server.
$database = 'ArvidsonFotoTaxonomyTest'
$sqlDataDirectory = 'D:\SQL' # Mapp på SQL Server-värden.
$backupDirectory = 'D:\SQL\Backups' # Mapp på SQL Server-värden.
$workDirectory = 'C:\TaxonomyMigration'
$sourceDump = (Resolve-Path '.\SQL-db-backup-2026-11-12.sql').Path
$sqlcmd = (Get-Command sqlcmd -ErrorAction Stop).Source

New-Item -ItemType Directory -Path $workDirectory -Force | Out-Null
```

Ange rätt SQL Server-instans och mappar på själva SQL Server-värden. Skapa
`$sqlDataDirectory` och `$backupDirectory` på SQL Server-värden i förväg och
ge SQL Server-tjänstekontot läs- och skrivrättigheter. `New-Item` ovan skapar
endast klientens arbetsmapp.

Exemplen använder Windows-autentisering via `-E`. Om miljön kräver en annan
godkänd autentiseringsmetod byter du anslutningsargumenten. Skriv inte lösenord
i den här filen och checka inte in dem i repositoryt.

### 1. Skapa och återställ en separat testdatabas

Kontrollera först att måldatabasen inte redan finns. Välj ett nytt
`$database`-namn om den gör det:

```powershell
& $sqlcmd -S $server -E -b -Q "IF DB_ID(N'$database') IS NOT NULL THROW 51010, 'Target test database already exists. Choose a new name.', 1;"
if ($LASTEXITCODE -ne 0) { throw 'Databaskontrollen misslyckades; avbryt.' }
```

Skapa en arbetskopia av dumpen. Endast databasnamnet och de två fysiska
databasfilsökvägarna ändras; källdumpen lämnas orörd:

```powershell
$workingDump = Join-Path $workDirectory "$database.sql"
$dumpText = [System.IO.File]::ReadAllText($sourceDump, [System.Text.Encoding]::Unicode)
$dumpText = $dumpText.Replace('[arvidsonfoto_se_db]', "[$database]")
$dumpText = $dumpText.Replace(
    'D:\SQL\arvidsonfoto_se_db.mdf',
    "${sqlDataDirectory}\${database}.mdf"
)
$dumpText = $dumpText.Replace(
    'D:\SQL\arvidsonfoto_se_db_log.ldf',
    "${sqlDataDirectory}\${database}_log.ldf"
)

if ($dumpText.Contains('[arvidsonfoto_se_db]') -or
    $dumpText.Contains('D:\SQL\arvidsonfoto_se_db.mdf') -or
    $dumpText.Contains('D:\SQL\arvidsonfoto_se_db_log.ldf')) {
    throw 'Arbetsdumpen innehåller fortfarande ursprungligt databasnamn eller sökväg. Kör den inte.'
}

[System.IO.File]::WriteAllText(
    $workingDump,
    $dumpText,
    [System.Text.UTF8Encoding]::new($true)
)
```

Kör arbetskopian, inte källdumpen:

```powershell
$restoreLog = Join-Path $workDirectory "$database-restore.log"
& $sqlcmd -S $server -E -b -f 65001 -i $workingDump -o $restoreLog
if ($LASTEXITCODE -ne 0) {
    throw "Teståterställningen misslyckades. Läs $restoreLog; fortsätt inte."
}
```

Återställningen måste avslutas med returkod `0`. Om SQL Server inte hittar
eller inte kommer åt MDF/LDF-sökvägen rättar du `$sqlDataDirectory`, skapar
arbetskopian igen och återställer till ett nytt, tomt databasnamn. Kör inte
migreringen mot en äldre testdatabas. En tidigare förhandsvisning mot
`arvidsonfoto_migrationtest_20261022` stoppades med fel `51000`, eftersom ett
nödvändigt överordnat `menu_ID` saknades. Den databasen är inte en giltig
testgrund. Den misslyckade förhandsvisningen ändrade inte databasen.

### 2. Granska den återställda databasen

Kör den skrivskyddade inventeringen och spara hela resultatet:

```powershell
$auditDirectory = Join-Path $workDirectory "$database-audit"
New-Item -ItemType Directory -Path $auditDirectory -Force | Out-Null
$reviewBefore = Join-Path $auditDirectory '00-review-before.txt'

& $sqlcmd -S $server -E -d $database -b -f 65001 `
  -i '.\Scripts\Taxonomy\00-review-taxonomy.sql' `
  -o $reviewBefore
if ($LASTEXITCODE -ne 0) {
    throw "Taxonomigranskningen misslyckades. Läs $reviewBefore; fortsätt inte."
}
```

Läs alla resultatset i `$reviewBefore`. Avbryt om någon nödvändig
föräldrakategori saknas. Följande ID:n ska finnas med rätt kategorinamn:
`1`, `2`, `3`, `5`, `6`, `35`, `37`, `303`, `337`, `338` och `357`.
Granska även föräldralösa `image_art`-värden och ogiltiga `menu_URLtext`.
Rätta källdatabasen eller migreringsmappen och kör granskningen igen innan
du fortsätter.

### 3. Förhandsgranska och godkänn SQL-mappningen

Det incheckade skriptet har `@ApplyChanges = 0`, vilket är
förhandsvisningsläget. Kör det och spara hela resultatet:

```powershell
$sqlPreview = Join-Path $auditDirectory '01-update-preview.txt'
& $sqlcmd -S $server -E -d $database -b -f 65001 `
  -i '.\Scripts\Taxonomy\01-update-species-categories.sql' `
  -o $sqlPreview
if ($LASTEXITCODE -ne 0) {
    throw "SQL-förhandsvisningen misslyckades. Läs $sqlPreview; applicera inte ändringar."
}
```

Godkänn förhandsvisningen först när allt detta stämmer:

1. Föreslagna antal per källkategori, art och bild motsvarar den granskade
   mappningen. Varje alias går till avsedd målkategori.
2. Alla olösta bildtexter är förstådda. I den medföljande dumpen förväntas
   tomma bildtexter, `Kartlav` samt spindel-/fästingtexter under Övriga
   insekter bli kvar för granskning. De gissas inte till arter.
3. Det finns inga oväntade olösta bildtexter, ogiltiga sluggar eller SQL-fel.
   Alla fel, inklusive `51000`, `51001` eller `51002`, innebär stopp.
4. Antalet föreslagna bildändringar har antecknats i körloggen. Fortsätt inte
   enbart för att `sqlcmd` returnerade returkod `0`.

Om en bildtext behöver en godkänd mappning ändrar du `#Species` och/eller
`#CaptionAliases` i `01-update-species-categories.sql` och kör om
förhandsvisningen från början. Behåll `@ApplyChanges = 0` i den incheckade
filen.

### 4. Säkerhetskopiera och applicera SQL-ändringarna i testdatabasen

Ta och verifiera en databassäkerhetskopia omedelbart före appliceringen:

```powershell
$databaseBackup = Join-Path $backupDirectory "$database-before-taxonomy.bak"
$backupSql = "BACKUP DATABASE [$database] TO DISK = N'$databaseBackup' WITH COPY_ONLY, INIT, CHECKSUM; RESTORE VERIFYONLY FROM DISK = N'$databaseBackup' WITH CHECKSUM;"
& $sqlcmd -S $server -E -d master -b -Q $backupSql `
  -o (Join-Path $auditDirectory 'database-backup.log')
if ($LASTEXITCODE -ne 0) {
    throw 'Databassäkerhetskopiering eller RESTORE VERIFYONLY misslyckades; applicera inte migreringen.'
}
```

Skapa en tillfällig appliceringskopia. Den incheckade filen förblir i
förhandsvisningsläge:

```powershell
$applyScript = Join-Path $workDirectory '01-update-species-categories-apply.sql'
$sqlText = Get-Content -LiteralPath '.\Scripts\Taxonomy\01-update-species-categories.sql' -Raw -Encoding UTF8
$previewDeclaration = 'DECLARE @ApplyChanges bit = 0;'
$declarationCount = [regex]::Matches($sqlText, [regex]::Escape($previewDeclaration)).Count
if ($declarationCount -ne 1) {
    throw 'Förväntade exakt en deklaration för förhandsvisning. Skapa inte appliceringskopian.'
}
$applyText = $sqlText.Replace(
    $previewDeclaration,
    'DECLARE @ApplyChanges bit = 1;'
)
[System.IO.File]::WriteAllText(
    $applyScript,
    $applyText,
    [System.Text.UTF8Encoding]::new($true)
)
```

Kör den tillfälliga kopian exakt en gång mot testdatabasen:

```powershell
$sqlApplyLog = Join-Path $auditDirectory '01-update-apply.txt'
& $sqlcmd -S $server -E -d $database -b -f 65001 -i $applyScript -o $sqlApplyLog
if ($LASTEXITCODE -ne 0) {
    throw "SQL-appliceringen misslyckades eller återställdes. Läs $sqlApplyLog och kontrollera databasen innan nytt försök."
}
```

Kontrollera att resultatet visar `InsertedSpeciesCategories` och
`UpdatedImageRows`. Jämför ändrat antal bildrader med den godkända
förhandsvisningen. Skriptet kör bestående ändringar i en transaktion och
återställer dem vid fel. Återanvänd inte den tillfälliga appliceringskopian
utan att först skapa och granska en ny förhandsvisning av databasens aktuella
tillstånd.

### 5. Verifiera SQL-resultatet innan filer hanteras

Kör inventeringen igen:

```powershell
$reviewAfter = Join-Path $auditDirectory '00-review-after-sql.txt'
& $sqlcmd -S $server -E -d $database -b -f 65001 `
  -i '.\Scripts\Taxonomy\00-review-taxonomy.sql' `
  -o $reviewAfter
if ($LASTEXITCODE -ne 0) {
    throw "Granskning efter SQL-applicering misslyckades. Läs $reviewAfter; flytta inte filer."
}
```

Jämför kategori- och bildantal med förhandsgranskningen och appliceringsloggen.
Artbestämda bilder ska nu peka på sina underordnade `menu_ID`; befintliga
däggdjurs-ID:n ska vara oförändrade. Spara båda granskningsresultaten och
appliceringsloggen. Om antalen inte stämmer ska du stoppa och återställa
databassäkerhetskopian från före appliceringen innan du felsöker vidare.

### 6. Förhandsgranska filflytten

Börja inte förrän hela bildträdet har laddats ned och säkerhetskopierats.
Ange anslutningssträngen för den nyss verifierade testdatabasen. Ange
`$picturesRoot` till den befintliga roten (`Bilder` före migrering eller
`bilder` om den redan har gemener). Spara CSV-planen utanför bildträdet:

```powershell
$connectionString = "Server=$server;Database=$database;Integrated Security=True;TrustServerCertificate=True"
$picturesRoot = 'C:\path\to\Bilder'
$movePlan = Join-Path $auditDirectory '02-image-move-plan.csv'

.\Scripts\Taxonomy\02-organize-species-images.ps1 `
  -ConnectionString $connectionString `
  -PicturesRoot $picturesRoot `
  -PlanCsvPath $movePlan
```

Kommandot är endast en förhandsvisning eftersom `-Apply` saknas. Granska
varje rad och status i planen:

- `Ready`: filen kommer att flyttas.
- `AlreadyInPlace`: ingen åtgärd behövs.
- `NotInDatabase`: filen lämnas kvar; utred om den är orelaterad eller saknar
  databaspost.
- `MissingFile`, `DuplicateFilename`, `DuplicateImageAssignment`,
  `MissingCategoryPath`, `InvalidMenuUrlText`, `InvalidImageUrl`,
  `DestinationExists` och `ThumbnailDestinationExists`: blockerar
  appliceringen.

Applicera inte medan någon blockerare finns. Använd inte `-AllowIncomplete`
för att kringgå blockerare. Rätta orsaken, skapa en ny plan och granska den
igen. Kontrollera att målsökvägarna innehåller exempelvis
`bilder\faglar\dagrovfaglar\tornfalk` och att artbildernas mål använder rätt
svenska slug.

### 7. Säkerhetskopiera och applicera filflytten

Skapa en separat, återställningsbar kopia av hela bildträdet och skriv dess
sökväg i körloggen. Kör sedan samma kommando igen med `-Apply`:

```powershell
.\Scripts\Taxonomy\02-organize-species-images.ps1 `
  -ConnectionString $connectionString `
  -PicturesRoot $picturesRoot `
  -PlanCsvPath $movePlan `
  -Apply
```

Skriptet flyttar endast mediafiler som finns i databasen och matchande
`.thumb`-filer med samma filändelse. Det döper inte om bildfiler och skriver
inte över befintliga mål. Om `$picturesRoot` var `Bilder` döps roten om till
`bilder` efter filflyttarna. Namnbytet görs via en tillfällig syskonmapp; om
det andra namnbytet misslyckas återställs det ursprungliga rotmappnamnet.
Avbryt inte processen medan filer flyttas.

Om ett I/O-fel avbryter flytten ska du behålla båda säkerhetskopiorna. Kör
inte om med `-AllowIncomplete`. Granska först CSV-planen och filsystemet;
kör sedan en vanlig förhandsvisning med det rotmappnamn som faktiskt finns.
Skriptet skiljer på `Ready` och `AlreadyInPlace`, så en granskad omkörning
kan fortsätta utan att skriva över färdiga mål.

### 8. Verifiera den färdiga testmigreringen

Efter en lyckad applicering:

1. Kontrollera att roten heter `bilder` och att fågelgrenen börjar med
   `bilder\faglar\`.
2. Kör filorganiserarens förhandsvisning igen med
   `$picturesRoot = 'C:\path\to\bilder'`. Alla databasregistrerade filer ska
   ha status `AlreadyInPlace`; det får inte finnas saknade filer, dubbla
   tilldelningar, ogiltiga sökvägar eller målkrockar. Granska
   `NotInDatabase`-filer separat.
3. Kör `00-review-taxonomy.sql` igen och jämför kategori- och bildantal med
   inventeringen före applicering och SQL-appliceringsloggen.
4. Kontrollera provbilder från blommor, bär och svampar, trädgårdsväxter,
   skalbaggar, övriga insekter, däggdjur och fåglar.
5. Kontrollera att webbplatsen bygger bildadresser från svenska
   `menu_URLtext` och serverar roten `/bilder/`. Webbplatsens källkod finns
   inte här, så kontrollen måste göras i den faktiska applikationen eller
   driftsmiljön innan produktionsfiler flyttas.
6. Fyll i `MIGRATION-RUN-LOG.template.md` och spara SQL-resultat,
   databassäkerhetskopia, flyttplan, bildträdssäkerhetskopia och godkännande.
7. När återställnings- och appliceringsloggarna är säkert sparade tar du
   endast bort de skapade arbetsfilerna:

   ```powershell
   Remove-Item -LiteralPath $workingDump
   Remove-Item -LiteralPath $applyScript
   ```

### 9. Krav innan produktionsmigrering

Applicera inte i produktion förrän allt detta är uppfyllt:

- Hela testförfarandet ovan har genomförts utan fel mot en ny återställning av
  den medföljande dumpen och ett komplett bildträd.
- Alla kvarvarande olösta bildtexter och `NotInDatabase`-filer har ett
  dokumenterat beslut.
- Produktionsdatabasen och hela bildträdet har varsin verifierad
  säkerhetskopia.
- Den aktiva webbplatsen har verifierats mot den nya URL-konventionen, eller
  dess kod/konfiguration har ändrats och testats separat.
- SQL-förhandsvisningen och filplanen har granskats och godkänts mot
  produktionsdata. Återanvänd inte testantal eller testplaner.

Upprepa därefter steg 2–8 mot produktionsdatabasen och produktionsbildträdet,
med samma krav på förhandsvisning före applicering och med en separat körlogg.
Återställ aldrig arbetskopian av SQL-dumpen mot produktion.

## Säkerhet och begränsningar

- SQL-skriptet ändrar inte bildtexter, översättningar, sidräknare eller
  befintliga däggdjurs-ID:n. Bestående SQL-ändringar är transaktionella.
- Osäkra, blandade eller felklassificerade bildtexter gissas inte till en
  art. Bilder som lämnas på en bred kategori placeras i den kategorins egen
  mapp.
- SQL- och filsystemändringarna är separata åtgärder och delar ingen
  transaktion. Behåll båda säkerhetskopiorna tills samtliga efterkontroller
  är godkända.
- Filorganiseraren skriver inte över mål. Oregistrerade filer rapporteras och
  lämnas orörda.
- Skripten ändrar inte webbplatsens källkod, omdirigeringar, SEO-metadata
  eller ruttgenerering. Det kräver webbplatsens källkod.
