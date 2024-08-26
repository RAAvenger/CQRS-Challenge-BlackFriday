using BlackFriday.Application.Persistence.Abstraction;
using BlackFriday.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace BlackFriday.Application.UseCases;

public sealed class GetProductByIdQuery : IRequest<Product?>
{
	public required string Id { get; init; }
}

public sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Product?>
{
	private readonly IBlackFridayDbContextFactory _dbContextFactory;

	public GetProductByIdQueryHandler(IBlackFridayDbContextFactory dbContextFactory)
	{
		_dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
	}

	public async ValueTask<Product?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
	{
		using var dbContext = _dbContextFactory.MakeDbContext();
		return await dbContext.Products
			.FirstOrDefaultAsync(x => x.Asin == request.Id, cancellationToken);
	}
}
