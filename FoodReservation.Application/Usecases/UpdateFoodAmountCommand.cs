using FoodReservation.Application.Repositories.Abstractions;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FoodReservation.Application;

public sealed class UpdateFoodAmountCommand : IRequest
{
    public required int Amount { get; init; }
    public required DateOnly Date { get; init; }
    public required Guid FoodId { get; init; }
}

public sealed class UpdateFoodAmountCommandHandler : IRequestHandler<UpdateFoodAmountCommand>
{
    private readonly IFoodReservationsDbContext _dbContext;

    public UpdateFoodAmountCommandHandler(IFoodReservationsDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async ValueTask<Unit> Handle(UpdateFoodAmountCommand request, CancellationToken cancellationToken)
    {
        var food = await _dbContext.DailyFoods
            .FirstAsync(x => x.FoodId == request.FoodId && x.Date == request.Date, cancellationToken: cancellationToken);
        food.Amount += request.Amount;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}