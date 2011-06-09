using System.Linq;
using CodeSmith.Data.Caching;
using NHibernate;
using Plinqo.NHibernate;

namespace CodeSmith.Data.Linq
{
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
        public static IFutureValue<T> FutureCacheFirstOrDefault<T>(this IQueryable<T> source)
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
        public static IFutureValue<T> FutureCacheFirstOrDefault<T>(this IQueryable<T> source, string profileName)
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
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureValue`1"/>
        public static IFutureValue<T> FutureCacheFirstOrDefault<T>(this IQueryable<T> source, int duration)
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
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureValue`1"/>
        public static IFutureValue<T> FutureCacheFirstOrDefault<T>(this IQueryable<T> source, CacheSettings cacheSettings)
        {
            if (source == null)
                return null;

            var key = source.GetHashKey();
            var cache = QueryResultCache.GetResultCache<T>(key, cacheSettings);

            return cache == null
                       ? source.FutureFirstOrDefault()
                       : new NHibernate.FutureValue<T>(cache.FirstOrDefault());
        }
    }
}