using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web.Caching;

namespace CodeSmith.Data.Caching
{
    public static class CacheSettingsExtensions
    {
        public static CacheSettings WithDuration(this CacheSettings settings, int duration)
        {
            settings.Duration = TimeSpan.FromSeconds(duration);
            settings.Mode = CacheExpirationMode.Duration;
            return settings;
        }

        public static CacheSettings WithDuration(this CacheSettings settings, TimeSpan duration)
        {
            settings.Duration = duration;
            settings.Mode = CacheExpirationMode.Duration;
            return settings;
        }

        public static CacheSettings WithAbsoluteExpiration(this CacheSettings settings, DateTime absoluteExpiration)
        {
            settings.AbsoluteExpiration = absoluteExpiration;
            settings.Mode = CacheExpirationMode.Absolute;
            return settings;
        }

        public static CacheSettings WithPriority(this CacheSettings settings, CacheItemPriority priority)
        {
            settings.Priority = priority;
            return settings;
        }

        public static CacheSettings WithCacheEmptyResult(this CacheSettings settings, bool cacheEmptyResult)
        {
            settings.CacheEmptyResult = cacheEmptyResult;
            return settings;
        }

        public static CacheSettings WithCacheDependency(this CacheSettings settings, CacheDependency cacheDependency)
        {
            settings.CacheDependency = cacheDependency;
            return settings;
        }

        public static CacheSettings AddCacheDependency(this CacheSettings settings, CacheDependency cacheDependency)
        {
            if (settings.CacheDependency == null)
                settings.CacheDependency = cacheDependency;
            else if (settings.CacheDependency is AggregateCacheDependency)
                (settings.CacheDependency as AggregateCacheDependency).Add(cacheDependency);
            else
            {
                var dependencies = new AggregateCacheDependency();
                dependencies.Add(settings.CacheDependency, cacheDependency);
                settings.CacheDependency = dependencies;
            }
            return settings;
        }

        public static CacheSettings AddCacheDependency(this CacheSettings settings, params CacheDependency[] cacheDependencies)
        {
            return AddCacheDependency(settings, (IEnumerable<CacheDependency>)cacheDependencies);
        }

        public static CacheSettings AddCacheDependency(this CacheSettings settings, IEnumerable<CacheDependency> cacheDependencies)
        {
            if (cacheDependencies != null)
            {
                if (settings.CacheDependency is AggregateCacheDependency)
                {
                    var agregateDependency = settings.CacheDependency as AggregateCacheDependency;
                    foreach (var dependency in cacheDependencies)
                        agregateDependency.Add(dependency);
                }
                else
                {
                    var agregateDependency = new AggregateCacheDependency();
                    if (settings.CacheDependency != null)
                        agregateDependency.Add(settings.CacheDependency);
                    
                    foreach (var dependency in cacheDependencies)
                        agregateDependency.Add(dependency);
                    
                    settings.CacheDependency = agregateDependency;
                }
            }
            return settings;
        }

        public static CacheSettings AddCacheDependency(this CacheSettings settings, params SqlCacheDependency[] cacheDependencies)
        {
            return AddCacheDependency(settings, cacheDependencies.Cast<CacheDependency>());
        }

        public static CacheSettings AddCacheDependency(this CacheSettings settings, string databaseName, params string[] sqlCacheDependencyTableNames)
        {
            if (sqlCacheDependencyTableNames == null)
                return settings;
            
            var cacheDependencies = sqlCacheDependencyTableNames.Select(t => new SqlCacheDependency(databaseName, t));
            return AddCacheDependency(settings, cacheDependencies.Cast<CacheDependency>());
        }

        public static CacheSettings AddCacheDependency<T>(this CacheSettings settings, IQueryable<T> queryable, params string[] sqlCacheDependencyTableNames)
        {
            var db = DataContextProvider.GetDataConext(queryable);
            var connectionString = db == null
                                       ? String.Empty
                                       : db.ConnectionString;

            return AddCacheDependency(settings, connectionString, sqlCacheDependencyTableNames);
        }

        public static CacheSettings WithCacheItemRemovedCallback(this CacheSettings settings, CacheItemRemovedCallback cacheItemRemovedCallback)
        {
            settings.CacheItemRemovedCallback = cacheItemRemovedCallback;
            return settings;
        }

        public static CacheSettings WithProvider(this CacheSettings settings, string provider)
        {
            settings.Provider = provider;
            return settings;
        }

        public static CacheSettings WithGroup(this CacheSettings settings, string group)
        {
            settings.Group = group;
            return settings;
        }
    }
}