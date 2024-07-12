using FoodReservation.Application.Repositories.Abstractions;
using FoodReservation.Domain.Constants;
using Mediator;

namespace FoodReservation.Application;

public sealed class AddReservationCommand : IRequest
{
    public required DateOnly Date { get; init; }
    public required Guid FoodId { get; init; }
    public required Guid UserId { get; init; }
}

public sealed class AddReservationCommandHandler : IRequestHandler<AddReservationCommand>
{
    private readonly IFoodReservationsDbContext _dbContext;

    public AddReservationCommandHandler(IFoodReservationsDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async ValueTask<Unit> Handle(AddReservationCommand request, CancellationToken cancellationToken)
    {
        _dbContext.Reservations.Add(new Domain.Entities.FoodReservation
        {
            UserId = request.UserId,
            Status = ReservationStatuses.NotDelivered,
            Date = request.Date,
            FoodId = request.FoodId,
        });
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}