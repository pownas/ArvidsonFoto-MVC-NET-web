# Unit Tests med ArvidsonFotoCoreDbSeeder

## Översikt
Enhetstesterna i projektet använder nu testdata från `ArvidsonFotoCoreDbSeeder.cs` istället för hårdkodad mock-data. Detta gör testerna mer realistiska och närmare produktionsdata.

## Fördelar med ArvidsonFotoCoreDbSeeder

### ✅ Realistisk Testdata
- **543 kategorier** från produktionsdatabasen
- **100 bilder** med verkliga värden
- **Korrekta relationer** mellan kategorier och bilder
- **Svenska tecken** och speciella URL-formatering testad

### ✅ Lättare Underhåll
- En central plats för testdata
- Samma data används i både tester och lokal utveckling
- Enklare att uppdatera när produktionsdata ändras

### ✅ Bättre Testkvalitet
- Tester reflekterar faktiska användningsfall
- Hanterar kantfall från verkliga data
- Upptäcker buggar relaterade till speciella tecken (ÅÄÖ)

## Tillgänglig Testdata

### Kategorier (TblMenu)
```csharp
// Huvudkategorier (MenuParentCategoryId = 0)
MenuCategoryId=1: "Fåglar" (Faglar)
MenuCategoryId=2: "Däggdjur" (Daggdjur)
MenuCategoryId=3: "Kräldjur" (Kraldjur)
MenuCategoryId=5: "Insekter" (Insekter)
MenuCategoryId=6: "Växter" (Vaxter)
MenuCategoryId=7: "Landskap" (Landskap)
MenuCategoryId=8: "Årstider" (Arstider)
MenuCategoryId=9: "Resor" (Resor)

// Exempel underkategorier
MenuCategoryId=23: "Tättingar" (Tattingar) - under Fåglar
MenuCategoryId=208: "Mesar" (Mesar) - under Tättingar
MenuCategoryId=243: "Blåmes" (Blames) - under Mesar
MenuCategoryId=27: "Hackspettar" (Hackspettar) - under Fåglar
MenuCategoryId=49: "Tretåig hackspett" (Tretaig-hackspett) - under Hackspettar
```

### Bilder (TblImage)
```csharp
// Exempel bilder från de första 100
ImageId=1: "AP2D6321" - Tretåig hackspett (ImageCategoryId=49)
ImageId=2: "AP2D6366" - Tretåig hackspett (ImageCategoryId=49)
ImageId=5: "_N0Q8131" - Apollofjäril (ImageCategoryId=50)
ImageId=12: "_N0Q4158" - Aspfjäril (ImageCategoryId=34)
```

## Hur Man Använder i Tester

### 1. Mock Services
Mock-tjänsterna använder automatiskt `ArvidsonFotoCoreDbSeeder`:

```csharp
public class BilderControllerTests
{
    private readonly BilderController _controller;

    public BilderControllerTests()
    {
        // Mock-services använder automatiskt ArvidsonFotoCoreDbSeeder
        var mockImageService = new MockImageService();
        var mockCategoryService = new MockCategoryService();
        var mockPageCounterService = new MockPageCounterService();

        _controller = new BilderController(
            mockImageService,
            mockCategoryService,
            mockPageCounterService);
    }
}
```

### 2. Använda Verkliga Kategorier i Tester

```csharp
[Fact]
public void Index_ReturnsViewResult_WithGalleryViewModel()
{
    // Arrange - Använd riktig kategori från ArvidsonFotoCoreDbSeeder
    string subLevel1 = "Faglar"; // MenuUrlSegment från ArvidsonFotoCoreDbSeeder

    // Act
    var result = _controller.Index(subLevel1, null, null, null, null, 1);

    // Assert
    var viewResult = Assert.IsType<ViewResult>(result);
    var model = Assert.IsType<GalleryViewModel>(viewResult.Model);
    Assert.Equal("/Bilder/Faglar", model.CurrentUrl);
    Assert.NotNull(model.SelectedCategory);
    Assert.Equal("Fåglar", model.SelectedCategory.MenuText);
}
```

### 3. Använda Verkliga Bilder i Tester

```csharp
[Fact]
public void GetById_ValidId_ReturnsImage()
{
    // Arrange - Första bilden från ArvidsonFotoCoreDbSeeder
    int imageId = 1;

    // Act
    var result = _controller.GetById(imageId);

    // Assert
    Assert.IsType<ImageDto>(result);
    Assert.Equal(imageId, result.ImageId);
    Assert.Equal("AP2D6321", result.UrlImage);
}
```

### 4. Testa Underkategorier

