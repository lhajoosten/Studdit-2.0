using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Studdit.Persistence.Monitoring
{
    /// <summary>
    /// Database performance monitoring
    /// </summary>
    public class DatabasePerformanceMonitor
    {
        private readonly ILogger<DatabasePerformanceMonitor> _logger;

        public DatabasePerformanceMonitor(ILogger<DatabasePerformanceMonitor> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Log slow queries
        /// </summary>
        public void LogSlowQuery(string query, TimeSpan duration, object? parameters = null)
        {
            if (duration.TotalMilliseconds > 1000) // Log queries slower than 1 second
            {
                _logger.LogWarning("Slow query detected: {Query} took {Duration}ms with parameters {@Parameters}",
                    query, duration.TotalMilliseconds, parameters);
            }
        }

        /// <summary>
        /// Monitor database connection health
        /// </summary>
        public async Task<bool> IsHealthyAsync(DbContext context)
        {
            try
            {
                await context.Database.ExecuteSqlRawAsync("SELECT 1");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database health check failed");
                return false;
            }
        }
    }
}
