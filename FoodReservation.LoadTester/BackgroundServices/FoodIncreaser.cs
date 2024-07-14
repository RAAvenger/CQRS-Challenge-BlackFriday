using FoodReservation.Infrastructure.Controllers.Dtos;
using FoodReservation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Json;

namespace FoodReservation.LoadTester.BackgroundServices
{
    internal class FoodIncreaser : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public FoodIncreaser(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var dailyFoods = await dbContext.DailyFoods.ToArrayAsync(stoppingToken);
            var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            var users = new List<Task>();
            var index = -1;
            foreach (var i in Enumerable.Range(0, 10))
            {
                users.Add(Task.Run(async () =>
                    {
                        using var httpClient = httpClientFactory.CreateClient("food_reservation");
                        while (!stoppingToken.IsCancellationRequested)
                        {
                            var currentIndex = Interlocked.Increment(ref index);
                            if (index >= dailyFoods.Length)
                            {
                                return;
                            }
                            var dailyFood = dailyFoods[currentIndex];
                            var increaseReservableFoodAmountRequest = new IncreaseReservableFoodAmountRequestDto
                            {
                                Date = dailyFood.Date,
                                FoodId = dailyFood.FoodId,
                                Amount = 1
                            };

                            var content = JsonContent.Create(increaseReservableFoodAmountRequest);
                            var response = await httpClient.PostAsync($"foods/increase-amount", content);
                            await Task.Delay(TimeSpan.FromSeconds(1));
                        }
                    }, stoppingToken));
            }
            await Task.WhenAll(users);
        }
    }
}