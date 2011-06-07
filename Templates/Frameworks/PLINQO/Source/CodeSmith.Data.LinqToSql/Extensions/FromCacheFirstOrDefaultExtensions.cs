using System.Data.Linq;
using System.Linq;
using CodeSmith.Data.Caching;

namespace CodeSmith.Data.Linq
{
    public static class FromCacheFirstOrDefaultExtensions
    {
        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// The cache entry has a one minute sliding expiration with normal priority.
        /// Queries, caches, and returns only the first entity.
        /// </summary>
        /// <typeparam name="T">The type of the data in the data source.</typeparam>
        /// <param name="query">The query to be materialized.</param>
        /// <returns>The first or default result of the query.</returns>
        public static T FromCacheFirstOrDefault<T>(this IQueryable<T> query)
        {
            return query
                .Take(1)
                .FromCache()
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// The cache entry has a one minute sliding expiration with normal priority.
        /// Queries, caches, and returns only the first entity.
        /// </summary>
        /// <param name="query">The query to be materialized.</param>
        /// <returns>The first or default result of the query.</returns>
        public static object FromCacheFirstOrDefault(this IQueryable query)
        {
            return query.Cast<object>().FromCacheFirstOrDefault();
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// Queries, caches, and returns only the first entity.
        /// </summary>
        /// <typeparam name="T">The type of the data in the data source.</typeparam>
        /// <param name="query">The query to be materialized.</param>
        /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
        /// <returns>The first or default result of the query.</returns>
        public static T FromCacheFirstOrDefault<T>(this IQueryable<T> query, int duration)
        {
            return query
                .Take(1)
                .FromCache(duration)
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// Queries, caches, and returns only the first entity.
        /// </summary>
        /// <param name="query">The query to be materialized.</param>
        /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
        /// <returns>The first or default result of the query.</returns>
        public static object FromCacheFirstOrDefault(this IQueryable query, int duration)
        {
            return query.Cast<object>().FromCacheFirstOrDefault(duration);
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// Queries, caches, and returns only the first entity.
        /// </summary>
        /// <typeparam name="T">The type of the data in the data source.</typeparam>
        /// <param name="query">The query to be materialized.</param>
        /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
        /// <param name="sqlCacheDependencyTables">The tables for which to add SQL Cache Dependencies</param>
        /// <returns>The first or default result of the query.</returns>
        public static T FromCacheFirstOrDefault<T>(this IQueryable<T> query, int duration,
                                                   params ITable[] sqlCacheDependencyTables)
        {
            CacheSettings cacheSettings = new CacheSettings(duration).AddCacheDependency(query.GetDataContext().Connection.Database, sqlCacheDependencyTables);
            return query
                .Take(1)
                .FromCache(cacheSettings)
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// Queries, caches, and returns only the first entity.
        /// </summary>
        /// <param name="query">The query to be materialized.</param>
        /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
        /// <param name="sqlCacheDependencyTables">The tables for which to add SQL Cache Dependencies</param>
        /// <returns>The first or default result of the query.</returns>
        public static object FromCacheFirstOrDefault(this IQueryable query, int duration,
                                                     params ITable[] sqlCacheDependencyTables)
        {
            return query.Cast<object>().FromCacheFirstOrDefault(duration, sqlCacheDependencyTables);
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// Queries, caches, and returns only the first entity.
        /// </summary>
        /// <typeparam name="T">The type of the data in the data source.</typeparam>
        /// <param name="query">The query to be materialized.</param>
        /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
        /// <param name="sqlCacheDependencyTableNames">The table names for which to add SQL Cache Dependencies</param>
        /// <returns>The first or default result of the query.</returns>
        public static T FromCacheFirstOrDefault<T>(this IQueryable<T> query, int duration,
                                                   params string[] sqlCacheDependencyTableNames)
        {
            CacheSettings cacheSettings = new CacheSettings(duration).AddCacheDependency(query.GetDataContext().Connection.Database, sqlCacheDependencyTableNames);
            return query
                .Take(1)
                .FromCache(cacheSettings)
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// Queries, caches, and returns only the first entity.
        /// </summary>
        /// <param name="query">The query to be materialized.</param>
        /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
        /// <param name="sqlCacheDependencyTableNames">The table names for which to add SQL Cache Dependencies</param>
        /// <returns>The first or default result of the query.</returns>
        public static object FromCacheFirstOrDefault(this IQueryable query, int duration,
                                                     params string[] sqlCacheDependencyTableNames)
        {
            return query.Cast<object>().FromCacheFirstOrDefault(duration, sqlCacheDependencyTableNames);
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// Queries, caches, and returns only the first entity.
        /// </summary>
        /// <typeparam name="T">The type of the data in the data source.</typeparam>
        /// <param name="query">The query to be materialized.</param>
        /// <param name="profileName">Name of the cache profile to use.</param>
        /// <returns>The first or default result of the query.</returns>
        public static T FromCacheFirstOrDefault<T>(this IQueryable<T> query, string profileName)
        {
            return query
                .Take(1)
                .FromCache(profileName)
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// Queries, caches, and returns only the first entity.
        /// </summary>
        /// <param name="query">The query to be materialized.</param>
        /// <param name="profileName">Name of the cache profile to use.</param>
        /// <returns>The first or default result of the query.</returns>
        public static object FromCacheFirstOrDefault(this IQueryable query, string profileName)
        {
            return query.Cast<object>().FromCacheFirstOrDefault(profileName);
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// Queries, caches, and returns only the first entity.
        /// </summary>
        /// <typeparam name="T">The type of the data in the data source.</typeparam>
        /// <param name="query">The query to be materialized.</param>
        /// <param name="profileName">Name of the cache profile to use.</param>
        /// <param name="sqlCacheDependencyTables">The tables for which to add SQL Cache Dependencies</param>
        /// <returns>The first or default result of the query.</returns>
        public static T FromCacheFirstOrDefault<T>(this IQueryable<T> query, string profileName,
                                                   params ITable[] sqlCacheDependencyTables)
        {
            CacheSettings cacheSettings = CacheManager.GetProfile(profileName).AddCacheDependency(query.GetDataContext().Connection.Database, sqlCacheDependencyTables);
            return query
                .Take(1)
                .FromCache(cacheSettings)
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// Queries, caches, and returns only the first entity.
        /// </summary>
        /// <param name="query">The query to be materialized.</param>
        /// <param name="profileName">Name of the cache profile to use.</param>
        /// <param name="sqlCacheDependencyTables">The tables for which to add SQL Cache Dependencies</param>
        /// <returns>The first or default result of the query.</returns>
        public static object FromCacheFirstOrDefault(this IQueryable query, string profileName,
                                                     params ITable[] sqlCacheDependencyTables)
        {
            return query.Cast<object>().FromCacheFirstOrDefault(profileName, sqlCacheDependencyTables);
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// Queries, caches, and returns only the first entity.
        /// </summary>
        /// <typeparam name="T">The type of the data in the data source.</typeparam>
        /// <param name="query">The query to be materialized.</param>
        /// <param name="profileName">Name of the cache profile to use.</param>
        /// <param name="sqlCacheDependencyTableNames">The table names for which to add SQL Cache Dependencies</param>
        /// <returns>The first or default result of the query.</returns>
        public static T FromCacheFirstOrDefault<T>(this IQueryable<T> query, string profileName,
                                                   params string[] sqlCacheDependencyTableNames)
        {
            CacheSettings cacheSettings = CacheManager.GetProfile(profileName).AddCacheDependency(query.GetDataContext().Connection.Database, sqlCacheDependencyTableNames);
            return query
                .Take(1)
                .FromCache(cacheSettings)
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// Queries, caches, and returns only the first entity.
        /// </summary>
        /// <param name="query">The query to be materialized.</param>
        /// <param name="profileName">Name of the cache profile to use.</param>
        /// <param name="sqlCacheDependencyTableNames">The table names for which to add SQL Cache Dependencies</param>
        /// <returns>The first or default result of the query.</returns>
        public static object FromCacheFirstOrDefault(this IQueryable query, string profileName,
                                                     params string[] sqlCacheDependencyTableNames)
        {
            return query.Cast<object>().FromCacheFirstOrDefault(profileName, sqlCacheDependencyTableNames);
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// Queries, caches, and returns only the first entity.
        /// </summary>
        /// <typeparam name="T">The type of the data in the data source.</typeparam>
        /// <param name="query">The query to be materialized.</param>
        /// <param name="settings">Cache settings object.</param>
        /// <returns>The first or default result of the query.</returns>
        public static T FromCacheFirstOrDefault<T>(this IQueryable<T> query, CacheSettings settings)
        {
            return query
                .Take(1)
                .FromCache(settings)
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// Queries, caches, and returns only the first entity.
        /// </summary>
        /// <param name="query">The query to be materialized.</param>
        /// <param name="settings">Cache settings object.</param>
        /// <returns>The first or default result of the query.</returns>
        public static object FromCacheFirstOrDefault(this IQueryable query, CacheSettings settings)
        {
            return query.Cast<object>().FromCacheFirstOrDefault(settings);
        }
    }
}