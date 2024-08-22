using BlackFriday.Application.Persistence.Abstraction;
using BlackFriday.Infrastructure;
using Mediator;

namespace BlackFriday.Application.UseCases;

public sealed class AddItemToBasketCommand : IRequest
{
	public required string BasketId { get; init; }
	public required string ProductId { get; init; }
	public required string UserId { get; init; }
}

internal sealed class AddItemToBasketCommandHandler : IRequestHandler<AddItemToBasketCommand>
{
	private readonly IBlackFridayDbContextFactory _dbContextFactory;

	public AddItemToBasketCommandHandler(IBlackFridayDbContextFactory dbContextFactory)
	{
		_dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
	}

	public async ValueTask<Unit> Handle(AddItemToBasketCommand request, CancellationToken cancellationToken)
	{
		using var dbContext = _dbContextFactory.MakeDbContext();
		dbContext.Baskets.Add(new Basket
		{
			BasketId = request.BasketId,
			UserId = request.UserId,
			IsCheckedOut = false,
			ProductId = request.ProductId,
		});
		await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
		return Unit.Value;
	}
}
