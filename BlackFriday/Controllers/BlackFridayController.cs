using BlackFriday.Application.Repositories.Abstractions;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlackFriday.Infrastructure.Controllers
{
    [ApiController]
    public class BlackFridayController : ControllerBase
    {
        private readonly IBlackFridaysDbContext _dbContext;
        private readonly IMediator _mediator;

        public BlackFridayController(IMediator mediator, IBlackFridaysDbContext dbContext)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IReadOnlyCollection<string>>> GetAllCategories(CancellationToken cancellationToken)
        {
            var categories = await _dbContext.Products
                .GroupBy(x => x.CategoryName)
                .Select(x => x.Key)
                .ToArrayAsync(cancellationToken);

            return Ok(categories);
        }

        [HttpGet("items/{id}")]
        public async Task<ActionResult<IReadOnlyCollection<Product>>> GetItemById([FromRoute] string id, CancellationToken cancellationToken)
        {
            var item = await _dbContext.Products
                .FirstOrDefaultAsync(x => x.Asin == id, cancellationToken);
            if (item is null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpGet("categories/{category}")]
        public async Task<ActionResult<IReadOnlyCollection<Product>>> GetItemsForGivenCategory([FromRoute] string category, CancellationToken cancellationToken)
        {
            var categoryItems = await _dbContext.Products
                .Where(x => x.CategoryName == category)
                .ToArrayAsync(cancellationToken);
            if (categoryItems.Length == 0)
            {
                return NotFound();
            }
            return Ok(categoryItems);
        }

        //[HttpPost("deliver")]
        //public async Task<ActionResult> DeliverFoodToUser([FromBody] DeliverFoodToUserRequestDto request,
        //    CancellationToken cancellationToken)
        //{
        //await _mediator.Send(new UpdateReservationStatusCommand
        //{
        //    FoodId = request.FoodId,
        //    Date = request.Date,
        //    Status = ReservationStatuses.Delivered,
        //    UserId = request.UserId
        //}, cancellationToken);
        //    return Ok();
        //}

        //[HttpGet("{date}")]
        //public async Task<ActionResult<IReadOnlyCollection<ReservableDailyFood>>> GetAllFoodsForDate([FromRoute] DateOnly date,
        //    CancellationToken cancellationToken)
        //{
        //    var result = await _mediator.Send(new GetRemainingFoodsForGivenDateQuery { Date = date }, cancellationToken);
        //    return Ok(result);
        //}

        //[HttpGet("users/{userId}")]
        //public async Task<ActionResult<IReadOnlyCollection<UserReservedFood>>> GetUserFoods([FromRoute] Guid userId,
        //    CancellationToken cancellationToken)
        //{
        //    var result = await _mediator.Send(new GetReservedUserFoodsQuery { UserId = userId }, cancellationToken);
        //    return Ok(result);
        //}

        //[HttpPost("increase-amount")]
        //public async Task<ActionResult> IncreaseReservableFoodAmount([FromBody] IncreaseReservableFoodAmountRequestDto request,
        //    CancellationToken cancellationToken)
        //{
        //    await _mediator.Send(new UpdateFoodAmountCommand
        //    {
        //        FoodId = request.FoodId,
        //        Amount = request.Amount,
        //        Date = request.Date
        //    }, cancellationToken);
        //    return Ok();
        //}

        //[HttpPost("reserve")]
        //public async Task<ActionResult> ReserveFoodForUser([FromBody] ReserveFoodRequestDto request,
        //    CancellationToken cancellationToken)
        //{
        //    await _mediator.Send(new AddReservationCommand
        //    {
        //        FoodId = request.FoodId,
        //        Date = request.Date,
        //        UserId = request.UserId
        //    }, cancellationToken);
        //    return Ok();
        //}
    }
}