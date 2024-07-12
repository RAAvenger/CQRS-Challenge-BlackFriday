// See https://aka.ms/new-console-template for more information
using Bogus;
using FoodReservation.DataGenerator;
using FoodReservation.Domain.Constants;
using FoodReservation.Domain.Entities;
using FoodReservation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var foods = ConstantValue.PersianFoods
    .Select(x => new ReservableFood { Name = x, Id = Guid.NewGuid() })
    .ToArray();
var currentDate = new DateTime(year: 2024, month: 1, day: 1);
var endDate = new DateTime(year: 2025, month: 1, day: 1);
var usersCount = 500;

var reservableDailyFoods = GetReservableDailyFoods(foods, currentDate, endDate);
var reservations = GetReservations(reservableDailyFoods, usersCount);

using var dbContext = new ApplicationDbContext(new DbContextOptionsBuilder()
    .UseNpgsql("Host=localhost;Database=food_db;Username=postgres;Password=!@#123qwe")
    .Options);
await dbContext.Database.CanConnectAsync();
await dbContext.Database.OpenConnectionAsync();
dbContext.Foods.AddRange(foods);
dbContext.DailyFoods.AddRange(reservableDailyFoods);
dbContext.Reservations.AddRange(reservations);
await dbContext.SaveChangesAsync();

static List<ReservableDailyFood> GetReservableDailyFoods(IEnumerable<ReservableFood> foods, DateTime currentDate, DateTime endDate)
{
    var reservableDailyFoodFaker = new Faker<ReservableDailyFood>()
        .RuleFor(x => x.Amount, x => x.Random.Number(50, 250))
        .RuleFor(x => x.FoodId, x => x.PickRandom(foods).Id);
    var reservableDailyFoods = new List<ReservableDailyFood>();
    while (currentDate < endDate)
    {
        currentDate = currentDate.AddDays(1);
        if (currentDate.DayOfWeek is DayOfWeek.Friday)
        {
            continue;
        }

        var dailyFoods = reservableDailyFoodFaker.GenerateBetween(3, 5);
        while (dailyFoods.GroupBy(x => new { x.FoodId }).Any(x => x.Count() > 1))
        {
            dailyFoods = reservableDailyFoodFaker.GenerateBetween(3, 5);
        }
        foreach (var dailyFood in dailyFoods)
        {
            dailyFood.Date = DateOnly.FromDateTime(currentDate);
        }
        reservableDailyFoods.AddRange(dailyFoods);
    }

    return reservableDailyFoods;
}

static List<FoodReservation.Domain.Entities.FoodReservation> GetReservations(List<ReservableDailyFood> reservableDailyFoods, int usersCount)
{
    var userIds = Enumerable.Range(0, usersCount).Select(x => Guid.NewGuid()).ToArray();
    var reservableFoodsByDate = reservableDailyFoods.GroupBy(x => x.Date);
    var reservations = new List<FoodReservation.Domain.Entities.FoodReservation>();
    foreach (var reservableDailyFoodsGroupe in reservableFoodsByDate)
    {
        var foodCount = new Dictionary<Guid, int>();
        var foodsCount = reservableDailyFoodsGroupe.Count();
        var foodsGroup = reservableDailyFoodsGroupe.ToArray();
        foreach (var userId in userIds)
        {
            var chois = Random.Shared.Next(0, foodsCount * 3);
            if (chois >= foodsCount)
            {
                continue;
            }
            
            if (!foodCount.TryGetValue(foodsGroup[chois].FoodId, out var count))
            {
                foodCount.Add(foodsGroup[chois].FoodId, foodsGroup[chois].Amount);
                count = foodsGroup[chois].Amount;
            }
            if (count == 0)
            {
                continue;
            }

            reservations.Add(new FoodReservation.Domain.Entities.FoodReservation
            {
                Date = reservableDailyFoodsGroupe.Key,
                FoodId = foodsGroup[chois].FoodId,
                Status = ReservationStatuses.NotDelivered,
                UserId = userId,
            });
        }
    }
    return reservations;
}