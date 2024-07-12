using FoodReservation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FoodReservation.LoadTester.BackgroundServices
{
    internal class UsersFoodsGetter : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public UsersFoodsGetter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userIds = await dbContext.Reservations.Select(r => r.UserId).ToArrayAsync(stoppingToken);
            var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            var users = new List<Task>();
            foreach (var i in Enumerable.Range(0, 5))
            {
                users.Add(Task.Run(async () =>
                    {
                        using var httpClient = httpClientFactory.CreateClient("food_reservation");
                        while (!stoppingToken.IsCancellationRequested)
                        {
                            var userId = userIds[Random.Shared.Next(0, userIds.Length)];
                            var response = await httpClient.GetAsync($"foods/users/{userId}");
                            await Task.Delay(TimeSpan.FromSeconds(0.5));
                        }
                    }, stoppingToken));
            }
            await Task.WhenAll(users);
        }
    }
}