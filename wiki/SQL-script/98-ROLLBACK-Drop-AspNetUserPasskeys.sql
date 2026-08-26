-- =============================================
-- SQL Script: ROLLBACK - Drop AspNetUserPasskeys Table
-- Description: Removes the passkey table and de-registers the EF Core migration.
--              Equivalent to rolling back EF migration: 20250315234213_AddPasskeySupport
-- Author: ArvidsonFoto Migration
-- Date: 2025-03-15
-- Version: 1.0
-- WARNING: This will DELETE ALL passkey credentials stored in AspNetUserPasskeys!
--          Users will no longer be able to sign in with passkeys.
-- =============================================

USE [ArvidsonFoto]
GO

PRINT '========================================='
PRINT 'ROLLBACK SCRIPT FOR AspNetUserPasskeys'
PRINT '========================================='
PRINT ''
PRINT 'WARNING: This script will:'
PRINT '  1. Drop the index IX_AspNetUserPasskeys_UserId'
PRINT '  2. Drop the foreign key FK_AspNetUserPasskeys_AspNetUsers_UserId'
PRINT '  3. DROP the AspNetUserPasskeys table'
PRINT '  4. Remove the migration entry from __EFMigrationsHistory'
PRINT '  5. ALL PASSKEY DATA WILL BE LOST!'
PRINT ''
PRINT 'Press Ctrl+C to cancel or wait 10 seconds to continue...'
GO

WAITFOR DELAY '00:00:10'
GO

-- =============================================
-- STEP 1: Drop index
-- =============================================
PRINT ''
PRINT 'Step 1: Dropping index IX_AspNetUserPasskeys_UserId...'
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserPasskeys]') AND name = N'IX_AspNetUserPasskeys_UserId')
BEGIN
    DROP INDEX [IX_AspNetUserPasskeys_UserId] ON [dbo].[AspNetUserPasskeys]
    PRINT 'Index IX_AspNetUserPasskeys_UserId dropped.'
END
ELSE
BEGIN
    PRINT 'Index IX_AspNetUserPasskeys_UserId does not exist.'
END
GO

-- =============================================
-- STEP 2: Drop the table (cascade drops the FK automatically)
-- =============================================
PRINT ''
PRINT 'Step 2: Dropping table AspNetUserPasskeys...'
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserPasskeys]') AND type = N'U')
BEGIN
    DROP TABLE [dbo].[AspNetUserPasskeys]
    PRINT 'Table AspNetUserPasskeys dropped successfully!'
END
ELSE
BEGIN
    PRINT 'Table AspNetUserPasskeys does not exist.'
END
GO

-- =============================================
-- STEP 3: De-register migration from EF history table
-- =============================================
PRINT ''
PRINT 'Step 3: Removing migration from __EFMigrationsHistory...'
GO

IF EXISTS (
    SELECT 1 FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250315234213_AddPasskeySupport'
)
BEGIN
    DELETE FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250315234213_AddPasskeySupport'

    PRINT 'Migration 20250315234213_AddPasskeySupport removed from __EFMigrationsHistory.'
END
ELSE
BEGIN
    PRINT 'Migration 20250315234213_AddPasskeySupport not found in __EFMigrationsHistory.'
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

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserPasskeys]') AND type = N'U')
    PRINT '✓ Table AspNetUserPasskeys successfully removed'
ELSE
    PRINT '✗ ERROR: Table AspNetUserPasskeys still exists!'
GO

IF NOT EXISTS (
    SELECT 1 FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250315234213_AddPasskeySupport'
)
    PRINT '✓ Migration 20250315234213_AddPasskeySupport removed from __EFMigrationsHistory'
ELSE
    PRINT '✗ ERROR: Migration entry still exists in __EFMigrationsHistory!'
GO

PRINT ''
PRINT '========================================='
PRINT 'ROLLBACK COMPLETED'
PRINT '========================================='
PRINT ''
PRINT 'IMPORTANT NOTES:'
PRINT '  - All passkey credentials have been deleted'
PRINT '  - Users can no longer sign in with passkeys'
PRINT '  - To re-apply, run 03-Create-AspNetUserPasskeys.sql'
PRINT ''
GO
