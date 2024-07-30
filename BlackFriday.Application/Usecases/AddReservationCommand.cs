using BlackFriday.Application.Repositories.Abstractions;
using BlackFriday.Domain.Constants;
using Mediator;

namespace BlackFriday.Application;

public sealed class AddReservationCommand : IRequest
{
    public required DateOnly Date { get; init; }
    public required Guid FoodId { get; init; }
    public required Guid UserId { get; init; }
}

public sealed class AddReservationCommandHandler : IRequestHandler<AddReservationCommand>
{
    private readonly IBlackFridaysDbContext _dbContext;

    public AddReservationCommandHandler(IBlackFridaysDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async ValueTask<Unit> Handle(AddReservationCommand request, CancellationToken cancellationToken)
    {
        _dbContext.Reservations.Add(new Domain.Entities.BlackFriday
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