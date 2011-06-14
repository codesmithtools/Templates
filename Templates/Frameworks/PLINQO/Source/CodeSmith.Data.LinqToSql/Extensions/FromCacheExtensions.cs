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
using CodeSmith.Data.LinqToSql;

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
        /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
        /// <param name="sqlCacheDependencyTableNames">The table names for which to add SQL Cache Dependencies</param>
        /// <returns>The result of the query.</returns>
        public static IEnumerable<T> FromCache<T>(this IQueryable<T> query, int duration,
            params string[] sqlCacheDependencyTableNames)
        {
            var db = DataContextProvider.GetDataConext(query);
            var connectionString = db == null
                                       ? String.Empty
                                       : db.ConnectionString;

            CacheSettings cacheSettings = new CacheSettings(duration).AddCacheDependency(connectionString, sqlCacheDependencyTableNames);
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
        /// <param name="sqlCacheDependencyTables">The tables for which to add SQL Cache Dependencies</param>
        /// <returns>The result of the query.</returns>
        public static IEnumerable<T> FromCache<T>(this IQueryable<T> query, string profileName,
            params ITable[] sqlCacheDependencyTables)
        {
            var db = LinqToSqlDataContextProvider.GetDataContext(query);

            var cacheSettings = CacheManager
                .GetProfile(profileName)
                .AddCacheDependency(db.Connection.Database, sqlCacheDependencyTables);

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
            var db = LinqToSqlDataContextProvider.GetDataContext(query);

            var cacheSettings = CacheManager
                .GetProfile(profileName)
                .AddCacheDependency(db.Connection.Database, sqlCacheDependencyTableNames);
            
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
    }
}