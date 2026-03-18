-- ===========================================================================
-- SQL Script: Add English Language Support Columns
-- ===========================================================================
-- Description: This script adds English language columns to support 
--              multilingual content in the ArvidsonFoto database.
--              It adds nullable nvarchar columns for English translations
--              of existing Swedish text fields.
--
-- Database: ArvidsonFoto
-- Date: 2025-12-29
-- Version: 1.0
-- Author: Copilot
--
-- Tables Modified:
--   - tbl_gb (Guestbook)
--   - tbl_menu (Menu Categories)  
--   - tbl_images (Images)
--
-- Migration: This script corresponds to EF Core migration
--            "20251229090805_AddEnglishColumns"
--
-- IMPORTANT: 
--   - All new columns are NULLABLE to prevent breaking existing data
--   - Existing functionality will continue to work unchanged
--   - English values can be populated later via application or manual UPDATE
-- ===========================================================================

USE [ArvidsonFoto]
GO

-- ===========================================================================
-- Table: tbl_gb (Guestbook)
-- Description: Add English columns for guestbook entries
-- ===========================================================================

-- Add English name column
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[tbl_gb]') 
    AND name = 'GB_name_EN'
)
BEGIN
    ALTER TABLE [dbo].[tbl_gb]
    ADD [GB_name_EN] NVARCHAR(100) NULL;
    
    PRINT 'Added column GB_name_EN to tbl_gb';
END
ELSE
BEGIN
    PRINT 'Column GB_name_EN already exists in tbl_gb';
END
GO

-- Add English text column
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[tbl_gb]') 
    AND name = 'GB_text_EN'
)
BEGIN
    ALTER TABLE [dbo].[tbl_gb]
    ADD [GB_text_EN] NVARCHAR(MAX) NULL;
    
    PRINT 'Added column GB_text_EN to tbl_gb';
END
ELSE
BEGIN
    PRINT 'Column GB_text_EN already exists in tbl_gb';
END
GO

-- ===========================================================================
-- Table: tbl_menu (Menu Categories)
-- Description: Add English columns for menu display names and URL segments
-- ===========================================================================

-- Add English menu display name column
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[tbl_menu]') 
    AND name = 'menu_text_EN'
)
BEGIN
    ALTER TABLE [dbo].[tbl_menu]
    ADD [menu_text_EN] NVARCHAR(50) NULL;
    
    PRINT 'Added column menu_text_EN to tbl_menu';
END
ELSE
BEGIN
    PRINT 'Column menu_text_EN already exists in tbl_menu';
END
GO

-- Add English menu URL segment column
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[tbl_menu]') 
    AND name = 'menu_URLtext_EN'
)
BEGIN
    ALTER TABLE [dbo].[tbl_menu]
    ADD [menu_URLtext_EN] NVARCHAR(50) NULL;
    
    PRINT 'Added column menu_URLtext_EN to tbl_menu';
END
ELSE
BEGIN
    PRINT 'Column menu_URLtext_EN already exists in tbl_menu';
END
GO

-- ===========================================================================
-- Table: tbl_images (Images)
-- Description: Add English column for image descriptions
-- ===========================================================================

-- Add English image description column
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[tbl_images]') 
    AND name = 'image_description_EN'
)
BEGIN
    ALTER TABLE [dbo].[tbl_images]
    ADD [image_description_EN] NVARCHAR(150) NULL;
    
    PRINT 'Added column image_description_EN to tbl_images';
END
ELSE
BEGIN
    PRINT 'Column image_description_EN already exists in tbl_images';
END
GO

-- ===========================================================================
-- Verification Query
-- Description: Verify that all columns were added successfully
-- ===========================================================================

PRINT '';
PRINT '========== Verification Results ==========';
PRINT '';

-- Check tbl_gb columns
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[tbl_gb]') AND name = 'GB_name_EN')
    PRINT '✓ tbl_gb.GB_name_EN exists';
