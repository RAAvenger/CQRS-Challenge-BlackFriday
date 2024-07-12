using FoodReservation.Application;
using FoodReservation.Application.Usecases.Dtos;
using FoodReservation.Domain.Constants;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace FoodReservation.Infrastructure.Controllers
{
    [ApiController]
    [Route("foods")]
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

        [HttpPost("/increase-amount")]
        public async Task<ActionResult> IncreaseReservableFoodAmount([FromBody] Guid foodId,
            [FromBody] DateOnly date,
            [FromBody] int amount,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(new UpdateFoodAmountCommand
            {
                FoodId = foodId,
                Amount = amount,
                Date = date
            }, cancellationToken);
            return Ok();
        }

        [HttpPost("/deliver")]
        public async Task<ActionResult> IncreaseReservableFoodAmount([FromBody] Guid foodId,
            [FromBody] Guid userId,
            [FromBody] DateOnly date,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(new UpdateReservationStatusCommand
            {
                FoodId = foodId,
                Date = date,
                Status = ReservationStatuses.Delivered,
                UserId = userId
            }, cancellationToken);
            return Ok();
        }

        [HttpPost("/reserve")]
        public async Task<ActionResult> ReserveFoodForUser([FromBody] Guid foodId,
            [FromBody] Guid userId,
            [FromBody] DateOnly date,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(new AddReservationCommand
            {
                FoodId = foodId,
                Date = date,
                UserId = userId
            }, cancellationToken);
            return Ok();
        }
    }
}