```csharp
[Fact]
public void Index_ReturnsViewResult_WithSubCategory()
{
    // Arrange - Använd underkategori från ArvidsonFotoCoreDbSeeder
    // Blåmes (MenuCategoryId=243) -> Mesar (MenuCategoryId=208) -> Tättingar (MenuCategoryId=23) -> Fåglar (MenuCategoryId=1)
    string subLevel1 = "Tattingar";
    string subLevel2 = "Mesar";
    string subLevel3 = "Blames";

    // Act
    var result = _controller.Index(subLevel1, subLevel2, subLevel3, null, null, 1);

    // Assert
    var viewResult = Assert.IsType<ViewResult>(result);
    var model = Assert.IsType<GalleryViewModel>(viewResult.Model);
    Assert.Equal("/Bilder/Tattingar/Mesar/Blames", model.CurrentUrl);
    Assert.NotNull(model.SelectedCategory);
    Assert.Equal("Blåmes", model.SelectedCategory.MenuText);
}
```

## Vanliga Testscenarier

### Scenario 1: Testa Huvudkategori
```csharp
[Fact]
public void TestMainCategory()
{
    // Använd huvudkategori (MenuParentCategoryId = 0)
    int categoryId = 1; // Fåglar
    string urlText = "Faglar";
    
    // Test här...
}
```

### Scenario 2: Testa Kategori med Underkategorier
```csharp
[Fact]
public void GetSubsList_ValidParentId_ReturnsOkWithSubcategories()
{
    // Fåglar (MenuCategoryId=1) har många underkategorier
    int parentId = 1;

    var result = _controller.GetSubsList(parentId);

    var okResult = Assert.IsType<OkObjectResult>(result);
    var subcategories = Assert.IsType<List<CategoryDto>>(okResult.Value);
    
    // Verifiera att underkategorier finns
    Assert.Contains(subcategories, c => c.Name == "Tättingar");
    Assert.Contains(subcategories, c => c.Name == "Dagrovfåglar");
    Assert.Contains(subcategories, c => c.Name == "Hackspettar");
}
```

### Scenario 3: Testa Bilder per Kategori
```csharp
[Fact]
public void GetImagesByCategoryID_ValidCategoryId_ReturnsImages()
{
    // Tretåig hackspett (ImageCategoryId=49) har flera bilder i ArvidsonFotoCoreDbSeeder
    int categoryId = 49;

    var result = _controller.GetImagesByCategoryID(categoryId);

    Assert.IsType<List<ImageDto>>(result);
    var images = result.Where(i => i.CategoryId == categoryId).ToList();
    Assert.True(images.Count > 0);
}
```

### Scenario 4: Testa URL-omvandling (ÅÄÖ)
```csharp
[Fact]
public void TestSwedishCharacters()
{
    // MenuDisplayName: "Fåglar"  -> MenuUrlSegment: "Faglar"
    // MenuDisplayName: "Däggdjur" -> MenuUrlSegment: "Daggdjur"
    // MenuDisplayName: "Blåmes"   -> MenuUrlSegment: "Blames"
    
    string categoryName = "Blames"; // URL-format
    
    var result = _controller.GetByName(categoryName);
    
    Assert.Equal("Blåmes", result.Name); // Display-format
}
```

## Mock Services Implementation

### MockCategoryService
```csharp
public class MockCategoryService : ICategoryService
{
    public int GetLastId() => 
        ArvidsonFotoCoreDbSeeder.MenuCategories.Max(x => x.MenuCategoryId);
    
    public TblMenu GetByName(string categoryName)
    {
        var coreMenu = ArvidsonFotoCoreDbSeeder.MenuCategories
            .FirstOrDefault(c => c.MenuUrlSegment != null && 
                                c.MenuUrlSegment.Equals(categoryName));
        
        if (coreMenu == null) return null;
        
        return new TblMenu
        {
            Id = coreMenu.Id,
            MenuId = coreMenu.MenuCategoryId,
            MenuMainId = coreMenu.MenuParentCategoryId,
            MenuText = coreMenu.MenuDisplayName,
            MenuUrltext = coreMenu.MenuUrlSegment
        };
    }
    
    public List<TblMenu> GetAll() => 
        ArvidsonFotoCoreDbSeeder.MenuCategories
            .Select(c => new TblMenu
            {
                Id = c.Id,
                MenuId = c.MenuCategoryId,
                MenuMainId = c.MenuParentCategoryId,
                MenuText = c.MenuDisplayName,
                MenuUrltext = c.MenuUrlSegment
            }).ToList();
}
```

### MockImageService
```csharp
public class MockImageService : IImageService
{
    public TblImage GetById(int imageId) =>
        ArvidsonFotoCoreDbSeeder.Images
            .Where(i => i.ImageId.Equals(imageId))
            .FirstOrDefault() ?? new TblImage();

    public List<TblImage> GetAllImagesByCategoryID(int categoryID) =>
        ArvidsonFotoCoreDbSeeder.Images
            .Where(i => i.ImageCategoryId == categoryID
                     || i.ImageFamilyId == categoryID
                     || i.ImageMainFamilyId == categoryID)
            .ToList() ?? new List<TblImage>();
}
```

