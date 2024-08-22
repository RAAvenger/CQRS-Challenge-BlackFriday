using BlackFriday.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BlackFriday.Application.Persistence.Abstraction;

public interface IBlackFridaysDbContext : IDisposable
{
	public DbSet<Basket> Baskets { get; }
	public DbSet<Invoice> Invoices { get; }
	public DbSet<ProductCount> ProductCounts { get; }
	public DbSet<Product> Products { get; }

	Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
