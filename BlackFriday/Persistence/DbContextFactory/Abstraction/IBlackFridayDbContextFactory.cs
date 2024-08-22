using BlackFriday.Application.Persistence.Abstraction;

namespace BlackFriday.Infrastructure.Persistence.DbContextFactory.Abstraction;

public interface IBlackFridayDbContextFactory
{
	IBlackFridaysDbContext MakeDbContext();
}
