/*
    Upgrade of the restored Arvidsonfoto-db-backup-with-data-20261020.sql copy.

    This is deliberately separate from 01-Core.idempotent.sql and
    02-Identity.idempotent.sql. It recognizes the legacy migration history in
    this backup, preserves the existing rows, applies the current model's
    missing schema, and baselines the current EF migration IDs only after the
    schema changes succeed.

    Each DDL/data phase commits separately to limit transaction-log usage.
    If a phase fails, rerun the script after resolving the reported issue;
    the completed phases are guarded and the EF migration IDs are recorded
    only after every phase succeeds. Keep the application offline until then.

    Run against a restored copy first. Do not run against production without
    reviewing the target database and taking a fresh backup.
*/
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;

IF OBJECT_ID(N'[dbo].[__EFMigrationsHistory]', N'U') IS NULL
    THROW 51000, 'Expected dbo.__EFMigrationsHistory was not found. This script only supports the 2026-10-20 database dump.', 1;

IF NOT EXISTS (SELECT 1 FROM [dbo].[__EFMigrationsHistory] WHERE [MigrationId] = N'20210204094738_SkaparTabeller')
    THROW 51001, 'Legacy core migration marker is missing. Refusing to apply this backup-specific upgrade.', 1;

IF NOT EXISTS (SELECT 1 FROM [dbo].[__EFMigrationsHistory] WHERE [MigrationId] = N'20250315234213_AddPasskeySupport')
    THROW 51002, 'Expected Identity passkey migration marker is missing. Refusing to apply this backup-specific upgrade.', 1;

IF OBJECT_ID(N'[dbo].[tbl_gb]', N'U') IS NULL
   OR OBJECT_ID(N'[dbo].[tbl_images]', N'U') IS NULL
   OR OBJECT_ID(N'[dbo].[tbl_menu]', N'U') IS NULL
   OR OBJECT_ID(N'[dbo].[tbl_PageCounter]', N'U') IS NULL
   OR OBJECT_ID(N'[dbo].[tbl_kontakt]', N'U') IS NULL
   OR OBJECT_ID(N'[dbo].[AspNetUsers]', N'U') IS NULL
   OR OBJECT_ID(N'[dbo].[AspNetRoles]', N'U') IS NULL
   OR OBJECT_ID(N'[dbo].[AspNetRoleClaims]', N'U') IS NULL
   OR OBJECT_ID(N'[dbo].[AspNetUserClaims]', N'U') IS NULL
   OR OBJECT_ID(N'[dbo].[AspNetUserLogins]', N'U') IS NULL
   OR OBJECT_ID(N'[dbo].[AspNetUserRoles]', N'U') IS NULL
   OR OBJECT_ID(N'[dbo].[AspNetUserTokens]', N'U') IS NULL
   OR OBJECT_ID(N'[dbo].[AspNetUserPasskeys]', N'U') IS NULL
    THROW 51003, 'One or more expected core or Identity tables are missing. No changes were made.', 1;

IF COL_LENGTH(N'dbo.tbl_PageCounter', N'PageCounter_Name') IS NULL
   OR COL_LENGTH(N'dbo.tbl_PageCounter', N'PageCounter_MonthViewed') IS NULL
   OR COL_LENGTH(N'dbo.tbl_PageCounter', N'PageCounter_CategoryId') IS NULL
   OR COL_LENGTH(N'dbo.tbl_menu', N'menu_ID') IS NULL
   OR COL_LENGTH(N'dbo.tbl_menu', N'menu_mainID') IS NULL
   OR COL_LENGTH(N'dbo.tbl_images', N'image_ID') IS NULL
   OR COL_LENGTH(N'dbo.tbl_images', N'image_art') IS NULL
   OR COL_LENGTH(N'dbo.tbl_gb', N'GB_IP') IS NULL
   OR COL_LENGTH(N'dbo.AspNetUsers', N'ShowAllLogs') IS NULL
   OR COL_LENGTH(N'dbo.tbl_kontakt', N'SubmitDate') IS NULL
   OR COL_LENGTH(N'dbo.tbl_kontakt', N'EmailSent') IS NULL
    THROW 51004, 'The existing table definitions do not match the inspected backup. No changes were made.', 1;

