﻿using BlackFriday.Application.Persistence.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace BlackFriday.Infrastructure.Persistence
{
	public class ApplicationDbContext : DbContext, IBlackFridayDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Basket> Baskets { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<ProductCount> ProductCounts { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(x => x.Asin);

                entity.Property(e => e.Asin)
                    .HasMaxLength(200)
                    .HasColumnName("asin");
                entity.Property(e => e.BoughtInLastMonth)
                    .HasColumnName("boughtInLastMonth");
                entity.Property(e => e.CategoryName)
                    .HasMaxLength(200)
                    .HasColumnName("categoryName");
                entity.Property(e => e.ImgUrl)
                    .HasMaxLength(200)
                    .HasColumnName("imgUrl");
                entity.Property(e => e.IsBestSeller)
                    .HasColumnName("isBestSeller");
                entity.Property(e => e.Price)
                    .HasPrecision(28, 6)
                    .HasColumnName("price");
                entity.Property(e => e.ProductUrl)
                    .HasMaxLength(200)
                    .HasColumnName("productUrl");
                entity.Property(e => e.Reviews)
                    .HasColumnName("reviews");
                entity.Property(e => e.Stars)
                    .HasPrecision(28, 6)
                    .HasColumnName("stars");
                entity.Property(e => e.Title)
                    .HasMaxLength(1500)
                    .HasColumnName("title");
            });

            modelBuilder.Entity<Basket>(entity =>
            {
                entity.HasKey(x => new { x.ProductId, x.UserId, x.BasketId });
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(x => new { x.UserId, x.BasketId });
            });

            modelBuilder.Entity<ProductCount>(entity =>
            {
                entity.HasKey(x => x.Asin);
            });
        }
    }
}