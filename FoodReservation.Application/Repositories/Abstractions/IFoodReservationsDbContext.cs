using FoodReservation.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodReservation.Application.Repositories.Abstractions
{
    public interface IFoodReservationsDbContext
    {
        DbSet<ReservableFoodMetadata> FoodMetadata { get; }

        DbSet<ReservableFood> Foods { get; }

        DbSet<Domain.Entities.FoodReservation> Reservations { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}