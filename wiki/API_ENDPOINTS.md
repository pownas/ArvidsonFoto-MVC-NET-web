# API-endpoints

API-exemplen nedan motsvarar routning och DTO:er i `ArvidsonFoto/Controllers/ApiControllers`. I Development finns även Scalar på `/scalar/v1` och OpenAPI JSON på `/openapi/v1.json`.

## Bilder per kategorisökväg

```http
GET /api/bilder/{categoryPath}?page=1&limit=48
```

`/api/images/{categoryPath}` är ett alias. Kategorisökvägen kan innehålla flera segment, till exempel:

```text
GET /api/bilder/Faglar/Duvor?page=1&limit=24
```

Parametrar:

- `page`: 1-baserad sida. Utelämnad eller `0` använder det äldre icke-paginerade flödet.
- `limit`: antal poster per sida; standardvärde 48.
- `sortBy`: `uploaded`, `imagetaken` eller `categoryname`.
- `sortOrder`: `asc` eller `desc`.

Svaret är ett `ImageListResponse`, inte ett objekt med nästlade `category`- och `pagination`-objekt. Fält omfattar `categoryId`, `categoryNameSv`, `categoryNameEn`, `urlCategorySv`, `urlCategoryEn`, `imageCategoryTotalCount`, `imageResultCount`, `images`, `querySortBy`, `querySortOrder`, `queryLimit`, `currentPage`, `totalPages` och `pageSize`. Varje bild är en `ImageDto`; fältnamn omfattar bland annat `imageId`, `categoryId`, `categoryNameSv`, `categoryNameEn`, `descriptionSv`, `descriptionEn`, `urlImage`, `dateImageTaken`, `dateUploaded`, `categoryIsGameSelectable` och `isGameSelectable`.

Okänd kategori ger 404. Exempel:

```powershell
curl.exe "http://localhost:5000/api/bilder/Faglar/Duvor?page=1&limit=24"
```

## Kategorier

### Lättviktslista för klientcache

```http
GET /api/category/AllLightweight
```

Alias: `GET /api/category/Lightweight`. Endpointen är anonym och har HTTP-cache på 24 timmar. Returnerade fält är `categoryId`, `nameSv`, `nameEn`, språkspecifika URL-fält samt `parentCategoryId`.

### Spelkategorier

```http
GET /api/game/categories
```

Returnerar kategorier som kan användas av spelet. Kategorier och överordnade kategorier påverkar om bilder är tillgängliga för spel.

## Bilder

### Senast uppladdade

```http
GET /api/image/Latest?count=6
```

`count` måste vara mellan 1 och 100; standardvärdet är 6. Ogiltigt antal ger HTTP 400.

### Spelbilder

```http
GET /api/game/images?count=20&categoryId=13
```

`categoryId` är valfritt. Resultatet innehåller `ImageDto`-fält, inklusive `isGameSelectable`.

### Bild per kategori

```http
GET /api/image/GetOneImageFromCategory/{categoryId}
```

Använd denna route för den representativa bilden i kategoripopupen.