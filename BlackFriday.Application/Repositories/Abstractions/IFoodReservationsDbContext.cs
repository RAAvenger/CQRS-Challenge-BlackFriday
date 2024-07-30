using BlackFriday.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlackFriday.Application.Repositories.Abstractions
{
    public interface IBlackFridaysDbContext
    {
        DbSet<ReservableDailyFood> DailyFoods { get; }

        DbSet<ReservableFood> Foods { get; }

        DbSet<Domain.Entities.BlackFriday> Reservations { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}