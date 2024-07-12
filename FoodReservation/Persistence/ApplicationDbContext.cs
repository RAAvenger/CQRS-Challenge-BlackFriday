using FoodReservation.Application.Repositories.Abstractions;
using FoodReservation.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodReservation.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IFoodReservationsDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ReservableFoodMetadata> FoodMetadata { get; }

        public DbSet<ReservableFood> Foods { get; }

        public DbSet<Domain.Entities.FoodReservation> Reservations { get; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ReservableFood>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<ReservableFoodMetadata>()
                .HasKey(x => new { x.FoodId, x.Date });

            modelBuilder.Entity<Domain.Entities.FoodReservation>()
                .HasKey(x => new { x.FoodId, x.UserId, x.Date });

            // relations
            modelBuilder.Entity<ReservableFood>()
                .HasMany<ReservableFoodMetadata>()
                .WithOne()
                .HasForeignKey(x => x.FoodId)
                .HasPrincipalKey(x => x.Id);

            modelBuilder.Entity<ReservableFood>()
                .HasMany<Domain.Entities.FoodReservation>()
                .WithOne()
                .HasForeignKey(x => x.FoodId)
                .HasPrincipalKey(x => x.Id);

            modelBuilder.Entity<ReservableFoodMetadata>()
                .HasMany<Domain.Entities.FoodReservation>()
                .WithOne()
                .HasForeignKey(x => new { x.FoodId, x.Date })
                .HasPrincipalKey(x => new { x.FoodId, x.Date });
        }
    }
}