ELSE
    PRINT '✗ tbl_gb.GB_name_EN MISSING';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[tbl_gb]') AND name = 'GB_text_EN')
    PRINT '✓ tbl_gb.GB_text_EN exists';
ELSE
    PRINT '✗ tbl_gb.GB_text_EN MISSING';

-- Check tbl_menu columns
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[tbl_menu]') AND name = 'menu_text_EN')
    PRINT '✓ tbl_menu.menu_text_EN exists';
ELSE
    PRINT '✗ tbl_menu.menu_text_EN MISSING';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[tbl_menu]') AND name = 'menu_URLtext_EN')
    PRINT '✓ tbl_menu.menu_URLtext_EN exists';
ELSE
    PRINT '✗ tbl_menu.menu_URLtext_EN MISSING';

-- Check tbl_images columns
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[tbl_images]') AND name = 'image_description_EN')
    PRINT '✓ tbl_images.image_description_EN exists';
ELSE
    PRINT '✗ tbl_images.image_description_EN MISSING';

PRINT '';
PRINT '========== Script Complete ==========';
PRINT '';

-- ===========================================================================
-- OPTIONAL: Initialize English columns with Swedish values
-- ===========================================================================
-- Uncomment the following section if you want to copy Swedish text to 
-- English columns as a starting point for translation.
--
-- NOTE: This is OPTIONAL and can be run later if needed.
-- ===========================================================================

/*
PRINT 'Initializing English columns with Swedish values...';

-- Copy Swedish guestbook data to English columns
UPDATE [dbo].[tbl_gb]
SET 
    [GB_name_EN] = [GB_name],
    [GB_text_EN] = [GB_text]
WHERE [GB_name_EN] IS NULL;

-- Copy Swedish menu data to English columns
UPDATE [dbo].[tbl_menu]
SET 
    [menu_text_EN] = [menu_text],
    [menu_URLtext_EN] = [menu_URLtext]
WHERE [menu_text_EN] IS NULL;

-- Copy Swedish image descriptions to English columns
UPDATE [dbo].[tbl_images]
SET 
    [image_description_EN] = [image_description]
WHERE [image_description_EN] IS NULL;

PRINT 'English columns initialized with Swedish values.';
PRINT 'You can now update these values with proper English translations.';
*/

-- ===========================================================================
-- ROLLBACK SCRIPT (if needed)
-- ===========================================================================
-- Uncomment and run this section if you need to remove the English columns
-- ===========================================================================

/*
PRINT 'Rolling back English column additions...';

-- Remove columns from tbl_gb
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[tbl_gb]') AND name = 'GB_name_EN')
BEGIN
    ALTER TABLE [dbo].[tbl_gb] DROP COLUMN [GB_name_EN];
    PRINT 'Removed GB_name_EN from tbl_gb';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[tbl_gb]') AND name = 'GB_text_EN')
BEGIN
    ALTER TABLE [dbo].[tbl_gb] DROP COLUMN [GB_text_EN];
    PRINT 'Removed GB_text_EN from tbl_gb';
END

-- Remove columns from tbl_menu
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[tbl_menu]') AND name = 'menu_text_EN')
BEGIN
    ALTER TABLE [dbo].[tbl_menu] DROP COLUMN [menu_text_EN];
    PRINT 'Removed menu_text_EN from tbl_menu';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[tbl_menu]') AND name = 'menu_URLtext_EN')
BEGIN
    ALTER TABLE [dbo].[tbl_menu] DROP COLUMN [menu_URLtext_EN];
    PRINT 'Removed menu_URLtext_EN from tbl_menu';
END

-- Remove columns from tbl_images
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[tbl_images]') AND name = 'image_description_EN')
BEGIN
    ALTER TABLE [dbo].[tbl_images] DROP COLUMN [image_description_EN];
    PRINT 'Removed image_description_EN from tbl_images';
END

PRINT 'Rollback complete.';
*/

GO
