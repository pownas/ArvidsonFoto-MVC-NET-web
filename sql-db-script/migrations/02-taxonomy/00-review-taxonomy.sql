/*
    Read-only inventory for the restored database.
    Run this first and save/export every result set before editing the map
    in 01-update-species-categories.sql.
*/
SET NOCOUNT ON;

DECLARE @BiologicalRoots TABLE (MenuId int NOT NULL PRIMARY KEY);
INSERT INTO @BiologicalRoots (MenuId)
VALUES (1), (2), (3), (5), (6);

;WITH CategoryTree AS
(
    SELECT
        m.menu_ID,
        m.menu_mainID,
        m.menu_text,
        m.menu_URLtext,
        m.menu_text_en,
        m.menu_URLtext_en,
        m.menu_ID AS RootMenuId,
        0 AS Depth,
        CONVERT(nvarchar(2000), COALESCE(m.menu_text, N'')) AS CategoryPath
    FROM dbo.tbl_menu AS m
    INNER JOIN @BiologicalRoots AS r ON r.MenuId = m.menu_ID
    WHERE m.menu_mainID = 0

    UNION ALL

    SELECT
        child.menu_ID,
        child.menu_mainID,
        child.menu_text,
        child.menu_URLtext,
        child.menu_text_en,
        child.menu_URLtext_en,
        parent.RootMenuId,
        parent.Depth + 1,
        CONVERT(nvarchar(2000), parent.CategoryPath + N' > ' + COALESCE(child.menu_text, N''))
    FROM dbo.tbl_menu AS child
    INNER JOIN CategoryTree AS parent ON child.menu_mainID = parent.menu_ID
),
ImageCounts AS
(
    SELECT
        i.image_art AS MenuId,
        COUNT_BIG(*) AS ImageCount,
        SUM(CASE WHEN NULLIF(LTRIM(RTRIM(i.image_description)), N'') IS NULL THEN 1 ELSE 0 END) AS WithoutCaptionCount,
        COUNT(DISTINCT NULLIF(LTRIM(RTRIM(i.image_description)), N'')) AS DistinctCaptionCount
    FROM dbo.tbl_images AS i
    GROUP BY i.image_art
),
ChildCounts AS
(
    SELECT menu_mainID AS MenuId, COUNT_BIG(*) AS ChildCount
    FROM dbo.tbl_menu
    WHERE menu_mainID IS NOT NULL
    GROUP BY menu_mainID
)
SELECT
    t.RootMenuId,
    t.CategoryPath,
    t.menu_ID AS MenuId,
    t.menu_mainID AS ParentMenuId,
    t.menu_text AS NameSv,
    t.menu_URLtext AS UrlSv,
    t.menu_text_en AS NameEn,
    t.menu_URLtext_en AS UrlEn,
    t.Depth,
    COALESCE(c.ChildCount, 0) AS ChildCount,
    COALESCE(i.ImageCount, 0) AS ImageCount,
    COALESCE(i.WithoutCaptionCount, 0) AS WithoutCaptionCount,
    COALESCE(i.DistinctCaptionCount, 0) AS DistinctCaptionCount
FROM CategoryTree AS t
LEFT JOIN ImageCounts AS i ON i.MenuId = t.menu_ID
LEFT JOIN ChildCounts AS c ON c.MenuId = t.menu_ID
ORDER BY t.RootMenuId, t.CategoryPath
OPTION (MAXRECURSION 100);

;WITH CategoryTree AS
(
    SELECT
        m.menu_ID,
        m.menu_mainID,
        m.menu_text,
        m.menu_ID AS RootMenuId,
        CONVERT(nvarchar(2000), COALESCE(m.menu_text, N'')) AS CategoryPath
    FROM dbo.tbl_menu AS m
    INNER JOIN @BiologicalRoots AS r ON r.MenuId = m.menu_ID
    WHERE m.menu_mainID = 0

    UNION ALL

    SELECT
        child.menu_ID,
        child.menu_mainID,
        child.menu_text,
        parent.RootMenuId,
        CONVERT(nvarchar(2000), parent.CategoryPath + N' > ' + COALESCE(child.menu_text, N''))
    FROM dbo.tbl_menu AS child
    INNER JOIN CategoryTree AS parent ON child.menu_mainID = parent.menu_ID
)
SELECT
    t.RootMenuId,
    t.CategoryPath,
    i.image_art AS SourceArtId,
    i.image_description AS RawCaption,
    COUNT_BIG(*) AS ImageCount,
    MIN(i.image_URL) AS ExampleImageUrl
