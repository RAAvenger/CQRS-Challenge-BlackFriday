using BlackFriday.Application.Persistence.Abstraction;
using BlackFriday.Infrastructure.Persistence.DbContextFactory.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace BlackFriday.Infrastructure.Persistence.DbContextFactory;

public class BlackFridayDbContextFactory : IBlackFridayDbContextFactory
{
	private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

	public BlackFridayDbContextFactory(IDbContextFactory<ApplicationDbContext> dbContextFactory)
	{
		_dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
	}

	public IBlackFridaysDbContext MakeDbContext()
	{
		return _dbContextFactory.CreateDbContext();
	}
}
