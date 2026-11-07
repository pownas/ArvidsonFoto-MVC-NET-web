# Förbättringsförslag för design och adminflöden

## Nuläge

Besökare kan bläddra i en flerspråkig kategorimeny, söka efter bilder, se nya bilder och öppna galleribilder i en större vy. Webbplatsen har responsiv layout, språkval och ljust/mörkt tema. Köp av bilder och information om fotografen finns redan som tydliga delar av besöksflödet.

Administrationen samlar bild- och kategorihantering, översättningsgranskning, Facebook-publicering, gästbok, statistik och loggbok i navigeringen. Bildflödet länkar in en bild med filnamn och metadata; formuläret överför inte själva bildfilen. Bildredigeringen visar 25 senast registrerade poster per sida i en bred tabell med redigering och radering via dialoger.

Statistiken visar månadsvisa sid- och kategorivisningar, topplistor och jämförelse med föregående år. Räknaren summerar sidvisningar, inte unika besökare. Loggboken visar en dags lokala loggfil, låter administratören välja datum och växla mellan filtrerade och fullständiga loggrader.

## Prioriterade förslag

Insats avser uppskattad utvecklingsinsats, inte kalenderlöfte. Börja med små steg och mät om de minskar administrationstid eller gör det lättare att upptäcka problem.

### 1. Gör bildbiblioteket sökbart och lättare att rätta

