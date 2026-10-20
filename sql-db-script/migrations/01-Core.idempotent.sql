IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20210208214305_SkapatTabeller'
)
BEGIN
    CREATE TABLE [tbl_gb] (
        [ID] int NOT NULL IDENTITY,
        [GB_ID] int NOT NULL,
        [GB_name] nvarchar(100) NULL,
        [GB_email] nvarchar(100) NULL,
        [GB_homepage] nvarchar(255) NULL,
        [GB_text] nvarchar(max) NULL,
        [GB_date] smalldatetime NULL,
        [GB_ReadPost] bit NULL,
        [GB_IP] nvarchar(50) NULL,
        CONSTRAINT [PK_tbl_gb] PRIMARY KEY ([ID])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20210208214305_SkapatTabeller'
)
BEGIN
    CREATE TABLE [tbl_images] (
        [ID] int NOT NULL IDENTITY,
        [image_ID] int NOT NULL,
        [image_huvudfamilj] int NULL,
        [image_familj] int NULL,
        [image_art] int NOT NULL,
        [image_URL] nvarchar(50) NULL,
        [image_date] smalldatetime NULL,
        [image_description] nvarchar(150) NULL,
        [image_update] datetime NOT NULL,
        CONSTRAINT [PK_tbl_images] PRIMARY KEY ([ID])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20210208214305_SkapatTabeller'
)
BEGIN
    CREATE TABLE [tbl_menu] (
        [ID] int NOT NULL IDENTITY,
        [menu_ID] int NOT NULL,
        [menu_mainID] int NULL,
        [menu_text] nvarchar(50) NULL,
        [menu_URLtext] nvarchar(50) NULL,
        [menu_lastshowdate] datetime NULL,
        [menu_pagecounter] int NULL,
        CONSTRAINT [PK_tbl_menu] PRIMARY KEY ([ID])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20210208214305_SkapatTabeller'
)
BEGIN
    CREATE TABLE [tbl_PageCounter] (
        [PageCounter_ID] int NOT NULL IDENTITY,
        [PageCounter_Views] int NOT NULL,
        [PageCounter_Name] nvarchar(50) NULL,
        [PageCounter_MonthViewed] nvarchar(20) NULL,
        [PageCounter_LastShowDate] datetime NOT NULL,
        CONSTRAINT [PK_tbl_PageCounter] PRIMARY KEY ([PageCounter_ID])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20210208214305_SkapatTabeller'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210208214305_SkapatTabeller', N'11.0.0-rc.2.26504.105');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20211122184803_DatabaseSeed'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME(OBJECT_NAME([c].[default_object_id]))
    FROM [sys].[columns] [c]
    WHERE [c].[object_id] = OBJECT_ID(N'[tbl_menu]') AND [c].[name] = N'menu_lastshowdate';
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [tbl_menu] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [tbl_menu] DROP COLUMN [menu_lastshowdate];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20211122184803_DatabaseSeed'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME(OBJECT_NAME([c].[default_object_id]))
    FROM [sys].[columns] [c]
    WHERE [c].[object_id] = OBJECT_ID(N'[tbl_menu]') AND [c].[name] = N'menu_pagecounter';
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [tbl_menu] DROP CONSTRAINT ' + @var1 + ';');
    ALTER TABLE [tbl_menu] DROP COLUMN [menu_pagecounter];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20211122184803_DatabaseSeed'
)
BEGIN
    ALTER TABLE [tbl_PageCounter] ADD [PageCounter_CategoryId] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20211122184803_DatabaseSeed'
)
BEGIN
    ALTER TABLE [tbl_PageCounter] ADD [PageCounter_PicturePage] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20211122184803_DatabaseSeed'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PageCounter_ID', N'PageCounter_CategoryId', N'PageCounter_LastShowDate', N'PageCounter_MonthViewed', N'PageCounter_Name', N'PageCounter_Views', N'PageCounter_PicturePage') AND [object_id] = OBJECT_ID(N'[tbl_PageCounter]'))
        SET IDENTITY_INSERT [tbl_PageCounter] ON;
    EXEC(N'INSERT INTO [tbl_PageCounter] ([PageCounter_ID], [PageCounter_CategoryId], [PageCounter_LastShowDate], [PageCounter_MonthViewed], [PageCounter_Name], [PageCounter_Views], [PageCounter_PicturePage])
    VALUES (1, 1, ''2021-11-22T16:16:00.000'', N''2021-11'', N''Fåglar'', 0, CAST(1 AS bit))');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PageCounter_ID', N'PageCounter_CategoryId', N'PageCounter_LastShowDate', N'PageCounter_MonthViewed', N'PageCounter_Name', N'PageCounter_Views', N'PageCounter_PicturePage') AND [object_id] = OBJECT_ID(N'[tbl_PageCounter]'))
        SET IDENTITY_INSERT [tbl_PageCounter] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20211122184803_DatabaseSeed'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'GB_date', N'GB_email', N'GB_homepage', N'GB_ID', N'GB_IP', N'GB_name', N'GB_ReadPost', N'GB_text') AND [object_id] = OBJECT_ID(N'[tbl_gb]'))
        SET IDENTITY_INSERT [tbl_gb] ON;
    EXEC(N'INSERT INTO [tbl_gb] ([ID], [GB_date], [GB_email], [GB_homepage], [GB_ID], [GB_IP], [GB_name], [GB_ReadPost], [GB_text])
    VALUES (1, ''2021-11-22T00:00:00'', N''pownas@outlook.com'', N''github.com/pownas'', 1, NULL, N''pownas'', CAST(0 AS bit), N''Ett första test inlägg i databasen...'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'GB_date', N'GB_email', N'GB_homepage', N'GB_ID', N'GB_IP', N'GB_name', N'GB_ReadPost', N'GB_text') AND [object_id] = OBJECT_ID(N'[tbl_gb]'))
        SET IDENTITY_INSERT [tbl_gb] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20211122184803_DatabaseSeed'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'image_art', N'image_date', N'image_description', N'image_familj', N'image_huvudfamilj', N'image_ID', N'image_update', N'image_URL') AND [object_id] = OBJECT_ID(N'[tbl_images]'))
        SET IDENTITY_INSERT [tbl_images] ON;
    EXEC(N'INSERT INTO [tbl_images] ([ID], [image_art], [image_date], [image_description], [image_familj], [image_huvudfamilj], [image_ID], [image_update], [image_URL])
    VALUES (1, 11, ''2021-11-22T16:21:00'', N''En fjällabbs beskrivning...'', 9, NULL, 1, ''2021-11-22T16:21:00.000'', N''08TA3696''),
    (2, 13, ''2021-11-22T16:21:00'', N''Hane, beskrivning av blåmes....'', 12, 10, 2, ''2021-11-22T16:21:00.000'', N''B57W4725''),
    (3, 14, ''2021-11-22T16:21:00'', N''i Sverige'', 2, NULL, 3, ''2021-11-22T16:21:00.000'', N''B59W4837''),
    (4, 15, ''2021-11-22T16:21:00'', N'''', 3, NULL, 4, ''2021-11-22T16:21:00.000'', N''13TA5142''),
    (5, 16, ''2021-11-22T16:21:00'', N''Ekoxe, hane'', 4, NULL, 5, ''2021-11-22T16:21:00.000'', N''B60W1277''),
    (6, 17, ''2021-11-22T16:21:00'', N''Fjällviol'', 5, NULL, 6, ''2021-11-22T16:21:00.000'', N''09TA8491''),
    (7, 18, ''2021-11-22T16:21:00'', N''Rauk'', 6, NULL, 7, ''2021-11-22T16:21:00.000'', N''B57W8697''),
    (8, 19, ''2021-11-22T16:21:00'', N''Jordgubbar'', 7, NULL, 8, ''2021-11-22T16:21:00.000'', N''IMG_1496''),
    (9, 20, ''2021-11-22T16:21:00'', N''Hane, beskrivning av Costa rica....'', 8, NULL, 9, ''2021-11-22T16:21:00.000'', N''_N0Q8690'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'image_art', N'image_date', N'image_description', N'image_familj', N'image_huvudfamilj', N'image_ID', N'image_update', N'image_URL') AND [object_id] = OBJECT_ID(N'[tbl_images]'))
        SET IDENTITY_INSERT [tbl_images] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20211122184803_DatabaseSeed'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'menu_ID', N'menu_mainID', N'menu_text', N'menu_URLtext') AND [object_id] = OBJECT_ID(N'[tbl_menu]'))
        SET IDENTITY_INSERT [tbl_menu] ON;
    EXEC(N'INSERT INTO [tbl_menu] ([ID], [menu_ID], [menu_mainID], [menu_text], [menu_URLtext])
    VALUES (1, 1, 0, N''Fåglar'', N''Faglar''),
    (2, 2, 0, N''Däggdjur'', N''Daggdjur''),
    (3, 3, 0, N''Kräldjur'', N''Kraldjur''),
    (4, 4, 0, N''Insekter'', N''Insekter''),
    (5, 5, 0, N''Växter'', N''Vaxter''),
    (6, 6, 0, N''Landskap'', N''Landskap''),
    (7, 7, 0, N''Årstider'', N''Arstider''),
    (8, 8, 0, N''Resor'', N''Resor''),
    (9, 9, 1, N''Alkor och labbar'', N''Alkor-och-labbar''),
    (10, 10, 1, N''Tättingar'', N''Tattingar''),
    (11, 11, 9, N''Fjällabb'', N''Fjallabb''),
    (12, 12, 10, N''Mesar'', N''Mesar''),
    (13, 13, 12, N''Blåmes'', N''Blames''),
    (14, 14, 2, N''Björn'', N''Bjorn''),
    (15, 15, 3, N''Hasselsnok'', N''Hasselsnok''),
    (16, 16, 4, N''Skalbaggar'', N''Skalbaggar''),
    (17, 17, 5, N''Blommor'', N''Blommor''),
    (18, 18, 6, N''Gotland'', N''Gotland''),
    (19, 19, 7, N''Sommar'', N''Sommar''),
    (20, 20, 8, N''2008 Costa Rica'', N''2008-Costa-Rica'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'menu_ID', N'menu_mainID', N'menu_text', N'menu_URLtext') AND [object_id] = OBJECT_ID(N'[tbl_menu]'))
        SET IDENTITY_INSERT [tbl_menu] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20211122184803_DatabaseSeed'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20211122184803_DatabaseSeed', N'11.0.0-rc.2.26504.105');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_PageCounter]
    WHERE [PageCounter_ID] = 1;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_gb]
    WHERE [ID] = 1;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_images]
    WHERE [ID] = 1;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_images]
    WHERE [ID] = 2;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_images]
    WHERE [ID] = 3;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_images]
    WHERE [ID] = 4;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_images]
    WHERE [ID] = 5;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_images]
    WHERE [ID] = 6;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_images]
    WHERE [ID] = 7;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_images]
    WHERE [ID] = 8;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_images]
    WHERE [ID] = 9;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_menu]
    WHERE [ID] = 1;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_menu]
    WHERE [ID] = 2;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_menu]
    WHERE [ID] = 3;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_menu]
    WHERE [ID] = 4;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_menu]
    WHERE [ID] = 5;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_menu]
    WHERE [ID] = 6;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_menu]
    WHERE [ID] = 7;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_menu]
    WHERE [ID] = 8;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_menu]
    WHERE [ID] = 9;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_menu]
    WHERE [ID] = 10;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_menu]
    WHERE [ID] = 11;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_menu]
    WHERE [ID] = 12;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_menu]
    WHERE [ID] = 13;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_menu]
    WHERE [ID] = 14;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_menu]
    WHERE [ID] = 15;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_menu]
    WHERE [ID] = 16;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_menu]
    WHERE [ID] = 17;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_menu]
    WHERE [ID] = 18;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_menu]
    WHERE [ID] = 19;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    EXEC(N'DELETE FROM [tbl_menu]
    WHERE [ID] = 20;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    DECLARE @var2 nvarchar(max);
    SELECT @var2 = QUOTENAME(OBJECT_NAME([c].[default_object_id]))
    FROM [sys].[columns] [c]
    WHERE [c].[object_id] = OBJECT_ID(N'[tbl_PageCounter]') AND [c].[name] = N'PageCounter_Name';
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [tbl_PageCounter] DROP CONSTRAINT ' + @var2 + ';');
    EXEC(N'UPDATE [tbl_PageCounter] SET [PageCounter_Name] = N'''' WHERE [PageCounter_Name] IS NULL');
    ALTER TABLE [tbl_PageCounter] ALTER COLUMN [PageCounter_Name] nvarchar(50) NOT NULL;
    ALTER TABLE [tbl_PageCounter] ADD DEFAULT N'' FOR [PageCounter_Name];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    DECLARE @var3 nvarchar(max);
    SELECT @var3 = QUOTENAME(OBJECT_NAME([c].[default_object_id]))
    FROM [sys].[columns] [c]
    WHERE [c].[object_id] = OBJECT_ID(N'[tbl_PageCounter]') AND [c].[name] = N'PageCounter_MonthViewed';
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [tbl_PageCounter] DROP CONSTRAINT ' + @var3 + ';');
    EXEC(N'UPDATE [tbl_PageCounter] SET [PageCounter_MonthViewed] = N'''' WHERE [PageCounter_MonthViewed] IS NULL');
    ALTER TABLE [tbl_PageCounter] ALTER COLUMN [PageCounter_MonthViewed] nvarchar(20) NOT NULL;
    ALTER TABLE [tbl_PageCounter] ADD DEFAULT N'' FOR [PageCounter_MonthViewed];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    DECLARE @var4 nvarchar(max);
    SELECT @var4 = QUOTENAME(OBJECT_NAME([c].[default_object_id]))
    FROM [sys].[columns] [c]
    WHERE [c].[object_id] = OBJECT_ID(N'[tbl_menu]') AND [c].[name] = N'menu_ID';
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [tbl_menu] DROP CONSTRAINT ' + @var4 + ';');
    ALTER TABLE [tbl_menu] ALTER COLUMN [menu_ID] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    DECLARE @var5 nvarchar(max);
    SELECT @var5 = QUOTENAME(OBJECT_NAME([c].[default_object_id]))
    FROM [sys].[columns] [c]
    WHERE [c].[object_id] = OBJECT_ID(N'[tbl_images]') AND [c].[name] = N'image_update';
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [tbl_images] DROP CONSTRAINT ' + @var5 + ';');
    ALTER TABLE [tbl_images] ALTER COLUMN [image_update] datetime NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    DECLARE @var6 nvarchar(max);
    SELECT @var6 = QUOTENAME(OBJECT_NAME([c].[default_object_id]))
    FROM [sys].[columns] [c]
    WHERE [c].[object_id] = OBJECT_ID(N'[tbl_images]') AND [c].[name] = N'image_art';
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [tbl_images] DROP CONSTRAINT ' + @var6 + ';');
    ALTER TABLE [tbl_images] ALTER COLUMN [image_art] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    DECLARE @var7 nvarchar(max);
    SELECT @var7 = QUOTENAME(OBJECT_NAME([c].[default_object_id]))
    FROM [sys].[columns] [c]
    WHERE [c].[object_id] = OBJECT_ID(N'[tbl_images]') AND [c].[name] = N'image_ID';
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [tbl_images] DROP CONSTRAINT ' + @var7 + ';');
    ALTER TABLE [tbl_images] ALTER COLUMN [image_ID] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    DECLARE @var8 nvarchar(max);
    SELECT @var8 = QUOTENAME(OBJECT_NAME([c].[default_object_id]))
    FROM [sys].[columns] [c]
    WHERE [c].[object_id] = OBJECT_ID(N'[tbl_gb]') AND [c].[name] = N'GB_text';
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [tbl_gb] DROP CONSTRAINT ' + @var8 + ';');
    EXEC(N'UPDATE [tbl_gb] SET [GB_text] = N'''' WHERE [GB_text] IS NULL');
    ALTER TABLE [tbl_gb] ALTER COLUMN [GB_text] nvarchar(max) NOT NULL;
    ALTER TABLE [tbl_gb] ADD DEFAULT N'' FOR [GB_text];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    DECLARE @var9 nvarchar(max);
    SELECT @var9 = QUOTENAME(OBJECT_NAME([c].[default_object_id]))
    FROM [sys].[columns] [c]
    WHERE [c].[object_id] = OBJECT_ID(N'[tbl_gb]') AND [c].[name] = N'GB_name';
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [tbl_gb] DROP CONSTRAINT ' + @var9 + ';');
    EXEC(N'UPDATE [tbl_gb] SET [GB_name] = N'''' WHERE [GB_name] IS NULL');
    ALTER TABLE [tbl_gb] ALTER COLUMN [GB_name] nvarchar(100) NOT NULL;
    ALTER TABLE [tbl_gb] ADD DEFAULT N'' FOR [GB_name];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    DECLARE @var10 nvarchar(max);
    SELECT @var10 = QUOTENAME(OBJECT_NAME([c].[default_object_id]))
    FROM [sys].[columns] [c]
    WHERE [c].[object_id] = OBJECT_ID(N'[tbl_gb]') AND [c].[name] = N'GB_homepage';
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [tbl_gb] DROP CONSTRAINT ' + @var10 + ';');
    EXEC(N'UPDATE [tbl_gb] SET [GB_homepage] = N'''' WHERE [GB_homepage] IS NULL');
    ALTER TABLE [tbl_gb] ALTER COLUMN [GB_homepage] nvarchar(255) NOT NULL;
    ALTER TABLE [tbl_gb] ADD DEFAULT N'' FOR [GB_homepage];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    DECLARE @var11 nvarchar(max);
    SELECT @var11 = QUOTENAME(OBJECT_NAME([c].[default_object_id]))
    FROM [sys].[columns] [c]
    WHERE [c].[object_id] = OBJECT_ID(N'[tbl_gb]') AND [c].[name] = N'GB_email';
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [tbl_gb] DROP CONSTRAINT ' + @var11 + ';');
    EXEC(N'UPDATE [tbl_gb] SET [GB_email] = N'''' WHERE [GB_email] IS NULL');
    ALTER TABLE [tbl_gb] ALTER COLUMN [GB_email] nvarchar(100) NOT NULL;
    ALTER TABLE [tbl_gb] ADD DEFAULT N'' FOR [GB_email];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    DECLARE @var12 nvarchar(max);
    SELECT @var12 = QUOTENAME(OBJECT_NAME([c].[default_object_id]))
    FROM [sys].[columns] [c]
    WHERE [c].[object_id] = OBJECT_ID(N'[tbl_gb]') AND [c].[name] = N'GB_IP';
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [tbl_gb] DROP CONSTRAINT ' + @var12 + ';');
    EXEC(N'UPDATE [tbl_gb] SET [GB_IP] = N'''' WHERE [GB_IP] IS NULL');
    ALTER TABLE [tbl_gb] ALTER COLUMN [GB_IP] nvarchar(50) NOT NULL;
    ALTER TABLE [tbl_gb] ADD DEFAULT N'' FOR [GB_IP];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250714143852_UpdateDatabaseSchema'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250714143852_UpdateDatabaseSchema', N'11.0.0-rc.2.26504.105');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260926105909_AddEnglishColumns'
)
BEGIN
    ALTER TABLE [tbl_menu] ADD [menu_URLtext_en] nvarchar(50) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260926105909_AddEnglishColumns'
)
BEGIN
    ALTER TABLE [tbl_menu] ADD [menu_text_en] nvarchar(50) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260926105909_AddEnglishColumns'
)
BEGIN
    ALTER TABLE [tbl_images] ADD [image_description_en] nvarchar(150) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260926105909_AddEnglishColumns'
)
BEGIN
    ALTER TABLE [tbl_gb] ADD [GB_name_en] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260926105909_AddEnglishColumns'
)
BEGIN
    ALTER TABLE [tbl_gb] ADD [GB_text_en] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260926105909_AddEnglishColumns'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260926105909_AddEnglishColumns', N'11.0.0-rc.2.26504.105');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017162636_AddTranslationReviewStatus'
)
BEGIN
    ALTER TABLE [tbl_menu] ADD [menu_URLtext_en_reviewed] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017162636_AddTranslationReviewStatus'
)
BEGIN
    ALTER TABLE [tbl_menu] ADD [menu_text_en_reviewed] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017162636_AddTranslationReviewStatus'
)
BEGIN
    ALTER TABLE [tbl_images] ADD [image_description_en_reviewed] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017162636_AddTranslationReviewStatus'
)
BEGIN
    ALTER TABLE [tbl_gb] ADD [GB_name_en_reviewed] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017162636_AddTranslationReviewStatus'
)
BEGIN
    ALTER TABLE [tbl_gb] ADD [GB_text_en_reviewed] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017162636_AddTranslationReviewStatus'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261017162636_AddTranslationReviewStatus', N'11.0.0-rc.2.26504.105');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018210222_AddGameSelectableFlags'
)
BEGIN
    ALTER TABLE [tbl_menu] ADD [is_game_selectable] bit NOT NULL DEFAULT CAST(1 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018210222_AddGameSelectableFlags'
)
BEGIN
    ALTER TABLE [tbl_images] ADD [is_game_selectable] bit NOT NULL DEFAULT CAST(1 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018210222_AddGameSelectableFlags'
)
BEGIN
    DECLARE @excludedCategories TABLE (menu_ID int PRIMARY KEY);

    WITH excludedCategories AS (
        SELECT menu_ID
        FROM tbl_menu
        WHERE COALESCE(menu_mainID, 0) = 0
            AND (
                menu_text IN (N'Årstider', N'Resor', N'Landskap')
                OR menu_text_en IN (N'Seasons', N'Travel', N'Landscapes')
                OR menu_URLtext IN (N'Arstider', N'Resor', N'Landskap')
                OR menu_URLtext_en IN (N'seasons', N'travel', N'landscapes')
            )

        UNION ALL

        SELECT child.menu_ID
        FROM tbl_menu AS child
        INNER JOIN excludedCategories AS parent
            ON child.menu_mainID = parent.menu_ID
    )
    INSERT INTO @excludedCategories (menu_ID)
    SELECT menu_ID
    FROM excludedCategories
    OPTION (MAXRECURSION 32767);

    UPDATE tbl_menu
    SET is_game_selectable = 0
    WHERE menu_ID IN (SELECT menu_ID FROM @excludedCategories);

    UPDATE tbl_images
    SET is_game_selectable = 0
    WHERE image_art IN (SELECT menu_ID FROM @excludedCategories)
        OR image_familj IN (SELECT menu_ID FROM @excludedCategories)
        OR image_huvudfamilj IN (SELECT menu_ID FROM @excludedCategories);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018210222_AddGameSelectableFlags'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261018210222_AddGameSelectableFlags', N'11.0.0-rc.2.26504.105');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020222500_CreateTblKontaktIfMissing'
)
BEGIN
    IF OBJECT_ID(N'[dbo].[tbl_kontakt]', N'U') IS NULL
    BEGIN
        CREATE TABLE [dbo].[tbl_kontakt] (
            [ID] int IDENTITY(1,1) NOT NULL,
            [SubmitDate] datetime NOT NULL,
            [Name] nvarchar(50) NOT NULL,
            [Email] nvarchar(150) NOT NULL,
            [Subject] nvarchar(50) NOT NULL,
            [Message] nvarchar(2000) NOT NULL,
            [SourcePage] nvarchar(50) NOT NULL,
            [EmailSent] bit NOT NULL,
            [ErrorMessage] nvarchar(500) NULL,
            CONSTRAINT [PK_tbl_kontakt] PRIMARY KEY CLUSTERED ([ID])
        );
    END;

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
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020222500_CreateTblKontaktIfMissing'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261020222500_CreateTblKontaktIfMissing', N'11.0.0-rc.2.26504.105');
END;

COMMIT;
GO

