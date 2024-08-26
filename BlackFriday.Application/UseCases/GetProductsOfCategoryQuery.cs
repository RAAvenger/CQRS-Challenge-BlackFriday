using BlackFriday.Application.Persistence.Abstraction;
using BlackFriday.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace BlackFriday.Application.UseCases;

public sealed class GetProductsOfCategoryQuery : IRequest<IReadOnlyCollection<Product>>
{
	public required string Id { get; init; }
}

public sealed class GetProductsOfCategoryQueryHandler : IRequestHandler<GetProductsOfCategoryQuery, IReadOnlyCollection<Product>>
{
	private readonly IBlackFridayDbContextFactory _dbContextFactory;

	public GetProductsOfCategoryQueryHandler(IBlackFridayDbContextFactory dbContextFactory)
	{
		_dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
	}

	public async ValueTask<IReadOnlyCollection<Product>> Handle(GetProductsOfCategoryQuery request, CancellationToken cancellationToken)
	{
		using var dbContext = _dbContextFactory.MakeDbContext();
		return await dbContext.Products
			.Where(x => x.CategoryName == request.Id)
			.ToArrayAsync(cancellationToken);
	}
}
