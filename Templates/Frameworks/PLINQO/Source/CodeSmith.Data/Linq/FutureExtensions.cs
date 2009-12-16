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
        public static FutureQuery<T> Future<T>(this IQueryable<T> source)
        {
            return FutureCache(source, (CacheSettings)null);
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source"/> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to add to the batch of future queries.</param>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains elements from the input sequence.
        /// </returns>
        public static FutureQuery<T> FutureCache<T>(this IQueryable<T> source)
        {
            var cacheSettings = CacheManager.GetProfile();
            return FutureCache(source, cacheSettings);
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source"/> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to add to the batch of future queries.</param>
        /// <param name="profileName">Name of the cache profile to use.</param>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains elements from the input sequence.
        /// </returns>
        public static FutureQuery<T> FutureCache<T>(this IQueryable<T> source, string profileName)
        {
            CacheSettings cacheSettings = CacheManager.GetProfile(profileName);
            return FutureCache(source, cacheSettings);
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source"/> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to add to the batch of future queries.</param>
        /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains elements from the input sequence.
        /// </returns>
        public static FutureQuery<T> FutureCache<T>(this IQueryable<T> source, int duration)
        {
            return FutureCache(source, new CacheSettings(duration));
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
        public static FutureQuery<T> FutureCache<T>(this IQueryable<T> source, CacheSettings cacheSettings)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            IFutureContext db = GetFutureContext(source);
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
            return FutureCacheCount(source, (CacheSettings)null);
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source"/> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to add to the batch of future queries.</param>
        /// <returns>
        /// An instance of FutureCount that contains the result of the query.
        /// </returns>
        public static FutureCount FutureCacheCount<T>(this IQueryable<T> source)
        {
            var cacheSettings = CacheManager.GetProfile();
            return FutureCacheCount(source, cacheSettings);
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source"/> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to add to the batch of future queries.</param>
        /// <param name="profileName">Name of the cache profile to use.</param>
        /// <returns>
        /// An instance of FutureCount that contains the result of the query.
        /// </returns>
        public static FutureCount FutureCacheCount<T>(this IQueryable<T> source, string profileName)
        {
            CacheSettings cacheSettings = CacheManager.GetProfile(profileName);
            return FutureCacheCount(source, cacheSettings);
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source"/> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to add to the batch of future queries.</param>
        /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
        /// <returns>
        /// An instance of FutureCount that contains the result of the query.
        /// </returns>
        public static FutureCount FutureCacheCount<T>(this IQueryable<T> source, int duration)
        {
            return FutureCacheCount(source, new CacheSettings(duration));
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
        public static FutureCount FutureCacheCount<T>(this IQueryable<T> source, CacheSettings cacheSettings)
        {
            if (source == null)
                return new FutureCount(0);

            IFutureContext db = GetFutureContext(source);
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
            return FutureCacheFirstOrDefault(source, (CacheSettings)null);
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source"/> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to add to the batch of future queries.</param>
        /// <returns>
        /// An instance of <see cref="T:CodeSmith.Data.Linq.FutureValue`1"/> that contains the result of the query.
        /// </returns>
        public static FutureValue<T> FutureCacheFirstOrDefault<T>(this IQueryable<T> source)
        {
            var cacheSettings = CacheManager.GetProfile();
            return FutureCacheFirstOrDefault(source, cacheSettings);
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source"/> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to add to the batch of future queries.</param>
        /// <param name="profileName">Name of the cache profile to use.</param>
        /// <returns>
        /// An instance of <see cref="T:CodeSmith.Data.Linq.FutureValue`1"/> that contains the result of the query.
        /// </returns>
        public static FutureValue<T> FutureCacheFirstOrDefault<T>(this IQueryable<T> source, string profileName)
        {
            CacheSettings cacheSettings = CacheManager.GetProfile(profileName);
            return FutureCacheFirstOrDefault(source, cacheSettings);
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source"/> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to add to the batch of future queries.</param>
        /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
        /// <returns>
        /// An instance of <see cref="T:CodeSmith.Data.Linq.FutureValue`1"/> that contains the result of the query.
        /// </returns>
        public static FutureValue<T> FutureCacheFirstOrDefault<T>(this IQueryable<T> source, int duration)
        {
            return FutureCacheFirstOrDefault(source, new CacheSettings(duration));
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
        public static FutureValue<T> FutureCacheFirstOrDefault<T>(this IQueryable<T> source, CacheSettings cacheSettings)
        {
            if (source == null)
                return new FutureValue<T>(default(T));

            // make sure to only get the first value
            var firstQuery = source.Take(1);

            IFutureContext db = GetFutureContext(source); 
            var future = new FutureValue<T>(firstQuery, db.ExecuteFutureQueries, cacheSettings);
            db.FutureQueries.Add(future);
            return future;
        }

        private static IFutureContext GetFutureContext(IQueryable source)
        {
            DataContext context = source.GetDataContext();
            if (context == null)
                throw new ArgumentException("The source must originate from a DataContext that implements IFutureContext.", "source");
            if (context.LoadOptions != null)
                throw new NotSupportedException("Future queries are not supported when the DataContext has LoadOptions.");

            IFutureContext futureContext = context as IFutureContext;
            if (futureContext == null)
                throw new ArgumentException("The source must originate from a DataContext that implements IFutureContext.", "source");

            return futureContext;
        }

    }
}
