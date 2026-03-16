# ArvidsonFoto API Endpoints

## Image Category API Endpoints

The API now supports the same hierarchical category structure as the web interface, with up to 4 levels of subcategories.

### Get Images by Category Levels

#### Single Level
```
GET /api/bilder/{subLevel1}?page=1&pageSize=48
```
Example: `GET /api/bilder/Faglar?page=1&pageSize=48`

#### Two Levels
```
GET /api/bilder/{subLevel1}/{subLevel2}?page=1&pageSize=48
```
Example: `GET /api/bilder/Faglar/Duvor?page=1&pageSize=48`

#### Three Levels
```
GET /api/bilder/{subLevel1}/{subLevel2}/{subLevel3}?page=1&pageSize=48
```
Example: `GET /api/bilder/Faglar/Duvor/Skogsduva?page=1&pageSize=48`

#### Four Levels
```
GET /api/bilder/{subLevel1}/{subLevel2}/{subLevel3}/{subLevel4}?page=1&pageSize=48
```
Example: `GET /api/bilder/Faglar/Duvor/Skogsduva/Vuxen?page=1&pageSize=48`

### Response Format

```json
{
  "category": {
    "id": 123,
    "name": "Category Name",
    "urlName": "category-url-name"
  },
  "images": [
    {
      "id": 456,
      "url": "image-filename.jpg",
      "description": "Image description",
      "date": "2023-01-01T00:00:00",
      "imageUrl": "/api/image/image-filename.jpg",
      "thumbnailUrl": "/api/image/image-filename.jpg"
    }
  ],
  "pagination": {
    "currentPage": 1,
    "totalPages": 5,
    "totalImages": 240,
    "pageSize": 48,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "url": "/api/bilder/Faglar/Duvor"
}
```

### Additional Endpoints

#### Get All Categories
```
GET /api/categories
```

Returns all available categories with their hierarchical information.

#### Search Categories
```
GET /api/search?s={searchTerm}
```

Search for categories by name, similar to the web interface search functionality.

#### Get All Images (Root Level)
```
GET /api/images
```

Returns all image filenames from the root images directory.

#### Get Specific Image
```
GET /api/image/{filename}
```

Returns the actual image file with proper content type.

### Features

- **Pagination**: All category endpoints support `page` and `pageSize` parameters
- **String Replacement**: Handles ÅÄÖ character replacement using `SharedStaticFunctions.ReplaceAAO()`
- **Error Handling**: Proper HTTP status codes and error messages
- **Consistent Sorting**: Images sorted by ImageId and ImageDate in descending order
- **Category Validation**: Returns 404 if category is not found

### Usage Examples

```bash
# Get first page of birds category
curl "http://localhost:5000/api/bilder/Faglar"

# Get second page with custom page size
curl "http://localhost:5000/api/bilder/Faglar/Duvor?page=2&pageSize=24"

# Search for categories containing "duva"
curl "http://localhost:5000/api/search?s=duva"

# Get all categories
curl "http://localhost:5000/api/categories"

# Get specific image
curl "http://localhost:5000/api/image/skogsduva_001.jpg"
```