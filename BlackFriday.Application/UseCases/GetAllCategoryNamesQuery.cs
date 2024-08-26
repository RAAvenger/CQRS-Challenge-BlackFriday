using BlackFriday.Application.Persistence.Abstraction;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace BlackFriday.Application.UseCases;

public sealed class GetAllCategoryNamesQuery : IRequest<IReadOnlyCollection<string>>
{
}

public sealed class GetAllCategoryNamesQueryHandler : IRequestHandler<GetAllCategoryNamesQuery, IReadOnlyCollection<string>>
{
	private readonly IBlackFridayDbContextFactory _dbContextFactory;

	public GetAllCategoryNamesQueryHandler(IBlackFridayDbContextFactory dbContextFactory)
	{
		_dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
	}

	public async ValueTask<IReadOnlyCollection<string>> Handle(GetAllCategoryNamesQuery request, CancellationToken cancellationToken)
	{
		using var dbContext = _dbContextFactory.MakeDbContext();
		var categories = await dbContext.Products
			.GroupBy(x => x.CategoryName)
			.Select(x => x.Key)
			.ToArrayAsync(cancellationToken);

		return categories;
	}
}
