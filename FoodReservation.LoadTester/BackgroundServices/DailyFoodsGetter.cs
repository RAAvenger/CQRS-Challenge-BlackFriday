using FoodReservation.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FoodReservation.LoadTester.BackgroundServices
{
    internal class DailyFoodsGetter : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public DailyFoodsGetter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            var users = new List<Task>();
            foreach (var i in Enumerable.Range(0, 5))
            {
                users.Add(Task.Run(async () =>
                    {
                        using var httpClient = httpClientFactory.CreateClient("food_reservation");
                        var baseDate = new DateTime(year: 2024, month: 1, day: 1);
                        while (!stoppingToken.IsCancellationRequested)
                        {
                            var randomDate = baseDate.AddDays(Random.Shared.Next(1, 365));
                            var response = await httpClient.GetAsync($"foods/{DateOnly.FromDateTime(randomDate):o}");
                            await Task.Delay(TimeSpan.FromSeconds(0.5));
                        }
                    }, stoppingToken));
            }
            await Task.WhenAll(users);
        }
    }
}