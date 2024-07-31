namespace BlackFriday.Infrastructure;

public sealed class Basket
{
    public string BasketId { get; set; }
    public bool IsCheckedOut { get; set; }
    public string ProductId { get; set; }
    public string UserId { get; set; }
}