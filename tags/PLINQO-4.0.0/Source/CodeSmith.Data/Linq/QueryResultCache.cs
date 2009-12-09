using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using CodeSmith.Data.Caching;

namespace CodeSmith.Data.Linq
{
    /// <summary>
    /// Extension methods for caching IQuerable objects.
    /// </summary>
    public static class QueryResultCache
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
            CacheSettings cacheSettings = CacheManager.Current.DefaultProfile ?? new CacheSettings();
            return query.FromCache(cacheSettings);
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
        /// <typeparam name="T">The type of the data in the data source.</typeparam>
        /// <param name="query">The query to be materialized.</param>
        /// <param name="profileName">Name of the cache profile to use.</param>
        /// <returns>The result of the query.</returns>
        public static IEnumerable<T> FromCache<T>(this IQueryable<T> query, string profileName)
        {
            CacheSettings cacheSettings = CacheManager.Current.GetProfile(profileName);            
            return query.FromCache(cacheSettings);
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
            var key = query.GetHashKey();

            // try to get the query result from the cache
            var result = GetResultCache<T>(key, settings.Provider);

            if (result != null)
                return result;

            // materialize the query
            result = query.ToList();

            SetResultCache(key, settings, result);

            return result;
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

        #endregion

        #region ClearCache

        /// <summary>
        /// Clears the cache of a given query.
        /// </summary>
        /// <typeparam name="T">The type of the data in the data source.</typeparam>
        /// <param name="query">The query to be cleared.</param>
        public static void ClearCache<T>(this IQueryable<T> query)
        {
            ClearCache(query, null);
        }

        /// <summary>
        /// Clears the cache of a given query.
        /// </summary>
        /// <typeparam name="T">The type of the data in the data source.</typeparam>
        /// <param name="query">The query to be cleared.</param>
        /// <param name="provider">The name of the cache provider.</param>
        public static void ClearCache<T>(this IQueryable<T> query, string provider)
        {
            ICacheProvider cacheProvider = CacheManager.Current.GetProvider(provider);
            string key = query.GetHashKey();
            cacheProvider.Remove(key);            
        }

        #endregion

        internal static void SetResultCache<T>(string key, CacheSettings settings, ICollection<T> result)
        {
            // Don't cache empty result.
            if (result.Count == 0 && !settings.CacheEmptyResult)
                return;

            //detach for cache
            foreach (var item in result)
            {
                var entity = item as ILinqEntity;
                if (entity != null)
                    entity.Detach();
            }
#if DEBUG
            Debug.WriteLine("Cache Insert for key " + key);
#endif
            ICacheProvider provider = CacheManager.Current.GetProvider(settings.Provider);
            provider.Save(key, result, settings);
        }

        internal static ICollection<T> GetResultCache<T>(string key, string provider)
        {
            ICacheProvider cacheProvider = CacheManager.Current.GetProvider(provider);
            var collection = cacheProvider.Get<ICollection<T>>(key);

#if DEBUG
            if (collection != null)
                Debug.WriteLine("Cache Hit for key " + key);
#endif
            return collection;
        }

        #region GetHashKey

        /// <summary>
        /// Gets a unique Md5 key for a query.
        /// </summary>
        /// <param name="query">The query to build a key from.</param>
        /// <returns>A Md5 hash unique to the query.</returns>
        public static string GetHashKey(this IQueryable query)
        {
            // locally evaluate as much of the query as possible
            Expression expression = Evaluator.PartialEval(
                query.Expression,
                CanBeEvaluatedLocally);

            // use the string representation of the query for the cache key
            string key = expression.ToString();

            // the key is potentially very long, so use an md5 fingerprint
            // (fine if the query result data isn't critically sensitive)
            return ToMd5Fingerprint(key);
        }

        private static Func<Expression, bool> CanBeEvaluatedLocally
        {
            get
            {
                return expression =>
                {
                    // don't evaluate parameters
                    if (expression.NodeType == ExpressionType.Parameter)
                        return false;

                    // can't evaluate queries
                    if (typeof(IQueryable).IsAssignableFrom(expression.Type))
                        return false;

                    return true;
                };
            }
        }

        private static string ToMd5Fingerprint(string s)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(s.ToCharArray());
            byte[] hash = new MD5CryptoServiceProvider().ComputeHash(bytes);

            // concat the hash bytes into one long string
            return hash.Aggregate(new StringBuilder(32),
                (sb, b) => sb.Append(b.ToString("X2")))
                .ToString();
        }

        #endregion
    }
}