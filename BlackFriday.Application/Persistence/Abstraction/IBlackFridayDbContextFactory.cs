namespace BlackFriday.Application.Persistence.Abstraction;

public interface IBlackFridayDbContextFactory
{
	IBlackFridaysDbContext MakeDbContext();
}
