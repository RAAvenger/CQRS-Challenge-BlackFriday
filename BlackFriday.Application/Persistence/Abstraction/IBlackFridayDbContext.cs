using BlackFriday.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BlackFriday.Application.Persistence.Abstraction;

public interface IBlackFridayDbContext : IDisposable
{
	public DbSet<Basket> Baskets { get; }
	public DbSet<Invoice> Invoices { get; }
	public DbSet<ProductCount> ProductCounts { get; }
	public DbSet<Product> Products { get; }

	Task<int> SaveChangesAsync(CancellationToken cancellationToken);

	public DatabaseFacade Database { get; }
}
