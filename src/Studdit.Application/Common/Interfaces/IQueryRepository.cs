using Studdit.Domain.Common;
using System.Linq.Expressions;

namespace Studdit.Application.Common.Interfaces
{
    /// <summary>
    /// Generic repository interface for read-only operations, following CQRS pattern
    /// All operations are asynchronous to improve application scalability
    /// </summary>
    /// <typeparam name="T">The domain entity type</typeparam>
    /// <typeparam name="TKey">The type of entity's primary key</typeparam>
    public interface IQueryRepository<T, TKey> where T : BaseEntity<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Gets an entity by its ID asynchronously
        /// </summary>
        /// <param name="id">The entity's ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="includeProperties">Optional navigation properties to include</param>
        /// <returns>The entity if found, null otherwise</returns>
        Task<T> GetByIdAsync(TKey id, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includeProperties);

        /// <summary>
        /// Gets a queryable collection of entities
        /// </summary>
        /// <remarks>
        /// Use this method carefully as it returns an IQueryable that could lead to lazy loading.
        /// Prefer to use the more specific async methods whenever possible.
        /// </remarks>
        /// <returns>Queryable collection for further filtering</returns>
        IQueryable<T> AsQueryable();

        /// <summary>
        /// Finds entities based on a predicate asynchronously
        /// </summary>
        /// <param name="predicate">The filter expression</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="includeProperties">Optional navigation properties to include</param>
        /// <returns>Collection of entities matching the criteria</returns>
        Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includeProperties);

        /// <summary>
        /// Checks if any entity matches the given predicate asynchronously
        /// </summary>
        /// <param name="predicate">The filter expression</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if matching entity exists, false otherwise</returns>
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all entities asynchronously
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="includeProperties">Optional navigation properties to include</param>
        /// <returns>All entities of type T</returns>
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includeProperties);

        /// <summary>
        /// Counts entities that match a predicate asynchronously
        /// </summary>
        /// <param name="predicate">The filter expression</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Count of entities matching the criteria</returns>
        Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a single entity matching a predicate asynchronously
        /// </summary>
        /// <param name="predicate">The filter expression</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="includeProperties">Optional navigation properties to include</param>
        /// <returns>The entity if found, null otherwise</returns>
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includeProperties);

        /// <summary>
        /// Gets a single entity matching a predicate asynchronously, throws if not found
        /// </summary>
        /// <param name="predicate">The filter expression</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="includeProperties">Optional navigation properties to include</param>
        /// <returns>The entity if found</returns>
        /// <exception cref="System.InvalidOperationException">If no entity matches the predicate</exception>
        Task<T> FirstAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includeProperties);
    }

    /// <summary>
    /// Convenience interface for entities with int keys
    /// </summary>
    /// <typeparam name="T">The domain entity type</typeparam>
    public interface IQueryRepository<T> : IQueryRepository<T, int> where T : BaseEntity<int>
    {
    }
}