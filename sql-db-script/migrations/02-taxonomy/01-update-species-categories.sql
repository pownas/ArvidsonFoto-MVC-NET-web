/*
    Adds approved species categories below the existing biological groups
    and moves only exact/explicitly mapped image captions to those new IDs.

    Run 00-review-taxonomy.sql first. Review the preview below, then set
    @ApplyChanges = 1 to commit. Unmapped captions remain at their current
    category and are reported for later review.
*/
SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @ApplyChanges bit = 0;

CREATE TABLE #Species
(
    ParentMenuId int NOT NULL,
    SpeciesName nvarchar(50) NOT NULL,
    PRIMARY KEY (ParentMenuId, SpeciesName)
);

INSERT INTO #Species (ParentMenuId, SpeciesName)
VALUES
    (303, N'Adam och Eva'),
    (303, N'Backsippa'),
    (303, N'Backtimjan'),
    (303, N'Bergskrabba'),
    (303, N'Blåeld'),
    (303, N'Blåklocka'),
    (303, N'Blåsippa'),
    (303, N'Brudborst'),
    (303, N'Brunkulla'),
    (303, N'Cikoria'),
    (303, N'Fjällbrud'),
    (303, N'Fjällgentiana'),
    (303, N'Fjällglim'),
    (303, N'Fjällgröna'),
    (303, N'Fjällnejlika'),
    (303, N'Fjällsippa'),
    (303, N'Fjällvedel'),
    (303, N'Fjällviol'),
    (303, N'Flugblomster'),
    (303, N'Fältsippa'),
    (303, N'Getrams'),
    (303, N'Grönvit nattviol'),
    (303, N'Guckusko'),
    (303, N'Gul fetknopp'),
    (303, N'Gullviva'),
    (303, N'Gulsippa'),
    (303, N'Göknycklar'),
    (303, N'Harsyra'),
    (303, N'Hjortron'),
    (303, N'Humleblomster'),
    (303, N'Hästhov'),
    (303, N'Hönsbär'),
    (303, N'Jättebjörnloka'),
    (303, N'Kabbeleka'),
    (303, N'Kaprifol'),
    (303, N'Krasse'),
    (303, N'Krutbrännare'),
    (303, N'Krypljung'),
    (303, N'Kärrknipprot'),
    (303, N'Lappljung'),
    (303, N'Liljekonvalj'),
    (303, N'Luktviol'),
    (303, N'Lungört'),
    (303, N'Lupin'),
    (303, N'Majviva'),
    (303, N'Mandelblom'),
    (303, N'Martorn'),
    (303, N'Maskros'),
    (303, N'Mistel'),
    (303, N'Mjölkört'),
    (303, N'Mosippa'),
    (303, N'Mossljung'),
    (303, N'Natt och dag'),
    (303, N'Nattviol'),
    (303, N'Nipsippa'),
    (303, N'Norna'),
    (303, N'Nunneört'),
    (303, N'Nyponros'),
    (303, N'Nästrot'),
    (303, N'Ostronört'),
    (303, N'Pestskråp'),
    (303, N'Purpurbräcka'),
    (303, N'Purpurknipprot'),
    (303, N'Ros'),
    (303, N'Rosenrot'),
    (303, N'Rosling'),
    (303, N'Röd näckros'),
    (303, N'Rödblära'),
    (303, N'Rödfibbla'),
    (303, N'Tranbär'),
    (303, N'Salepsrot'),
    (303, N'Saltarv'),
    (303, N'Sankt Pers nycklar'),
    (303, N'Skogsnycklar'),
    (303, N'Skogsnäva'),
    (303, N'Skogsstjärna'),
    (303, N'Skvattram'),
    (303, N'Smultron'),
    (303, N'Solros'),
    (303, N'Stor blåklocka'),
    (303, N'Storrams'),
    (303, N'Strandkål'),
    (303, N'Strandvial'),
    (303, N'Strutbräken'),
    (303, N'Styvmorsviol'),
    (303, N'Syrenbuddleja'),
    (303, N'Tallört'),
    (303, N'Tibast'),
    (303, N'Tjärblomster'),
    (303, N'Tovsippa'),
    (303, N'Tvåblad'),
    (303, N'Tätört'),
    (303, N'Vallmo'),
    (303, N'Viol'),
    (303, N'Vitpyrola'),
    (303, N'Vitsippa'),
    (303, N'Våradonis'),
    (303, N'Vårlök'),
    (303, N'Vätteros'),
    (303, N'Ängsklocka'),
    (303, N'Övriga blommor'),
    (337, N'Björnbär'),
    (337, N'Blåbär'),
    (337, N'Blåhallon'),
    (337, N'Hallon'),
    (337, N'Hjortron'),
    (337, N'Jordgubbe'),
    (337, N'Kantarell'),
    (337, N'Körsbär'),
    (337, N'Lingon'),
    (337, N'Nypon'),
    (337, N'Röd flugsvamp'),
    (337, N'Röda vinbär'),
    (337, N'Skogshallon'),
    (337, N'Slånbär'),
    (337, N'Smultron'),
    (337, N'Stenmurkla'),
    (337, N'Trattkantarell'),
    (337, N'Övriga bär och svampar'),
    (338, N'Körsbärsträd'),
    (338, N'Äppelträd'),
    (357, N'Amaryllis'),
    (357, N'Dahlia'),
    (357, N'Daggkåpa'),
    (357, N'Fingerborgsblomma'),
    (357, N'Forsytia'),
    (357, N'Gräslök'),
    (357, N'Hibiskus'),
    (357, N'Iris'),
    (357, N'Krasse'),
    (357, N'Krokus'),
    (357, N'Löjtnantshjärta'),
    (357, N'Magnolia'),
    (357, N'Malva'),
    (357, N'Orkidé'),
    (357, N'Pingstlilja'),
    (357, N'Pion'),
    (357, N'Prydnadspumpa'),
    (357, N'Påsklilja'),
    (357, N'Rabarber'),
    (357, N'Rhododendron'),
    (357, N'Ros'),
    (357, N'Rudbeckia'),
    (357, N'Röd solhatt'),
    (357, N'Solros'),
    (357, N'Stockros'),
    (357, N'Tulpan'),
    (357, N'Vintergäck'),
    (357, N'Vitlök'),
    (357, N'Övriga trädgårdsväxter'),
    (37, N'Brun sandjägare'),
    (37, N'Ekoxe'),
    (37, N'Granbarkborre'),
    (37, N'Guldbagge'),
    (37, N'Grön sandjägare'),
    (37, N'Långhorningar'),
    (37, N'Lövträdlöpare'),
    (37, N'Myskbock'),
    (37, N'Nyckelpigor'),
    (37, N'Ollonborre'),
    (37, N'Rödbandad dödgrävare'),
    (37, N'Smedbock'),
    (37, N'Taggbock'),
    (37, N'Videbock'),
    (37, N'Vivlar'),
    (37, N'Övriga skalbaggar'),
    (35, N'Bålgeting'),
    (35, N'Bin'),
    (35, N'Bromsar'),
    (35, N'Bärfisar'),
    (35, N'Eldskinnbagge'),
    (35, N'Flugor'),
    (35, N'Getingar'),
    (35, N'Grön vårtbitare'),
    (35, N'Myrlejon'),
    (35, N'Myggor'),
    (35, N'Myror'),
    (35, N'Riddarskinnbagge'),
    (35, N'Rödbent bärfis'),
    (35, N'Svävflugor'),
    (35, N'Tvestjärtar'),
    (35, N'Vargbi'),
    (35, N'Vårtbitare'),
    (6, N'Blåstång');

