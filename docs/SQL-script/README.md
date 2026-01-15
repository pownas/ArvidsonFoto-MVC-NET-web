# SQL Scripts - Snabbguide för tbl_kontakt

Alla scripts för att skapa och hantera kontaktformulär-tabellen i ArvidsonFoto-databasen.

---

## 📁 Filer i Mappen

| Fil | Syfte | Kör när? |
|-----|-------|----------|
| **01-Create-tbl_kontakt.sql** | Skapar tabell, index, constraints | ⭐ Kör först! |
| **02-Sample-Queries-tbl_kontakt.sql** | Användbara SELECT/Admin queries | Efter create |
| **99-ROLLBACK-Drop-tbl_kontakt.sql** | Tar bort tabellen (med backup) | Bara vid problem |
| **README_tbl_kontakt.md** | Detaljerad dokumentation | Läs för detaljer |
| **INSTALLATION_GUIDE.md** | Steg-för-steg guide | Läs före installation |

---

## ⚡ Snabbstart (30 sekunder)

```sql
-- 1. Öppna SQL Server Management Studio
-- 2. Anslut till ArvidsonFoto-databasen
-- 3. Öppna 01-Create-tbl_kontakt.sql
-- 4. Tryck F5 (Execute)
-- 5. Klart! ✅
```

---

## 📊 Vad Skapas

### Tabell: `tbl_kontakt`

| Kolumn | Typ | Beskrivning |
|--------|-----|-------------|
| ID | int IDENTITY | Primary key |
| SubmitDate | datetime | När formuläret skickades |
| Name | nvarchar(50) | Namn |
| Email | nvarchar(150) | E-post |
| Subject | nvarchar(50) | Rubrik |
| Message | nvarchar(2000) | Meddelande |
| SourcePage | nvarchar(50) | Från vilken sida |
| EmailSent | bit | Om e-post skickades |
| ErrorMessage | nvarchar(500) | Felmeddelande (nullable) |

### Index
- `IX_tbl_kontakt_SubmitDate` - Snabb sortering på datum
- `IX_tbl_kontakt_EmailSent` - Filtrera misslyckade e-post

### Stored Procedure
- `sp_GetContactSubmission` - Hämta specifik submission

---

## 🎯 Vanligaste Användningsfall

### Se senaste submissions
```sql
SELECT TOP 10 * FROM tbl_kontakt 
ORDER BY SubmitDate DESC
```

### Hitta misslyckade e-post
```sql
SELECT * FROM tbl_kontakt 
WHERE EmailSent = 0 
ORDER BY SubmitDate DESC
```

### Räkna submissions per månad
```sql
SELECT 
    FORMAT(SubmitDate, 'yyyy-MM') AS Month,
    COUNT(*) AS Total
FROM tbl_kontakt
GROUP BY FORMAT(SubmitDate, 'yyyy-MM')
ORDER BY Month DESC
```

---

## ✅ Verifiering

Efter att ha kört create-scriptet:

```sql
-- Kontrollera att tabellen finns
SELECT * FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME = 'tbl_kontakt'

-- Räkna kolumner (ska vara 9)
SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'tbl_kontakt'

-- Testa INSERT
INSERT INTO tbl_kontakt (SubmitDate, Name, Email, Subject, Message, SourcePage, EmailSent)
VALUES (GETDATE(), 'Test', 'test@test.com', 'Test', 'Test', 'Kontakta', 1)

-- Testa SELECT
SELECT * FROM tbl_kontakt WHERE Email = 'test@test.com'

-- Cleanup
DELETE FROM tbl_kontakt WHERE Email = 'test@test.com'
```

---

## 🔧 Troubleshooting

### "Table already exists"
✅ OK! Scriptet är idempotent - inget händer om tabellen redan finns.

### "Permission denied"
❌ Du behöver `db_ddladmin` eller `db_owner` permissions.

### "Cannot insert NULL"
❌ Alla kolumner utom `ErrorMessage` är required.

---

## ⏮️ Ångra Installation

```sql
-- Kör 99-ROLLBACK-Drop-tbl_kontakt.sql
-- En backup skapas automatiskt innan drop
```

---

## 📚 Mer Information

- **Detaljerad guide**: Läs `INSTALLATION_GUIDE.md`
- **Queries**: Se `02-Sample-Queries-tbl_kontakt.sql`
- **Dokumentation**: Läs `README_tbl_kontakt.md`

---

## 🎓 Integration med C#

Tabellen används automatiskt av:

```csharp
// ContactService.cs
public void SaveContactSubmission(TblKontakt kontakt)
{
    _context.TblKontakt.Add(kontakt);
    _context.SaveChanges();
}
```

Se:
- `ArvidsonFoto/Core/Models/TblKontakt.cs`
- `ArvidsonFoto/Core/Services/ContactService.cs`
- `ArvidsonFoto/Controllers/InfoController.cs`

---

**Status**: ✅ Production Ready  
**Testad**: SQL Server 2019/2022, Azure SQL Database  
**Skapad**: 2025-01-20
