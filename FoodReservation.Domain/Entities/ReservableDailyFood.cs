namespace FoodReservation.Domain.Entities
{
    public sealed class ReservableDailyFood
    {
        public int Amount { get; set; }
        public DateOnly Date { get; set; }
        public Guid FoodId { get; set; }
    }
}