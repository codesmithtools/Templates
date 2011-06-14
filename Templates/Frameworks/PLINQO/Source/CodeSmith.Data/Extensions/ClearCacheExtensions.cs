using System;
using System.Linq;
using CodeSmith.Data.Caching;

namespace CodeSmith.Data.Linq
{
    public static class ClearCacheExtensions
    {
        /// <summary>
        /// Clears the cache of a given query.
        /// </summary>
        /// <typeparam name="T">The type of the data in the data source.</typeparam>
        /// <param name="query">The query to be cleared.</param>
        public static bool ClearCache<T>(this IQueryable<T> query)
        {
            return ClearCache(query, (CacheSettings)null);
        }

        /// <summary>
        /// Clears the cache of a given query.
        /// </summary>
        /// <typeparam name="T">The type of the data in the data source.</typeparam>
        /// <param name="query">The query to be cleared.</param>
        /// <param name="group">The name of the cache group.</param>
        public static bool ClearCache<T>(this IQueryable<T> query, string group)
        {
            return ClearCache(query, group, null);
        }

        /// <summary>
        /// Clears the cache of a given query.
        /// </summary>
        /// <typeparam name="T">The type of the data in the data source.</typeparam>
        /// <param name="query">The query to be cleared.</param>
        /// <param name="group">The cache group.</param>
        /// <param name="provider">The name of the cache provider.</param>
        public static bool ClearCache<T>(this IQueryable<T> query, string group, string provider)
        {
            ICacheProvider cacheProvider = CacheManager.GetProvider(provider);
            string key = query.GetHashKey();

            // A group was specified, use it.
            if (!String.IsNullOrEmpty(group))
                return cacheProvider.Remove(key, group);

            return cacheProvider.Remove(key);
        }

        /// <summary>
        /// Clears the cache of a given query.
        /// </summary>
        /// <typeparam name="T">The type of the data in the data source.</typeparam>
        /// <param name="query">The query to be cleared.</param>
        /// <param name="settings">Cache settings object.</param>
        public static bool ClearCache<T>(this IQueryable<T> query, CacheSettings settings)
        {
            if (settings == null)
                settings = CacheManager.GetProfile();

            return ClearCache(query, settings.Group, settings.Provider);
        }
    }
}