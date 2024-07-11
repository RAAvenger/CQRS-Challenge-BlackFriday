namespace FoodReservation.Domain.Entities
{
    public sealed class FoodReservation
    {
        public Guid FoodId { get; set; }

        public Guid UserId { get; set; }
    }
}