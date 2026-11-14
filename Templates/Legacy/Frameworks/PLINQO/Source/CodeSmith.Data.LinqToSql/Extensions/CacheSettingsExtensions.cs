using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Web.Caching;
using CodeSmith.Data.Caching;
using CodeSmith.Data.Linq;

namespace CodeSmith.Data.LinqToSql
{
    /// <summary>
    /// Cache Settings Extension Methods.
    /// </summary>
    public static class CacheSettingsExtensions
    {
        /// <summary>
        /// Adds a Cache Dependency.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="settings"></param>
        /// <param name="queryable"></param>
        /// <param name="sqlCacheDependencyTables"></param>
        /// <returns></returns>
        public static CacheSettings AddCacheDependency<T>(this CacheSettings settings, IQueryable<T> queryable, params ITable[] sqlCacheDependencyTables)
        {
            var db = DataContextProvider.GetDataConext(queryable);
            var connectionString = db == null
                                       ? String.Empty
                                       : db.ConnectionString;

            return AddCacheDependency(settings, connectionString, sqlCacheDependencyTables);
        }

        /// <summary>
        /// Adds a Cache Dependency.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="databaseName"></param>
        /// <param name="sqlCacheDependencyTables"></param>
        /// <returns></returns>
        public static CacheSettings AddCacheDependency(this CacheSettings settings, string databaseName, params ITable[] sqlCacheDependencyTables)
        {
            if (sqlCacheDependencyTables == null)
                return settings;

            var cacheDependencies = sqlCacheDependencyTables.Select(t => new SqlCacheDependency(databaseName, t.TableName()));
            return settings.AddCacheDependency(cacheDependencies.Cast<CacheDependency>());
        }
    }
}
