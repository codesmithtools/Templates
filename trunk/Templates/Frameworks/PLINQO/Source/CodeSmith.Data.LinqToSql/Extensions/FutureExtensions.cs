using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeSmith.Data.Caching;

namespace CodeSmith.Data.Linq
{
    public static class FutureExtensions
    {
        /// <summary>
        /// Provides for defering the execution of the <paramref name="source" /> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1" /> to add to the batch of future queries.</param>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains elements from the input sequence.</returns>
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureQuery`1"/>
        public static FutureQuery<T> Future<T>(this IQueryable<T> source)
        {
            return source.FutureCache((CacheSettings)null);
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source" /> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1" /> to add to the batch of future queries.</param>
        /// <returns>An instance of <see cref="FutureCount"/> that contains the result of the query.</returns>
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureCount"/>
        public static FutureCount FutureCount<T>(this IQueryable<T> source)
        {
            return source.FutureCacheCount((CacheSettings)null);
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source" /> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1" /> to add to the batch of future queries.</param>
        /// <returns>An instance of <see cref="T:CodeSmith.Data.Linq.FutureValue`1"/> that contains the result of the query.</returns>
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureValue`1"/>
        public static FutureValue<T> FutureFirstOrDefault<T>(this IQueryable<T> source)
        {
            return source.FutureCacheFirstOrDefault((CacheSettings)null);
        }

        internal static IFutureContext GetFutureContext(this IQueryable source)
        {
            DataContext context = source.GetDataContext();
            if (context == null)
                throw new ArgumentException("The source must originate from a DataContext that implements IFutureContext.", "source");

            IFutureContext futureContext = context as IFutureContext;
            if (futureContext == null)
                throw new ArgumentException("The source must originate from a DataContext that implements IFutureContext.", "source");

            return futureContext;
        }
    }

    public static class FutureCacheExtensions
    {
        /// <summary>
        /// Provides for defering the execution of the <paramref name="source"/> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to add to the batch of future queries.</param>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains elements from the input sequence.
        /// </returns>
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureQuery`1"/>
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
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureQuery`1"/>
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
        /// <param name="profileName">Name of the cache profile to use.</param>
        /// <param name="sqlCacheDependencyTableNames">The table names for which to add SQL Cache Dependencies</param>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains elements from the input sequence.
        /// </returns>
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureQuery`1"/>
        public static FutureQuery<T> FutureCache<T>(this IQueryable<T> source, string profileName
            , params string[] sqlCacheDependencyTableNames)
        {
            CacheSettings cacheSettings = CacheManager.GetProfile(profileName).AddCacheDependency(source, sqlCacheDependencyTableNames);
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
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureQuery`1"/>
        public static FutureQuery<T> FutureCache<T>(this IQueryable<T> source, int duration)
        {
            return FutureCache(source, new CacheSettings(duration));
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source"/> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to add to the batch of future queries.</param>
        /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
        /// <param name="sqlCacheDependencyTableNames">The table names for which to add SQL Cache Dependencies</param>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains elements from the input sequence.
        /// </returns>
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureQuery`1"/>
        public static FutureQuery<T> FutureCache<T>(this IQueryable<T> source, int duration,
            params string[] sqlCacheDependencyTableNames)
        {
            return FutureCache(source, new CacheSettings(duration)
                .AddCacheDependency(source, sqlCacheDependencyTableNames));
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
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureQuery`1"/>
        public static FutureQuery<T> FutureCache<T>(this IQueryable<T> source, CacheSettings cacheSettings)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            IFutureContext db = source.GetFutureContext();
            var future = new FutureQuery<T>(source, db.ExecuteFutureQueries, cacheSettings);
            db.FutureQueries.Add(future);
            return future;
        }
    }

    public static class FutureCacheCountExtensions
    {
        /// <summary>
        /// Provides for defering the execution of the <paramref name="source"/> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to add to the batch of future queries.</param>
        /// <returns>
        /// An instance of <see cref="FutureCount"/> that contains the result of the query.
        /// </returns>
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureCount"/>
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
        /// An instance of <see cref="FutureCount"/> that contains the result of the query.
        /// </returns>
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureCount"/>
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
        /// <param name="profileName">Name of the cache profile to use.</param>
        /// <param name="sqlCacheDependencyTableNames">The table names for which to add SQL Cache Dependencies</param>
        /// <returns>
        /// An instance of <see cref="FutureCount"/> that contains the result of the query.
        /// </returns>
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureCount"/>
        public static FutureCount FutureCacheCount<T>(this IQueryable<T> source, string profileName, params string[] sqlCacheDependencyTableNames)
        {
            CacheSettings cacheSettings = CacheManager.GetProfile(profileName).AddCacheDependency(source, sqlCacheDependencyTableNames);
            return FutureCacheCount(source, cacheSettings);
        }
        
        /// <summary>
        /// Provides for defering the execution of the <paramref name="source"/> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to add to the batch of future queries.</param>
        /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
        /// <param name="sqlCacheDependencyTableNames">The table names for which to add SQL Cache Dependencies</param>
        /// <returns>
        /// An instance of <see cref="FutureCount"/> that contains the result of the query.
        /// </returns>
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureCount"/>
        public static FutureCount FutureCacheCount<T>(this IQueryable<T> source, int duration
            , params string[] sqlCacheDependencyTableNames)
        {
            return FutureCacheCount(source, new CacheSettings(duration).AddCacheDependency(source, sqlCacheDependencyTableNames));
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source"/> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to add to the batch of future queries.</param>
        /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
        /// <returns>
        /// An instance of <see cref="FutureCount"/> that contains the result of the query.
        /// </returns>
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureCount"/>
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
        /// An instance of <see cref="FutureCount"/> that contains the result of the query.
        /// </returns>
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureCount"/>
        public static FutureCount FutureCacheCount<T>(this IQueryable<T> source, CacheSettings cacheSettings)
        {
            if (source == null)
                return new FutureCount(0);

            IFutureContext db = source.GetFutureContext();
            var future = new FutureCount(source, db.ExecuteFutureQueries, cacheSettings);
            db.FutureQueries.Add(future);
            return future;
        }
    }

    public static class FutureCacheFirstOrDefaultExtensions
    {
        /// <summary>
        /// Provides for defering the execution of the <paramref name="source"/> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to add to the batch of future queries.</param>
        /// <returns>
        /// An instance of <see cref="T:CodeSmith.Data.Linq.FutureValue`1"/> that contains the result of the query.
        /// </returns>
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureValue`1"/>
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
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureValue`1"/>
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
        /// <param name="profileName">Name of the cache profile to use.</param>
        /// <param name="sqlCacheDependencyTableNames">The table names for which to add SQL Cache Dependencies</param>
        /// <returns>
        /// An instance of <see cref="T:CodeSmith.Data.Linq.FutureValue`1"/> that contains the result of the query.
        /// </returns>
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureValue`1"/>
        public static FutureValue<T> FutureCacheFirstOrDefault<T>(this IQueryable<T> source, string profileName
            , params string[] sqlCacheDependencyTableNames)
        {
            CacheSettings cacheSettings = CacheManager.GetProfile(profileName).AddCacheDependency(source, sqlCacheDependencyTableNames);
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
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureValue`1"/>
        public static FutureValue<T> FutureCacheFirstOrDefault<T>(this IQueryable<T> source, int duration)
        {
            return FutureCacheFirstOrDefault(source, new CacheSettings(duration));
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source"/> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to add to the batch of future queries.</param>
        /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
        /// <param name="sqlCacheDependencyTableNames">The table names for which to add SQL Cache Dependencies</param>
        /// <returns>
        /// An instance of <see cref="T:CodeSmith.Data.Linq.FutureValue`1"/> that contains the result of the query.
        /// </returns>
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureValue`1"/>
        public static FutureValue<T> FutureCacheFirstOrDefault<T>(this IQueryable<T> source, int duration
            , params string[] sqlCacheDependencyTableNames)
        {
            return FutureCacheFirstOrDefault(source, new CacheSettings(duration).AddCacheDependency(source, sqlCacheDependencyTableNames));
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
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureValue`1"/>
        public static FutureValue<T> FutureCacheFirstOrDefault<T>(this IQueryable<T> source, CacheSettings cacheSettings)
        {
            if (source == null)
                return new FutureValue<T>(default(T));

            // make sure to only get the first value
            var firstQuery = source.Take(1);

            IFutureContext db = source.GetFutureContext();
            var future = new FutureValue<T>(firstQuery, db.ExecuteFutureQueries, cacheSettings);
            db.FutureQueries.Add(future);
            return future;
        }
    }
}