CREATE TABLE #CaptionAliases
(
    SourceCategoryId int NOT NULL,
    ParentMenuId int NOT NULL,
    Caption nvarchar(150) NOT NULL,
    SpeciesName nvarchar(50) NOT NULL,
    PRIMARY KEY (SourceCategoryId, Caption)
);

INSERT INTO #CaptionAliases (SourceCategoryId, ParentMenuId, Caption, SpeciesName)
VALUES
    (303, 303, N'Blåsippa  röd', N'Blåsippa'),
    (303, 303, N'Blåsippa (röd)', N'Blåsippa'),
    (303, 303, N'Blåsippa (rödsippa)', N'Blåsippa'),
    (303, 303, N'Blåsippa, röd variant', N'Blåsippa'),
    (303, 303, N'Brunkulla, orkide, Jämtland', N'Brunkulla'),
    (303, 303, N'Fjällglimm', N'Fjällglim'),
    (303, 303, N'Hjortronblomma', N'Hjortron'),
    (303, 303, N'Lupiner', N'Lupin'),
    (303, 303, N'Mjölkört (Rallarros)', N'Mjölkört'),
    (303, 303, N'Norna, orkide', N'Norna'),
    (303, 303, N'Skogsnäva (Midsommarblomster)', N'Skogsnäva'),
    (303, 303, N'Skogsnäva, Midsommarblomster', N'Skogsnäva'),
    (303, 303, N'Tibast, giftiga bär', N'Tibast'),
    (303, 303, N'Tvåblad, orkide', N'Tvåblad'),
    (303, 303, N'Kärrknipprot, orkide', N'Kärrknipprot'),
    (303, 303, N'Purpurknipprot, orkide', N'Purpurknipprot'),
    (303, 303, N'Våradonis (Arontorpsros)', N'Våradonis'),
    (303, 303, N'Bergskrabba och Ölandsolvända', N'Övriga blommor'),
    (303, 303, N'Blåsippa och Vitsippa', N'Övriga blommor'),
    (303, 303, N'Daldocka, smörboll', N'Övriga blommor'),
    (303, 303, N'Gullviva o Majviva', N'Övriga blommor'),
    (303, 303, N'Gullviva och Göknycklar', N'Övriga blommor'),
    (303, 303, N'Gullviva och Majviva', N'Övriga blommor'),
    (303, 303, N'Gullviva och orkidé', N'Övriga blommor'),
    (303, 303, N'Gullviva och Sankt Pers nycklar', N'Övriga blommor'),
    (303, 303, N'Guulviva och Sankt Pers nycklar', N'Övriga blommor'),
    (303, 303, N'Göknyclar o gullvivor', N'Övriga blommor'),
    (303, 303, N'Majviva och Gullviva', N'Övriga blommor'),
    (303, 303, N'Prästkrage och Blåklocka', N'Övriga blommor'),
    (303, 303, N'Prästkrage och Klöver', N'Övriga blommor'),
    (303, 303, N'Röd och Vit näckros', N'Övriga blommor'),
    (303, 303, N'Sankt Pers nycklar o gullvivor', N'Övriga blommor'),
    (303, 303, N'Skogsnäva o Smörboll', N'Övriga blommor'),
    (303, 303, N'Smörboll o Skogsnäva', N'Övriga blommor'),
    (303, 303, N'Smörboll, daldocka', N'Övriga blommor'),
    (303, 303, N'Stor blåklocka (vit)', N'Stor blåklocka'),
    (303, 303, N'Vallmo och cikoria', N'Övriga blommor'),
    (303, 303, N'Vallmo, Blåklint och Baldersbrå', N'Övriga blommor'),
    (303, 303, N'Vitsippa o gulsippa', N'Övriga blommor'),
    (303, 6, N'Blåstång', N'Blåstång'),
    (337, 337, N'Jordgubbar', N'Jordgubbe'),
    (337, 337, N'Jurdgubbar', N'Jordgubbe'),
    (337, 337, N'Gul kantarell', N'Kantarell'),
    (337, 337, N'Kantareller', N'Kantarell'),
    (337, 337, N'nypon', N'Nypon'),
    (337, 337, N'Salmbär (blåhallon)', N'Blåhallon'),
    (337, 337, N'Salmbär, blåhallon', N'Blåhallon'),
    (337, 337, N'Trattkantareller', N'Trattkantarell'),
    (337, 337, N'Kantareller och lingon', N'Övriga bär och svampar'),
    (337, 337, N'Svarta och röda vinbär', N'Övriga bär och svampar'),
    (337, 337, N'Bärplockning', N'Övriga bär och svampar'),
    (337, 337, N'Kokning av lingonsylt', N'Övriga bär och svampar'),
    (357, 357, N'Dalia', N'Dahlia'),
    (357, 357, N'Baldersbrå, Vallmo o Blåklint', N'Övriga trädgårdsväxter'),
    (357, 357, N'höstblommor', N'Övriga trädgårdsväxter'),
    (357, 357, N'Orkide', N'Orkidé'),
    (357, 357, N'Ros Ingrid Bergman', N'Ros'),
    (357, 338, N'Äppelblom', N'Äppelträd'),
    (357, 338, N'Körsbärsträd', N'Körsbärsträd'),
    (37, 37, N'Ekoxe, hane', N'Ekoxe'),
    (37, 37, N'Ekoxe, hane och hona', N'Ekoxe'),
    (37, 35, N'Eldskinnbagge', N'Eldskinnbagge'),
    (37, 37, N'hane', N'Övriga skalbaggar'),
    (37, 37, N'hona', N'Övriga skalbaggar'),
    (37, 37, N'Långhorning', N'Långhorningar'),
    (37, 37, N'Nyckelpiga', N'Nyckelpigor'),
    (37, 37, N'Taggbock', N'Taggbock'),
    (37, 37, N'Vivel', N'Vivlar'),
    (37, 37, N'Grön Sandjägare', N'Grön sandjägare'),
    (37, 37, N'Myskbock, parande', N'Myskbock'),
    (37, 37, N'Smedbock (även kallad större vedbock)', N'Smedbock'),
    (35, 35, N'Bi på födosök', N'Bin'),
    (35, 35, N'Bin', N'Bin'),
    (35, 35, N'Broms', N'Bromsar'),
    (35, 35, N'Bärfis', N'Bärfisar'),
    (35, 35, N'Fluga', N'Flugor'),
    (35, 35, N'Geting', N'Getingar'),
    (35, 35, N'Mygga', N'Myggor'),
    (35, 35, N'Myra', N'Myror'),
    (35, 35, N'Svävfluga', N'Svävflugor'),
    (35, 35, N'Tvestjärt', N'Tvestjärtar'),
    (35, 35, N'Vårtbitare, hona', N'Vårtbitare'),
    (35, 35, N'Grön vårtbitare, hona', N'Grön vårtbitare'),
    (35, 35, N'Myrlejon med byte', N'Myrlejon');

