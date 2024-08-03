using System.Text.Json.Serialization;

namespace BlackFriday.Infrastructure.Controllers.Dtos
{
    public sealed record AddItemToBasketRequestDto
    {
        [JsonPropertyName("product-id")]
        public string ProductId { get; set; }

        [JsonPropertyName("user-id")]
        public string UserId { get; set; }

        [JsonPropertyName("basket-id")]
        public string BasketId { get; set; }
    }
}
