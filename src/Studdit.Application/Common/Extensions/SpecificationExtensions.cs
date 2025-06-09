using Studdit.Application.Common.Specifications;
using Studdit.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Studdit.Application.Common.Extensions
{
    /// <summary>
    /// Specification extensions for common entity operations
    /// </summary>
    public static class SpecificationExtensions
    {
        /// <summary>
        /// Filter by ID for entities that inherit from BaseEntity
        /// </summary>
        public static BaseEntitySpecification<T> ById<T>(this BaseEntitySpecification<T> spec, int id)
            where T : BaseEntity
        {
            return spec.Where(x => x.Id == id);
        }

        /// <summary>
        /// Filter by multiple IDs for entities that inherit from BaseEntity
        /// </summary>
        public static BaseEntitySpecification<T> ByIds<T>(this BaseEntitySpecification<T> spec, IEnumerable<int> ids)
            where T : BaseEntity
        {
            return spec.Where(x => ids.Contains(x.Id));
        }

        /// <summary>
        /// Filter by creation date range for entities that inherit from BaseEntity
        /// </summary>
        public static BaseEntitySpecification<T> CreatedBetween<T>(this BaseEntitySpecification<T> spec, DateTime startDate, DateTime endDate)
            where T : BaseEntity
        {
            return spec.Where(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate);
        }

        /// <summary>
        /// Search in text properties (generic text search)
        /// </summary>
        public static BaseEntitySpecification<T> Search<T>(this BaseEntitySpecification<T> spec,
            string searchTerm, params Expression<Func<T, string>>[] searchProperties)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || !searchProperties.Any())
                return spec;

            Expression<Func<T, bool>>? combinedExpression = null;

            foreach (var property in searchProperties)
            {
                var containsExpression = CreateContainsExpression(property, searchTerm);

                if (combinedExpression == null)
                {
                    combinedExpression = containsExpression;
                }
                else
                {
                    combinedExpression = CombineWithOr(combinedExpression, containsExpression);
                }
            }

            return combinedExpression != null ? spec.Where(combinedExpression) : spec;
        }

        private static Expression<Func<T, bool>> CreateContainsExpression<T>(
            Expression<Func<T, string>> propertyExpression, string searchTerm)
        {
            var parameter = propertyExpression.Parameters.First();
            var property = propertyExpression.Body;

            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
            var searchValue = Expression.Constant(searchTerm, typeof(string));
            var containsCall = Expression.Call(property, containsMethod, searchValue);

            return Expression.Lambda<Func<T, bool>>(containsCall, parameter);
        }

        private static Expression<Func<T, bool>> CombineWithOr<T>(
            Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftBody = ReplaceParameter(left.Body, left.Parameters[0], parameter);
            var rightBody = ReplaceParameter(right.Body, right.Parameters[0], parameter);

            var orExpression = Expression.OrElse(leftBody, rightBody);
            return Expression.Lambda<Func<T, bool>>(orExpression, parameter);
        }

        private static Expression ReplaceParameter(Expression expression, ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            return new ParameterReplacer(oldParameter, newParameter).Visit(expression);
        }

        private class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression _oldParameter;
            private readonly ParameterExpression _newParameter;

            public ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _oldParameter ? _newParameter : node;
            }
        }
    }
}
