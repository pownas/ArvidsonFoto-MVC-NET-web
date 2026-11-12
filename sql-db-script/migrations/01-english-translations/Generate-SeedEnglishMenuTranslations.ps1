$ErrorActionPreference = 'Stop'

$repositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$seederPath = Join-Path $repositoryRoot 'ArvidsonFoto\Core\Data\ArvidsonFotoCoreDbSeeder.cs'
$outputPath = Join-Path $PSScriptRoot '04-Seed-EnglishMenuTranslations.sql'
$source = [System.IO.File]::ReadAllText($seederPath)

$menuRowPattern = '(?m)^\s*new\(\)\s*\{\s*Id\s*=\s*\d+,\s*MenuCategoryId\s*='
$translationPattern = '(?m)^\s*new\(\)\s*\{\s*Id\s*=\s*\d+,\s*MenuCategoryId\s*=\s*(?<id>\d+),[^\r\n]*?MenuDisplayNameEn\s*=\s*"(?<name>(?:\\.|[^"\\])*)"[^\r\n]*?MenuUrlSegmentEn\s*=\s*"(?<url>(?:\\.|[^"\\])*)"'
$menuRowCount = [regex]::Matches($source, $menuRowPattern).Count
$translations = [regex]::Matches($source, $translationPattern)

if ($menuRowCount -eq 0 -or $translations.Count -ne $menuRowCount) {
    throw "Expected English name and URL translations on every menu seed row; found $($translations.Count) translations for $menuRowCount menu rows."
}

