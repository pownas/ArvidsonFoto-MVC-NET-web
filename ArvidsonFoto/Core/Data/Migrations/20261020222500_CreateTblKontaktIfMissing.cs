using ArvidsonFoto.Core.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArvidsonFoto.Core.Data.Migrations;

[DbContext(typeof(ArvidsonFotoCoreDbContext))]
[Migration("20261020222500_CreateTblKontaktIfMissing")]
public partial class CreateTblKontaktIfMissing : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
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
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}