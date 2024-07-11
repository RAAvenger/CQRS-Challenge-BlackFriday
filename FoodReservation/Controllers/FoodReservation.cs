using FoodReservation.Domain;
using Microsoft.AspNetCore.Mvc;

namespace FoodReservation.Infrastructure.Controllers
{
    [ApiController]
    [Route("Foods")]
    public class FoodReservation : ControllerBase
    {
        [HttpGet()]
        public ActionResult<IReadOnlyCollection<ReservableFood>> GetAllFoodsForDate([FromRoute] DateOnly date)
        {
        }
    }
}