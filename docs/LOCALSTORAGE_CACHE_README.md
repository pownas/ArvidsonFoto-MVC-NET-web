# localStorage Caching Implementation

## Översikt

Denna implementation lägger till localStorage-caching för att minska onödiga SQL-frågor mot databasen och förbättra prestanda för besökare. All cache har en standardförfallotid (TTL) på 1 timme.

## Komponenter

### 1. LocalStorageCache (localStorageCache.js)
Centraliserad cache-utility som hanterar all localStorage-funktionalitet med TTL-stöd.

**Funktioner:**
- `set(key, value, ttl)` - Sparar data i cache med TTL (standard: 1 timme)
- `get(key)` - Hämtar data från cache (returnerar null om utgången)
- `remove(key)` - Tar bort specifik cache-post
- `has(key)` - Kontrollerar om cache finns och är giltig
- `clearExpired()` - Rensar alla utgångna cache-poster
- `clearAll()` - Rensar all cache med app-prefix
- `getStats()` - Hämtar statistik om cache
- `getRemainingTTL(key)` - Hämtar återstående tid för en cache-post

**Cache-versionshantering:**
Cache-poster har en version som automatiskt invaliderar gammal cache vid uppdateringar.

### 2. NavigationCache (navigationCache.js)
Cachar navigationsmenyn för att minska databasfrågor vid varje sidladdning.

**Funktioner:**
- Hämtar alla kategorier från `/api/category/All`
- Cachar data i localStorage med 1 timmes TTL
- Preladdar kategorier på sidladdning om inte cachad
- Fallback till API om cache saknas

### 3. CategoryTooltip (categoryTooltip.js) - Uppdaterad
Förbättrad med localStorage-caching för kategori-bildförhandsvisningar.

**Funktioner:**
- Tvåskikts-cache: In-memory (snabbast) + localStorage (persistent)
- Cachar kategori-bilder från `/api/image/GetOneImageFromCategory/{categoryId}`
- Reducerar API-anrop för återkommande hoveringar

### 4. HomepageGalleryCache (homepageGalleryCache.js)
Utility för att cacha slumpmässiga galleribilder på startsidan.

**Funktioner:**
- `setGalleryData(data)` - Sparar galleribilder
- `getGalleryData()` - Hämtar cachade galleribilder
- `clearCache()` - Rensar cache för galleriet
- `isCached()` - Kontrollerar om data är cachad

### 5. StaticContentCache (staticContentCache.js)
Utility för att cacha statiskt sidinnehåll som sällan ändras.

**Funktioner:**
- `setContent(pageId, content, ttl)` - Cachar sidinnehåll
- `getContent(pageId)` - Hämtar cachat innehåll
- `clearContent(pageId)` - Rensar specifik sidcache
- `cacheElementContent(pageId, selector)` - Cachar HTML från element
- `restoreElementContent(pageId, selector)` - Återställer HTML till element

**Lämpliga sidor att cacha:**
- Om mig (`/Info/Om_mig`)
- Copyright (`/Info/Copyright`)
- Kontaktinformation (`/Info/Kontakta`)
- Köp av bilder (`/Info/Köp_av_bilder`)

## Implementationsdetaljer

### TTL (Time To Live)
- **Standard TTL:** 1 timme (3600000 ms)
- Alla cache-poster går automatiskt ut efter TTL
- Utgången cache rensas automatiskt vid sidladdning
- Custom TTL kan anges för specifika användningsfall

### Cache-nyckelprefix
Alla cache-nycklar har prefixet `arvidsonfoto_` för att undvika konflikter med andra applikationer.

### Felhantering
- Kontrollerar localStorage-tillgänglighet innan användning
- Hanterar QuotaExceededError genom att rensa utgången cache
- Fallback till API/server vid cache-miss eller fel

### Browser-kompatibilitet
Fungerar i alla moderna webbläsare som stödjer localStorage (IE8+, alla moderna browsers).

## Användningsexempel

### Använda NavigationCache
```javascript
// Hämta kategorier (från cache eller API)
NavigationCache.getCategories().then(function(categories) {
    console.log('Categories:', categories);
});

// Kontrollera om cachad
if (NavigationCache.isCached()) {
    console.log('Navigation menu is cached');
}

// Rensa cache
NavigationCache.clearCache();
```

