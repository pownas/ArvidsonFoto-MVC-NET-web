# Förslag till sub-issues: adminernas användarupplevelse

Det här dokumentet handlar endast om arbetsflöden i administrationssidorna. För besökarnas visuella upplevelse, se [förslag för webbplatsens design](WEBSITE_DESIGN_IMPROVEMENTS.md).

Förslagen är kandidater för separata sub-issues under issue #257, inte redan beslutade krav. Varje punkt har ett avgränsat mål och en föreslagen acceptansnivå. Prioritet och insats är relativa uppskattningar; tekniska val bör bekräftas innan issue skapas.

## Nuläge

Adminnavigationen samlar bild- och kategorihantering, översättningsgranskning, Facebook-publicering, gästbok, statistik och loggbok. Bildflödet registrerar en sökväg/filnamn och metadata men överför inte bildfilen. Bildredigeringen visar de senaste 25 posterna per sida och saknar sökning; kategoriredigeraren har däremot redan sökning i trädet. Översättningsgranskningen kan redan söka bland bilder och filtrera saknade/ogjorda översättningar.

Statistiken visar månadsvisa sid- och kategorivisningar, topplistor och valfri jämförelse med föregående år. Den räknar sidvisningar, inte unika besökare. Loggboken visar dagliga lokala loggfiler, har kalender och kan växla mellan filtrerade och fullständiga rader.

## Prioriterade kandidater

### A1. Sök och filtrera bildposter i admin

