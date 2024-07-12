namespace FoodReservation.Infrastructure.BackgroundServices.Logs
{
    public static partial class DatabaseUpdaterLogs
    {
        [LoggerMessage(EventId = 1,
            Level = LogLevel.Information,
            Message = "Database migration ended")]
        public static partial void EndMigration(ILogger logger);

        [LoggerMessage(EventId = 1,
            Level = LogLevel.Error,
            Message = "Database migration failed with error {exception}")]
        public static partial void MigrationFailed(ILogger logger, Exception exception);

        [LoggerMessage(EventId = 1,
            Level = LogLevel.Information,
            Message = "There is no pending migrations")]
        public static partial void NoPendingMigrations(ILogger logger);

        [LoggerMessage(EventId = 1,
            Level = LogLevel.Information,
            Message = "Running pending migrations {pendingMigrations}")]
        public static partial void RunningPendingMigrations(ILogger logger, IReadOnlyCollection<string> pendingMigrations);

        [LoggerMessage(EventId = 1,
            Level = LogLevel.Information,
            Message = "Database migration started")]
        public static partial void StartMigration(ILogger logger);
    }
}