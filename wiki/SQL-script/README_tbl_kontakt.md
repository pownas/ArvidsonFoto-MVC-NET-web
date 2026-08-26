# SQL Scripts för tbl_kontakt

Detta dokumentet beskriver SQL-scripten för att skapa och hantera kontaktformulär-tabellen i ArvidsonFoto-databasen.

---

## 📁 Filer

### 1. `01-Create-tbl_kontakt.sql`
**Syfte**: Skapar tabellen `tbl_kontakt` i databasen

**Vad scriptet gör**:
- ✅ Skapar tabell med korrekt schema
- ✅ Skapar primary key (IDENTITY kolumn)
- ✅ Skapar index för bättre prestanda
- ✅ Lägger till default constraints
- ✅ Verifierar att allt är korrekt skapat
- ✅ Idempotent - kan köras flera gånger utan problem

**Kör detta script först!**

### 2. `02-Sample-Queries-tbl_kontakt.sql`
**Syfte**: Innehåller användbara queries för att hantera kontaktformulär-data

**Innehåll**:
- 📊 SELECT queries för rapporter
- 🔍 Admin queries för uppföljning
- 🛠️ Maintenance queries
- 🧪 Testing queries
- 📦 Stored procedures

---

## 🚀 Snabbstart

### Steg 1: Skapa tabellen

```sql
-- Kör i SQL Server Management Studio eller Azure Data Studio
-- Anslut till din ArvidsonFoto-databas först

-- Kör hela scriptet:
USE ArvidsonFoto
GO

-- Kör sedan 01-Create-tbl_kontakt.sql
```

### Steg 2: Verifiera installation

```sql
-- Kontrollera att tabellen skapades
SELECT * FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME = 'tbl_kontakt'

-- Kontrollera kolumner
SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'tbl_kontakt'
ORDER BY ORDINAL_POSITION

-- Kontrollera index
EXEC sp_helpindex 'tbl_kontakt'
```

---

## 📊 Tabellstruktur

### Kolumner

| Kolumn | Typ | Längd | Nullable | Beskrivning |
|--------|-----|-------|----------|-------------|
| **ID** | int | - | NOT NULL | Primary key (IDENTITY) |
| **SubmitDate** | datetime | - | NOT NULL | När formuläret skickades |
| **Name** | nvarchar | 50 | NOT NULL | Avsändarens namn |
| **Email** | nvarchar | 150 | NOT NULL | Avsändarens e-post |
| **Subject** | nvarchar | 50 | NOT NULL | Ärendetext |
| **Message** | nvarchar | 2000 | NOT NULL | Meddelande |
| **SourcePage** | nvarchar | 50 | NOT NULL | Från vilken sida (Kontakta/Kop_av_bilder) |
| **EmailSent** | bit | - | NOT NULL | Om e-post skickades OK |
| **ErrorMessage** | nvarchar | 500 | NULL | Felmeddelande om email misslyckades |

### Index

1. **PK_tbl_kontakt** (Primary Key)
   - Kolumn: ID

2. **IX_tbl_kontakt_SubmitDate** (Non-clustered)
   - Kolumn: SubmitDate DESC
   - Inkluderar: EmailSent, SourcePage
   - Syfte: Snabb hämtning av senaste inlämningar

3. **IX_tbl_kontakt_EmailSent** (Non-clustered)
   - Kolumn: EmailSent
   - Inkluderar: SubmitDate, Name, Email
   - Syfte: Filtrera misslyckade e-post

---

## 📋 Vanliga Queries

### Visa senaste inlämningar

```sql
SELECT TOP 10
    ID,
    SubmitDate,
    Name,
    Email,
    Subject,
    SourcePage,
    EmailSent
FROM tbl_kontakt
ORDER BY SubmitDate DESC
```

### Hitta misslyckade e-post

```sql
SELECT 
    ID,
    SubmitDate,
    Name,
    Email,
    Subject,
    ErrorMessage
FROM tbl_kontakt
WHERE EmailSent = 0
ORDER BY SubmitDate DESC
```

### Statistik per månad

```sql
SELECT 
    FORMAT(SubmitDate, 'yyyy-MM') AS YearMonth,
    COUNT(*) AS TotalSubmissions,
    SUM(CASE WHEN EmailSent = 1 THEN 1 ELSE 0 END) AS Successful,
    SUM(CASE WHEN EmailSent = 0 THEN 1 ELSE 0 END) AS Failed
FROM tbl_kontakt
WHERE SubmitDate >= DATEADD(MONTH, -12, GETDATE())
GROUP BY FORMAT(SubmitDate, 'yyyy-MM')
ORDER BY YearMonth DESC
```

