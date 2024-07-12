using FoodReservation.Infrastructure.Controllers.Dtos;
using FoodReservation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Json;

namespace FoodReservation.LoadTester.BackgroundServices
{
    internal class UsersFoodsDeliverer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public UsersFoodsDeliverer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var reservations = await dbContext.Reservations.ToArrayAsync(stoppingToken);
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
                            if (index >= reservations.Length)
                            {
                                return;
                            }
                            var currentIndex = Interlocked.Increment(ref index);
                            var reservation = reservations[currentIndex];
                            var deliverFoodRequest = new DeliverFoodToUserRequestDto
                            {
                                Date = reservation.Date,
                                FoodId = reservation.FoodId,
                                UserId = reservation.UserId,
                            };

                            var content = JsonContent.Create(deliverFoodRequest);
                            var response = await httpClient.PostAsync($"foods/deliver", content);
                            await Task.Delay(TimeSpan.FromSeconds(1));
                        }
                    }, stoppingToken));
            }
            await Task.WhenAll(users);
        }
    }
}