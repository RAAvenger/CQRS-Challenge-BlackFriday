using BlackFriday.Application.Repositories.Abstractions;
using BlackFriday.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlackFriday.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IBlackFridaysDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ReservableDailyFood> DailyFoods { get; set; }

        public DbSet<ReservableFood> Foods { get; set; }

        public DbSet<Domain.Entities.BlackFriday> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ReservableFood>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<ReservableDailyFood>()
                .HasKey(x => new { x.FoodId, x.Date });

            modelBuilder.Entity<Domain.Entities.BlackFriday>()
                .HasKey(x => new { x.FoodId, x.UserId, x.Date });

            // relations
            modelBuilder.Entity<ReservableFood>()
                .HasMany<ReservableDailyFood>()
                .WithOne()
                .HasForeignKey(x => x.FoodId)
                .HasPrincipalKey(x => x.Id);

            modelBuilder.Entity<ReservableFood>()
                .HasMany<Domain.Entities.BlackFriday>()
                .WithOne()
                .HasForeignKey(x => x.FoodId)
                .HasPrincipalKey(x => x.Id);

            modelBuilder.Entity<ReservableDailyFood>()
                .HasMany<Domain.Entities.BlackFriday>()
                .WithOne()
                .HasForeignKey(x => new { x.FoodId, x.Date })
                .HasPrincipalKey(x => new { x.FoodId, x.Date });
        }
    }
}