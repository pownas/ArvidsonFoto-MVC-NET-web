﻿using Microsoft.EntityFrameworkCore;

#nullable disable

namespace ArvidsonFoto.Core.Data;

/// <summary>
/// Databaskontext för ArvidsonFoto-applikationen.
/// </summary>
/// <remarks>
/// Denna klass representerar en Entity Framework Core-databaskontext som innehåller
/// DbSet-egenskaper för alla huvudentiteter i applikationen.
/// </remarks>
public partial class ArvidsonFotoDbContext : DbContext
{
    /// <summary>
    /// Initierar en ny instans av ArvidsonFotoDbContext.
    /// </summary>
    public ArvidsonFotoDbContext()
    {
    }

    /// <summary>
    /// Initierar en ny instans av ArvidsonFotoDbContext med specificerade alternativ.
    /// </summary>
    /// <param name="options">Databaskontext-alternativ</param>
    public ArvidsonFotoDbContext(DbContextOptions<ArvidsonFotoDbContext> options)
        : base(options)
    {
    }

    /// <summary>DbSet för gästboksinlägg</summary>
    public virtual DbSet<TblGb> TblGbs { get; set; } = null!;

    /// <summary>DbSet för bilder</summary>
    public virtual DbSet<TblImage> TblImages { get; set; } = null!;

    /// <summary>DbSet för menykategorier</summary>
    public virtual DbSet<TblMenu> TblMenus { get; set; } = null!;

    /// <summary>DbSet för sidräknare</summary>
    public virtual DbSet<TblPageCounter> TblPageCounter { get; set; } = null!;


    /// <summary>
    /// Konfigurerar databasmodellen och entitetsrelationer.
    /// </summary>
    /// <param name="modelBuilder">Modellbyggare för Entity Framework</param>
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

            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<TblImage>(entity =>
        {
            entity.ToTable("tbl_images");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.ImageId)
                .HasColumnName("image_ID");

            entity.Property(e => e.ImageMainFamilyId)
                .HasColumnName("image_huvudfamilj");

            entity.Property(e => e.ImageFamilyId)
                .HasColumnName("image_familj");

            entity.Property(e => e.ImageCategoryId)
                .HasColumnName("image_art");

            entity.Property(e => e.ImageUrlName)
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

            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<TblMenu>(entity =>
        {
            entity.ToTable("tbl_menu");

            //entity.Property(e => e.MenuEngtext)
            //    .HasMaxLength(50)
            //    .HasColumnName("menu_ENGtext");
            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.MenuCategoryId)
                .HasColumnName("menu_ID");

            entity.Property(e => e.MenuParentCategoryId)
                .HasColumnName("menu_mainID");

            entity.Property(e => e.MenuDisplayName)
                .HasMaxLength(50)
                .HasColumnName("menu_text");

            entity.Property(e => e.MenuUrlSegment)
                .HasMaxLength(50)
                .HasColumnName("menu_URLtext");

            entity.Property(e => e.MenuDateUpdated)
                .HasColumnType("datetime")
                .HasColumnName("menu_dateUpdated");

            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<TblPageCounter>(entity =>
        {
            entity.ToTable("tbl_PageCounter");

            entity.Property(e => e.Id)
                .HasColumnName("PageCounter_ID")
                .ValueGeneratedOnAdd()
                .UseIdentityColumn(1, 1); // Explicitly configure as IDENTITY column

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

            entity.HasKey(e => e.Id);
        });


        ////Kommentera bort denna raden för att göra en initial Databas Seed
        //modelBuilder.InitialDatabaseSeed();

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}