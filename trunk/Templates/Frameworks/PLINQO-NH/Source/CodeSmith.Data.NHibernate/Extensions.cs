using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using CodeSmith.Data.Caching;
using CodeSmith.Data.Linq;
using NHibernate;
using NHibernate.Linq;

namespace Plinqo.NHibernate
{
    public static class Extensions
    {
        #region Future

        public static IEnumerable<T> Future<T>(this IQueryable<T> query)
        {
            return query.ToFuture();
        }

        public static IFutureValue<T> FutureFirstOrDefault<T>(this IQueryable<T> query)
        {
            return query.ToFutureValue();
        }

        #endregion

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

        #region FutureCache

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

            return cache ?? source.Future();
        }

        #endregion

        #region FutureCacheFirstOrDefault

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source"/> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to add to the batch of future queries.</param>
        /// <returns>
        /// An instance of <see cref="T:CodeSmith.Data.Linq.FutureValue`1"/> that contains the result of the query.
        /// </returns>
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureValue`1"/>
        public static IFutureValue<T> FutureCacheFirstOrDefault<T>(this IQueryable<T> source)
        {
            var cacheSettings = CacheManager.GetProfile();
            return FutureCacheFirstOrDefault(source, cacheSettings);
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source"/> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to add to the batch of future queries.</param>
        /// <param name="profileName">Name of the cache profile to use.</param>
        /// <returns>
        /// An instance of <see cref="T:CodeSmith.Data.Linq.FutureValue`1"/> that contains the result of the query.
        /// </returns>
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureValue`1"/>
        public static IFutureValue<T> FutureCacheFirstOrDefault<T>(this IQueryable<T> source, string profileName)
        {
            CacheSettings cacheSettings = CacheManager.GetProfile(profileName);
            return FutureCacheFirstOrDefault(source, cacheSettings);
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source"/> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to add to the batch of future queries.</param>
        /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
        /// <returns>
        /// An instance of <see cref="T:CodeSmith.Data.Linq.FutureValue`1"/> that contains the result of the query.
        /// </returns>
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureValue`1"/>
        public static IFutureValue<T> FutureCacheFirstOrDefault<T>(this IQueryable<T> source, int duration)
        {
            return FutureCacheFirstOrDefault(source, new CacheSettings(duration));
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source"/> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to add to the batch of future queries.</param>
        /// <param name="cacheSettings">The cache settings.</param>
        /// <returns>
        /// An instance of <see cref="T:CodeSmith.Data.Linq.FutureValue`1"/> that contains the result of the query.
        /// </returns>
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureValue`1"/>
        public static IFutureValue<T> FutureCacheFirstOrDefault<T>(this IQueryable<T> source, CacheSettings cacheSettings)
        {
            if (source == null)
                return null;

            var key = source.GetHashKey();
            var cache = QueryResultCache.GetResultCache<T>(key, cacheSettings);

            return cache == null
                ? source.FutureFirstOrDefault()
                : new FutureValue<T>(cache.FirstOrDefault());
        }

        #endregion

        #region FutureCount

        /*/// <summary>
        /// Provides for defering the execution of the <paramref name="source" /> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1" /> to add to the batch of future queries.</param>
        /// <returns>An instance of <see cref="FutureCount"/> that contains the result of the query.</returns>
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureCount"/>
        public static FutureCount FutureCount<T>(this IQueryable<T> source)
        {
            return FutureCacheCount(source, (CacheSettings)null);
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source"/> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to add to the batch of future queries.</param>
        /// <returns>
        /// An instance of <see cref="FutureCount"/> that contains the result of the query.
        /// </returns>
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureCount"/>
        public static FutureCount FutureCacheCount<T>(this IQueryable<T> source)
        {
            var cacheSettings = CacheManager.GetProfile();
            return FutureCacheCount(source, cacheSettings);
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source"/> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to add to the batch of future queries.</param>
        /// <param name="profileName">Name of the cache profile to use.</param>
        /// <returns>
        /// An instance of <see cref="FutureCount"/> that contains the result of the query.
        /// </returns>
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureCount"/>
        public static FutureCount FutureCacheCount<T>(this IQueryable<T> source, string profileName)
        {
            CacheSettings cacheSettings = CacheManager.GetProfile(profileName);
            return FutureCacheCount(source, cacheSettings);
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source"/> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to add to the batch of future queries.</param>
        /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
        /// <returns>
        /// An instance of <see cref="FutureCount"/> that contains the result of the query.
        /// </returns>
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureCount"/>
        public static FutureCount FutureCacheCount<T>(this IQueryable<T> source, int duration)
        {
            return FutureCacheCount(source, new CacheSettings(duration));
        }

        /// <summary>
        /// Provides for defering the execution of the <paramref name="source"/> query to a batch of future queries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="T:System.Linq.IQueryable`1"/> to add to the batch of future queries.</param>
        /// <param name="cacheSettings">The cache settings.</param>
        /// <returns>
        /// An instance of <see cref="FutureCount"/> that contains the result of the query.
        /// </returns>
        /// <seealso cref="T:CodeSmith.Data.Linq.FutureCount"/>
        public static FutureCount FutureCacheCount<T>(this IQueryable<T> source, CacheSettings cacheSettings)
        {
            if (source == null)
                return new FutureCount(0);

            IFutureContext db = GetFutureContext(source);
            var future = new FutureCount(source, db.ExecuteFutureQueries, cacheSettings);
            db.FutureQueries.Add(future);
            return future;
        }*/

        #endregion

        #region Utilities

        private static FieldInfo _sessionField = null;

        private static FieldInfo SessionField
        {
            get
            {
                if (_sessionField == null)
                {
                    var type = typeof (NhQueryProvider);
                    _sessionField = type.GetField("_session", BindingFlags.NonPublic | BindingFlags.Instance);
                }

                return _sessionField;
            }
        }

        public static DataContext GetDataContext(this IQueryable query)
        {
            var provider = query.Provider as NhQueryProvider;
            if (provider == null)
                return null;

            var session = SessionField.GetValue(provider);
            if (session == null)
                return null;

            return DataContext.GetBySession(session);
        }

        public static string GetHashKey(this IQueryable query)
        {
            return QueryResultCache.GetHashKey(query);
        }

        #endregion
    }

    public class NHibernateQueryResultCacheHelper : IQueryResultCacheHelper
    {
        public string GetDbName(IQueryable query)
        {
            var db = query.GetDataContext();

            return db == null
                ? String.Empty
                : db.DatabaseName;
        }

        public void Detach<T>(ICollection<T> results, IQueryable query)
        {
            var db = query.GetDataContext();

            if (db == null)
                return;

            if (!db.ObjectTrackingEnabled || !db.Advanced.HasSession)
                return;

            try
            {
                foreach (var result in results)
                    db.Advanced.Session.Evict(result);
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
            // ReSharper restore EmptyGeneralCatchClause
            {
            }
        }
    }

    internal class FutureValue<T> : IFutureValue<T>
    {
        private readonly T _value;

        public FutureValue(T value)
        {
            _value = value;
        }

        public T Value
        {
            get { return _value; }
        }
    }
}