- **Problem:** Bildredigeringen visar poster i fallande bild-ID med 25 poster per sida. Äldre bilder och poster med saknad metadata kräver manuell bläddring.
- **Avgränsning:** Lägg till sökning på filnamn/beskrivning och filter för kategori, datum samt saknad svensk/engelsk beskrivning. Behåll nuvarande redigering och radering; massredigering är inte del av denna första leverans.
- **Acceptans:** Filter kan kombineras och återställas; resultat och sidnumrering följer aktiv sökning/filter; tomt resultat förklaras; befintlig redigering fungerar med filtrerade resultat; förfrågan läser bara aktuell sida/resultatmängd i stället för att först ladda alla bildposter.
- **Hjälper / nytta:** Admin kan hitta en post snabbt och åtgärda metadatafel. Mät tid för att hitta en bild och antal saknade beskrivningar.
- **Prioritet / insats:** Hög / medel.
- **Nulägesstöd:** [bildtabell och nuvarande sidtext](../ArvidsonFoto/Views/UploadAdmin/RedigeraBilder.cshtml#L41), [hämtning och sidindelning](../ArvidsonFoto/Controllers/UploadAdminController.cs#L370).

### A2. Gör bildregistreringen till ett verifierat arbetsflöde

- **Problem:** Formuläret skapar en databaspost med en filreferens, men laddar inte upp bildfilen. Det saknas en kontrollpunkt som bekräftar att filen finns där den förväntas.
- **Avgränsning:** Bekräfta först den befintliga filpubliceringsrutinen. Bygg därefter ett guidat steg som verifierar referens och miniatyr, visar vald kategori och metadata och låter administratören kontrollera innan sparande. Själva filuppladdningen och batchstöd hanteras som separata sub-issues efter beslut om lagringslösning.
- **Acceptans:** Flödet beskriver korrekt om fil väljs/länkas eller laddas upp; felaktig eller saknad fil kan inte sparas utan tydlig förklaring; förhandsgranskning och sparat resultat visar samma kategori och metadata; svenska och engelska etiketter finns.
- **Hjälper / nytta:** Färre trasiga bildlänkar och rättelser. Mät misslyckade bildreferenser och tid för att registrera en bild.
- **Prioritet / insats:** Hög / medel, beroende på filpublicering.
- **Nulägesstöd:** [Ny bild-vyn](../ArvidsonFoto/Views/UploadAdmin/NyBild.cshtml#L32), [formulär för bildreferens](../ArvidsonFoto/Views/UploadAdmin/_ImageInputForm.cshtml#L3), [sparad bildpost](../ArvidsonFoto/Controllers/UploadAdminController.cs#L141).

### A3. Samla prioriterade åtgärder på adminstartsidan

- **Problem:** Adminstartsidan visar välkomsttext och statistik, medan administratören själv behöver navigera till respektive del för att hitta ofärdiga uppgifter.
- **Avgränsning:** Visa ett litet antal datadrivna åtgärdskort med antal och direktlänk, exempelvis olästa gästboksinlägg, saknade bildbeskrivningar, översättningar att granska och allvarliga fel från senaste loggperioden. Visa en lugn tom vy när allt är hanterat.
- **Acceptans:** Varje kort länkar till en befintlig filtrerad vy; antal motsvarar underliggande listor; felaktigt eller otillgängligt datalager blockerar inte övriga adminfunktioner; vyn fungerar på smala skärmar och på svenska och engelska.
- **Hjälper / nytta:** Admin ser vad som behöver göras utan att öppna alla sektioner. Mät tid till första relevanta åtgärd och gamla öppna uppgifter.
- **Prioritet / insats:** Hög / medel.
- **Nulägesstöd:** [adminstartsida](../ArvidsonFoto/Views/UploadAdmin/Index.cshtml#L18), [adminnavigation](../ArvidsonFoto/Views/Shared/_NavUploadAdmin.cshtml#L16), [olästa gästboksinlägg](../ArvidsonFoto/Views/UploadAdmin/HanteraGB.cshtml#L49).

### A4. Tydliggör statistikens mått och möjliggör återanvändning

- **Problem:** Periodval, jämförelse och topplistor finns redan, men summor kan misstolkas som unika besökare och innevarande månad är ofullständig. Rapporttabeller kan inte laddas ned från vyn.
- **Avgränsning:** Märk värden som sidvisningar, förklara begränsningen och markera att pågående månad är ofullständig. Lägg till CSV-export för den period och de mått som visas. Nya analysmått och insamling av ytterligare beteendedata lämnas utanför.
- **Acceptans:** Etiketter och hjälptext finns på svenska och engelska; ofullständig period framgår; CSV har rubriker, vald period, UTF-8 och matchar den synliga filtrerade rapporten; exporten kräver samma adminåtkomst som sidan.
- **Hjälper / nytta:** Admin kan tolka och återanvända rapporter korrekt. Mät exportanvändning och användartest kring förståelse av siffrorna.
- **Prioritet / insats:** Hög / låg till medel.
- **Nulägesstöd:** [periodfilter och mätetal](../ArvidsonFoto/Views/UploadAdmin/_HomepageStatistics.cshtml#L15), [jämförelsesiffror](../ArvidsonFoto/Views/UploadAdmin/_HomepageStatistics.cshtml#L47), [räknare per sidvisning](../ArvidsonFoto/Core/Services/PageCounterService.cs#L28).

### A5. Gör loggboken sökbar och bättre för felsökning

- **Problem:** Dagväljare, filtrering av viss rutintrafik och läge för fullständiga loggar finns, men det går inte att söka på text eller begränsa vyn efter nivå/källa.
- **Avgränsning:** Lägg till textsökning, nivåfilter, källa om tillgänglig samt antal träffar för vald dag. Behåll datumval och existerande filter. Hantera spårnings-ID som extra möjlighet endast om loggarna faktiskt innehåller det.
- **Acceptans:** Filtren kan kombineras och återställas; resultatens datum och nivå är tydliga; ingen träff visas begripligt; känsliga värden exponeras inte i sökparametrar/export; loggbehörighet bibehålls.
- **Hjälper / nytta:** Admin kan hitta återkommande fel och minska felsökningstid. Mät tid att hitta relevanta felrader.
- **Prioritet / insats:** Medel / medel.
- **Nulägesstöd:** [dag- och lägesval](../ArvidsonFoto/Views/UploadAdmin/VisaLoggboken.cshtml#L31), [loggradsvisning](../ArvidsonFoto/Views/UploadAdmin/VisaLoggboken.cshtml#L124), [daglig lokal loggläsare](../ArvidsonFoto/Core/Services/AppLogReaderService.cs#L12).

### A6. Skapa en mobil och tillgänglig adminarbetsyta

- **Problem:** Adminsidorna delar huvudnavigation och använder breda tabeller samt långa formulär. Bildredigeringssidan anger själv att horisontell skrollning kan behövas på telefon.
- **Avgränsning:** Gruppera adminlänkar i Bilder, Innehåll och Uppföljning. Gör aktiv sektion och sidrubrik konsekvent, förbättra mobilpresentation av bildposter och säkerställ tangentbord, fokus, kontrast och skärmläsarstöd. Undvik att ändra publik webbdesign i denna sub-issue.
- **Acceptans:** Alla adminfunktioner nås vid smal viewport utan oavsiktlig horisontell skrollning utom avsiktliga tabeller; aktiv länk har aktuell sidmarkering; kärnflödena kan genomföras med tangentbord; statusmeddelanden kan upptäckas av hjälpmedel; sv/en och båda teman fungerar.
- **Hjälper / nytta:** Admin på mobil och sällananvändare; färre navigeringsmisstag. Mät genomgång av kärnflöden med tangentbord och liten skärm.
- **Prioritet / insats:** Medel / medel.
- **Nulägesstöd:** [delad adminnavigation](../ArvidsonFoto/Views/Shared/_NavUploadAdmin.cshtml#L16), [adminstartsida](../ArvidsonFoto/Views/UploadAdmin/Index.cshtml#L18), [mobilnotis i bildredigering](../ArvidsonFoto/Views/UploadAdmin/RedigeraBilder.cshtml#L41).

### A7. Förbättra kategoriredigeringens återkoppling och arbetsordning

- **Problem:** Kategoriträdet kan redan filtreras och en kategori kan redigeras, men redigering sker i ett formulär ovanför hela trädet och sök/filterstatus kan vara svår att uppfatta när många grenar finns.
- **Avgränsning:** Gör vald kategori och dess hierarkiska sökväg tydlig, gruppera svenska/engelska namn och URL-segment och visa en bekräftad förhandsvisning av resulterande lokala URL:er före sparning. Flytta, massredigera och ändra släktskap ingår inte.
- **Acceptans:** Vald kategori och förälder/hierarki visas; URL-förhandsvisning uppdateras efter redigering; valideringsfel kopplas till fält och bevarar inskrivna värden; befintlig filtrering och URL-kollisionkontroll fungerar.
- **Hjälper / nytta:** Admin kan minska oavsiktliga URL- och översättningsfel. Mät fel vid sparning och efterföljande rättelser.
- **Prioritet / insats:** Medel / låg till medel.
- **Nulägesstöd:** [kategoriformulär och trädfilter](../ArvidsonFoto/Views/UploadAdmin/RedigeraKategori.cshtml#L16), [filter med bevarade föräldrar](../ArvidsonFoto/Views/UploadAdmin/RedigeraKategori.cshtml#L87).

### A8. Gör översättningsgranskning snabbare att återuppta

- **Problem:** Granskningen har redan språkfält, sök/filter, status och sidindelning. När många poster återstår saknas ändå en arbetsöversikt som hjälper admin att prioritera och återuppta just det som är ofärdigt.
- **Avgränsning:** Förbättra befintlig vy med tydlig återstående mängd per posttyp/fält och en konsekvent åtgärd för att öppna nästa ej granskade post. Bygg inte parallell översättningshantering eller automatöversättning.
- **Acceptans:** Sammanfattningen matchar listans verkliga filter; nästa-post-åtgärden respekterar vald typ, sök och status; sparad granskning behåller relevant vy; ingen granskning markeras klar utan explicit administratörsåtgärd.
- **Hjälper / nytta:** Admin kan dela upp översättningsarbetet och fortsätta senare. Mät återstående omarkerade poster och tid per granskad post.
- **Prioritet / insats:** Medel / medel.
- **Nulägesstöd:** [typväljare och översättningsöversikt](../ArvidsonFoto/Views/TranslationReview/Index.cshtml#L10), [sök/filter/status](../ArvidsonFoto/Views/TranslationReview/Index.cshtml#L19).

### A9. Prioritera olästa gästboksinlägg i admin

- **Problem:** Olästa inlägg räknas i navigationen, men hanteringsvyn visar inlägg som kort och kräver scrollning för att hitta olästa poster.
- **Avgränsning:** Lägg till filter och sortering för oläst/läst och datum samt en kompakt listöversikt med tydlig status. Behåll befintlig detaljvisning, svarslänk, läsmarkering och bekräftad borttagning.
- **Acceptans:** Olästa kan visas separat och sorteras; varje inlägg visar status och mottagningsdatum i listan; läsmarkering uppdaterar status; delete kräver bekräftelse och state-changing adminåtgärder har skyddad POST.
- **Hjälper / nytta:** Admin kan hantera nya meddelanden utan att missa dem. Mät ålder på äldsta olästa inlägg.
- **Prioritet / insats:** Medel / medel.
- **Nulägesstöd:** [oläststatus och åtgärder](../ArvidsonFoto/Views/UploadAdmin/HanteraGB.cshtml#L49), [gästboksnavigering med antal olästa](../ArvidsonFoto/Views/Shared/_NavUploadAdmin.cshtml#L30).

### A10. Lägg till granskningsbar bekräftelse för destruktiva adminåtgärder

- **Problem:** Bild- och gästboksradering är viktiga oåterkalleliga åtgärder; i bildvyn bekräftas borttagning i dialog, varefter en länk utför operationen.
- **Avgränsning:** Granska adminraderingar och flytta mutationer till antiförfalskningsskyddade POST-formulär med tydlig målidentitet, bekräftelse och konsekvent framgångs-/felmeddelande. Detta är en avgränsad förbättring av adminflöde och skydd, inte ett nytt behörighetssystem.
- **Acceptans:** Radering kan inte genomföras via GET; POST kräver giltig antiförfalskningstoken; dialogen identifierar exakt bild/inlägg; avbruten åtgärd ändrar inget; resultatet meddelas och tester täcker avvisad GET och lyckad POST.
- **Hjälper / nytta:** Admin minskar risken för felklick och oönskad borttagning; säkrare användning av adminverktygen.
- **Prioritet / insats:** Hög / låg till medel.
- **Nulägesstöd:** [bekräftelsedialog och delete-länk för bild](../ArvidsonFoto/Views/UploadAdmin/RedigeraBilder.cshtml#L121), [raderingsväg i controller](../ArvidsonFoto/Controllers/UploadAdminController.cs#L481).

### A11. Förenkla urvalet inför Facebook-publicering

- **Problem:** Facebook-vyn listar de 60 senaste bilderna för manuellt urval. Den som vill hitta en äldre eller viss kategori kan behöva leta bland alla miniatyrer.
- **Avgränsning:** Lägg till klientfilter/sök på redan laddade poster för namn och kategori samt tydlig sammanfattning av valda bilder. Serverbaserad bläddring av hela arkivet är en separat möjlig fortsättning.
- **Acceptans:** Admin kan söka/filtera utan att förlora val; valt antal och maxgräns uppdateras korrekt; tomt resultat visas; formulärets validering och publicering förblir oförändrad.
- **Hjälper / nytta:** Admin väljer bilder snabbare och ser vad som publiceras. Mät antal steg/tid för att välja avsedda bilder.
- **Prioritet / insats:** Lägre / låg.
- **Nulägesstöd:** [senaste 60 bilderna i urvalet](../ArvidsonFoto/Controllers/UploadAdminController.cs#L611), [bildrutnät och val](../ArvidsonFoto/Views/UploadAdmin/FacebookUpload.cshtml#L73).

## Föreslagen uppdelning och ordning

Skapa en sub-issue per A-punkt efter att omfattning och beroenden bekräftats. Börja med A1, A4 och A10 som tydliga separata leveranser. A2 måste föregås av beslut om filpublicering. A3 beror på att källor och definition av varje åtgärdsantal är klara. A6 kan delas i navigation respektive tillgänglighet om de visar sig vara för stora. A7 och A8 använder befintliga verktyg och kan planeras oberoende. A9/A11 är kompletterande, efter uppskattning av administratörernas dagliga behov.

Utförlig nulägesbeskrivning och sub-issue-kandidater för besökarinriktad visuell design finns i [det separata designförslagsdokumentet](WEBSITE_DESIGN_IMPROVEMENTS.md).
