using System.Text.Json;
using BlackFriday.Application.Persistence.Abstraction;
using BlackFriday.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace BlackFriday.Application.UseCases;

public sealed class CheckoutBasketCommand : IRequest
{
	public required string BasketId { get; init; }
	public required string UserId { get; init; }
}

public sealed class CheckoutBasketCommandHandler : IRequestHandler<CheckoutBasketCommand>
{
	private readonly IBlackFridayDbContextFactory _dbContextFactory;

	public CheckoutBasketCommandHandler(IBlackFridayDbContextFactory dbContextFactory)
	{
		_dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
	}

	public async ValueTask<Unit> Handle(CheckoutBasketCommand request, CancellationToken cancellationToken)
	{
		using var dbContext = _dbContextFactory.MakeDbContext();
		var basketItems = await dbContext.Baskets
			.Where(x => x.BasketId == x.BasketId && x.UserId == x.UserId && !x.IsCheckedOut)
			.ToArrayAsync(cancellationToken: cancellationToken);

		foreach (var item in basketItems)
		{
			item.IsCheckedOut = true;
		}
		var itemsJson = JsonSerializer.Serialize(basketItems.Select(x => x.ProductId).ToArray());
		dbContext.Invoices.Add(new Invoice
		{
			BasketId = request.BasketId,
			UserId = request.UserId,
			Items = itemsJson
		});
		using (var transaction = dbContext.Database.BeginTransaction())
		{
			await UpdateCountsAsync(dbContext, basketItems, cancellationToken);
			await dbContext.SaveChangesAsync(cancellationToken);
			await transaction.CommitAsync(cancellationToken);
		}
		return Unit.Value;
	}

	private static async Task UpdateCountsAsync(IBlackFridayDbContext dbContext, Basket[] basketItems, CancellationToken cancellationToken)
	{
		foreach (var item in basketItems)
		{
			await dbContext.ProductCounts
				.Where(x => x.Asin == item.ProductId)
				.ExecuteUpdateAsync(x => x.SetProperty(productCount => productCount.Count,
						productCount => productCount.Count - 1),
					cancellationToken: cancellationToken);
		}
	}
}
