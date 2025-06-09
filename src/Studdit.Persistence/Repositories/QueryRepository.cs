using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Studdit.Application.Common.Interfaces;
using Studdit.Application.Common.Specifications;
using Studdit.Domain.Common;
using Studdit.Persistence.Context;
using System.Linq.Expressions;

namespace Studdit.Persistence.Repositories
{
    /// <summary>
    /// Generic repository implementation for read operations with specification support
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <typeparam name="TKey">Primary key type</typeparam>
    public class QueryRepository<T, TKey> : IQueryRepository<T, TKey>
        where T : BaseEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public QueryRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = _dbSet.AsQueryable();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
        }

        public virtual IQueryable<T> AsQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public virtual async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = _dbSet.Where(predicate);

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(predicate, cancellationToken);
        }

        public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = _dbSet.AsQueryable();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.CountAsync(predicate, cancellationToken);
        }

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = _dbSet.Where(predicate);

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<T> FirstAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = _dbSet.Where(predicate);

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.FirstAsync(cancellationToken);
        }

        /// <summary>
        /// Find entities using a specification
        /// </summary>
        public virtual async Task<IReadOnlyList<T>> FindAsync(BaseEntitySpecification<T> specification, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(specification).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Count entities using a specification
        /// </summary>
        public virtual async Task<int> CountAsync(BaseEntitySpecification<T> specification, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(specification).CountAsync(cancellationToken);
        }

        /// <summary>
        /// Check if any entities match the specification
        /// </summary>
        public virtual async Task<bool> AnyAsync(BaseEntitySpecification<T> specification, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(specification).AnyAsync(cancellationToken);
        }

        /// <summary>
        /// Get first entity matching the specification
        /// </summary>
        public virtual async Task<T?> FirstOrDefaultAsync(BaseEntitySpecification<T> specification, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Apply specification to the query
        /// </summary>
        protected virtual IQueryable<T> ApplySpecification(BaseEntitySpecification<T> specification)
        {
            return SpecificationEvaluator.Default.GetQuery(_dbSet.AsQueryable(), specification);
        }
    }

    /// <summary>
    /// Convenience implementation for entities with int keys
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public class QueryRepository<T> : QueryRepository<T, int>, IQueryRepository<T>
        where T : BaseEntity<int>
    {
        public QueryRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
