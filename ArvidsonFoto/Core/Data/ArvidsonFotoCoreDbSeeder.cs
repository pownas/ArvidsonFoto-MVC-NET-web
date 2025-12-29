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
    public static List<TblGb> DbSeed_Tbl_Guestbook => new()
    {
        new() {
            Id = 1,
            GbId = 1,
            GbDate = new DateTime(2021, 11, 22),
            GbEmail = "pownas@outlook.com",
            GbName = "pownas",
            GbHomepage = "github.com/pownas",
            GbReadPost = false,
            GbText = "Ett första test inlägg i databasen..."
        },
        new() {
            Id = 2,
            GbId = 2,
            GbDate = new DateTime(2025, 12, 16),
            GbEmail = "pownas@outlook.com",
            GbName = "pownas",
            GbHomepage = "github.com/pownas",
            GbReadPost = false,
            GbText = "Ett andra test inlägg i databasen... 😊"
        },
    };

    #endregion

    #region NavMenu data

    /// <summary>
    /// Menu categories seed data
    /// </summary>
    /// <remarks>
    /// Exportera ut alla rader av SQL-data från databasen med tbl_menu:
    /// <code>
    /// -- ===========================================
    /// -- Export TblMenu (2025-15-16)
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
    /// ORDER BY [Id];
    /// </code>
    /// </remarks>
    public static List<TblMenu> DbSeed_Tbl_MenuCategories => new()
    {
        // Main categories (parent = 0)
        new() { Id = 1, MenuCategoryId = 1, MenuParentCategoryId = 0, MenuDisplayName = "Fåglar", MenuUrlSegment = "Faglar", MenuDateUpdated = DateTime.Parse("2021-03-25T20:27:00") },
        new() { Id = 2, MenuCategoryId = 2, MenuParentCategoryId = 0, MenuDisplayName = "Däggdjur", MenuUrlSegment = "Daggdjur", MenuDateUpdated = DateTime.Parse("2021-03-19T13:56:00") },
        new() { Id = 3, MenuCategoryId = 3, MenuParentCategoryId = 0, MenuDisplayName = "Kräldjur", MenuUrlSegment = "Kraldjur", MenuDateUpdated = DateTime.Parse("2021-03-17T14:40:00") },
        new() { Id = 4, MenuCategoryId = 4, MenuParentCategoryId = 5, MenuDisplayName = "Fjärilar", MenuUrlSegment = "Fjarilar", MenuDateUpdated = DateTime.Parse("2021-03-09T08:55:12") },
        new() { Id = 5, MenuCategoryId = 5, MenuParentCategoryId = 0, MenuDisplayName = "Insekter", MenuUrlSegment = "Insekter", MenuDateUpdated = DateTime.Parse("2021-03-10T11:53:00") },
        new() { Id = 6, MenuCategoryId = 6, MenuParentCategoryId = 0, MenuDisplayName = "Växter", MenuUrlSegment = "Vaxter", MenuDateUpdated = DateTime.Parse("2021-03-16T07:25:00") },
        new() { Id = 7, MenuCategoryId = 7, MenuParentCategoryId = 0, MenuDisplayName = "Landskap", MenuUrlSegment = "Landskap", MenuDateUpdated = DateTime.Parse("2021-03-19T13:56:00") },
        new() { Id = 8, MenuCategoryId = 8, MenuParentCategoryId = 0, MenuDisplayName = "Årstider", MenuUrlSegment = "Arstider", MenuDateUpdated = DateTime.Parse("2021-03-16T07:45:00") },
        new() { Id = 9, MenuCategoryId = 9, MenuParentCategoryId = 0, MenuDisplayName = "Resor", MenuUrlSegment = "Resor", MenuDateUpdated = DateTime.Parse("2021-03-19T13:57:00") },

        // Sub categories
        new() { Id = 10, MenuCategoryId = 10, MenuParentCategoryId = 1, MenuDisplayName = "Dagrovfåglar", MenuUrlSegment = "Dagrovfaglar", MenuDateUpdated = DateTime.Parse("2021-03-23T21:53:46") },
        new() { Id = 11, MenuCategoryId = 11, MenuParentCategoryId = 2, MenuDisplayName = "Bäver", MenuUrlSegment = "Baver", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:07") },
        new() { Id = 12, MenuCategoryId = 12, MenuParentCategoryId = 2, MenuDisplayName = "Utter", MenuUrlSegment = "Utter", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:09") },
        new() { Id = 13, MenuCategoryId = 13, MenuParentCategoryId = 2, MenuDisplayName = "Myskoxe", MenuUrlSegment = "Myskoxe", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:09") },
        new() { Id = 14, MenuCategoryId = 14, MenuParentCategoryId = 2, MenuDisplayName = "Hare", MenuUrlSegment = "Hare", MenuDateUpdated = DateTime.Parse("2021-03-23T05:03:55") },
        new() { Id = 15, MenuCategoryId = 15, MenuParentCategoryId = 2, MenuDisplayName = "Älg", MenuUrlSegment = "Alg", MenuDateUpdated = DateTime.Parse("2021-03-19T09:39:29") },
        new() { Id = 16, MenuCategoryId = 16, MenuParentCategoryId = 2, MenuDisplayName = "Rådjur", MenuUrlSegment = "Radjur", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:09") },
        new() { Id = 17, MenuCategoryId = 17, MenuParentCategoryId = 2, MenuDisplayName = "Fjällräv", MenuUrlSegment = "Fjallrav", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:08") },
        new() { Id = 18, MenuCategoryId = 18, MenuParentCategoryId = 2, MenuDisplayName = "Räv", MenuUrlSegment = "Rav", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:09") },
        new() { Id = 19, MenuCategoryId = 19, MenuParentCategoryId = 2, MenuDisplayName = "Dovhjort", MenuUrlSegment = "Dovhjort", MenuDateUpdated = DateTime.Parse("2021-03-24T00:22:05") },
        new() { Id = 20, MenuCategoryId = 20, MenuParentCategoryId = 1, MenuDisplayName = "Svanar", MenuUrlSegment = "Svanar", MenuDateUpdated = DateTime.Parse("2021-03-25T20:29:54") },
        new() { Id = 21, MenuCategoryId = 21, MenuParentCategoryId = 1, MenuDisplayName = "Skogs- och fälthöns", MenuUrlSegment = "Skogs-Och-Falthons", MenuDateUpdated = DateTime.Parse("2021-03-01T06:01:44") },
        new() { Id = 22, MenuCategoryId = 22, MenuParentCategoryId = 1, MenuDisplayName = "Vadare", MenuUrlSegment = "Vadare", MenuDateUpdated = DateTime.Parse("2021-03-22T06:00:11") },
        new() { Id = 23, MenuCategoryId = 23, MenuParentCategoryId = 1, MenuDisplayName = "Tättingar", MenuUrlSegment = "Tattingar", MenuDateUpdated = DateTime.Parse("2021-03-25T20:30:17") },
        new() { Id = 24, MenuCategoryId = 24, MenuParentCategoryId = 1, MenuDisplayName = "Hägrar, storkar och tranor", MenuUrlSegment = "Hagrar-storkar-tranor", MenuDateUpdated = DateTime.Parse("2021-03-25T22:06:31") },
        new() { Id = 25, MenuCategoryId = 25, MenuParentCategoryId = 1, MenuDisplayName = "Änder", MenuUrlSegment = "Ander", MenuDateUpdated = DateTime.Parse("2021-03-01T04:26:12") },
        new() { Id = 26, MenuCategoryId = 26, MenuParentCategoryId = 1, MenuDisplayName = "Doppingar och skarvar", MenuUrlSegment = "Doppingar-skarvar", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:15") },
        new() { Id = 27, MenuCategoryId = 27, MenuParentCategoryId = 1, MenuDisplayName = "Hackspettar", MenuUrlSegment = "Hackspettar", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:31") },
        new() { Id = 28, MenuCategoryId = 28, MenuParentCategoryId = 1, MenuDisplayName = "Måsar, trutar och tärnor", MenuUrlSegment = "Masar-trutar-tarnor", MenuDateUpdated = DateTime.Parse("2021-03-25T17:20:43") },
        new() { Id = 29, MenuCategoryId = 29, MenuParentCategoryId = 3, MenuDisplayName = "Huggorm", MenuUrlSegment = "Huggorm", MenuDateUpdated = DateTime.Parse("2021-03-11T19:17:49") },
        new() { Id = 30, MenuCategoryId = 30, MenuParentCategoryId = 3, MenuDisplayName = "Snok", MenuUrlSegment = "Snok", MenuDateUpdated = DateTime.Parse("2021-03-05T13:32:27") },
        new() { Id = 31, MenuCategoryId = 31, MenuParentCategoryId = 4, MenuDisplayName = "Nässelfjäril", MenuUrlSegment = "Nasselfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T06:45:08") },
        new() { Id = 32, MenuCategoryId = 32, MenuParentCategoryId = 4, MenuDisplayName = "Påfågelöga", MenuUrlSegment = "Pafageloga", MenuDateUpdated = DateTime.Parse("2021-03-01T06:25:05") },
        new() { Id = 33, MenuCategoryId = 33, MenuParentCategoryId = 4, MenuDisplayName = "Sorgmantel", MenuUrlSegment = "Sorgmantel", MenuDateUpdated = DateTime.Parse("2021-03-01T06:06:29") },
        new() { Id = 34, MenuCategoryId = 34, MenuParentCategoryId = 4, MenuDisplayName = "Aspfjäril", MenuUrlSegment = "Aspfjaril", MenuDateUpdated = DateTime.Parse("2021-03-16T07:24:33") },
        new() { Id = 35, MenuCategoryId = 35, MenuParentCategoryId = 5, MenuDisplayName = "Övriga insekter", MenuUrlSegment = "Ovriga-insekter", MenuDateUpdated = DateTime.Parse("2021-03-01T05:37:24") },
        new() { Id = 36, MenuCategoryId = 36, MenuParentCategoryId = 5, MenuDisplayName = "Sländor", MenuUrlSegment = "Slandor", MenuDateUpdated = DateTime.Parse("2021-03-01T21:28:24") },
        new() { Id = 37, MenuCategoryId = 37, MenuParentCategoryId = 5, MenuDisplayName = "Skalbaggar", MenuUrlSegment = "Skalbaggar", MenuDateUpdated = DateTime.Parse("2021-03-24T00:22:13") },
        new() { Id = 38, MenuCategoryId = 38, MenuParentCategoryId = 8, MenuDisplayName = "Vår", MenuUrlSegment = "Var", MenuDateUpdated = DateTime.Parse("2021-03-23T04:58:36") },
        new() { Id = 39, MenuCategoryId = 39, MenuParentCategoryId = 8, MenuDisplayName = "Sommar", MenuUrlSegment = "Sommar", MenuDateUpdated = DateTime.Parse("2021-03-14T17:57:07") },
        new() { Id = 40, MenuCategoryId = 40, MenuParentCategoryId = 8, MenuDisplayName = "Höst", MenuUrlSegment = "Host", MenuDateUpdated = DateTime.Parse("2021-03-17T15:17:52") },
        new() { Id = 41, MenuCategoryId = 41, MenuParentCategoryId = 8, MenuDisplayName = "Vinter", MenuUrlSegment = "Vinter", MenuDateUpdated = DateTime.Parse("2021-03-16T07:37:43") },
        new() { Id = 42, MenuCategoryId = 42, MenuParentCategoryId = 7, MenuDisplayName = "Oset och Rynningeviken", MenuUrlSegment = "Oset-och-rynningeviken", MenuDateUpdated = DateTime.Parse("2021-03-11T00:23:42") },
        new() { Id = 43, MenuCategoryId = 43, MenuParentCategoryId = 7, MenuDisplayName = "Kvismaren", MenuUrlSegment = "Kvismaren", MenuDateUpdated = DateTime.Parse("2021-03-12T21:17:16") },
        new() { Id = 44, MenuCategoryId = 44, MenuParentCategoryId = 7, MenuDisplayName = "Tysslingen", MenuUrlSegment = "Tysslingen", MenuDateUpdated = DateTime.Parse("2021-03-20T11:44:31") },
        new() { Id = 45, MenuCategoryId = 45, MenuParentCategoryId = 9, MenuDisplayName = "2010 Island", MenuUrlSegment = "2010-Island", MenuDateUpdated = DateTime.Parse("2021-03-17T08:48:24") },
        new() { Id = 46, MenuCategoryId = 46, MenuParentCategoryId = 1, MenuDisplayName = "Gäss", MenuUrlSegment = "Gass", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:17") },
        new() { Id = 47, MenuCategoryId = 47, MenuParentCategoryId = 1, MenuDisplayName = "Trastar och duvor", MenuUrlSegment = "Trastar-och-duvor", MenuDateUpdated = DateTime.Parse("2021-03-01T05:33:39") },
        new() { Id = 48, MenuCategoryId = 48, MenuParentCategoryId = 1, MenuDisplayName = "Alkor och labbar", MenuUrlSegment = "Alkor-och-labbar", MenuDateUpdated = DateTime.Parse("2021-03-23T15:19:39") },
        new() { Id = 49, MenuCategoryId = 49, MenuParentCategoryId = 27, MenuDisplayName = "Tretåig hackspett", MenuUrlSegment = "Tretaig-hackspett", MenuDateUpdated = DateTime.Parse("2021-03-19T10:01:31") },
        new() { Id = 50, MenuCategoryId = 50, MenuParentCategoryId = 4, MenuDisplayName = "Apollofjäril", MenuUrlSegment = "Apollofjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T06:23:32") },
        new() { Id = 51, MenuCategoryId = 51, MenuParentCategoryId = 10, MenuDisplayName = "Aftonfalk", MenuUrlSegment = "Aftonfalk", MenuDateUpdated = DateTime.Parse("2021-03-19T09:30:52") },
        new() { Id = 52, MenuCategoryId = 52, MenuParentCategoryId = 10, MenuDisplayName = "Brun kärrhök", MenuUrlSegment = "Brun-karrhok", MenuDateUpdated = DateTime.Parse("2021-03-19T09:50:19") },
        new() { Id = 53, MenuCategoryId = 53, MenuParentCategoryId = 26, MenuDisplayName = "Gråhakedopping", MenuUrlSegment = "Grahakedopping", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:15") },
        new() { Id = 54, MenuCategoryId = 54, MenuParentCategoryId = 10, MenuDisplayName = "Havsörn", MenuUrlSegment = "Havsorn", MenuDateUpdated = DateTime.Parse("2021-03-23T13:25:35") },
        new() { Id = 55, MenuCategoryId = 55, MenuParentCategoryId = 2, MenuDisplayName = "Lodjur", MenuUrlSegment = "Lodjur", MenuDateUpdated = DateTime.Parse("2021-03-16T07:26:47") },
        new() { Id = 56, MenuCategoryId = 56, MenuParentCategoryId = 10, MenuDisplayName = "Lärkfalk", MenuUrlSegment = "Larkfalk", MenuDateUpdated = DateTime.Parse("2021-03-19T09:31:32") },
        new() { Id = 57, MenuCategoryId = 57, MenuParentCategoryId = 4, MenuDisplayName = "Makaonfjäril", MenuUrlSegment = "Makaonfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T06:24:51") },
        new() { Id = 58, MenuCategoryId = 58, MenuParentCategoryId = 27, MenuDisplayName = "Mindre hackspett", MenuUrlSegment = "Mindre-hackspett", MenuDateUpdated = DateTime.Parse("2021-03-19T09:50:30") },
        new() { Id = 59, MenuCategoryId = 59, MenuParentCategoryId = 25, MenuDisplayName = "Skedand", MenuUrlSegment = "Skedand", MenuDateUpdated = DateTime.Parse("2021-03-19T07:52:32") },
        new() { Id = 60, MenuCategoryId = 60, MenuParentCategoryId = 25, MenuDisplayName = "Snatterand", MenuUrlSegment = "Snatterand", MenuDateUpdated = DateTime.Parse("2021-03-19T09:20:08") },
        new() { Id = 61, MenuCategoryId = 62, MenuParentCategoryId = 26, MenuDisplayName = "Svarthakedopping", MenuUrlSegment = "Svarthakedopping", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:16") },
        new() { Id = 62, MenuCategoryId = 63, MenuParentCategoryId = 25, MenuDisplayName = "Årta", MenuUrlSegment = "Arta", MenuDateUpdated = DateTime.Parse("2021-03-01T05:16:49") },
        new() { Id = 63, MenuCategoryId = 64, MenuParentCategoryId = 214, MenuDisplayName = "Blåhake", MenuUrlSegment = "Blahake", MenuDateUpdated = DateTime.Parse("2021-03-24T23:00:01") },
        new() { Id = 64, MenuCategoryId = 65, MenuParentCategoryId = 204, MenuDisplayName = "Domherre", MenuUrlSegment = "Domherre", MenuDateUpdated = DateTime.Parse("2021-03-01T09:19:27") },
        new() { Id = 65, MenuCategoryId = 66, MenuParentCategoryId = 204, MenuDisplayName = "Gråsiska", MenuUrlSegment = "Grasiska", MenuDateUpdated = DateTime.Parse("2021-03-25T16:52:21") },
        new() { Id = 66, MenuCategoryId = 67, MenuParentCategoryId = 210, MenuDisplayName = "Lappsparv", MenuUrlSegment = "Lappsparv", MenuDateUpdated = DateTime.Parse("2021-03-19T09:52:18") },
        new() { Id = 67, MenuCategoryId = 68, MenuParentCategoryId = 214, MenuDisplayName = "Näktergal", MenuUrlSegment = "Naktergal", MenuDateUpdated = DateTime.Parse("2021-03-19T09:59:17") },
        new() { Id = 68, MenuCategoryId = 69, MenuParentCategoryId = 204, MenuDisplayName = "Steglits", MenuUrlSegment = "Steglits", MenuDateUpdated = DateTime.Parse("2021-03-01T09:20:02") },
        new() { Id = 69, MenuCategoryId = 70, MenuParentCategoryId = 204, MenuDisplayName = "Stenknäck", MenuUrlSegment = "Stenknack", MenuDateUpdated = DateTime.Parse("2021-03-25T16:50:18") },
        new() { Id = 70, MenuCategoryId = 71, MenuParentCategoryId = 204, MenuDisplayName = "Vinterhämpling", MenuUrlSegment = "Vinterhampling", MenuDateUpdated = DateTime.Parse("2021-03-03T13:02:35") },
        new() { Id = 71, MenuCategoryId = 72, MenuParentCategoryId = 2, MenuDisplayName = "Ekorre", MenuUrlSegment = "Ekorre", MenuDateUpdated = DateTime.Parse("2021-03-23T21:06:08") },
        new() { Id = 72, MenuCategoryId = 73, MenuParentCategoryId = 273, MenuDisplayName = "Fjällpipare", MenuUrlSegment = "Fjallpipare", MenuDateUpdated = DateTime.Parse("2021-03-19T13:56:00") },
        new() { Id = 73, MenuCategoryId = 74, MenuParentCategoryId = 288, MenuDisplayName = "Kärrsnäppa", MenuUrlSegment = "Karrsnappa", MenuDateUpdated = DateTime.Parse("2021-03-19T09:55:22") },
        new() { Id = 74, MenuCategoryId = 75, MenuParentCategoryId = 22, MenuDisplayName = "Roskarl", MenuUrlSegment = "Roskarl", MenuDateUpdated = DateTime.Parse("2021-03-01T05:20:51") },
        new() { Id = 75, MenuCategoryId = 76, MenuParentCategoryId = 274, MenuDisplayName = "Rödspov", MenuUrlSegment = "Rodspov", MenuDateUpdated = DateTime.Parse("2021-03-19T13:56:55") },
        new() { Id = 76, MenuCategoryId = 77, MenuParentCategoryId = 22, MenuDisplayName = "Skärfläcka", MenuUrlSegment = "Skarflacka", MenuDateUpdated = DateTime.Parse("2021-03-19T09:22:07") },
        new() { Id = 77, MenuCategoryId = 78, MenuParentCategoryId = 288, MenuDisplayName = "Skärsnäppa", MenuUrlSegment = "Skarsnappa", MenuDateUpdated = DateTime.Parse("2021-03-01T07:20:17") },
        new() { Id = 78, MenuCategoryId = 79, MenuParentCategoryId = 274, MenuDisplayName = "Storspov", MenuUrlSegment = "Storspov", MenuDateUpdated = DateTime.Parse("2021-03-19T09:30:29") },
        new() { Id = 79, MenuCategoryId = 80, MenuParentCategoryId = 22, MenuDisplayName = "Tofsvipa", MenuUrlSegment = "Tofsvipa", MenuDateUpdated = DateTime.Parse("2021-03-23T21:27:36") },
        new() { Id = 80, MenuCategoryId = 81, MenuParentCategoryId = 1, MenuDisplayName = "Ugglor", MenuUrlSegment = "Ugglor", MenuDateUpdated = DateTime.Parse("2021-03-01T04:26:00") },
        new() { Id = 81, MenuCategoryId = 82, MenuParentCategoryId = 81, MenuDisplayName = "Berguv", MenuUrlSegment = "Berguv", MenuDateUpdated = DateTime.Parse("2021-03-20T18:34:22") },
        new() { Id = 82, MenuCategoryId = 83, MenuParentCategoryId = 81, MenuDisplayName = "Hornuggla", MenuUrlSegment = "Hornuggla", MenuDateUpdated = DateTime.Parse("2021-03-19T07:57:57") },
        new() { Id = 83, MenuCategoryId = 84, MenuParentCategoryId = 81, MenuDisplayName = "Hökuggla", MenuUrlSegment = "Hokuggla", MenuDateUpdated = DateTime.Parse("2021-03-01T05:29:29") },
        new() { Id = 84, MenuCategoryId = 85, MenuParentCategoryId = 81, MenuDisplayName = "Jorduggla", MenuUrlSegment = "Jorduggla", MenuDateUpdated = DateTime.Parse("2021-03-19T07:58:09") },
        new() { Id = 85, MenuCategoryId = 86, MenuParentCategoryId = 81, MenuDisplayName = "Kattuggla", MenuUrlSegment = "Kattuggla", MenuDateUpdated = DateTime.Parse("2021-03-04T10:44:49") },
        new() { Id = 86, MenuCategoryId = 87, MenuParentCategoryId = 81, MenuDisplayName = "Lappuggla", MenuUrlSegment = "Lappuggla", MenuDateUpdated = DateTime.Parse("2021-03-25T21:23:59") },
        new() { Id = 87, MenuCategoryId = 88, MenuParentCategoryId = 81, MenuDisplayName = "Pärluggla", MenuUrlSegment = "Parluggla", MenuDateUpdated = DateTime.Parse("2021-03-01T05:34:49") },
        new() { Id = 88, MenuCategoryId = 89, MenuParentCategoryId = 81, MenuDisplayName = "Slaguggla", MenuUrlSegment = "Slaguggla", MenuDateUpdated = DateTime.Parse("2021-03-23T21:49:38") },
        new() { Id = 89, MenuCategoryId = 90, MenuParentCategoryId = 81, MenuDisplayName = "Sparvuggla", MenuUrlSegment = "Sparvuggla", MenuDateUpdated = DateTime.Parse("2021-03-16T07:45:49") },
        new() { Id = 90, MenuCategoryId = 91, MenuParentCategoryId = 20, MenuDisplayName = "Knölsvan", MenuUrlSegment = "Knolsvan", MenuDateUpdated = DateTime.Parse("2021-03-22T22:55:43") },
        new() { Id = 91, MenuCategoryId = 92, MenuParentCategoryId = 20, MenuDisplayName = "Mindre Sångsvan", MenuUrlSegment = "Mindre-Sangsvan", MenuDateUpdated = DateTime.Parse("2021-03-19T09:34:17") },
        new() { Id = 92, MenuCategoryId = 93, MenuParentCategoryId = 20, MenuDisplayName = "Sångsvan", MenuUrlSegment = "Sangsvan", MenuDateUpdated = DateTime.Parse("2021-03-25T20:28:23") },
        new() { Id = 93, MenuCategoryId = 94, MenuParentCategoryId = 21, MenuDisplayName = "Fasan", MenuUrlSegment = "Fasan", MenuDateUpdated = DateTime.Parse("2021-03-19T09:46:45") },
        new() { Id = 94, MenuCategoryId = 95, MenuParentCategoryId = 21, MenuDisplayName = "Orre", MenuUrlSegment = "Orre", MenuDateUpdated = DateTime.Parse("2021-03-01T06:48:16") },
        new() { Id = 95, MenuCategoryId = 96, MenuParentCategoryId = 21, MenuDisplayName = "Rapphöna", MenuUrlSegment = "Rapphona", MenuDateUpdated = DateTime.Parse("2021-03-19T08:31:18") },
        new() { Id = 96, MenuCategoryId = 97, MenuParentCategoryId = 21, MenuDisplayName = "Tjäder", MenuUrlSegment = "Tjader", MenuDateUpdated = DateTime.Parse("2021-03-19T09:56:27") },
        new() { Id = 97, MenuCategoryId = 98, MenuParentCategoryId = 25, MenuDisplayName = "Alfågel", MenuUrlSegment = "Alfagel", MenuDateUpdated = DateTime.Parse("2021-03-19T07:55:56") },
        new() { Id = 98, MenuCategoryId = 99, MenuParentCategoryId = 25, MenuDisplayName = "Bläsand", MenuUrlSegment = "Blasand", MenuDateUpdated = DateTime.Parse("2021-03-19T07:56:22") },
        new() { Id = 99, MenuCategoryId = 100, MenuParentCategoryId = 25, MenuDisplayName = "Ejder", MenuUrlSegment = "Ejder", MenuDateUpdated = DateTime.Parse("2021-03-01T09:36:50") },
        new() { Id = 100, MenuCategoryId = 101, MenuParentCategoryId = 25, MenuDisplayName = "Gräsand", MenuUrlSegment = "Grasand", MenuDateUpdated = DateTime.Parse("2021-03-19T07:56:35") },
        new() { Id = 101, MenuCategoryId = 102, MenuParentCategoryId = 25, MenuDisplayName = "Knipa", MenuUrlSegment = "Knipa", MenuDateUpdated = DateTime.Parse("2021-03-01T09:37:16") },
        new() { Id = 102, MenuCategoryId = 103, MenuParentCategoryId = 25, MenuDisplayName = "Kricka", MenuUrlSegment = "Kricka", MenuDateUpdated = DateTime.Parse("2021-03-19T07:49:32") },
        new() { Id = 103, MenuCategoryId = 104, MenuParentCategoryId = 25, MenuDisplayName = "Mandarinand", MenuUrlSegment = "Mandarinand", MenuDateUpdated = DateTime.Parse("2021-03-19T09:21:11") },
        new() { Id = 104, MenuCategoryId = 105, MenuParentCategoryId = 25, MenuDisplayName = "Praktejder", MenuUrlSegment = "Praktejder", MenuDateUpdated = DateTime.Parse("2021-03-23T21:10:45") },
        new() { Id = 105, MenuCategoryId = 106, MenuParentCategoryId = 25, MenuDisplayName = "Salskrake", MenuUrlSegment = "Salskrake", MenuDateUpdated = DateTime.Parse("2021-03-19T09:18:46") },
        new() { Id = 106, MenuCategoryId = 107, MenuParentCategoryId = 25, MenuDisplayName = "Storskrake", MenuUrlSegment = "Storskrake", MenuDateUpdated = DateTime.Parse("2021-03-22T15:53:49") },
        new() { Id = 107, MenuCategoryId = 108, MenuParentCategoryId = 46, MenuDisplayName = "Bläsgås", MenuUrlSegment = "Blasgas", MenuDateUpdated = DateTime.Parse("2021-03-19T07:52:42") },
        new() { Id = 108, MenuCategoryId = 109, MenuParentCategoryId = 46, MenuDisplayName = "Grågås", MenuUrlSegment = "Gragas", MenuDateUpdated = DateTime.Parse("2021-03-24T00:22:00") },
        new() { Id = 109, MenuCategoryId = 110, MenuParentCategoryId = 46, MenuDisplayName = "Kanadagås", MenuUrlSegment = "Kanadagas", MenuDateUpdated = DateTime.Parse("2021-03-19T07:57:00") },
        new() { Id = 110, MenuCategoryId = 111, MenuParentCategoryId = 46, MenuDisplayName = "Prutgås", MenuUrlSegment = "Prutgas", MenuDateUpdated = DateTime.Parse("2021-03-19T07:50:10") },
        new() { Id = 111, MenuCategoryId = 112, MenuParentCategoryId = 46, MenuDisplayName = "Sädgås", MenuUrlSegment = "Sadgas", MenuDateUpdated = DateTime.Parse("2021-03-19T13:55:42") },
        new() { Id = 112, MenuCategoryId = 113, MenuParentCategoryId = 46, MenuDisplayName = "Vitkindad gås", MenuUrlSegment = "Vitkindad-gas", MenuDateUpdated = DateTime.Parse("2021-03-19T09:23:09") },
        new() { Id = 113, MenuCategoryId = 114, MenuParentCategoryId = 1, MenuDisplayName = "Kråkfåglar", MenuUrlSegment = "Krakfaglar", MenuDateUpdated = DateTime.Parse("2021-03-01T05:15:56") },
        new() { Id = 114, MenuCategoryId = 115, MenuParentCategoryId = 114, MenuDisplayName = "Kaja", MenuUrlSegment = "Kaja", MenuDateUpdated = DateTime.Parse("2021-03-19T09:19:12") },
        new() { Id = 115, MenuCategoryId = 116, MenuParentCategoryId = 114, MenuDisplayName = "Kråka", MenuUrlSegment = "Kraka", MenuDateUpdated = DateTime.Parse("2021-03-23T18:10:34") },
        new() { Id = 116, MenuCategoryId = 117, MenuParentCategoryId = 114, MenuDisplayName = "Lavskrika", MenuUrlSegment = "Lavskrika", MenuDateUpdated = DateTime.Parse("2021-03-19T09:30:03") },
        new() { Id = 117, MenuCategoryId = 118, MenuParentCategoryId = 114, MenuDisplayName = "Nötkråka", MenuUrlSegment = "Notkraka", MenuDateUpdated = DateTime.Parse("2021-03-19T09:32:51") },
        new() { Id = 118, MenuCategoryId = 119, MenuParentCategoryId = 114, MenuDisplayName = "Nötskrika", MenuUrlSegment = "Notskrika", MenuDateUpdated = DateTime.Parse("2021-03-19T09:33:20") },
        new() { Id = 119, MenuCategoryId = 120, MenuParentCategoryId = 114, MenuDisplayName = "Råka", MenuUrlSegment = "Raka", MenuDateUpdated = DateTime.Parse("2021-03-10T08:31:40") },
        new() { Id = 120, MenuCategoryId = 121, MenuParentCategoryId = 114, MenuDisplayName = "Skata", MenuUrlSegment = "Skata", MenuDateUpdated = DateTime.Parse("2021-03-19T09:20:23") },
        new() { Id = 121, MenuCategoryId = 122, MenuParentCategoryId = 2, MenuDisplayName = "Igelkott", MenuUrlSegment = "Igelkott", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:08") },
        new() { Id = 122, MenuCategoryId = 123, MenuParentCategoryId = 2, MenuDisplayName = "Björn", MenuUrlSegment = "Bjorn", MenuDateUpdated = DateTime.Parse("2021-03-17T14:38:59") },
        new() { Id = 123, MenuCategoryId = 124, MenuParentCategoryId = 2, MenuDisplayName = "Vildren", MenuUrlSegment = "Vildren", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:11") },
        new() { Id = 124, MenuCategoryId = 125, MenuParentCategoryId = 2, MenuDisplayName = "Knubbsäl", MenuUrlSegment = "Knubbsal", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:08") },
        new() { Id = 125, MenuCategoryId = 126, MenuParentCategoryId = 24, MenuDisplayName = "Vit stork", MenuUrlSegment = "Vit-stork", MenuDateUpdated = DateTime.Parse("2021-03-01T09:17:46") },
        new() { Id = 126, MenuCategoryId = 127, MenuParentCategoryId = 28, MenuDisplayName = "Dvärgmås", MenuUrlSegment = "Dvargmas", MenuDateUpdated = DateTime.Parse("2021-03-01T09:17:57") },
        new() { Id = 127, MenuCategoryId = 128, MenuParentCategoryId = 28, MenuDisplayName = "Fisktärna", MenuUrlSegment = "Fisktarna", MenuDateUpdated = DateTime.Parse("2021-03-01T09:18:11") },
        new() { Id = 128, MenuCategoryId = 129, MenuParentCategoryId = 28, MenuDisplayName = "Skrattmås", MenuUrlSegment = "Skrattmas", MenuDateUpdated = DateTime.Parse("2021-03-20T18:34:04") },
        new() { Id = 129, MenuCategoryId = 130, MenuParentCategoryId = 28, MenuDisplayName = "Småtärna", MenuUrlSegment = "Smatarna", MenuDateUpdated = DateTime.Parse("2021-03-01T09:18:42") },
        new() { Id = 130, MenuCategoryId = 131, MenuParentCategoryId = 24, MenuDisplayName = "Häger", MenuUrlSegment = "Hager", MenuDateUpdated = DateTime.Parse("2021-03-01T08:32:50") },
        new() { Id = 131, MenuCategoryId = 132, MenuParentCategoryId = 24, MenuDisplayName = "Trana", MenuUrlSegment = "Trana", MenuDateUpdated = DateTime.Parse("2021-03-20T18:33:57") },
        new() { Id = 132, MenuCategoryId = 133, MenuParentCategoryId = 3, MenuDisplayName = "Padda", MenuUrlSegment = "Padda", MenuDateUpdated = DateTime.Parse("2021-03-25T18:36:01") },
        new() { Id = 133, MenuCategoryId = 134, MenuParentCategoryId = 3, MenuDisplayName = "Åkergroda", MenuUrlSegment = "Akergroda", MenuDateUpdated = DateTime.Parse("2021-03-05T13:32:29") },
        new() { Id = 134, MenuCategoryId = 135, MenuParentCategoryId = 47, MenuDisplayName = "Björktrast", MenuUrlSegment = "Bjorktrast", MenuDateUpdated = DateTime.Parse("2021-03-01T07:26:24") },
        new() { Id = 135, MenuCategoryId = 136, MenuParentCategoryId = 47, MenuDisplayName = "Rödvingetrast", MenuUrlSegment = "Rodvingetrast", MenuDateUpdated = DateTime.Parse("2021-03-01T08:17:41") },
        new() { Id = 136, MenuCategoryId = 137, MenuParentCategoryId = 214, MenuDisplayName = "Sidensvans", MenuUrlSegment = "Sidensvans", MenuDateUpdated = DateTime.Parse("2021-03-19T10:00:33") },
        new() { Id = 137, MenuCategoryId = 138, MenuParentCategoryId = 214, MenuDisplayName = "Stare", MenuUrlSegment = "Stare", MenuDateUpdated = DateTime.Parse("2021-03-19T09:37:31") },
        new() { Id = 138, MenuCategoryId = 139, MenuParentCategoryId = 28, MenuDisplayName = "Gråtrut", MenuUrlSegment = "Gratrut", MenuDateUpdated = DateTime.Parse("2021-03-01T08:36:42") },
        new() { Id = 139, MenuCategoryId = 140, MenuParentCategoryId = 4, MenuDisplayName = "Almsnabbvinge", MenuUrlSegment = "Almsnabbvinge", MenuDateUpdated = DateTime.Parse("2021-03-01T06:23:19") },
        new() { Id = 140, MenuCategoryId = 142, MenuParentCategoryId = 4, MenuDisplayName = "Asknätfjäril", MenuUrlSegment = "Asknatfjaril", MenuDateUpdated = DateTime.Parse("2021-03-10T08:32:17") },
        new() { Id = 141, MenuCategoryId = 143, MenuParentCategoryId = 4, MenuDisplayName = "Aurorafjäril", MenuUrlSegment = "Aurorafjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T06:23:58") },
        new() { Id = 142, MenuCategoryId = 144, MenuParentCategoryId = 4, MenuDisplayName = "Bredbrämad bastardsvärmare", MenuUrlSegment = "Bredbramad-bastardsvarmare", MenuDateUpdated = DateTime.Parse("2021-03-01T09:27:16") },
        new() { Id = 143, MenuCategoryId = 145, MenuParentCategoryId = 4, MenuDisplayName = "Brunfläckig pärlemorfjäril", MenuUrlSegment = "Brunflackig-parlemorfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T09:29:37") },
        new() { Id = 144, MenuCategoryId = 146, MenuParentCategoryId = 4, MenuDisplayName = "Citronfjäril", MenuUrlSegment = "Citronfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T06:24:21") },
        new() { Id = 145, MenuCategoryId = 147, MenuParentCategoryId = 4, MenuDisplayName = "Dårgräsfjäril", MenuUrlSegment = "Dargrasfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T07:06:07") },
        new() { Id = 146, MenuCategoryId = 148, MenuParentCategoryId = 4, MenuDisplayName = "Eldsnabbvinge", MenuUrlSegment = "Eldsnabbvinge", MenuDateUpdated = DateTime.Parse("2021-03-01T06:24:31") },
        new() { Id = 147, MenuCategoryId = 149, MenuParentCategoryId = 4, MenuDisplayName = "Gullvivefjäril", MenuUrlSegment = "Gullvivefjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T06:55:14") },
        new() { Id = 148, MenuCategoryId = 150, MenuParentCategoryId = 4, MenuDisplayName = "Klubbsprötad bastardsvärmare", MenuUrlSegment = "Klubbsprotad-bastardsvarmare", MenuDateUpdated = DateTime.Parse("2021-03-19T13:56:14") },
        new() { Id = 149, MenuCategoryId = 151, MenuParentCategoryId = 4, MenuDisplayName = "Kvickgräsfjäril", MenuUrlSegment = "Kvickgrasfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T07:22:02") },
        new() { Id = 150, MenuCategoryId = 152, MenuParentCategoryId = 4, MenuDisplayName = "Pärlgräsfjäril", MenuUrlSegment = "Parlgrasfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T07:22:50") },
        new() { Id = 151, MenuCategoryId = 153, MenuParentCategoryId = 4, MenuDisplayName = "Rapsfjäril", MenuUrlSegment = "Rapsfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T06:11:26") },
        new() { Id = 152, MenuCategoryId = 154, MenuParentCategoryId = 4, MenuDisplayName = "Rovfjäril", MenuUrlSegment = "Rovfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T06:06:18") },
        new() { Id = 153, MenuCategoryId = 155, MenuParentCategoryId = 4, MenuDisplayName = "Sandgräsfjäril", MenuUrlSegment = "Sandgrasfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T07:08:46") },
        new() { Id = 154, MenuCategoryId = 156, MenuParentCategoryId = 4, MenuDisplayName = "Sexfläckig bastardsvärmare", MenuUrlSegment = "Sexflackig-bastardsvarmare", MenuDateUpdated = DateTime.Parse("2021-03-01T09:27:48") },
        new() { Id = 155, MenuCategoryId = 157, MenuParentCategoryId = 4, MenuDisplayName = "Silverblåvinge", MenuUrlSegment = "Silverblavinge", MenuDateUpdated = DateTime.Parse("2021-03-01T06:57:24") },
        new() { Id = 156, MenuCategoryId = 158, MenuParentCategoryId = 4, MenuDisplayName = "Silverstreckad pärlemorfjäril", MenuUrlSegment = "Silverstreckad-parlemorfjaril", MenuDateUpdated = DateTime.Parse("2021-03-05T22:05:47") },
        new() { Id = 157, MenuCategoryId = 159, MenuParentCategoryId = 4, MenuDisplayName = "Skogsgräsfjäril", MenuUrlSegment = "Skogsgrasfjaril", MenuDateUpdated = DateTime.Parse("2021-03-19T08:31:19") },
        new() { Id = 158, MenuCategoryId = 160, MenuParentCategoryId = 4, MenuDisplayName = "Skogsnätfjäril", MenuUrlSegment = "Skogsnatfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T07:09:35") },
        new() { Id = 159, MenuCategoryId = 161, MenuParentCategoryId = 4, MenuDisplayName = "Skogspärlemorfjäril", MenuUrlSegment = "Skogsparlemorfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T08:24:09") },
        new() { Id = 160, MenuCategoryId = 162, MenuParentCategoryId = 4, MenuDisplayName = "Skogsvitvinge", MenuUrlSegment = "Skogsvitvinge", MenuDateUpdated = DateTime.Parse("2021-03-01T06:26:12") },
        new() { Id = 161, MenuCategoryId = 163, MenuParentCategoryId = 4, MenuDisplayName = "Smultronvisslare", MenuUrlSegment = "Smultronvisslare", MenuDateUpdated = DateTime.Parse("2021-03-19T13:55:40") },
        new() { Id = 162, MenuCategoryId = 164, MenuParentCategoryId = 4, MenuDisplayName = "Sotnätfjäril", MenuUrlSegment = "Sotnatfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T06:45:48") },
        new() { Id = 163, MenuCategoryId = 165, MenuParentCategoryId = 4, MenuDisplayName = "Svartfläckig blåvinge", MenuUrlSegment = "Svartflackig-blavinge", MenuDateUpdated = DateTime.Parse("2021-03-01T08:34:22") },
        new() { Id = 164, MenuCategoryId = 166, MenuParentCategoryId = 4, MenuDisplayName = "Svavelgul höfjäril", MenuUrlSegment = "Svavelgul-hofjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T08:16:59") },
        new() { Id = 165, MenuCategoryId = 167, MenuParentCategoryId = 4, MenuDisplayName = "Tistelfjäril", MenuUrlSegment = "Tistelfjaril", MenuDateUpdated = DateTime.Parse("2021-03-07T16:27:23") },
        new() { Id = 166, MenuCategoryId = 168, MenuParentCategoryId = 4, MenuDisplayName = "Vinbärsfux", MenuUrlSegment = "Vinbarsfux", MenuDateUpdated = DateTime.Parse("2021-03-01T06:11:52") },
        new() { Id = 167, MenuCategoryId = 169, MenuParentCategoryId = 4, MenuDisplayName = "Väddnätfjäril", MenuUrlSegment = "Vaddnatfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T07:11:29") },
        new() { Id = 168, MenuCategoryId = 170, MenuParentCategoryId = 4, MenuDisplayName = "Älggräspärlemorfjäril", MenuUrlSegment = "Alggrasparlemorfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T09:16:37") },
        new() { Id = 169, MenuCategoryId = 171, MenuParentCategoryId = 4, MenuDisplayName = "Ängspärlemorfjäril", MenuUrlSegment = "Angsparlemorfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T08:25:42") },
        new() { Id = 170, MenuCategoryId = 172, MenuParentCategoryId = 36, MenuDisplayName = "Fyrfläckig trollslända", MenuUrlSegment = "Fyrflackig-trollslanda", MenuDateUpdated = DateTime.Parse("2021-03-01T08:35:31") },
        new() { Id = 171, MenuCategoryId = 173, MenuParentCategoryId = 36, MenuDisplayName = "Kungstrollslända", MenuUrlSegment = "Kungstrollslanda", MenuDateUpdated = DateTime.Parse("2021-03-19T13:56:25") },
        new() { Id = 172, MenuCategoryId = 177, MenuParentCategoryId = 48, MenuDisplayName = "Fjällabb", MenuUrlSegment = "Fjallabb", MenuDateUpdated = DateTime.Parse("2021-03-23T15:19:43") },
        new() { Id = 173, MenuCategoryId = 178, MenuParentCategoryId = 48, MenuDisplayName = "Kustlabb", MenuUrlSegment = "Kustlabb", MenuDateUpdated = DateTime.Parse("2021-03-23T15:19:45") },
        new() { Id = 174, MenuCategoryId = 179, MenuParentCategoryId = 48, MenuDisplayName = "Lunnefågel", MenuUrlSegment = "Lunnefagel", MenuDateUpdated = DateTime.Parse("2021-03-23T15:19:48") },
        new() { Id = 175, MenuCategoryId = 180, MenuParentCategoryId = 48, MenuDisplayName = "Sillgrissla", MenuUrlSegment = "Sillgrissla", MenuDateUpdated = DateTime.Parse("2021-03-23T15:19:50") },
        new() { Id = 176, MenuCategoryId = 181, MenuParentCategoryId = 48, MenuDisplayName = "Storlabb", MenuUrlSegment = "Storlabb", MenuDateUpdated = DateTime.Parse("2021-03-19T09:40:01") },
        new() { Id = 177, MenuCategoryId = 182, MenuParentCategoryId = 48, MenuDisplayName = "Tobisgrissla", MenuUrlSegment = "Tobisgrissla", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:12") },
        new() { Id = 178, MenuCategoryId = 183, MenuParentCategoryId = 48, MenuDisplayName = "Tordmule", MenuUrlSegment = "Tordmule", MenuDateUpdated = DateTime.Parse("2021-03-19T09:40:46") },
        new() { Id = 179, MenuCategoryId = 184, MenuParentCategoryId = 26, MenuDisplayName = "Skäggdopping", MenuUrlSegment = "Skaggdopping", MenuDateUpdated = DateTime.Parse("2021-03-23T19:32:08") },
        new() { Id = 180, MenuCategoryId = 185, MenuParentCategoryId = 26, MenuDisplayName = "Storskarv", MenuUrlSegment = "Storskarv", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:16") },
        new() { Id = 181, MenuCategoryId = 186, MenuParentCategoryId = 26, MenuDisplayName = "Toppskarv", MenuUrlSegment = "Toppskarv", MenuDateUpdated = DateTime.Parse("2021-03-19T13:55:34") },
        new() { Id = 182, MenuCategoryId = 187, MenuParentCategoryId = 47, MenuDisplayName = "Ringduva", MenuUrlSegment = "Ringduva", MenuDateUpdated = DateTime.Parse("2021-03-19T09:44:24") },
        new() { Id = 183, MenuCategoryId = 188, MenuParentCategoryId = 47, MenuDisplayName = "Större turturduva", MenuUrlSegment = "Storre-turturduva", MenuDateUpdated = DateTime.Parse("2021-03-01T08:40:33") },
        new() { Id = 184, MenuCategoryId = 189, MenuParentCategoryId = 47, MenuDisplayName = "Tamduva", MenuUrlSegment = "Tamduva", MenuDateUpdated = DateTime.Parse("2021-03-19T09:42:12") },
        new() { Id = 185, MenuCategoryId = 190, MenuParentCategoryId = 47, MenuDisplayName = "Turkduva", MenuUrlSegment = "Turkduva", MenuDateUpdated = DateTime.Parse("2021-03-01T06:50:58") },
        new() { Id = 186, MenuCategoryId = 191, MenuParentCategoryId = 1, MenuDisplayName = "Lommar", MenuUrlSegment = "Lommar", MenuDateUpdated = DateTime.Parse("2021-03-16T07:23:33") },
        new() { Id = 187, MenuCategoryId = 192, MenuParentCategoryId = 191, MenuDisplayName = "Smålom", MenuUrlSegment = "Smalom", MenuDateUpdated = DateTime.Parse("2021-03-02T10:20:44") },
        new() { Id = 188, MenuCategoryId = 193, MenuParentCategoryId = 10, MenuDisplayName = "Duvhök", MenuUrlSegment = "Duvhok", MenuDateUpdated = DateTime.Parse("2021-03-19T09:24:59") },
        new() { Id = 189, MenuCategoryId = 194, MenuParentCategoryId = 10, MenuDisplayName = "Fjällvråk", MenuUrlSegment = "Fjallvrak", MenuDateUpdated = DateTime.Parse("2021-03-19T09:41:29") },
        new() { Id = 190, MenuCategoryId = 195, MenuParentCategoryId = 10, MenuDisplayName = "Röd glada", MenuUrlSegment = "Rod-glada", MenuDateUpdated = DateTime.Parse("2021-03-23T21:53:25") },
        new() { Id = 191, MenuCategoryId = 196, MenuParentCategoryId = 10, MenuDisplayName = "Jaktfalk", MenuUrlSegment = "Jaktfalk", MenuDateUpdated = DateTime.Parse("2021-03-23T21:33:17") },
        new() { Id = 192, MenuCategoryId = 197, MenuParentCategoryId = 10, MenuDisplayName = "Kungsörn", MenuUrlSegment = "Kungsorn", MenuDateUpdated = DateTime.Parse("2021-03-23T13:26:16") },
        new() { Id = 193, MenuCategoryId = 198, MenuParentCategoryId = 10, MenuDisplayName = "Ormvråk", MenuUrlSegment = "Ormvrak", MenuDateUpdated = DateTime.Parse("2021-03-19T09:28:54") },
        new() { Id = 194, MenuCategoryId = 199, MenuParentCategoryId = 10, MenuDisplayName = "Pilgrimsfalk", MenuUrlSegment = "Pilgrimsfalk", MenuDateUpdated = DateTime.Parse("2021-03-23T21:52:44") },
        new() { Id = 195, MenuCategoryId = 200, MenuParentCategoryId = 10, MenuDisplayName = "Sparvhök", MenuUrlSegment = "Sparvhok", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:14") },
        new() { Id = 196, MenuCategoryId = 201, MenuParentCategoryId = 10, MenuDisplayName = "Tornfalk", MenuUrlSegment = "Tornfalk", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:15") },
        new() { Id = 197, MenuCategoryId = 202, MenuParentCategoryId = 9, MenuDisplayName = "2008 Costa Rica", MenuUrlSegment = "2008-Costa-Rica", MenuDateUpdated = DateTime.Parse("2021-03-17T15:29:04") },
        new() { Id = 198, MenuCategoryId = 203, MenuParentCategoryId = 22, MenuDisplayName = "Vattenrall", MenuUrlSegment = "Vattenrall", MenuDateUpdated = DateTime.Parse("2021-03-19T09:19:52") },
        new() { Id = 199, MenuCategoryId = 204, MenuParentCategoryId = 23, MenuDisplayName = "Finkar och siskor", MenuUrlSegment = "Finkar-Siskor", MenuDateUpdated = DateTime.Parse("2021-03-04T08:03:42") },
        new() { Id = 200, MenuCategoryId = 205, MenuParentCategoryId = 23, MenuDisplayName = "Flugsnappare", MenuUrlSegment = "Flugsnappare", MenuDateUpdated = DateTime.Parse("2021-03-01T21:25:14") },
        new() { Id = 201, MenuCategoryId = 206, MenuParentCategoryId = 23, MenuDisplayName = "Korsnäbbar", MenuUrlSegment = "Korsnabbar", MenuDateUpdated = DateTime.Parse("2021-03-25T20:29:33") },
        new() { Id = 202, MenuCategoryId = 207, MenuParentCategoryId = 23, MenuDisplayName = "Lärkor", MenuUrlSegment = "Larkor", MenuDateUpdated = DateTime.Parse("2021-03-18T22:57:45") },
        new() { Id = 203, MenuCategoryId = 208, MenuParentCategoryId = 23, MenuDisplayName = "Mesar", MenuUrlSegment = "Mesar", MenuDateUpdated = DateTime.Parse("2021-03-01T21:30:22") },
        new() { Id = 204, MenuCategoryId = 209, MenuParentCategoryId = 23, MenuDisplayName = "Skvättor", MenuUrlSegment = "Skvattor", MenuDateUpdated = DateTime.Parse("2021-03-18T18:35:41") },
        new() { Id = 205, MenuCategoryId = 210, MenuParentCategoryId = 23, MenuDisplayName = "Sparvar", MenuUrlSegment = "Sparvar", MenuDateUpdated = DateTime.Parse("2021-03-22T08:43:30") },
        new() { Id = 206, MenuCategoryId = 211, MenuParentCategoryId = 23, MenuDisplayName = "Svalor", MenuUrlSegment = "Svalor", MenuDateUpdated = DateTime.Parse("2021-03-01T21:25:03") },
        new() { Id = 207, MenuCategoryId = 212, MenuParentCategoryId = 23, MenuDisplayName = "Sångare", MenuUrlSegment = "Sangare", MenuDateUpdated = DateTime.Parse("2021-03-01T21:30:12") },
        new() { Id = 208, MenuCategoryId = 213, MenuParentCategoryId = 23, MenuDisplayName = "Ärlor", MenuUrlSegment = "Arlor", MenuDateUpdated = DateTime.Parse("2021-03-01T21:24:59") },
        new() { Id = 209, MenuCategoryId = 214, MenuParentCategoryId = 23, MenuDisplayName = "Övriga", MenuUrlSegment = "Ovriga", MenuDateUpdated = DateTime.Parse("2021-03-01T21:24:57") },
        new() { Id = 210, MenuCategoryId = 215, MenuParentCategoryId = 214, MenuDisplayName = "Blåstjärt", MenuUrlSegment = "Blastjart", MenuDateUpdated = DateTime.Parse("2021-03-19T10:02:50") },
        new() { Id = 211, MenuCategoryId = 216, MenuParentCategoryId = 214, MenuDisplayName = "Kungsfiskare", MenuUrlSegment = "Kungsfiskare", MenuDateUpdated = DateTime.Parse("2021-03-06T21:34:20") },
        new() { Id = 212, MenuCategoryId = 217, MenuParentCategoryId = 214, MenuDisplayName = "Kungsfågel", MenuUrlSegment = "Kungsfagel", MenuDateUpdated = DateTime.Parse("2021-03-19T10:03:03") },
        new() { Id = 213, MenuCategoryId = 218, MenuParentCategoryId = 214, MenuDisplayName = "Nattskärra", MenuUrlSegment = "Nattskarra", MenuDateUpdated = DateTime.Parse("2021-03-19T10:03:28") },
        new() { Id = 214, MenuCategoryId = 219, MenuParentCategoryId = 214, MenuDisplayName = "Nötväcka", MenuUrlSegment = "Notvacka", MenuDateUpdated = DateTime.Parse("2021-03-19T09:59:40") },
        new() { Id = 215, MenuCategoryId = 220, MenuParentCategoryId = 214, MenuDisplayName = "Rödhake", MenuUrlSegment = "Rodhake", MenuDateUpdated = DateTime.Parse("2021-03-23T19:25:52") },
        new() { Id = 216, MenuCategoryId = 221, MenuParentCategoryId = 214, MenuDisplayName = "Rödhuvad törnskata", MenuUrlSegment = "Rodhuvad-tornskata", MenuDateUpdated = DateTime.Parse("2021-03-01T09:26:36") },
        new() { Id = 217, MenuCategoryId = 222, MenuParentCategoryId = 214, MenuDisplayName = "Strömstare", MenuUrlSegment = "Stromstare", MenuDateUpdated = DateTime.Parse("2021-03-01T08:05:58") },
        new() { Id = 218, MenuCategoryId = 223, MenuParentCategoryId = 214, MenuDisplayName = "Svart rödstjärt", MenuUrlSegment = "Svart-rodstjart", MenuDateUpdated = DateTime.Parse("2021-03-01T09:15:03") },
        new() { Id = 219, MenuCategoryId = 224, MenuParentCategoryId = 214, MenuDisplayName = "Trädkrypare", MenuUrlSegment = "Tradkrypare", MenuDateUpdated = DateTime.Parse("2021-03-01T08:15:30") },
        new() { Id = 220, MenuCategoryId = 225, MenuParentCategoryId = 214, MenuDisplayName = "Törnskata", MenuUrlSegment = "Tornskata", MenuDateUpdated = DateTime.Parse("2021-03-19T10:01:10") },
        new() { Id = 221, MenuCategoryId = 226, MenuParentCategoryId = 214, MenuDisplayName = "Varfågel", MenuUrlSegment = "Varfagel", MenuDateUpdated = DateTime.Parse("2021-03-19T09:53:45") },
        new() { Id = 222, MenuCategoryId = 227, MenuParentCategoryId = 27, MenuDisplayName = "Göktyta", MenuUrlSegment = "Goktyta", MenuDateUpdated = DateTime.Parse("2021-03-19T09:23:44") },
        new() { Id = 223, MenuCategoryId = 228, MenuParentCategoryId = 27, MenuDisplayName = "Spillkråka", MenuUrlSegment = "Spillkraka", MenuDateUpdated = DateTime.Parse("2021-03-19T09:32:28") },
        new() { Id = 224, MenuCategoryId = 229, MenuParentCategoryId = 27, MenuDisplayName = "Större hackspett", MenuUrlSegment = "Storre-hackspett", MenuDateUpdated = DateTime.Parse("2021-03-01T07:24:54") },
        new() { Id = 225, MenuCategoryId = 230, MenuParentCategoryId = 204, MenuDisplayName = "Bofink", MenuUrlSegment = "Bofink", MenuDateUpdated = DateTime.Parse("2021-03-01T08:38:17") },
        new() { Id = 226, MenuCategoryId = 231, MenuParentCategoryId = 204, MenuDisplayName = "Grönfink", MenuUrlSegment = "Gronfink", MenuDateUpdated = DateTime.Parse("2021-03-01T09:22:19") },
        new() { Id = 227, MenuCategoryId = 232, MenuParentCategoryId = 204, MenuDisplayName = "Grönsiska", MenuUrlSegment = "Gronsiska", MenuDateUpdated = DateTime.Parse("2021-03-01T09:26:00") },
        new() { Id = 228, MenuCategoryId = 233, MenuParentCategoryId = 204, MenuDisplayName = "Hämpling", MenuUrlSegment = "Hampling", MenuDateUpdated = DateTime.Parse("2021-03-01T09:22:29") },
        new() { Id = 229, MenuCategoryId = 234, MenuParentCategoryId = 204, MenuDisplayName = "Pilfink", MenuUrlSegment = "Pilfink", MenuDateUpdated = DateTime.Parse("2021-03-01T08:40:56") },
        new() { Id = 230, MenuCategoryId = 235, MenuParentCategoryId = 205, MenuDisplayName = "Grå flugsnappare", MenuUrlSegment = "Gra-flugsnappare", MenuDateUpdated = DateTime.Parse("2021-03-01T09:31:47") },
        new() { Id = 231, MenuCategoryId = 236, MenuParentCategoryId = 205, MenuDisplayName = "Halsbandsflugsnappare", MenuUrlSegment = "Halsbandsflugsnappare", MenuDateUpdated = DateTime.Parse("2021-03-01T09:35:56") },
        new() { Id = 232, MenuCategoryId = 237, MenuParentCategoryId = 205, MenuDisplayName = "Mindre flugsnappare", MenuUrlSegment = "Mindre-flugsnappare", MenuDateUpdated = DateTime.Parse("2021-03-17T08:47:58") },
        new() { Id = 233, MenuCategoryId = 238, MenuParentCategoryId = 205, MenuDisplayName = "Svartvit flugsnappare", MenuUrlSegment = "Svartvit-flugsnappare", MenuDateUpdated = DateTime.Parse("2021-03-01T09:36:16") },
        new() { Id = 234, MenuCategoryId = 239, MenuParentCategoryId = 206, MenuDisplayName = "Mindre korsnäbb", MenuUrlSegment = "Mindre-korsnabb", MenuDateUpdated = DateTime.Parse("2021-03-25T20:29:09") },
        new() { Id = 235, MenuCategoryId = 240, MenuParentCategoryId = 206, MenuDisplayName = "Tallbit", MenuUrlSegment = "Tallbit", MenuDateUpdated = DateTime.Parse("2021-03-25T20:43:52") },
        new() { Id = 236, MenuCategoryId = 241, MenuParentCategoryId = 207, MenuDisplayName = "Berglärka", MenuUrlSegment = "Berglarka", MenuDateUpdated = DateTime.Parse("2021-03-19T09:57:19") },
        new() { Id = 237, MenuCategoryId = 242, MenuParentCategoryId = 207, MenuDisplayName = "Sånglärka", MenuUrlSegment = "Sanglarka", MenuDateUpdated = DateTime.Parse("2021-03-19T19:39:27") },
        new() { Id = 238, MenuCategoryId = 243, MenuParentCategoryId = 208, MenuDisplayName = "Blåmes", MenuUrlSegment = "Blames", MenuDateUpdated = DateTime.Parse("2021-03-01T06:21:49") },
        new() { Id = 239, MenuCategoryId = 244, MenuParentCategoryId = 208, MenuDisplayName = "Entita", MenuUrlSegment = "Entita", MenuDateUpdated = DateTime.Parse("2021-03-19T09:34:51") },
        new() { Id = 240, MenuCategoryId = 245, MenuParentCategoryId = 208, MenuDisplayName = "Pungmes", MenuUrlSegment = "Pungmes", MenuDateUpdated = DateTime.Parse("2021-03-01T06:22:11") },
        new() { Id = 241, MenuCategoryId = 246, MenuParentCategoryId = 208, MenuDisplayName = "Skäggmes", MenuUrlSegment = "Skaggmes", MenuDateUpdated = DateTime.Parse("2021-03-19T09:45:06") },
        new() { Id = 242, MenuCategoryId = 247, MenuParentCategoryId = 208, MenuDisplayName = "Stjärtmes", MenuUrlSegment = "Stjartmes", MenuDateUpdated = DateTime.Parse("2021-03-06T21:35:55") },
        new() { Id = 243, MenuCategoryId = 248, MenuParentCategoryId = 208, MenuDisplayName = "Svartmes", MenuUrlSegment = "Svartmes", MenuDateUpdated = DateTime.Parse("2021-03-23T10:33:29") },
        new() { Id = 244, MenuCategoryId = 249, MenuParentCategoryId = 208, MenuDisplayName = "Talgoxe", MenuUrlSegment = "Talgoxe", MenuDateUpdated = DateTime.Parse("2021-03-20T18:34:12") },
        new() { Id = 245, MenuCategoryId = 250, MenuParentCategoryId = 208, MenuDisplayName = "Tofsmes", MenuUrlSegment = "Tofsmes", MenuDateUpdated = DateTime.Parse("2021-03-19T09:37:00") },
        new() { Id = 246, MenuCategoryId = 251, MenuParentCategoryId = 209, MenuDisplayName = "Buskskvätta", MenuUrlSegment = "Buskskvatta", MenuDateUpdated = DateTime.Parse("2021-03-01T08:27:58") },
        new() { Id = 247, MenuCategoryId = 252, MenuParentCategoryId = 209, MenuDisplayName = "Stenskvätta", MenuUrlSegment = "Stenskvatta", MenuDateUpdated = DateTime.Parse("2021-03-01T08:28:24") },
        new() { Id = 248, MenuCategoryId = 253, MenuParentCategoryId = 210, MenuDisplayName = "Gråsparv", MenuUrlSegment = "Grasparv", MenuDateUpdated = DateTime.Parse("2021-03-19T09:51:41") },
        new() { Id = 249, MenuCategoryId = 254, MenuParentCategoryId = 210, MenuDisplayName = "Gulsparv", MenuUrlSegment = "Gulsparv", MenuDateUpdated = DateTime.Parse("2021-03-14T10:26:00") },
        new() { Id = 250, MenuCategoryId = 255, MenuParentCategoryId = 210, MenuDisplayName = "Järnsparv", MenuUrlSegment = "Jarnsparv", MenuDateUpdated = DateTime.Parse("2021-03-19T09:57:51") },
        new() { Id = 251, MenuCategoryId = 256, MenuParentCategoryId = 210, MenuDisplayName = "Snösparv", MenuUrlSegment = "Snosparv", MenuDateUpdated = DateTime.Parse("2021-03-19T09:52:54") },
        new() { Id = 252, MenuCategoryId = 257, MenuParentCategoryId = 210, MenuDisplayName = "Svartstrupig järnsparv", MenuUrlSegment = "Svartstrupig-jarnsparv", MenuDateUpdated = DateTime.Parse("2021-03-01T09:32:25") },
        new() { Id = 253, MenuCategoryId = 258, MenuParentCategoryId = 210, MenuDisplayName = "Sävsparv", MenuUrlSegment = "Savsparv", MenuDateUpdated = DateTime.Parse("2021-03-19T09:53:12") },
        new() { Id = 254, MenuCategoryId = 259, MenuParentCategoryId = 211, MenuDisplayName = "Backsvala", MenuUrlSegment = "Backsvala", MenuDateUpdated = DateTime.Parse("2021-03-14T10:26:07") },
        new() { Id = 255, MenuCategoryId = 260, MenuParentCategoryId = 211, MenuDisplayName = "Hussvala", MenuUrlSegment = "Hussvala", MenuDateUpdated = DateTime.Parse("2021-03-01T06:52:05") },
        new() { Id = 256, MenuCategoryId = 261, MenuParentCategoryId = 211, MenuDisplayName = "Ladusvala", MenuUrlSegment = "Ladusvala", MenuDateUpdated = DateTime.Parse("2021-03-19T09:47:16") },
        new() { Id = 257, MenuCategoryId = 262, MenuParentCategoryId = 211, MenuDisplayName = "Tornsvala", MenuUrlSegment = "Tornsvala", MenuDateUpdated = DateTime.Parse("2021-03-01T07:01:32") },
        new() { Id = 258, MenuCategoryId = 263, MenuParentCategoryId = 212, MenuDisplayName = "Grönsångare", MenuUrlSegment = "Gronsangare", MenuDateUpdated = DateTime.Parse("2021-03-01T08:29:02") },
        new() { Id = 259, MenuCategoryId = 264, MenuParentCategoryId = 212, MenuDisplayName = "Lövsångare", MenuUrlSegment = "Lovsangare", MenuDateUpdated = DateTime.Parse("2021-03-20T18:34:15") },
        new() { Id = 260, MenuCategoryId = 265, MenuParentCategoryId = 212, MenuDisplayName = "Rörsångare", MenuUrlSegment = "Rorsangare", MenuDateUpdated = DateTime.Parse("2021-03-01T08:22:05") },
        new() { Id = 261, MenuCategoryId = 266, MenuParentCategoryId = 212, MenuDisplayName = "Svarthätta", MenuUrlSegment = "Svarthatta", MenuDateUpdated = DateTime.Parse("2021-03-01T08:14:24") },
        new() { Id = 262, MenuCategoryId = 267, MenuParentCategoryId = 212, MenuDisplayName = "Sävsångare", MenuUrlSegment = "Savsangare", MenuDateUpdated = DateTime.Parse("2021-03-01T08:22:38") },
        new() { Id = 263, MenuCategoryId = 268, MenuParentCategoryId = 212, MenuDisplayName = "Trastsångare", MenuUrlSegment = "Trastsangare", MenuDateUpdated = DateTime.Parse("2021-03-01T08:29:48") },
        new() { Id = 264, MenuCategoryId = 269, MenuParentCategoryId = 212, MenuDisplayName = "Törnsångare", MenuUrlSegment = "Tornsangare", MenuDateUpdated = DateTime.Parse("2021-03-01T08:30:24") },
        new() { Id = 265, MenuCategoryId = 270, MenuParentCategoryId = 212, MenuDisplayName = "Ärtsångare", MenuUrlSegment = "Artsangare", MenuDateUpdated = DateTime.Parse("2021-03-01T08:23:01") },
        new() { Id = 266, MenuCategoryId = 271, MenuParentCategoryId = 213, MenuDisplayName = "Gulärla", MenuUrlSegment = "Gularla", MenuDateUpdated = DateTime.Parse("2021-03-19T09:45:25") },
        new() { Id = 267, MenuCategoryId = 272, MenuParentCategoryId = 213, MenuDisplayName = "Sädesärla", MenuUrlSegment = "Sadesarla", MenuDateUpdated = DateTime.Parse("2021-03-19T09:59:04") },
        new() { Id = 268, MenuCategoryId = 273, MenuParentCategoryId = 22, MenuDisplayName = "Pipare", MenuUrlSegment = "Pipare", MenuDateUpdated = DateTime.Parse("2021-03-01T05:18:13") },
        new() { Id = 269, MenuCategoryId = 274, MenuParentCategoryId = 22, MenuDisplayName = "Spovar", MenuUrlSegment = "Spovar", MenuDateUpdated = DateTime.Parse("2021-03-01T05:18:27") },
        new() { Id = 270, MenuCategoryId = 275, MenuParentCategoryId = 288, MenuDisplayName = "Spovsnäppa", MenuUrlSegment = "Spovsnappa", MenuDateUpdated = DateTime.Parse("2021-03-19T09:48:59") },
        new() { Id = 271, MenuCategoryId = 276, MenuParentCategoryId = 22, MenuDisplayName = "Brushane", MenuUrlSegment = "Brushane", MenuDateUpdated = DateTime.Parse("2021-03-25T05:37:19") },
        new() { Id = 272, MenuCategoryId = 277, MenuParentCategoryId = 288, MenuDisplayName = "Drillsnäppa", MenuUrlSegment = "Drillsnappa", MenuDateUpdated = DateTime.Parse("2021-03-01T07:19:20") },
        new() { Id = 273, MenuCategoryId = 278, MenuParentCategoryId = 22, MenuDisplayName = "Enkelbeckasin", MenuUrlSegment = "Enkelbeckasin", MenuDateUpdated = DateTime.Parse("2021-03-19T09:24:20") },
        new() { Id = 274, MenuCategoryId = 279, MenuParentCategoryId = 22, MenuDisplayName = "Grönbena", MenuUrlSegment = "Gronbena", MenuDateUpdated = DateTime.Parse("2021-03-19T09:17:26") },
        new() { Id = 275, MenuCategoryId = 280, MenuParentCategoryId = 24, MenuDisplayName = "Kohäger", MenuUrlSegment = "Kohager", MenuDateUpdated = DateTime.Parse("2021-03-01T08:40:19") },
        new() { Id = 276, MenuCategoryId = 281, MenuParentCategoryId = 273, MenuDisplayName = "Kustpipare", MenuUrlSegment = "Kustpipare", MenuDateUpdated = DateTime.Parse("2021-03-19T09:37:39") },
        new() { Id = 277, MenuCategoryId = 282, MenuParentCategoryId = 288, MenuDisplayName = "Kustsnäppa", MenuUrlSegment = "Kustsnappa", MenuDateUpdated = DateTime.Parse("2021-03-01T07:03:54") },
        new() { Id = 278, MenuCategoryId = 283, MenuParentCategoryId = 288, MenuDisplayName = "Kärrsnäppa sydlig", MenuUrlSegment = "Karrsnappa-sydlig", MenuDateUpdated = DateTime.Parse("2021-03-01T08:38:58") },
        new() { Id = 279, MenuCategoryId = 284, MenuParentCategoryId = 273, MenuDisplayName = "Ljungpipare", MenuUrlSegment = "Ljungpipare", MenuDateUpdated = DateTime.Parse("2021-03-19T09:42:29") },
        new() { Id = 280, MenuCategoryId = 285, MenuParentCategoryId = 273, MenuDisplayName = "Mindre strandpipare", MenuUrlSegment = "Mindre-strandpipare", MenuDateUpdated = DateTime.Parse("2021-03-01T08:30:36") },
        new() { Id = 281, MenuCategoryId = 286, MenuParentCategoryId = 273, MenuDisplayName = "Större strandpipare", MenuUrlSegment = "Storre-strandpipare", MenuDateUpdated = DateTime.Parse("2021-03-01T08:33:08") },
        new() { Id = 282, MenuCategoryId = 287, MenuParentCategoryId = 288, MenuDisplayName = "Mosnäppa", MenuUrlSegment = "Mosnappa", MenuDateUpdated = DateTime.Parse("2021-03-19T09:43:11") },
        new() { Id = 283, MenuCategoryId = 288, MenuParentCategoryId = 22, MenuDisplayName = "Snäppor", MenuUrlSegment = "Snappor", MenuDateUpdated = DateTime.Parse("2021-03-01T05:23:50") },
        new() { Id = 284, MenuCategoryId = 289, MenuParentCategoryId = 288, MenuDisplayName = "Smalnäbbad simsnäppa", MenuUrlSegment = "Smalnabbad-simsnappa", MenuDateUpdated = DateTime.Parse("2021-03-01T09:24:53") },
        new() { Id = 285, MenuCategoryId = 290, MenuParentCategoryId = 288, MenuDisplayName = "Småsnäppa", MenuUrlSegment = "Smasnappa", MenuDateUpdated = DateTime.Parse("2021-03-19T09:48:42") },
        new() { Id = 286, MenuCategoryId = 291, MenuParentCategoryId = 288, MenuDisplayName = "Svartsnäppa", MenuUrlSegment = "Svartsnappa", MenuDateUpdated = DateTime.Parse("2021-03-19T09:55:58") },
        new() { Id = 287, MenuCategoryId = 292, MenuParentCategoryId = 288, MenuDisplayName = "Tuvsnäppa", MenuUrlSegment = "Tuvsnappa", MenuDateUpdated = DateTime.Parse("2021-03-19T09:46:09") },
        new() { Id = 288, MenuCategoryId = 293, MenuParentCategoryId = 274, MenuDisplayName = "Myrspov", MenuUrlSegment = "Myrspov", MenuDateUpdated = DateTime.Parse("2021-03-19T09:27:53") },
        new() { Id = 289, MenuCategoryId = 294, MenuParentCategoryId = 274, MenuDisplayName = "Småspov", MenuUrlSegment = "Smaspov", MenuDateUpdated = DateTime.Parse("2021-03-01T06:10:14") },
        new() { Id = 290, MenuCategoryId = 295, MenuParentCategoryId = 22, MenuDisplayName = "Prärielöpare", MenuUrlSegment = "Prarielopare", MenuDateUpdated = DateTime.Parse("2021-03-19T09:27:10") },
        new() { Id = 291, MenuCategoryId = 296, MenuParentCategoryId = 22, MenuDisplayName = "Rödbena", MenuUrlSegment = "Rodbena", MenuDateUpdated = DateTime.Parse("2021-03-01T05:23:06") },
        new() { Id = 292, MenuCategoryId = 297, MenuParentCategoryId = 22, MenuDisplayName = "Rördrom", MenuUrlSegment = "Rordrom", MenuDateUpdated = DateTime.Parse("2021-03-01T05:23:31") },
        new() { Id = 293, MenuCategoryId = 298, MenuParentCategoryId = 22, MenuDisplayName = "Rörhöna", MenuUrlSegment = "Rorhona", MenuDateUpdated = DateTime.Parse("2021-03-19T09:18:19") },
        new() { Id = 294, MenuCategoryId = 299, MenuParentCategoryId = 22, MenuDisplayName = "Sandlöpare", MenuUrlSegment = "Sandlopare", MenuDateUpdated = DateTime.Parse("2021-03-19T09:20:35") },
        new() { Id = 295, MenuCategoryId = 300, MenuParentCategoryId = 22, MenuDisplayName = "Sothöna", MenuUrlSegment = "Sothona", MenuDateUpdated = DateTime.Parse("2021-03-19T07:54:59") },
        new() { Id = 296, MenuCategoryId = 301, MenuParentCategoryId = 22, MenuDisplayName = "Strandskata", MenuUrlSegment = "Strandskata", MenuDateUpdated = DateTime.Parse("2021-03-19T09:20:48") },
        new() { Id = 297, MenuCategoryId = 302, MenuParentCategoryId = 22, MenuDisplayName = "Svartvingad vadarsvala", MenuUrlSegment = "Svartvingad-vadarsvala", MenuDateUpdated = DateTime.Parse("2021-03-01T07:58:10") },
        new() { Id = 298, MenuCategoryId = 303, MenuParentCategoryId = 6, MenuDisplayName = "Blommor", MenuUrlSegment = "Blommor", MenuDateUpdated = DateTime.Parse("2021-03-25T18:36:38") },
        new() { Id = 299, MenuCategoryId = 304, MenuParentCategoryId = 22, MenuDisplayName = "Dubbelbeckasin", MenuUrlSegment = "Dubbelbeckasin", MenuDateUpdated = DateTime.Parse("2021-03-19T09:26:27") },
        new() { Id = 300, MenuCategoryId = 305, MenuParentCategoryId = 36, MenuDisplayName = "Flickslända", MenuUrlSegment = "Flickslanda", MenuDateUpdated = DateTime.Parse("2021-03-01T06:11:59") },
        new() { Id = 301, MenuCategoryId = 306, MenuParentCategoryId = 4, MenuDisplayName = "Allmän poppelglasvinge", MenuUrlSegment = "Allman-poppelglasvinge", MenuDateUpdated = DateTime.Parse("2021-03-01T08:33:34") },
        new() { Id = 302, MenuCategoryId = 307, MenuParentCategoryId = 4, MenuDisplayName = "Brunsprötad skymningssvärmare", MenuUrlSegment = "Brunsprotad-skymningssvarmare", MenuDateUpdated = DateTime.Parse("2021-03-01T09:33:43") },
        new() { Id = 303, MenuCategoryId = 308, MenuParentCategoryId = 4, MenuDisplayName = "Humlelik dagsvärmare", MenuUrlSegment = "Humlelik-dagsvarmare", MenuDateUpdated = DateTime.Parse("2021-03-24T15:40:50") },
        new() { Id = 304, MenuCategoryId = 309, MenuParentCategoryId = 4, MenuDisplayName = "Kamgräsfjäril", MenuUrlSegment = "Kamgrasfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T06:56:05") },
        new() { Id = 305, MenuCategoryId = 310, MenuParentCategoryId = 3, MenuDisplayName = "Kopparödla", MenuUrlSegment = "Kopparodla", MenuDateUpdated = DateTime.Parse("2021-03-05T13:32:34") },
        new() { Id = 306, MenuCategoryId = 311, MenuParentCategoryId = 4, MenuDisplayName = "Ligustersvärmare", MenuUrlSegment = "Ligustersvarmare", MenuDateUpdated = DateTime.Parse("2021-03-01T07:22:27") },
        new() { Id = 307, MenuCategoryId = 312, MenuParentCategoryId = 4, MenuDisplayName = "Ljungblåvinge", MenuUrlSegment = "Ljungblavinge", MenuDateUpdated = DateTime.Parse("2021-03-01T06:44:23") },
        new() { Id = 308, MenuCategoryId = 313, MenuParentCategoryId = 4, MenuDisplayName = "Luktgräsfjäril", MenuUrlSegment = "Luktgrasfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T07:07:08") },
        new() { Id = 309, MenuCategoryId = 314, MenuParentCategoryId = 4, MenuDisplayName = "Mindre blåvinge", MenuUrlSegment = "Mindre-blavinge", MenuDateUpdated = DateTime.Parse("2021-03-01T07:07:33") },
        new() { Id = 310, MenuCategoryId = 315, MenuParentCategoryId = 4, MenuDisplayName = "Mindre tåtelsmygare", MenuUrlSegment = "Mindre-tatelsmygare", MenuDateUpdated = DateTime.Parse("2021-03-01T08:15:57") },
        new() { Id = 311, MenuCategoryId = 316, MenuParentCategoryId = 3, MenuDisplayName = "Mindre vattensalamander", MenuUrlSegment = "Mindre-vattensalamander", MenuDateUpdated = DateTime.Parse("2021-03-25T20:56:30") },
        new() { Id = 312, MenuCategoryId = 317, MenuParentCategoryId = 4, MenuDisplayName = "Prydlig pärlemorfjäril", MenuUrlSegment = "Prydlig-parlemorfjaril", MenuDateUpdated = DateTime.Parse("2021-03-14T10:26:05") },
        new() { Id = 313, MenuCategoryId = 318, MenuParentCategoryId = 4, MenuDisplayName = "Slåttergräsfjäril", MenuUrlSegment = "Slattergrasfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T08:16:25") },
        new() { Id = 314, MenuCategoryId = 319, MenuParentCategoryId = 4, MenuDisplayName = "Starrgräsfjäril", MenuUrlSegment = "Starrgrasfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T07:23:25") },
        new() { Id = 315, MenuCategoryId = 320, MenuParentCategoryId = 4, MenuDisplayName = "Stor dagsvärmare", MenuUrlSegment = "Stor-dagsvarmare", MenuDateUpdated = DateTime.Parse("2021-03-24T16:38:33") },
        new() { Id = 316, MenuCategoryId = 321, MenuParentCategoryId = 4, MenuDisplayName = "Större snabelsvärmare", MenuUrlSegment = "Storre-snabelsvarmare", MenuDateUpdated = DateTime.Parse("2021-03-01T08:33:59") },
        new() { Id = 317, MenuCategoryId = 322, MenuParentCategoryId = 4, MenuDisplayName = "Svartfläckig glanssmygare", MenuUrlSegment = "Svartflackig-glanssmygare", MenuDateUpdated = DateTime.Parse("2021-03-01T09:20:49") },
        new() { Id = 318, MenuCategoryId = 323, MenuParentCategoryId = 4, MenuDisplayName = "Svingelgräsfjäril", MenuUrlSegment = "Svingelgrasfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T08:08:01") },
        new() { Id = 319, MenuCategoryId = 324, MenuParentCategoryId = 4, MenuDisplayName = "Tallgräsfjäril", MenuUrlSegment = "Tallgrasfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T07:10:52") },
        new() { Id = 320, MenuCategoryId = 325, MenuParentCategoryId = 4, MenuDisplayName = "Tåtelsmygare", MenuUrlSegment = "Tatelsmygare", MenuDateUpdated = DateTime.Parse("2021-03-01T06:27:10") },
        new() { Id = 321, MenuCategoryId = 326, MenuParentCategoryId = 4, MenuDisplayName = "Vitfläckig guldvinge", MenuUrlSegment = "Vitflackig-guldvinge", MenuDateUpdated = DateTime.Parse("2021-03-01T08:24:51") },
        new() { Id = 322, MenuCategoryId = 327, MenuParentCategoryId = 4, MenuDisplayName = "Vitgräsfjäril", MenuUrlSegment = "Vitgrasfjaril", MenuDateUpdated = DateTime.Parse("2021-03-20T18:34:01") },
        new() { Id = 323, MenuCategoryId = 328, MenuParentCategoryId = 28, MenuDisplayName = "Fiskmås", MenuUrlSegment = "Fiskmas", MenuDateUpdated = DateTime.Parse("2021-03-01T08:36:08") },
        new() { Id = 324, MenuCategoryId = 329, MenuParentCategoryId = 28, MenuDisplayName = "Silvertärna", MenuUrlSegment = "Silvertarna", MenuDateUpdated = DateTime.Parse("2021-03-01T09:25:07") },
        new() { Id = 325, MenuCategoryId = 330, MenuParentCategoryId = 28, MenuDisplayName = "Svarthuvad mås", MenuUrlSegment = "Svarthuvad-mas", MenuDateUpdated = DateTime.Parse("2021-03-19T08:31:13") },
        new() { Id = 326, MenuCategoryId = 331, MenuParentCategoryId = 28, MenuDisplayName = "Tretåig mås", MenuUrlSegment = "Tretaig-mas", MenuDateUpdated = DateTime.Parse("2021-03-19T13:56:42") },
        new() { Id = 327, MenuCategoryId = 332, MenuParentCategoryId = 25, MenuDisplayName = "Gravand", MenuUrlSegment = "Gravand", MenuDateUpdated = DateTime.Parse("2021-03-19T07:52:01") },
        new() { Id = 328, MenuCategoryId = 333, MenuParentCategoryId = 25, MenuDisplayName = "Vigg", MenuUrlSegment = "Vigg", MenuDateUpdated = DateTime.Parse("2021-03-01T09:36:32") },
        new() { Id = 329, MenuCategoryId = 334, MenuParentCategoryId = 9, MenuDisplayName = "2010 Turkiet, Istanbul", MenuUrlSegment = "2010-Turkiet,-Istanbul", MenuDateUpdated = DateTime.Parse("2021-03-09T11:26:03") },
        new() { Id = 330, MenuCategoryId = 335, MenuParentCategoryId = 7, MenuDisplayName = "Öland", MenuUrlSegment = "Oland", MenuDateUpdated = DateTime.Parse("2021-03-17T15:39:37") },
        new() { Id = 331, MenuCategoryId = 336, MenuParentCategoryId = 7, MenuDisplayName = "Fjällen", MenuUrlSegment = "Fjallen", MenuDateUpdated = DateTime.Parse("2021-03-17T15:37:09") },
        new() { Id = 332, MenuCategoryId = 337, MenuParentCategoryId = 6, MenuDisplayName = "Bär och svampar", MenuUrlSegment = "Bar-och-svampar", MenuDateUpdated = DateTime.Parse("2021-03-24T00:22:51") },
        new() { Id = 333, MenuCategoryId = 338, MenuParentCategoryId = 6, MenuDisplayName = "Träd och buskar", MenuUrlSegment = "Trad-och-buskar", MenuDateUpdated = DateTime.Parse("2021-03-01T05:32:18") },
        new() { Id = 334, MenuCategoryId = 339, MenuParentCategoryId = 338, MenuDisplayName = "Al", MenuUrlSegment = "Al", MenuDateUpdated = DateTime.Parse("2021-03-01T05:42:32") },
        new() { Id = 335, MenuCategoryId = 340, MenuParentCategoryId = 338, MenuDisplayName = "Alm", MenuUrlSegment = "Alm", MenuDateUpdated = DateTime.Parse("2021-03-01T05:58:00") },
        new() { Id = 336, MenuCategoryId = 341, MenuParentCategoryId = 338, MenuDisplayName = "Ask", MenuUrlSegment = "Ask", MenuDateUpdated = DateTime.Parse("2021-03-01T05:58:19") },
        new() { Id = 337, MenuCategoryId = 342, MenuParentCategoryId = 338, MenuDisplayName = "Asp", MenuUrlSegment = "Asp", MenuDateUpdated = DateTime.Parse("2021-03-09T08:52:26") },
        new() { Id = 338, MenuCategoryId = 343, MenuParentCategoryId = 338, MenuDisplayName = "Björk", MenuUrlSegment = "Bjork", MenuDateUpdated = DateTime.Parse("2021-03-01T06:19:44") },
        new() { Id = 339, MenuCategoryId = 344, MenuParentCategoryId = 338, MenuDisplayName = "Bok", MenuUrlSegment = "Bok", MenuDateUpdated = DateTime.Parse("2021-03-01T05:58:58") },
        new() { Id = 340, MenuCategoryId = 345, MenuParentCategoryId = 338, MenuDisplayName = "Ek", MenuUrlSegment = "Ek", MenuDateUpdated = DateTime.Parse("2021-03-09T08:53:01") },
        new() { Id = 341, MenuCategoryId = 346, MenuParentCategoryId = 338, MenuDisplayName = "Gran", MenuUrlSegment = "Gran", MenuDateUpdated = DateTime.Parse("2021-03-20T18:34:35") },
        new() { Id = 342, MenuCategoryId = 347, MenuParentCategoryId = 338, MenuDisplayName = "Hassel", MenuUrlSegment = "Hassel", MenuDateUpdated = DateTime.Parse("2021-03-01T06:20:18") },
        new() { Id = 343, MenuCategoryId = 348, MenuParentCategoryId = 338, MenuDisplayName = "Hägg", MenuUrlSegment = "Hagg", MenuDateUpdated = DateTime.Parse("2021-03-01T06:12:24") },
        new() { Id = 344, MenuCategoryId = 349, MenuParentCategoryId = 338, MenuDisplayName = "Kastanj", MenuUrlSegment = "Kastanj", MenuDateUpdated = DateTime.Parse("2021-03-01T06:28:12") },
        new() { Id = 345, MenuCategoryId = 350, MenuParentCategoryId = 338, MenuDisplayName = "Lönn", MenuUrlSegment = "Lonn", MenuDateUpdated = DateTime.Parse("2021-03-01T06:12:42") },
        new() { Id = 346, MenuCategoryId = 351, MenuParentCategoryId = 338, MenuDisplayName = "Nypon", MenuUrlSegment = "Nypon", MenuDateUpdated = DateTime.Parse("2021-03-01T06:12:54") },
        new() { Id = 347, MenuCategoryId = 352, MenuParentCategoryId = 338, MenuDisplayName = "Rönn", MenuUrlSegment = "Ronn", MenuDateUpdated = DateTime.Parse("2021-03-01T06:13:14") },
        new() { Id = 348, MenuCategoryId = 353, MenuParentCategoryId = 338, MenuDisplayName = "Slån", MenuUrlSegment = "Slan", MenuDateUpdated = DateTime.Parse("2021-03-01T06:13:41") },
        new() { Id = 349, MenuCategoryId = 354, MenuParentCategoryId = 338, MenuDisplayName = "Syren", MenuUrlSegment = "Syren", MenuDateUpdated = DateTime.Parse("2021-03-19T08:31:15") },
        new() { Id = 350, MenuCategoryId = 355, MenuParentCategoryId = 338, MenuDisplayName = "Sälg", MenuUrlSegment = "Salg", MenuDateUpdated = DateTime.Parse("2021-03-01T06:13:56") },
        new() { Id = 351, MenuCategoryId = 356, MenuParentCategoryId = 338, MenuDisplayName = "Tall", MenuUrlSegment = "Tall", MenuDateUpdated = DateTime.Parse("2021-03-01T06:07:19") },
        new() { Id = 352, MenuCategoryId = 357, MenuParentCategoryId = 6, MenuDisplayName = "Trädgårdsväxter", MenuUrlSegment = "Tradgardsvaxter", MenuDateUpdated = DateTime.Parse("2021-03-24T06:47:03") },
        new() { Id = 353, MenuCategoryId = 358, MenuParentCategoryId = 2, MenuDisplayName = "Brunråtta", MenuUrlSegment = "Brunratta", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:07") },
        new() { Id = 354, MenuCategoryId = 359, MenuParentCategoryId = 2, MenuDisplayName = "Mård", MenuUrlSegment = "Mard", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:09") },
        new() { Id = 355, MenuCategoryId = 360, MenuParentCategoryId = 22, MenuDisplayName = "Morkulla", MenuUrlSegment = "Morkulla", MenuDateUpdated = DateTime.Parse("2021-03-19T07:54:30") },
        new() { Id = 356, MenuCategoryId = 361, MenuParentCategoryId = 214, MenuDisplayName = "Härfågel", MenuUrlSegment = "Harfagel", MenuDateUpdated = DateTime.Parse("2021-03-01T07:55:01") },
        new() { Id = 357, MenuCategoryId = 366, MenuParentCategoryId = 114, MenuDisplayName = "Korp", MenuUrlSegment = "Korp", MenuDateUpdated = DateTime.Parse("2021-03-19T09:19:19") },
        new() { Id = 358, MenuCategoryId = 367, MenuParentCategoryId = 25, MenuDisplayName = "Brunand", MenuUrlSegment = "Brunand", MenuDateUpdated = DateTime.Parse("2021-03-19T08:31:17") },
        new() { Id = 359, MenuCategoryId = 368, MenuParentCategoryId = 10, MenuDisplayName = "Svartvingad glada", MenuUrlSegment = "Svartvingad-glada", MenuDateUpdated = DateTime.Parse("2021-03-23T11:15:44") },
        new() { Id = 360, MenuCategoryId = 369, MenuParentCategoryId = 25, MenuDisplayName = "Stjärtand", MenuUrlSegment = "Stjartand", MenuDateUpdated = DateTime.Parse("2021-03-01T05:36:30") },
        new() { Id = 361, MenuCategoryId = 370, MenuParentCategoryId = 210, MenuDisplayName = "Ortolansparv", MenuUrlSegment = "Ortolansparv", MenuDateUpdated = DateTime.Parse("2021-03-01T08:12:57") },
        new() { Id = 362, MenuCategoryId = 371, MenuParentCategoryId = 204, MenuDisplayName = "Rosenfink", MenuUrlSegment = "Rosenfink", MenuDateUpdated = DateTime.Parse("2021-03-24T20:59:47") },
        new() { Id = 363, MenuCategoryId = 372, MenuParentCategoryId = 10, MenuDisplayName = "Ängshök", MenuUrlSegment = "Angshok", MenuDateUpdated = DateTime.Parse("2021-03-19T09:31:54") },
        new() { Id = 364, MenuCategoryId = 373, MenuParentCategoryId = 212, MenuDisplayName = "Gransångare", MenuUrlSegment = "Gransangare", MenuDateUpdated = DateTime.Parse("2021-03-01T08:20:39") },
        new() { Id = 365, MenuCategoryId = 374, MenuParentCategoryId = 212, MenuDisplayName = "Busksångare", MenuUrlSegment = "Busksangare", MenuDateUpdated = DateTime.Parse("2021-03-01T08:19:45") },
        new() { Id = 366, MenuCategoryId = 375, MenuParentCategoryId = 338, MenuDisplayName = "Hagtorn", MenuUrlSegment = "Hagtorn", MenuDateUpdated = DateTime.Parse("2021-03-01T06:27:48") },
        new() { Id = 367, MenuCategoryId = 376, MenuParentCategoryId = 25, MenuDisplayName = "Rostand", MenuUrlSegment = "Rostand", MenuDateUpdated = DateTime.Parse("2021-03-01T05:21:36") },
        new() { Id = 368, MenuCategoryId = 377, MenuParentCategoryId = 204, MenuDisplayName = "Bergfink", MenuUrlSegment = "Bergfink", MenuDateUpdated = DateTime.Parse("2021-03-01T09:19:16") },
        new() { Id = 369, MenuCategoryId = 378, MenuParentCategoryId = 21, MenuDisplayName = "Fjällripa", MenuUrlSegment = "Fjallripa", MenuDateUpdated = DateTime.Parse("2021-03-01T08:17:26") },
        new() { Id = 370, MenuCategoryId = 379, MenuParentCategoryId = 21, MenuDisplayName = "Dalripa", MenuUrlSegment = "Dalripa", MenuDateUpdated = DateTime.Parse("2021-03-01T07:25:16") },
        new() { Id = 371, MenuCategoryId = 380, MenuParentCategoryId = 25, MenuDisplayName = "Sjöorre", MenuUrlSegment = "Sjoorre", MenuDateUpdated = DateTime.Parse("2021-03-01T05:26:23") },
        new() { Id = 372, MenuCategoryId = 381, MenuParentCategoryId = 191, MenuDisplayName = "Storlom", MenuUrlSegment = "Storlom", MenuDateUpdated = DateTime.Parse("2021-03-19T07:50:24") },
        new() { Id = 373, MenuCategoryId = 382, MenuParentCategoryId = 4, MenuDisplayName = "Karminspinnare", MenuUrlSegment = "Karminspinnare", MenuDateUpdated = DateTime.Parse("2021-03-01T06:43:45") },
        new() { Id = 374, MenuCategoryId = 383, MenuParentCategoryId = 4, MenuDisplayName = "Mindre guldvinge", MenuUrlSegment = "Mindre-guldvinge", MenuDateUpdated = DateTime.Parse("2021-03-01T07:08:29") },
        new() { Id = 375, MenuCategoryId = 384, MenuParentCategoryId = 2, MenuDisplayName = "Mullvad", MenuUrlSegment = "Mullvad", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:08") },
        new() { Id = 376, MenuCategoryId = 385, MenuParentCategoryId = 288, MenuDisplayName = "Gulbröstad snäppa", MenuUrlSegment = "Gulbrostad-snappa", MenuDateUpdated = DateTime.Parse("2021-03-01T08:38:26") },
        new() { Id = 377, MenuCategoryId = 386, MenuParentCategoryId = 10, MenuDisplayName = "Stäpphök", MenuUrlSegment = "Stapphok", MenuDateUpdated = DateTime.Parse("2021-03-19T09:35:52") },
        new() { Id = 378, MenuCategoryId = 387, MenuParentCategoryId = 10, MenuDisplayName = "Blå kärrhök", MenuUrlSegment = "Bla-karrhok", MenuDateUpdated = DateTime.Parse("2021-03-20T18:33:49") },
        new() { Id = 379, MenuCategoryId = 388, MenuParentCategoryId = 46, MenuDisplayName = "Spetsbergsgås", MenuUrlSegment = "Spetsbergsgas", MenuDateUpdated = DateTime.Parse("2021-03-19T09:22:33") },
        new() { Id = 380, MenuCategoryId = 389, MenuParentCategoryId = 7, MenuDisplayName = "Skog", MenuUrlSegment = "Skog", MenuDateUpdated = DateTime.Parse("2021-03-17T15:36:06") },
        new() { Id = 381, MenuCategoryId = 390, MenuParentCategoryId = 7, MenuDisplayName = "Vatten", MenuUrlSegment = "Vatten", MenuDateUpdated = DateTime.Parse("2021-03-11T00:25:18") },
        new() { Id = 382, MenuCategoryId = 391, MenuParentCategoryId = 47, MenuDisplayName = "Koltrast", MenuUrlSegment = "Koltrast", MenuDateUpdated = DateTime.Parse("2021-03-02T12:11:03") },
        new() { Id = 383, MenuCategoryId = 392, MenuParentCategoryId = 26, MenuDisplayName = "Smådopping", MenuUrlSegment = "Smadopping", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:15") },
        new() { Id = 384, MenuCategoryId = 393, MenuParentCategoryId = 46, MenuDisplayName = "Fjällgås", MenuUrlSegment = "Fjallgas", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:24") },
        new() { Id = 385, MenuCategoryId = 394, MenuParentCategoryId = 208, MenuDisplayName = "Talltita", MenuUrlSegment = "Talltita", MenuDateUpdated = DateTime.Parse("2021-03-01T06:41:52") },
        new() { Id = 386, MenuCategoryId = 395, MenuParentCategoryId = 47, MenuDisplayName = "Svarthalsad trast", MenuUrlSegment = "Svarthalsad-trast", MenuDateUpdated = DateTime.Parse("2021-03-01T08:37:56") },
        new() { Id = 387, MenuCategoryId = 396, MenuParentCategoryId = 2, MenuDisplayName = "Vildsvin", MenuUrlSegment = "Vildsvin", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:11") },
        new() { Id = 388, MenuCategoryId = 397, MenuParentCategoryId = 9, MenuDisplayName = "2013 Florida", MenuUrlSegment = "2013-Florida", MenuDateUpdated = DateTime.Parse("2021-03-14T10:25:57") },
        new() { Id = 389, MenuCategoryId = 398, MenuParentCategoryId = 213, MenuDisplayName = "Citronärla", MenuUrlSegment = "Citronarla", MenuDateUpdated = DateTime.Parse("2021-03-19T09:58:30") },
        new() { Id = 390, MenuCategoryId = 399, MenuParentCategoryId = 28, MenuDisplayName = "Vitvingad tärna", MenuUrlSegment = "Vitvingad-tarna", MenuDateUpdated = DateTime.Parse("2021-03-01T09:34:49") },
        new() { Id = 391, MenuCategoryId = 400, MenuParentCategoryId = 212, MenuDisplayName = "Flodsångare", MenuUrlSegment = "Flodsangare", MenuDateUpdated = DateTime.Parse("2021-03-01T08:20:11") },
        new() { Id = 392, MenuCategoryId = 402, MenuParentCategoryId = 214, MenuDisplayName = "Gök", MenuUrlSegment = "Gok", MenuDateUpdated = DateTime.Parse("2021-03-01T06:18:04") },
        new() { Id = 393, MenuCategoryId = 403, MenuParentCategoryId = 28, MenuDisplayName = "Svarttärna", MenuUrlSegment = "Svarttarna", MenuDateUpdated = DateTime.Parse("2021-03-01T09:21:56") },
        new() { Id = 394, MenuCategoryId = 404, MenuParentCategoryId = 212, MenuDisplayName = "Kärrsångare", MenuUrlSegment = "Karrsangare", MenuDateUpdated = DateTime.Parse("2021-03-01T08:29:31") },
        new() { Id = 395, MenuCategoryId = 405, MenuParentCategoryId = 4, MenuDisplayName = "Grönsnabbvinge", MenuUrlSegment = "Gronsnabbvinge", MenuDateUpdated = DateTime.Parse("2021-03-02T06:01:58") },
        new() { Id = 396, MenuCategoryId = 406, MenuParentCategoryId = 36, MenuDisplayName = "Blå Jungfruslända", MenuUrlSegment = "Bla-Jungfruslanda", MenuDateUpdated = DateTime.Parse("2021-03-01T08:00:43") },
        new() { Id = 397, MenuCategoryId = 407, MenuParentCategoryId = 4, MenuDisplayName = "Mnemosynefjäril", MenuUrlSegment = "Mnemosynefjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T07:08:39") },
        new() { Id = 398, MenuCategoryId = 408, MenuParentCategoryId = 4, MenuDisplayName = "Brun gräsfjäril", MenuUrlSegment = "Brun-grasfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T07:20:49") },
        new() { Id = 399, MenuCategoryId = 409, MenuParentCategoryId = 191, MenuDisplayName = "Vitnäbbad islom", MenuUrlSegment = "Vitnabbad-islom", MenuDateUpdated = DateTime.Parse("2021-03-19T09:33:49") },
        new() { Id = 400, MenuCategoryId = 410, MenuParentCategoryId = 10, MenuDisplayName = "Dvärgörn", MenuUrlSegment = "Dvargorn", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:13") },
        new() { Id = 401, MenuCategoryId = 411, MenuParentCategoryId = 10, MenuDisplayName = "Fiskgjuse", MenuUrlSegment = "Fiskgjuse", MenuDateUpdated = DateTime.Parse("2021-03-19T09:30:58") },
        new() { Id = 402, MenuCategoryId = 412, MenuParentCategoryId = 4, MenuDisplayName = "Kartfjäril", MenuUrlSegment = "Kartfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T06:10:43") },
        new() { Id = 403, MenuCategoryId = 413, MenuParentCategoryId = 4, MenuDisplayName = "Vinkelbandat ordensfly", MenuUrlSegment = "Vinkelbandat-ordensfly", MenuDateUpdated = DateTime.Parse("2021-03-01T08:31:53") },
        new() { Id = 404, MenuCategoryId = 414, MenuParentCategoryId = 214, MenuDisplayName = "Brandkronad kungsfågel", MenuUrlSegment = "Brandkronad-kungsfagel", MenuDateUpdated = DateTime.Parse("2021-03-01T09:32:58") },
        new() { Id = 405, MenuCategoryId = 415, MenuParentCategoryId = 10, MenuDisplayName = "Gåsgam", MenuUrlSegment = "Gasgam", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:14") },
        new() { Id = 406, MenuCategoryId = 416, MenuParentCategoryId = 214, MenuDisplayName = "Gärdsmyg", MenuUrlSegment = "Gardsmyg", MenuDateUpdated = DateTime.Parse("2021-03-01T07:17:52") },
        new() { Id = 407, MenuCategoryId = 417, MenuParentCategoryId = 7, MenuDisplayName = "Norge", MenuUrlSegment = "Norge", MenuDateUpdated = DateTime.Parse("2021-03-12T21:17:06") },
        new() { Id = 408, MenuCategoryId = 418, MenuParentCategoryId = 209, MenuDisplayName = "Ökenstenskvätta", MenuUrlSegment = "Okenstenskvatta", MenuDateUpdated = DateTime.Parse("2021-03-17T11:50:11") },
        new() { Id = 409, MenuCategoryId = 419, MenuParentCategoryId = 206, MenuDisplayName = "Bändelkorsnäbb", MenuUrlSegment = "Bandelkorsnabb", MenuDateUpdated = DateTime.Parse("2021-03-25T20:28:43") },
        new() { Id = 410, MenuCategoryId = 420, MenuParentCategoryId = 273, MenuDisplayName = "Kaspisk pipare", MenuUrlSegment = "Kaspisk-pipare", MenuDateUpdated = DateTime.Parse("2021-03-19T09:54:14") },
        new() { Id = 411, MenuCategoryId = 421, MenuParentCategoryId = 46, MenuDisplayName = "Rödhalsad gås", MenuUrlSegment = "Rodhalsad-gas", MenuDateUpdated = DateTime.Parse("2021-03-22T16:19:34") },
        new() { Id = 412, MenuCategoryId = 422, MenuParentCategoryId = 24, MenuDisplayName = "Silkeshäger", MenuUrlSegment = "Silkeshager", MenuDateUpdated = DateTime.Parse("2021-03-01T09:28:25") },
        new() { Id = 413, MenuCategoryId = 423, MenuParentCategoryId = 212, MenuDisplayName = "Gräshoppsångare", MenuUrlSegment = "Grashoppsangare", MenuDateUpdated = DateTime.Parse("2021-03-01T09:20:37") },
        new() { Id = 414, MenuCategoryId = 424, MenuParentCategoryId = 28, MenuDisplayName = "Havstrut", MenuUrlSegment = "Havstrut", MenuDateUpdated = DateTime.Parse("2021-03-01T08:37:16") },
        new() { Id = 415, MenuCategoryId = 425, MenuParentCategoryId = 207, MenuDisplayName = "Rödstrupig piplärka", MenuUrlSegment = "Rodstrupig-piplarka", MenuDateUpdated = DateTime.Parse("2021-03-01T09:29:31") },
        new() { Id = 416, MenuCategoryId = 426, MenuParentCategoryId = 207, MenuDisplayName = "Skärpiplärka", MenuUrlSegment = "Skarpiplarka", MenuDateUpdated = DateTime.Parse("2021-03-01T08:26:33") },
        new() { Id = 417, MenuCategoryId = 427, MenuParentCategoryId = 48, MenuDisplayName = "Spetsbergsgrissla", MenuUrlSegment = "Spetsbergsgrissla", MenuDateUpdated = DateTime.Parse("2021-03-19T13:56:17") },
        new() { Id = 418, MenuCategoryId = 428, MenuParentCategoryId = 24, MenuDisplayName = "Rallhäger", MenuUrlSegment = "Rallhager", MenuDateUpdated = DateTime.Parse("2021-03-01T09:21:30") },
        new() { Id = 419, MenuCategoryId = 429, MenuParentCategoryId = 9, MenuDisplayName = "2015 Gran Canaria", MenuUrlSegment = "2015-Gran-Canaria", MenuDateUpdated = DateTime.Parse("2021-03-23T05:03:26") },
        new() { Id = 420, MenuCategoryId = 430, MenuParentCategoryId = 213, MenuDisplayName = "Forsärla", MenuUrlSegment = "Forsarla", MenuDateUpdated = DateTime.Parse("2021-03-20T19:45:42") },
        new() { Id = 421, MenuCategoryId = 431, MenuParentCategoryId = 207, MenuDisplayName = "Trädlärka", MenuUrlSegment = "Tradlarka", MenuDateUpdated = DateTime.Parse("2021-03-19T10:02:26") },
        new() { Id = 422, MenuCategoryId = 432, MenuParentCategoryId = 24, MenuDisplayName = "Småtrapp", MenuUrlSegment = "Smatrapp", MenuDateUpdated = DateTime.Parse("2021-03-01T09:17:40") },
        new() { Id = 423, MenuCategoryId = 433, MenuParentCategoryId = 211, MenuDisplayName = "Rostgumpsvala", MenuUrlSegment = "Rostgumpsvala", MenuDateUpdated = DateTime.Parse("2021-03-01T08:14:03") },
        new() { Id = 424, MenuCategoryId = 434, MenuParentCategoryId = 214, MenuDisplayName = "Rödstjärt", MenuUrlSegment = "Rodstjart", MenuDateUpdated = DateTime.Parse("2021-03-19T10:03:54") },
        new() { Id = 425, MenuCategoryId = 435, MenuParentCategoryId = 2, MenuDisplayName = "Ren", MenuUrlSegment = "Ren", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:09") },
        new() { Id = 426, MenuCategoryId = 436, MenuParentCategoryId = 4, MenuDisplayName = "Silversmygare", MenuUrlSegment = "Silversmygare", MenuDateUpdated = DateTime.Parse("2021-03-01T06:25:22") },
        new() { Id = 427, MenuCategoryId = 437, MenuParentCategoryId = 4, MenuDisplayName = "Tallprocessionsspinnare", MenuUrlSegment = "Tallprocessionsspinnare", MenuDateUpdated = DateTime.Parse("2021-03-01T08:34:52") },
        new() { Id = 428, MenuCategoryId = 438, MenuParentCategoryId = 4, MenuDisplayName = "Amiral", MenuUrlSegment = "Amiral", MenuDateUpdated = DateTime.Parse("2021-03-24T16:38:33") },
        new() { Id = 429, MenuCategoryId = 439, MenuParentCategoryId = 2, MenuDisplayName = "Kanin", MenuUrlSegment = "Kanin", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:08") },
        new() { Id = 430, MenuCategoryId = 440, MenuParentCategoryId = 2, MenuDisplayName = "Delfin", MenuUrlSegment = "Delfin", MenuDateUpdated = DateTime.Parse("2021-03-24T00:22:10") },
        new() { Id = 431, MenuCategoryId = 441, MenuParentCategoryId = 9, MenuDisplayName = "2015 Oman", MenuUrlSegment = "2015-Oman", MenuDateUpdated = DateTime.Parse("2021-03-09T11:26:11") },
        new() { Id = 432, MenuCategoryId = 442, MenuParentCategoryId = 27, MenuDisplayName = "Gråspett", MenuUrlSegment = "Graspett", MenuDateUpdated = DateTime.Parse("2021-03-14T08:41:57") },
        new() { Id = 433, MenuCategoryId = 443, MenuParentCategoryId = 9, MenuDisplayName = "2016 Spanien-Frankrike", MenuUrlSegment = "2016-Spanien-Frankrike", MenuDateUpdated = DateTime.Parse("2021-03-09T11:26:11") },
        new() { Id = 434, MenuCategoryId = 444, MenuParentCategoryId = 4, MenuDisplayName = "Ekspinnare", MenuUrlSegment = "Ekspinnare", MenuDateUpdated = DateTime.Parse("2021-03-09T08:38:02") },
        new() { Id = 435, MenuCategoryId = 445, MenuParentCategoryId = 4, MenuDisplayName = "Myrpärlemorfjäril", MenuUrlSegment = "Myrparlemorfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T08:07:16") },
        new() { Id = 436, MenuCategoryId = 446, MenuParentCategoryId = 4, MenuDisplayName = "Sälgskimmerfjäril", MenuUrlSegment = "Salgskimmerfjaril", MenuDateUpdated = DateTime.Parse("2021-03-17T15:45:54") },
        new() { Id = 437, MenuCategoryId = 447, MenuParentCategoryId = 4, MenuDisplayName = "Mindre bastardsvärmare", MenuUrlSegment = "Mindre-bastardsvarmare", MenuDateUpdated = DateTime.Parse("2021-03-01T08:33:48") },
        new() { Id = 438, MenuCategoryId = 448, MenuParentCategoryId = 4, MenuDisplayName = "Kålfjäril", MenuUrlSegment = "Kalfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T06:11:00") },
        new() { Id = 439, MenuCategoryId = 449, MenuParentCategoryId = 4, MenuDisplayName = "Storfläckig pärlemorfjäril", MenuUrlSegment = "Storflackig-parlemorfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T09:30:19") },
        new() { Id = 440, MenuCategoryId = 451, MenuParentCategoryId = 4, MenuDisplayName = "Puktörneblåvinge", MenuUrlSegment = "Puktorneblavinge", MenuDateUpdated = DateTime.Parse("2021-03-01T07:59:47") },
        new() { Id = 441, MenuCategoryId = 452, MenuParentCategoryId = 212, MenuDisplayName = "Tajgasångare", MenuUrlSegment = "Tajgasangare", MenuDateUpdated = DateTime.Parse("2021-03-01T08:29:38") },
        new() { Id = 442, MenuCategoryId = 453, MenuParentCategoryId = 210, MenuDisplayName = "Dvärgsparv", MenuUrlSegment = "Dvargsparv", MenuDateUpdated = DateTime.Parse("2021-03-01T08:02:41") },
        new() { Id = 443, MenuCategoryId = 454, MenuParentCategoryId = 210, MenuDisplayName = "Sibirisk järnsparv", MenuUrlSegment = "Sibirisk-jarnsparv", MenuDateUpdated = DateTime.Parse("2021-03-19T08:31:18") },
        new() { Id = 444, MenuCategoryId = 455, MenuParentCategoryId = 209, MenuDisplayName = "Isabellastenskvätta", MenuUrlSegment = "Isabellastenskvatta", MenuDateUpdated = DateTime.Parse("2021-03-01T09:31:57") },
        new() { Id = 445, MenuCategoryId = 456, MenuParentCategoryId = 212, MenuDisplayName = "Brunsångare", MenuUrlSegment = "Brunsangare", MenuDateUpdated = DateTime.Parse("2021-03-01T08:18:52") },
        new() { Id = 446, MenuCategoryId = 457, MenuParentCategoryId = 204, MenuDisplayName = "Snösiska", MenuUrlSegment = "Snosiska", MenuDateUpdated = DateTime.Parse("2021-03-01T09:23:33") },
        new() { Id = 447, MenuCategoryId = 458, MenuParentCategoryId = 28, MenuDisplayName = "Vitvingad trut", MenuUrlSegment = "Vitvingad-trut", MenuDateUpdated = DateTime.Parse("2021-03-01T09:31:41") },
        new() { Id = 448, MenuCategoryId = 459, MenuParentCategoryId = 10, MenuDisplayName = "Stenfalk", MenuUrlSegment = "Stenfalk", MenuDateUpdated = DateTime.Parse("2021-03-19T09:29:30") },
        new() { Id = 449, MenuCategoryId = 460, MenuParentCategoryId = 47, MenuDisplayName = "Dubbeltrast", MenuUrlSegment = "Dubbeltrast", MenuDateUpdated = DateTime.Parse("2021-03-01T07:51:02") },
        new() { Id = 450, MenuCategoryId = 461, MenuParentCategoryId = 207, MenuDisplayName = "Ängspiplärka", MenuUrlSegment = "Angspiplarka", MenuDateUpdated = DateTime.Parse("2021-03-01T08:27:33") },
        new() { Id = 451, MenuCategoryId = 462, MenuParentCategoryId = 4, MenuDisplayName = "Skogsvisslare", MenuUrlSegment = "Skogsvisslare", MenuDateUpdated = DateTime.Parse("2021-03-01T06:25:46") },
        new() { Id = 452, MenuCategoryId = 463, MenuParentCategoryId = 46, MenuDisplayName = "Nilgås", MenuUrlSegment = "Nilgas", MenuDateUpdated = DateTime.Parse("2021-03-19T07:48:24") },
        new() { Id = 453, MenuCategoryId = 464, MenuParentCategoryId = 7, MenuDisplayName = "Kust", MenuUrlSegment = "Kust", MenuDateUpdated = DateTime.Parse("2021-03-22T00:30:27") },
        new() { Id = 454, MenuCategoryId = 465, MenuParentCategoryId = 4, MenuDisplayName = "Ängssmygare", MenuUrlSegment = "Angssmygare", MenuDateUpdated = DateTime.Parse("2021-03-14T10:26:12") },
        new() { Id = 455, MenuCategoryId = 466, MenuParentCategoryId = 4, MenuDisplayName = "Fjällvickerblåvinge", MenuUrlSegment = "Fjallvickerblavinge", MenuDateUpdated = DateTime.Parse("2021-03-01T21:24:29") },
        new() { Id = 456, MenuCategoryId = 467, MenuParentCategoryId = 4, MenuDisplayName = "Violett blåvinge", MenuUrlSegment = "Violett-blavinge", MenuDateUpdated = DateTime.Parse("2021-03-01T07:23:53") },
        new() { Id = 457, MenuCategoryId = 468, MenuParentCategoryId = 4, MenuDisplayName = "Violett guldvinge", MenuUrlSegment = "Violett-guldvinge", MenuDateUpdated = DateTime.Parse("2021-03-01T07:24:28") },
        new() { Id = 458, MenuCategoryId = 469, MenuParentCategoryId = 4, MenuDisplayName = "Violettkantad guldvinge", MenuUrlSegment = "Violettkantad-guldvinge", MenuDateUpdated = DateTime.Parse("2021-03-01T08:35:15") },
        new() { Id = 459, MenuCategoryId = 470, MenuParentCategoryId = 28, MenuDisplayName = "Silltrut", MenuUrlSegment = "Silltrut", MenuDateUpdated = DateTime.Parse("2021-03-01T08:37:41") },
        new() { Id = 460, MenuCategoryId = 471, MenuParentCategoryId = 288, MenuDisplayName = "Gluttsnäppa", MenuUrlSegment = "Gluttsnappa", MenuDateUpdated = DateTime.Parse("2021-03-19T09:54:42") },
        new() { Id = 461, MenuCategoryId = 472, MenuParentCategoryId = 7, MenuDisplayName = "Gotland", MenuUrlSegment = "Gotland", MenuDateUpdated = DateTime.Parse("2021-03-22T00:30:34") },
        new() { Id = 462, MenuCategoryId = 473, MenuParentCategoryId = 4, MenuDisplayName = "Kattunvisslare", MenuUrlSegment = "Kattunvisslare", MenuDateUpdated = DateTime.Parse("2021-03-01T06:44:11") },
        new() { Id = 463, MenuCategoryId = 474, MenuParentCategoryId = 28, MenuDisplayName = "Skräntärna", MenuUrlSegment = "Skrantarna", MenuDateUpdated = DateTime.Parse("2021-03-01T09:25:32") },
        new() { Id = 464, MenuCategoryId = 475, MenuParentCategoryId = 28, MenuDisplayName = "Kentsk tärna", MenuUrlSegment = "Kentsk-tarna", MenuDateUpdated = DateTime.Parse("2021-03-01T09:28:37") },
        new() { Id = 465, MenuCategoryId = 476, MenuParentCategoryId = 4, MenuDisplayName = "Större träfjäril", MenuUrlSegment = "Storre-trafjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T08:07:24") },
        new() { Id = 466, MenuCategoryId = 477, MenuParentCategoryId = 209, MenuDisplayName = "Svarthakad buskskvätta", MenuUrlSegment = "Svarthakad-buskskvatta", MenuDateUpdated = DateTime.Parse("2021-03-01T09:35:42") },
        new() { Id = 467, MenuCategoryId = 478, MenuParentCategoryId = 27, MenuDisplayName = "Vitryggig hackspett", MenuUrlSegment = "Vitryggig-hackspett", MenuDateUpdated = DateTime.Parse("2021-03-01T08:11:23") },
        new() { Id = 468, MenuCategoryId = 479, MenuParentCategoryId = 47, MenuDisplayName = "Ringtrast", MenuUrlSegment = "Ringtrast", MenuDateUpdated = DateTime.Parse("2021-03-01T06:58:43") },
        new() { Id = 469, MenuCategoryId = 480, MenuParentCategoryId = 47, MenuDisplayName = "Turturduva", MenuUrlSegment = "Turturduva", MenuDateUpdated = DateTime.Parse("2021-03-01T12:42:24") },
        new() { Id = 470, MenuCategoryId = 481, MenuParentCategoryId = 4, MenuDisplayName = "Poppelsvärmare", MenuUrlSegment = "Poppelsvarmare", MenuDateUpdated = DateTime.Parse("2021-03-01T06:56:49") },
        new() { Id = 471, MenuCategoryId = 487, MenuParentCategoryId = 7, MenuDisplayName = "Övrigt", MenuUrlSegment = "Ovrigt", MenuDateUpdated = DateTime.Parse("2021-03-19T08:31:13") },
        new() { Id = 472, MenuCategoryId = 482, MenuParentCategoryId = 4, MenuDisplayName = "Videsvärmare", MenuUrlSegment = "Videsvarmare", MenuDateUpdated = DateTime.Parse("2021-03-01T06:27:24") },
        new() { Id = 473, MenuCategoryId = 483, MenuParentCategoryId = 4, MenuDisplayName = "Mindre snabelsvärmare", MenuUrlSegment = "Mindre-snabelsvarmare", MenuDateUpdated = DateTime.Parse("2021-03-01T08:31:11") },
        new() { Id = 474, MenuCategoryId = 484, MenuParentCategoryId = 4, MenuDisplayName = "Ängsnätfjäril", MenuUrlSegment = "Angsnatfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T07:11:41") },
        new() { Id = 475, MenuCategoryId = 485, MenuParentCategoryId = 212, MenuDisplayName = "Höksångare", MenuUrlSegment = "Hoksangare", MenuDateUpdated = DateTime.Parse("2021-03-01T08:20:58") },
        new() { Id = 476, MenuCategoryId = 486, MenuParentCategoryId = 214, MenuDisplayName = "Rosenstare", MenuUrlSegment = "Rosenstare", MenuDateUpdated = DateTime.Parse("2021-03-19T10:00:11") },
        new() { Id = 477, MenuCategoryId = 488, MenuParentCategoryId = 4, MenuDisplayName = "Fjällgräsfjäril", MenuUrlSegment = "Fjallgrasfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T07:58:50") },
        new() { Id = 478, MenuCategoryId = 489, MenuParentCategoryId = 4, MenuDisplayName = "Blåfläckig träfjäril", MenuUrlSegment = "Blaflackig-trafjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T08:39:28") },
        new() { Id = 479, MenuCategoryId = 490, MenuParentCategoryId = 4, MenuDisplayName = "Eksnabbvinge", MenuUrlSegment = "Eksnabbvinge", MenuDateUpdated = DateTime.Parse("2021-03-09T08:37:50") },
        new() { Id = 480, MenuCategoryId = 491, MenuParentCategoryId = 46, MenuDisplayName = "Stripgås", MenuUrlSegment = "Stripgas", MenuDateUpdated = DateTime.Parse("2021-03-19T07:53:04") },
        new() { Id = 481, MenuCategoryId = 492, MenuParentCategoryId = 24, MenuDisplayName = "Ägretthäger", MenuUrlSegment = "Agretthager", MenuDateUpdated = DateTime.Parse("2021-03-01T09:31:00") },
        new() { Id = 482, MenuCategoryId = 493, MenuParentCategoryId = 4, MenuDisplayName = "Åkervindesvärmare", MenuUrlSegment = "Akervindesvarmare", MenuDateUpdated = DateTime.Parse("2021-03-01T08:09:23") },
        new() { Id = 483, MenuCategoryId = 494, MenuParentCategoryId = 10, MenuDisplayName = "Brun glada", MenuUrlSegment = "Brun-glada", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:13") },
        new() { Id = 484, MenuCategoryId = 495, MenuParentCategoryId = 9, MenuDisplayName = "2018 Spanien", MenuUrlSegment = "2018-Spanien", MenuDateUpdated = DateTime.Parse("2021-03-20T18:34:08") },
        new() { Id = 485, MenuCategoryId = 496, MenuParentCategoryId = 9, MenuDisplayName = "2019 Donau, Rumänien", MenuUrlSegment = "2019-Donau,-Rumanien", MenuDateUpdated = DateTime.Parse("2021-03-17T15:25:54") },
        new() { Id = 486, MenuCategoryId = 497, MenuParentCategoryId = 4, MenuDisplayName = "Svävfluglik dagsvärmare", MenuUrlSegment = "Svavfluglik-dagsvarmare", MenuDateUpdated = DateTime.Parse("2021-03-01T09:16:21") },
        new() { Id = 487, MenuCategoryId = 498, MenuParentCategoryId = 4, MenuDisplayName = "Hagtornsfjäril", MenuUrlSegment = "Hagtornsfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T06:55:57") },
        new() { Id = 488, MenuCategoryId = 499, MenuParentCategoryId = 4, MenuDisplayName = "Ljusgul höfjäril", MenuUrlSegment = "Ljusgul-hofjaril", MenuDateUpdated = DateTime.Parse("2021-03-09T08:54:55") },
        new() { Id = 489, MenuCategoryId = 500, MenuParentCategoryId = 10, MenuDisplayName = "Bivråk", MenuUrlSegment = "Bivrak", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:13") },
        new() { Id = 490, MenuCategoryId = 501, MenuParentCategoryId = 22, MenuDisplayName = "Dvärgbeckasin", MenuUrlSegment = "Dvargbeckasin", MenuDateUpdated = DateTime.Parse("2021-03-01T06:03:34") },
        new() { Id = 491, MenuCategoryId = 502, MenuParentCategoryId = 9, MenuDisplayName = "2020 Thailand", MenuUrlSegment = "2020-Thailand", MenuDateUpdated = DateTime.Parse("2021-03-22T00:30:27") },
        new() { Id = 492, MenuCategoryId = 503, MenuParentCategoryId = 20, MenuDisplayName = "Svart svan", MenuUrlSegment = "Svart-svan", MenuDateUpdated = DateTime.Parse("2021-03-11T18:03:41") },
        new() { Id = 493, MenuCategoryId = 504, MenuParentCategoryId = 47, MenuDisplayName = "Taltrast", MenuUrlSegment = "Taltrast", MenuDateUpdated = DateTime.Parse("2021-03-19T09:44:38") },
        new() { Id = 494, MenuCategoryId = 505, MenuParentCategoryId = 4, MenuDisplayName = "Klöverblåvinge", MenuUrlSegment = "Kloverblavinge", MenuDateUpdated = DateTime.Parse("2021-03-01T07:06:35") },
        new() { Id = 495, MenuCategoryId = 506, MenuParentCategoryId = 4, MenuDisplayName = "Fetörtsblåvinge", MenuUrlSegment = "Fetortsblavinge", MenuDateUpdated = DateTime.Parse("2021-03-01T07:21:22") },
        new() { Id = 496, MenuCategoryId = 508, MenuParentCategoryId = 4, MenuDisplayName = "Brun blåvinge", MenuUrlSegment = "Brun-blavinge", MenuDateUpdated = DateTime.Parse("2021-03-01T06:43:06") },
        new() { Id = 497, MenuCategoryId = 509, MenuParentCategoryId = 4, MenuDisplayName = "Berggräsfjäril", MenuUrlSegment = "Berggrasfjaril", MenuDateUpdated = DateTime.Parse("2021-03-01T07:05:48") },
        new() { Id = 498, MenuCategoryId = 510, MenuParentCategoryId = 4, MenuDisplayName = "Busksnabbvinge", MenuUrlSegment = "Busksnabbvinge", MenuDateUpdated = DateTime.Parse("2021-03-01T06:43:13") },
        new() { Id = 499, MenuCategoryId = 511, MenuParentCategoryId = 4, MenuDisplayName = "Fjällbastardsvärmare", MenuUrlSegment = "Fjallbastardsvarmare", MenuDateUpdated = DateTime.Parse("2021-03-01T08:31:03") },
        new() { Id = 500, MenuCategoryId = 512, MenuParentCategoryId = 4, MenuDisplayName = "Brun björnspinnare", MenuUrlSegment = "Brun-bjornspinnare", MenuDateUpdated = DateTime.Parse("2021-03-01T08:06:40") },
        new() { Id = 501, MenuCategoryId = 513, MenuParentCategoryId = 4, MenuDisplayName = "Krattsnabbvinge", MenuUrlSegment = "Krattsnabbvinge", MenuDateUpdated = DateTime.Parse("2021-03-01T06:56:32") },
        new() { Id = 502, MenuCategoryId = 514, MenuParentCategoryId = 4, MenuDisplayName = "Tallsvärmare", MenuUrlSegment = "Tallsvarmare", MenuDateUpdated = DateTime.Parse("2021-03-01T06:26:36") },
        new() { Id = 503, MenuCategoryId = 515, MenuParentCategoryId = 21, MenuDisplayName = "Järpe", MenuUrlSegment = "Jarpe", MenuDateUpdated = DateTime.Parse("2021-03-19T16:15:36") },
        new() { Id = 504, MenuCategoryId = 516, MenuParentCategoryId = 2, MenuDisplayName = "Större skogsmus", MenuUrlSegment = "Storre-skogsmus", MenuDateUpdated = DateTime.Parse("2021-03-11T16:44:09") },
        new() { Id = 505, MenuCategoryId = 517, MenuParentCategoryId = 2, MenuDisplayName = "Skogssork", MenuUrlSegment = "Skogssork", MenuDateUpdated = DateTime.Parse("2021-03-19T09:38:52") },
        new() { Id = 506, MenuCategoryId = 518, MenuParentCategoryId = 2, MenuDisplayName = "Mink", MenuUrlSegment = "Mink", MenuDateUpdated = DateTime.Parse("2021-03-24T06:33:10") },
        new() { Id = 507, MenuCategoryId = 519, MenuParentCategoryId = 3, MenuDisplayName = "Vanlig groda", MenuUrlSegment = "Vanlig-groda", MenuDateUpdated = DateTime.Parse("2022-09-24T16:23:40") },
        new() { Id = 508, MenuCategoryId = 520, MenuParentCategoryId = 4, MenuDisplayName = "Körsbärsfux", MenuUrlSegment = "Korsbarsfux", MenuDateUpdated = DateTime.Parse("2022-09-24T16:23:43") },
        new() { Id = 509, MenuCategoryId = 521, MenuParentCategoryId = 4, MenuDisplayName = "Videfux", MenuUrlSegment = "Videfux", MenuDateUpdated = DateTime.Parse("2022-09-24T16:23:53") },
        new() { Id = 510, MenuCategoryId = 522, MenuParentCategoryId = 2, MenuDisplayName = "Knölval", MenuUrlSegment = "Knolval", MenuDateUpdated = DateTime.Parse("2022-09-24T16:23:56") },
        new() { Id = 511, MenuCategoryId = 523, MenuParentCategoryId = 25, MenuDisplayName = "Småskrake", MenuUrlSegment = "Smaskrake", MenuDateUpdated = DateTime.Parse("2022-09-24T16:23:58") },
        new() { Id = 512, MenuCategoryId = 524, MenuParentCategoryId = 4, MenuDisplayName = "Tosteblåvinge", MenuUrlSegment = "Tosteblavinge", MenuDateUpdated = DateTime.Parse("2022-09-24T16:24:00") },
        new() { Id = 513, MenuCategoryId = 525, MenuParentCategoryId = 4, MenuDisplayName = "Gräsulv", MenuUrlSegment = "Grasulv", MenuDateUpdated = DateTime.Parse("2022-09-24T16:24:01") },
        new() { Id = 514, MenuCategoryId = 526, MenuParentCategoryId = 25, MenuDisplayName = "Vitnackad svärta", MenuUrlSegment = "Vitnackad-svarta", MenuDateUpdated = DateTime.Parse("2022-09-24T16:24:02") },
        new() { Id = 515, MenuCategoryId = 527, MenuParentCategoryId = 4, MenuDisplayName = "Tryfjäril", MenuUrlSegment = "Tryfjaril", MenuDateUpdated = DateTime.Parse("2022-09-24T16:24:21") },
        new() { Id = 516, MenuCategoryId = 528, MenuParentCategoryId = 4, MenuDisplayName = "Rödfläckig blåvinge", MenuUrlSegment = "Rodflackig-blavinge", MenuDateUpdated = DateTime.Parse("2022-09-24T16:24:26") },
        new() { Id = 517, MenuCategoryId = 529, MenuParentCategoryId = 4, MenuDisplayName = "Alkonblåvinge", MenuUrlSegment = "Alkonblavinge", MenuDateUpdated = DateTime.Parse("2022-09-24T16:24:28") },
        new() { Id = 518, MenuCategoryId = 530, MenuParentCategoryId = 4, MenuDisplayName = "Hedpärlemofjäril", MenuUrlSegment = "Hedparlemofjaril", MenuDateUpdated = DateTime.Parse("2022-09-24T16:24:29") },
        new() { Id = 519, MenuCategoryId = 531, MenuParentCategoryId = 4, MenuDisplayName = "Oxhuvudspinnare", MenuUrlSegment = "Oxhuvudspinnare", MenuDateUpdated = DateTime.Parse("2022-09-24T16:24:40") },
        new() { Id = 520, MenuCategoryId = 532, MenuParentCategoryId = 4, MenuDisplayName = "Aspskimmerfjäril", MenuUrlSegment = "Aspskimmerfjaril", MenuDateUpdated = DateTime.Parse("2022-09-24T16:24:44") },
        new() { Id = 521, MenuCategoryId = 533, MenuParentCategoryId = 3, MenuDisplayName = "Hasselsnok", MenuUrlSegment = "Hasselsnok", MenuDateUpdated = DateTime.Parse("2022-09-24T16:24:49") },
        new() { Id = 522, MenuCategoryId = 534, MenuParentCategoryId = 4, MenuDisplayName = "Grönfläckig vitfjäril", MenuUrlSegment = "Gronflackig-vitfjaril", MenuDateUpdated = DateTime.Parse("2022-09-24T16:24:52") },
        new() { Id = 523, MenuCategoryId = 535, MenuParentCategoryId = 2, MenuDisplayName = "Varg", MenuUrlSegment = "Varg", MenuDateUpdated = DateTime.Parse("2022-09-24T16:24:53") },
        new() { Id = 524, MenuCategoryId = 536, MenuParentCategoryId = 214, MenuDisplayName = "Rubinnäktergal", MenuUrlSegment = "Rubinnaktergal", MenuDateUpdated = DateTime.Parse("2022-09-24T16:24:55") },
        new() { Id = 525, MenuCategoryId = 537, MenuParentCategoryId = 9, MenuDisplayName = "2022 Spanien", MenuUrlSegment = "2022-Spanien", MenuDateUpdated = DateTime.Parse("2022-09-24T16:24:59") },
        new() { Id = 526, MenuCategoryId = 538, MenuParentCategoryId = 36, MenuDisplayName = "Mindre kejsartrollslända", MenuUrlSegment = "Mindre-kejsartrollslanda", MenuDateUpdated = DateTime.Parse("2022-09-24T16:25:02") },
        new() { Id = 527, MenuCategoryId = 539, MenuParentCategoryId = 4, MenuDisplayName = "Turkos blåvinge", MenuUrlSegment = "Turkos-blavinge", MenuDateUpdated = DateTime.Parse("2022-09-24T16:25:05") },
        new() { Id = 528, MenuCategoryId = 540, MenuParentCategoryId = 36, MenuDisplayName = "Blodröd ängstrollslända", MenuUrlSegment = "Blodrod-angstrollslanda", MenuDateUpdated = DateTime.Parse("2022-09-24T16:25:09") },
        new() { Id = 529, MenuCategoryId = 541, MenuParentCategoryId = 4, MenuDisplayName = "Fjällpärlemorfjäril", MenuUrlSegment = "", MenuDateUpdated = null },
        new() { Id = 530, MenuCategoryId = 542, MenuParentCategoryId = 4, MenuDisplayName = "Ängsblåvinge", MenuUrlSegment = "", MenuDateUpdated = null },
        new() { Id = 531, MenuCategoryId = 543, MenuParentCategoryId = 26, MenuDisplayName = "Havssula", MenuUrlSegment = "", MenuDateUpdated = null },
        new() { Id = 532, MenuCategoryId = 544, MenuParentCategoryId = 214, MenuDisplayName = "Vitstjärnig blåhake", MenuUrlSegment = "", MenuDateUpdated = null },
        new() { Id = 533, MenuCategoryId = 545, MenuParentCategoryId = 26, MenuDisplayName = "Svarthalsad dopping", MenuUrlSegment = "", MenuDateUpdated = null },
        new() { Id = 534, MenuCategoryId = 546, MenuParentCategoryId = 4, MenuDisplayName = "Fjällhöfjäril", MenuUrlSegment = "", MenuDateUpdated = null },
        new() { Id = 535, MenuCategoryId = 547, MenuParentCategoryId = 28, MenuDisplayName = "Stormfågel", MenuUrlSegment = "", MenuDateUpdated = null },
        new() { Id = 536, MenuCategoryId = 548, MenuParentCategoryId = 2, MenuDisplayName = "Kronhjort", MenuUrlSegment = "", MenuDateUpdated = null },
        new() { Id = 537, MenuCategoryId = 549, MenuParentCategoryId = 210, MenuDisplayName = "Kornsparv", MenuUrlSegment = "", MenuDateUpdated = null },
        new() { Id = 538, MenuCategoryId = 550, MenuParentCategoryId = 338, MenuDisplayName = "Körsbärsträd", MenuUrlSegment = "", MenuDateUpdated = null },
        new() { Id = 539, MenuCategoryId = 551, MenuParentCategoryId = 4, MenuDisplayName = "Mindre påfågelspinnare", MenuUrlSegment = "", MenuDateUpdated = null },
        new() { Id = 540, MenuCategoryId = 552, MenuParentCategoryId = 4, MenuDisplayName = "Ängsmetallvinge", MenuUrlSegment = "", MenuDateUpdated = null },
        new() { Id = 541, MenuCategoryId = 553, MenuParentCategoryId = 10, MenuDisplayName = "Stäppörn", MenuUrlSegment = "", MenuDateUpdated = null },
        new() { Id = 542, MenuCategoryId = 554, MenuParentCategoryId = 338, MenuDisplayName = "Havtorn", MenuUrlSegment = "", MenuDateUpdated = null },
        new() { Id = 543, MenuCategoryId = 555, MenuParentCategoryId = 2, MenuDisplayName = "Större brunfladdermus", MenuUrlSegment = "", MenuDateUpdated = null },
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
    public static List<TblPageCounter> DbSeed_Tbl_PageCounters => new()
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
    public static List<TblImage> DbSeed_Tbl_Image => new()
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
        modelBuilder.Entity<TblGb>().HasData(DbSeed_Tbl_Guestbook);
        modelBuilder.Entity<TblMenu>().HasData(DbSeed_Tbl_MenuCategories);
        modelBuilder.Entity<TblPageCounter>().HasData(DbSeed_Tbl_PageCounters);
        modelBuilder.Entity<TblImage>().HasData(DbSeed_Tbl_Image);
    }

    #endregion

    #region Seed data definitions

    /// <summary>
    /// Seeds the in-memory database with test data
    /// </summary>
    /// <remarks>
    /// Används för att seeda in-memory databasen med test-data när applikationen körs i Codespaces eller utvecklingsmiljö
    /// </remarks>
    public static void SeedInMemoryDatabase(this ArvidsonFotoCoreDbContext context)
    {
        // Kontrollera om databasen redan har data
        if (context.TblGbs.Any() || context.TblMenus.Any() || context.TblImages.Any())
        {
            return; // Databasen är redan seedand
        }

        // Lägg till test-data
        context.TblGbs.AddRange(DbSeed_Tbl_Guestbook);
        context.TblMenus.AddRange(DbSeed_Tbl_MenuCategories);
        context.TblPageCounter.AddRange(DbSeed_Tbl_PageCounters);
        context.TblImages.AddRange(DbSeed_Tbl_Image);

        // Spara ändringar
        context.SaveChanges();
    }

    #endregion

}