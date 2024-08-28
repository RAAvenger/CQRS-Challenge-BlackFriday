using System.Diagnostics;
using BlackFriday.Application.Commons.Tracing;
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
	private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
	private IReadOnlyCollection<string>? _categories = null;

	public GetAllCategoryNamesQueryHandler(IBlackFridayDbContextFactory dbContextFactory)
	{
		_dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
	}

	public async ValueTask<IReadOnlyCollection<string>> Handle(GetAllCategoryNamesQuery request, CancellationToken cancellationToken)
	{
		using var activity = ApplicationActivityProvider.ActivitySource.StartActivity(nameof(GetAllCategoryNamesQuery));
		if (_categories is not null)
		{
			return _categories;
		}

		activity?.AddEvent(new ActivityEvent("wait for lock"));
		await _semaphore.WaitAsync(cancellationToken);
		try
		{
			activity?.AddEvent(new ActivityEvent("enter lock"));
			if (_categories is null)
			{
				activity?.AddEvent(new ActivityEvent("propagate categories"));
				_categories = await GetCategories(cancellationToken);
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
			_semaphore.Release(1);
		}

		return _categories;
	}

	private async Task<string[]> GetCategories(CancellationToken cancellationToken)
	{
		using var dbContext = _dbContextFactory.MakeDbContext();
		return await dbContext.Products
			.GroupBy(x => x.CategoryName)
			.Select(x => x.Key)
			.ToArrayAsync(cancellationToken);
	}
}
