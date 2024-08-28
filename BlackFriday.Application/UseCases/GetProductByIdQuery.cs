using System.Collections.Concurrent;
using System.Diagnostics;
using BlackFriday.Application.Commons.Tracing;
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
	private readonly Dictionary<string, Product> _products = new Dictionary<string, Product>();
	private readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores = new ConcurrentDictionary<string, SemaphoreSlim>();

	public GetProductByIdQueryHandler(IBlackFridayDbContextFactory dbContextFactory)
	{
		_dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
	}

	public async ValueTask<Product?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
	{
		using var activity = ApplicationActivityProvider.ActivitySource.StartActivity(nameof(GetProductByIdQuery));
		activity?.SetTag("ProductId", request.Id);
		Product? product = null;
		if (_products.TryGetValue(request.Id, out product))
		{
			return product;
		}

		var semaphore = _semaphores.GetOrAdd(request.Id, (id) => new SemaphoreSlim(1, 1));
		try
		{
			activity?.AddEvent(new ActivityEvent("wait for lock"));
			semaphore.Wait(cancellationToken);
			activity?.AddEvent(new ActivityEvent("enter lock"));
			if (!_products.TryGetValue(request.Id, out product))
			{
				activity?.AddEvent(new ActivityEvent("get product from database"));
				product = await GetProductFromDbAsync(request, cancellationToken);
				if (product is not null)
				{
					_products.Add(request.Id, product);
				}
				else
				{
					_semaphores.Remove(request.Id, out _);
				}
			}
			activity?.AddEvent(new ActivityEvent("exit lock"));
		}
		catch (Exception exception)
		{
			activity?.SetTag("exception", exception);
			throw;
		}
		finally
		{
			semaphore.Release(1);
		}
		return product;
	}

	private async ValueTask<Product?> GetProductFromDbAsync(GetProductByIdQuery request, CancellationToken cancellationToken)
	{
		using var dbContext = _dbContextFactory.MakeDbContext();
		return await dbContext.Products
			.FirstOrDefaultAsync(x => x.Asin == request.Id, cancellationToken);
	}
}
