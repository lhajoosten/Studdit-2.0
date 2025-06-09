using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Studdit.Application.Common.Interfaces;
using Studdit.Application.Common.Models;
using Studdit.Application.Common.Specifications;
using Studdit.Domain.Common;
using Studdit.Persistence.Context;
using System.Linq.Expressions;

namespace Studdit.Persistence.Repositories
{
    /// <summary>
    /// Enhanced generic repository implementation for read operations with AutoMapper projection support
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <typeparam name="TKey">Primary key type</typeparam>
    public class QueryRepository<T, TKey> : IQueryRepository<T, TKey>
        where T : BaseEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;
        protected readonly IMapper _mapper;

        public QueryRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _dbSet = context.Set<T>();
            _mapper = mapper;
        }

        #region Entity-based Methods (for when you need full entities)

        public virtual async Task<T> GetByIdAsync(TKey id, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includeProperties)
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

        public virtual async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includeProperties)
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

        #endregion

        #region Specification-based Methods (for entities)

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
        public virtual async Task<T> FirstOrDefaultAsync(BaseEntitySpecification<T> specification, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
        }

        #endregion

        #region AutoMapper Projection Methods (for DTOs - PERFORMANCE OPTIMIZED)

        /// <summary>
        /// Project entity to DTO by ID using AutoMapper - PERFORMANCE OPTIMIZED
        /// </summary>
        public virtual async Task<TDto?> GetByIdProjectedAsync<TDto>(TKey id, CancellationToken cancellationToken = default)
            where TDto : class
        {
            return await _dbSet
                .Where(e => e.Id.Equals(id))
                .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Project entities to DTOs using predicate - PERFORMANCE OPTIMIZED
        /// </summary>
        public virtual async Task<IReadOnlyList<TDto>> FindProjectedAsync<TDto>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            where TDto : class
        {
            return await _dbSet
                .Where(predicate)
                .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Project all entities to DTOs - PERFORMANCE OPTIMIZED
        /// </summary>
        public virtual async Task<IReadOnlyList<TDto>> GetAllProjectedAsync<TDto>(CancellationToken cancellationToken = default)
            where TDto : class
        {
            return await _dbSet
                .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Project first entity to DTO using predicate - PERFORMANCE OPTIMIZED
        /// </summary>
        public virtual async Task<TDto?> FirstOrDefaultProjectedAsync<TDto>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            where TDto : class
        {
            return await _dbSet
                .Where(predicate)
                .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Project entities to DTOs using specification - PERFORMANCE OPTIMIZED
        /// </summary>
        public virtual async Task<IReadOnlyList<TDto>> FindProjectedAsync<TDto>(BaseEntitySpecification<T> specification, CancellationToken cancellationToken = default)
            where TDto : class
        {
            return await ApplySpecification(specification)
                .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Project first entity to DTO using specification - PERFORMANCE OPTIMIZED
        /// </summary>
        public virtual async Task<TDto?> FirstOrDefaultProjectedAsync<TDto>(BaseEntitySpecification<T> specification, CancellationToken cancellationToken = default)
            where TDto : class
        {
            return await ApplySpecification(specification)
                .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Get paginated DTOs using specification - PERFORMANCE OPTIMIZED
        /// </summary>
        public virtual async Task<PaginatedList<TDto>> GetPaginatedProjectedAsync<TDto>(
            BaseEntitySpecification<T> specification,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
            where TDto : class
        {
            // Count query (without projection for performance)
            var countSpec = CreateCountSpecification(specification);
            var totalCount = await ApplySpecification(countSpec).CountAsync(cancellationToken);

            // Data query with projection
            var items = await ApplySpecification(specification)
                .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return new PaginatedList<TDto>(items, totalCount, pageNumber, pageSize);
        }

        /// <summary>
        /// Get paginated DTOs using predicate - PERFORMANCE OPTIMIZED
        /// </summary>
        public virtual async Task<PaginatedList<TDto>> GetPaginatedProjectedAsync<TDto>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> orderBy,
            bool descending,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
            where TDto : class
        {
            var baseQuery = _dbSet.Where(predicate);

            // Count query
            var totalCount = await baseQuery.CountAsync(cancellationToken);

            // Data query with projection
            var dataQuery = descending
                ? baseQuery.OrderByDescending(orderBy)
                : baseQuery.OrderBy(orderBy);

            var items = await dataQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return new PaginatedList<TDto>(items, totalCount, pageNumber, pageSize);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Apply specification to the query
        /// </summary>
        protected virtual IQueryable<T> ApplySpecification(BaseEntitySpecification<T> specification)
        {
            return SpecificationEvaluator.Default.GetQuery(_dbSet.AsQueryable(), specification);
        }

        /// <summary>
        /// Create a count-only specification from an existing specification
        /// </summary>
        protected virtual BaseEntitySpecification<T> CreateCountSpecification(BaseEntitySpecification<T> specification)
        {
            // Create a new specification with the same criteria but without includes, ordering, pagination
            var countSpec = new BaseEntitySpecification<T>();

            // Copy where expressions
            foreach (var whereExpression in specification.WhereExpressions)
            {
                countSpec.Where(whereExpression.Filter);
            }

            return countSpec;
        }

        #endregion
    }

    /// <summary>
    /// Convenience implementation for entities with int keys
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public class QueryRepository<T> : QueryRepository<T, int>, IQueryRepository<T>
        where T : BaseEntity<int>
    {
        public QueryRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }
    }
}