SET NOCOUNT ON;
SET XACT_ABORT ON;

IF OBJECT_ID(N'dbo.tbl_menu', N'U') IS NULL
    THROW 51000, 'Required table dbo.tbl_menu was not found.', 1;

IF COL_LENGTH(N'dbo.tbl_menu', N'menu_URLtext') IS NULL
   OR COL_LENGTH(N'dbo.tbl_menu', N'menu_URLtext_en') IS NULL
    THROW 51001, 'Expected Swedish and English category URL slug columns were not found.', 1;

BEGIN TRY
    BEGIN TRANSACTION;

    UPDATE dbo.tbl_menu
    SET menu_URLtext = LOWER(menu_URLtext),
        menu_URLtext_en = LOWER(menu_URLtext_en)
    WHERE (menu_URLtext IS NOT NULL
           AND menu_URLtext COLLATE Latin1_General_100_BIN2 <> LOWER(menu_URLtext) COLLATE Latin1_General_100_BIN2)
       OR (menu_URLtext_en IS NOT NULL
           AND menu_URLtext_en COLLATE Latin1_General_100_BIN2 <> LOWER(menu_URLtext_en) COLLATE Latin1_General_100_BIN2);

    DECLARE @UpdatedRows int = @@ROWCOUNT;

    COMMIT TRANSACTION;

    PRINT CONCAT('Category slug normalization completed. Rows updated: ', @UpdatedRows, '.');
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
