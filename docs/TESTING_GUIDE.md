# Testing Guide - Database Query Reduction

## Quick Verification Steps

### 1. **Verifiera Build**
```bash
dotnet build
# ✅ Förväntat resultat: Build successful
```

### 2. **Starta Applikationen**
```bash
dotnet run --project ArvidsonFoto
```

### 3. **Öppna Developer Tools (F12)**

#### A. Kontrollera Eager Loading
1. Öppna Console tab
2. Ladda startsidan
3. Leta efter:
   ```
   ✓ Category cache pre-loaded successfully in Xms with 650 categories
   ✓ Category tooltips initialized with lazy loading (650 links)
   📊 Category Cache Stats: {isCached: true, count: 650, ...}
   ```

#### B. Kontrollera localStorage
1. Öppna Application/Storage tab
2. Expandera Local Storage → `http://localhost:XXXX`
3. Verifiera att dessa nycklar finns:
   - `arvidsonfoto_categories` (~300 KB)
   - `arvidsonfoto_categories_version` (värde: "1.0")
   - `arvidsonfoto_categories_timestamp`

#### C. Kontrollera Lazy Loading
1. Öppna Network tab
2. Filtrera på "XHR" eller "Fetch"
3. Hovra över en kategori-länk i menyn
4. Efter ~400ms: Se request till `/api/image/GetOneImageFromCategory/{id}`
5. Hovra över samma länk igen: Ingen ny request (cachat!)

#### D. Verifiera Popover
1. Hovra över en kategori-länk (t.ex. "Blåmes")
2. Efter ~400ms: Se spinner "Laddar..."
3. Efter ~500ms: Se bild + kategorinamn + datum
4. Flytta musen bort: Popover försvinner
5. Hovra över samma länk igen: Instant visning (cachat!)

### 4. **SQL Server Profiler (Om tillgängligt)**

#### A. Starta Profiler
```sql
-- Öppna SQL Server Profiler
-- File → New Trace → Connect to din databas
-- Events Selection → Välj:
--   - SQL:BatchCompleted
--   - RPC:Completed
```

#### B. Räkna Queries
1. **Kör Profiler**
2. **Ladda startsidan** i browser
3. **Stoppa Profiler**
4. **Räkna antal rader** i trace

**Förväntade resultat:**
- **Första gången (cold start):** ~50-100 queries
  - ~10 queries för categories (om cache miss)
  - ~40-90 queries för bilder
- **Andra gången (warm cache):** ~0-10 queries
  - Alla categories från cache
  - Endast nödvändiga image queries

**Jämför med innan:**
- **Innan:** ~9,000 queries per sidladdning
- **Efter:** ~50-100 queries första gången, ~0-10 efteråt
- **Förbättring:** 99%+ reduktion

### 5. **Verifiera Navbar Prestanda**

#### A. Chrome DevTools Performance
1. Öppna DevTools → Performance tab
2. Klicka Record (⚫)
3. Ladda sidan
4. Stoppa recording efter 2-3 sekunder
5. Leta efter "Navbar rendering" eller "Parse HTML"

**Före:** ~800ms för navbar (650 DB queries)  
**Efter:** ~10ms för navbar (0 DB queries)

#### B. Visuell Kontroll
1. Ladda sidan i normal hastighet
2. Navbar ska visas **omedelbart** utan fördröjning
3. Inga "hoppande" eller "laddande" effekter

### 6. **Smart Prefetching Test**

1. Ladda sidan och vänta 2 sekunder
2. Öppna Network tab och rensa (Clear)
3. Hovra över de första 5 kategori-länkarna som är synliga
4. **Förväntat:** Instant visning av popover (redan förladdade)
5. Hovra över en kategori långt ner på sidan
6. **Förväntat:** Kort loading spinner, sedan bild visas

### 7. **Cache Invalidation Test**

#### A. localStorage Version Bump
```javascript
// I Developer Tools Console:
localStorage.setItem('arvidsonfoto_categories_version', '0.9');
location.reload();

// Förväntat:
// ⚠ Cache version mismatch, clearing...
// ✓ Loaded X categories from server and cached in localStorage
```

#### B. Manual Cache Clear
```javascript
// I Developer Tools Console:
CategoryCache.clearCache();
location.reload();

// Förväntat:
// ⌛ Loading categories from server...
// ✓ Loaded X categories from server and cached in localStorage
```

### 8. **Mobile Testing**

1. Öppna DevTools → Toggle device toolbar (Ctrl+Shift+M)
2. Välj "iPhone 12 Pro" eller liknande
3. Ladda sidan
4. Hovra/touch på kategori-länkar
5. **Förväntat:** Popover visas med större offset (40px) för att undvika +/- knappar

---

## Performance Benchmarks

### Mål

| Metrik | Mål |
|--------|-----|
| Första sidladdning | <100 DB queries |
| Efterföljande sidor | <10 DB queries |
| Navbar rendering | <20ms |
| Popover response time | <500ms |
| localStorage size | <500 KB |

### Verktyg

1. **SQL Server Profiler** - Räkna DB queries
2. **Chrome DevTools Network** - HTTP requests
3. **Chrome DevTools Performance** - Rendering time
4. **Browser Console** - Cache stats

---

## Troubleshooting

### Problem: "Category cache pre-loaded" loggen syns inte

**Lösning:**
```csharp
// Kontrollera Program.cs
// Verifiera att eager loading-koden finns efter database seeding
```

### Problem: localStorage är tomt

**Lösning:**
```javascript
// I Console:
await CategoryCache.getCategories();
// Manuellt ladda kategorier
```

### Problem: Popover visar inte bilder

**Lösning:**
1. Kontrollera Network tab - finns request till `/api/image/GetOneImageFromCategory/{id}`?
2. Kontrollera Console - finns JavaScript-fel?
3. Kontrollera att `categoryTooltip.js` är inkluderad i `_Layout.cshtml`

### Problem: Många DB queries fortfarande

**Lösning:**
1. Verifiera att `_NavBar.cshtml` INTE har `@inject IApiImageService`
2. Verifiera att `GetPopoverAttr()` funktionen är borttagen
3. Kontrollera SQL Profiler - vilka queries körs?

---

## Success Criteria ✅

- [ ] Build successful
- [ ] Startsidan laddar på <1 sekund
- [ ] <100 DB queries vid första laddning (SQL Profiler)
- [ ] <10 DB queries vid andra laddning (warm cache)
- [ ] localStorage innehåller `arvidsonfoto_categories`
- [ ] Console visar "Category cache pre-loaded successfully"
- [ ] Popover-bilder laddas lazy (endast vid hover)
- [ ] Smart prefetching fungerar (första 5 länkar instant)
- [ ] Navbar renderar på <20ms (DevTools Performance)

---

**Lycka till med testningen!** 🚀

Om något inte fungerar som förväntat, kontrollera logs och SQL Profiler för att se var queries fortfarande genereras.
