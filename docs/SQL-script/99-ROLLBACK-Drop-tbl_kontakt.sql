-- =============================================
-- SQL Script: ROLLBACK - Drop tbl_kontakt
-- Description: Removes the contact form table and all associated objects
-- Author: ArvidsonFoto Migration
-- Date: 2025-01-20
-- Version: 1.0
-- WARNING: This will DELETE ALL DATA in tbl_kontakt!
-- =============================================

USE [ArvidsonFoto]
GO

PRINT '========================================='
PRINT 'ROLLBACK SCRIPT FOR tbl_kontakt'
PRINT '========================================='
PRINT ''
PRINT 'WARNING: This script will:'
PRINT '  1. Drop all indexes on tbl_kontakt'
PRINT '  2. Drop all constraints on tbl_kontakt'
PRINT '  3. Drop the stored procedure sp_GetContactSubmission'
PRINT '  4. DROP the tbl_kontakt table'
PRINT '  5. ALL DATA WILL BE LOST!'
PRINT ''
PRINT 'Press Ctrl+C to cancel or wait 10 seconds to continue...'
GO

WAITFOR DELAY '00:00:10'
GO

-- =============================================
-- STEP 1: Create backup before dropping
-- =============================================
PRINT ''
PRINT 'Step 1: Creating backup...'
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tbl_kontakt]') AND type in (N'U'))
BEGIN
    DECLARE @BackupTableName NVARCHAR(128)
    SET @BackupTableName = 'tbl_kontakt_backup_' + FORMAT(GETDATE(), 'yyyyMMdd_HHmmss')
    
    DECLARE @SQL NVARCHAR(MAX)
    SET @SQL = 'SELECT * INTO ' + @BackupTableName + ' FROM tbl_kontakt'
    
    EXEC sp_executesql @SQL
    
    DECLARE @RowCount INT
    SET @SQL = 'SELECT @Count = COUNT(*) FROM ' + @BackupTableName
    EXEC sp_executesql @SQL, N'@Count INT OUTPUT', @Count = @RowCount OUTPUT
    
    PRINT 'Backup created: ' + @BackupTableName + ' with ' + CAST(@RowCount AS NVARCHAR(10)) + ' rows'
END
ELSE
BEGIN
    PRINT 'Table tbl_kontakt does not exist. No backup needed.'
END
GO

-- =============================================
-- STEP 2: Drop stored procedure
-- =============================================
PRINT ''
PRINT 'Step 2: Dropping stored procedure...'
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetContactSubmission]') AND type in (N'P', N'PC'))
BEGIN
    DROP PROCEDURE [dbo].[sp_GetContactSubmission]
    PRINT 'Stored procedure sp_GetContactSubmission dropped.'
END
ELSE
BEGIN
    PRINT 'Stored procedure sp_GetContactSubmission does not exist.'
END
GO

-- =============================================
-- STEP 3: Drop non-clustered indexes
-- =============================================
PRINT ''
PRINT 'Step 3: Dropping indexes...'
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[tbl_kontakt]') AND name = N'IX_tbl_kontakt_EmailSent')
BEGIN
    DROP INDEX [IX_tbl_kontakt_EmailSent] ON [dbo].[tbl_kontakt]
    PRINT 'Index IX_tbl_kontakt_EmailSent dropped.'
END
ELSE
BEGIN
    PRINT 'Index IX_tbl_kontakt_EmailSent does not exist.'
END
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[tbl_kontakt]') AND name = N'IX_tbl_kontakt_SubmitDate')
BEGIN
    DROP INDEX [IX_tbl_kontakt_SubmitDate] ON [dbo].[tbl_kontakt]
    PRINT 'Index IX_tbl_kontakt_SubmitDate dropped.'
END
ELSE
BEGIN
    PRINT 'Index IX_tbl_kontakt_SubmitDate does not exist.'
END
GO

-- =============================================
-- STEP 4: Drop constraints
-- =============================================
PRINT ''
PRINT 'Step 4: Dropping constraints...'
GO

IF EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_tbl_kontakt_SubmitDate]') AND parent_object_id = OBJECT_ID(N'[dbo].[tbl_kontakt]'))
BEGIN
    ALTER TABLE [dbo].[tbl_kontakt] DROP CONSTRAINT [DF_tbl_kontakt_SubmitDate]
    PRINT 'Default constraint DF_tbl_kontakt_SubmitDate dropped.'
END
ELSE
BEGIN
    PRINT 'Default constraint DF_tbl_kontakt_SubmitDate does not exist.'
END
GO

-- =============================================
-- STEP 5: Drop the table
-- =============================================
PRINT ''
PRINT 'Step 5: Dropping table tbl_kontakt...'
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tbl_kontakt]') AND type in (N'U'))
BEGIN
    DROP TABLE [dbo].[tbl_kontakt]
    PRINT 'Table tbl_kontakt dropped successfully!'
END
ELSE
BEGIN
    PRINT 'Table tbl_kontakt does not exist.'
END
GO

-- =============================================
-- VERIFICATION
-- =============================================
PRINT ''
PRINT '========================================='
PRINT 'ROLLBACK VERIFICATION'
PRINT '========================================='
GO

-- Check if table still exists
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tbl_kontakt]') AND type in (N'U'))
BEGIN
    PRINT '✓ Table tbl_kontakt successfully removed'
END
ELSE
BEGIN
    PRINT '✗ ERROR: Table tbl_kontakt still exists!'
END
GO

-- Check if stored procedure still exists
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetContactSubmission]') AND type in (N'P', N'PC'))
BEGIN
    PRINT '✓ Stored procedure sp_GetContactSubmission successfully removed'
END
ELSE
BEGIN
    PRINT '✗ ERROR: Stored procedure sp_GetContactSubmission still exists!'
END
GO

-- List backup tables
PRINT ''
PRINT 'Available backup tables:'
SELECT 
    name AS BackupTableName,
    create_date AS CreatedDate
FROM sys.tables
WHERE name LIKE 'tbl_kontakt_backup_%'
ORDER BY create_date DESC
GO

PRINT ''
PRINT '========================================='
PRINT 'ROLLBACK COMPLETED'
PRINT '========================================='
PRINT ''
PRINT 'IMPORTANT NOTES:'
PRINT '  - All data from tbl_kontakt has been backed up'
PRINT '  - The table and all associated objects have been removed'
PRINT '  - To restore, run the backup table query above'
PRINT '  - To recreate, run 01-Create-tbl_kontakt.sql'
PRINT ''
GO

-- =============================================
-- OPTIONAL: Restore from backup
-- =============================================
/*
-- Uncomment to restore from a specific backup

-- 1. Find your backup table name from the list above
-- 2. Replace 'tbl_kontakt_backup_YYYYMMDD_HHMMSS' with your backup name
-- 3. Uncomment and run:

SELECT * INTO tbl_kontakt 
FROM tbl_kontakt_backup_YYYYMMDD_HHMMSS

-- 4. Recreate indexes and constraints by running 01-Create-tbl_kontakt.sql

PRINT 'Table restored from backup!'
*/
GO
