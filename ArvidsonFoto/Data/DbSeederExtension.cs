using Microsoft.EntityFrameworkCore;
namespace ArvidsonFoto.Data;

public static class DbSeederExtension
{
    /// <summary>En extensionmetod skapad via 'DbSeederExtension.cs' , som skapar upp test-data till databasen.</summary>
    public static void InitialDatabaseSeed(this ModelBuilder modelBuilder)
    {
        TblGb gb1 = new() { Id = 1, GbId = 1, GbDate = new DateTime(2021, 11, 22), GbEmail = "pownas@outlook.com", GbName = "pownas", GbHomepage = "github.com/pownas", GbReadPost = false, GbText = "Ett första test inlägg i databasen..." };
        modelBuilder.Entity<TblGb>().HasData(gb1);
        
        TblMenu menu1 = new() { Id = 1, MenuId = 1, MenuMainId = 0, MenuText = "Fåglar", MenuUrltext = "Faglar" };
        TblMenu menu2 = new() { Id = 2, MenuId = 2, MenuMainId = 0, MenuText = "Däggdjur", MenuUrltext = "Daggdjur" };
        TblMenu menu3 = new() { Id = 3, MenuId = 3, MenuMainId = 0, MenuText = "Kräldjur", MenuUrltext = "Kraldjur" };
        TblMenu menu4 = new() { Id = 4, MenuId = 4, MenuMainId = 0, MenuText = "Insekter", MenuUrltext = "Insekter" };
        TblMenu menu5 = new() { Id = 5, MenuId = 5, MenuMainId = 0, MenuText = "Vaxter", MenuUrltext = "Vaxter" };
        TblMenu menu6 = new() { Id = 6, MenuId = 6, MenuMainId = 0, MenuText = "Landskap", MenuUrltext = "Landskap" };
        TblMenu menu7 = new() { Id = 7, MenuId = 7, MenuMainId = 0, MenuText = "Årstider", MenuUrltext = "Arstider" };
        TblMenu menu8 = new() { Id = 8, MenuId = 8, MenuMainId = 0, MenuText = "Resor", MenuUrltext = "Resor" };
        TblMenu menu9 = new() { Id = 9, MenuId = 9, MenuMainId = 1, MenuText = "Alkor och labbar", MenuUrltext = "Alkor-och-labbar" };
        TblMenu menu10 = new() { Id = 10, MenuId = 10, MenuMainId = 1, MenuText = "Tättingar", MenuUrltext = "Tattingar" };
        TblMenu menu11 = new() { Id = 11, MenuId = 11, MenuMainId = 9, MenuText = "Fjällabb", MenuUrltext = "Fjallabb" };
        TblMenu menu12 = new() { Id = 12, MenuId = 12, MenuMainId = 10, MenuText = "Mesar", MenuUrltext = "Mesar" };
        TblMenu menu13 = new() { Id = 13, MenuId = 13, MenuMainId = 12, MenuText = "Blåmes", MenuUrltext = "Blames" };
        modelBuilder.Entity<TblMenu>().HasData(menu1, menu2, menu3, menu4, menu5, menu6, menu7, menu8, menu9, menu10, menu11, menu12, menu13);

        TblPageCounter pageCounter1 = new() { Id = 10, CategoryId = 1, PicturePage = true, PageName = "Fåglar", MonthViewed = "2021-11", LastShowDate = new DateTime(2021,11,22,16,16,00), PageViews = 0 };
        TblPageCounter pageCounter2 = new() { Id = 11, CategoryId = 2, PicturePage = true, PageName = "Däggdjur", MonthViewed = "2021-11", LastShowDate = new DateTime(2021, 11, 22, 16, 16, 00), PageViews = 0 };
        modelBuilder.Entity<TblPageCounter>().HasData(pageCounter1, pageCounter2);

        TblImage image1 = new() { Id = 1, ImageId = 1, ImageArt = 11, ImageFamilj = 9, ImageHuvudfamilj = null, ImageUrl="08TA3696", ImageDescription = "En fjällabbs beskrivning...", ImageDate = new DateTime(2021, 11, 22, 16,21, 00), ImageUpdate = new DateTime(2021, 11, 22, 16, 21, 00) };
        TblImage image2 = new() { Id = 2, ImageId = 2, ImageArt = 13, ImageFamilj = 12, ImageHuvudfamilj = 10, ImageUrl = "B57W4725", ImageDescription = "Hane, beskrivning av blåmes....", ImageDate = new DateTime(2021, 11, 22, 16, 21, 00), ImageUpdate = new DateTime(2021, 11, 22, 16, 21, 00) };
        modelBuilder.Entity<TblImage>().HasData(image1, image2);
    }
}
