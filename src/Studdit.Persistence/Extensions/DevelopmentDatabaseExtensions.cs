using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Studdit.Persistence.Context;

namespace Studdit.Persistence.Extensions
{
    /// <summary>
    /// Development-specific database extensions
    /// </summary>
    public static class DevelopmentDatabaseExtensions
    {
        public static async Task ResetDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

            logger.LogWarning("Resetting database - this will delete all data!");

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            logger.LogInformation("Database reset completed.");

            await serviceProvider.InitializeDatabaseAsync();
        }

        public static async Task<bool> IsDatabaseHealthyAsync(this IServiceProvider serviceProvider)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                return await context.CanConnectAsync() &&
                       await context.Database.CanConnectAsync();
            }
            catch
            {
                return false;
            }
        }
    }
}
