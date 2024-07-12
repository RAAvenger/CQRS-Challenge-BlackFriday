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

        public DbSet<ReservableDailyFood> DailyFoods { get; set; }

        public DbSet<ReservableFood> Foods { get; set; }

        public DbSet<Domain.Entities.FoodReservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ReservableFood>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<ReservableDailyFood>()
                .HasKey(x => new { x.FoodId, x.Date });

            modelBuilder.Entity<Domain.Entities.FoodReservation>()
                .HasKey(x => new { x.FoodId, x.UserId, x.Date });

            // relations
            modelBuilder.Entity<ReservableFood>()
                .HasMany<ReservableDailyFood>()
                .WithOne()
                .HasForeignKey(x => x.FoodId)
                .HasPrincipalKey(x => x.Id);

            modelBuilder.Entity<ReservableFood>()
                .HasMany<Domain.Entities.FoodReservation>()
                .WithOne()
                .HasForeignKey(x => x.FoodId)
                .HasPrincipalKey(x => x.Id);

            modelBuilder.Entity<ReservableDailyFood>()
                .HasMany<Domain.Entities.FoodReservation>()
                .WithOne()
                .HasForeignKey(x => new { x.FoodId, x.Date })
                .HasPrincipalKey(x => new { x.FoodId, x.Date });
        }
    }
}