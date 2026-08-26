# tbl_kontakt - Installation Guide

Komplett guide för att lägga till kontaktformulär-tabellen i ArvidsonFoto-databasen.

---

## 📋 Innehållsförteckning

1. [Översikt](#översikt)
2. [Förutsättningar](#förutsättningar)
3. [Installation](#installation)
4. [Verifiering](#verifiering)
5. [Användning](#användning)
6. [Troubleshooting](#troubleshooting)
7. [Rollback](#rollback)

---

## 📊 Översikt

### Vad är tbl_kontakt?

`tbl_kontakt` är en backup-tabell för kontaktformulär-inlämningar. Den sparar alla formulär-submissions även om e-post misslyckas att skickas.

### Varför behövs den?

- ✅ **Backup**: Säkerhetskopierar alla kontaktformulär
- ✅ **Felhantering**: Loggar varför e-post misslyckades
- ✅ **Statistik**: Möjliggör rapporter om formulär-användning
- ✅ **Uppföljning**: Administratörer kan se misslyckade e-post

### Integration

Tabellen används av:
- `ContactService.cs` - Sparar alla submissions
- `InfoController.cs` - Hanterar formulär-inlämningar
- `ContactFormDto.cs` - Datamodell för formuläret

---

## 🔧 Förutsättningar

### Databas-access

Du behöver:
- ✅ SQL Server Management Studio (SSMS) eller Azure Data Studio
- ✅ Connection till ArvidsonFoto-databasen
- ✅ `db_ddladmin` eller `db_owner` permissions

### Kontrollera access

```sql
-- Kör i SSMS för att verifiera din access
USE ArvidsonFoto
GO

SELECT 
    USER_NAME() AS CurrentUser,
    IS_MEMBER('db_owner') AS IsOwner,
    IS_MEMBER('db_ddladmin') AS CanCreateTables
```

---

## 🚀 Installation

### Steg-för-steg

#### 1. Anslut till databasen

**I SQL Server Management Studio:**
1. Öppna SSMS
2. Anslut till din server
3. Expand "Databases"
4. Högerklicka på "ArvidsonFoto" → New Query

**I Azure Data Studio:**
1. Öppna Azure Data Studio
2. Connect till din server
3. Välj "ArvidsonFoto" database
4. Ctrl+N för ny query

#### 2. Kör Create-scriptet

```sql
-- Öppna filen: 01-Create-tbl_kontakt.sql
-- Kör hela scriptet (F5 eller Execute)
```

**Förväntad output:**
```
Creating table tbl_kontakt...
Table tbl_kontakt created successfully!
Creating index on SubmitDate...
Index IX_tbl_kontakt_SubmitDate created successfully!
Creating index on EmailSent...
Index IX_tbl_kontakt_EmailSent created successfully!
Adding default constraint for SubmitDate...
Default constraint for SubmitDate added successfully!
```

#### 3. Verifiera installation

Kör verification query från scriptet:

```sql
-- Kontrollera kolumner
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'tbl_kontakt'
ORDER BY ORDINAL_POSITION
```

**Förväntat resultat (9 rader):**

| COLUMN_NAME | DATA_TYPE | CHARACTER_MAXIMUM_LENGTH | IS_NULLABLE |
|-------------|-----------|--------------------------|-------------|
| ID | int | NULL | NO |
| SubmitDate | datetime | NULL | NO |
| Name | nvarchar | 50 | NO |
| Email | nvarchar | 150 | NO |
| Subject | nvarchar | 50 | NO |
| Message | nvarchar | 2000 | NO |
| SourcePage | nvarchar | 50 | NO |
| EmailSent | bit | NULL | NO |
| ErrorMessage | nvarchar | 500 | YES |

---

## ✅ Verifiering

### 1. Testa INSERT

```sql
-- Insert test data
INSERT INTO tbl_kontakt 
    (SubmitDate, Name, Email, Subject, Message, SourcePage, EmailSent)
VALUES 
    (GETDATE(), 'Test User', 'test@example.com', 'Test Subject', 'Test message', 'Kontakta', 1)

-- Verify
SELECT * FROM tbl_kontakt WHERE Email = 'test@example.com'

-- Cleanup
DELETE FROM tbl_kontakt WHERE Email = 'test@example.com'
```

### 2. Testa Index

```sql
-- This query should use IX_tbl_kontakt_SubmitDate index
SELECT ID, SubmitDate, Name, Email, Subject
FROM tbl_kontakt
WHERE SubmitDate >= DATEADD(DAY, -7, GETDATE())
ORDER BY SubmitDate DESC

-- Check execution plan (Ctrl+L) to verify index usage
```

### 3. Testa Stored Procedure

```sql
-- Test stored procedure
EXEC sp_GetContactSubmission @ID = 1
```

---

## 📈 Användning

### I C#-applikationen

Tabellen används automatiskt när användare skickar kontaktformulär:

```csharp
// I ContactService.cs
public void SaveContactSubmission(TblKontakt kontakt)
{
    _context.TblKontakt.Add(kontakt);
    _context.SaveChanges();
}

// I InfoController.cs
var kontaktRecord = new TblKontakt
{
    SubmitDate = DateTime.Now,
    Name = contactFormModel.Name,
    Email = contactFormModel.Email,
    Subject = contactFormModel.Subject,
    Message = contactFormModel.Message,
    SourcePage = Page,
    EmailSent = emailSent,
    ErrorMessage = errorMessage
};

_contactService.SaveContactSubmission(kontaktRecord);
```

### Vanliga Admin-queries

#### Se senaste inlämningar
```sql
SELECT TOP 20
    ID, SubmitDate, Name, Email, Subject, SourcePage, EmailSent
FROM tbl_kontakt
ORDER BY SubmitDate DESC
```

#### Hitta misslyckade e-post
```sql
SELECT 
    ID, SubmitDate, Name, Email, Subject, ErrorMessage
FROM tbl_kontakt
WHERE EmailSent = 0
ORDER BY SubmitDate DESC
```

#### Månadsstatistik
```sql
SELECT 
    FORMAT(SubmitDate, 'yyyy-MM') AS Month,
    COUNT(*) AS Total,
    SUM(CASE WHEN EmailSent = 1 THEN 1 ELSE 0 END) AS Successful
FROM tbl_kontakt
GROUP BY FORMAT(SubmitDate, 'yyyy-MM')
ORDER BY Month DESC
```

---

## 🐛 Troubleshooting

### Problem: "Table already exists"

**Symptom:**
```
Msg 2714, Level 16, State 6
There is already an object named 'tbl_kontakt' in the database.
```

**Lösning:**
Scriptet är idempotent och hoppar över om tabellen redan finns. Inget behöver göras!

Om du vill droppa och återskapa:
```sql
-- Kör 99-ROLLBACK-Drop-tbl_kontakt.sql först
-- Sedan kör 01-Create-tbl_kontakt.sql igen
```

### Problem: "Permission denied"

**Symptom:**
```
Msg 262, Level 14, State 1
CREATE TABLE permission denied in database 'ArvidsonFoto'.
```

**Lösning:**
Kontakta din DBA för att få `db_ddladmin` eller `db_owner` permissions.

### Problem: "Cannot insert NULL value"

**Symptom:**
```
Msg 515, Level 16, State 2
Cannot insert the value NULL into column 'Name'
```

**Lösning:**
Alla kolumner utom `ErrorMessage` är required. Se till att fylla i alla fält:
```csharp
var kontakt = new TblKontakt
{
    SubmitDate = DateTime.Now,
    Name = contactFormModel.Name ?? "",  // Required
    Email = contactFormModel.Email ?? "", // Required
    Subject = contactFormModel.Subject ?? "", // Required
    Message = contactFormModel.Message ?? "", // Required
    SourcePage = Page ?? "", // Required
    EmailSent = emailSent,
    ErrorMessage = errorMessage // Nullable - OK
};
```

### Problem: "Index corruption"

**Symptom:**
Slow queries eller error messages om corrupted index.

**Lösning:**
Rebuild index:
```sql
ALTER INDEX IX_tbl_kontakt_SubmitDate ON tbl_kontakt REBUILD
ALTER INDEX IX_tbl_kontakt_EmailSent ON tbl_kontakt REBUILD
```

---

## ⏮️ Rollback

### När behövs rollback?

- ❌ Tabellen skapades med fel struktur
- ❌ Behöver testa create-scriptet igen
- ❌ Migration misslyckades

### Så här gör du rollback

**VARNING:** Detta tar bort all data! En backup skapas automatiskt.

```sql
-- Kör hela 99-ROLLBACK-Drop-tbl_kontakt.sql
-- Vänta 10 sekunder (eller tryck Ctrl+C för att avbryta)
```

### Restore från backup

```sql
-- 1. Lista tillgängliga backups
SELECT name, create_date 
FROM sys.tables
WHERE name LIKE 'tbl_kontakt_backup_%'
ORDER BY create_date DESC

-- 2. Restore från senaste backup
SELECT * INTO tbl_kontakt 
FROM tbl_kontakt_backup_20250120_143022  -- Använd ditt backup-namn

-- 3. Återskapa index och constraints
-- Kör 01-Create-tbl_kontakt.sql
```

---

## 📚 Relaterade Filer

### SQL Scripts
- `01-Create-tbl_kontakt.sql` - Skapar tabellen ⭐
- `02-Sample-Queries-tbl_kontakt.sql` - Användbara queries
- `99-ROLLBACK-Drop-tbl_kontakt.sql` - Tar bort tabellen
- `README_tbl_kontakt.md` - Detaljerad dokumentation

### C# Kod
- `ArvidsonFoto/Core/Models/TblKontakt.cs` - Datamodell
- `ArvidsonFoto/Core/Data/ArvidsonFotoCoreDbContext.cs` - EF Context
- `ArvidsonFoto/Core/Services/ContactService.cs` - Business logic
- `ArvidsonFoto/Core/Interfaces/IContactService.cs` - Interface
- `ArvidsonFoto/Controllers/InfoController.cs` - Controller
- `ArvidsonFoto/Core/DTOs/ContactFormDto.cs` - DTO

---

## 🎯 Checklista

Efter installation, verifiera att:

- [ ] Tabellen `tbl_kontakt` skapades
- [ ] 9 kolumner finns (ID, SubmitDate, Name, Email, Subject, Message, SourcePage, EmailSent, ErrorMessage)
- [ ] Primary key `PK_tbl_kontakt` finns på ID
- [ ] Index `IX_tbl_kontakt_SubmitDate` skapades
- [ ] Index `IX_tbl_kontakt_EmailSent` skapades
- [ ] Default constraint `DF_tbl_kontakt_SubmitDate` finns
- [ ] Stored procedure `sp_GetContactSubmission` skapades
- [ ] Test INSERT/SELECT fungerar
- [ ] C#-applikationen kan spara data till tabellen

---

## 💡 Best Practices

### Backup

Ta backup regelbundet:
```sql
-- Manual backup
SELECT * INTO tbl_kontakt_backup_manual FROM tbl_kontakt

-- Automated backup (add to maintenance plan)
```

### Monitoring

Övervaka misslyckade e-post:
```sql
-- Daily check för failed emails
SELECT COUNT(*) AS FailedToday
FROM tbl_kontakt
WHERE EmailSent = 0 
    AND SubmitDate >= CAST(GETDATE() AS DATE)
```

### Archiving

Arkivera gamla data efter 1-2 år:
```sql
-- Move old successful emails to archive
SELECT * INTO tbl_kontakt_archive_2023 
FROM tbl_kontakt
WHERE EmailSent = 1 
    AND YEAR(SubmitDate) = 2023

-- Delete archived records
DELETE FROM tbl_kontakt
WHERE EmailSent = 1 
    AND YEAR(SubmitDate) = 2023
    AND ID IN (SELECT ID FROM tbl_kontakt_archive_2023)
```

---

## 🆘 Support

### Problem?

1. Kolla [Troubleshooting](#troubleshooting)-sektionen
2. Läs `README_tbl_kontakt.md` för mer detaljer
3. Kolla `02-Sample-Queries-tbl_kontakt.sql` för exempel
4. Kontakta din DBA eller team lead

### Resurser

- [SQL Server Docs](https://learn.microsoft.com/en-us/sql/sql-server/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [ArvidsonFoto GitHub](https://github.com/pownas/ArvidsonFoto-MVC-NET-web)

---

**Skapad**: 2025-01-20  
**Version**: 1.0  
**Status**: ✅ Production Ready  
**Testad**: SQL Server 2019/2022, Azure SQL Database
