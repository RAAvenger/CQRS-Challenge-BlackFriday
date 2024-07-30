namespace BlackFriday.Domain.Entities
{
    public sealed class BlackFriday
    {
        public DateOnly Date { get; set; }

        public Guid FoodId { get; set; }

        public Guid UserId { get; set; }

        public string Status { get; set; }
    }
}