FROM dbo.tbl_images AS i
INNER JOIN CategoryTree AS t ON t.menu_ID = i.image_art
GROUP BY t.RootMenuId, t.CategoryPath, i.image_art, i.image_description
ORDER BY t.RootMenuId, t.CategoryPath, i.image_description
OPTION (MAXRECURSION 100);

SELECT
    i.image_art AS UnresolvedArtId,
    COUNT_BIG(*) AS ImageCount,
    SUM(CASE WHEN NULLIF(LTRIM(RTRIM(i.image_description)), N'') IS NULL THEN 1 ELSE 0 END) AS WithoutCaptionCount,
    MIN(i.image_URL) AS ExampleImageUrl
FROM dbo.tbl_images AS i
LEFT JOIN dbo.tbl_menu AS m ON m.menu_ID = i.image_art
WHERE i.image_art IS NULL
   OR m.menu_ID IS NULL
GROUP BY i.image_art
ORDER BY i.image_art;

;WITH CategoryTree AS
(
    SELECT m.menu_ID, m.menu_mainID, m.menu_text, m.menu_URLtext, m.menu_ID AS RootMenuId
    FROM dbo.tbl_menu AS m
    WHERE m.menu_mainID = 0

    UNION ALL

    SELECT child.menu_ID, child.menu_mainID, child.menu_text, child.menu_URLtext, parent.RootMenuId
    FROM dbo.tbl_menu AS child
    INNER JOIN CategoryTree AS parent ON child.menu_mainID = parent.menu_ID
)
SELECT
    t.RootMenuId,
    t.menu_ID AS MenuId,
    t.menu_mainID AS ParentMenuId,
    t.menu_text AS NameSv,
    t.menu_URLtext AS UrlSv
FROM CategoryTree AS t
WHERE t.menu_URLtext IS NULL
   OR LTRIM(RTRIM(t.menu_URLtext)) = N''
   OR t.menu_URLtext COLLATE Latin1_General_100_BIN2 LIKE N'%[^a-z0-9,-]%'
   OR t.menu_URLtext COLLATE Latin1_General_100_BIN2 LIKE N'%[åäöÅÄÖ]%'
ORDER BY t.RootMenuId, t.menu_ID
OPTION (MAXRECURSION 100);

/*
    Mammal and most other animal images already use species-level menu_IDs.
    This result exposes their current assignments for verification without
    changing established category IDs.
*/
;WITH CategoryTree AS
(
    SELECT m.menu_ID, m.menu_mainID, m.menu_text, m.menu_ID AS RootMenuId
    FROM dbo.tbl_menu AS m
    WHERE m.menu_ID = 2 AND m.menu_mainID = 0

    UNION ALL

    SELECT child.menu_ID, child.menu_mainID, child.menu_text, parent.RootMenuId
    FROM dbo.tbl_menu AS child
    INNER JOIN CategoryTree AS parent ON child.menu_mainID = parent.menu_ID
)
SELECT
    t.menu_ID AS SpeciesMenuId,
    t.menu_text AS SpeciesNameSv,
    COUNT_BIG(i.ID) AS ImageCount,
    SUM(CASE WHEN NULLIF(LTRIM(RTRIM(i.image_description)), N'') IS NULL THEN 1 ELSE 0 END) AS WithoutCaptionCount
FROM CategoryTree AS t
LEFT JOIN dbo.tbl_images AS i ON i.image_art = t.menu_ID
GROUP BY t.menu_ID, t.menu_text
ORDER BY t.menu_text
OPTION (MAXRECURSION 100);
