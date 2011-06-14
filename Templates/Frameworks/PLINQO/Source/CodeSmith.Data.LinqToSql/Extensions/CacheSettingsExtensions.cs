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
    public static class CacheSettingsExtensions
    {
        public static CacheSettings AddCacheDependency<T>(this CacheSettings settings, IQueryable<T> queryable, params ITable[] sqlCacheDependencyTables)
        {
            var db = DataContextProvider.GetDataConext(queryable);
            var connectionString = db == null
                                       ? String.Empty
                                       : db.ConnectionString;

            return AddCacheDependency(settings, connectionString, sqlCacheDependencyTables);
        }

        public static CacheSettings AddCacheDependency(this CacheSettings settings, string databaseName, params ITable[] sqlCacheDependencyTables)
        {
            if (sqlCacheDependencyTables == null)
                return settings;

            var cacheDependencies = sqlCacheDependencyTables.Select(t => new SqlCacheDependency(databaseName, t.TableName()));
            return settings.AddCacheDependency(cacheDependencies.Cast<CacheDependency>());
        }
    }
}