$records = foreach ($translation in $translations) {
    $id = [int]$translation.Groups['id'].Value
    $name = $translation.Groups['name'].Value.Replace('\"', '"').Replace('\\', '\')
    $url = $translation.Groups['url'].Value.Replace('\"', '"').Replace('\\', '\')

    if ($name.Length -gt 50 -or $url.Length -gt 50) {
        throw "English translation for menu_ID $id exceeds the database column limit."
    }

    [pscustomobject]@{
        Id = $id
        Name = $name
        Url = $url
    }
}

if (($records.Id | Sort-Object -Unique).Count -ne $records.Count) {
    throw 'Duplicate menu_ID values were found in the English menu seed data.'
}

$sqlRows = foreach ($record in $records) {
    $name = $record.Name.Replace("'", "''")
    $url = $record.Url.Replace("'", "''")
    "    ($($record.Id), N'$name', N'$url')"
}

$lines = @(
    '/*'
    'Generated from ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_MenuCategories.'
    'Imports only the production category names and URL segments; test guestbook and image seed text is intentionally excluded.'
    'Run after the English columns and review-status columns have been migrated.'
    'By default, existing non-empty translations are preserved. Set @OverwriteExisting to 1 to replace differing values.'
    'Changed translations have their corresponding English review status reset to 0.'
    '*/'
    'SET XACT_ABORT ON;'
    ''
    "IF OBJECT_ID(N'dbo.tbl_menu', N'U') IS NULL"
    "    THROW 51020, 'dbo.tbl_menu was not found.', 1;"
    "IF COL_LENGTH(N'dbo.tbl_menu', N'menu_text_en') IS NULL"
    "   OR COL_LENGTH(N'dbo.tbl_menu', N'menu_URLtext_en') IS NULL"
    "   OR COL_LENGTH(N'dbo.tbl_menu', N'menu_text_en_reviewed') IS NULL"
    "   OR COL_LENGTH(N'dbo.tbl_menu', N'menu_URLtext_en_reviewed') IS NULL"
    "    THROW 51021, 'Run the English-column and review-status migrations before importing these values.', 1;"
    ''
    'DECLARE @Translations TABLE ('
    '    [menu_ID] int NOT NULL PRIMARY KEY,'
    '    [menu_text_en] nvarchar(50) NOT NULL,'
    '    [menu_URLtext_en] nvarchar(50) NOT NULL'
    ');'
    ''
    'INSERT INTO @Translations ([menu_ID], [menu_text_en], [menu_URLtext_en])'
    'VALUES'
    ($sqlRows -join ",`r`n") + ';'
    ''
    'DECLARE @OverwriteExisting bit = 0;'
    'DECLARE @MatchedRows int;'
    'DECLARE @RowsToUpdate int;'
    ''
    'SELECT @MatchedRows = COUNT(*)'
    'FROM [dbo].[tbl_menu] AS m'
    'INNER JOIN @Translations AS t ON t.[menu_ID] = m.[menu_ID];'
    ''
    'IF @MatchedRows <> (SELECT COUNT(*) FROM @Translations)'
    "    THROW 51022, 'Not all seeded menu_ID values exist exactly once in dbo.tbl_menu. No values were imported.', 1;"
    ''
    'SELECT @RowsToUpdate = COUNT(*)'
    'FROM [dbo].[tbl_menu] AS m'
    'INNER JOIN @Translations AS t ON t.[menu_ID] = m.[menu_ID]'
    'WHERE (@OverwriteExisting = 1 AND (ISNULL(m.[menu_text_en], N'''') <> t.[menu_text_en] OR ISNULL(m.[menu_URLtext_en], N'''') <> t.[menu_URLtext_en]))'
    '   OR (@OverwriteExisting = 0 AND (NULLIF(m.[menu_text_en], N'''') IS NULL OR NULLIF(m.[menu_URLtext_en], N'''') IS NULL));'
    ''
    'PRINT CONCAT(N''Seeded category rows: '', @MatchedRows, N''; rows that will change: '', @RowsToUpdate, N''.'');'
    ''
    'BEGIN TRY'
    '    BEGIN TRANSACTION;'
    ''
    '    UPDATE m'
    '    SET [menu_text_en] = CASE WHEN @OverwriteExisting = 1 OR NULLIF(m.[menu_text_en], N'''') IS NULL THEN t.[menu_text_en] ELSE m.[menu_text_en] END,'
    '        [menu_URLtext_en] = CASE WHEN @OverwriteExisting = 1 OR NULLIF(m.[menu_URLtext_en], N'''') IS NULL THEN t.[menu_URLtext_en] ELSE m.[menu_URLtext_en] END,'
    '        [menu_text_en_reviewed] = CASE'
    '            WHEN (@OverwriteExisting = 1 AND ISNULL(m.[menu_text_en], N'''') <> t.[menu_text_en])'
    '              OR (@OverwriteExisting = 0 AND NULLIF(m.[menu_text_en], N'''') IS NULL) THEN 0'
    '            ELSE m.[menu_text_en_reviewed] END,'
    '        [menu_URLtext_en_reviewed] = CASE'
    '            WHEN (@OverwriteExisting = 1 AND ISNULL(m.[menu_URLtext_en], N'''') <> t.[menu_URLtext_en])'
    '              OR (@OverwriteExisting = 0 AND NULLIF(m.[menu_URLtext_en], N'''') IS NULL) THEN 0'
    '            ELSE m.[menu_URLtext_en_reviewed] END'
    '    FROM [dbo].[tbl_menu] AS m'
    '    INNER JOIN @Translations AS t ON t.[menu_ID] = m.[menu_ID]'
    '    WHERE (@OverwriteExisting = 1 AND (ISNULL(m.[menu_text_en], N'''') <> t.[menu_text_en] OR ISNULL(m.[menu_URLtext_en], N'''') <> t.[menu_URLtext_en]))'
    '       OR (@OverwriteExisting = 0 AND (NULLIF(m.[menu_text_en], N'''') IS NULL OR NULLIF(m.[menu_URLtext_en], N'''') IS NULL));'
    ''
    '    DECLARE @UpdatedRows int = @@ROWCOUNT;'
    '    COMMIT TRANSACTION;'
    '    SELECT @UpdatedRows AS [UpdatedCategoryRows];'
    'END TRY'
    'BEGIN CATCH'
    '    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;'
    '    THROW;'
    'END CATCH;'
)

$sql = $lines -join [Environment]::NewLine
[System.IO.File]::WriteAllText($outputPath, $sql + [Environment]::NewLine, [System.Text.UTF8Encoding]::new($false))
Write-Output "Generated $outputPath with $($records.Count) menu translations."