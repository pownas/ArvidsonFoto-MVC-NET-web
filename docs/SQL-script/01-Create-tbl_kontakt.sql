-- =============================================
-- SQL Script: Create tbl_kontakt Table
-- Description: Creates the contact form submission backup table
-- Author: ArvidsonFoto Migration
-- Date: 2025-12-29
-- Version: 1.0
-- =============================================

USE [ArvidsonFoto]
GO

-- Check if table already exists
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tbl_kontakt]') AND type in (N'U'))
BEGIN
    PRINT 'Creating table tbl_kontakt...'
    
    CREATE TABLE [dbo].[tbl_kontakt](
        [ID] [int] IDENTITY(1,1) NOT NULL,
        [SubmitDate] [datetime] NOT NULL,
        [Name] [nvarchar](50) NOT NULL,
        [Email] [nvarchar](150) NOT NULL,
        [Subject] [nvarchar](50) NOT NULL,
        [Message] [nvarchar](2000) NOT NULL,
        [SourcePage] [nvarchar](50) NOT NULL,
        [EmailSent] [bit] NOT NULL DEFAULT 0,
        [ErrorMessage] [nvarchar](500) NULL,
        
        CONSTRAINT [PK_tbl_kontakt] PRIMARY KEY CLUSTERED ([ID] ASC)
        WITH (
            PAD_INDEX = OFF, 
            STATISTICS_NORECOMPUTE = OFF, 
            IGNORE_DUP_KEY = OFF, 
            ALLOW_ROW_LOCKS = ON, 
            ALLOW_PAGE_LOCKS = ON
        ) ON [PRIMARY]
    ) ON [PRIMARY]
    
    PRINT 'Table tbl_kontakt created successfully!'
END
ELSE
BEGIN
    PRINT 'Table tbl_kontakt already exists. Skipping creation.'
END
GO

-- Create non-clustered index on SubmitDate for faster queries
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[tbl_kontakt]') AND name = N'IX_tbl_kontakt_SubmitDate')
BEGIN
    PRINT 'Creating index on SubmitDate...'
    
    CREATE NONCLUSTERED INDEX [IX_tbl_kontakt_SubmitDate]
    ON [dbo].[tbl_kontakt] ([SubmitDate] DESC)
    INCLUDE ([EmailSent], [SourcePage])
    WITH (
        PAD_INDEX = OFF,
        STATISTICS_NORECOMPUTE = OFF,
        SORT_IN_TEMPDB = OFF,
        DROP_EXISTING = OFF,
        ONLINE = OFF,
        ALLOW_ROW_LOCKS = ON,
        ALLOW_PAGE_LOCKS = ON
    ) ON [PRIMARY]
    
    PRINT 'Index IX_tbl_kontakt_SubmitDate created successfully!'
END
ELSE
BEGIN
    PRINT 'Index IX_tbl_kontakt_SubmitDate already exists. Skipping creation.'
END
GO

-- Create non-clustered index on EmailSent for filtering failed emails
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[tbl_kontakt]') AND name = N'IX_tbl_kontakt_EmailSent')
BEGIN
    PRINT 'Creating index on EmailSent...'
    
    CREATE NONCLUSTERED INDEX [IX_tbl_kontakt_EmailSent]
    ON [dbo].[tbl_kontakt] ([EmailSent])
    INCLUDE ([SubmitDate], [Name], [Email])
    WITH (
        PAD_INDEX = OFF,
        STATISTICS_NORECOMPUTE = OFF,
        SORT_IN_TEMPDB = OFF,
        DROP_EXISTING = OFF,
        ONLINE = OFF,
        ALLOW_ROW_LOCKS = ON,
        ALLOW_PAGE_LOCKS = ON
    ) ON [PRIMARY]
    
    PRINT 'Index IX_tbl_kontakt_EmailSent created successfully!'
END
ELSE
BEGIN
    PRINT 'Index IX_tbl_kontakt_EmailSent already exists. Skipping creation.'
END
GO

-- Add default constraint for SubmitDate
IF NOT EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_tbl_kontakt_SubmitDate]') AND parent_object_id = OBJECT_ID(N'[dbo].[tbl_kontakt]'))
BEGIN
    PRINT 'Adding default constraint for SubmitDate...'
    
    ALTER TABLE [dbo].[tbl_kontakt] 
    ADD CONSTRAINT [DF_tbl_kontakt_SubmitDate] DEFAULT (GETDATE()) FOR [SubmitDate]
    
    PRINT 'Default constraint for SubmitDate added successfully!'
END
ELSE
BEGIN
    PRINT 'Default constraint for SubmitDate already exists. Skipping.'
END
GO

-- Verify table structure
PRINT ''
PRINT '=== Verification ==='
PRINT 'Columns in tbl_kontakt:'
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'tbl_kontakt'
ORDER BY ORDINAL_POSITION
GO

PRINT ''
PRINT 'Indexes on tbl_kontakt:'
SELECT 
    i.name AS IndexName,
    i.type_desc AS IndexType,
    COL_NAME(ic.object_id, ic.column_id) AS ColumnName,
    ic.is_included_column AS IsIncluded
FROM sys.indexes AS i
INNER JOIN sys.index_columns AS ic 
    ON i.object_id = ic.object_id AND i.index_id = ic.index_id
WHERE i.object_id = OBJECT_ID('tbl_kontakt')
ORDER BY i.name, ic.key_ordinal
GO

PRINT ''
PRINT '=== Script completed successfully! ==='
PRINT 'Table tbl_kontakt is ready to use.'
GO
