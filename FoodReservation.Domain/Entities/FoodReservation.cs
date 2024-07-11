namespace FoodReservation.Domain.Entities
{
    public sealed class FoodReservation
    {
        public DateOnly Date { get; set; }

        public Guid FoodId { get; set; }

        public Guid UserId { get; set; }

        public string Status { get; set; }
    }
}