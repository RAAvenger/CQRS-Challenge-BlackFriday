using FoodReservation.Application;
using FoodReservation.Application.Usecases.Dtos;
using FoodReservation.Domain.Constants;
using FoodReservation.Infrastructure.Controllers.Dtos;
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

        [HttpPost("deliver")]
        public async Task<ActionResult> DeliverFoodToUser([FromBody] DeliverFoodToUserRequestDto request,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(new UpdateReservationStatusCommand
            {
                FoodId = request.FoodId,
                Date = request.Date,
                Status = ReservationStatuses.Delivered,
                UserId = request.UserId
            }, cancellationToken);
            return Ok();
        }

        [HttpGet("{date}")]
        public async Task<ActionResult<IReadOnlyCollection<ReservableDailyFood>>> GetAllFoodsForDate([FromRoute] DateOnly date,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetRemainingFoodsForGivenDateQuery { Date = date }, cancellationToken);
            return Ok(result);
        }

        [HttpGet("users/{userId}")]
        public async Task<ActionResult<IReadOnlyCollection<UserReservedFood>>> GetUserFoods([FromRoute] Guid userId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetReservedUserFoodsQuery { UserId = userId }, cancellationToken);
            return Ok(result);
        }

        [HttpPost("increase-amount")]
        public async Task<ActionResult> IncreaseReservableFoodAmount([FromBody] IncreaseReservableFoodAmountRequestDto request,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(new UpdateFoodAmountCommand
            {
                FoodId = request.FoodId,
                Amount = request.Amount,
                Date = request.Date
            }, cancellationToken);
            return Ok();
        }

        [HttpPost("reserve")]
        public async Task<ActionResult> ReserveFoodForUser([FromBody] ReserveFoodRequestDto request,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(new AddReservationCommand
            {
                FoodId = request.FoodId,
                Date = request.Date,
                UserId = request.UserId
            }, cancellationToken);
            return Ok();
        }
    }
}