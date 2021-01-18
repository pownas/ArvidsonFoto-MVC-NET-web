using System;
using ArvidsonFoto.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ArvidsonFoto.Data
{
    public partial class ArvidsonFotoDbContext : DbContext
    {
        public ArvidsonFotoDbContext()
        {
        }

        public ArvidsonFotoDbContext(DbContextOptions<ArvidsonFotoDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TblAdmin> TblAdmins { get; set; }
        public virtual DbSet<TblGb> TblGbs { get; set; }
        public virtual DbSet<TblImage> TblImages { get; set; }
        public virtual DbSet<TblMenu> TblMenus { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=ArvidsonFotoBlazorDapperTest;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Finnish_Swedish_CI_AS");

            modelBuilder.Entity<TblAdmin>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("tbl_admin");

                entity.Property(e => e.AdminId).HasColumnName("admin_ID");

                entity.Property(e => e.AdminLastonline)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("admin_lastonline");

                entity.Property(e => e.AdminMail)
                    .HasMaxLength(100)
                    .HasColumnName("admin_mail");

                entity.Property(e => e.AdminName)
                    .HasMaxLength(30)
                    .HasColumnName("admin_name");

                entity.Property(e => e.AdminPass)
                    .HasMaxLength(255)
                    .HasColumnName("admin_pass");

                entity.Property(e => e.AdminUser)
                    .HasMaxLength(20)
                    .HasColumnName("admin_user");
            });

            modelBuilder.Entity<TblGb>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("tbl_gb");

                entity.Property(e => e.GbDate)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("GB_date");

                entity.Property(e => e.GbEmail)
                    .HasMaxLength(100)
                    .HasColumnName("GB_email");

                entity.Property(e => e.GbHomepage)
                    .HasMaxLength(255)
                    .HasColumnName("GB_homepage");

                entity.Property(e => e.GbId).HasColumnName("GB_ID");

                entity.Property(e => e.GbIp)
                    .HasMaxLength(255)
                    .HasColumnName("GB_IP");

                entity.Property(e => e.GbName)
                    .HasMaxLength(100)
                    .HasColumnName("GB_name");

                entity.Property(e => e.GbText).HasColumnName("GB_text");
            });

            modelBuilder.Entity<TblImage>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("tbl_images");

                entity.Property(e => e.ImageArt).HasColumnName("image_art");

                entity.Property(e => e.ImageDate)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("image_date");

                entity.Property(e => e.ImageDescription)
                    .HasMaxLength(50)
                    .HasColumnName("image_description");

                entity.Property(e => e.ImageFamilj).HasColumnName("image_familj");

                entity.Property(e => e.ImageHuvudfamilj).HasColumnName("image_huvudfamilj");

                entity.Property(e => e.ImageId).HasColumnName("image_ID");

                entity.Property(e => e.ImageUpdate)
                    .HasColumnType("datetime")
                    .HasColumnName("image_update");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(50)
                    .HasColumnName("image_URL");
            });

            modelBuilder.Entity<TblMenu>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("tbl_menu");

                entity.Property(e => e.MenuEngtext)
                    .HasMaxLength(50)
                    .HasColumnName("menu_ENGtext");

                entity.Property(e => e.MenuId).HasColumnName("menu_ID");

                entity.Property(e => e.MenuLastshowdate)
                    .HasColumnType("datetime")
                    .HasColumnName("menu_lastshowdate");

                entity.Property(e => e.MenuMainId).HasColumnName("menu_mainID");

                entity.Property(e => e.MenuPagecounter).HasColumnName("menu_pagecounter");

                entity.Property(e => e.MenuText)
                    .HasMaxLength(50)
                    .HasColumnName("menu_text");

                entity.Property(e => e.MenuUrltext)
                    .HasMaxLength(50)
                    .HasColumnName("menu_URLtext");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
