using FoodReservation.Application;
using FoodReservation.Application.Usecases.Dtos;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace FoodReservation.Infrastructure.Controllers
{
    [ApiController]
    [Route("Foods")]
    public class FoodReservationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FoodReservationController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("{date}")]
        public async Task<ActionResult<IReadOnlyCollection<ReservableDailyFood>>> GetAllFoodsForDate([FromRoute] DateOnly date,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetRemainingFoodsForGivenDateQuery { Date = date }, cancellationToken);
            return Ok(result);
        }

        [HttpGet("/users/{userId}")]
        public async Task<ActionResult<IReadOnlyCollection<UserReservedFood>>> GetUserFoods([FromRoute] Guid userId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetReservedUserFoodsQuery { UserId = userId }, cancellationToken);
            return Ok(result);
        }
    }
}