---

## 🔧 Underhåll

### Kontrollera tabellstorlek

```sql
SELECT 
    t.NAME AS TableName,
    p.rows AS RowCounts,
    SUM(a.total_pages) * 8 AS TotalSpaceKB
FROM sys.tables t
INNER JOIN sys.indexes i ON t.OBJECT_ID = i.object_id
INNER JOIN sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id
INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id
WHERE t.NAME = 'tbl_kontakt'
GROUP BY t.Name, p.Rows
```

### Indexunderhåll

```sql
-- Rebuild index om fragmentering > 30%
ALTER INDEX IX_tbl_kontakt_SubmitDate ON tbl_kontakt REBUILD

-- Reorganize index om fragmentering 10-30%
ALTER INDEX IX_tbl_kontakt_SubmitDate ON tbl_kontakt REORGANIZE
```

---

## 🔒 Säkerhet

### Backup

**Rekommendation**: Ta alltid backup innan du kör DDL-scripts (CREATE, ALTER, DROP)

```sql
-- Backup tabellen
SELECT * INTO tbl_kontakt_backup_20250120 FROM tbl_kontakt

-- Verifiera backup
SELECT COUNT(*) FROM tbl_kontakt_backup_20250120
```

### Permissions

Tabellen ärver permissions från databasen, men du kan explicit sätta:

```sql
-- Ge läsrättigheter till en användare
GRANT SELECT ON tbl_kontakt TO [YourUser]

-- Ge skrivrättigheter (för applikationen)
GRANT INSERT, UPDATE ON tbl_kontakt TO [AppUser]
```

---

## 📈 Prestanda

### Optimeringstips

1. **Index används automatiskt** för queries med:
   - `WHERE SubmitDate >= ...`
   - `WHERE EmailSent = 0`
   - `ORDER BY SubmitDate DESC`

2. **Undvik full table scans**:
   - Använd `WHERE`-klausuler när möjligt
   - Filtrera på indexerade kolumner

3. **Arkivering**:
   - Flytta gamla poster till arkiv-tabell efter 1-2 år
   - Behåll misslyckade e-post längre för uppföljning

---

## 🧪 Testing

### Insert test data

```sql
INSERT INTO tbl_kontakt (SubmitDate, Name, Email, Subject, Message, SourcePage, EmailSent)
VALUES 
    (GETDATE(), 'Test Testsson', 'test@example.com', 'Test', 'Detta är ett test', 'Kontakta', 1)
```

### Verify data

```sql
SELECT * FROM tbl_kontakt WHERE Email = 'test@example.com'
```

### Cleanup test data

```sql
DELETE FROM tbl_kontakt WHERE Email = 'test@example.com'
```

---

## ❓ Troubleshooting

### Problem: Tabellen finns inte efter att ha kört scriptet

**Lösning**:
1. Kontrollera att du är ansluten till rätt databas
2. Kolla efter felmeddelanden i Messages-panelen
3. Verifiera permissions

### Problem: Index skapas inte

**Lösning**:
1. Kontrollera att tabellen skapades först
2. Verifiera att inga dubletter finns i index-namn
3. Kolla disk space

### Problem: Kan inte INSERT data

**Lösning**:
1. Verifiera att alla required kolumner fylls i
2. Kontrollera datatyper och längder
3. Se till att du har INSERT-permissions

---

## 📚 Relaterade Filer

- **C# Model**: `ArvidsonFoto/Core/Models/TblKontakt.cs`
- **DbContext**: `ArvidsonFoto/Core/Data/ArvidsonFotoCoreDbContext.cs`
- **Service**: `ArvidsonFoto/Core/Services/ContactService.cs`
- **Controller**: `ArvidsonFoto/Controllers/InfoController.cs`
- **DTO**: `ArvidsonFoto/Core/DTOs/ContactFormDto.cs`

---

## 📝 Changelog

### Version 1.0 (2025-01-20)
- ✅ Initial creation av tbl_kontakt
- ✅ Added indexes för prestanda
- ✅ Added default constraints
- ✅ Created sample queries
- ✅ Added documentation

---

## 🔗 Länkar

- [SQL Server Documentation](https://learn.microsoft.com/en-us/sql/sql-server/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [SQL Server Index Design Guide](https://learn.microsoft.com/en-us/sql/relational-databases/sql-server-index-design-guide)

---

**Skapad**: 2025-01-20  
**Version**: 1.0  
**Status**: ✅ Production Ready
