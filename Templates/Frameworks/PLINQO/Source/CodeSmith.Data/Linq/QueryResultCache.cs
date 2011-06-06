using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using CodeSmith.Data.Caching;
using System.Data.Linq;

namespace CodeSmith.Data.Linq
{
    /// <summary>
    /// Extension methods for caching IQuerable objects.
    /// </summary>
    /// <remarks>
    /// <para>Based on the work by Pete Montgomery.</para>
    /// <para>
    /// Copyright (c) 2010 Pete Montgomery.
    /// http://petemontgomery.wordpress.com
    /// Licenced under GNU LGPL v3.
    /// http://www.gnu.org/licenses/lgpl.html
    /// </para>
    /// </remarks>
    public static class QueryableCachingExtensions
    {
        #region FromCache

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
        /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
        /// <param name="sqlCacheDependencyTableNames">The table names for which to add SQL Cache Dependencies</param>
        /// <returns>The result of the query.</returns>
        public static IEnumerable<T> FromCache<T>(this IQueryable<T> query, int duration,
            params string[] sqlCacheDependencyTableNames)
        {
            CacheSettings cacheSettings = new CacheSettings(duration).AddCacheDependency(query.GetDataContext().Connection.Database, sqlCacheDependencyTableNames);
            return query.FromCache(cacheSettings);
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// </summary>
        /// <param name="query">The query to be materialized.</param>
        /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
        /// <param name="sqlCacheDependencyTableNames">The table names for which to add SQL Cache Dependencies</param>
        /// <returns>The result of the query.</returns>
        public static IEnumerable<object> FromCache(this IQueryable query, int duration,
            params string[] sqlCacheDependencyTableNames)
        {
            return query.Cast<object>().FromCache(duration, sqlCacheDependencyTableNames);
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// </summary>
        /// <typeparam name="T">The type of the data in the data source.</typeparam>
        /// <param name="query">The query to be materialized.</param>
        /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
        /// <param name="sqlCacheDependencyTables">The tables for which to add SQL Cache Dependencies</param>
        /// <returns>The result of the query.</returns>
        public static IEnumerable<T> FromCache<T>(this IQueryable<T> query, int duration,
            params ITable[] sqlCacheDependencyTables)
        {
            CacheSettings cacheSettings = new CacheSettings(duration).AddCacheDependency(query, sqlCacheDependencyTables);
            return query.FromCache(cacheSettings);
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// </summary>
        /// <param name="query">The query to be materialized.</param>
        /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
        /// <param name="sqlCacheDependencyTables">The tables for which to add SQL Cache Dependencies</param>
        /// <returns>The result of the query.</returns>
        public static IEnumerable<object> FromCache(this IQueryable query, int duration,
            params ITable[] sqlCacheDependencyTables)
        {
            return query.Cast<object>().FromCache(duration, sqlCacheDependencyTables);
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
        /// <param name="profileName">Name of the cache profile to use.</param>
        /// <param name="sqlCacheDependencyTables">The tables for which to add SQL Cache Dependencies</param>
        /// <returns>The result of the query.</returns>
        public static IEnumerable<T> FromCache<T>(this IQueryable<T> query, string profileName,
            params ITable[] sqlCacheDependencyTables)
        {
            CacheSettings cacheSettings = CacheManager.GetProfile(profileName).AddCacheDependency(query.GetDataContext().Connection.Database, sqlCacheDependencyTables);
            return query.FromCache(cacheSettings);
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// </summary>
        /// <param name="query">The query to be materialized.</param>
        /// <param name="profileName">Name of the cache profile to use.</param>
        /// <param name="sqlCacheDependencyTables">The tables for which to add SQL Cache Dependencies</param>
        /// <returns>The result of the query.</returns>
        public static IEnumerable<object> FromCache(this IQueryable query, string profileName,
            params ITable[] sqlCacheDependencyTables)
        {
            return query.Cast<object>().FromCache(profileName, sqlCacheDependencyTables);
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// </summary>
        /// <typeparam name="T">The type of the data in the data source.</typeparam>
        /// <param name="query">The query to be materialized.</param>
        /// <param name="profileName">Name of the cache profile to use.</param>
        /// <param name="sqlCacheDependencyTableNames">The table names for which to add SQL Cache Dependencies</param>
        /// <returns>The result of the query.</returns>
        public static IEnumerable<T> FromCache<T>(this IQueryable<T> query, string profileName,
            params string[] sqlCacheDependencyTableNames)
        {
            CacheSettings cacheSettings = CacheManager.GetProfile(profileName).AddCacheDependency(query.GetDataContext().Connection.Database, sqlCacheDependencyTableNames);
            return query.FromCache(cacheSettings);
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// </summary>
        /// <param name="query">The query to be materialized.</param>
        /// <param name="profileName">Name of the cache profile to use.</param>
        /// <param name="sqlCacheDependencyTableNames">The table names for which to add SQL Cache Dependencies</param>
        /// <returns>The result of the query.</returns>
        public static IEnumerable<object> FromCache(this IQueryable query, string profileName,
            params string[] sqlCacheDependencyTableNames)
        {
            return query.Cast<object>().FromCache(profileName, sqlCacheDependencyTableNames);
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

        #endregion

        #region FromCacheFirstOrDefault

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

        #endregion

        #region ClearCache

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

        #endregion

        #region ClearCacheFirstOrDefault

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

        #endregion

        #region GetHashKey

        public static string GetHashKey(this IQueryable query)
        {
            return QueryResultCache.GetHashKey(query);
        }

        #endregion
    }
}