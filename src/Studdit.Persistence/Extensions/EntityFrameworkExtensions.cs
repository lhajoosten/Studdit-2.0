using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Studdit.Persistence.Extensions
{
    /// <summary>
    /// Extension methods for Entity Framework operations
    /// </summary>
    public static class EntityFrameworkExtensions
    {
        /// <summary>
        /// Bulk insert entities for better performance
        /// </summary>
        public static async Task BulkInsertAsync<T>(this DbSet<T> dbSet, IEnumerable<T> entities, CancellationToken cancellationToken = default)
            where T : class
        {
            await dbSet.AddRangeAsync(entities, cancellationToken);
        }

        /// <summary>
        /// Get entities that have been modified
        /// </summary>
        public static IEnumerable<EntityEntry<T>> GetModifiedEntries<T>(this ChangeTracker changeTracker)
            where T : class
        {
            return changeTracker.Entries<T>().Where(e => e.State == EntityState.Modified);
        }

        /// <summary>
        /// Get entities that have been added
        /// </summary>
        public static IEnumerable<EntityEntry<T>> GetAddedEntries<T>(this ChangeTracker changeTracker)
            where T : class
        {
            return changeTracker.Entries<T>().Where(e => e.State == EntityState.Added);
        }

        /// <summary>
        /// Detach all entities to prevent tracking issues
        /// </summary>
        public static void DetachAllEntities(this DbContext context)
        {
            var changedEntriesCopy = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                           e.State == EntityState.Modified ||
                           e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in changedEntriesCopy)
                entry.State = EntityState.Detached;
        }
    }
}