IF EXISTS (SELECT 1 FROM [dbo].[tbl_PageCounter] WHERE [PageCounter_Name] IS NULL OR [PageCounter_MonthViewed] IS NULL)
    THROW 51005, 'tbl_PageCounter contains NULL names or months. Resolve those rows explicitly before enforcing the EF-required columns.', 1;

IF EXISTS (
    SELECT [NormalizedUserName]
    FROM [dbo].[AspNetUsers]
    WHERE [NormalizedUserName] IS NOT NULL
    GROUP BY [NormalizedUserName]
    HAVING COUNT(*) > 1
)
    THROW 51006, 'Duplicate normalized user names prevent creation of the Identity unique index.', 1;

IF EXISTS (
    SELECT [NormalizedName]
    FROM [dbo].[AspNetRoles]
    WHERE [NormalizedName] IS NOT NULL
    GROUP BY [NormalizedName]
    HAVING COUNT(*) > 1
)
    THROW 51007, 'Duplicate normalized role names prevent creation of the Identity unique index.', 1;

BEGIN TRY
    PRINT CONCAT(N'Applying the data-preserving schema upgrade to ', QUOTENAME(DB_NAME()), N'.');

    /* Match nullable EF model fields without narrowing or deleting stored values. */
    BEGIN TRANSACTION;

    IF EXISTS (
        SELECT 1 FROM sys.columns
        WHERE [object_id] = OBJECT_ID(N'[dbo].[tbl_menu]')
            AND [name] = N'menu_ID'
            AND [is_nullable] = 0
    )
    BEGIN
        DECLARE @menuUniqueIndexName sysname;
        DECLARE @menuUniqueIndexIsConstraint bit;
        DECLARE @dropMenuIndexSql nvarchar(4000);

        SELECT TOP (1)
            @menuUniqueIndexName = i.[name],
            @menuUniqueIndexIsConstraint = i.[is_unique_constraint]
        FROM sys.indexes AS i
        WHERE i.[object_id] = OBJECT_ID(N'[dbo].[tbl_menu]')
            AND i.[is_unique] = 1
            AND i.[index_id] > 0
            AND EXISTS (
                SELECT 1
                FROM sys.index_columns AS ic
                INNER JOIN sys.columns AS c
                    ON c.[object_id] = ic.[object_id]
                    AND c.[column_id] = ic.[column_id]
                WHERE ic.[object_id] = i.[object_id]
                    AND ic.[index_id] = i.[index_id]
                    AND ic.[key_ordinal] = 1
                    AND c.[name] = N'menu_ID'
            )
            AND NOT EXISTS (
                SELECT 1
                FROM sys.index_columns AS ic
                INNER JOIN sys.columns AS c
                    ON c.[object_id] = ic.[object_id]
                    AND c.[column_id] = ic.[column_id]
                WHERE ic.[object_id] = i.[object_id]
                    AND ic.[index_id] = i.[index_id]
                    AND ic.[key_ordinal] > 1
            );

        IF @menuUniqueIndexName IS NOT NULL AND @menuUniqueIndexIsConstraint = 1
        BEGIN
            SET @dropMenuIndexSql = N'ALTER TABLE [dbo].[tbl_menu] DROP CONSTRAINT ' + QUOTENAME(@menuUniqueIndexName) + N';';
            EXEC sys.sp_executesql @dropMenuIndexSql;
        END
        ELSE IF @menuUniqueIndexName IS NOT NULL
        BEGIN
            SET @dropMenuIndexSql = N'DROP INDEX ' + QUOTENAME(@menuUniqueIndexName) + N' ON [dbo].[tbl_menu];';
            EXEC sys.sp_executesql @dropMenuIndexSql;
        END;

        ALTER TABLE [dbo].[tbl_menu] ALTER COLUMN [menu_ID] int NULL;
    END;

    IF NOT EXISTS (
        SELECT 1
        FROM sys.indexes AS i
        WHERE i.[object_id] = OBJECT_ID(N'[dbo].[tbl_menu]')
            AND i.[is_unique] = 1
            AND i.[index_id] > 0
            AND EXISTS (
                SELECT 1
                FROM sys.index_columns AS ic
                INNER JOIN sys.columns AS c
                    ON c.[object_id] = ic.[object_id]
                    AND c.[column_id] = ic.[column_id]
                WHERE ic.[object_id] = i.[object_id]
                    AND ic.[index_id] = i.[index_id]
                    AND ic.[key_ordinal] = 1
                    AND c.[name] = N'menu_ID'
            )
            AND NOT EXISTS (
                SELECT 1
                FROM sys.index_columns AS ic
                WHERE ic.[object_id] = i.[object_id]
                    AND ic.[index_id] = i.[index_id]
                    AND ic.[key_ordinal] > 1
            )
    )
        CREATE UNIQUE INDEX [UQ_tbl_menu_menu_ID]
            ON [dbo].[tbl_menu] ([menu_ID]);

    IF EXISTS (
        SELECT 1 FROM sys.columns
        WHERE [object_id] = OBJECT_ID(N'[dbo].[tbl_images]')
            AND [name] = N'image_ID'
            AND [is_nullable] = 0
    )
        ALTER TABLE [dbo].[tbl_images] ALTER COLUMN [image_ID] int NULL;

    IF EXISTS (
        SELECT 1 FROM sys.columns
        WHERE [object_id] = OBJECT_ID(N'[dbo].[tbl_images]')
            AND [name] = N'image_art'
            AND [is_nullable] = 0
    )
        ALTER TABLE [dbo].[tbl_images] ALTER COLUMN [image_art] int NULL;

    IF EXISTS (
        SELECT 1 FROM sys.columns
        WHERE [object_id] = OBJECT_ID(N'[dbo].[tbl_images]')
            AND [name] = N'image_update'
            AND [is_nullable] = 0
    )
        ALTER TABLE [dbo].[tbl_images] ALTER COLUMN [image_update] datetime NULL;

    COMMIT TRANSACTION;
    PRINT N'Phase 1 committed: nullable menu/image fields.';

    BEGIN TRANSACTION;
    IF EXISTS (
        SELECT 1 FROM sys.columns
        WHERE [object_id] = OBJECT_ID(N'[dbo].[tbl_PageCounter]')
            AND [name] = N'PageCounter_Name'
            AND [is_nullable] = 1
    )
        ALTER TABLE [dbo].[tbl_PageCounter] ALTER COLUMN [PageCounter_Name] nvarchar(50) NOT NULL;
    COMMIT TRANSACTION;
    PRINT N'Phase 2 committed: PageCounter_Name.';

    BEGIN TRANSACTION;
    IF EXISTS (
        SELECT 1 FROM sys.columns
        WHERE [object_id] = OBJECT_ID(N'[dbo].[tbl_PageCounter]')
            AND [name] = N'PageCounter_MonthViewed'
            AND [is_nullable] = 1
    )
        ALTER TABLE [dbo].[tbl_PageCounter] ALTER COLUMN [PageCounter_MonthViewed] nvarchar(20) NOT NULL;
    COMMIT TRANSACTION;
    PRINT N'Phase 3 committed: PageCounter_MonthViewed.';

    /* Add English values and translation review state. */
    BEGIN TRANSACTION;
    IF COL_LENGTH(N'dbo.tbl_menu', N'menu_text_en') IS NULL
        ALTER TABLE [dbo].[tbl_menu] ADD [menu_text_en] nvarchar(50) NULL;
    IF COL_LENGTH(N'dbo.tbl_menu', N'menu_URLtext_en') IS NULL
        ALTER TABLE [dbo].[tbl_menu] ADD [menu_URLtext_en] nvarchar(50) NULL;
    IF COL_LENGTH(N'dbo.tbl_images', N'image_description_en') IS NULL
        ALTER TABLE [dbo].[tbl_images] ADD [image_description_en] nvarchar(150) NULL;
    IF COL_LENGTH(N'dbo.tbl_gb', N'GB_name_en') IS NULL
        ALTER TABLE [dbo].[tbl_gb] ADD [GB_name_en] nvarchar(100) NULL;
    IF COL_LENGTH(N'dbo.tbl_gb', N'GB_text_en') IS NULL
        ALTER TABLE [dbo].[tbl_gb] ADD [GB_text_en] nvarchar(max) NULL;
    COMMIT TRANSACTION;
    PRINT N'Phase 4 committed: English text columns.';

    BEGIN TRANSACTION;
    IF COL_LENGTH(N'dbo.tbl_menu', N'menu_text_en_reviewed') IS NULL
        ALTER TABLE [dbo].[tbl_menu] ADD [menu_text_en_reviewed] bit NOT NULL CONSTRAINT [DF_Migration_menu_text_en_reviewed] DEFAULT (0);
    IF COL_LENGTH(N'dbo.tbl_menu', N'menu_URLtext_en_reviewed') IS NULL
        ALTER TABLE [dbo].[tbl_menu] ADD [menu_URLtext_en_reviewed] bit NOT NULL CONSTRAINT [DF_Migration_menu_URLtext_en_reviewed] DEFAULT (0);
    IF COL_LENGTH(N'dbo.tbl_images', N'image_description_en_reviewed') IS NULL
        ALTER TABLE [dbo].[tbl_images] ADD [image_description_en_reviewed] bit NOT NULL CONSTRAINT [DF_Migration_image_description_en_reviewed] DEFAULT (0);
    IF COL_LENGTH(N'dbo.tbl_gb', N'GB_name_en_reviewed') IS NULL
        ALTER TABLE [dbo].[tbl_gb] ADD [GB_name_en_reviewed] bit NOT NULL CONSTRAINT [DF_Migration_GB_name_en_reviewed] DEFAULT (0);
    IF COL_LENGTH(N'dbo.tbl_gb', N'GB_text_en_reviewed') IS NULL
        ALTER TABLE [dbo].[tbl_gb] ADD [GB_text_en_reviewed] bit NOT NULL CONSTRAINT [DF_Migration_GB_text_en_reviewed] DEFAULT (0);

    IF NOT EXISTS (
        SELECT 1 FROM sys.default_constraints AS dc
        INNER JOIN sys.columns AS c
            ON c.[object_id] = dc.[parent_object_id]
            AND c.[column_id] = dc.[parent_column_id]
        WHERE dc.[parent_object_id] = OBJECT_ID(N'[dbo].[tbl_menu]')
            AND c.[name] = N'menu_text_en_reviewed'
    )
        ALTER TABLE [dbo].[tbl_menu] ADD CONSTRAINT [DF_Migration_menu_text_en_reviewed] DEFAULT (0) FOR [menu_text_en_reviewed];
    IF NOT EXISTS (
        SELECT 1 FROM sys.default_constraints AS dc
        INNER JOIN sys.columns AS c
            ON c.[object_id] = dc.[parent_object_id]
            AND c.[column_id] = dc.[parent_column_id]
        WHERE dc.[parent_object_id] = OBJECT_ID(N'[dbo].[tbl_menu]')
            AND c.[name] = N'menu_URLtext_en_reviewed'
    )
        ALTER TABLE [dbo].[tbl_menu] ADD CONSTRAINT [DF_Migration_menu_URLtext_en_reviewed] DEFAULT (0) FOR [menu_URLtext_en_reviewed];
    IF NOT EXISTS (
        SELECT 1 FROM sys.default_constraints AS dc
        INNER JOIN sys.columns AS c
            ON c.[object_id] = dc.[parent_object_id]
            AND c.[column_id] = dc.[parent_column_id]
        WHERE dc.[parent_object_id] = OBJECT_ID(N'[dbo].[tbl_images]')
            AND c.[name] = N'image_description_en_reviewed'
    )
        ALTER TABLE [dbo].[tbl_images] ADD CONSTRAINT [DF_Migration_image_description_en_reviewed] DEFAULT (0) FOR [image_description_en_reviewed];
    IF NOT EXISTS (
        SELECT 1 FROM sys.default_constraints AS dc
        INNER JOIN sys.columns AS c
            ON c.[object_id] = dc.[parent_object_id]
            AND c.[column_id] = dc.[parent_column_id]
        WHERE dc.[parent_object_id] = OBJECT_ID(N'[dbo].[tbl_gb]')
            AND c.[name] = N'GB_name_en_reviewed'
    )
        ALTER TABLE [dbo].[tbl_gb] ADD CONSTRAINT [DF_Migration_GB_name_en_reviewed] DEFAULT (0) FOR [GB_name_en_reviewed];
    IF NOT EXISTS (
        SELECT 1 FROM sys.default_constraints AS dc
        INNER JOIN sys.columns AS c
            ON c.[object_id] = dc.[parent_object_id]
            AND c.[column_id] = dc.[parent_column_id]
        WHERE dc.[parent_object_id] = OBJECT_ID(N'[dbo].[tbl_gb]')
            AND c.[name] = N'GB_text_en_reviewed'
    )
        ALTER TABLE [dbo].[tbl_gb] ADD CONSTRAINT [DF_Migration_GB_text_en_reviewed] DEFAULT (0) FOR [GB_text_en_reviewed];
    COMMIT TRANSACTION;
    PRINT N'Phase 5 committed: translation review flags.';

    /* Add game selection switches, default existing rows to selectable. */
    BEGIN TRANSACTION;
    IF COL_LENGTH(N'dbo.tbl_menu', N'is_game_selectable') IS NULL
        ALTER TABLE [dbo].[tbl_menu] ADD [is_game_selectable] bit NOT NULL CONSTRAINT [DF_Migration_tbl_menu_is_game_selectable] DEFAULT (1);
    IF COL_LENGTH(N'dbo.tbl_images', N'is_game_selectable') IS NULL
        ALTER TABLE [dbo].[tbl_images] ADD [is_game_selectable] bit NOT NULL CONSTRAINT [DF_Migration_tbl_images_is_game_selectable] DEFAULT (1);

    IF NOT EXISTS (
        SELECT 1 FROM sys.default_constraints AS dc
        INNER JOIN sys.columns AS c
            ON c.[object_id] = dc.[parent_object_id]
            AND c.[column_id] = dc.[parent_column_id]
        WHERE dc.[parent_object_id] = OBJECT_ID(N'[dbo].[tbl_menu]')
            AND c.[name] = N'is_game_selectable'
    )
        ALTER TABLE [dbo].[tbl_menu] ADD CONSTRAINT [DF_Migration_tbl_menu_is_game_selectable] DEFAULT (1) FOR [is_game_selectable];
    IF NOT EXISTS (
        SELECT 1 FROM sys.default_constraints AS dc
        INNER JOIN sys.columns AS c
            ON c.[object_id] = dc.[parent_object_id]
            AND c.[column_id] = dc.[parent_column_id]
        WHERE dc.[parent_object_id] = OBJECT_ID(N'[dbo].[tbl_images]')
            AND c.[name] = N'is_game_selectable'
    )
        ALTER TABLE [dbo].[tbl_images] ADD CONSTRAINT [DF_Migration_tbl_images_is_game_selectable] DEFAULT (1) FOR [is_game_selectable];
    COMMIT TRANSACTION;
    PRINT N'Phase 6 committed: game selection flags.';

    BEGIN TRANSACTION;
    IF COL_LENGTH(N'dbo.tbl_menu', N'menu_dateUpdated') IS NULL
        ALTER TABLE [dbo].[tbl_menu] ADD [menu_dateUpdated] datetime NULL;

    /* Ensure contact submission indexes/defaults without touching submitted rows. */
    IF NOT EXISTS (
        SELECT 1 FROM sys.indexes
        WHERE [object_id] = OBJECT_ID(N'[dbo].[tbl_kontakt]')
            AND [name] = N'IX_tbl_kontakt_EmailSent'
    )
        CREATE INDEX [IX_tbl_kontakt_EmailSent]
            ON [dbo].[tbl_kontakt] ([EmailSent])
            INCLUDE ([SubmitDate], [Name], [Email]);

    IF NOT EXISTS (
        SELECT 1 FROM sys.indexes
        WHERE [object_id] = OBJECT_ID(N'[dbo].[tbl_kontakt]')
            AND [name] = N'IX_tbl_kontakt_SubmitDate'
    )
        CREATE INDEX [IX_tbl_kontakt_SubmitDate]
            ON [dbo].[tbl_kontakt] ([SubmitDate] DESC)
            INCLUDE ([EmailSent], [SourcePage]);

    IF NOT EXISTS (
        SELECT 1
        FROM sys.default_constraints AS dc
        INNER JOIN sys.columns AS c
            ON c.[object_id] = dc.[parent_object_id]
            AND c.[column_id] = dc.[parent_column_id]
        WHERE dc.[parent_object_id] = OBJECT_ID(N'[dbo].[tbl_kontakt]')
            AND c.[name] = N'SubmitDate'
    )
        ALTER TABLE [dbo].[tbl_kontakt]
            ADD CONSTRAINT [DF_tbl_kontakt_SubmitDate] DEFAULT (GETDATE()) FOR [SubmitDate];

    IF NOT EXISTS (
        SELECT 1
        FROM sys.default_constraints AS dc
        INNER JOIN sys.columns AS c
            ON c.[object_id] = dc.[parent_object_id]
            AND c.[column_id] = dc.[parent_column_id]
        WHERE dc.[parent_object_id] = OBJECT_ID(N'[dbo].[tbl_kontakt]')
            AND c.[name] = N'EmailSent'
    )
        ALTER TABLE [dbo].[tbl_kontakt]
            ADD CONSTRAINT [DF_tbl_kontakt_EmailSent] DEFAULT ((0)) FOR [EmailSent];
    COMMIT TRANSACTION;
    PRINT N'Phase 7 committed: menu date and contact indexes/defaults.';

    /* Restore ASP.NET Identity indexes omitted from the database export. */
    BEGIN TRANSACTION;
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [object_id] = OBJECT_ID(N'[dbo].[AspNetUsers]') AND [name] = N'EmailIndex')
        CREATE INDEX [EmailIndex] ON [dbo].[AspNetUsers] ([NormalizedEmail]);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [object_id] = OBJECT_ID(N'[dbo].[AspNetUsers]') AND [name] = N'UserNameIndex')
        CREATE UNIQUE INDEX [UserNameIndex] ON [dbo].[AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [object_id] = OBJECT_ID(N'[dbo].[AspNetRoles]') AND [name] = N'RoleNameIndex')
        CREATE UNIQUE INDEX [RoleNameIndex] ON [dbo].[AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [object_id] = OBJECT_ID(N'[dbo].[AspNetRoleClaims]') AND [name] = N'IX_AspNetRoleClaims_RoleId')
        CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [dbo].[AspNetRoleClaims] ([RoleId]);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [object_id] = OBJECT_ID(N'[dbo].[AspNetUserClaims]') AND [name] = N'IX_AspNetUserClaims_UserId')
        CREATE INDEX [IX_AspNetUserClaims_UserId] ON [dbo].[AspNetUserClaims] ([UserId]);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [object_id] = OBJECT_ID(N'[dbo].[AspNetUserLogins]') AND [name] = N'IX_AspNetUserLogins_UserId')
        CREATE INDEX [IX_AspNetUserLogins_UserId] ON [dbo].[AspNetUserLogins] ([UserId]);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [object_id] = OBJECT_ID(N'[dbo].[AspNetUserRoles]') AND [name] = N'IX_AspNetUserRoles_RoleId')
        CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles] ([RoleId]);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [object_id] = OBJECT_ID(N'[dbo].[AspNetUserPasskeys]') AND [name] = N'IX_AspNetUserPasskeys_UserId')
        CREATE INDEX [IX_AspNetUserPasskeys_UserId] ON [dbo].[AspNetUserPasskeys] ([UserId]);
    COMMIT TRANSACTION;
    PRINT N'Phase 8 committed: ASP.NET Identity indexes.';

    /* Mark categories excluded by the existing game-selection policy. */
    BEGIN TRANSACTION;
    IF NOT EXISTS (
        SELECT 1 FROM [dbo].[__EFMigrationsHistory]
        WHERE [MigrationId] = N'20261018210222_AddGameSelectableFlags'
    )
    BEGIN
        DECLARE @updateGameFlagsSql nvarchar(max) = N'
            DECLARE @excludedCategories TABLE (menu_ID int PRIMARY KEY);

            ;WITH excludedCategories AS (
                SELECT menu_ID
                FROM [dbo].[tbl_menu]
                WHERE COALESCE(menu_mainID, 0) = 0
                    AND (
                        menu_text IN (N''Årstider'', N''Resor'', N''Landskap'')
                        OR menu_text_en IN (N''Seasons'', N''Travel'', N''Landscapes'')
                        OR menu_URLtext IN (N''Arstider'', N''Resor'', N''Landskap'')
                        OR menu_URLtext_en IN (N''seasons'', N''travel'', N''landscapes'')
                    )

                UNION ALL

                SELECT child.menu_ID
                FROM [dbo].[tbl_menu] AS child
                INNER JOIN excludedCategories AS parent
                    ON child.menu_mainID = parent.menu_ID
            )
            INSERT INTO @excludedCategories (menu_ID)
            SELECT menu_ID
            FROM excludedCategories
            OPTION (MAXRECURSION 32767);

            UPDATE [dbo].[tbl_menu]
            SET [is_game_selectable] = 0
            WHERE [menu_ID] IN (SELECT menu_ID FROM @excludedCategories);

            UPDATE [dbo].[tbl_images]
            SET [is_game_selectable] = 0
            WHERE [image_art] IN (SELECT menu_ID FROM @excludedCategories)
                OR [image_familj] IN (SELECT menu_ID FROM @excludedCategories)
                OR [image_huvudfamilj] IN (SELECT menu_ID FROM @excludedCategories);';

        EXEC sys.sp_executesql @updateGameFlagsSql;
    END;
    COMMIT TRANSACTION;
    PRINT N'Phase 9 committed: game-selectability defaults.';

    /*
       The old EF migration 20250714143852 deletes seed IDs 1-20. That deletion
       is intentionally not replayed against this data-bearing database copy.
       Existing rows stand in for the old seed state; only schema is baselined.
    */
    BEGIN TRANSACTION;
    DECLARE @currentProductVersion nvarchar(32) = N'11.0.0-rc.2.26504.105';
    DECLARE @currentMigrations TABLE ([MigrationId] nvarchar(150) PRIMARY KEY);

    INSERT INTO @currentMigrations ([MigrationId])
    VALUES
        (N'20210208214305_SkapatTabeller'),
        (N'20211122184803_DatabaseSeed'),
        (N'20250714143852_UpdateDatabaseSchema'),
        (N'20260926105909_AddEnglishColumns'),
        (N'20261017162636_AddTranslationReviewStatus'),
        (N'20261018210222_AddGameSelectableFlags'),
        (N'20261020222500_CreateTblKontaktIfMissing'),
        (N'20261020222600_AddMenuDateUpdatedIfMissing'),
        (N'20210203230745_SkaparIdentittTabeller'),
        (N'20210311155651_LagtTillShowAllLogs'),
        (N'20250315234213_AddPasskeySupport');

    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    SELECT m.[MigrationId], @currentProductVersion
    FROM @currentMigrations AS m
    WHERE NOT EXISTS (
        SELECT 1
        FROM [dbo].[__EFMigrationsHistory] AS h
        WHERE h.[MigrationId] = m.[MigrationId]
    );

    COMMIT TRANSACTION;
    PRINT N'Schema upgrade completed. Existing application data was retained.';
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