### Använda StaticContentCache
```javascript
// Cache sidinnehåll
StaticContentCache.setContent('om_mig', {
    html: '<div>Innehåll...</div>',
    title: 'Om mig'
});

// Hämta cachat innehåll
var content = StaticContentCache.getContent('om_mig');
if (content) {
    console.log('Cached content:', content);
}

// Cache element direkt
StaticContentCache.cacheElementContent('om_mig', '#page-content');

// Återställ från cache
StaticContentCache.restoreElementContent('om_mig', '#page-content');
```

### Använda LocalStorageCache direkt
```javascript
// Spara med custom TTL (30 minuter)
LocalStorageCache.set('my_data', { value: 123 }, 1800000);

// Hämta data
var data = LocalStorageCache.get('my_data');

// Kontrollera återstående tid
var remaining = LocalStorageCache.getRemainingTTL('my_data');
console.log('Expires in:', remaining, 'ms');

// Få statistik
var stats = LocalStorageCache.getStats();
console.log('Cache stats:', stats);
```

## Prestandafördelar

1. **Minskad databas-load:** Färre SQL-frågor för återkommande data
2. **Snabbare laddningstider:** Cachad data hämtas lokalt istället för från servern
3. **Bättre skalbarhet:** Mindre serverbelastning vid många samtidiga användare
4. **Förbättrad användarupplevelse:** Snabbare sidnavigering och interaktioner

## Cache-invalidering

### Automatisk invalidering
- Cache går ut automatiskt efter TTL (1 timme)
- Utgången cache rensas vid varje sidladdning

### Manuell invalidering
```javascript
// Rensa specifik cache
LocalStorageCache.remove('cache_key');

// Rensa all utgången cache
LocalStorageCache.clearExpired();

// Rensa all app-cache
LocalStorageCache.clearAll();

// Uppdatera cache-version (invaliderar all cache)
LocalStorageCache.setVersion('1.1');
```

## Underhåll och övervakning

### Kontrollera cache-status
```javascript
// Få cache-statistik
var stats = LocalStorageCache.getStats();
console.log('Total cache entries:', stats.total);
console.log('Valid entries:', stats.valid);
console.log('Expired entries:', stats.expired);
console.log('Wrong version entries:', stats.wrongVersion);
```

### Rensa gamla poster
```javascript
// Rensa automatiskt vid sidladdning (görs redan)
var cleared = LocalStorageCache.clearExpired();
console.log('Cleared', cleared, 'expired entries');
```

## Säkerhetsöverväganden

1. **Ingen känslig data:** Cache endast publik data som är tillgänglig för alla användare
2. **Ingen authentication data:** Cacha aldrig tokens, lösenord eller användardata
3. **Versionering:** Cache-versioner säkerställer att gammal/ogiltig cache inte används
4. **XSS-skydd:** All HTML-innehåll bör saneras innan caching och rendering

## Framtida förbättringar

1. **API-endpoint för menu-rendering:** Skapa endpoint som returnerar färdig HTML för navigation
2. **Service Worker:** Implementera Service Worker för ännu bättre offline-stöd
3. **Indexerad DB:** För större dataset, överväg IndexedDB istället för localStorage
4. **Cache-varning UI:** Visa användarnotifikation när cache uppdateras
5. **Adaptiv TTL:** Dynamisk TTL baserad på innehållstyp och uppdateringsfrekvens

## Utvecklardokumentation

### Lägga till ny cache-funktion

1. Använd `LocalStorageCache` som bas
2. Skapa specifik cache-manager (se `NavigationCache` som exempel)
3. Definiera lämplig TTL för ditt användningsfall
4. Implementera fallback-logik
5. Lägg till i `_Layout.cshtml` om det är en global funktion

### Debugging

Öppna Browser DevTools Console och kör:
```javascript
// Se all cache
for (var i = 0; i < localStorage.length; i++) {
    var key = localStorage.key(i);
    if (key.startsWith('arvidsonfoto_')) {
        console.log(key, localStorage.getItem(key));
    }
}

// Cache-statistik
console.log(LocalStorageCache.getStats());
```

## Support och frågor

För frågor eller problem relaterade till cache-implementationen, kontakta utvecklingsteamet eller skapa ett issue i GitHub-repositoryt.
