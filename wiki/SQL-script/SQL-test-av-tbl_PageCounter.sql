use master; 
go

if db_id('ArvidsonFoto-Test') is not null
	drop database [ArvidsonFoto-Test]; 
go

create database [ArvidsonFoto-Test];
go 

use [ArvidsonFoto-Test]; 
go

CREATE TABLE [tbl_PageCounter]
(PageCounter_ID INTEGER IDENTITY(1,1) NOT NULL PRIMARY KEY,
PageCounter_Views int NOT NULL,
PageCounter_Name NVARCHAR(50),
PageCounter_MonthViewed NVARCHAR(20),
PageCounter_LastShowDate DATETIME NOT NULL
);

INSERT INTO tbl_PageCounter VALUES 
(44,'Startsidan','2021-02','2021-02-04 18:03:32.427'),
(4,'Info','2021-02','2021-02-04 15:31:52.620'),
(18,'Köp av bilder','2021-02','2021-02-04 16:28:56.040'),
(600,'Bilder','2021-02','2021-02-04 18:12:52.170'),
(11,'Kontaktinformation','2021-02','2021-02-04 18:06:38.663'),
(3,'Om mig','2021-02','2021-02-04 15:49:59.877'),
(4,'Sidkarta','2021-02','2021-02-04 15:49:55.273'),
(6,'Copyright','2021-02','2021-02-04 15:54:09.000'),
(21,'Senast-Fotograferad','2021-02','2021-02-04 17:53:59.307'),
(3,'Senast-Uppladdad','2021-02','2021-02-04 15:51:24.580'),
(5,'Senast-Per kategori','2021-02','2021-02-04 16:13:32.193'),
(6,'Sök','2021-02','2021-02-04 15:49:49.770'),
(9,'Gästbok','2021-02','2021-02-04 15:50:08.657'),
(129534,'Startsidan','2021-01','2021-02-04 12:27:41.000'),
(47522,'Senast-Fotograferad','2021-01','2021-02-04 12:28:50.000'),
(42334,'Senast-Uppladdad','2021-01','2021-02-04 12:18:31.000'),
(2695,'Köp av bilder','2021-01','2021-02-04 09:25:35.000'),
(30876,'Gästbok','2021-01','2021-02-04 11:04:26.000'),
(3092,'Kontaktinformation','2021-01','2021-02-03 14:22:22.000'),
(2708,'Om mig','2021-01','2021-02-04 12:16:10.000'),
(2427,'Sidkarta','2021-01','2021-02-03 16:56:27.000'),
(2473,'Copyright','2021-01','2021-02-03 20:23:25.000'),
(2611,'Sök','2021-01','2021-02-03 22:33:06.000'),
(5466,'Senast-Per kategori','2021-01','2021-02-04 03:49:33.000');



--För att välja ut den datan ovan, körs detta scriptet: 
SELECT SUM(PageCounter_Views) AS PageCounter_Views, PageCounter_Name, MAX(PageCounter_LastShowDate) AS PageCounter_LastShowDate
	  FROM tbl_PageCounter 
	  GROUP BY PageCounter_Name
	  ORDER BY PageCounter_Views DESC;