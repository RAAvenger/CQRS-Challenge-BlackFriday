using FoodReservation.Application.Repositories.Abstractions;
using FoodReservation.Application.Usecases.Dtos;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FoodReservation.Application;

public sealed class GetReservedUserFoodsQuery : IRequest<IReadOnlyCollection<UserReservedFood>>
{
    public required Guid UserId { get; init; }
}

public sealed class GetReservedUserFoodsQueryHandler : IRequestHandler<GetReservedUserFoodsQuery, IReadOnlyCollection<UserReservedFood>>
{
    private readonly IFoodReservationsDbContext _dbContext;

    public GetReservedUserFoodsQueryHandler(IFoodReservationsDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async ValueTask<IReadOnlyCollection<UserReservedFood>> Handle(GetReservedUserFoodsQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.Reservations
            .Where(x => x.UserId == request.UserId)
            .Join(_dbContext.Foods, x => x.FoodId, y => y.Id, (x, y) => new UserReservedFood
            {
                FoodId = x.FoodId,
                FoodName = y.Name,
                Date = x.Date,
                Status = x.Status
            })
            .ToArrayAsync(cancellationToken: cancellationToken);
    }
}