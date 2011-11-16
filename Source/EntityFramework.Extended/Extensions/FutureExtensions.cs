using System;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using EntityFramework.Future;
using EntityFramework.Reflection;

namespace EntityFramework.Extensions
{
    /// <summary>
    /// Extension methods for future queries.
    /// </summary>
    /// <seealso cref="T:EntityFramework.Future.FutureQuery`1"/>
    /// <seealso cref="T:EntityFramework.Future.FutureValue`1"/>
    /// <seealso cref="T:EntityFramework.Future.FutureCount"/>
    public static class FutureExtensions
    {
        /// <summary>
        /// Provides for defering the execution of the <paramref name="source" /> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="TEntity">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1" /> to add to the batch of future queries.</param>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains elements from the input sequence.</returns>
        /// <seealso cref="T:EntityFramework.Future.FutureQuery`1"/>
        public static FutureQuery<TEntity> Future<TEntity>(this IQueryable<TEntity> source)
          where TEntity : class
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var futureContext = GetFutureContext(source);
            var future = new FutureQuery<TEntity>(source, futureContext.ExecuteFutureQueries);
            futureContext.FutureQueries.Add(future);
            return future;
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source" /> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="TEntity">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1" /> to add to the batch of future queries.</param>
        /// <returns>An instance of <see cref="FutureCount"/> that contains the result of the query.</returns>
        /// <seealso cref="T:EntityFramework.Future.FutureCount"/>
        public static FutureCount FutureCount<TEntity>(this IQueryable<TEntity> source)
            where TEntity : class
        {
            if (source == null)
                return new FutureCount(0);

            // create count expression
            var expression = Expression.Call(
              typeof(Queryable),
              "Count",
              new[] { source.ElementType },
              source.Expression);

            // ObjectQueryProvider
            dynamic providerProxy = new DynamicProxy(source.Provider);
            IQueryable countQuery = providerProxy.CreateQuery(expression, typeof(int));

            var futureContext = GetFutureContext(source);
            var future = new FutureCount(countQuery, futureContext.ExecuteFutureQueries);
            futureContext.FutureQueries.Add(future);
            return future;
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source" /> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="TEntity">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1" /> to add to the batch of future queries.</param>
        /// <returns>An instance of <see cref="T:EntityFramework.Future.FutureValue`1"/> that contains the result of the query.</returns>
        /// <seealso cref="T:EntityFramework.Future.FutureValue`1"/>
        public static FutureValue<TEntity> FutureFirstOrDefault<TEntity>(this IQueryable<TEntity> source)
          where TEntity : class
        {
            if (source == null)
                return new FutureValue<TEntity>(default(TEntity));

            // make sure to only get the first value
            var firstQuery = source.Take(1);

            var futureContext = GetFutureContext(source);
            var future = new FutureValue<TEntity>(firstQuery, futureContext.ExecuteFutureQueries);
            futureContext.FutureQueries.Add(future);
            return future;
        }

        private static IFutureContext GetFutureContext<TEntity>(IQueryable<TEntity> source) where TEntity : class
        {
            // first try getting IFutureContext directly off ObjectContext
            var objectQuery = source as ObjectQuery;
            if (objectQuery != null)
            {
                var objectContext = objectQuery.Context as IFutureContext;
                if (objectContext != null)
                    return objectContext;
            }

            // next use FutureStore
            var futureContext = FutureStore.Default.GetOrCreate(source);
            return futureContext;
        }

    }
}
