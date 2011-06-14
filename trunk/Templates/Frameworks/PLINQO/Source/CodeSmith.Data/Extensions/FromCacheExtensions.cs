using System.Collections.Generic;
using System.Linq;
using CodeSmith.Data.Caching;

namespace CodeSmith.Data.Linq
{
    public static class FromCacheExtensions
    {
        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// </summary>
        /// <typeparam name="T">The type of the data in the data source.</typeparam>
        /// <param name="query">The query to be materialized.</param>
        /// <returns>The result of the query.</returns>
        public static IEnumerable<T> FromCache<T>(this IQueryable<T> query)
        {
            return query.FromCache((CacheSettings)null);
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// </summary>
        /// <param name="query">The query to be materialized.</param>
        /// <returns>The result of the query.</returns>
        public static IEnumerable<object> FromCache(this IQueryable query)
        {
            return query.Cast<object>().FromCache();
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// </summary>
        /// <typeparam name="T">The type of the data in the data source.</typeparam>
        /// <param name="query">The query to be materialized.</param>
        /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
        /// <returns>The result of the query.</returns>
        public static IEnumerable<T> FromCache<T>(this IQueryable<T> query, int duration)
        {
            return query.FromCache(new CacheSettings(duration));
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// </summary>
        /// <param name="query">The query to be materialized.</param>
        /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
        /// <returns>The result of the query.</returns>
        public static IEnumerable<object> FromCache(this IQueryable query, int duration)
        {
            return query.Cast<object>().FromCache(duration);
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// </summary>
        /// <typeparam name="T">The type of the data in the data source.</typeparam>
        /// <param name="query">The query to be materialized.</param>
        /// <param name="profileName">Name of the cache profile to use.</param>
        /// <returns>The result of the query.</returns>
        public static IEnumerable<T> FromCache<T>(this IQueryable<T> query, string profileName)
        {
            CacheSettings cacheSettings = CacheManager.GetProfile(profileName);
            return query.FromCache(cacheSettings);
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// </summary>
        /// <param name="query">The query to be materialized.</param>
        /// <param name="profileName">Name of the cache profile to use.</param>
        /// <returns>The result of the query.</returns>
        public static IEnumerable<object> FromCache(this IQueryable query, string profileName)
        {
            return query.Cast<object>().FromCache(profileName);
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// </summary>
        /// <typeparam name="T">The type of the data in the data source.</typeparam>
        /// <param name="query">The query to be materialized.</param>
        /// <param name="settings">Cache settings object.</param>
        /// <returns>The result of the query.</returns>
        public static IEnumerable<T> FromCache<T>(this IQueryable<T> query, CacheSettings settings)
        {
            return QueryResultCache.FromCache(query, settings);
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// </summary>
        /// <param name="query">The query to be materialized.</param>
        /// <param name="settings">Cache settings object.</param>
        /// <returns>The result of the query.</returns>
        public static IEnumerable<object> FromCache(this IQueryable query, CacheSettings settings)
        {
            return query.Cast<object>().FromCache(settings);
        }
    }
}