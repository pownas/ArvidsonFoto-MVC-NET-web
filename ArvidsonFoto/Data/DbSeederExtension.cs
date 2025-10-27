using ArvidsonFoto.Models;
using Microsoft.EntityFrameworkCore;
namespace ArvidsonFoto.Data;

/// <summary>
/// Database Seeder Extension
/// </summary>
/// <remarks>
/// Används för att skapa upp test-data lokalt till databasen.
/// </remarks>
public static class DbSeederExtension
{
    public static List<TblGb> DbSeed_Tbl_Gb { get; set; } = new List<TblGb>
    {
        new TblGb { 
            Id = 1, 
            GbId = 1, 
            GbDate = new DateTime(2021, 11, 22), 
            GbEmail = "pownas@outlook.com",
            GbName = "pownas", 
            GbNameEn = "pownas",
            GbHomepage = "github.com/pownas", 
            GbReadPost = false, 
            GbText = "Ett första test inlägg i databasen...",
            GbTextEn = "A first test post in the database..."
        },
    };

    public static List<TblMenu> DbSeed_Tbl_Menu { get; set; } = new List<TblMenu>
    {
        new TblMenu { Id = 1, MenuId = 1, MenuMainId = 0, MenuText = "Fåglar", MenuTextEn = "Birds", MenuUrltext = "Faglar", MenuUrltextEn = "Birds" },
        new TblMenu { Id = 2, MenuId = 2, MenuMainId = 0, MenuText = "Däggdjur", MenuTextEn = "Mammals", MenuUrltext = "Daggdjur", MenuUrltextEn = "Mammals" },
        new TblMenu { Id = 3, MenuId = 3, MenuMainId = 0, MenuText = "Kräldjur", MenuTextEn = "Reptiles", MenuUrltext = "Kraldjur", MenuUrltextEn = "Reptiles" },
        new TblMenu { Id = 4, MenuId = 4, MenuMainId = 0, MenuText = "Insekter", MenuTextEn = "Insects", MenuUrltext = "Insekter", MenuUrltextEn = "Insects" },
        new TblMenu { Id = 5, MenuId = 5, MenuMainId = 0, MenuText = "Växter", MenuTextEn = "Plants", MenuUrltext = "Vaxter", MenuUrltextEn = "Plants" },
        new TblMenu { Id = 6, MenuId = 6, MenuMainId = 0, MenuText = "Landskap", MenuTextEn = "Landscapes", MenuUrltext = "Landskap", MenuUrltextEn = "Landscapes" },
        new TblMenu { Id = 7, MenuId = 7, MenuMainId = 0, MenuText = "Årstider", MenuTextEn = "Seasons", MenuUrltext = "Arstider", MenuUrltextEn = "Seasons" },
        new TblMenu { Id = 8, MenuId = 8, MenuMainId = 0, MenuText = "Resor", MenuTextEn = "Travel", MenuUrltext = "Resor", MenuUrltextEn = "Travel" },
        new TblMenu { Id = 9, MenuId = 9, MenuMainId = 1, MenuText = "Alkor och labbar", MenuTextEn = "Auks and Puffins", MenuUrltext = "Alkor-och-labbar", MenuUrltextEn = "Auks-and-Puffins" },
        new TblMenu { Id = 10, MenuId = 10, MenuMainId = 1, MenuText = "Tättingar", MenuTextEn = "Passerines", MenuUrltext = "Tattingar", MenuUrltextEn = "Passerines" },
        new TblMenu { Id = 11, MenuId = 11, MenuMainId = 9, MenuText = "Fjällabb", MenuTextEn = "Little Auk", MenuUrltext = "Fjallabb", MenuUrltextEn = "Little-Auk" },
        new TblMenu { Id = 12, MenuId = 12, MenuMainId = 10, MenuText = "Mesar", MenuTextEn = "Tits", MenuUrltext = "Mesar", MenuUrltextEn = "Tits" },
        new TblMenu { Id = 13, MenuId = 13, MenuMainId = 12, MenuText = "Blåmes", MenuTextEn = "Blue Tit", MenuUrltext = "Blames", MenuUrltextEn = "Blue-Tit" },
        new TblMenu { Id = 14, MenuId = 14, MenuMainId = 2, MenuText = "Björn", MenuTextEn = "Bear", MenuUrltext = "Bjorn", MenuUrltextEn = "Bear" },
        new TblMenu { Id = 15, MenuId = 15, MenuMainId = 3, MenuText = "Hasselsnok", MenuTextEn = "Grass Snake", MenuUrltext = "Hasselsnok", MenuUrltextEn = "Grass-Snake" },
        new TblMenu { Id = 16, MenuId = 16, MenuMainId = 4, MenuText = "Skalbaggar", MenuTextEn = "Beetles", MenuUrltext = "Skalbaggar", MenuUrltextEn = "Beetles" },
        new TblMenu { Id = 17, MenuId = 17, MenuMainId = 5, MenuText = "Blommor", MenuTextEn = "Flowers", MenuUrltext = "Blommor", MenuUrltextEn = "Flowers" },
        new TblMenu { Id = 18, MenuId = 18, MenuMainId = 6, MenuText = "Gotland", MenuTextEn = "Gotland", MenuUrltext = "Gotland", MenuUrltextEn = "Gotland" },
        new TblMenu { Id = 19, MenuId = 19, MenuMainId = 7, MenuText = "Sommar", MenuTextEn = "Summer", MenuUrltext = "Sommar", MenuUrltextEn = "Summer" },
        new TblMenu { Id = 20, MenuId = 20, MenuMainId = 8, MenuText = "2008 Costa Rica", MenuTextEn = "2008 Costa Rica", MenuUrltext = "2008-Costa-Rica", MenuUrltextEn = "2008-Costa-Rica" },
    };