IF EXISTS
(
    SELECT 1
    FROM #Species AS s
    LEFT JOIN dbo.tbl_menu AS parent ON parent.menu_ID = s.ParentMenuId
    WHERE parent.menu_ID IS NULL
)
    THROW 51000, 'A target parent menu_ID does not exist. Run 00-review-taxonomy.sql and correct the map.', 1;

IF EXISTS
(
    SELECT 1
    FROM #Species AS s
    CROSS APPLY
    (
        VALUES
        (
            REPLACE(
                REPLACE(
                    REPLACE(
                        LOWER(TRANSLATE(s.SpeciesName, N'ÅÄÖåäöÉé', N'AAOaaoEe')),
                        N' ',
                        N'-'
                    ),
                    N'''',
                    N''
                ),
                N'.',
                N''
            )
        )
    ) AS generated(MenuUrlText)
    WHERE generated.MenuUrlText COLLATE Latin1_General_100_BIN2 LIKE N'%[^a-z0-9,-]%'
)
    THROW 51002, 'A species name generates a non-ASCII folder slug. Correct the transliteration map before applying.', 1;

IF EXISTS
(
    SELECT 1
    FROM #CaptionAliases AS a
    LEFT JOIN #Species AS s
        ON s.ParentMenuId = a.ParentMenuId
       AND s.SpeciesName = a.SpeciesName
    WHERE s.SpeciesName IS NULL
)
    THROW 51001, 'A caption alias points to a species absent from #Species.', 1;

CREATE TABLE #ImagePlan
(
    ImageRowId int NOT NULL PRIMARY KEY,
    PreviousArtId int NOT NULL,
    ParentMenuId int NOT NULL,
    SpeciesName nvarchar(50) NOT NULL
);

INSERT INTO #ImagePlan (ImageRowId, PreviousArtId, ParentMenuId, SpeciesName)
SELECT i.ID, i.image_art, s.ParentMenuId, s.SpeciesName
FROM dbo.tbl_images AS i
INNER JOIN #Species AS s
    ON s.ParentMenuId = i.image_art
   AND s.SpeciesName = LTRIM(RTRIM(i.image_description));

INSERT INTO #ImagePlan (ImageRowId, PreviousArtId, ParentMenuId, SpeciesName)
SELECT i.ID, i.image_art, a.ParentMenuId, a.SpeciesName
FROM dbo.tbl_images AS i
INNER JOIN #CaptionAliases AS a
    ON a.SourceCategoryId = i.image_art
   AND a.Caption = i.image_description
WHERE NOT EXISTS
(
    SELECT 1
    FROM #ImagePlan AS p
    WHERE p.ImageRowId = i.ID
);

SELECT
    p.PreviousArtId AS SourceCategoryId,
    p.ParentMenuId,
    p.SpeciesName,
    COUNT_BIG(*) AS ImageCount
FROM #ImagePlan AS p
GROUP BY p.PreviousArtId, p.ParentMenuId, p.SpeciesName
ORDER BY p.PreviousArtId, p.SpeciesName;

SELECT
    i.image_art AS UnresolvedCategoryId,
    m.menu_text AS UnresolvedCategoryName,
    i.image_description AS UnresolvedCaption,
    COUNT_BIG(*) AS ImageCount,
    MIN(i.image_URL) AS ExampleImageUrl
FROM dbo.tbl_images AS i
LEFT JOIN dbo.tbl_menu AS m ON m.menu_ID = i.image_art
WHERE i.image_art IN (303, 337, 357, 35, 37)
  AND NOT EXISTS
  (
      SELECT 1
      FROM #ImagePlan AS p
      WHERE p.ImageRowId = i.ID
  )
GROUP BY i.image_art, m.menu_text, i.image_description
ORDER BY i.image_art, i.image_description;

SELECT
    m.menu_ID AS ExistingMammalSpeciesMenuId,
    m.menu_text AS SpeciesNameSv,
    m.menu_URLtext AS FolderSlugSv,
    COUNT_BIG(i.ID) AS ImageCount
FROM dbo.tbl_menu AS m
LEFT JOIN dbo.tbl_images AS i ON i.image_art = m.menu_ID
WHERE m.menu_mainID = 2
GROUP BY m.menu_ID, m.menu_text, m.menu_URLtext
ORDER BY m.menu_text;

IF @ApplyChanges = 0
BEGIN
    PRINT 'Preview only. Review every proposed and unresolved caption before setting @ApplyChanges = 1.';
    RETURN;
END;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @LastMenuId int;
    SELECT @LastMenuId = ISNULL(MAX(menu_ID), 0)
    FROM dbo.tbl_menu WITH (UPDLOCK, HOLDLOCK);

    ;WITH RequiredSpecies AS
    (
        SELECT DISTINCT p.ParentMenuId, p.SpeciesName
        FROM #ImagePlan AS p
    ),
    MissingSpecies AS
    (
        SELECT
            r.ParentMenuId,
            r.SpeciesName,
            ROW_NUMBER() OVER (ORDER BY r.ParentMenuId, r.SpeciesName) AS RowNumber
        FROM RequiredSpecies AS r
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM dbo.tbl_menu AS m
            WHERE m.menu_mainID = r.ParentMenuId
              AND m.menu_text = r.SpeciesName
        )
    )
    INSERT INTO dbo.tbl_menu
    (
        menu_ID,
        menu_mainID,
        menu_text,
        menu_URLtext,
        menu_dateUpdated,
        menu_oldPagecounter,
        menu_pagecounter,
        menu_text_en,
        menu_URLtext_en,
        menu_text_en_reviewed,
        menu_URLtext_en_reviewed,
        is_game_selectable
    )
    SELECT
        @LastMenuId + CONVERT(int, missing.RowNumber),
        missing.ParentMenuId,
        missing.SpeciesName,
        REPLACE(
            REPLACE(
                REPLACE(
                LOWER(TRANSLATE(missing.SpeciesName, N'ÅÄÖåäöÉé', N'AAOaaoEe')),
                    N' ',
                    N'-'
                ),
                N'''',
                N''
            ),
            N'.',
            N''
        ),
        GETDATE(),
        NULL,
        0,
        NULL,
        NULL,
        0,
        0,
        1
    FROM MissingSpecies AS missing;

    DECLARE @InsertedCategoryCount int = @@ROWCOUNT;

    UPDATE i
    SET image_art = m.menu_ID
    FROM dbo.tbl_images AS i
    INNER JOIN #ImagePlan AS p ON p.ImageRowId = i.ID
    INNER JOIN dbo.tbl_menu AS m
        ON m.menu_mainID = p.ParentMenuId
       AND m.menu_text = p.SpeciesName
    WHERE i.image_art = p.PreviousArtId;

    DECLARE @UpdatedImageCount int = @@ROWCOUNT;

    COMMIT TRANSACTION;

    SELECT
        @InsertedCategoryCount AS InsertedSpeciesCategories,
        @UpdatedImageCount AS UpdatedImageRows;

    PRINT 'Migration committed. Mammal species IDs were retained; no English labels were guessed.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;
