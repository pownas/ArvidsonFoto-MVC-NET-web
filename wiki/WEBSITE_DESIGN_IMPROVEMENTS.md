# Förslag till sub-issues: modernisera webbplatsens design

Det här dokumentet handlar om den publika webbplatsens visuella utformning och besökarens upplevelse av bildinnehållet. Adminflöden är avgränsade till [adminernas UX-förslag](ADMIN_UX_IMPROVEMENTS.md).

Förslagen är kandidater för separata sub-issues under issue #257, inte redan beslutade krav. Varje kandidat beskriver ett avgränsat resultat och acceptanskriterier som kan förfinas när en issue skapas. Prioritet och insats är relativa uppskattningar. Börja gärna med klickbara skisser och användartest innan en bred omarbetning.

## Nuläge

Webbplatsen är en fotografsida med svenskt/engelskt innehåll, kategoribaserad navigering och bildgallerier med större visning i ljusbox. Startsidan kombinerar presentation, slumpmässiga bilder och fyra utforskningskort. Sökresultaten har redan hjälptips, tomt resultat och länkar till kategorier/nya bilder. Köpvägar och bildinformation finns i ljusboxen. Webbplatsen har ljust och mörkt tema samt responsiva regler för miniatyrer.

Designarbetet ska bevara dessa befintliga möjligheter, hålla fotografierna i fokus, fungera på mobil och dator och stödja svenska/engelska. För ändringar i CSS är `site.scss` källan; det genererade `site.css` och dess source map ska uppdateras från SCSS enligt projektets arbetsflöde.

## Prioriterade kandidater

### D1. Inför ett sammanhållet visuellt designsystem

