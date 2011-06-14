using System.Linq;
using CodeSmith.Data.Caching;
using CodeSmith.Data.Future;

namespace CodeSmith.Data.Linq
{
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
        public static IFutureValue<int> FutureCacheCount<T>(this IQueryable<T> source)
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
        public static IFutureValue<int> FutureCacheCount<T>(this IQueryable<T> source, string profileName)
        {
            var cacheSettings = CacheManager.GetProfile(profileName);

            return FutureCacheCount(source, cacheSettings);
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
        public static IFutureValue<int> FutureCacheCount<T>(this IQueryable<T> source, int duration)
        {
            var cacheSettings = new CacheSettings(duration);

            return FutureCacheCount(source, cacheSettings);
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
        public static IFutureValue<int> FutureCacheCount<T>(this IQueryable<T> source, CacheSettings cacheSettings)
        {
            if (source == null)
                return new LoadedFutureValue<int>(0, source);

            var db = source.GetFutureConext();

            return db.FutureCount(source, cacheSettings);
        }
    }
}