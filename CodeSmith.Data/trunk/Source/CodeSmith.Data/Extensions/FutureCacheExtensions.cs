using System;
using System.Collections.Generic;
using System.Linq;
using CodeSmith.Data.Caching;

namespace CodeSmith.Data.Linq
{
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
        public static IEnumerable<T> FutureCache<T>(this IQueryable<T> source)
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
        public static IEnumerable<T> FutureCache<T>(this IQueryable<T> source, string profileName)
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
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureQuery`1"/>
        public static IEnumerable<T> FutureCache<T>(this IQueryable<T> source, int duration)
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
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureQuery`1"/>
        public static IEnumerable<T> FutureCache<T>(this IQueryable<T> source, CacheSettings cacheSettings)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var key = source.GetHashKey();
            var cache = QueryResultCache.GetResultCache<T>(key, cacheSettings);

            return cache ?? source.Future(cacheSettings);
        }
    }
}