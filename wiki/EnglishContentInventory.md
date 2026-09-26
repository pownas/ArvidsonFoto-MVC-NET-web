# Inventering av språkberoende databasfält

Engelska kolumner är nullable. Tomma/NULL-värden visas med svensk fallback; befintlig data kopieras inte automatiskt. Nya översättningar kan fyllas i efter migrering.

| Tabell | Svenskt fält | Engelskt fält | SQL-typ | Initialt värde |
| --- | --- | --- | --- | --- |
| `tbl_gb` | `GB_name` (`GbName`) | `GB_name_en` (`GbNameEn`) | `nvarchar(100) NULL` | NULL, utom översatta testposter |
| `tbl_gb` | `GB_text` (`GbText`) | `GB_text_en` (`GbTextEn`) | `nvarchar(max) NULL` | NULL, utom översatta testposter |
| `tbl_menu` | `menu_text` (`MenuDisplayName`) | `menu_text_en` (`MenuDisplayNameEn`) | `nvarchar(50) NULL` | NULL, utom exempel i dev-seed |
| `tbl_menu` | `menu_URLtext` (`MenuUrlSegment`) | `menu_URLtext_en` (`MenuUrlSegmentEn`) | `nvarchar(50) NULL` | NULL, utom exempel i dev-seed |
| `tbl_images` | `image_description` (`ImageDescription`) | `image_description_en` (`ImageDescriptionEn`) | `nvarchar(150) NULL` | NULL |

Övriga `Tbl*`-fält granskades: `TblKontakt.Name/Subject/Message` är besökares egna inskickade uppgifter, inte redaktionella översättningar; `SourcePage`, `ErrorMessage` och `TblPageCounter.PageName/MonthViewed` är system-/statistikmetadata. `TblImage.ImageUrlName` är ett filnamn; kontaktuppgifter, ID, datum och flaggor är inte översättningsbara. Dessa får inga nya kolumner.

Välj `English` eller `Svenska` i sidhuvudet (alternativt `?culture=en-US` / `?culture=sv-SE`). Valet sparas i en språkkaka; utan val används svenska. Kategori- och bild-DTO:er exponerar även de engelska fälten i API-svar. Engelska URL-segment används där de är ifyllda, annars behålls svenska länkar.

Ta backup före produktionsändring. Kör `dotnet ef database update --project ArvidsonFoto --context ArvidsonFotoCoreDbContext` mot staging först och verifiera kolumner, svenska fallback och engelska värden. Rulla tillbaka genom `dotnet ef database update 20250714143852_UpdateDatabaseSchema --project ArvidsonFoto --context ArvidsonFotoCoreDbContext` (engelska kolumner och deras data tas då bort) eller återställ backup. Migrationen är begränsad till fem `AddColumn`/`DropColumn`; den ändrar inte befintliga fält.
