namespace BlackFriday.Application.Persistence.Abstraction;

public interface IBlackFridayDbContextFactory
{
	IBlackFridayDbContext MakeDbContext();
}
