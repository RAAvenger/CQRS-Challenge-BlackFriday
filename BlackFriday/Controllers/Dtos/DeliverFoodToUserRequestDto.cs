using System.Text.Json.Serialization;

namespace BlackFriday.Infrastructure.Controllers.Dtos
{
    public sealed record DeliverFoodToUserRequestDto
    {
        [JsonPropertyName("date")]
        public DateOnly Date { get; set; }

        [JsonPropertyName("food-id")]
        public Guid FoodId { get; set; }

        [JsonPropertyName("user-id")]
        public Guid UserId { get; set; }
    }
}