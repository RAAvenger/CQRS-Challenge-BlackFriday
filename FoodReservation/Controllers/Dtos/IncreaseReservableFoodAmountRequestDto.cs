using System.Text.Json.Serialization;

namespace FoodReservation.Infrastructure.Controllers.Dtos
{
    public sealed class IncreaseReservableFoodAmountRequestDto
    {
        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("date")]
        public DateOnly Date { get; set; }

        [JsonPropertyName("food-id")]
        public Guid FoodId { get; set; }
    }
}