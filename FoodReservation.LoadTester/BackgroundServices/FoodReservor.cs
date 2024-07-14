using FoodReservation.Domain.Entities;
using FoodReservation.Infrastructure.Controllers.Dtos;
using FoodReservation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Json;

namespace FoodReservation.LoadTester.BackgroundServices
{
    internal class FoodReservor : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public FoodReservor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var users = await dbContext.Reservations.Select(x => x.UserId).Distinct().ToArrayAsync(stoppingToken);
            var reservationsPerDate = await dbContext.Reservations.GroupBy(x => x.Date).ToArrayAsync(stoppingToken);
            var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            var senders = new List<Task>();
            var index = -1;
            foreach (var i in Enumerable.Range(0, 10))
            {
                senders.Add(Task.Run(async () =>
                    {
                        using var httpClient = httpClientFactory.CreateClient("food_reservation");
                        while (!stoppingToken.IsCancellationRequested)
                        {
                            var currentIndex = Interlocked.Increment(ref index);
                            if (index >= reservationsPerDate.Length)
                            {
                                return;
                            }
                            var date = reservationsPerDate[currentIndex].Key;
                            var userIds = users.Where(x => !reservationsPerDate[currentIndex].Any(y => y.UserId == x)).ToArray();
                            foreach (var userId in userIds)
                            {
                                var dailyFoodsResponse = await httpClient.GetAsync($"foods/{date:o}");
                                var dailyFoods = await dailyFoodsResponse.Content.ReadFromJsonAsync<IReadOnlyCollection<ReservableDailyFood>>();
                                if (!dailyFoods.Any(x => x.Amount > 0))
                                {
                                    continue;
                                }
                                var deliverFoodRequest = new DeliverFoodToUserRequestDto
                                {
                                    Date = date,
                                    FoodId = dailyFoods.First(x => x.Amount > 0).FoodId,
                                    UserId = userId,
                                };

                                var content = JsonContent.Create(deliverFoodRequest);
                                var response = await httpClient.PostAsync($"foods/reserve", content);
                                await Task.Delay(TimeSpan.FromSeconds(1));
                            }
                        }
                    }, stoppingToken));
            }
            await Task.WhenAll(senders);
        }
    }
}