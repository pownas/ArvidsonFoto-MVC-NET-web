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

## Fler förslag för att modernisera hela webbplatsens design

Följande idéer gäller besökarnas helhetsupplevelse, inte bara admin. De bygger vidare på sådant som redan finns: fotografin är innehållets huvudfokus, galleriet har ljusbox och köpvägar, och webbplatsen har svenska/engelska samt ljust/mörkt tema. Börja med klickbara skisser och testa dem på mobil och dator innan en större visuell omarbetning.

### 8. Skapa ett konsekvent visuellt designsystem

- **Problem:** Stilmallen blandar Bootstrap-standarder med egna mörka ytor, grå text och blå accentfärger. Befintliga delar som kort, knappar, länkar och rubriker kan därför upplevas som separata komponenter snarare än en sammanhållen fotografisk identitet.
- **Förslag:** Definiera ett litet set återanvändbara färg-, typografi-, avstånds- och radievariabler. Ta fram en varm, diskret palett och tydlig rubrikhierarki som låter bilderna dominera; bevara ljust och mörkt tema och kontrollera kontrast i båda. Prova tokens på startsida, galleri och informationssidor innan resten av gränssnittet uppdateras.
- **Hjälper:** Alla besökare genom tydligare läsbarhet och igenkänning; utvecklare får ett enklare sätt att hålla nya och befintliga vyer konsekventa.
- **Prioritet / insats / effekt:** Hög / medel / hög.
- **Mät:** Kontrastkontroll i båda teman och kort användartest där besökare får hitta galleri, kontakt och köp av bilder.
- **Nulägesstöd:** [globala färger och tema](../ArvidsonFoto/wwwroot/css/site.scss#L37), [mörkt temastöd](../ArvidsonFoto/wwwroot/css/site.scss#L129), [kortens egna färg- och hoverregler](../ArvidsonFoto/wwwroot/css/site.scss#L772).

### 9. Gör startsidan mer redaktionell och fotocentrerad

- **Problem:** Startsidan kombinerar introduktion, slumpmässiga bilder och fyra likvärdigt utformade informationskort. Det ger flera möjliga ingångar men lyfter inte nödvändigtvis en aktuell bild eller en tydlig första väg in i fotoarkivet.
- **Förslag:** Testa en luftig introduktionsyta med en utvald bild i stort format, en kort presentation och två tydliga handlingar, till exempel ”Utforska bilder” och ”Köp en bild”. Följ den med en handplockad samling eller senaste bilder. Anpassa ordning och text för svenska och engelska; undvik automatisk karusell och låt besökaren själv styra bildbyten.
- **Hjälper:** Förstagångsbesökare som snabbt vill förstå innehållet och återkommande besökare som vill vidare till nya bilder.
- **Prioritet / insats / effekt:** Hög / medel / hög.
- **Mät:** Klick till galleri, senaste bilder och köpinformation samt användartest av om sidans syfte uppfattas direkt.
- **Nulägesstöd:** [introduktion och slumpgalleri](../ArvidsonFoto/Views/Home/Index.cshtml#L31), [fyra utforskningskort](../ArvidsonFoto/Views/Home/Index.cshtml#L63).

### 10. Modernisera kategorinavigeringen utan att tappa orienteringen

- **Problem:** Huvudmenyn visar en flernivåstruktur av kategorier. En rik taxonomi är användbar för den som vet vad den söker, men kan bli lång och svår att överblicka, särskilt på mobil.
- **Förslag:** Minska antalet primära val till tydliga ingångar och låt besökaren fördjupa sig stegvis i kategorier. Gör sök och ”Senaste” lätta att hitta på alla skärmstorlekar och använd brödsmulor samt en tydlig sidrubrik för att visa var i kategoriträdet besökaren befinner sig. Behåll befintliga kategorilänkar och mät användbarheten innan navigeringen ändras brett.
- **Hjälper:** Mobilbesökare och personer som inte känner till kategorinamn; vana besökare behåller direktvägar till kategorierna.
- **Prioritet / insats / effekt:** Hög / medel / hög.
- **Mät:** Tid att hitta en bildkategori i användartest, användning av sök och andel återvändanden till föregående sida.
- **Nulägesstöd:** [hierarkisk huvudmeny](../ArvidsonFoto/Views/Shared/_NavBar.cshtml#L23), [brödsmulor och kategoriingångar](../ArvidsonFoto/Views/Bilder/Index.cshtml#L82).

### 11. Ge galleriet en mer genomarbetad bildrytm

- **Problem:** Bildrutnätet behöver visa många fotografier och namn, men små skärmar kan ge trånga rutor och långa kategorinamn kapas. För fotografier varierar dessutom bildformat, så enhetlig beskärning riskerar att dölja motiv.
- **Förslag:** Finjustera responsiv kolumnbredd, luft och bildtext så att galleriet känns lugnare utan att minska mängden bilder för mycket. Behåll hela motivet i miniatyrerna och gör bildtexter läsbara även när de är långa; erbjud vid behov en valbar kompakt/luftig vy. Se till att större bild, fotografiuppgifter, bildköp och stäng/nästa i ljusboxen är konsekventa och enkla att använda med tangentbord och beröring.
- **Hjälper:** Besökare som bläddrar länge eller använder mobil; fotografen får en mer enhetlig presentation av bilder i olika format.
- **Prioritet / insats / effekt:** Medel / medel / hög.
- **Mät:** Mobil- och tangentbordstest av galleri/ljusbox samt jämförelse av bildöppningar och vidareklick till köp.
- **Nulägesstöd:** [galleriets responsiva kolumner och bildtexter](../ArvidsonFoto/Views/Bilder/_Gallery.cshtml#L21), [miniatyrregler för små skärmar](../ArvidsonFoto/wwwroot/css/site.scss#L606), [bildinformation och köpväg i ljusboxen](../ArvidsonFoto/Views/Bilder/_Gallery.cshtml#L123).

### 12. Förbättra typografi och läsflöde på innehållssidorna

- **Problem:** Information, kontakt, köp och presentation av fotografen är separata sidor. Om rubriker, textbredd och länkar inte följer samma visuella mönster får besökaren mindre hjälp att skumma och hitta nästa steg.
- **Förslag:** Använd en lättläst typsnittsskala med tydliga rubriknivåer, begränsad textbredd och konsekvent avstånd mellan avsnitt. Presentera relaterad kontakt- eller köpinformation i tydliga, återkommande block med en primär handling per sida. Prioritera läsbarhet och stöd för förstoring framför dekorativa effekter.
- **Hjälper:** Besökare som läser på mobil, använder skärmförstoring eller vill hitta kontakt- och köpinformation snabbt.
- **Prioritet / insats / effekt:** Medel / låg till medel / medel.
- **Mät:** Tillgänglighetsgranskning av rubrikstruktur, kontrast och zoom samt användartest för att hitta kontakt- och köpinformation.
- **Nulägesstöd:** [informationssidornas navigationsingångar](../ArvidsonFoto/Views/Info/Index.cshtml#L32), [globala text- och temastilar](../ArvidsonFoto/wwwroot/css/site.scss#L121).

## Föreslagen ordning

1. Börja med tydligare statistikdefinitioner och sök/filter i bildbiblioteket; båda har avgränsad omfattning och ger snabb nytta.
2. Skissa en fotocentrerad startsida och gemensamma designtokens; testa navigering och galleri på mobil innan större omarbetning.
3. Lägg till kvalitetskö i adminstartsidan och förbättra loggsökning när första iterationen har utvärderats.
4. Bestäm filpubliceringsflöde innan batchregistrering eller riktig bilduppladdning byggs.
5. Förnya adminlayout och testa kuraterade besökarsektioner efter att användarbehov och mätetal har bekräftats.

För varje steg bör åtkomstskydd, tillgänglighet, svensk/engelsk text och integritet kontrolleras. Förslag som innebär ny insamling av besöksdata ska bedömas separat innan de genomförs.