    public static List<TblPageCounter> DbSeed_Tbl_PageCounter { get; set; } = new List<TblPageCounter>
    {
        new TblPageCounter { 
            Id = 1, 
            CategoryId = 1, 
            PicturePage = true, 
            PageName = "Fåglar",
            PageNameEn = "Birds",
            MonthViewed = "2021-11",
            LastShowDate = new DateTime(2021,11,22,16,16,00), 
            PageViews = 0
        },
    };

    public static List<TblImage> DbSeed_Tbl_Image { get; set; } = new List<TblImage>
    {
        new TblImage {
            Id = 1,
            ImageId = 1,
            ImageArt = 11,
            ImageFamilj = 9,
            ImageHuvudfamilj = null,
            ImageUrl="08TA3696",
            ImageDescription = "En fjällabbs beskrivning...",
            ImageDescriptionEn = "A little auk description...",
            ImageDate = new DateTime(2021, 11, 22, 16,21, 00),
            ImageUpdate = new DateTime(2021, 11, 22, 16, 21, 00)
        },
        new TblImage {
            Id = 2,
            ImageId = 2,
            ImageArt = 13,
            ImageFamilj = 12,
            ImageHuvudfamilj = 10,
            ImageUrl = "B57W4725",
            ImageDescription = "Hane, beskrivning av blåmes....",
            ImageDescriptionEn = "Male, blue tit description....",
            ImageDate = new DateTime(2021, 11, 22, 16, 21, 00),
            ImageUpdate = new DateTime(2021, 11, 22, 16, 21, 00)
        },
        new TblImage {
            Id = 3,
            ImageId = 3,
            ImageArt = 14,
            ImageFamilj = 2,
            ImageHuvudfamilj = null,
            ImageUrl = "B59W4837",
            ImageDescription = "i Sverige",
            ImageDescriptionEn = "in Sweden",
            ImageDate = new DateTime(2021, 11, 22, 16, 21, 00),
            ImageUpdate = new DateTime(2021, 11, 22, 16, 21, 00)
        },
        new TblImage
        {
            Id = 4,
            ImageId = 4,
            ImageArt = 15,
            ImageFamilj = 3,
            ImageHuvudfamilj = null,
            ImageUrl = "13TA5142",
            ImageDescription = "",
            ImageDescriptionEn = "",
            ImageDate = new DateTime(2021, 11, 22, 16, 21, 00),
            ImageUpdate = new DateTime(2021, 11, 22, 16, 21, 00)
        },
        new TblImage
        {
            Id = 5,
            ImageId = 5,
            ImageArt = 16,
            ImageFamilj = 4,
            ImageHuvudfamilj = null,
            ImageUrl = "B60W1277",
            ImageDescription = "Ekoxe, hane",
            ImageDescriptionEn = "Stag beetle, male",
            ImageDate = new DateTime(2021, 11, 22, 16, 21, 00),
            ImageUpdate = new DateTime(2021, 11, 22, 16, 21, 00)
        },
        new TblImage
        {
            Id = 6,
            ImageId = 6,
            ImageArt = 17,
            ImageFamilj = 5,
            ImageHuvudfamilj = null,
            ImageUrl = "09TA8491",
            ImageDescription = "Fjällviol",
            ImageDescriptionEn = "Mountain violet",
            ImageDate = new DateTime(2021, 11, 22, 16, 21, 00),
            ImageUpdate = new DateTime(2021, 11, 22, 16, 21, 00)
        },
        new TblImage
        {
            Id = 7,
            ImageId = 7,
            ImageArt = 18,
            ImageFamilj = 6,
            ImageHuvudfamilj = null,
            ImageUrl = "B57W8697",
            ImageDescription = "Rauk",
            ImageDescriptionEn = "Sea stack",
            ImageDate = new DateTime(2021, 11, 22, 16, 21, 00),
            ImageUpdate = new DateTime(2021, 11, 22, 16, 21, 00)
        },
        new TblImage
        {
            Id = 8,
            ImageId = 8,
            ImageArt = 19,
            ImageFamilj = 7,
            ImageHuvudfamilj = null,
            ImageUrl = "IMG_1496",
            ImageDescription = "Jordgubbar",
            ImageDescriptionEn = "Strawberries",
            ImageDate = new DateTime(2021, 11, 22, 16, 21, 00),
            ImageUpdate = new DateTime(2021, 11, 22, 16, 21, 00)
        },
        new TblImage
        {
            Id = 9,
            ImageId = 9,
            ImageArt = 20,
            ImageFamilj = 8,
            ImageHuvudfamilj = null,
            ImageUrl = "_N0Q8690",
            ImageDescription = "Hane, beskrivning av Costa rica....",
            ImageDescriptionEn = "Male, Costa Rica description....",
            ImageDate = new DateTime(2021, 11, 22, 16, 21, 00),
            ImageUpdate = new DateTime(2021, 11, 22, 16, 21, 00)
        },
    };

