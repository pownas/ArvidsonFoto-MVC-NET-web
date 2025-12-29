# Migration Guide: English Language Support

## Overview

This migration adds English language support to the ArvidsonFoto database by introducing new columns for English translations of existing Swedish text fields.

## What Was Added

### Database Changes

Six new nullable columns were added across three tables:

| Table | New Columns | Purpose |
|-------|------------|---------|
| `tbl_gb` | `GB_name_EN`, `GB_text_EN` | English versions of guestbook entry name and text |
| `tbl_menu` | `menu_text_EN`, `menu_URLtext_EN` | English menu display names and URL segments |
| `tbl_images` | `image_description_EN` | English image descriptions |

All columns are **nullable** to maintain backward compatibility.

## Migration Options

You have two options to apply these database changes:

### Option 1: Using Entity Framework Core Migration (Recommended)

This is the recommended approach as it integrates with the application's migration history.

```bash
# Navigate to the solution root
cd /path/to/ArvidsonFoto-MVC-NET-web

# Apply the migration
dotnet ef database update --project ArvidsonFoto --context ArvidsonFotoCoreDbContext
```

**Migration Details:**
- Name: `20251229090805_AddEnglishColumns`
- Files:
  - `ArvidsonFoto/Core/Data/Migrations/20251229090805_AddEnglishColumns.cs`
  - `ArvidsonFoto/Core/Data/Migrations/20251229090805_AddEnglishColumns.Designer.cs`

### Option 2: Using SQL Script Directly

If you prefer to apply changes directly to the database using SQL, use the provided script.

```bash
# Using SQL Server Management Studio (SSMS)
# Open and execute: docs/AddEnglishColumns.sql

# Or via command line (sqlcmd)
sqlcmd -S your_server_name -d ArvidsonFoto -i docs/AddEnglishColumns.sql
```

**Script Features:**
- ✅ Idempotent (safe to run multiple times)
- ✅ Includes verification queries
- ✅ Includes optional data initialization
- ✅ Includes rollback script (commented out)

## Code Changes

### Models

All model classes now have English properties:

**TblGb.cs**
```csharp
public string? GbNameEn { get; set; }    // English name
public string? GbTextEn { get; set; }    // English text
```

**TblMenu.cs**
```csharp
public string? MenuDisplayNameEn { get; set; }  // English display name
public string? MenuUrlSegmentEn { get; set; }   // English URL segment
```

**TblImage.cs**
```csharp
public string? ImageDescriptionEn { get; set; }  // English description
```

### Database Context

The `ArvidsonFotoCoreDbContext` has been updated to configure the new columns with proper:
- Column names (using `_EN` suffix)
- Max lengths (matching Swedish counterparts)
- Nullable constraints

## Usage in Application

### Fallback Logic Pattern

When displaying content, use this pattern to show English when available, otherwise fallback to Swedish:

```csharp
// Example for menu display name
var displayName = !string.IsNullOrWhiteSpace(menu.MenuDisplayNameEn) 
    ? menu.MenuDisplayNameEn 
    : menu.MenuDisplayName;
```

### Language Selection

You can implement language selection in your application:

```csharp
// Get current language from user preferences/cookies/URL
var currentLanguage = GetCurrentLanguage(); // "sv" or "en"

// Select appropriate field
var menuName = currentLanguage == "en" && !string.IsNullOrWhiteSpace(menu.MenuDisplayNameEn)
    ? menu.MenuDisplayNameEn
    : menu.MenuDisplayName;
```

## Data Population

### Option A: Leave English Fields NULL

English fields can remain NULL. The application should implement fallback logic to show Swedish text when English is not available.

### Option B: Copy Swedish to English (Starting Point)

If you want to populate English fields with Swedish text as a starting point for translation:

```sql
-- This is included in the SQL script (commented out)
UPDATE [dbo].[tbl_gb]
SET 
    [GB_name_EN] = [GB_name],
    [GB_text_EN] = [GB_text]
WHERE [GB_name_EN] IS NULL;

UPDATE [dbo].[tbl_menu]
SET 
    [menu_text_EN] = [menu_text],
    [menu_URLtext_EN] = [menu_URLtext]
WHERE [menu_text_EN] IS NULL;

UPDATE [dbo].[tbl_images]
SET 
    [image_description_EN] = [image_description]
WHERE [image_description_EN] IS NULL;
```

### Option C: Translate Content

Populate fields with actual English translations through:
1. Manual translation via database updates
2. Admin UI for content management
3. Translation services/APIs

## Verification

### Check Migration Applied

```bash
# List all migrations
dotnet ef migrations list --project ArvidsonFoto --context ArvidsonFotoCoreDbContext

# Should include:
# 20251229090805_AddEnglishColumns
```

### Verify Database Schema

```sql
-- Check tbl_gb columns
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'tbl_gb' AND COLUMN_NAME LIKE '%_EN';

-- Check tbl_menu columns
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'tbl_menu' AND COLUMN_NAME LIKE '%_EN';

-- Check tbl_images columns
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'tbl_images' AND COLUMN_NAME LIKE '%_EN';
```

Expected results:
- All `*_EN` columns should exist
- All should be NULLABLE (IS_NULLABLE = 'YES')
- Character lengths should match their Swedish counterparts

## Rollback

### Using EF Core

```bash
# Rollback to previous migration
dotnet ef database update 20250714143852_UpdateDatabaseSchema --project ArvidsonFoto --context ArvidsonFotoCoreDbContext
```

### Using SQL Script

Uncomment and run the rollback section in `docs/AddEnglishColumns.sql`.

## Testing Recommendations

1. **Build Test**: Verify project builds without errors
   ```bash
   dotnet build ArvidsonFoto/ArvidsonFoto.csproj
   ```

2. **Migration Test**: Apply migration to development/staging database
   ```bash
   dotnet ef database update --project ArvidsonFoto --context ArvidsonFotoCoreDbContext
   ```

3. **Data Test**: 
   - Insert test data with both Swedish and English values
   - Verify fallback logic works when English is NULL
   - Test language switching in UI

4. **Existing Functionality**: 
   - Verify all existing features work unchanged
   - Confirm NULL English fields don't break anything

## Next Steps

After applying this migration:

1. **Update Views/Controllers**: Add language selection logic
2. **Update DTOs**: Include English fields in API responses
3. **Update Admin UI**: Allow editing of English fields
4. **Translate Content**: Begin populating English fields
5. **Update Documentation**: Document language switching for users

## Support

For questions or issues:
- See `docs/english-columns-inventory.md` for detailed field inventory
- Review migration files in `ArvidsonFoto/Core/Data/Migrations/`
- Check the SQL script: `docs/AddEnglishColumns.sql`

## Summary

✅ **Migration Created**: 20251229090805_AddEnglishColumns  
✅ **SQL Script Available**: docs/AddEnglishColumns.sql  
✅ **Models Updated**: TblGb, TblMenu, TblImage  
✅ **Backward Compatible**: All new columns nullable  
✅ **Build Status**: Success (0 errors)  
✅ **Ready to Deploy**: Yes (after testing in dev/staging)
