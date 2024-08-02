﻿// <auto-generated />
using BlackFriday.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BlackFriday.Infrastructure.Persistence.DbMigrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BlackFriday.Infrastructure.Basket", b =>
                {
                    b.Property<string>("ProductId")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("BasketId")
                        .HasColumnType("text");

                    b.Property<bool>("IsCheckedOut")
                        .HasColumnType("boolean");

                    b.HasKey("ProductId", "UserId", "BasketId");

                    b.ToTable("Baskets");
                });

            modelBuilder.Entity("BlackFriday.Infrastructure.Invoice", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("BasketId")
                        .HasColumnType("text");

                    b.Property<string>("Items")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UserId", "BasketId");

                    b.ToTable("Invoices");
                });

            modelBuilder.Entity("BlackFriday.Infrastructure.Product", b =>
                {
                    b.Property<string>("Asin")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("asin");

                    b.Property<long>("BoughtInLastMonth")
                        .HasColumnType("bigint")
                        .HasColumnName("boughtInLastMonth");

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("categoryName");

                    b.Property<string>("ImgUrl")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("imgUrl");

                    b.Property<bool>("IsBestSeller")
                        .HasColumnType("boolean")
                        .HasColumnName("isBestSeller");

                    b.Property<decimal>("Price")
                        .HasPrecision(28, 6)
                        .HasColumnType("numeric(28,6)")
                        .HasColumnName("price");

                    b.Property<string>("ProductUrl")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("productUrl");

                    b.Property<long>("Reviews")
                        .HasColumnType("bigint")
                        .HasColumnName("reviews");

                    b.Property<decimal>("Stars")
                        .HasPrecision(28, 6)
                        .HasColumnType("numeric(28,6)")
                        .HasColumnName("stars");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(1500)
                        .HasColumnType("character varying(1500)")
                        .HasColumnName("title");

                    b.HasKey("Asin");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("BlackFriday.Infrastructure.ProductCount", b =>
                {
                    b.Property<string>("Asin")
                        .HasColumnType("text");

                    b.Property<int>("Count")
                        .HasColumnType("integer");

                    b.HasKey("Asin");

                    b.ToTable("ProductCounts");
                });
#pragma warning restore 612, 618
        }
    }
}
