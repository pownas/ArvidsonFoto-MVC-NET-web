-- =============================================
-- SQL Script: Create AspNetUserPasskeys Table
-- Description: Creates the passkey table for WebAuthn / passkey support
--              (ASP.NET Core Identity v3, .NET 10).
--              Equivalent to EF Core migration: 20250315234213_AddPasskeySupport
-- Author: ArvidsonFoto Migration
-- Date: 2025-03-15
-- Version: 1.0
-- =============================================
-- IMPORTANT: After running this script you MUST also run the
--            __EFMigrationsHistory insert at the bottom of this file
--            so that EF Core does not try to re-apply the migration
--            on the next application start-up or 'dotnet ef database update'.
-- =============================================

USE [ArvidsonFoto]
GO

-- =============================================
-- STEP 1: Create AspNetUserPasskeys table
-- =============================================
PRINT '========================================='
PRINT 'Creating AspNetUserPasskeys table'
PRINT '========================================='
PRINT ''

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserPasskeys]') AND type = N'U')
BEGIN
    PRINT 'Creating table AspNetUserPasskeys...'

    CREATE TABLE [dbo].[AspNetUserPasskeys] (
        [CredentialId] [varbinary](1024) NOT NULL,
        [UserId]       [nvarchar](450)   NOT NULL,
        [Data]         [nvarchar](max)   NOT NULL,

        CONSTRAINT [PK_AspNetUserPasskeys]
            PRIMARY KEY CLUSTERED ([CredentialId] ASC)
            WITH (
                PAD_INDEX              = OFF,
                STATISTICS_NORECOMPUTE = OFF,
                IGNORE_DUP_KEY         = OFF,
                ALLOW_ROW_LOCKS        = ON,
                ALLOW_PAGE_LOCKS       = ON
            ) ON [PRIMARY],

        CONSTRAINT [FK_AspNetUserPasskeys_AspNetUsers_UserId]
            FOREIGN KEY ([UserId])
            REFERENCES [dbo].[AspNetUsers] ([Id])
            ON DELETE CASCADE
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

    PRINT 'Table AspNetUserPasskeys created successfully!'
END
ELSE
BEGIN
    PRINT 'Table AspNetUserPasskeys already exists. Skipping creation.'
END
GO

-- =============================================
-- STEP 2: Create index on UserId
-- =============================================
PRINT ''
PRINT 'Creating index IX_AspNetUserPasskeys_UserId...'
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserPasskeys]') AND name = N'IX_AspNetUserPasskeys_UserId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_AspNetUserPasskeys_UserId]
        ON [dbo].[AspNetUserPasskeys] ([UserId] ASC)
        WITH (
            PAD_INDEX              = OFF,
            STATISTICS_NORECOMPUTE = OFF,
            SORT_IN_TEMPDB         = OFF,
            DROP_EXISTING          = OFF,
            ONLINE                 = OFF,
            ALLOW_ROW_LOCKS        = ON,
            ALLOW_PAGE_LOCKS       = ON
        ) ON [PRIMARY]

    PRINT 'Index IX_AspNetUserPasskeys_UserId created successfully!'
END
ELSE
BEGIN
    PRINT 'Index IX_AspNetUserPasskeys_UserId already exists. Skipping creation.'
END
GO

-- =============================================
-- STEP 3: Register migration in EF history table
--         so EF Core does not re-apply this migration
-- =============================================
PRINT ''
PRINT 'Registering migration in __EFMigrationsHistory...'
GO

IF NOT EXISTS (
    SELECT 1 FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250315234213_AddPasskeySupport'
)
BEGIN
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250315234213_AddPasskeySupport', N'10.0.5')

    PRINT 'Migration 20250315234213_AddPasskeySupport registered successfully!'
END
ELSE
BEGIN
    PRINT 'Migration 20250315234213_AddPasskeySupport already registered. Skipping.'
END
GO

-- =============================================
-- VERIFICATION
-- =============================================
PRINT ''
PRINT '========================================='
PRINT 'VERIFICATION'
PRINT '========================================='
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserPasskeys]') AND type = N'U')
    PRINT '✓ Table AspNetUserPasskeys exists'
ELSE
    PRINT '✗ ERROR: Table AspNetUserPasskeys was NOT created!'
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserPasskeys]') AND name = N'IX_AspNetUserPasskeys_UserId')
    PRINT '✓ Index IX_AspNetUserPasskeys_UserId exists'
ELSE
    PRINT '✗ ERROR: Index IX_AspNetUserPasskeys_UserId was NOT created!'
GO

IF EXISTS (
    SELECT 1 FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250315234213_AddPasskeySupport'
)
    PRINT '✓ Migration 20250315234213_AddPasskeySupport registered in __EFMigrationsHistory'
ELSE
    PRINT '✗ ERROR: Migration was NOT registered in __EFMigrationsHistory!'
GO

PRINT ''
PRINT 'Columns in AspNetUserPasskeys:'
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'AspNetUserPasskeys'
ORDER BY ORDINAL_POSITION
GO

PRINT ''
PRINT '========================================='
PRINT 'Script completed successfully!'
PRINT 'AspNetUserPasskeys table is ready to use.'
PRINT '========================================='
GO
