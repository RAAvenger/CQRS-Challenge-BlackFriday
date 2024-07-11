namespace FoodReservation.Application.Usecases.Dtos
{
    public sealed class UserReservedFood
    {
        public DateOnly Date { get; init; }

        public Guid FoodId { get; init; }

        public string FoodName { get; init; }

        public string Status { get; init; }
    }
}