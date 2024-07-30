using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BlackFriday.Infrastructure;

public partial class BlackFridayDbContext : DbContext
{
    public BlackFridayDbContext()
    {
    }

    public BlackFridayDbContext(DbContextOptions<BlackFridayDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=black_friday_db;Username=postgres;Password=!@#123qwe");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("products", tb => tb.HasComment("TRIAL"));

            entity.Property(e => e.Asin)
                .HasMaxLength(200)
                .HasComment("TRIAL")
                .HasColumnName("asin");
            entity.Property(e => e.BoughtInLastMonth)
                .HasComment("TRIAL")
                .HasColumnName("boughtInLastMonth");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(200)
                .HasComment("TRIAL")
                .HasColumnName("categoryName");
            entity.Property(e => e.ImgUrl)
                .HasMaxLength(200)
                .HasComment("TRIAL")
                .HasColumnName("imgUrl");
            entity.Property(e => e.IsBestSeller)
                .HasComment("TRIAL")
                .HasColumnName("isBestSeller");
            entity.Property(e => e.Price)
                .HasPrecision(28, 6)
                .HasComment("TRIAL")
                .HasColumnName("price");
            entity.Property(e => e.ProductUrl)
                .HasMaxLength(200)
                .HasComment("TRIAL")
                .HasColumnName("productUrl");
            entity.Property(e => e.Reviews)
                .HasComment("TRIAL")
                .HasColumnName("reviews");
            entity.Property(e => e.Stars)
                .HasPrecision(28, 6)
                .HasComment("TRIAL")
                .HasColumnName("stars");
            entity.Property(e => e.Title)
                .HasMaxLength(1500)
                .HasComment("TRIAL")
                .HasColumnName("title");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