    /// <summary>
    /// En extensionmetod skapad via 'DbSeederExtension.cs'
    /// </summary>
    /// <remarks>
    /// Skapar upp den test-data till databasen via Entity Framework Core för att kunna starta applikationen lokalt.
    /// </remarks>
    public static void InitialDatabaseSeed(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblGb>().HasData(DbSeed_Tbl_Gb);
        
        modelBuilder.Entity<TblMenu>().HasData(DbSeed_Tbl_Menu);

        modelBuilder.Entity<TblPageCounter>().HasData(DbSeed_Tbl_PageCounter);

        modelBuilder.Entity<TblImage>().HasData(DbSeed_Tbl_Image);
    }

    /// <summary>
    /// Seeds the in-memory database with test data
    /// </summary>
    /// <remarks>
    /// Används för att seeda in-memory databasen med test-data när applikationen körs i Codespaces eller utvecklingsmiljö
    /// </remarks>
    public static void SeedInMemoryDatabase(this ArvidsonFotoDbContext context)
    {
        // Kontrollera om databasen redan har data
        if (context.TblGbs.Any() || context.TblMenus.Any() || context.TblImages.Any())
        {
            return; // Databasen är redan seedand
        }

        // Lägg till test-data
        context.TblGbs.AddRange(DbSeed_Tbl_Gb);
        context.TblMenus.AddRange(DbSeed_Tbl_Menu);
        context.TblPageCounter.AddRange(DbSeed_Tbl_PageCounter);
        context.TblImages.AddRange(DbSeed_Tbl_Image);

        // Spara ändringar
        context.SaveChanges();
    }
}