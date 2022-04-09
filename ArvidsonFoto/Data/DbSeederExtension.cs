using Microsoft.EntityFrameworkCore;
namespace ArvidsonFoto.Data;

public static class DbSeederExtension
{
    /// <summary>En extensionmetod skapad via 'DbSeederExtension.cs' , som skapar upp test-data till databasen.</summary>
    public static void InitialDatabaseSeed(this ModelBuilder modelBuilder)
    {
        TblGb gb1 = new() { Id = 1, GbId = 1, GbDate = new DateTime(2021, 11, 22), GbEmail = "pownas@outlook.com", 
                            GbName = "pownas", GbHomepage = "github.com/pownas", GbReadPost = false, GbText = "Ett första test inlägg i databasen..." };
        modelBuilder.Entity<TblGb>().HasData(gb1);
        
        TblMenu menu1 = new() { Id = 1, MenuId = 1, MenuMainId = 0, MenuText = "Fåglar", MenuUrltext = "Faglar" };
        TblMenu menu2 = new() { Id = 2, MenuId = 2, MenuMainId = 0, MenuText = "Däggdjur", MenuUrltext = "Daggdjur" };
        TblMenu menu3 = new() { Id = 3, MenuId = 3, MenuMainId = 0, MenuText = "Kräldjur", MenuUrltext = "Kraldjur" };
        TblMenu menu4 = new() { Id = 4, MenuId = 4, MenuMainId = 0, MenuText = "Insekter", MenuUrltext = "Insekter" };
        TblMenu menu5 = new() { Id = 5, MenuId = 5, MenuMainId = 0, MenuText = "Växter", MenuUrltext = "Vaxter" };
        TblMenu menu6 = new() { Id = 6, MenuId = 6, MenuMainId = 0, MenuText = "Landskap", MenuUrltext = "Landskap" };
        TblMenu menu7 = new() { Id = 7, MenuId = 7, MenuMainId = 0, MenuText = "Årstider", MenuUrltext = "Arstider" };
        TblMenu menu8 = new() { Id = 8, MenuId = 8, MenuMainId = 0, MenuText = "Resor", MenuUrltext = "Resor" };
        TblMenu menu9 = new() { Id = 9, MenuId = 9, MenuMainId = 1, MenuText = "Alkor och labbar", MenuUrltext = "Alkor-och-labbar" };
        TblMenu menu10 = new() { Id = 10, MenuId = 10, MenuMainId = 1, MenuText = "Tättingar", MenuUrltext = "Tattingar" };
        TblMenu menu11 = new() { Id = 11, MenuId = 11, MenuMainId = 9, MenuText = "Fjällabb", MenuUrltext = "Fjallabb" };
        TblMenu menu12 = new() { Id = 12, MenuId = 12, MenuMainId = 10, MenuText = "Mesar", MenuUrltext = "Mesar" };
        TblMenu menu13 = new() { Id = 13, MenuId = 13, MenuMainId = 12, MenuText = "Blåmes", MenuUrltext = "Blames" };
        TblMenu menu14 = new() { Id = 14, MenuId = 14, MenuMainId = 2, MenuText = "Björn", MenuUrltext = "Bjorn" };
        TblMenu menu15 = new() { Id = 15, MenuId = 15, MenuMainId = 3, MenuText = "Hasselsnok", MenuUrltext = "Hasselsnok" };
        TblMenu menu16 = new() { Id = 16, MenuId = 16, MenuMainId = 4, MenuText = "Skalbaggar", MenuUrltext = "Skalbaggar" };
        TblMenu menu17 = new() { Id = 17, MenuId = 17, MenuMainId = 5, MenuText = "Blommor", MenuUrltext = "Blommor" };
        TblMenu menu18 = new() { Id = 18, MenuId = 18, MenuMainId = 6, MenuText = "Gotland", MenuUrltext = "Gotland" };
        TblMenu menu19 = new() { Id = 19, MenuId = 19, MenuMainId = 7, MenuText = "Sommar", MenuUrltext = "Sommar" };
        TblMenu menu20 = new() { Id = 20, MenuId = 20, MenuMainId = 8, MenuText = "2008 Costa Rica", MenuUrltext = "2008-Costa-Rica" };
        modelBuilder.Entity<TblMenu>().HasData(menu1, menu2, menu3, menu4, menu5, menu6, menu7, menu8, menu9, menu10, 
                                               menu11, menu12, menu13, menu14, menu15, menu16, menu17, menu18, menu19, menu20);

        TblPageCounter pageCounter1 = new() { Id = 1, CategoryId = 1, PicturePage = true, PageName = "Fåglar", MonthViewed = "2021-11", 
                                              LastShowDate = new DateTime(2021,11,22,16,16,00), PageViews = 0 };
        modelBuilder.Entity<TblPageCounter>().HasData(pageCounter1);

        TblImage image1 = new() { 
            Id = 1, 
            ImageId = 1, 
            ImageArt = 11, 
            ImageFamilj = 9, 
            ImageHuvudfamilj = null, 
            ImageUrl="08TA3696", 
            ImageDescription = "En fjällabbs beskrivning...", 
            ImageDate = new DateTime(2021, 11, 22, 16,21, 00), 
            ImageUpdate = new DateTime(2021, 11, 22, 16, 21, 00)
        };
        TblImage image2 = new() { 
            Id = 2, 
            ImageId = 2, 
            ImageArt = 13, 
            ImageFamilj = 12, 
            ImageHuvudfamilj = 10, 
            ImageUrl = "B57W4725", 
            ImageDescription = "Hane, beskrivning av blåmes....", 
            ImageDate = new DateTime(2021, 11, 22, 16, 21, 00), 
            ImageUpdate = new DateTime(2021, 11, 22, 16, 21, 00)
        };
        TblImage image3 = new()
        {
            Id = 3,
            ImageId = 3,
            ImageArt = 14,
            ImageFamilj = 2,
            ImageHuvudfamilj = null,
            ImageUrl = "B59W4837",
            ImageDescription = "i Sverige",
            ImageDate = new DateTime(2021, 11, 22, 16, 21, 00),
            ImageUpdate = new DateTime(2021, 11, 22, 16, 21, 00)
        };
        TblImage image4 = new()
        {
            Id = 4,
            ImageId = 4,
            ImageArt = 15,
            ImageFamilj = 3,
            ImageHuvudfamilj = null,
            ImageUrl = "13TA5142",
            ImageDescription = "",
            ImageDate = new DateTime(2021, 11, 22, 16, 21, 00),
            ImageUpdate = new DateTime(2021, 11, 22, 16, 21, 00)
        };
        TblImage image5 = new()
        {
            Id = 5,
            ImageId = 5,
            ImageArt = 16,
            ImageFamilj = 4,
            ImageHuvudfamilj = null,
            ImageUrl = "B60W1277",
            ImageDescription = "Ekoxe, hane",
            ImageDate = new DateTime(2021, 11, 22, 16, 21, 00),
            ImageUpdate = new DateTime(2021, 11, 22, 16, 21, 00)
        };
        TblImage image6 = new()
        {
            Id = 6,
            ImageId = 6,
            ImageArt = 17,
            ImageFamilj = 5,
            ImageHuvudfamilj = null,
            ImageUrl = "09TA8491",
            ImageDescription = "Fjällviol",
            ImageDate = new DateTime(2021, 11, 22, 16, 21, 00),
            ImageUpdate = new DateTime(2021, 11, 22, 16, 21, 00)
        };
        TblImage image7 = new()
        {
            Id = 7,
            ImageId = 7,
            ImageArt = 18,
            ImageFamilj = 6,
            ImageHuvudfamilj = null,
            ImageUrl = "B57W8697",
            ImageDescription = "Rauk",
            ImageDate = new DateTime(2021, 11, 22, 16, 21, 00),
            ImageUpdate = new DateTime(2021, 11, 22, 16, 21, 00)
        };
        TblImage image8 = new()
        {
            Id = 8,
            ImageId = 8,
            ImageArt = 19,
            ImageFamilj = 7,
            ImageHuvudfamilj = null,
            ImageUrl = "IMG_1496",
            ImageDescription = "Jordgubbar",
            ImageDate = new DateTime(2021, 11, 22, 16, 21, 00),
            ImageUpdate = new DateTime(2021, 11, 22, 16, 21, 00)
        };
        TblImage image9 = new()
        {
            Id = 9,
            ImageId = 9,
            ImageArt = 20,
            ImageFamilj = 8,
            ImageHuvudfamilj = null,
            ImageUrl = "_N0Q8690",
            ImageDescription = "Hane, beskrivning av Costa rica....",
            ImageDate = new DateTime(2021, 11, 22, 16, 21, 00),
            ImageUpdate = new DateTime(2021, 11, 22, 16, 21, 00)
        };

        modelBuilder.Entity<TblImage>().HasData(image1, image2, image3, image4, image5, image6, image7, image8, image9);
    }
}