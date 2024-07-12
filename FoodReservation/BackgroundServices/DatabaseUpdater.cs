using FoodReservation.Infrastructure.BackgroundServices.Logs;
using FoodReservation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FoodReservation.Infrastructure.BackgroundServices
{
    public class DatabaseUpdater : IHostedService
    {
        private readonly ILogger<DatabaseUpdater> _logger;
        private readonly IServiceProvider _services;

        public DatabaseUpdater(IServiceProvider services, ILogger<DatabaseUpdater> logger)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            try
            {
                DatabaseUpdaterLogs.StartMigration(_logger);
                var pendingMigrations = (await dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).ToArray();
                if (pendingMigrations.Length > 0)
                {
                    DatabaseUpdaterLogs.RunningPendingMigrations(_logger, pendingMigrations);
                    await dbContext.Database.MigrateAsync(cancellationToken: cancellationToken);
                }
                else
                {
                    DatabaseUpdaterLogs.NoPendingMigrations(_logger);
                }
                DatabaseUpdaterLogs.EndMigration(_logger);
            }
            catch (Exception ex)
            {
                DatabaseUpdaterLogs.MigrationFailed(_logger, ex);
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // do nothing
            return Task.CompletedTask;
        }
    }
}