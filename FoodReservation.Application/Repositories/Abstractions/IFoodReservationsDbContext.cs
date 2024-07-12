using FoodReservation.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodReservation.Application.Repositories.Abstractions
{
    public interface IFoodReservationsDbContext
    {
        DbSet<ReservableDailyFood> DailyFoods { get; }

        DbSet<ReservableFood> Foods { get; }

        DbSet<Domain.Entities.FoodReservation> Reservations { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}