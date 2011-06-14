using System.Linq;

namespace CodeSmith.Data.Linq
{
    public static class ClearCacheFirstOrDefaultExtensions
    {
        /// <summary>
        /// Clears the cache of a given query.
        /// </summary>
        /// <typeparam name="T">The type of the data in the data source.</typeparam>
        /// <param name="query">The query to be cleared.</param>
        public static bool ClearCacheFirstOrDefault<T>(this IQueryable<T> query)
        {
            return ClearCacheFirstOrDefault(query, null, null);
        }

        /// <summary>
        /// Clears the cache of a given query.
        /// </summary>
        /// <typeparam name="T">The type of the data in the data source.</typeparam>
        /// <param name="query">The query to be cleared.</param>
        /// <param name="group">The name of the cache group.</param>
        public static bool ClearCacheFirstOrDefault<T>(this IQueryable<T> query, string group)
        {
            return ClearCacheFirstOrDefault(query, group, null);
        }

        /// <summary>
        /// Clears the cache of a given query.
        /// </summary>
        /// <typeparam name="T">The type of the data in the data source.</typeparam>
        /// <param name="query">The query to be cleared.</param>
        /// <param name="group">The cache group.</param>
        /// <param name="provider">The name of the cache provider.</param>
        public static bool ClearCacheFirstOrDefault<T>(this IQueryable<T> query, string group, string provider)
        {
            return query
                .Take(1)
                .ClearCache(group, provider);
        }
    }
}