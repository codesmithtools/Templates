using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeSmith.Data.Caching;

namespace CodeSmith.Data.Linq
{
    /// <summary>
    /// Extension methods for future queries.
    /// </summary>
    public static class FutureExtensions
    {
        /// <summary>
        /// Provides for defering the execution of the <paramref name="source" /> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1" /> to add to the batch of future queries.</param>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains elements from the input sequence.</returns>
        public static IEnumerable<T> Future<T>(this IQueryable<T> source)
        {
            return Future(source, null);
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source"/> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to add to the batch of future queries.</param>
        /// <param name="cacheSettings">The cache settings.</param>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains elements from the input sequence.
        /// </returns>
        public static IEnumerable<T> Future<T>(this IQueryable<T> source, CacheSettings cacheSettings)
        {
            if (source == null)
                return source;

            var db = source.GetDataContext() as IFutureContext;
            if (db == null)
                throw new ArgumentException("The source must originate from a DataContext that implements IFutureContext.", "source");

            var future = new FutureQuery<T>(source, db.ExecuteFutureQueries, cacheSettings);
            db.FutureQueries.Add(future);
            return future;
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source" /> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1" /> to add to the batch of future queries.</param>
        /// <returns>An instance of FutureCount that contains the result of the query.</returns>
        public static FutureCount FutureCount<T>(this IQueryable<T> source)
        {
            return FutureCount(source, null);
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source"/> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to add to the batch of future queries.</param>
        /// <param name="cacheSettings">The cache settings.</param>
        /// <returns>
        /// An instance of FutureCount that contains the result of the query.
        /// </returns>
        public static FutureCount FutureCount<T>(this IQueryable<T> source, CacheSettings cacheSettings)
        {
            if (source == null)
                return new FutureCount(0);

            var db = source.GetDataContext() as IFutureContext;
            if (db == null)
                throw new ArgumentException("The source must originate from a DataContext that implements IFutureContext.", "source");

            var future = new FutureCount(source, db.ExecuteFutureQueries, cacheSettings);
            db.FutureQueries.Add(future);
            return future;
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source" /> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1" /> to add to the batch of future queries.</param>
        /// <returns>An instance of <see cref="T:CodeSmith.Data.Linq.FutureValue`1"/> that contains the result of the query.</returns>
        public static FutureValue<T> FutureFirstOrDefault<T>(this IQueryable<T> source)
        {
            return FutureFirstOrDefault<T>(source, null);
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source"/> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to add to the batch of future queries.</param>
        /// <param name="cacheSettings">The cache settings.</param>
        /// <returns>
        /// An instance of <see cref="T:CodeSmith.Data.Linq.FutureValue`1"/> that contains the result of the query.
        /// </returns>
        public static FutureValue<T> FutureFirstOrDefault<T>(this IQueryable<T> source, CacheSettings cacheSettings)
        {
            if (source == null)
                return new FutureValue<T>(default(T));

            // make sure to only get the first value
            var firstQuery = source.Take(1);

            var db = source.GetDataContext() as IFutureContext;
            if (db == null)
                throw new ArgumentException("The source must originate from a DataContext that implements IFutureContext.", "source");

            var future = new FutureValue<T>(firstQuery, db.ExecuteFutureQueries, cacheSettings);
            db.FutureQueries.Add(future);
            return future;
        }

    }
}