- **Problem:** Redigera bilder visar en paginerad tabell sorterad efter bild-ID. Att hitta en äldre bild eller hitta poster med saknad metadata kräver manuell bläddring; flytt till rätt art beskrivs som att ta bort länken och skapa en ny.
- **Förslag:** Lägg till sökning på filnamn/beskrivning, filter för kategori, fotograferingsdatum och saknade svenska/engelska beskrivningar. Visa en responsiv miniatyrvy med tabell som alternativ. Lägg till säker kategoriändring där den är datamässigt giltig och förhandsgranskning av att bildfilen finns. Börja med filtrering och saknad metadata innan massredigering.
- **Hjälper:** Administratören vid löpande katalogvård; besökaren får lättare att hitta bilder med korrekta kategorier och beskrivningar.
- **Prioritet / insats / effekt:** Hög / medel / hög.
- **Mät:** Tid att hitta och rätta en bild samt antal saknade beskrivningar och felaktiga bildlänkar över tid.
- **Nulägesstöd:** [RedigeraBilder](../ArvidsonFoto/Views/UploadAdmin/RedigeraBilder.cshtml#L41), [bildlistning och sidindelning](../ArvidsonFoto/Controllers/UploadAdminController.cs#L370), [redigeringsformulär](../ArvidsonFoto/Views/UploadAdmin/_EditImageInputForm.cshtml#L24).

### 2. Samla registrering av bilder i ett guidat flöde

- **Problem:** Ny bild-flödet låter administratören välja en kategori och registrera filnamn, datum och beskrivningar, men laddar inte upp bildfilen. Det är lätt att uppfatta steget som en uppladdning och manuella filnamn kan leda till brutna länkar.
- **Förslag:** Först bekräfta var bildfilerna ska hanteras. Om filerna fortsatt publiceras externt, låt flödet välja eller verifiera en befintlig fil och visa miniatyr, kategori och metadata innan länken sparas. Om filuppladdning ska införas, gör den separat med tydlig status, tillåtna filtyper, storleksgränser, dubblettkontroll och återkoppling vid fel. Stöd därefter flera bilder med gemensamt datum/kategori och individuell metadata.
- **Hjälper:** Administratören, särskilt när många bilder registreras; besökaren får färre saknade eller felkategoriserade bilder.
- **Prioritet / insats / effekt:** Hög / medel till hög, beroende på befintlig filpublicering / hög.
- **Mät:** Andel registreringar som behöver rättas, misslyckade bildlänkar och tid per registrerad bild.
- **Nulägesstöd:** [Ny bild-vyn](../ArvidsonFoto/Views/UploadAdmin/NyBild.cshtml#L32), [formulär för att länka en bild](../ArvidsonFoto/Views/UploadAdmin/_ImageInputForm.cshtml#L3), [sparad bildpost](../ArvidsonFoto/Controllers/UploadAdminController.cs#L141).

### 3. Gör adminstartsidan till en arbetsöversikt

- **Problem:** Startsidan ger en välkomsttext och statistik, medan arbetsflödena ligger som en lång navigationslista. Nya ärenden eller innehåll som behöver kompletteras kräver att administratören själv letar.
- **Förslag:** Behåll den befintliga statistiken men lägg överst till ett litet antal uppgifter med direktlänkar: gästboksinlägg att läsa, bilder med saknade beskrivningar eller trasiga länkar, kategorier som behöver översättning och nyligen loggade allvarliga fel. Visa tydlig tom vy när inget kräver åtgärd och undvik att duplicera fullständiga rapporter på startsidan.
- **Hjälper:** Administratören som snabbt vill se vad som behöver göras och ta sig direkt till rätt vy.
- **Prioritet / insats / effekt:** Hög / medel / hög.
- **Mät:** Tid från inloggning till åtgärd och antal ouppmärksammade eller långvarigt öppna uppgifter.
- **Nulägesstöd:** [Adminstartsida](../ArvidsonFoto/Views/UploadAdmin/Index.cshtml#L18), [adminnavigation](../ArvidsonFoto/Views/Shared/_NavUploadAdmin.cshtml#L16), [olästa gästboksinlägg](../ArvidsonFoto/Views/UploadAdmin/HanteraGB.cshtml#L49).

### 4. Gör statistiken mer handlingsbar och tydlig med vad den mäter

- **Problem:** Nuvarande rapporter ger sid- och kategorivisningar över valbara perioder, jämförelse med föregående år och topplistor, men en visning kan lätt tolkas som en unik besökare eller ett mått på bildintresse.
- **Förslag:** Märk mätetalen som sidvisningar och förklara att samma besökare kan räknas flera gånger. Markera att innevarande månad är ofullständig. Lägg till nedladdning av den valda rapportens tabell som CSV. Som en senare, separat insats kan sökningar utan träff och gallerianvändning följas aggregerat och med integritet i åtanke, om verksamheten behöver den informationen.
- **Hjälper:** Administratören som prioriterar innehåll och vill kunna jämföra eller återanvända rapportdata.
- **Prioritet / insats / effekt:** Hög för tydligare definitioner, medel för export / låg till medel.
- **Mät:** Färre missförstånd om vad statistiken betyder och hur ofta rapporten exporteras eller används för innehållsbeslut.
- **Nulägesstöd:** [statistikfilter och mätetal](../ArvidsonFoto/Views/UploadAdmin/_HomepageStatistics.cshtml#L15), [jämförelser och diagram](../ArvidsonFoto/Views/UploadAdmin/_HomepageStatistics.cshtml#L47), [räknare som ökar per sidvisning](../ArvidsonFoto/Core/Services/PageCounterService.cs#L28).

### 5. Förbättra loggboken från dagsvisning till felsökningsverktyg

- **Problem:** Loggboken har kalender, dagligt urval, ett filter mot viss rutintrafik och ett läge för alla loggrader. Stora dagar kan ändå vara svåra att söka igenom, jämföra eller följa från fel till relaterade händelser.
- **Förslag:** Lägg till textsökning samt filter för nivå och källa, behåll tydlig datum- och tidsordning och visa antal träffar. Gruppera återkommande fel och ge en säker väg från fel till sammanhängande spårnings-ID där sådant finns. Lägg till kopierbar/exporterbar vy. Maskera personuppgifter och hemligheter och behåll åtkomstskyddet innan loggar görs sökbara eller exporteras.
- **Hjälper:** Administratören vid felsökning; snabbare diagnos kan också minska avbrott för besökare.
- **Prioritet / insats / effekt:** Medel / medel / medel till hög.
- **Mät:** Tid att identifiera återkommande fel och antal relevanta loggrader som hittas per felsökning.
- **Nulägesstöd:** [datum och lägen i loggboken](../ArvidsonFoto/Views/UploadAdmin/VisaLoggboken.cshtml#L31), [filtrering och visning av loggrader](../ArvidsonFoto/Views/UploadAdmin/VisaLoggboken.cshtml#L124), [daglig lokal loggläsare](../ArvidsonFoto/Core/Services/AppLogReaderService.cs#L12).

### 6. Putsa administrationsgränssnittet runt vanliga uppgifter

- **Problem:** Webbplatsen har redan responsiv layout, språkval och tema, men administrationen återanvänder webbplatsens navigationsstruktur och visar långa formulär och tabeller. Små skärmar och sällananvändare kan behöva mer vägledning.
- **Förslag:** Gör adminnavigationen till en tydligt avgränsad, mobilanpassad arbetsyta med korta grupper (Bilder, Innehåll, Uppföljning), konsekvent sidrubrik och synlig aktuell plats. Lägg till stegindikator och förhandsgranskning i bildflödet, tydliga sparstatusar och fokusmarkeringar. Behåll språk- och temastöd och kontrollera kontrast, tangentbordsflöde och skärmläsarbeskrivningar innan färger eller layout byts.
- **Hjälper:** Alla administratörer, särskilt vid mobil användning och sällan utförda uppgifter; besökare gynnas av ett fortsatt sammanhållet och tillgängligt galleri.
- **Prioritet / insats / effekt:** Medel / medel / medel.
- **Mät:** Färre avbrutna formulär, färre navigeringsfel och godkänt tangentbordstest för kärnflöden.
- **Nulägesstöd:** [delad navigering](../ArvidsonFoto/Views/Shared/_NavBar.cshtml#L15), [adminlayout](../ArvidsonFoto/Views/UploadAdmin/Index.cshtml#L18), [språk- och temaväljare](../ArvidsonFoto/Views/Shared/_Layout.cshtml#L70).

### 7. Använd befintlig besöksstatistik för att förbättra upptäckbarheten

- **Problem:** Kategorier och senaste bilder är redan centrala vägar in i galleriet, men topplistor över sidvisningar leder inte i sig besökaren till relevant innehåll.
- **Förslag:** Använd de mest besökta kategorierna som redaktionellt underlag för kuraterade ingångar, exempelvis säsongssamlingar eller relaterade kategorier. Testa först som en liten handredigerad sektion; automatisera inte popularitetsranking innan resultat och redaktionell relevans har utvärderats. Behåll befintliga sök-, kategori- och köpvägar.
- **Hjälper:** Besökare som vill upptäcka fler bilder och administratören som vill återanvända statistik för innehållsplanering.
- **Prioritet / insats / effekt:** Lägre / låg för handredigerat test / potentiellt medel.
- **Mät:** Klick vidare från samlingen och besök i rekommenderade kategorier jämfört med en liknande period.
- **Nulägesstöd:** [startsidan med senaste bilder och kategorier](../ArvidsonFoto/Views/Home/Index.cshtml#L63), [kategori- och söknavigering](../ArvidsonFoto/Views/Shared/_NavBar.cshtml#L28), [topplistor i statistiken](../ArvidsonFoto/Views/UploadAdmin/_HomepageStatistics.cshtml#L7).

## Föreslagen ordning

1. Börja med tydligare statistikdefinitioner och sök/filter i bildbiblioteket; båda har avgränsad omfattning och ger snabb nytta.
2. Lägg till kvalitetskö i adminstartsidan och förbättra loggsökning när första iterationen har utvärderats.
3. Bestäm filpubliceringsflöde innan batchregistrering eller riktig bilduppladdning byggs.
4. Förnya administrationslayout och testa en kuraterad besökarsektion efter att användarbehov och mätetal har bekräftats.

För varje steg bör åtkomstskydd, tillgänglighet, svensk/engelsk text och integritet kontrolleras. Förslag som innebär ny insamling av besöksdata ska bedömas separat innan de genomförs.
