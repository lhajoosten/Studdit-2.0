using Microsoft.EntityFrameworkCore;
using Studdit.Application.Common.Interfaces;
using Studdit.Domain.Common;
using Studdit.Persistence.Context;

namespace Studdit.Persistence.Repositories
{
    /// <summary>
    /// Generic repository implementation for write operations
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <typeparam name="TKey">Primary key type</typeparam>
    public class CommandRepository<T, TKey> : ICommandRepository<T, TKey>
        where T : BaseEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public CommandRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T> AddAsync(T entity, int userId, CancellationToken cancellationToken = default)
        {
            var entityEntry = await _dbSet.AddAsync(entity, cancellationToken);
            return entityEntry.Entity;
        }

        public virtual Task UpdateAsync(T entity, int userId, CancellationToken cancellationToken = default)
        {
            _context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        public virtual async Task<bool> DeleteByIdAsync(TKey id, CancellationToken cancellationToken = default)
        {
            var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
            if (entity == null)
            {
                return false;
            }

            _dbSet.Remove(entity);
            return true;
        }

        public virtual Task SoftDeleteAsync(T entity, int userId, CancellationToken cancellationToken = default)
        {
            // If your BaseEntity had an IsDeleted property, you could implement soft delete here
            // For now, we'll just mark as modified to trigger audit fields
            _context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Convenience implementation for entities with int keys
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public class CommandRepository<T> : CommandRepository<T, int>, ICommandRepository<T>
        where T : BaseEntity<int>
    {
        public CommandRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
