using Microsoft.EntityFrameworkCore;

namespace Studdit.Persistence.Helpers
{
    /// <summary>
    /// Migration helper for programmatic migrations
    /// </summary>
    public static class MigrationHelper
    {
        /// <summary>
        /// Apply migrations programmatically
        /// </summary>
        public static async Task ApplyMigrationsAsync(DbContext context)
        {
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                await context.Database.MigrateAsync();
            }
        }

        /// <summary>
        /// Check if database exists
        /// </summary>
        public static async Task<bool> DatabaseExistsAsync(DbContext context)
        {
            try
            {
                return await context.Database.CanConnectAsync();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get applied migrations
        /// </summary>
        public static async Task<IEnumerable<string>> GetAppliedMigrationsAsync(DbContext context)
        {
            return await context.Database.GetAppliedMigrationsAsync();
        }

        /// <summary>
        /// Get pending migrations
        /// </summary>
        public static async Task<IEnumerable<string>> GetPendingMigrationsAsync(DbContext context)
        {
            return await context.Database.GetPendingMigrationsAsync();
        }
    }
}
