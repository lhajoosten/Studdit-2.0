using Microsoft.EntityFrameworkCore;
using Studdit.Domain.Common;

namespace Studdit.Persistence.Extensions
{
    /// <summary>
    /// Extensions for query optimization
    /// </summary>
    public static class QueryOptimizationExtensions
    {
        /// <summary>
        /// Apply standard optimizations for read-only queries
        /// </summary>
        public static IQueryable<T> AsOptimizedQuery<T>(this IQueryable<T> query)
            where T : class
        {
            return query.AsNoTracking().AsSplitQuery();
        }

        /// <summary>
        /// Apply pagination with consistent ordering
        /// </summary>
        public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, int pageNumber, int pageSize)
            where T : BaseEntity
        {
            return query
                .OrderByDescending(e => e.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        /// <summary>
        /// Include standard audit information
        /// </summary>
        public static IQueryable<T> IncludeAuditInfo<T>(this IQueryable<T> query)
            where T : BaseEntity
        {
            // This would include audit-related navigation properties if they existed
            return query;
        }
    }
}
