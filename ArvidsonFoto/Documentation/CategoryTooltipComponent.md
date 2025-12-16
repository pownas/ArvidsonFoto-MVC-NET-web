# Category Tooltip Komponent

En återanvändbar komponent för att visa bild-tooltips när användaren hovrar över kategori-länkar.

## Funktioner
- Visar en popover med den senaste bilden från kategorin
- Cachar bilder för att minimera API-anrop
- Responsiv placering (höger/vänster/under/över)
- Funkar i både light och dark mode

## Användning

### Metod 1: Tag Helper (Enklast)

Lägg till Tag Helper namespace i `_ViewImports.cshtml` (om det inte redan finns):

```cshtml
@addTagHelper *, ArvidsonFoto
```

Använd sedan attributet `category-tooltip` på vilken länk som helst:

```cshtml
<a category-tooltip="1" href="/Bilder/Faglar">Fåglar</a>
<a category-tooltip="25" href="/Bilder/Insekter/Fjarilar">Fjärilar</a>
```

### Metod 2: View Component (Mer kontroll)

```cshtml
@await Component.InvokeAsync("CategoryTooltip", new { 
    categoryId = 1, 
    categoryName = "Fåglar", 
    url = "/Bilder/Faglar",
    cssClass = "btn btn-primary"  // Optional extra CSS classes
})
```

### Metod 3: Manuell HTML (Full kontroll)

```html
<a href="/Bilder/Faglar" 
   class="has-category-tooltip" 
   data-category-id="1">
    Fåglar
</a>
```

## JavaScript

JavaScript initieras automatiskt när sidan laddas genom `categoryTooltip.js`.

### Konfiguration

Om du behöver ändra konfigurationen, editera `wwwroot/js/categoryTooltip.js`:

```javascript
var config = {
    delay: 1000,           // Delay innan tooltip visas (ms)
    imageMaxWidth: 300,   // Max bredd på förhandsvisning
    imageMaxHeight: 200,  // Max höjd på förhandsvisning
    apiEndpoint: '/api/image/GetOneImageFromCategory/'
};
```

## CSS

Styling finns i `wwwroot/css/site.scss` under `.category-tooltip-popover`.

### Anpassa styling:

```scss
.category-tooltip-popover {
    max-width: 350px;  // Ändra max bredd
    z-index: 10000 !important;
    
    .popover-body {
        padding: 0.5rem;
        background-color: #333;  // Ändra bakgrundsfärg
        color: #ddd;
    }
}
```

## API Endpoint

Komponenten förväntar sig att följande API endpoint finns:

```
GET /api/image/GetOneImageFromCategory/{categoryId}
```

Response format:
```json
{
    "urlImage": "/wwwroot/images/bilder/path/to/image",
    "dateImageTaken": "2024-01-15T10:30:00",
    "name": "Image name"
}
```

## Exempel

### Enkel meny med tooltips

```cshtml
<ul class="nav">
    <li class="nav-item">
        <a category-tooltip="1" href="/Bilder/Faglar" class="nav-link">Fåglar</a>
    </li>
    <li class="nav-item">
        <a category-tooltip="2" href="/Bilder/Daggdjur" class="nav-link">Däggdjur</a>
    </li>
    <li class="nav-item">
        <a category-tooltip="3" href="/Bilder/Insekter" class="nav-link">Insekter</a>
    </li>
</ul>
```

### Användning i loop

```cshtml
@foreach (var category in Model.Categories)
{
    <a category-tooltip="@category.Id" href="@category.Url">
        @category.Name
    </a>
}
```

### Med View Component i en card

```cshtml
<div class="card">
    <div class="card-body">
        <h5 class="card-title">Populära kategorier</h5>
        <div class="list-group">
            @foreach (var category in Model.PopularCategories)
            {
                <div class="list-group-item">
                    @await Component.InvokeAsync("CategoryTooltip", new { 
                        categoryId = category.Id, 
                        categoryName = category.Name, 
                        url = category.Url,
                        cssClass = "text-decoration-none"
                    })
                </div>
            }
        </div>
    </div>
</div>
```

## Felsökning

### Tooltip visas inte
1. Kontrollera att `categoryTooltip.js` är inkluderad i sidan
2. Kontrollera att Bootstrap 5.3+ är inladdat
3. Öppna browser console och leta efter JavaScript-fel

### Fel API-svar
1. Kontrollera att API endpoint `/api/image/GetOneImageFromCategory/{categoryId}` fungerar
2. Testa endpoint direkt i browser: `https://yoursite.com/api/image/GetOneImageFromCategory/1`
3. Kontrollera att response format matchar förväntat format

### Tooltip hamnar bakom andra element
Öka z-index i CSS:

```scss
.category-tooltip-popover {
    z-index: 10000 !important;  // Öka detta värde
}
```

## Prestanda

- Bilder cachas efter första hämtningen
- Endast en tooltip visas åt gången
- Lazy loading - bilder hämtas endast när användaren hovrar
- Thumbnail-versioner används för snabbare laddning (`.thumb.jpg`)

## Browser Support

- Chrome/Edge: ✅
- Firefox: ✅
- Safari: ✅
- Mobile browsers: ✅

Kräver Bootstrap 5.3+ för popover-funktionalitet.
