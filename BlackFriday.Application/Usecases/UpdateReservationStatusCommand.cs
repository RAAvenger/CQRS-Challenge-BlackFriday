using BlackFriday.Application.Repositories.Abstractions;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace BlackFriday.Application;

public sealed class UpdateReservationStatusCommand : IRequest
{
    public required DateOnly Date { get; init; }
    public required Guid FoodId { get; init; }
    public required string Status { get; init; }
    public required Guid UserId { get; init; }
}

public sealed class UpdateReservationStatusCommandHandler : IRequestHandler<UpdateReservationStatusCommand>
{
    private readonly IBlackFridaysDbContext _dbContext;

    public UpdateReservationStatusCommandHandler(IBlackFridaysDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async ValueTask<Unit> Handle(UpdateReservationStatusCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _dbContext.Reservations
            .FirstAsync(x => x.FoodId == request.FoodId && x.Date == request.Date && x.UserId == request.UserId, cancellationToken: cancellationToken);
        reservation.Status = request.Status;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}