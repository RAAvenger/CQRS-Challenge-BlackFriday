using BlackFriday.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BlackFriday.Application.Repositories.Abstractions
{
    public interface IBlackFridaysDbContext
    {
        public DbSet<Product> Products { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}