-- =============================================
-- SQL Script: Sample Queries for tbl_kontakt
-- Description: Useful queries for managing contact form submissions
-- Author: ArvidsonFoto Migration
-- Date: 2025-12-29
-- Version: 1.0
-- =============================================

USE [ArvidsonFoto]
GO

-- =============================================
-- SECTION 1: SELECT QUERIES
-- =============================================

-- Get all contact form submissions (latest first)
SELECT 
    ID,
    SubmitDate,
    Name,
    Email,
    Subject,
    LEFT(Message, 100) + CASE WHEN LEN(Message) > 100 THEN '...' ELSE '' END AS MessagePreview,
    SourcePage,
    EmailSent,
    ErrorMessage
FROM tbl_kontakt
ORDER BY SubmitDate DESC
GO

-- Get failed email submissions only
SELECT 
    ID,
    SubmitDate,
    Name,
    Email,
    Subject,
    Message,
    SourcePage,
    ErrorMessage
FROM tbl_kontakt
WHERE EmailSent = 0
ORDER BY SubmitDate DESC
GO

-- Get submissions from last 7 days
SELECT 
    ID,
    SubmitDate,
    Name,
    Email,
    Subject,
    SourcePage,
    EmailSent
FROM tbl_kontakt
WHERE SubmitDate >= DATEADD(DAY, -7, GETDATE())
ORDER BY SubmitDate DESC
GO

-- Count submissions by source page
SELECT 
    SourcePage,
    COUNT(*) AS TotalSubmissions,
    SUM(CASE WHEN EmailSent = 1 THEN 1 ELSE 0 END) AS SuccessfulEmails,
    SUM(CASE WHEN EmailSent = 0 THEN 1 ELSE 0 END) AS FailedEmails
FROM tbl_kontakt
GROUP BY SourcePage
ORDER BY TotalSubmissions DESC
GO

-- Get submissions per month (last 12 months)
SELECT 
    FORMAT(SubmitDate, 'yyyy-MM') AS YearMonth,
    COUNT(*) AS TotalSubmissions,
    SUM(CASE WHEN EmailSent = 1 THEN 1 ELSE 0 END) AS SuccessfulEmails,
    SUM(CASE WHEN EmailSent = 0 THEN 1 ELSE 0 END) AS FailedEmails
FROM tbl_kontakt
WHERE SubmitDate >= DATEADD(MONTH, -12, GETDATE())
GROUP BY FORMAT(SubmitDate, 'yyyy-MM')
ORDER BY YearMonth DESC
GO

-- Get most active users (by email)
SELECT 
    Email,
    Name,
    COUNT(*) AS SubmissionCount,
    MIN(SubmitDate) AS FirstSubmission,
    MAX(SubmitDate) AS LastSubmission
FROM tbl_kontakt
GROUP BY Email, Name
HAVING COUNT(*) > 1
ORDER BY SubmissionCount DESC
GO

-- =============================================
-- SECTION 2: ADMIN QUERIES
-- =============================================

-- Find submissions that need follow-up (failed emails)
SELECT 
    ID,
    SubmitDate,
    Name,
    Email,
    Subject,
    Message,
    SourcePage,
    ErrorMessage,
    DATEDIFF(HOUR, SubmitDate, GETDATE()) AS HoursSinceSubmission
FROM tbl_kontakt
WHERE EmailSent = 0
    AND SubmitDate >= DATEADD(DAY, -30, GETDATE()) -- Last 30 days
ORDER BY SubmitDate DESC
GO

-- Get full details for a specific submission
-- EXEC sp_GetContactSubmission @ID = 1
GO
CREATE OR ALTER PROCEDURE sp_GetContactSubmission
    @ID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ID,
        SubmitDate,
        Name,
        Email,
        Subject,
        Message,
        SourcePage,
        EmailSent,
        ErrorMessage,
        DATEDIFF(MINUTE, SubmitDate, GETDATE()) AS MinutesAgo
    FROM tbl_kontakt
    WHERE ID = @ID
END
GO

-- =============================================
-- SECTION 3: MAINTENANCE QUERIES
-- =============================================

-- Count total submissions
SELECT COUNT(*) AS TotalSubmissions FROM tbl_kontakt
GO

-- Get table size information
SELECT 
    t.NAME AS TableName,
    s.Name AS SchemaName,
    p.rows AS RowCounts,
    SUM(a.total_pages) * 8 AS TotalSpaceKB, 
    SUM(a.used_pages) * 8 AS UsedSpaceKB, 
    (SUM(a.total_pages) - SUM(a.used_pages)) * 8 AS UnusedSpaceKB
FROM sys.tables t
INNER JOIN sys.indexes i ON t.OBJECT_ID = i.object_id
INNER JOIN sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id
INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id
LEFT OUTER JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE t.NAME = 'tbl_kontakt'
    AND t.is_ms_shipped = 0
    AND i.OBJECT_ID > 255 
GROUP BY t.Name, s.Name, p.Rows
GO

-- Check for duplicate submissions (same email, similar time)
SELECT 
    k1.ID AS ID1,
    k2.ID AS ID2,
    k1.Email,
    k1.Name,
    k1.SubmitDate AS SubmitDate1,
    k2.SubmitDate AS SubmitDate2,
    DATEDIFF(SECOND, k1.SubmitDate, k2.SubmitDate) AS SecondsBetween
FROM tbl_kontakt k1
INNER JOIN tbl_kontakt k2 
    ON k1.Email = k2.Email 
    AND k1.ID < k2.ID
    AND DATEDIFF(MINUTE, k1.SubmitDate, k2.SubmitDate) <= 5 -- Within 5 minutes
ORDER BY k1.SubmitDate DESC
GO

-- =============================================
-- SECTION 4: DATA CLEANUP (USE WITH CAUTION!)
-- =============================================

-- Archive old successful submissions (older than 1 year)
-- NOTE: Uncomment to execute, but create backup first!
/*
-- Create archive table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tbl_kontakt_archive]') AND type in (N'U'))
BEGIN
    SELECT * INTO tbl_kontakt_archive FROM tbl_kontakt WHERE 1=0
END
GO

-- Move old successful emails to archive
INSERT INTO tbl_kontakt_archive
SELECT * FROM tbl_kontakt
WHERE EmailSent = 1 
    AND SubmitDate < DATEADD(YEAR, -1, GETDATE())

-- Delete archived records from main table
DELETE FROM tbl_kontakt
WHERE EmailSent = 1 
    AND SubmitDate < DATEADD(YEAR, -1, GETDATE())
    AND ID IN (SELECT ID FROM tbl_kontakt_archive)
GO
*/

-- =============================================
-- SECTION 5: TESTING QUERIES
-- =============================================

-- Insert test data (for development/testing only)
/*
INSERT INTO tbl_kontakt (SubmitDate, Name, Email, Subject, Message, SourcePage, EmailSent, ErrorMessage)
VALUES 
    (GETDATE(), 'Test User', 'test@example.com', 'Test Subject', 'Test message content', 'Kontakta', 1, NULL),
    (DATEADD(DAY, -1, GETDATE()), 'Another User', 'user@example.com', 'Question', 'I have a question', 'Kop_av_bilder', 1, NULL),
    (DATEADD(DAY, -2, GETDATE()), 'Failed Email', 'fail@example.com', 'Failed Test', 'This email failed', 'Kontakta', 0, 'SMTP connection failed')
GO
*/

PRINT '=== Sample Queries Script Completed ==='
PRINT 'All query templates are ready to use.'
GO
