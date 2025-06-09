using Ardalis.Specification;
using System.Linq.Expressions;

namespace Studdit.Application.Common.Specifications
{
    /// <summary>
    /// Generic base specification that can be used with any entity
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    public class BaseEntitySpecification<T> : Specification<T> where T : class
    {
        public BaseEntitySpecification()
        {
        }

        /// <summary>
        /// Add a where condition to the specification
        /// </summary>
        public BaseEntitySpecification<T> Where(Expression<Func<T, bool>> criteria)
        {
            Query.Where(criteria);
            return this;
        }

        /// <summary>
        /// Add includes for navigation properties
        /// </summary>
        public BaseEntitySpecification<T> Include(Expression<Func<T, object>> includeExpression)
        {
            Query.Include(includeExpression);
            return this;
        }

        /// <summary>
        /// Add nested includes (ThenInclude)
        /// </summary>
        public BaseEntitySpecification<T> ThenInclude<TPreviousProperty, TProperty>(
            Expression<Func<T, TPreviousProperty>> includeExpression,
            Expression<Func<TPreviousProperty, TProperty>> thenIncludeExpression)
            where TPreviousProperty : class
            where TProperty : class
        {
            Query.Include(includeExpression).ThenInclude(thenIncludeExpression);
            return this;
        }

        /// <summary>
        /// Add ordering by a property
        /// </summary>
        public BaseEntitySpecification<T> OrderBy(Expression<Func<T, object?>> orderExpression)
        {
            Query.OrderBy(orderExpression);
            return this;
        }

        /// <summary>
        /// Add descending ordering by a property
        /// </summary>
        public BaseEntitySpecification<T> OrderByDescending(Expression<Func<T, object?>> orderExpression)
        {
            Query.OrderByDescending(orderExpression);
            return this;
        }

        /// <summary>
        /// Add pagination to the specification
        /// </summary>
        public BaseEntitySpecification<T> Paginate(int pageNumber, int pageSize)
        {
            if (pageNumber > 0 && pageSize > 0)
            {
                Query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            }
            return this;
        }

        /// <summary>
        /// Take only a specific number of items
        /// </summary>
        public new BaseEntitySpecification<T> Take(int count)
        {
            Query.Take(count);
            return this;
        }

        /// <summary>
        /// Skip a specific number of items
        /// </summary>
        public new BaseEntitySpecification<T> Skip(int count)
        {
            Query.Skip(count);
            return this;
        }

        /// <summary>
        /// Enable split query for better performance with multiple includes
        /// </summary>
        public new BaseEntitySpecification<T> AsSplitQuery()
        {
            Query.AsSplitQuery();
            return this;
        }

        /// <summary>
        /// Disable tracking for read-only scenarios
        /// </summary>
        public new BaseEntitySpecification<T> AsNoTracking()
        {
            Query.AsNoTracking();
            return this;
        }
    }

    /// <summary>
    /// Generic specification builder for fluent specification creation
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    public static class SpecificationBuilder<T> where T : class
    {
        public static BaseEntitySpecification<T> Create()
        {
            return new BaseEntitySpecification<T>();
        }

        public static BaseEntitySpecification<T> Where(Expression<Func<T, bool>> criteria)
        {
            return new BaseEntitySpecification<T>().Where(criteria);
        }
    }
}
