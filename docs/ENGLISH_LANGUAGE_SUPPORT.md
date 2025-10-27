# English Language Support (Engelska språkstöd)

## Overview / Översikt

This document describes the English language support implementation in the ArvidsonFoto database and models.

Detta dokument beskriver implementeringen av engelskt språkstöd i ArvidsonFoto-databasen och modellerna.

## Database Changes / Databasändringar

### Migration: 20251027173126_AddEnglishColumns

The following English columns have been added to support bilingual content:

Följande engelska kolumner har lagts till för att stödja tvåspråkigt innehåll:

### Table: tbl_gb (Guestbook / Gästbok)

| Swedish Column | English Column | Type | Description |
|----------------|----------------|------|-------------|
| `GB_name` | `GB_name_en` | nvarchar(100) | Visitor name / Besökarens namn |
| `GB_text` | `GB_text_en` | nvarchar(max) | Guestbook entry text / Gästboksinläggets text |

### Table: tbl_menu (Menu Categories / Menykategorier)

| Swedish Column | English Column | Type | Description |
|----------------|----------------|------|-------------|
| `menu_text` | `menu_text_en` | nvarchar(50) | Category display name / Kategorinamn |
| `menu_URLtext` | `menu_URLtext_en` | nvarchar(50) | URL-friendly slug / URL-vänlig slug |

### Table: tbl_images (Images / Bilder)

| Swedish Column | English Column | Type | Description |
|----------------|----------------|------|-------------|
| `image_description` | `image_description_en` | nvarchar(150) | Image description / Bildbeskrivning |

### Table: tbl_PageCounter (Page Views / Sidvisningar)

| Swedish Column | English Column | Type | Description |
|----------------|----------------|------|-------------|
| `PageCounter_Name` | `PageCounter_Name_en` | nvarchar(50) | Page name / Sidnamn |

## Model Changes / Modelländringar

All corresponding Entity Framework models have been updated with the new English properties:

Alla motsvarande Entity Framework-modeller har uppdaterats med de nya engelska egenskaperna:

### ArvidsonFoto.Models / ArvidsonFoto.Core.Models

- **TblGb**: Added `GbNameEn`, `GbTextEn`
- **TblMenu**: Added `MenuTextEn`/`MenuDisplayNameEn`, `MenuUrltextEn`/`MenuUrlSegmentEn`
- **TblImage**: Added `ImageDescriptionEn`
- **TblPageCounter**: Added `PageNameEn`

## Seed Data / Seed-data

Seed data files have been updated with English translations:

Seed-datafiler har uppdaterats med engelska översättningar:

### Main Category Translations / Huvudkategoriöversättningar

| Swedish (Svenska) | English (Engelska) |
|-------------------|-------------------|
| Fåglar | Birds |
| Däggdjur | Mammals |
| Kräldjur | Reptiles |
| Insekter | Insects |
| Växter | Plants |
| Landskap | Landscapes |
| Årstider | Seasons |
| Resor | Travel |

### Files Updated / Uppdaterade filer

- `ArvidsonFoto/Data/DbSeederExtension.cs`
- `ArvidsonFoto/Core/Data/ArvidsonFotoCoreDbSeeder.cs`

## Implementation Guidelines / Implementeringsriktlinjer

### Data Population / Datafyllning

All new English columns are **nullable** to support gradual data population:

Alla nya engelska kolumner är **nullable** för att stödja gradvis datafyllning:

- Existing records will have NULL in English columns until populated
- Befintliga poster kommer att ha NULL i engelska kolumner tills de fylls i

### UI Display Logic / UI-visningslogik

Recommended fallback logic for displaying content:

Rekommenderad fallback-logik för att visa innehåll:

```csharp
// Pseudocode example
var displayText = (language == "en" && !string.IsNullOrEmpty(model.FieldEn))
    ? model.FieldEn
    : model.FieldSv;
```

### Adding New Content / Lägga till nytt innehåll

When adding new content, both Swedish and English fields should be populated when possible:

När nytt innehåll läggs till bör både svenska och engelska fält fyllas i när det är möjligt:

```csharp
var newEntry = new TblMenu
{
    MenuDisplayName = "Svenska namnet",
    MenuDisplayNameEn = "English name",
    MenuUrlSegment = "svenska-namnet",
    MenuUrlSegmentEn = "english-name"
};
```

## Migration Instructions / Migrationsinstruktioner

### Development / Utveckling

```bash
# Apply migration
dotnet ef database update --context ArvidsonFotoCoreDbContext
```

### Production / Produktion

1. **Backup the database / Säkerhetskopiera databasen**
2. **Test in staging / Testa i staging**
3. **Apply migration during maintenance window / Applicera migrationen under planerat fönster**

```sql
-- Backup command example
BACKUP DATABASE [ArvidsonFoto] TO DISK = 'C:\Backups\ArvidsonFoto_PreEnglishMigration.bak'
```

### Rollback / Återställning

If needed, the migration can be rolled back:

Om nödvändigt kan migrationen återställas:

```bash
dotnet ef database update 20250714143852_UpdateDatabaseSchema --context ArvidsonFotoCoreDbContext
```

Or restore from backup / Eller återställ från backup.

## Future Enhancements / Framtida förbättringar

- [ ] Add language selector UI component / Lägg till språkväljare i UI
- [ ] Create admin interface for managing translations / Skapa admingränssnitt för att hantera översättningar
- [ ] Implement automatic translation API integration / Implementera automatisk översättnings-API-integration
- [ ] Add language-specific URL routing / Lägg till språkspecifik URL-routing
- [ ] Create translation management system / Skapa översättningshanteringssystem

## Related Files / Relaterade filer

### Models / Modeller
- `/ArvidsonFoto/Models/TblGb.cs`
- `/ArvidsonFoto/Models/TblMenu.cs`
- `/ArvidsonFoto/Models/TblImage.cs`
- `/ArvidsonFoto/Models/TblPageCounter.cs`
- `/ArvidsonFoto/Core/Models/` (corresponding Core models)

### Database Contexts / Databaskontexter
- `/ArvidsonFoto/Data/ArvidsonFotoDbContext.cs`
- `/ArvidsonFoto/Core/Data/ArvidsonFotoCoreDbContext.cs`

### Seed Data / Seed-data
- `/ArvidsonFoto/Data/DbSeederExtension.cs`
- `/ArvidsonFoto/Core/Data/ArvidsonFotoCoreDbSeeder.cs`

### Migrations / Migreringar
- `/ArvidsonFoto/Core/Data/Migrations/20251027173126_AddEnglishColumns.cs`

## Contact / Kontakt

For questions about the English language implementation, please contact the development team.

För frågor om implementeringen av engelskt språkstöd, vänligen kontakta utvecklingsteamet.

---

**Last Updated / Senast uppdaterad**: 2025-10-27  
**Migration Version / Migreringsversion**: 20251027173126_AddEnglishColumns
