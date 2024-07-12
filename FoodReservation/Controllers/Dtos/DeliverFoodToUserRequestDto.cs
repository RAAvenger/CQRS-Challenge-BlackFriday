using System.Text.Json.Serialization;

namespace FoodReservation.Infrastructure.Controllers.Dtos
{
    public sealed class DeliverFoodToUserRequestDto
    {
        [JsonPropertyName("date")]
        public DateOnly Date { get; set; }

        [JsonPropertyName("food-id")]
        public Guid FoodId { get; set; }

        [JsonPropertyName("user-id")]
        public Guid UserId { get; set; }
    }
}