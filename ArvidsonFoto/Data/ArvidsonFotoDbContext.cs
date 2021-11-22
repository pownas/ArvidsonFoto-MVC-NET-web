using Microsoft.EntityFrameworkCore;

#nullable disable

namespace ArvidsonFoto.Data;

public partial class ArvidsonFotoDbContext : DbContext
{
    public ArvidsonFotoDbContext()
    {
    }

    public ArvidsonFotoDbContext(DbContextOptions<ArvidsonFotoDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblGb> TblGbs { get; set; }
    public virtual DbSet<TblImage> TblImages { get; set; }
    public virtual DbSet<TblMenu> TblMenus { get; set; }
    public virtual DbSet<TblPageCounter> TblPageCounter { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("Relational:Collation", "Finnish_Swedish_CI_AS");

        modelBuilder.Entity<TblGb>(entity =>
        {
            entity.ToTable("tbl_gb");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.GbId)
                .HasColumnName("GB_ID");

            entity.Property(e => e.GbDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("GB_date");

            entity.Property(e => e.GbEmail)
                .HasMaxLength(100)
                .HasColumnName("GB_email");

            entity.Property(e => e.GbHomepage)
                .HasMaxLength(255)
                .HasColumnName("GB_homepage");

            entity.Property(e => e.GbIp)
                .HasMaxLength(50)
                .HasColumnName("GB_IP");

            entity.Property(e => e.GbReadPost)
                .HasColumnType("bit")
                .HasColumnName("GB_ReadPost");

            entity.Property(e => e.GbName)
                .HasMaxLength(100)
                .HasColumnName("GB_name");

            entity.Property(e => e.GbText)
                .HasColumnName("GB_text");

            entity.HasKey("Id");
        });

        modelBuilder.Entity<TblImage>(entity =>
        {
            entity.ToTable("tbl_images");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.ImageId)
                .HasColumnName("image_ID");

            entity.Property(e => e.ImageHuvudfamilj)
                .HasColumnName("image_huvudfamilj");

            entity.Property(e => e.ImageFamilj)
                .HasColumnName("image_familj");

            entity.Property(e => e.ImageArt)
                .HasColumnName("image_art");

            entity.Property(e => e.ImageUrl)
                .HasMaxLength(50)
                .HasColumnName("image_URL");

            entity.Property(e => e.ImageDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("image_date");

            entity.Property(e => e.ImageDescription)
                .HasMaxLength(150)
                .HasColumnName("image_description");

            entity.Property(e => e.ImageUpdate)
                .HasColumnType("datetime")
                .HasColumnName("image_update");

            entity.HasKey("Id");
        });

        modelBuilder.Entity<TblMenu>(entity =>
        {
            entity.ToTable("tbl_menu");

            //entity.Property(e => e.MenuEngtext)
            //    .HasMaxLength(50)
            //    .HasColumnName("menu_ENGtext");
            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.MenuId)
                .HasColumnName("menu_ID");

            entity.Property(e => e.MenuMainId)
                .HasColumnName("menu_mainID");

            entity.Property(e => e.MenuText)
                .HasMaxLength(50)
                .HasColumnName("menu_text");

            entity.Property(e => e.MenuUrltext)
                .HasMaxLength(50)
                .HasColumnName("menu_URLtext");

            entity.HasKey("Id");
        });

        modelBuilder.Entity<TblPageCounter>(entity =>
        {
            entity.ToTable("tbl_PageCounter");

            entity.Property(e => e.Id).HasColumnName("PageCounter_ID");

            entity.Property(e => e.PageViews)
                .HasColumnName("PageCounter_Views");

            entity.Property(e => e.PageName)
                .HasMaxLength(50)
                .HasColumnName("PageCounter_Name");

            entity.Property(e => e.MonthViewed)
                .HasMaxLength(20)
                .HasColumnName("PageCounter_MonthViewed");

            entity.Property(e => e.LastShowDate)
                .HasColumnType("datetime")
                .HasColumnName("PageCounter_LastShowDate");

            entity.Property(e => e.PicturePage)
                .HasColumnType("bit")
                .HasColumnName("PageCounter_PicturePage");

            entity.Property(e => e.CategoryId)
                .HasColumnName("PageCounter_CategoryId");

            entity.HasKey("Id");
        });


        ///Kommentera bort denna raden för att göra en initial Databas Seed
        //modelBuilder.SeedDatabase();

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}