- **Problem:** Grundfärger, länkar, Bootstrap-komponenter och specialkort har delvis egna värden. Resultatet kan sakna sammanhållning mellan header, startsida, galleri och informationssidor.
- **Avgränsning:** Skapa en liten uppsättning namngivna färg-, typografi-, avstånds-, hörnradie- och fokustoken. Uppdatera publika komponenter i avgränsade steg; ändra inte CSS-pipelinen vid körning.
- **Leverans:** Återanvändbara designtoken och korta skisser för sidhuvud, knapp, kort, formulär, sidrubrik och länkar, med varianter för båda teman.
- **Acceptans:** Alla designtoken har dokumenterade värden; publika grundkomponenter använder dem i ljust och mörkt tema; färgkontrast är kontrollerad; inga ändringar görs i admin utan separat omfattning.
- **Hjälper / nytta:** Besökare möter en mer konsekvent identitet och utvecklare kan bygga vidare utan lokala stilundantag.
- **Prioritet / insats:** Hög / medel.
- **Nulägesstöd:** [grundfärger](../ArvidsonFoto/wwwroot/css/site.scss#L26), [mörkt temastöd](../ArvidsonFoto/wwwroot/css/site.scss#L129), [kortens egna färg- och hoverregler](../ArvidsonFoto/wwwroot/css/site.scss#L772).
- **Beroende:** Godkänd visuell riktning, gärna via skiss först.

### D2. Gör startsidan fotocentrerad och tydlig för nya besökare

- **Problem:** Startsidan presenterar fotografen, slumpmässiga bilder och fyra likvärdiga utforskningskort. Den kan tydligare visa vad webbplatsen erbjuder och vilken första handling som är bäst.
- **Avgränsning:** Skapa en hero med en vald bild, kort presentation och tydliga vägar till ”Utforska bilder” och ”Köp bilder”; följ med senaste bilder eller en redaktionell samling. Undvik automatisk karusell och låt fotografen bestämma eventuell utvald bild.
- **Acceptans:** Bildens motiv beskärs inte oavsiktligt; primära länkar och presentation fungerar i sv/en; viktig navigation fungerar även utan hero-bild; layout har kontrollerats på telefon och dator.
- **Hjälper / nytta:** Förstagångsbesökare förstår snabbare sidans syfte och återkommande besökare hittar galleriet direkt.
- **Prioritet / insats:** Hög / medel.
- **Mät:** Användartest av sidans syfte och klick vidare till galleri, senaste bilder och köpinformation.
- **Nulägesstöd:** [introduktion och slumpgalleri](../ArvidsonFoto/Views/Home/Index.cshtml#L31), [fyra utforskningskort](../ArvidsonFoto/Views/Home/Index.cshtml#L63).

### D3. Förenkla publik navigering och behåll tydlig orientering

- **Problem:** Huvudmenyn speglar en flernivå-kategoristruktur, som kan bli lång att överblicka eller svår att använda på mobil. Besökaren behöver även snabbt förstå aktuell kategori och hitta sök/nytt.
- **Avgränsning:** Prova färre primära val, tydligare kategoriingångar samt lättillgängliga sök- och senaste-länkar. Behåll kategoriernas URL:er och brödsmulor; omarbeta inte kategoristrukturen eller datamodellen.
- **Acceptans:** Alla befintliga kategorier går fortsatt att nå; sök och senaste finns på mobil och dator; aktuell plats visas begripligt; meny fungerar med tangentbord, skärmläsare och beröring på sv/en.
- **Hjälper / nytta:** Besökare som inte redan känner till kategorinamn hittar lättare fram utan att vana besökare förlorar direktvägar.
- **Prioritet / insats:** Hög / medel.
- **Mät:** Tid att hitta en kategori i användartest och antal avbrutna/återvända navigeringssteg.
- **Nulägesstöd:** [hierarkisk huvudmeny](../ArvidsonFoto/Views/Shared/_NavBar.cshtml#L23), [brödsmulor och kategorier](../ArvidsonFoto/Views/Bilder/Index.cshtml#L82), [sökväg](../ArvidsonFoto/Views/Shared/_NavBar.cshtml#L111).

### D4. Förfina galleri och bildvisning för olika skärmar

- **Problem:** Galleriet visar många miniatyrer och etiketter; små vyer har särskilda regler och långa etiketter kan kapas. Automatisk beskärning kan skada fotografiska motiv.
- **Avgränsning:** Finjustera kolumnantal, mellanrum, textplacering och miniatyrstorlek i stället för att byta ut galleri eller ljusbox. Behåll bildens hela motiv och köp-/copyrightinformation.
- **Acceptans:** Vanliga mobil-, surfplatta- och desktopbredder kontrolleras; miniatyrer beskär inte motiv; långa bildtexter går att förstå; ljusboxens öppna/stäng/nästa och bildens köpväg fungerar med touch och tangentbord.
- **Hjälper / nytta:** Besökare kan bläddra bekvämt och fotografier presenteras konsekvent.
- **Prioritet / insats:** Hög / medel.
- **Mät:** Mobil- och tangentbordstest, bildöppningar och vidareklick till köp.
- **Nulägesstöd:** [responsiva kolumner och bildtext](../ArvidsonFoto/Views/Bilder/_Gallery.cshtml#L21), [miniatyrregler för mobil](../ArvidsonFoto/wwwroot/css/site.scss#L606), [bildinformation och köpväg](../ArvidsonFoto/Views/Bilder/_Gallery.cshtml#L123).

### D5. Ge information, kontakt och köp ett enhetligt läsflöde

- **Problem:** Informationssidor är olika ingångar till köp, gästbok, kontakt och presentation. En besökare kan behöva skumma text för att hitta nästa användbara handling.
- **Avgränsning:** Enas om textbredd, rubrikstege, avstånd och mönster för kontakt-/köpblock. Använd en tydlig primär handling per sida; innehåll och kontaktuppgifter ändras inte i denna designuppgift.
- **Acceptans:** Sidorna använder konsekventa rubriknivåer och block; text fungerar vid 200 % zoom och smal viewport; länkar har begripliga namn; innehållet är fortsatt korrekt i sv/en.
- **Hjälper / nytta:** Besökare hittar kontakt och köpväg snabbare, även på mobil och med förstoring.
- **Prioritet / insats:** Medel / låg till medel.
- **Mät:** Användartest för att hitta kontakt- och köpinformation samt kontrast-/zoomgranskning.
- **Nulägesstöd:** [informationssidornas länkingångar](../ArvidsonFoto/Views/Info/Index.cshtml#L32), [globala sid- och textstilar](../ArvidsonFoto/wwwroot/css/site.scss#L121).

### D6. Modernisera header och footer för responsiv användning

- **Problem:** Header innehåller en kategoritung meny och footern har separata markup-block för olika skärmstorlekar samt en extern Facebook-iframe. Skillnaden mellan mobil/dator kan försvåra både konsekvent utseende och underhåll.
- **Avgränsning:** Gör headerns primära navigation tydlig och bygg en enda responsiv footerstruktur med kontakt, copyright och social länk. Granska inbäddningens plats och storlek; ta inte bort affärskritisk kontaktinformation utan beslut.
- **Acceptans:** Samma semantiska footerinnehåll används vid alla bredder; innehåll och länkar fungerar i sv/en; footer kan navigeras med tangentbord; externa inbäddningar får tydlig storlek/alternativ om de inte laddas.
- **Hjälper / nytta:** Besökare hittar kontakt, copyright och sociala länkar; mobilvisning blir mer konsekvent.
- **Prioritet / insats:** Medel / låg till medel.
- **Mät:** Skärmbreddsgranskning och kontroll att kontaktvägar hittas vid användartest.
- **Nulägesstöd:** [delad headernavigation](../ArvidsonFoto/Views/Shared/_NavBar.cshtml#L15), [responsivt duplicerade footerblock](../ArvidsonFoto/Views/Shared/_Footer.cshtml#L3).

### D7. Gör visuella interaktioner tillgängliga och rörelsereduserade

- **Problem:** Meny, hoverkort, popovers och ljusbox gör sidan mer levande men designspecifika effekter behöver också fungera med tangentbord, pekskärm och inställningen för reducerad rörelse.
- **Avgränsning:** Granska och justera fokusmarkeringar, kontrast, hover/focus-ekvivalens och övergångar i publika komponenter. Implementera `prefers-reduced-motion` för nya eller befintliga dekorativa rörelser där de förekommer.
- **Acceptans:** Alla primära navigerings- och bildåtgärder är tangentbordsåtkomliga; fokus syns; hoverinnehåll kan öppnas med fokus/beröring; reducerad rörelse respekteras; ljus och mörk palett uppfyller kontrollerad kontrast.
- **Hjälper / nytta:** Besökare med rörelsekänslighet, motoriska eller visuella behov samt mobilanvändare.
- **Prioritet / insats:** Hög / medel.
- **Mät:** Tangentbordsgenomgång samt automatisk/manuell kontrast- och reducerad-rörelsegranskning.
- **Nulägesstöd:** [kortens hoverrörelse](../ArvidsonFoto/wwwroot/css/site.scss#L772), [kategoripopover](../ArvidsonFoto/wwwroot/css/site.scss#L648), [tema och inställningar](../ArvidsonFoto/Views/Shared/_Layout.cshtml#L70).

### D8. Samordna sökresultatets utseende med galleriet

- **Problem:** Sidan för sökning innehåller redan hjälptips, tomt resultat och genvägar, men resultaten behöver kännas som en naturlig del av galleriet snarare än en fristående formulärsida.
- **Avgränsning:** Visuell anpassning av sökfält, resultatrubrik, resultatantal och tomt resultat med samma designtoken, bildkort och typografi som galleriet. Rör inte söklogik eller resultatordning.
- **Acceptans:** Sökväg och frågetext är tydliga; resultatantal och nollresultat har läsbar hierarki; kort och fokustillstånd stämmer med galleriet; layout fungerar vid mobilbredd och för sv/en.
- **Hjälper / nytta:** Besökare får kontinuitet från sökning till bildutforskning och förstår bättre vad de ska göra när en sökning saknar träffar.
- **Prioritet / insats:** Medel / låg.
- **Mät:** Snabbt användartest med träff och utan träff; klick vidare från tomt resultat.
- **Nulägesstöd:** [sökformulär och resultatstatus](../ArvidsonFoto/Views/Bilder/Search.cshtml#L40), [hjälpvägar utan träff](../ArvidsonFoto/Views/Bilder/Search.cshtml#L78).

### D9. Ge kategorisidor en tydligare visuell ingång

- **Problem:** Kategorisidan visar kategori och underkategorier före bildgalleriet, men hierarki och visuella exempel kan behöva göra det enklare att välja rätt gren i en stor taxonomi.
- **Avgränsning:** Testa en diskret kategorisammanfattning med lokaliserad titel, valfri representativ miniatyr och tydliga underkategorier. Använd befintlig kategori- och bilddata; ingen ny redaktionell datamodell eller automatisk popularitetsranking.
- **Acceptans:** Namn och navigering är korrekta för aktuell språkvariant; kategori och underkategorier är synliga även om miniatyr saknas; bildexempel har en begriplig etikett och är inte dekorativt klickbara; vyn fungerar på mobil och med tangentbord.
- **Hjälper / nytta:** Besökare får visuell och hierarkisk vägledning och kan snabbare välja kategori.
- **Prioritet / insats:** Medel / medel.
- **Mät:** Kategorival i användartest och andel sessioner som går vidare från föräldrakategori till relevant underkategori.
- **Nulägesstöd:** [kategorirubrik och underkategorier](../ArvidsonFoto/Views/Bilder/Index.cshtml#L112), [miniatyrbaserad kategoritips](../ArvidsonFoto/Views/Bilder/Index.cshtml#L63).

### D10. Använd statistik som redaktionellt underlag, inte automatisk design

- **Problem:** Admin har redan topplistor för sidvisningar, men de förändrar inte hur besökare upptäcker innehållet.
- **Avgränsning:** Prova en manuellt kuraterad säsongs- eller temasektion på startsidan med befintliga kategorier/bilder. Popularitet kan informera valet men inte styra automatisk placering. Detta förutsätter att innehållet kan underhållas på ett enkelt och godkänt sätt.
- **Acceptans:** Sektionen kan styras/redigeras av ägaren; innehållet har svenska/engelska etiketter; befintliga sök-, kategori- och köpvägar kvarstår; före/efter-test kan mäta vidareklick utan ny persondata.
- **Hjälper / nytta:** Besökare upptäcker fler fotografier och redaktören kan använda befintlig statistik utan att popularitet ersätter redaktionellt omdöme.
- **Prioritet / insats:** Lägre / låg till medel.
- **Mät:** Klick vidare från samlingen och besök i rekommenderade kategorier mot liknande period.
- **Nulägesstöd:** [startsida med galleri och utforskningskort](../ArvidsonFoto/Views/Home/Index.cshtml#L55), [topplistor i statistiken](../ArvidsonFoto/Views/UploadAdmin/_HomepageStatistics.cshtml#L7).
- **Beroende:** Beslut om hur innehållet väljs och lagras.

## Föreslagen uppdelning och ordning

Skapa en separat sub-issue per D-punkt efter att omfattning bekräftats. Börja med D1 som grund för D2, D4, D5 och D8; D3 och D6 kan planeras parallellt efter en navigationsskiss. D7 bör användas som acceptanskrav i alla visuella issues och kan dessutom spåras som en egen tillgänglighetsgranskning. D9 och D10 är valbara upptäckbarhetsförbättringar när grunddesignen är testad.

CSS ska ändras i `site.scss` och genererade `site.css` samt source map ska uppdateras enligt befintlig kompilering. Ändra inte runtime-bundling för CSS. Varje implementation bör kontrolleras i båda teman och språk samt vid mobil-, dator- och tangentbordsanvändning.
