using ArvidsonFoto.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ArvidsonFoto.Core.Data;

/// <summary>
/// Statisk klass för att hantera seed-data till databasen.
/// </summary>
/// <remarks>
/// Denna klass innehåller extensionmetoder för att skapa initial testdata
/// i databasen via Entity Framework Core ModelBuilder.
/// </remarks>
public static class ArvidsonFotoCoreDbSeeder
{
    #region Guestbook data

    /// <summary>
    /// Guestbook entries seed data
    /// </summary>
    public static List<TblGb> GuestbookEntries => new()
    {
        new()
        {
            Id = 1,
            GbId = 1,
            GbDate = new DateTime(2021, 11, 22),
            GbEmail = "pownas@outlook.com",
            GbName = "pownas",
            GbHomepage = "github.com/pownas",
            GbReadPost = false,
            GbText = "Ett första test inlägg i databasen..."
        }
    };

    #endregion

    #region NavMenu data

    /// <summary>
    /// Menu categories seed data with hierarchical structure
    /// </summary>
    /// <remarks>
    /// Exportera ut 200 rader av SQL-data från databasen med:
    /// <code>
    /// -- ===========================================
    /// -- Export TblMenu(200 rows)
    /// -- ===========================================
    /// SELECT 
    /// '    new() { ' +
    /// 'Id = ' + CAST(ISNULL([Id], 'null') AS NVARCHAR(10)) + ', ' +
    /// 
    /// 'MenuCategoryId = ' + CAST(ISNULL([menu_ID], 'null') AS NVARCHAR(10)) + ', ' +
    /// 'MenuParentCategoryId = ' + CASE
    /// WHEN[menu_mainID] IS NULL OR[menu_mainID] = 0 THEN '0' 
    /// ELSE CAST([menu_mainID] AS NVARCHAR(10)) 
    /// END + ', ' +
    /// 'MenuDisplayName = "' + REPLACE(ISNULL([menu_text], ''), '"', '\"') + '", ' +
    /// 'MenuUrlSegment = "' + REPLACE(ISNULL([menu_URLtext], ''), '"', '\"') + '", ' +
    /// 'MenuDateUpdated = ' + CASE
    /// WHEN[menu_dateUpdated] IS NULL THEN 'null'
    /// ELSE 'DateTime.Parse("' + FORMAT([menu_dateUpdated], 'yyyy-MM-ddTHH:mm:ss') + '")'
    /// END +
    /// 
    /// ' },' AS CSharpCode
    /// FROM [tbl_menu]
    /// ORDER BY [Id]
    /// OFFSET 0 ROWS FETCH NEXT 200 ROWS ONLY;
    /// </code>
    /// </remarks>
    public static List<TblMenu> MenuCategories => new()
    {
        // Main categories (parent = 0)
        new() { Id = 1, MenuCategoryId = 1, MenuParentCategoryId = 0, MenuDisplayName = "Fåglar", MenuUrlSegment = "Faglar", MenuDateUpdated = DateTime.Parse("2021-03-25T20:27:00") },
        new() { Id = 2, MenuCategoryId = 2, MenuParentCategoryId = 0, MenuDisplayName = "Däggdjur", MenuUrlSegment = "Daggdjur", MenuDateUpdated = DateTime.Parse("2021-03-19T13:56:00") },
        new() { Id = 3, MenuCategoryId = 3, MenuParentCategoryId = 0, MenuDisplayName = "Kräldjur", MenuUrlSegment = "Kraldjur", MenuDateUpdated = DateTime.Parse("2021-03-17T14:40:00") },
        new() { Id = 5, MenuCategoryId = 5, MenuParentCategoryId = 0, MenuDisplayName = "Insekter", MenuUrlSegment = "Insekter", MenuDateUpdated = DateTime.Parse("2021-03-10T11:53:00") },
        new() { Id = 6, MenuCategoryId = 6, MenuParentCategoryId = 0, MenuDisplayName = "Växter", MenuUrlSegment = "Vaxter", MenuDateUpdated = DateTime.Parse("2021-03-16T07:25:00") },
        new() { Id = 7, MenuCategoryId = 7, MenuParentCategoryId = 0, MenuDisplayName = "Landskap", MenuUrlSegment = "Landskap", MenuDateUpdated = DateTime.Parse("2021-03-19T13:56:00") },
        new() { Id = 8, MenuCategoryId = 8, MenuParentCategoryId = 0, MenuDisplayName = "Årstider", MenuUrlSegment = "Arstider", MenuDateUpdated = DateTime.Parse("2021-03-16T07:45:00") },
        new() { Id = 9, MenuCategoryId = 9, MenuParentCategoryId = 0, MenuDisplayName = "Resor", MenuUrlSegment = "Resor", MenuDateUpdated = DateTime.Parse("2021-03-19T13:57:00") },
        
        // Sub-categories under Insekter (parent = 5)
        new() { Id = 4, MenuCategoryId = 4, MenuParentCategoryId = 5, MenuDisplayName = "Fjärilar", MenuUrlSegment = "Fjarilar", MenuDateUpdated = DateTime.Parse("2021-03-09T08:55:00") },
        new() { Id = 35, MenuCategoryId = 35, MenuParentCategoryId = 5, MenuDisplayName = "Övriga insekter", MenuUrlSegment = "Ovriga-insekter", MenuDateUpdated = DateTime.Parse("2021-03-01T05:37:00") },
        new() { Id = 36, MenuCategoryId = 36, MenuParentCategoryId = 5, MenuDisplayName = "Sländor", MenuUrlSegment = "Slandor", MenuDateUpdated = DateTime.Parse("2021-03-01T21:28:00") },
        new() { Id = 37, MenuCategoryId = 37, MenuParentCategoryId = 5, MenuDisplayName = "Skalbaggar", MenuUrlSegment = "Skalbaggar", MenuDateUpdated = DateTime.Parse("2021-03-24T00:22:00") },

        // Sub-categories under Fåglar (parent = 1)
        new() { Id = 10, MenuCategoryId = 10, MenuParentCategoryId = 1, MenuDisplayName = "Dagrovfåglar", MenuUrlSegment = "Dagrovfaglar", MenuDateUpdated = DateTime.Parse("2021-03-23T21:53:00") },
        new() { Id = 20, MenuCategoryId = 20, MenuParentCategoryId = 1, MenuDisplayName = "Svanar", MenuUrlSegment = "Svanar", MenuDateUpdated = DateTime.Parse("2021-03-25T20:29:00") },
        new() { Id = 21, MenuCategoryId = 21, MenuParentCategoryId = 1, MenuDisplayName = "Skogs- och fälthöns", MenuUrlSegment = "Skogs-Och-Falthons", MenuDateUpdated = DateTime.Parse("2021-03-01T06:01:00") },
        new() { Id = 22, MenuCategoryId = 22, MenuParentCategoryId = 1, MenuDisplayName = "Vadare", MenuUrlSegment = "Vadare", MenuDateUpdated = DateTime.Parse("2021-03-22T06:00:00") },
        new() { Id = 23, MenuCategoryId = 23, MenuParentCategoryId = 1, MenuDisplayName = "Tättingar", MenuUrlSegment = "Tattingar", MenuDateUpdated = DateTime.Parse("2021-03-25T20:30:00") },
        new() { Id = 24, MenuCategoryId = 24, MenuParentCategoryId = 1, MenuDisplayName = "Hägrar, storkar och tranor", MenuUrlSegment = "Hagrar-storkar-tranor", MenuDateUpdated = DateTime.Parse("2021-03-25T22:06:00") },
        new() { Id = 25, MenuCategoryId = 25, MenuParentCategoryId = 1, MenuDisplayName = "Änder", MenuUrlSegment = "Ander", MenuDateUpdated = DateTime.Parse("2021-03-01T04:26:00") },
        new() { Id = 26, MenuCategoryId = 26, MenuParentCategoryId = 1, MenuDisplayName = "Doppingar och skarvar", MenuUrlSegment = "Doppingar-skarvar", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:00") },
        new() { Id = 27, MenuCategoryId = 27, MenuParentCategoryId = 1, MenuDisplayName = "Hackspettar", MenuUrlSegment = "Hackspettar", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:00") },
        new() { Id = 28, MenuCategoryId = 28, MenuParentCategoryId = 1, MenuDisplayName = "Måsar, trutar och tärnor", MenuUrlSegment = "Masar-trutar-tarnor", MenuDateUpdated = DateTime.Parse("2021-03-25T17:20:00") },
        new() { Id = 46, MenuCategoryId = 46, MenuParentCategoryId = 1, MenuDisplayName = "Gäss", MenuUrlSegment = "Gass", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:00") },
        new() { Id = 47, MenuCategoryId = 47, MenuParentCategoryId = 1, MenuDisplayName = "Trastar och duvor", MenuUrlSegment = "Trastar-och-duvor", MenuDateUpdated = DateTime.Parse("2021-03-01T05:33:00") },
        new() { Id = 48, MenuCategoryId = 48, MenuParentCategoryId = 1, MenuDisplayName = "Alkor och labbar", MenuUrlSegment = "Alkor-och-labbar", MenuDateUpdated = DateTime.Parse("2021-03-23T15:19:00") },
        
        // Sub-categories under Däggdjur (parent = 2)
        new() { Id = 11, MenuCategoryId = 11, MenuParentCategoryId = 2, MenuDisplayName = "Bäver", MenuUrlSegment = "Baver", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:00") },
        new() { Id = 12, MenuCategoryId = 12, MenuParentCategoryId = 2, MenuDisplayName = "Utter", MenuUrlSegment = "Utter", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:00") },
        new() { Id = 13, MenuCategoryId = 13, MenuParentCategoryId = 2, MenuDisplayName = "Myskoxe", MenuUrlSegment = "Myskoxe", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:00") },
        new() { Id = 14, MenuCategoryId = 14, MenuParentCategoryId = 2, MenuDisplayName = "Hare", MenuUrlSegment = "Hare", MenuDateUpdated = DateTime.Parse("2021-03-23T05:03:00") },
        new() { Id = 15, MenuCategoryId = 15, MenuParentCategoryId = 2, MenuDisplayName = "Älg", MenuUrlSegment = "Alg", MenuDateUpdated = DateTime.Parse("2021-03-19T09:39:00") },
        new() { Id = 16, MenuCategoryId = 16, MenuParentCategoryId = 2, MenuDisplayName = "Rådjur", MenuUrlSegment = "Radjur", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:00") },
        new() { Id = 17, MenuCategoryId = 17, MenuParentCategoryId = 2, MenuDisplayName = "Fjällräv", MenuUrlSegment = "Fjallrav", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:00") },
        new() { Id = 18, MenuCategoryId = 18, MenuParentCategoryId = 2, MenuDisplayName = "Räv", MenuUrlSegment = "Rav", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:00") },
        new() { Id = 19, MenuCategoryId = 19, MenuParentCategoryId = 2, MenuDisplayName = "Dovhjort", MenuUrlSegment = "Dovhjort", MenuDateUpdated = DateTime.Parse("2021-03-24T00:22:00") },
        
        // Sub-categories under Kräldjur (parent = 3)
        new() { Id = 29, MenuCategoryId = 29, MenuParentCategoryId = 3, MenuDisplayName = "Huggorm", MenuUrlSegment = "Huggorm", MenuDateUpdated = DateTime.Parse("2021-03-11T19:17:00") },
        new() { Id = 30, MenuCategoryId = 30, MenuParentCategoryId = 3, MenuDisplayName = "Snok", MenuUrlSegment = "Snok", MenuDateUpdated = DateTime.Parse("2021-03-05T13:32:00") },

        // Sub-Sub-categories under Fjärilar (parent = 4)
        new() { Id = 31, MenuCategoryId = 31, MenuParentCategoryId = 4, MenuDisplayName = "Nässelfjäril", MenuUrlSegment = "Nasselfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T06:45:00") },
        new() { Id = 32, MenuCategoryId = 32, MenuParentCategoryId = 4, MenuDisplayName = "Påfågelöga", MenuUrlSegment = "Pafageloga", MenuDateUpdated = DateTime.Parse("2021-03-01T06:25:00") },
        new() { Id = 33, MenuCategoryId = 33, MenuParentCategoryId = 4, MenuDisplayName = "Sorgmantel", MenuUrlSegment = "Sorgmantel", MenuDateUpdated = DateTime.Parse("2021-03-01T06:06:00") },
        new() { Id = 34, MenuCategoryId = 34, MenuParentCategoryId = 4, MenuDisplayName = "Aspfjäril", MenuUrlSegment = "Aspfjaril", MenuDateUpdated = DateTime.Parse("2021-03-16T07:24:00") },
        new() { Id = 50, MenuCategoryId = 50, MenuParentCategoryId = 4, MenuDisplayName = "Apollofjäril", MenuUrlSegment = "Apollofjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T06:23:00") },
        new() { Id = 57, MenuCategoryId = 57, MenuParentCategoryId = 4, MenuDisplayName = "Makaonfjäril", MenuUrlSegment = "Makaonfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T06:24:00") },


        // Sub-categories under Årstider (parent = 8)
        new() { Id = 38, MenuCategoryId = 38, MenuParentCategoryId = 8, MenuDisplayName = "Vår", MenuUrlSegment = "Var", MenuDateUpdated = DateTime.Parse("2021-03-23T04:58:00") },
        new() { Id = 39, MenuCategoryId = 39, MenuParentCategoryId = 8, MenuDisplayName = "Sommar", MenuUrlSegment = "Sommar", MenuDateUpdated = DateTime.Parse("2021-03-14T17:57:00") },
        new() { Id = 40, MenuCategoryId = 40, MenuParentCategoryId = 8, MenuDisplayName = "Höst", MenuUrlSegment = "Host", MenuDateUpdated = DateTime.Parse("2021-03-17T15:17:00") },
        new() { Id = 41, MenuCategoryId = 41, MenuParentCategoryId = 8, MenuDisplayName = "Vinter", MenuUrlSegment = "Vinter", MenuDateUpdated = DateTime.Parse("2021-03-16T07:37:00") },


        // Sub-categories under Landskap (parent = 7)
        new() { Id = 42, MenuCategoryId = 42, MenuParentCategoryId = 7, MenuDisplayName = "Oset och Rynningeviken", MenuUrlSegment = "Oset-och-rynningeviken", MenuDateUpdated = DateTime.Parse("2021-03-11T00:23:00") },
        new() { Id = 43, MenuCategoryId = 43, MenuParentCategoryId = 7, MenuDisplayName = "Kvismaren", MenuUrlSegment = "Kvismaren", MenuDateUpdated = DateTime.Parse("2021-03-12T21:17:00") },
        new() { Id = 44, MenuCategoryId = 44, MenuParentCategoryId = 7, MenuDisplayName = "Tysslingen", MenuUrlSegment = "Tysslingen", MenuDateUpdated = DateTime.Parse("2021-03-20T11:44:00") },

        // Sub-categories under Resor (parent = 9)
        new() { Id = 45, MenuCategoryId = 45, MenuParentCategoryId = 9, MenuDisplayName = "2010 Island", MenuUrlSegment = "2010-Island", MenuDateUpdated = DateTime.Parse("2021-03-17T08:48:00") },
        new() { Id = 197, MenuCategoryId = 202, MenuParentCategoryId = 9, MenuDisplayName = "2008 Costa Rica", MenuUrlSegment = "2008-Costa-Rica", MenuDateUpdated = DateTime.Parse("2021-03-17T15:29:00") },

            
        // Sub-categories under other parent categories
        new() { Id = 49, MenuCategoryId = 49, MenuParentCategoryId = 27, MenuDisplayName = "Tretåig hackspett", MenuUrlSegment = "Tretaig-hackspett", MenuDateUpdated = DateTime.Parse("2021-03-19T10:01:00") },
        new() { Id = 51, MenuCategoryId = 51, MenuParentCategoryId = 10, MenuDisplayName = "Aftonfalk", MenuUrlSegment = "Aftonfalk", MenuDateUpdated = DateTime.Parse("2021-03-19T09:30:00") },
        new() { Id = 52, MenuCategoryId = 52, MenuParentCategoryId = 10, MenuDisplayName = "Brun kärrhök", MenuUrlSegment = "Brun-karrhok", MenuDateUpdated = DateTime.Parse("2021-03-19T09:50:00") },
        new() { Id = 53, MenuCategoryId = 53, MenuParentCategoryId = 26, MenuDisplayName = "Gråhakedopping", MenuUrlSegment = "Grahakedopping", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:00") },
        new() { Id = 54, MenuCategoryId = 54, MenuParentCategoryId = 10, MenuDisplayName = "Havsörn", MenuUrlSegment = "Havsorn", MenuDateUpdated = DateTime.Parse("2021-03-23T13:25:00") },
        new() { Id = 55, MenuCategoryId = 55, MenuParentCategoryId = 2, MenuDisplayName = "Lodjur", MenuUrlSegment = "Lodjur", MenuDateUpdated = DateTime.Parse("2021-03-16T07:26:00") },
        new() { Id = 56, MenuCategoryId = 56, MenuParentCategoryId = 10, MenuDisplayName = "Lärkfalk", MenuUrlSegment = "Larkfalk", MenuDateUpdated = DateTime.Parse("2021-03-19T09:31:00") },
        new() { Id = 58, MenuCategoryId = 58, MenuParentCategoryId = 27, MenuDisplayName = "Mindre hackspett", MenuUrlSegment = "Mindre-hackspett", MenuDateUpdated = DateTime.Parse("2021-03-19T09:50:00") },
        new() { Id = 59, MenuCategoryId = 59, MenuParentCategoryId = 25, MenuDisplayName = "Skedand", MenuUrlSegment = "Skedand", MenuDateUpdated = DateTime.Parse("2021-03-19T07:52:00") },
        new() { Id = 60, MenuCategoryId = 60, MenuParentCategoryId = 25, MenuDisplayName = "Snatterand", MenuUrlSegment = "Snatterand", MenuDateUpdated = DateTime.Parse("2021-03-19T09:20:00") },
        new() { Id = 61, MenuCategoryId = 62, MenuParentCategoryId = 26, MenuDisplayName = "Svarthakedopping", MenuUrlSegment = "Svarthakedopping", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:00") },
        new() { Id = 62, MenuCategoryId = 63, MenuParentCategoryId = 25, MenuDisplayName = "Årta", MenuUrlSegment = "Arta", MenuDateUpdated = DateTime.Parse("2021-03-01T05:16:00") },
        new() { Id = 63, MenuCategoryId = 64, MenuParentCategoryId = 214, MenuDisplayName = "Blåhake", MenuUrlSegment = "Blahake", MenuDateUpdated = DateTime.Parse("2021-03-24T23:00:00") },
        new() { Id = 64, MenuCategoryId = 65, MenuParentCategoryId = 204, MenuDisplayName = "Domherre", MenuUrlSegment = "Domherre", MenuDateUpdated = DateTime.Parse("2021-03-01T09:19:00") },
        new() { Id = 65, MenuCategoryId = 66, MenuParentCategoryId = 204, MenuDisplayName = "Gråsiska", MenuUrlSegment = "Grasiska", MenuDateUpdated = DateTime.Parse("2021-03-25T16:52:00") },
        new() { Id = 66, MenuCategoryId = 67, MenuParentCategoryId = 210, MenuDisplayName = "Lappsparv", MenuUrlSegment = "Lappsparv", MenuDateUpdated = DateTime.Parse("2021-03-19T09:52:00") },
        new() { Id = 67, MenuCategoryId = 68, MenuParentCategoryId = 214, MenuDisplayName = "Näktergal", MenuUrlSegment = "Naktergal", MenuDateUpdated = DateTime.Parse("2021-03-19T09:59:00") },
        new() { Id = 68, MenuCategoryId = 69, MenuParentCategoryId = 204, MenuDisplayName = "Steglits", MenuUrlSegment = "Steglits", MenuDateUpdated = DateTime.Parse("2021-03-01T09:20:00") },
        new() { Id = 69, MenuCategoryId = 70, MenuParentCategoryId = 204, MenuDisplayName = "Stenknäck", MenuUrlSegment = "Stenknack", MenuDateUpdated = DateTime.Parse("2021-03-25T16:50:00") },
        new() { Id = 70, MenuCategoryId = 71, MenuParentCategoryId = 204, MenuDisplayName = "Vinterhämpling", MenuUrlSegment = "Vinterhampling", MenuDateUpdated = DateTime.Parse("2021-03-03T13:02:00") },
        new() { Id = 71, MenuCategoryId = 72, MenuParentCategoryId = 2, MenuDisplayName = "Ekorre", MenuUrlSegment = "Ekorre", MenuDateUpdated = DateTime.Parse("2021-03-23T21:06:00") },
        new() { Id = 72, MenuCategoryId = 73, MenuParentCategoryId = 273, MenuDisplayName = "Fjällpipare", MenuUrlSegment = "Fjallpipare", MenuDateUpdated = DateTime.Parse("2021-03-19T13:56:00") },
        new() { Id = 73, MenuCategoryId = 74, MenuParentCategoryId = 288, MenuDisplayName = "Kärrsnäppa", MenuUrlSegment = "Karrsnappa", MenuDateUpdated = DateTime.Parse("2021-03-19T09:55:00") },
        new() { Id = 74, MenuCategoryId = 75, MenuParentCategoryId = 22, MenuDisplayName = "Roskarl", MenuUrlSegment = "Roskarl", MenuDateUpdated = DateTime.Parse("2021-03-01T05:20:00") },
        new() { Id = 75, MenuCategoryId = 76, MenuParentCategoryId = 274, MenuDisplayName = "Rödspov", MenuUrlSegment = "Rodspov", MenuDateUpdated = DateTime.Parse("2021-03-19T13:56:00") },
        new() { Id = 76, MenuCategoryId = 77, MenuParentCategoryId = 22, MenuDisplayName = "Skärfläcka", MenuUrlSegment = "Skarflacka", MenuDateUpdated = DateTime.Parse("2021-03-19T09:22:00") },
        new() { Id = 77, MenuCategoryId = 78, MenuParentCategoryId = 288, MenuDisplayName = "Skärsnäppa", MenuUrlSegment = "Skarsnappa", MenuDateUpdated = DateTime.Parse("2021-03-01T07:20:00") },
        new() { Id = 78, MenuCategoryId = 79, MenuParentCategoryId = 274, MenuDisplayName = "Storspov", MenuUrlSegment = "Storspov", MenuDateUpdated = DateTime.Parse("2021-03-19T09:30:00") },
        new() { Id = 79, MenuCategoryId = 80, MenuParentCategoryId = 22, MenuDisplayName = "Tofsvipa", MenuUrlSegment = "Tofsvipa", MenuDateUpdated = DateTime.Parse("2021-03-23T21:27:00") },
        new() { Id = 80, MenuCategoryId = 81, MenuParentCategoryId = 1, MenuDisplayName = "Ugglor", MenuUrlSegment = "Ugglor", MenuDateUpdated = DateTime.Parse("2021-03-01T04:26:00") },
        new() { Id = 81, MenuCategoryId = 82, MenuParentCategoryId = 81, MenuDisplayName = "Berguv", MenuUrlSegment = "Berguv", MenuDateUpdated = DateTime.Parse("2021-03-20T18:34:00") },
        new() { Id = 82, MenuCategoryId = 83, MenuParentCategoryId = 81, MenuDisplayName = "Hornuggla", MenuUrlSegment = "Hornuggla", MenuDateUpdated = DateTime.Parse("2021-03-19T07:57:00") },
        new() { Id = 83, MenuCategoryId = 84, MenuParentCategoryId = 81, MenuDisplayName = "Hökuggla", MenuUrlSegment = "Hokuggla", MenuDateUpdated = DateTime.Parse("2021-03-01T05:29:00") },
        new() { Id = 84, MenuCategoryId = 85, MenuParentCategoryId = 81, MenuDisplayName = "Jorduggla", MenuUrlSegment = "Jorduggla", MenuDateUpdated = DateTime.Parse("2021-03-19T07:58:00") },
        new() { Id = 85, MenuCategoryId = 86, MenuParentCategoryId = 81, MenuDisplayName = "Kattuggla", MenuUrlSegment = "Kattuggla", MenuDateUpdated = DateTime.Parse("2021-03-04T10:44:00") },
        new() { Id = 86, MenuCategoryId = 87, MenuParentCategoryId = 81, MenuDisplayName = "Lappuggla", MenuUrlSegment = "Lappuggla", MenuDateUpdated = DateTime.Parse("2021-03-25T21:23:00") },
        new() { Id = 87, MenuCategoryId = 88, MenuParentCategoryId = 81, MenuDisplayName = "Pärluggla", MenuUrlSegment = "Parluggla", MenuDateUpdated = DateTime.Parse("2021-03-01T05:34:00") },
        new() { Id = 88, MenuCategoryId = 89, MenuParentCategoryId = 81, MenuDisplayName = "Slaguggla", MenuUrlSegment = "Slaguggla", MenuDateUpdated = DateTime.Parse("2021-03-23T21:49:00") },
        new() { Id = 89, MenuCategoryId = 90, MenuParentCategoryId = 81, MenuDisplayName = "Sparvuggla", MenuUrlSegment = "Sparvuggla", MenuDateUpdated = DateTime.Parse("2021-03-16T07:45:00") },
        new() { Id = 90, MenuCategoryId = 91, MenuParentCategoryId = 20, MenuDisplayName = "Knölsvan", MenuUrlSegment = "Knolsvan", MenuDateUpdated = DateTime.Parse("2021-03-22T22:55:00") },
        new() { Id = 91, MenuCategoryId = 92, MenuParentCategoryId = 20, MenuDisplayName = "Mindre Sångsvan", MenuUrlSegment = "Mindre-Sangsvan", MenuDateUpdated = DateTime.Parse("2021-03-19T09:34:00") },
        new() { Id = 92, MenuCategoryId = 93, MenuParentCategoryId = 20, MenuDisplayName = "Sångsvan", MenuUrlSegment = "Sangsvan", MenuDateUpdated = DateTime.Parse("2021-03-25T20:28:00") },
        new() { Id = 93, MenuCategoryId = 94, MenuParentCategoryId = 21, MenuDisplayName = "Fasan", MenuUrlSegment = "Fasan", MenuDateUpdated = DateTime.Parse("2021-03-19T09:46:00") },
        new() { Id = 94, MenuCategoryId = 95, MenuParentCategoryId = 21, MenuDisplayName = "Orre", MenuUrlSegment = "Orre", MenuDateUpdated = DateTime.Parse("2021-03-01T06:48:00") },
        new() { Id = 95, MenuCategoryId = 96, MenuParentCategoryId = 21, MenuDisplayName = "Rapphöna", MenuUrlSegment = "Rapphona", MenuDateUpdated = DateTime.Parse("2021-03-19T08:31:00") },
        new() { Id = 96, MenuCategoryId = 97, MenuParentCategoryId = 21, MenuDisplayName = "Tjäder", MenuUrlSegment = "Tjader", MenuDateUpdated = DateTime.Parse("2021-03-19T09:56:00") },
        new() { Id = 97, MenuCategoryId = 98, MenuParentCategoryId = 25, MenuDisplayName = "Alfågel", MenuUrlSegment = "Alfagel", MenuDateUpdated = DateTime.Parse("2021-03-19T07:55:00") },
        new() { Id = 98, MenuCategoryId = 99, MenuParentCategoryId = 25, MenuDisplayName = "Bläsand", MenuUrlSegment = "Blasand", MenuDateUpdated = DateTime.Parse("2021-03-19T07:56:00") },
        new() { Id = 99, MenuCategoryId = 100, MenuParentCategoryId = 25, MenuDisplayName = "Ejder", MenuUrlSegment = "Ejder", MenuDateUpdated = DateTime.Parse("2021-03-01T09:36:00") },
        new() { Id = 100, MenuCategoryId = 101, MenuParentCategoryId = 25, MenuDisplayName = "Gräsand", MenuUrlSegment = "Grasand", MenuDateUpdated = DateTime.Parse("2021-03-19T07:56:00") },
        new() { Id = 101, MenuCategoryId = 102, MenuParentCategoryId = 25, MenuDisplayName = "Knipa", MenuUrlSegment = "Knipa", MenuDateUpdated = DateTime.Parse("2021-03-01T09:37:00") },
        new() { Id = 102, MenuCategoryId = 103, MenuParentCategoryId = 25, MenuDisplayName = "Kricka", MenuUrlSegment = "Kricka", MenuDateUpdated = DateTime.Parse("2021-03-19T07:49:00") },
        new() { Id = 103, MenuCategoryId = 104, MenuParentCategoryId = 25, MenuDisplayName = "Mandarinand", MenuUrlSegment = "Mandarinand", MenuDateUpdated = DateTime.Parse("2021-03-19T09:21:00") },
        new() { Id = 104, MenuCategoryId = 105, MenuParentCategoryId = 25, MenuDisplayName = "Praktejder", MenuUrlSegment = "Praktejder", MenuDateUpdated = DateTime.Parse("2021-03-23T21:10:00") },
        new() { Id = 105, MenuCategoryId = 106, MenuParentCategoryId = 25, MenuDisplayName = "Salskrake", MenuUrlSegment = "Salskrake", MenuDateUpdated = DateTime.Parse("2021-03-19T09:18:00") },
        new() { Id = 106, MenuCategoryId = 107, MenuParentCategoryId = 25, MenuDisplayName = "Storskrake", MenuUrlSegment = "Storskrake", MenuDateUpdated = DateTime.Parse("2021-03-22T15:53:00") },
        new() { Id = 107, MenuCategoryId = 108, MenuParentCategoryId = 46, MenuDisplayName = "Bläsgås", MenuUrlSegment = "Blasgas", MenuDateUpdated = DateTime.Parse("2021-03-19T07:52:00") },
        new() { Id = 108, MenuCategoryId = 109, MenuParentCategoryId = 46, MenuDisplayName = "Grågås", MenuUrlSegment = "Gragas", MenuDateUpdated = DateTime.Parse("2021-03-24T00:22:00") },
        new() { Id = 109, MenuCategoryId = 110, MenuParentCategoryId = 46, MenuDisplayName = "Kanadagås", MenuUrlSegment = "Kanadagas", MenuDateUpdated = DateTime.Parse("2021-03-19T07:57:00") },
        new() { Id = 110, MenuCategoryId = 111, MenuParentCategoryId = 46, MenuDisplayName = "Prutgås", MenuUrlSegment = "Prutgas", MenuDateUpdated = DateTime.Parse("2021-03-19T07:50:00") },
        new() { Id = 111, MenuCategoryId = 112, MenuParentCategoryId = 46, MenuDisplayName = "Sädgås", MenuUrlSegment = "Sadgas", MenuDateUpdated = DateTime.Parse("2021-03-19T13:55:00") },
        new() { Id = 112, MenuCategoryId = 113, MenuParentCategoryId = 46, MenuDisplayName = "Vitkindad gås", MenuUrlSegment = "Vitkindad-gas", MenuDateUpdated = DateTime.Parse("2021-03-19T09:23:00") },
        new() { Id = 113, MenuCategoryId = 114, MenuParentCategoryId = 1, MenuDisplayName = "Kråkfåglar", MenuUrlSegment = "Krakfaglar", MenuDateUpdated = DateTime.Parse("2021-03-01T05:15:00") },
        new() { Id = 114, MenuCategoryId = 115, MenuParentCategoryId = 114, MenuDisplayName = "Kaja", MenuUrlSegment = "Kaja", MenuDateUpdated = DateTime.Parse("2021-03-19T09:19:00") },
        new() { Id = 115, MenuCategoryId = 116, MenuParentCategoryId = 114, MenuDisplayName = "Kråka", MenuUrlSegment = "Kraka", MenuDateUpdated = DateTime.Parse("2021-03-23T18:10:00") },
        new() { Id = 116, MenuCategoryId = 117, MenuParentCategoryId = 114, MenuDisplayName = "Lavskrika", MenuUrlSegment = "Lavskrika", MenuDateUpdated = DateTime.Parse("2021-03-19T09:30:00") },
        new() { Id = 117, MenuCategoryId = 118, MenuParentCategoryId = 114, MenuDisplayName = "Nötkråka", MenuUrlSegment = "Notkraka", MenuDateUpdated = DateTime.Parse("2021-03-19T09:32:00") },
        new() { Id = 118, MenuCategoryId = 119, MenuParentCategoryId = 114, MenuDisplayName = "Nötskrika", MenuUrlSegment = "Notskrika", MenuDateUpdated = DateTime.Parse("2021-03-19T09:33:00") },
        new() { Id = 119, MenuCategoryId = 120, MenuParentCategoryId = 114, MenuDisplayName = "Råka", MenuUrlSegment = "Raka", MenuDateUpdated = DateTime.Parse("2021-03-10T08:31:00") },
        new() { Id = 120, MenuCategoryId = 121, MenuParentCategoryId = 114, MenuDisplayName = "Skata", MenuUrlSegment = "Skata", MenuDateUpdated = DateTime.Parse("2021-03-19T09:20:00") },
        new() { Id = 121, MenuCategoryId = 122, MenuParentCategoryId = 2, MenuDisplayName = "Igelkott", MenuUrlSegment = "Igelkott", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:00") },
        new() { Id = 122, MenuCategoryId = 123, MenuParentCategoryId = 2, MenuDisplayName = "Björn", MenuUrlSegment = "Bjorn", MenuDateUpdated = DateTime.Parse("2021-03-17T14:38:00") },
        new() { Id = 123, MenuCategoryId = 124, MenuParentCategoryId = 2, MenuDisplayName = "Vildren", MenuUrlSegment = "Vildren", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:00") },
        new() { Id = 124, MenuCategoryId = 125, MenuParentCategoryId = 2, MenuDisplayName = "Knubbsäl", MenuUrlSegment = "Knubbsal", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:00") },
        new() { Id = 125, MenuCategoryId = 126, MenuParentCategoryId = 24, MenuDisplayName = "Vit stork", MenuUrlSegment = "Vit-stork", MenuDateUpdated = DateTime.Parse("2021-03-01T09:17:00") },
        new() { Id = 126, MenuCategoryId = 127, MenuParentCategoryId = 28, MenuDisplayName = "Dvärgmås", MenuUrlSegment = "Dvargmas", MenuDateUpdated = DateTime.Parse("2021-03-01T09:17:00") },
        new() { Id = 127, MenuCategoryId = 128, MenuParentCategoryId = 28, MenuDisplayName = "Fisktärna", MenuUrlSegment = "Fisktarna", MenuDateUpdated = DateTime.Parse("2021-03-01T09:18:00") },
        new() { Id = 128, MenuCategoryId = 129, MenuParentCategoryId = 28, MenuDisplayName = "Skrattmås", MenuUrlSegment = "Skrattmas", MenuDateUpdated = DateTime.Parse("2021-03-20T18:34:00") },
        new() { Id = 129, MenuCategoryId = 130, MenuParentCategoryId = 28, MenuDisplayName = "Småtärna", MenuUrlSegment = "Smatarna", MenuDateUpdated = DateTime.Parse("2021-03-01T09:18:00") },
        new() { Id = 130, MenuCategoryId = 131, MenuParentCategoryId = 24, MenuDisplayName = "Häger", MenuUrlSegment = "Hager", MenuDateUpdated = DateTime.Parse("2021-03-01T08:32:00") },
        new() { Id = 131, MenuCategoryId = 132, MenuParentCategoryId = 24, MenuDisplayName = "Trana", MenuUrlSegment = "Trana", MenuDateUpdated = DateTime.Parse("2021-03-20T18:33:00") },
        new() { Id = 132, MenuCategoryId = 133, MenuParentCategoryId = 3, MenuDisplayName = "Padda", MenuUrlSegment = "Padda", MenuDateUpdated = DateTime.Parse("2021-03-25T18:36:00") },
        new() { Id = 133, MenuCategoryId = 134, MenuParentCategoryId = 3, MenuDisplayName = "Åkergroda", MenuUrlSegment = "Akergroda", MenuDateUpdated = DateTime.Parse("2021-03-05T13:32:00") },
        new() { Id = 134, MenuCategoryId = 135, MenuParentCategoryId = 47, MenuDisplayName = "Björktrast", MenuUrlSegment = "Bjorktrast", MenuDateUpdated = DateTime.Parse("2021-03-01T07:26:00") },
        new() { Id = 135, MenuCategoryId = 136, MenuParentCategoryId = 47, MenuDisplayName = "Rödvingetrast", MenuUrlSegment = "Rodvingetrast", MenuDateUpdated = DateTime.Parse("2021-03-01T08:17:00") },
        new() { Id = 136, MenuCategoryId = 137, MenuParentCategoryId = 214, MenuDisplayName = "Sidensvans", MenuUrlSegment = "Sidensvans", MenuDateUpdated = DateTime.Parse("2021-03-19T10:00:00") },
        new() { Id = 137, MenuCategoryId = 138, MenuParentCategoryId = 214, MenuDisplayName = "Stare", MenuUrlSegment = "Stare", MenuDateUpdated = DateTime.Parse("2021-03-19T09:37:00") },
        new() { Id = 138, MenuCategoryId = 139, MenuParentCategoryId = 28, MenuDisplayName = "Gråtrut", MenuUrlSegment = "Gratrut", MenuDateUpdated = DateTime.Parse("2021-03-01T08:36:00") },
        new() { Id = 139, MenuCategoryId = 140, MenuParentCategoryId = 4, MenuDisplayName = "Almsnabbvinge", MenuUrlSegment = "Almsnabbvinge", MenuDateUpdated = DateTime.Parse("2021-03-01T06:23:00") },
        new() { Id = 140, MenuCategoryId = 142, MenuParentCategoryId = 4, MenuDisplayName = "Asknätfjäril", MenuUrlSegment = "Asknatfjaril", MenuDateUpdated = DateTime.Parse("2021-03-10T08:32:00") },
        new() { Id = 141, MenuCategoryId = 143, MenuParentCategoryId = 4, MenuDisplayName = "Aurorafjäril", MenuUrlSegment = "Aurorafjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T06:23:00") },
        new() { Id = 142, MenuCategoryId = 144, MenuParentCategoryId = 4, MenuDisplayName = "Bredbrämad bastardsvärmare", MenuUrlSegment = "Bredbramad-bastardsvarmare", MenuDateUpdated = DateTime.Parse("2021-03-01T09:27:00") },
        new() { Id = 143, MenuCategoryId = 145, MenuParentCategoryId = 4, MenuDisplayName = "Brunfläckig pärlemorfjäril", MenuUrlSegment = "Brunflackig-parlemorfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T09:29:00") },
        new() { Id = 144, MenuCategoryId = 146, MenuParentCategoryId = 4, MenuDisplayName = "Citronfjäril", MenuUrlSegment = "Citronfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T06:24:00") },
        new() { Id = 145, MenuCategoryId = 147, MenuParentCategoryId = 4, MenuDisplayName = "Dårgräsfjäril", MenuUrlSegment = "Dargrasfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T07:06:00") },
        new() { Id = 146, MenuCategoryId = 148, MenuParentCategoryId = 4, MenuDisplayName = "Eldsnabbvinge", MenuUrlSegment = "Eldsnabbvinge", MenuDateUpdated = DateTime.Parse("2021-03-01T06:24:00") },
        new() { Id = 147, MenuCategoryId = 149, MenuParentCategoryId = 4, MenuDisplayName = "Gullvivefjäril", MenuUrlSegment = "Gullvivefjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T06:55:00") },
        new() { Id = 148, MenuCategoryId = 150, MenuParentCategoryId = 4, MenuDisplayName = "Klubbsprötad bastardsvärmare", MenuUrlSegment = "Klubbsprotad-bastardsvarmare", MenuDateUpdated = DateTime.Parse("2021-03-19T13:56:00") },
        new() { Id = 149, MenuCategoryId = 151, MenuParentCategoryId = 4, MenuDisplayName = "Kvickgräsfjäril", MenuUrlSegment = "Kvickgrasfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T07:22:00") },
        new() { Id = 150, MenuCategoryId = 152, MenuParentCategoryId = 4, MenuDisplayName = "Pärlgräsfjäril", MenuUrlSegment = "Parlgrasfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T07:22:00") },
        new() { Id = 151, MenuCategoryId = 153, MenuParentCategoryId = 4, MenuDisplayName = "Rapsfjäril", MenuUrlSegment = "Rapsfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T06:11:00") },
        new() { Id = 152, MenuCategoryId = 154, MenuParentCategoryId = 4, MenuDisplayName = "Rovfjäril", MenuUrlSegment = "Rovfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T06:06:00") },
        new() { Id = 153, MenuCategoryId = 155, MenuParentCategoryId = 4, MenuDisplayName = "Sandgräsfjäril", MenuUrlSegment = "Sandgrasfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T07:08:00") },
        new() { Id = 154, MenuCategoryId = 156, MenuParentCategoryId = 4, MenuDisplayName = "Sexfläckig bastardsvärmare", MenuUrlSegment = "Sexflackig-bastardsvarmare", MenuDateUpdated = DateTime.Parse("2021-03-01T09:27:00") },
        new() { Id = 155, MenuCategoryId = 157, MenuParentCategoryId = 4, MenuDisplayName = "Silverblåvinge", MenuUrlSegment = "Silverblavinge", MenuDateUpdated = DateTime.Parse("2021-03-01T06:57:00") },
        new() { Id = 156, MenuCategoryId = 158, MenuParentCategoryId = 4, MenuDisplayName = "Silverstreckad pärlemorfjäril", MenuUrlSegment = "Silverstreckad-parlemorfjaril", MenuDateUpdated = DateTime.Parse("2021-03-05T22:05:00") },
        new() { Id = 157, MenuCategoryId = 159, MenuParentCategoryId = 4, MenuDisplayName = "Skogsgräsfjäril", MenuUrlSegment = "Skogsgrasfjaril", MenuDateUpdated = DateTime.Parse("2021-03-19T08:31:00") },
        new() { Id = 158, MenuCategoryId = 160, MenuParentCategoryId = 4, MenuDisplayName = "Skogsnätfjäril", MenuUrlSegment = "Skogsnatfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T07:09:00") },
        new() { Id = 159, MenuCategoryId = 161, MenuParentCategoryId = 4, MenuDisplayName = "Skogspärlemorfjäril", MenuUrlSegment = "Skogsparlemorfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T08:24:00") },
        new() { Id = 160, MenuCategoryId = 162, MenuParentCategoryId = 4, MenuDisplayName = "Skogsvitvinge", MenuUrlSegment = "Skogsvitvinge", MenuDateUpdated = DateTime.Parse("2021-03-01T06:26:00") },
        new() { Id = 161, MenuCategoryId = 163, MenuParentCategoryId = 4, MenuDisplayName = "Smultronvisslare", MenuUrlSegment = "Smultronvisslare", MenuDateUpdated = DateTime.Parse("2021-03-19T13:55:00") },
        new() { Id = 162, MenuCategoryId = 164, MenuParentCategoryId = 4, MenuDisplayName = "Sotnätfjäril", MenuUrlSegment = "Sotnatfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T06:45:00") },
        new() { Id = 163, MenuCategoryId = 165, MenuParentCategoryId = 4, MenuDisplayName = "Svartfläckig blåvinge", MenuUrlSegment = "Svartflackig-blavinge", MenuDateUpdated = DateTime.Parse("2021-03-01T08:34:00") },
        new() { Id = 164, MenuCategoryId = 166, MenuParentCategoryId = 4, MenuDisplayName = "Svavelgul höfjäril", MenuUrlSegment = "Svavelgul-hofjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T08:16:00") },
        new() { Id = 165, MenuCategoryId = 167, MenuParentCategoryId = 4, MenuDisplayName = "Tistelfjäril", MenuUrlSegment = "Tistelfjaril", MenuDateUpdated = DateTime.Parse("2021-03-07T16:27:00") },
        new() { Id = 166, MenuCategoryId = 168, MenuParentCategoryId = 4, MenuDisplayName = "Vinbärsfux", MenuUrlSegment = "Vinbarsfux", MenuDateUpdated = DateTime.Parse("2021-03-01T06:11:00") },
        new() { Id = 167, MenuCategoryId = 169, MenuParentCategoryId = 4, MenuDisplayName = "Väddnätfjäril", MenuUrlSegment = "Vaddnatfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T07:11:00") },
        new() { Id = 168, MenuCategoryId = 170, MenuParentCategoryId = 4, MenuDisplayName = "Älggräspärlemorfjäril", MenuUrlSegment = "Alggrasparlemorfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T09:16:00") },
        new() { Id = 169, MenuCategoryId = 171, MenuParentCategoryId = 4, MenuDisplayName = "Ängspärlemorfjäril", MenuUrlSegment = "Angsparlemorfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T08:25:00") },
        new() { Id = 170, MenuCategoryId = 172, MenuParentCategoryId = 36, MenuDisplayName = "Fyrfläckig trollslända", MenuUrlSegment = "Fyrflackig-trollslanda", MenuDateUpdated = DateTime.Parse("2021-03-01T08:35:00") },
        new() { Id = 171, MenuCategoryId = 173, MenuParentCategoryId = 36, MenuDisplayName = "Kungstrollslända", MenuUrlSegment = "Kungstrollslanda", MenuDateUpdated = DateTime.Parse("2021-03-19T13:56:00") },
        new() { Id = 172, MenuCategoryId = 177, MenuParentCategoryId = 48, MenuDisplayName = "Fjällabb", MenuUrlSegment = "Fjallabb", MenuDateUpdated = DateTime.Parse("2021-03-23T15:19:00") },
        new() { Id = 173, MenuCategoryId = 178, MenuParentCategoryId = 48, MenuDisplayName = "Kustlabb", MenuUrlSegment = "Kustlabb", MenuDateUpdated = DateTime.Parse("2021-03-23T15:19:00") },
        new() { Id = 174, MenuCategoryId = 179, MenuParentCategoryId = 48, MenuDisplayName = "Lunnefågel", MenuUrlSegment = "Lunnefagel", MenuDateUpdated = DateTime.Parse("2021-03-23T15:19:00") },
        new() { Id = 175, MenuCategoryId = 180, MenuParentCategoryId = 48, MenuDisplayName = "Sillgrissla", MenuUrlSegment = "Sillgrissla", MenuDateUpdated = DateTime.Parse("2021-03-23T15:19:00") },
        new() { Id = 176, MenuCategoryId = 181, MenuParentCategoryId = 48, MenuDisplayName = "Storlabb", MenuUrlSegment = "Storlabb", MenuDateUpdated = DateTime.Parse("2021-03-19T09:40:00") },
        new() { Id = 177, MenuCategoryId = 182, MenuParentCategoryId = 48, MenuDisplayName = "Tobisgrissla", MenuUrlSegment = "Tobisgrissla", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:00") },
        new() { Id = 178, MenuCategoryId = 183, MenuParentCategoryId = 48, MenuDisplayName = "Tordmule", MenuUrlSegment = "Tordmule", MenuDateUpdated = DateTime.Parse("2021-03-19T09:40:00") },
        new() { Id = 179, MenuCategoryId = 184, MenuParentCategoryId = 26, MenuDisplayName = "Skäggdopping", MenuUrlSegment = "Skaggdopping", MenuDateUpdated = DateTime.Parse("2021-03-23T19:32:00") },
        new() { Id = 180, MenuCategoryId = 185, MenuParentCategoryId = 26, MenuDisplayName = "Storskarv", MenuUrlSegment = "Storskarv", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:00") },
        new() { Id = 181, MenuCategoryId = 186, MenuParentCategoryId = 26, MenuDisplayName = "Toppskarv", MenuUrlSegment = "Toppskarv", MenuDateUpdated = DateTime.Parse("2021-03-19T13:55:00") },
        new() { Id = 182, MenuCategoryId = 187, MenuParentCategoryId = 47, MenuDisplayName = "Ringduva", MenuUrlSegment = "Ringduva", MenuDateUpdated = DateTime.Parse("2021-03-19T09:44:00") },
        new() { Id = 183, MenuCategoryId = 188, MenuParentCategoryId = 47, MenuDisplayName = "Större turturduva", MenuUrlSegment = "Storre-turturduva", MenuDateUpdated = DateTime.Parse("2021-03-01T08:40:00") },
        new() { Id = 184, MenuCategoryId = 189, MenuParentCategoryId = 47, MenuDisplayName = "Tamduva", MenuUrlSegment = "Tamduva", MenuDateUpdated = DateTime.Parse("2021-03-19T09:42:00") },
        new() { Id = 185, MenuCategoryId = 190, MenuParentCategoryId = 47, MenuDisplayName = "Turkduva", MenuUrlSegment = "Turkduva", MenuDateUpdated = DateTime.Parse("2021-03-01T06:50:00") },
        new() { Id = 186, MenuCategoryId = 191, MenuParentCategoryId = 1, MenuDisplayName = "Lommar", MenuUrlSegment = "Lommar", MenuDateUpdated = DateTime.Parse("2021-03-16T07:23:00") },
        new() { Id = 187, MenuCategoryId = 192, MenuParentCategoryId = 191, MenuDisplayName = "Smålom", MenuUrlSegment = "Smalom", MenuDateUpdated = DateTime.Parse("2021-03-02T10:20:00") },
        new() { Id = 188, MenuCategoryId = 193, MenuParentCategoryId = 10, MenuDisplayName = "Duvhök", MenuUrlSegment = "Duvhok", MenuDateUpdated = DateTime.Parse("2021-03-19T09:24:00") },
        new() { Id = 189, MenuCategoryId = 194, MenuParentCategoryId = 10, MenuDisplayName = "Fjällvråk", MenuUrlSegment = "Fjallvrak", MenuDateUpdated = DateTime.Parse("2021-03-19T09:41:00") },
        new() { Id = 190, MenuCategoryId = 195, MenuParentCategoryId = 10, MenuDisplayName = "Röd glada", MenuUrlSegment = "Rod-glada", MenuDateUpdated = DateTime.Parse("2021-03-23T21:53:00") },
        new() { Id = 191, MenuCategoryId = 196, MenuParentCategoryId = 10, MenuDisplayName = "Jaktfalk", MenuUrlSegment = "Jaktfalk", MenuDateUpdated = DateTime.Parse("2021-03-23T21:33:00") },
        new() { Id = 192, MenuCategoryId = 197, MenuParentCategoryId = 10, MenuDisplayName = "Kungsörn", MenuUrlSegment = "Kungsorn", MenuDateUpdated = DateTime.Parse("2021-03-23T13:26:00") },
        new() { Id = 193, MenuCategoryId = 198, MenuParentCategoryId = 10, MenuDisplayName = "Ormvråk", MenuUrlSegment = "Ormvrak", MenuDateUpdated = DateTime.Parse("2021-03-19T09:28:00") },
        new() { Id = 194, MenuCategoryId = 199, MenuParentCategoryId = 10, MenuDisplayName = "Pilgrimsfalk", MenuUrlSegment = "Pilgrimsfalk", MenuDateUpdated = DateTime.Parse("2021-03-23T21:52:00") },
        new() { Id = 195, MenuCategoryId = 200, MenuParentCategoryId = 10, MenuDisplayName = "Sparvhök", MenuUrlSegment = "Sparvhok", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:00") },
        new() { Id = 196, MenuCategoryId = 201, MenuParentCategoryId = 10, MenuDisplayName = "Tornfalk", MenuUrlSegment = "Tornfalk", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:00") },
        new() { Id = 198, MenuCategoryId = 203, MenuParentCategoryId = 22, MenuDisplayName = "Vattenrall", MenuUrlSegment = "Vattenrall", MenuDateUpdated = DateTime.Parse("2021-03-19T09:19:00") },
        new() { Id = 199, MenuCategoryId = 204, MenuParentCategoryId = 23, MenuDisplayName = "Finkar och siskor", MenuUrlSegment = "Finkar-Siskor", MenuDateUpdated = DateTime.Parse("2021-03-04T08:03:00") },
        new() { Id = 200, MenuCategoryId = 205, MenuParentCategoryId = 23, MenuDisplayName = "Flugsnappare", MenuUrlSegment = "Flugsnappare", MenuDateUpdated = DateTime.Parse("2021-03-01T21:25:00") },
    };

    #endregion

    #region PageCounters data

    /// <summary>
    /// Page counters seed data
    /// </summary>
    /// <remarks>
    /// <code>
    /// -- ===========================================
    /// -- Export TblPageCounter(100 rows)
    /// -- ===========================================
    /// SELECT 
    /// '    new() { ' +
    /// 'Id = ' + (CASE WHEN [PageCounter_ID] IS NULL THEN 'null' ELSE CAST([PageCounter_ID] AS NVARCHAR(10)) END) + ', ' +
    /// 'CategoryId = ' + (CASE WHEN [PageCounter_CategoryId] IS NULL THEN '0' ELSE CAST([PageCounter_CategoryId] AS NVARCHAR(10)) END) + ', ' +
    /// 'PicturePage = ' + (CASE WHEN [PageCounter_PicturePage] = 1 THEN 'true' ELSE 'false' END) + ', ' +
    /// 'PageName = "' + REPLACE(ISNULL([PageCounter_Name], ''), '"', '\"') + '", ' +
    /// 'MonthViewed = "' + REPLACE(ISNULL([PageCounter_MonthViewed], ''), '"', '\"') + '", ' +
    /// 'LastShowDate = new DateTime(' + 
    /// CAST(YEAR(ISNULL([PageCounter_LastShowDate], GETDATE())) AS NVARCHAR(4)) + ', ' +
    /// CAST(MONTH(ISNULL([PageCounter_LastShowDate], GETDATE())) AS NVARCHAR(2)) + ', ' +
    /// CAST(DAY(ISNULL([PageCounter_LastShowDate], GETDATE())) AS NVARCHAR(2)) + ', ' +
    /// CAST(DATEPART(HOUR, ISNULL([PageCounter_LastShowDate], GETDATE())) AS NVARCHAR(2)) + ', ' +
    /// CAST(DATEPART(MINUTE, ISNULL([PageCounter_LastShowDate], GETDATE())) AS NVARCHAR(2)) + ', ' +
    /// CAST(DATEPART(SECOND, ISNULL([PageCounter_LastShowDate], GETDATE())) AS NVARCHAR(2)) + '), ' +
    /// 'PageViews = ' + (CASE WHEN [PageCounter_Views] IS NULL THEN '0' ELSE CAST([PageCounter_Views] AS NVARCHAR(10)) END) +
    /// ' },' AS CSharpCode
    /// FROM[tbl_PageCounter]
    /// ORDER BY[PageCounter_ID]
    /// OFFSET 0 ROWS FETCH NEXT 100 ROWS ONLY;
    /// </code>
    /// </remarks>
    public static List<TblPageCounter> PageCounters => new()
    {
        new() { Id = 1, CategoryId = 0, PicturePage = false, PageName = "Startsidan", MonthViewed = "2025-07", LastShowDate = new DateTime(2025, 7, 9, 22, 5, 26), PageViews = 31 },
        new() { Id = 2, CategoryId = 0, PicturePage = true, PageName = "Bild: ", MonthViewed = "2025-07", LastShowDate = new DateTime(2025, 7, 7, 17, 57, 18), PageViews = 100 },
        new() { Id = 3, CategoryId = 0, PicturePage = false, PageName = "Bilder", MonthViewed = "2025-07", LastShowDate = new DateTime(2025, 7, 9, 22, 5, 32), PageViews = 32 },
        new() { Id = 4, CategoryId = 0, PicturePage = true, PageName = "Kategori: Fåglar", MonthViewed = "2025-07", LastShowDate = new DateTime(2025, 7, 9, 22, 5, 33), PageViews = 9 },
        new() { Id = 5, CategoryId = 0, PicturePage = true, PageName = "Kategori: Årstider", MonthViewed = "2025-07", LastShowDate = new DateTime(2025, 7, 6, 2, 6, 26), PageViews = 1 },
        new() { Id = 6, CategoryId = 0, PicturePage = true, PageName = "Bild: parande", MonthViewed = "2025-07", LastShowDate = new DateTime(2025, 7, 6, 12, 2, 59), PageViews = 6 },
        new() { Id = 7, CategoryId = 0, PicturePage = true, PageName = "Kategori: Kråkfåglar", MonthViewed = "2025-07", LastShowDate = new DateTime(2025, 7, 7, 17, 57, 27), PageViews = 5 },
        new() { Id = 8, CategoryId = 0, PicturePage = true, PageName = "Kategori: Lommar", MonthViewed = "2025-07", LastShowDate = new DateTime(2025, 7, 6, 2, 27, 34), PageViews = 3 },
        new() { Id = 9, CategoryId = 0, PicturePage = true, PageName = "Bild: med ungar", MonthViewed = "2025-07", LastShowDate = new DateTime(2025, 7, 6, 2, 27, 24), PageViews = 2 },
        new() { Id = 10, CategoryId = 0, PicturePage = true, PageName = "Bild: parningsspel", MonthViewed = "2025-07", LastShowDate = new DateTime(2025, 7, 6, 2, 27, 21), PageViews = 1 },
        new() { Id = 11, CategoryId = 0, PicturePage = true, PageName = "Kategori: Resor", MonthViewed = "2025-07", LastShowDate = new DateTime(2025, 7, 6, 2, 45, 49), PageViews = 1 },
        new() { Id = 12, CategoryId = 0, PicturePage = true, PageName = "Bild: Lövträdlöpare", MonthViewed = "2025-07", LastShowDate = new DateTime(2025, 7, 6, 11, 19, 53), PageViews = 4 },
        new() { Id = 13, CategoryId = 0, PicturePage = true, PageName = "Kategori: Skogs- och fälthöns", MonthViewed = "2025-07", LastShowDate = new DateTime(2025, 7, 7, 17, 9, 23), PageViews = 1 },
        new() { Id = 14, CategoryId = 0, PicturePage = true, PageName = "Kategori: Trastar och duvor", MonthViewed = "2025-07", LastShowDate = new DateTime(2025, 7, 7, 17, 9, 32), PageViews = 1 },
        new() { Id = 15, CategoryId = 0, PicturePage = true, PageName = "Kategori: Större turturduva", MonthViewed = "2025-07", LastShowDate = new DateTime(2025, 7, 7, 17, 9, 34), PageViews = 1 },
        new() { Id = 16, CategoryId = 0, PicturePage = true, PageName = "Kategori: Gäss", MonthViewed = "2025-07", LastShowDate = new DateTime(2025, 7, 7, 17, 44, 18), PageViews = 1 },
        new() { Id = 17, CategoryId = 0, PicturePage = true, PageName = "Kategori: Kanadagås", MonthViewed = "2025-07", LastShowDate = new DateTime(2025, 7, 7, 17, 44, 40), PageViews = 2 },
        new() { Id = 18, CategoryId = 0, PicturePage = true, PageName = "Kategori: Hägrar, storkar och tranor", MonthViewed = "2025-07", LastShowDate = new DateTime(2025, 7, 7, 17, 48, 6), PageViews = 1 },
        new() { Id = 19, CategoryId = 0, PicturePage = true, PageName = "Kategori: Hackspettar", MonthViewed = "2025-07", LastShowDate = new DateTime(2025, 7, 9, 22, 5, 36), PageViews = 1 },
        new() { Id = 20, CategoryId = 0, PicturePage = true, PageName = "Kategori: Större hackspett", MonthViewed = "2025-07", LastShowDate = new DateTime(2025, 7, 9, 22, 5, 37), PageViews = 1 },
        new() { Id = 21, CategoryId = 0, PicturePage = false, PageName = "Startsidan", MonthViewed = "2025-08", LastShowDate = new DateTime(2025, 8, 5, 2, 22, 25), PageViews = 86 },
        new() { Id = 22, CategoryId = 0, PicturePage = false, PageName = "Bilder", MonthViewed = "2025-08", LastShowDate = new DateTime(2025, 8, 5, 2, 18, 27), PageViews = 19 },
        new() { Id = 23, CategoryId = 0, PicturePage = true, PageName = "Kategori: Däggdjur", MonthViewed = "2025-08", LastShowDate = new DateTime(2025, 8, 4, 23, 15, 11), PageViews = 4 },
        new() { Id = 24, CategoryId = 0, PicturePage = true, PageName = "Kategori: Fåglar", MonthViewed = "2025-08", LastShowDate = new DateTime(2025, 8, 4, 22, 55, 46), PageViews = 1 },
        new() { Id = 25, CategoryId = 0, PicturePage = true, PageName = "Kategori: Lunnefågel", MonthViewed = "2025-08", LastShowDate = new DateTime(2025, 8, 5, 1, 3, 23), PageViews = 4 },
        new() { Id = 26, CategoryId = 0, PicturePage = true, PageName = "Kategori: Storlabb", MonthViewed = "2025-08", LastShowDate = new DateTime(2025, 8, 5, 1, 32, 53), PageViews = 2 },
        new() { Id = 27, CategoryId = 0, PicturePage = true, PageName = "Kategori: Fjällabb", MonthViewed = "2025-08", LastShowDate = new DateTime(2025, 8, 4, 23, 54, 12), PageViews = 4 },
        new() { Id = 28, CategoryId = 0, PicturePage = true, PageName = "Kategori: Kustlabb", MonthViewed = "2025-08", LastShowDate = new DateTime(2025, 8, 5, 0, 17, 45), PageViews = 2 },
        new() { Id = 29, CategoryId = 0, PicturePage = true, PageName = "Kategori: Sillgrissla", MonthViewed = "2025-08", LastShowDate = new DateTime(2025, 8, 5, 1, 33, 29), PageViews = 3 },
        new() { Id = 30, CategoryId = 0, PicturePage = true, PageName = "Kategori: Dvärgörn", MonthViewed = "2025-08", LastShowDate = new DateTime(2025, 8, 5, 0, 20, 53), PageViews = 1 },
        new() { Id = 31, CategoryId = 0, PicturePage = true, PageName = "Kategori: Sparvhök", MonthViewed = "2025-08", LastShowDate = new DateTime(2025, 8, 5, 1, 29, 6), PageViews = 6 },
        new() { Id = 32, CategoryId = 0, PicturePage = true, PageName = "Kategori: Älg", MonthViewed = "2025-08", LastShowDate = new DateTime(2025, 8, 5, 1, 29, 34), PageViews = 1 },
        new() { Id = 33, CategoryId = 0, PicturePage = true, PageName = "Kategori: Pungmes", MonthViewed = "2025-08", LastShowDate = new DateTime(2025, 8, 5, 2, 3, 6), PageViews = 1 },
        new() { Id = 34, CategoryId = 0, PicturePage = true, PageName = "Kategori: Entita", MonthViewed = "2025-08", LastShowDate = new DateTime(2025, 8, 5, 2, 3, 7), PageViews = 1 },
        new() { Id = 35, CategoryId = 0, PicturePage = true, PageName = "Kategori: Blåmes", MonthViewed = "2025-08", LastShowDate = new DateTime(2025, 8, 5, 2, 15, 16), PageViews = 3 },
        new() { Id = 36, CategoryId = 0, PicturePage = true, PageName = "Kategori: Brunsångare", MonthViewed = "2025-08", LastShowDate = new DateTime(2025, 8, 5, 2, 3, 13), PageViews = 1 },
        new() { Id = 37, CategoryId = 0, PicturePage = true, PageName = "Kategori: Flodsångare", MonthViewed = "2025-08", LastShowDate = new DateTime(2025, 8, 5, 2, 3, 15), PageViews = 1 },
        new() { Id = 38, CategoryId = 0, PicturePage = true, PageName = "Kategori: Busksångare", MonthViewed = "2025-08", LastShowDate = new DateTime(2025, 8, 5, 2, 3, 16), PageViews = 1 },
        new() { Id = 39, CategoryId = 0, PicturePage = true, PageName = "Kategori: Sångare", MonthViewed = "2025-08", LastShowDate = new DateTime(2025, 8, 5, 2, 3, 25), PageViews = 1 },
        new() { Id = 40, CategoryId = 0, PicturePage = true, PageName = "Kategori: Törnsångare", MonthViewed = "2025-08", LastShowDate = new DateTime(2025, 8, 5, 2, 3, 37), PageViews = 1 },
        new() { Id = 41, CategoryId = 0, PicturePage = true, PageName = "Kategori: Tretåig hackspett", MonthViewed = "2025-08", LastShowDate = new DateTime(2025, 8, 5, 2, 3, 43), PageViews = 1 },
        new() { Id = 42, CategoryId = 0, PicturePage = false, PageName = "Kontakt", MonthViewed = "2025-08", LastShowDate = new DateTime(2025, 8, 5, 2, 18, 6), PageViews = 2 },
        new() { Id = 43, CategoryId = 0, PicturePage = false, PageName = "Gästbok", MonthViewed = "2025-08", LastShowDate = new DateTime(2025, 8, 5, 2, 18, 26), PageViews = 2 },
        new() { Id = 44, CategoryId = 0, PicturePage = true, PageName = "Kategori: Vår", MonthViewed = "2025-08", LastShowDate = new DateTime(2025, 8, 5, 2, 22, 40), PageViews = 1 },
    };

    #endregion

    #region Images Data

    /// <summary>
    /// Images seed data
    /// </summary>
    /// <remarks>
    /// <code>
    /// SELECT
    /// '    new() { ' +
    /// 'Id = ' + (CASE WHEN [Id] IS NULL THEN 'null' ELSE CAST([Id] AS NVARCHAR(10)) END) + ', ' +
    /// 'ImageId = ' + (CASE WHEN [image_ID] IS NULL THEN 'null' ELSE CAST([image_ID] AS NVARCHAR(10)) END) + ', ' +
    /// 'ImageCategoryId = ' + (CASE WHEN [image_art] IS NULL THEN 'null' ELSE CAST([image_art] AS NVARCHAR(10)) END) + ', ' +
    /// 'ImageFamilyId = ' + (CASE WHEN [image_familj] IS NULL THEN 'null' ELSE CAST([image_familj] AS NVARCHAR(10)) END) + ', ' +
    /// 'ImageMainFamilyId = ' + (CASE WHEN [image_huvudfamilj] IS NULL THEN 'null' ELSE CAST([image_huvudfamilj] AS NVARCHAR(10)) END) + ', ' +
    /// 'ImageUrlName = "' + REPLACE(ISNULL([image_URL], ''), '"', '"') + '", ' +
    /// 'ImageDescription = "' + REPLACE(ISNULL([image_description], ''), '"', '"') + '", ' +
    /// 'ImageDate = new DateTime(' +
    /// CAST(YEAR(ISNULL([image_date], GETDATE())) AS NVARCHAR(4)) + ', ' +
    /// CAST(MONTH(ISNULL([image_date], GETDATE())) AS NVARCHAR(2)) + ', ' +
    /// CAST(DAY(ISNULL([image_date], GETDATE())) AS NVARCHAR(2)) + ', ' +
    /// CAST(DATEPART(HOUR, ISNULL([image_date], GETDATE())) AS NVARCHAR(2)) + ', ' +
    /// CAST(DATEPART(MINUTE, ISNULL([image_date], GETDATE())) AS NVARCHAR(2)) + ', ' +
    /// CAST(DATEPART(SECOND, ISNULL([image_date], GETDATE())) AS NVARCHAR(2)) +
    /// '), ' +
    /// 'ImageUpdate = new DateTime(' +
    /// CAST(YEAR(ISNULL([image_update], GETDATE())) AS NVARCHAR(4)) + ', ' +
    /// CAST(MONTH(ISNULL([image_update], GETDATE())) AS NVARCHAR(2)) + ', ' +
    /// CAST(DAY(ISNULL([image_update], GETDATE())) AS NVARCHAR(2)) + ', ' +
    /// CAST(DATEPART(HOUR, ISNULL([image_update], GETDATE())) AS NVARCHAR(2)) + ', ' +
    /// CAST(DATEPART(MINUTE, ISNULL([image_update], GETDATE())) AS NVARCHAR(2)) + ', ' +
    /// CAST(DATEPART(SECOND, ISNULL([image_update], GETDATE())) AS NVARCHAR(2)) +
    /// ')' +
    /// ' },'
    /// AS CSharpCode
    /// FROM[tbl_images]
    /// ORDER BY[Id]
    /// OFFSET 0 ROWS FETCH NEXT 100 ROWS ONLY;
    /// </code>
    /// </remarks>
    public static List<TblImage> Images => new()
    {
        new() { Id = 1, ImageId = 1, ImageCategoryId = 49, ImageFamilyId = 27, ImageMainFamilyId = null, ImageUrlName = "AP2D6321", ImageDescription = "Beskrivning", ImageDate = new DateTime(2011, 3, 2, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 11, 18, 37, 0) },
        new() { Id = 2, ImageId = 2, ImageCategoryId = 49, ImageFamilyId = 27, ImageMainFamilyId = null, ImageUrlName = "AP2D6366", ImageDescription = "Testar lite text", ImageDate = new DateTime(2011, 3, 2, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 11, 18, 37, 0) },
        new() { Id = 3, ImageId = 3, ImageCategoryId = 49, ImageFamilyId = 27, ImageMainFamilyId = null, ImageUrlName = "AP2D6437", ImageDescription = "Mer beskrivande info...", ImageDate = new DateTime(2011, 3, 2, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 11, 18, 37, 0) },
        new() { Id = 4, ImageId = 4, ImageCategoryId = 49, ImageFamilyId = 27, ImageMainFamilyId = null, ImageUrlName = "AP2D6492", ImageDescription = "Mer beskrivande info...", ImageDate = new DateTime(2011, 3, 2, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 11, 18, 37, 0) },
        new() { Id = 5, ImageId = 5, ImageCategoryId = 50, ImageFamilyId = 4, ImageMainFamilyId = 5, ImageUrlName = "_N0Q8131", ImageDescription = "Mer beskrivande info...", ImageDate = new DateTime(2005, 7, 27, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 11, 18, 39, 0) },
        new() { Id = 6, ImageId = 6, ImageCategoryId = 50, ImageFamilyId = 4, ImageMainFamilyId = 5, ImageUrlName = "_N0Q8168", ImageDescription = "Mer beskrivande info...", ImageDate = new DateTime(2005, 7, 27, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 11, 18, 40, 0) },
        new() { Id = 7, ImageId = 7, ImageCategoryId = 50, ImageFamilyId = 4, ImageMainFamilyId = 5, ImageUrlName = "IMG_8129 kopiera", ImageDescription = "Mer beskrivande info...", ImageDate = new DateTime(2004, 7, 25, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 11, 18, 41, 0) },
        new() { Id = 8, ImageId = 8, ImageCategoryId = 50, ImageFamilyId = 4, ImageMainFamilyId = 5, ImageUrlName = "IMG_8139 kopiera", ImageDescription = "Mer beskrivande info...", ImageDate = new DateTime(2004, 7, 25, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 11, 18, 42, 0) },
        new() { Id = 9, ImageId = 9, ImageCategoryId = 51, ImageFamilyId = 10, ImageMainFamilyId = null, ImageUrlName = "AP2D9330", ImageDescription = "", ImageDate = new DateTime(2011, 5, 17, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 11, 18, 43, 0) },
        new() { Id = 10, ImageId = 10, ImageCategoryId = 51, ImageFamilyId = 10, ImageMainFamilyId = null, ImageUrlName = "AP2D9461", ImageDescription = "", ImageDate = new DateTime(2011, 5, 18, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 11, 18, 44, 0) },
        new() { Id = 11, ImageId = 11, ImageCategoryId = 51, ImageFamilyId = 10, ImageMainFamilyId = null, ImageUrlName = "AP2D9486", ImageDescription = "", ImageDate = new DateTime(2011, 5, 18, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 11, 18, 44, 0) },
        new() { Id = 12, ImageId = 12, ImageCategoryId = 34, ImageFamilyId = 4, ImageMainFamilyId = 5, ImageUrlName = "_N0Q4158", ImageDescription = "", ImageDate = new DateTime(2007, 7, 3, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 12, 17, 14, 0) },
        new() { Id = 13, ImageId = 13, ImageCategoryId = 34, ImageFamilyId = 4, ImageMainFamilyId = 5, ImageUrlName = "IMG_1445", ImageDescription = "", ImageDate = new DateTime(2010, 6, 28, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 12, 17, 14, 0) },
        new() { Id = 14, ImageId = 14, ImageCategoryId = 34, ImageFamilyId = 4, ImageMainFamilyId = 5, ImageUrlName = "IMG_1454", ImageDescription = "", ImageDate = new DateTime(2010, 6, 28, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 12, 17, 14, 0) },
        new() { Id = 15, ImageId = 15, ImageCategoryId = 52, ImageFamilyId = 10, ImageMainFamilyId = null, ImageUrlName = "IMG_9499", ImageDescription = "", ImageDate = new DateTime(2009, 4, 24, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 12, 17, 15, 0) },
        new() { Id = 16, ImageId = 16, ImageCategoryId = 52, ImageFamilyId = 10, ImageMainFamilyId = null, ImageUrlName = "IMG_9525-2", ImageDescription = "", ImageDate = new DateTime(2009, 4, 24, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 12, 17, 15, 0) },
        new() { Id = 17, ImageId = 17, ImageCategoryId = 53, ImageFamilyId = 26, ImageMainFamilyId = null, ImageUrlName = "AP2D7474", ImageDescription = "", ImageDate = new DateTime(2011, 5, 7, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 12, 17, 16, 0) },
        new() { Id = 18, ImageId = 18, ImageCategoryId = 53, ImageFamilyId = 26, ImageMainFamilyId = null, ImageUrlName = "AP2D7514", ImageDescription = "", ImageDate = new DateTime(2011, 5, 7, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 12, 17, 17, 0) },
        new() { Id = 19, ImageId = 19, ImageCategoryId = 53, ImageFamilyId = 26, ImageMainFamilyId = null, ImageUrlName = "AP2D8169", ImageDescription = "", ImageDate = new DateTime(2011, 5, 9, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 12, 17, 16, 0) },
        new() { Id = 20, ImageId = 20, ImageCategoryId = 53, ImageFamilyId = 26, ImageMainFamilyId = null, ImageUrlName = "AP2D8177", ImageDescription = "", ImageDate = new DateTime(2011, 5, 9, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 12, 17, 16, 0) },
        new() { Id = 21, ImageId = 21, ImageCategoryId = 54, ImageFamilyId = 10, ImageMainFamilyId = null, ImageUrlName = "AP2D8161", ImageDescription = "", ImageDate = new DateTime(2010, 3, 7, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 13, 15, 0, 0) },
        new() { Id = 22, ImageId = 22, ImageCategoryId = 54, ImageFamilyId = 10, ImageMainFamilyId = null, ImageUrlName = "IMG_2559", ImageDescription = "", ImageDate = new DateTime(2009, 9, 17, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 13, 15, 2, 0) },
        new() { Id = 23, ImageId = 23, ImageCategoryId = 55, ImageFamilyId = 2, ImageMainFamilyId = null, ImageUrlName = "AP2D8984", ImageDescription = "", ImageDate = new DateTime(2011, 5, 11, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 13, 15, 13, 0) },
        new() { Id = 24, ImageId = 24, ImageCategoryId = 56, ImageFamilyId = 10, ImageMainFamilyId = null, ImageUrlName = "AP2D9737", ImageDescription = "", ImageDate = new DateTime(2011, 5, 19, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 13, 15, 14, 0) },
        new() { Id = 25, ImageId = 25, ImageCategoryId = 57, ImageFamilyId = 4, ImageMainFamilyId = 5, ImageUrlName = "MC-TA00748", ImageDescription = "", ImageDate = new DateTime(2025, 8, 5, 21, 52, 0), ImageUpdate = new DateTime(2011, 8, 13, 15, 16, 0) },
        new() { Id = 26, ImageId = 26, ImageCategoryId = 57, ImageFamilyId = 4, ImageMainFamilyId = 5, ImageUrlName = "IMG_5730", ImageDescription = "", ImageDate = new DateTime(2025, 8, 5, 21, 52, 0), ImageUpdate = new DateTime(2011, 8, 13, 15, 17, 0) },
        new() { Id = 27, ImageId = 27, ImageCategoryId = 58, ImageFamilyId = 27, ImageMainFamilyId = null, ImageUrlName = "AP2D3127", ImageDescription = "", ImageDate = new DateTime(2011, 6, 8, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 13, 15, 18, 0) },
        new() { Id = 28, ImageId = 28, ImageCategoryId = 58, ImageFamilyId = 27, ImageMainFamilyId = null, ImageUrlName = "AP2D3286", ImageDescription = "", ImageDate = new DateTime(2011, 6, 8, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 13, 15, 19, 0) },
        new() { Id = 29, ImageId = 29, ImageCategoryId = 16, ImageFamilyId = 2, ImageMainFamilyId = null, ImageUrlName = "AP2D6656", ImageDescription = "", ImageDate = new DateTime(2011, 3, 8, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 13, 15, 20, 0) },
        new() { Id = 30, ImageId = 30, ImageCategoryId = 16, ImageFamilyId = 2, ImageMainFamilyId = null, ImageUrlName = "AP2D6803", ImageDescription = "", ImageDate = new DateTime(2011, 3, 8, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 13, 15, 21, 0) },
        new() { Id = 31, ImageId = 31, ImageCategoryId = 16, ImageFamilyId = 2, ImageMainFamilyId = null, ImageUrlName = "AP2D6806", ImageDescription = "", ImageDate = new DateTime(2011, 3, 8, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 13, 15, 22, 0) },
        new() { Id = 32, ImageId = 32, ImageCategoryId = 59, ImageFamilyId = 25, ImageMainFamilyId = null, ImageUrlName = "AP2D8473", ImageDescription = "", ImageDate = new DateTime(2011, 5, 9, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 13, 15, 23, 0) },
        new() { Id = 33, ImageId = 33, ImageCategoryId = 59, ImageFamilyId = 25, ImageMainFamilyId = null, ImageUrlName = "AP2D8572", ImageDescription = "", ImageDate = new DateTime(2011, 5, 9, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 13, 15, 24, 0) },
        new() { Id = 34, ImageId = 34, ImageCategoryId = 59, ImageFamilyId = 25, ImageMainFamilyId = null, ImageUrlName = "AP2D8628", ImageDescription = "", ImageDate = new DateTime(2011, 5, 9, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 13, 15, 25, 0) },
        new() { Id = 35, ImageId = 35, ImageCategoryId = 60, ImageFamilyId = 25, ImageMainFamilyId = null, ImageUrlName = "AP2D6397", ImageDescription = "", ImageDate = new DateTime(2011, 5, 4, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 13, 15, 26, 0) },
        new() { Id = 36, ImageId = 36, ImageCategoryId = 60, ImageFamilyId = 25, ImageMainFamilyId = null, ImageUrlName = "AP2D7683", ImageDescription = "", ImageDate = new DateTime(2011, 5, 8, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 13, 15, 27, 0) },
        new() { Id = 37, ImageId = 37, ImageCategoryId = 33, ImageFamilyId = 4, ImageMainFamilyId = 5, ImageUrlName = "IMG_1655", ImageDescription = "", ImageDate = new DateTime(2009, 9, 10, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 13, 15, 28, 0) },
        new() { Id = 38, ImageId = 38, ImageCategoryId = 33, ImageFamilyId = 4, ImageMainFamilyId = 5, ImageUrlName = "IMG_1692", ImageDescription = "", ImageDate = new DateTime(2009, 9, 10, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 13, 15, 29, 0) },
        new() { Id = 39, ImageId = 39, ImageCategoryId = 33, ImageFamilyId = 4, ImageMainFamilyId = 5, ImageUrlName = "IMG_1889", ImageDescription = "", ImageDate = new DateTime(2009, 9, 11, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 13, 15, 30, 0) },
        new() { Id = 40, ImageId = 40, ImageCategoryId = 62, ImageFamilyId = 26, ImageMainFamilyId = null, ImageUrlName = "AP2D6900", ImageDescription = "", ImageDate = new DateTime(2011, 5, 6, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 13, 15, 30, 0) },
        new() { Id = 41, ImageId = 41, ImageCategoryId = 62, ImageFamilyId = 26, ImageMainFamilyId = null, ImageUrlName = "AP2D6945", ImageDescription = "", ImageDate = new DateTime(2011, 5, 6, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 13, 15, 31, 0) },
        new() { Id = 42, ImageId = 42, ImageCategoryId = 62, ImageFamilyId = 26, ImageMainFamilyId = null, ImageUrlName = "AP2D7990", ImageDescription = "", ImageDate = new DateTime(2011, 5, 9, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 13, 15, 32, 0) },
        new() { Id = 43, ImageId = 43, ImageCategoryId = 62, ImageFamilyId = 26, ImageMainFamilyId = null, ImageUrlName = "AP2D8054", ImageDescription = "", ImageDate = new DateTime(2011, 5, 9, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 13, 15, 33, 0) },
        new() { Id = 44, ImageId = 44, ImageCategoryId = 63, ImageFamilyId = 25, ImageMainFamilyId = null, ImageUrlName = "AP2D8303", ImageDescription = "", ImageDate = new DateTime(2011, 5, 9, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 13, 15, 34, 0) },
        new() { Id = 45, ImageId = 45, ImageCategoryId = 64, ImageFamilyId = 214, ImageMainFamilyId = 23, ImageUrlName = "_N0Q1092", ImageDescription = "", ImageDate = new DateTime(2006, 6, 29, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 18, 23, 34, 0) },
        new() { Id = 46, ImageId = 46, ImageCategoryId = 64, ImageFamilyId = 214, ImageMainFamilyId = 23, ImageUrlName = "AP2D5358", ImageDescription = "", ImageDate = new DateTime(2011, 6, 16, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 18, 23, 35, 0) },
        new() { Id = 47, ImageId = 47, ImageCategoryId = 64, ImageFamilyId = 214, ImageMainFamilyId = 23, ImageUrlName = "AP2D5501", ImageDescription = "", ImageDate = new DateTime(2011, 6, 16, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 18, 23, 36, 0) },
        new() { Id = 48, ImageId = 48, ImageCategoryId = 64, ImageFamilyId = 214, ImageMainFamilyId = 23, ImageUrlName = "IMG_7228 kopiera", ImageDescription = "", ImageDate = new DateTime(2004, 7, 1, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 18, 23, 37, 0) },
        new() { Id = 49, ImageId = 49, ImageCategoryId = 64, ImageFamilyId = 214, ImageMainFamilyId = 23, ImageUrlName = "IMG_7313 kopiera", ImageDescription = "", ImageDate = new DateTime(2004, 7, 1, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 18, 23, 38, 0) },
        new() { Id = 50, ImageId = 50, ImageCategoryId = 65, ImageFamilyId = 204, ImageMainFamilyId = 23, ImageUrlName = "AP2D2194", ImageDescription = "", ImageDate = new DateTime(2010, 12, 22, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 18, 23, 39, 0) },
        new() { Id = 51, ImageId = 51, ImageCategoryId = 65, ImageFamilyId = 204, ImageMainFamilyId = 23, ImageUrlName = "AP2D2569", ImageDescription = "", ImageDate = new DateTime(2011, 1, 3, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 18, 23, 40, 0) },
        new() { Id = 52, ImageId = 52, ImageCategoryId = 65, ImageFamilyId = 204, ImageMainFamilyId = 23, ImageUrlName = "AP2D2990", ImageDescription = "", ImageDate = new DateTime(2010, 2, 19, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 18, 23, 41, 0) },
        new() { Id = 53, ImageId = 53, ImageCategoryId = 66, ImageFamilyId = 204, ImageMainFamilyId = 23, ImageUrlName = "AP2D3417", ImageDescription = "", ImageDate = new DateTime(2010, 2, 23, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 18, 23, 42, 0) },
        new() { Id = 54, ImageId = 54, ImageCategoryId = 66, ImageFamilyId = 204, ImageMainFamilyId = 23, ImageUrlName = "AP2D3813", ImageDescription = "", ImageDate = new DateTime(2011, 6, 13, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 18, 23, 43, 0) },
        new() { Id = 55, ImageId = 55, ImageCategoryId = 66, ImageFamilyId = 204, ImageMainFamilyId = 23, ImageUrlName = "IMG_8461", ImageDescription = "", ImageDate = new DateTime(2010, 1, 6, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 18, 23, 44, 0) },
        new() { Id = 56, ImageId = 56, ImageCategoryId = 67, ImageFamilyId = 210, ImageMainFamilyId = 23, ImageUrlName = "_N0Q5775", ImageDescription = "", ImageDate = new DateTime(2005, 6, 19, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 18, 23, 45, 0) },
        new() { Id = 57, ImageId = 57, ImageCategoryId = 67, ImageFamilyId = 210, ImageMainFamilyId = 23, ImageUrlName = "AP2D4689", ImageDescription = "", ImageDate = new DateTime(2011, 6, 15, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 18, 23, 46, 0) },
        new() { Id = 58, ImageId = 58, ImageCategoryId = 67, ImageFamilyId = 210, ImageMainFamilyId = 23, ImageUrlName = "AP2D4710", ImageDescription = "", ImageDate = new DateTime(2011, 6, 15, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 18, 23, 47, 0) },
        new() { Id = 59, ImageId = 59, ImageCategoryId = 68, ImageFamilyId = 214, ImageMainFamilyId = 23, ImageUrlName = "AP2D2581", ImageDescription = "", ImageDate = new DateTime(2011, 6, 1, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 18, 23, 48, 0) },
        new() { Id = 60, ImageId = 60, ImageCategoryId = 68, ImageFamilyId = 214, ImageMainFamilyId = 23, ImageUrlName = "AP2D2660", ImageDescription = "", ImageDate = new DateTime(2011, 6, 1, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 18, 23, 49, 0) },
        new() { Id = 61, ImageId = 61, ImageCategoryId = 68, ImageFamilyId = 214, ImageMainFamilyId = 23, ImageUrlName = "AP2D6442", ImageDescription = "", ImageDate = new DateTime(2010, 5, 2, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 18, 23, 50, 0) },
        new() { Id = 62, ImageId = 62, ImageCategoryId = 68, ImageFamilyId = 214, ImageMainFamilyId = 23, ImageUrlName = "Näktergal 22 kopiera", ImageDescription = "", ImageDate = new DateTime(2003, 5, 12, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 18, 23, 51, 0) },
        new() { Id = 63, ImageId = 63, ImageCategoryId = 69, ImageFamilyId = 204, ImageMainFamilyId = 23, ImageUrlName = "AP2D1955", ImageDescription = "", ImageDate = new DateTime(2010, 12, 2, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 18, 23, 53, 0) },
        new() { Id = 64, ImageId = 64, ImageCategoryId = 69, ImageFamilyId = 204, ImageMainFamilyId = 23, ImageUrlName = "AP2D1994", ImageDescription = "", ImageDate = new DateTime(2010, 12, 2, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 18, 23, 54, 0) },
        new() { Id = 65, ImageId = 65, ImageCategoryId = 70, ImageFamilyId = 204, ImageMainFamilyId = 23, ImageUrlName = "_TAR9601", ImageDescription = "", ImageDate = new DateTime(2008, 4, 29, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 18, 23, 55, 0) },
        new() { Id = 66, ImageId = 66, ImageCategoryId = 70, ImageFamilyId = 204, ImageMainFamilyId = 23, ImageUrlName = "AP2D3309", ImageDescription = "", ImageDate = new DateTime(2010, 2, 22, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 18, 23, 56, 0) },
        new() { Id = 67, ImageId = 67, ImageCategoryId = 70, ImageFamilyId = 204, ImageMainFamilyId = 23, ImageUrlName = "AP2D3316", ImageDescription = "", ImageDate = new DateTime(2010, 2, 22, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 18, 23, 57, 0) },
        new() { Id = 68, ImageId = 68, ImageCategoryId = 71, ImageFamilyId = 204, ImageMainFamilyId = 23, ImageUrlName = "IMG_8423", ImageDescription = "", ImageDate = new DateTime(2010, 1, 6, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 18, 23, 58, 0) },
        new() { Id = 69, ImageId = 69, ImageCategoryId = 71, ImageFamilyId = 204, ImageMainFamilyId = 23, ImageUrlName = "IMG_8438", ImageDescription = "", ImageDate = new DateTime(2010, 1, 6, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 18, 23, 59, 0) },
        new() { Id = 70, ImageId = 70, ImageCategoryId = 72, ImageFamilyId = 2, ImageMainFamilyId = null, ImageUrlName = "TTAR5548", ImageDescription = "", ImageDate = new DateTime(2008, 10, 13, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 0, 0, 0) },
        new() { Id = 71, ImageId = 71, ImageCategoryId = 72, ImageFamilyId = 2, ImageMainFamilyId = null, ImageUrlName = "VN0Q0652", ImageDescription = "", ImageDate = new DateTime(2008, 10, 18, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 0, 1, 0) },
        new() { Id = 72, ImageId = 72, ImageCategoryId = 72, ImageFamilyId = 2, ImageMainFamilyId = null, ImageUrlName = "VN0Q0755", ImageDescription = "", ImageDate = new DateTime(2008, 10, 20, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 0, 2, 0) },
        new() { Id = 73, ImageId = 73, ImageCategoryId = 72, ImageFamilyId = 2, ImageMainFamilyId = null, ImageUrlName = "VN0Q1362", ImageDescription = "", ImageDate = new DateTime(2008, 10, 22, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 0, 3, 0) },
        new() { Id = 74, ImageId = 74, ImageCategoryId = 72, ImageFamilyId = 2, ImageMainFamilyId = null, ImageUrlName = "VN0Q9496", ImageDescription = "", ImageDate = new DateTime(2008, 10, 13, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 0, 4, 0) },
        new() { Id = 75, ImageId = 75, ImageCategoryId = 11, ImageFamilyId = 2, ImageMainFamilyId = null, ImageUrlName = "_N0Q2752", ImageDescription = "", ImageDate = new DateTime(2008, 8, 13, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 0, 5, 0) },
        new() { Id = 76, ImageId = 76, ImageCategoryId = 11, ImageFamilyId = 2, ImageMainFamilyId = null, ImageUrlName = "_N0Q2815", ImageDescription = "", ImageDate = new DateTime(2008, 8, 20, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 0, 6, 0) },
        new() { Id = 77, ImageId = 77, ImageCategoryId = 11, ImageFamilyId = 2, ImageMainFamilyId = null, ImageUrlName = "_N0Q2840", ImageDescription = "", ImageDate = new DateTime(2008, 8, 20, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 0, 7, 0) },
        new() { Id = 78, ImageId = 78, ImageCategoryId = 11, ImageFamilyId = 2, ImageMainFamilyId = null, ImageUrlName = "_N0Q2897", ImageDescription = "", ImageDate = new DateTime(2008, 8, 21, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 0, 8, 0) },
        new() { Id = 79, ImageId = 79, ImageCategoryId = 11, ImageFamilyId = 2, ImageMainFamilyId = null, ImageUrlName = "_TAR1791", ImageDescription = "", ImageDate = new DateTime(2008, 8, 11, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 0, 9, 0) },
        new() { Id = 80, ImageId = 80, ImageCategoryId = 19, ImageFamilyId = 2, ImageMainFamilyId = null, ImageUrlName = "_TAR4754", ImageDescription = "", ImageDate = new DateTime(2006, 9, 30, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 0, 10, 0) },
        new() { Id = 81, ImageId = 81, ImageCategoryId = 19, ImageFamilyId = 2, ImageMainFamilyId = null, ImageUrlName = "IMG_2726", ImageDescription = "", ImageDate = new DateTime(2009, 9, 28, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 0, 11, 0) },
        new() { Id = 82, ImageId = 82, ImageCategoryId = 13, ImageFamilyId = 2, ImageMainFamilyId = null, ImageUrlName = "_N0Q1103", ImageDescription = "", ImageDate = new DateTime(2005, 9, 25, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 12, 11, 0) },
        new() { Id = 83, ImageId = 83, ImageCategoryId = 13, ImageFamilyId = 2, ImageMainFamilyId = null, ImageUrlName = "_N0Q1207", ImageDescription = "", ImageDate = new DateTime(2005, 9, 25, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 12, 12, 0) },
        new() { Id = 84, ImageId = 84, ImageCategoryId = 13, ImageFamilyId = 2, ImageMainFamilyId = null, ImageUrlName = "_N0Q1212", ImageDescription = "", ImageDate = new DateTime(2005, 9, 25, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 12, 13, 0) },
        new() { Id = 85, ImageId = 85, ImageCategoryId = 13, ImageFamilyId = 2, ImageMainFamilyId = null, ImageUrlName = "_N0Q1285", ImageDescription = "", ImageDate = new DateTime(2005, 9, 26, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 12, 14, 0) },
        new() { Id = 86, ImageId = 86, ImageCategoryId = 18, ImageFamilyId = 2, ImageMainFamilyId = null, ImageUrlName = "_TAR0107", ImageDescription = "", ImageDate = new DateTime(2008, 5, 20, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 12, 15, 0) },
        new() { Id = 87, ImageId = 87, ImageCategoryId = 18, ImageFamilyId = 2, ImageMainFamilyId = null, ImageUrlName = "IMG_2901", ImageDescription = "", ImageDate = new DateTime(2009, 3, 12, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 12, 16, 0) },
        new() { Id = 88, ImageId = 88, ImageCategoryId = 18, ImageFamilyId = 2, ImageMainFamilyId = null, ImageUrlName = "TTAR3086", ImageDescription = "", ImageDate = new DateTime(2008, 9, 23, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 12, 17, 0) },
        new() { Id = 89, ImageId = 89, ImageCategoryId = 15, ImageFamilyId = 2, ImageMainFamilyId = null, ImageUrlName = "IMG_2915-A", ImageDescription = "", ImageDate = new DateTime(2009, 10, 1, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 12, 18, 0) },
        new() { Id = 90, ImageId = 90, ImageCategoryId = 15, ImageFamilyId = 2, ImageMainFamilyId = null, ImageUrlName = "IMG_2930", ImageDescription = "", ImageDate = new DateTime(2009, 10, 1, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 12, 19, 0) },
        new() { Id = 91, ImageId = 91, ImageCategoryId = 15, ImageFamilyId = 2, ImageMainFamilyId = null, ImageUrlName = "IMG_2941", ImageDescription = "", ImageDate = new DateTime(2009, 10, 1, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 12, 20, 0) },
        new() { Id = 92, ImageId = 92, ImageCategoryId = 73, ImageFamilyId = 273, ImageMainFamilyId = 22, ImageUrlName = "_N0Q7056", ImageDescription = "", ImageDate = new DateTime(2005, 7, 11, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 12, 21, 0) },
        new() { Id = 93, ImageId = 93, ImageCategoryId = 73, ImageFamilyId = 273, ImageMainFamilyId = 22, ImageUrlName = "_N0Q7105", ImageDescription = "", ImageDate = new DateTime(2005, 7, 11, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 12, 22, 0) },
        new() { Id = 94, ImageId = 94, ImageCategoryId = 73, ImageFamilyId = 273, ImageMainFamilyId = 22, ImageUrlName = "_N0Q7155", ImageDescription = "", ImageDate = new DateTime(2005, 7, 11, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 12, 23, 0) },
        new() { Id = 95, ImageId = 95, ImageCategoryId = 74, ImageFamilyId = 288, ImageMainFamilyId = 22, ImageUrlName = "_N0Q6258", ImageDescription = "", ImageDate = new DateTime(2007, 9, 17, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 12, 24, 0) },
        new() { Id = 96, ImageId = 96, ImageCategoryId = 74, ImageFamilyId = 288, ImageMainFamilyId = 22, ImageUrlName = "_N0Q6556", ImageDescription = "", ImageDate = new DateTime(2007, 9, 24, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 12, 25, 0) },
        new() { Id = 97, ImageId = 97, ImageCategoryId = 74, ImageFamilyId = 288, ImageMainFamilyId = 22, ImageUrlName = "_TAR1671", ImageDescription = "", ImageDate = new DateTime(2008, 6, 17, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 12, 26, 0) },
        new() { Id = 98, ImageId = 98, ImageCategoryId = 74, ImageFamilyId = 288, ImageMainFamilyId = 22, ImageUrlName = "_TAR2403", ImageDescription = "", ImageDate = new DateTime(2007, 6, 13, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 12, 27, 0) },
        new() { Id = 99, ImageId = 99, ImageCategoryId = 74, ImageFamilyId = 288, ImageMainFamilyId = 22, ImageUrlName = "_TAR2446", ImageDescription = "", ImageDate = new DateTime(2007, 6, 13, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 12, 28, 0) },
        new() { Id = 100, ImageId = 100, ImageCategoryId = 74, ImageFamilyId = 288, ImageMainFamilyId = 22, ImageUrlName = "TTAR4615", ImageDescription = "", ImageDate = new DateTime(2008, 10, 1, 0, 0, 0), ImageUpdate = new DateTime(2011, 8, 19, 12, 29, 0) },
    };

    #endregion

    #region Initialization of the database

    /// <summary>
    /// En extensionmetod skapad via 'ArvidsonFotoCoreDbSeeder.cs' , som skapar upp test-data till databasen.
    /// </summary>
    public static void InitialDatabaseSeed(this ModelBuilder modelBuilder)
    {
        // Seed all data using the public properties
        modelBuilder.Entity<TblGb>().HasData(GuestbookEntries);
        modelBuilder.Entity<TblMenu>().HasData(MenuCategories);
        modelBuilder.Entity<TblPageCounter>().HasData(PageCounters);
        modelBuilder.Entity<TblImage>().HasData(Images);
    }

    #endregion

}