-----------------------SELECT----------------------------

SELECT * FROM [ArvidsonFotoDev].[dbo].[tbl_menu];
SELECT * FROM [ArvidsonFotoDev].[dbo].[tbl_PageCounter];

-----------------------UPDATE----------------------------

UPDATE tbl_menu SET menu_URLtext = 'Mink', menu_oldPagecounter = 0 WHERE Menu_ID = 518;

---------------------------------------------------------

DECLARE @Number INT = 1;
DECLARE @Total INT = 10; --Max ska vara: 506
DECLARE @CategoryId INT = 0;
DECLARE @CategoryName nvarChar(50) = '';
DECLARE @202012 int = 0;
DECLARE @202102 int = 0;
DECLARE @202103 int = 0;

PRINT 'CategoryId;CategoryName;OldPageCounter;PageCounter;2020-12;2021-02;2021-03';

WHILE @Number <= @Total
BEGIN
	SET @CategoryId = (SELECT Menu_ID FROM tbl_Menu WHERE ID = @Number);
	SET @CategoryName = (SELECT menu_text FROM tbl_menu WHERE ID = @Number);
	IF (@CategoryId > 0)
		BEGIN
			DECLARE @PageCounter INT = 0;
			DECLARE @OldPageCounter INT = 0;
			SET @PageCounter = (SELECT menu_pagecounter FROM tbl_Menu WHERE Menu_ID = @CategoryId);
			SET @OldPageCounter = (SELECT menu_oldPagecounter FROM tbl_Menu WHERE Menu_ID = @CategoryId);
			SET @202103 = (SELECT PageCounter_Views FROM tbl_PageCounter WHERE PageCounter_CategoryId = @CategoryId);
			IF (@202103 = 0)
			BEGIN
				SET @202103 = 0;
			END
			SET @202012 = @OldPageCounter;
			SET @202102 = @PageCounter / 2;
			SET @202103 = @202103 + (@PageCounter / 2);

			PRINT CAST(@CategoryId AS NVARCHAR(10)) + ';' + @CategoryName + ';' + CAST(@OldPageCounter AS NVARCHAR(10)) 
			      + ';' +  CAST(@PageCounter AS NVARCHAR(10)) + ';' +  CAST(@202012 AS NVARCHAR(10)) + ';' 
				  + CAST(@202102 AS NVARCHAR(10)) + ';' +  CAST(@202103 AS NVARCHAR(10));
		END
	ELSE 
		BEGIN
			PRINT 'Empty';
		END
	
	SET @Number = @Number + 1;
END
GO




---------------------------------------------------------
--Delar som går fel i scriptet ovan:
-- Rad 32-36 + Rad 39

DECLARE @202103 int = 0;
SET @202103 = (SELECT PageCounter_Views FROM tbl_PageCounter WHERE PageCounter_CategoryId = 3);
IF (@202103 = 0)
BEGIN
	SET @202103 = 1;
END

PRINT CAST(@202103 AS NVARCHAR(10));
GO
---------------------------------------------------------