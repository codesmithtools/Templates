using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CodeSmith.Data.Caching;
using CodeSmith.Data.LinqToSql;

namespace CodeSmith.Data.Linq
{
    public static class UtilityExtensions
    {
        /// <summary>
        /// Specifies the related objects to include in the query results.
        /// </summary>
        /// <typeparam name="T">The type of the data in the data source.</typeparam>
        /// <typeparam name="TInclude">The type of the include.</typeparam>
        /// <param name="query">The query to be materialized.</param>
        /// <param name="includeExpression">The expression of related objects to return in the query results.</param>
        /// <returns>The result of the query.</returns>
        //http://damieng.com/blog/2010/05/21/include-for-linq-to-sql-and-maybe-other-providers
        public static IEnumerable<T> Include<T, TInclude>(this IQueryable<T> query, Expression<Func<T, TInclude>> includeExpression)
        {
            var db = LinqToSqlDataContextProvider.GetDataContext(query);

            if (db == null)
                throw new ArgumentException("The query must originate from a DataContext.", "query");

            if (!db.ObjectTrackingEnabled || !db.DeferredLoadingEnabled)
                throw new ArgumentException("The query's originating DataContext must have ObjectTrackingEnabled and DeferredLoadingEnabled set to true.", "query");

            var elementParameter = includeExpression.Parameters.Single();
            var tupleType = typeof(Tuple<T, TInclude>);

            var body = Expression.New(tupleType.GetConstructor(new[] { typeof(T), typeof(TInclude) }),
                new[] { elementParameter, includeExpression.Body },
                tupleType.GetProperty("Item1"),
                tupleType.GetProperty("Item2"));

            var selector = Expression.Lambda<Func<T, Tuple<T, TInclude>>>(body, elementParameter);

            return query.Select(selector).AsEnumerable().Select(t => t.Item1);
        }

        private class Tuple<T1, T2>
        {
            public Tuple(T1 item1, T2 item2)
            {
                Item1 = item1;
                Item2 = item2;
            }

            public T1 Item1 { get; private set; }
            public T2 Item2 { get; private set; }
        }
    }
}