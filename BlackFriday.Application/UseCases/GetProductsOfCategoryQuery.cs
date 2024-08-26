using System.Collections.Concurrent;
using BlackFriday.Application.Commons.Tracing;
using System.Diagnostics;
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
	private readonly Dictionary<string, IReadOnlyCollection<Product>> _categories = new Dictionary<string, IReadOnlyCollection<Product>>();
	private readonly IBlackFridayDbContextFactory _dbContextFactory;
	private readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores = new ConcurrentDictionary<string, SemaphoreSlim>();

	public GetProductsOfCategoryQueryHandler(IBlackFridayDbContextFactory dbContextFactory)
	{
		_dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
	}

	public async ValueTask<IReadOnlyCollection<Product>> Handle(GetProductsOfCategoryQuery request, CancellationToken cancellationToken)
	{
		using var activity = ApplicationActivityProvider.ActivitySource.StartActivity(nameof(GetProductsOfCategoryQuery));
		activity?.SetTag("CategoryId", request.Id);
		IReadOnlyCollection<Product> products = null;
		if (_categories.TryGetValue(request.Id, out products))
		{
			return products;
		}

		var semaphore = _semaphores.GetOrAdd(request.Id, (id) => new SemaphoreSlim(1, 1));
		activity?.AddEvent(new ActivityEvent("wait for lock"));
		semaphore.Wait(cancellationToken);
		activity?.AddEvent(new ActivityEvent("enter lock"));
		if (!_categories.TryGetValue(request.Id, out products))
		{
			activity?.AddEvent(new ActivityEvent("get category from database"));
			products = await GetProductsFromDbAsync(request, cancellationToken);
			if (products.Count != 0)
			{
				_categories.Add(request.Id, products);
			}
			else
			{
				_semaphores.Remove(request.Id, out _);
			}
		}
		activity?.AddEvent(new ActivityEvent("exit lock"));
		semaphore.Release(1);
		return products;
	}

	private async ValueTask<IReadOnlyCollection<Product>> GetProductsFromDbAsync(GetProductsOfCategoryQuery request, CancellationToken cancellationToken)
	{
		using var dbContext = _dbContextFactory.MakeDbContext();
		return await dbContext.Products
			.Where(x => x.CategoryName == request.Id)
			.ToArrayAsync(cancellationToken);
	}
}
