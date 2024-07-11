namespace FoodReservation.Application.Usecases.Dtos
{
    public sealed class ReservableDailyFood
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int RemainingAmount { get; set; }
    }
}