## Best Practices

### ✅ DO:
- Använd verkliga ID:n och namn från ArvidsonFotoCoreDbSeeder
- Kommentera vilket data du använder i tester
- Verifiera att relationer mellan kategorier och bilder är korrekta
- Testa både svenska och URL-formaterade namn

### ❌ DON'T:
- Hårdkoda nya test-ID:n som inte finns i ArvidsonFotoCoreDbSeeder
- Anta att MenuCategoryId=1 alltid är "Blåmes" (det är "Fåglar")
- Glöm att hantera null-värden
- Ignorera svenska tecken (ÅÄÖ) i tester

## Uppdatera ArvidsonFotoCoreDbSeeder

### När ska man uppdatera?
- När nya kategorier läggs till i produktionen
- När bildstrukturen ändras
- När testfall kräver specifik data som saknas

### Hur uppdaterar man?
1. Kör SQL-query i `ArvidsonFotoCoreDbSeeder.cs` kommentarer
2. Kopiera resultatet till respektive lista
3. Kör tester för att verifiera
4. Commit ändringarna

```sql
-- Exempel SQL för att generera ny kategoridata (se ArvidsonFotoCoreDbSeeder.cs för fullständiga queries)
SELECT 
'    new() { ' +
'Id = ' + CAST(ISNULL([Id], 'null') AS NVARCHAR(10)) + ', ' +
'MenuCategoryId = ' + CAST(ISNULL([menu_ID], 'null') AS NVARCHAR(10)) + ', ' +
'MenuParentCategoryId = ' + CASE
    WHEN [menu_mainID] IS NULL OR [menu_mainID] = 0 THEN '0' 
    ELSE CAST([menu_mainID] AS NVARCHAR(10)) 
END + ', ' +
'MenuDisplayName = "' + REPLACE(ISNULL([menu_text], ''), '"', '\"') + '", ' +
'MenuUrlSegment = "' + REPLACE(ISNULL([menu_URLtext], ''), '"', '\"') + '" ' +
' },' AS CSharpCode
FROM [tbl_menu]
ORDER BY [Id];
```

## Köra Tester

```bash
# Kör alla tester
dotnet test

# Kör specifik testklass
dotnet test --filter "FullyQualifiedName~BilderControllerTests"

# Kör specifikt test
dotnet test --filter "FullyQualifiedName~BilderControllerTests.Index_ReturnsViewResult_WithGalleryViewModel"
```

## Felsökning

### Problem: "Category not found"
**Lösning**: Kontrollera att du använder `MenuUrlSegment` (t.ex. "Faglar") och inte `MenuDisplayName` (t.ex. "Fåglar")

### Problem: "Image not found"
**Lösning**: Kontrollera att ImageId finns i de första 100 bilderna från ArvidsonFotoCoreDbSeeder

### Problem: "Null reference exception"
**Lösning**: Lägg till null-checks eller använd verkliga ID:n som garanterat finns

## Migrera från DbSeederExtension

Om du har gamla tester som använder `DbSeederExtension`:

| Gammalt namn | Nytt namn |
|--------------|-----------|
| `DbSeederExtension.DbSeed_Tbl_Menu` | `ArvidsonFotoCoreDbSeeder.MenuCategories` |
| `DbSeederExtension.DbSeed_Tbl_Image` | `ArvidsonFotoCoreDbSeeder.Images` |
| `DbSeederExtension.DbSeed_Tbl_Gb` | `ArvidsonFotoCoreDbSeeder.GuestbookEntries` |
| `DbSeederExtension.DbSeed_Tbl_PageCounter` | `ArvidsonFotoCoreDbSeeder.PageCounters` |
| `MenuId` | `MenuCategoryId` |
| `MenuMainId` | `MenuParentCategoryId` |
| `MenuText` | `MenuDisplayName` |
| `MenuUrltext` | `MenuUrlSegment` |
| `ImageArt` | `ImageCategoryId` |
| `ImageFamilj` | `ImageFamilyId` |
| `ImageHuvudfamilj` | `ImageMainFamilyId` |
| `ImageUrl` | `ImageUrlName` |

## Referenser

- **ArvidsonFotoCoreDbSeeder.cs**: Central testdata-fil i Core-projektet
- **MockServices**: Mock-implementationer som använder ArvidsonFotoCoreDbSeeder
- **Integration Tests**: Använder verklig in-memory databas med samma seed-data

## Exempel Testklasser

Se följande klasser för fullständiga exempel:
- `BilderControllerTests.cs` - Controller-tester med routing
- `CategoryApiControllerTests.cs` - API-tester för kategorier
- `ImageApiControllerTests.cs` - API-tester för bilder
