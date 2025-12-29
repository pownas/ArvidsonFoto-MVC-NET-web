# Inventering av engelska kolumner för språkstöd

## Sammanfattning
Detta dokument listar alla textfält i databasen som får engelska motsvarigheter för flerspråksstöd.

## Identifierade tabeller och fält

### 1. TblGb (Gästbok)
**Tabell:** `tbl_gb`

| C# Property | Databas Kolumn | Ny C# Property | Ny Databas Kolumn | Datatyp | Nullable | Beskrivning |
|------------|----------------|----------------|-------------------|---------|----------|-------------|
| GbName | GB_name | GbNameEn | GB_name_EN | nvarchar(100) | YES | Namn på person som skrev inlägget (engelska) |
| GbText | GB_text | GbTextEn | GB_text_EN | nvarchar(max) | YES | Innehåll i gästboksinlägget (engelska) |

**Motivering:** Gästboksinlägg kan vara på olika språk. Engelska versioner möjliggör översättning eller flerspråkiga inlägg.

---

### 2. TblMenu (Menykategorier)
**Tabell:** `tbl_menu`

| C# Property | Databas Kolumn | Ny C# Property | Ny Databas Kolumn | Datatyp | Nullable | Beskrivning |
|------------|----------------|----------------|-------------------|---------|----------|-------------|
| MenuDisplayName | menu_text | MenuDisplayNameEn | menu_text_EN | nvarchar(50) | YES | Menyns visningsnamn (engelska) |
| MenuUrlSegment | menu_URLtext | MenuUrlSegmentEn | menu_URLtext_EN | nvarchar(50) | YES | URL-segment för menyn (engelska) |

**Motivering:** Menynamn och URL-segment behöver engelska versioner för internationell navigering.

**Exempel:**
- Svenska: "Fåglar" → Engelska: "Birds"
- Svenska URL: "Faglar" → Engelsk URL: "Birds"

---

### 3. TblImage (Bilder)
**Tabell:** `tbl_images`

| C# Property | Databas Kolumn | Ny C# Property | Ny Databas Kolumn | Datatyp | Nullable | Beskrivning |
|------------|----------------|----------------|-------------------|---------|----------|-------------|
| ImageDescription | image_description | ImageDescriptionEn | image_description_EN | nvarchar(150) | YES | Bildbeskrivning (engelska) |

**Motivering:** Bildbeskrivningar visas för användare och behöver engelska versioner för internationella besökare.

---

## Tabeller som INTE behöver engelska kolumner

### TblKontakt (Kontaktformulär)
**Motivering:** Innehåller användarinmatad data (namn, e-post, meddelanden) som inte översätts. ErrorMessage är systemintern.

### TblPageCounter (Sidräknare)
**Motivering:** Innehåller endast statistikdata och interna sidnamn.

### FeatureFlag
**Motivering:** Innehåller endast feature flag-konfiguration utan användarsynlig text.

---

## Implementation

### Totalt antal nya kolumner: 6

1. **TblGb:** 2 kolumner (GbNameEn, GbTextEn)
2. **TblMenu:** 2 kolumner (MenuDisplayNameEn, MenuUrlSegmentEn)
3. **TblImage:** 1 kolumn (ImageDescriptionEn)

### Migrationsstrategi

1. Alla nya kolumner är NULLABLE för att inte bryta befintlig funktionalitet
2. Initialt värde: NULL (kan uppdateras senare med kopior av svenska texter eller lämnas tomma)
3. Fallback-logik: Om engelsk kolumn är NULL eller tom, visa svenska versionen

### Nästa steg

- [x] Inventering klar
- [ ] Uppdatera C# modeller
- [ ] Uppdatera DbContext konfiguration
- [ ] Skapa EF Core migration
- [ ] Skapa SQL ALTER TABLE-script
- [ ] Uppdatera seed-data
- [ ] Testa migration

---

**Datum:** 2025-12-29  
**Version:** 1.0  
**Status:** Komplett inventering
