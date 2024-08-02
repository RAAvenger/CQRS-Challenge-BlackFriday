namespace BlackFriday.Infrastructure;

public sealed class Invoice
{
    public string BasketId { get; set; }
    public string Items { get; set; }
    public string UserId { get; set; }
}