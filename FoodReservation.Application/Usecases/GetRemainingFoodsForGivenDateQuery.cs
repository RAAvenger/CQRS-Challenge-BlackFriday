using FoodReservation.Application.Repositories.Abstractions;
using FoodReservation.Application.Usecases.Dtos;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FoodReservation.Application;

public sealed class GetRemainingFoodsForGivenDateQuery : IRequest<IReadOnlyCollection<ReservableDailyFood>>
{
    public required DateOnly Date { get; init; }
}

public sealed class GetRemainingFoodsForGivenDateQueryHandler : IRequestHandler<GetRemainingFoodsForGivenDateQuery, IReadOnlyCollection<ReservableDailyFood>>
{
    private readonly IFoodReservationsDbContext _dbContext;

    public GetRemainingFoodsForGivenDateQueryHandler(IFoodReservationsDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async ValueTask<IReadOnlyCollection<ReservableDailyFood>> Handle(GetRemainingFoodsForGivenDateQuery request, CancellationToken cancellationToken)
    {
        var reservations = _dbContext.Reservations
            .Where(x => x.Date == request.Date)
            .GroupBy(x => x.FoodId).Select(x => new { FoodId = x.Key, ReservationsCount = x.Count() });
        return await _dbContext.DailyFoods
            .Where(x => x.Date == request.Date)
            .Join(_dbContext.Foods, x => x.FoodId, y => y.Id, (x, y) => new
            {
                Food = y,
                Amount = x.Amount
            })
            .Join(reservations, x => x.Food.Id, y => y.FoodId, (x, y) => new ReservableDailyFood
            {
                Id = x.Food.Id,
                Name = x.Food.Name,
                RemainingAmount = x.Amount - y.ReservationsCount
            })
            .ToArrayAsync(cancellationToken: cancellationToken);
    }
}