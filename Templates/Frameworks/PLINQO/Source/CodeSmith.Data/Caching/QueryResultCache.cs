using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using CodeSmith.Data.Linq;

namespace CodeSmith.Data.Caching
{
    public static class QueryResultCache
    {
        public static IEnumerable<T> FromCache<T>(IQueryable<T> query, CacheSettings settings)
        {
            if (settings == null)
                settings = CacheManager.GetProfile();

            var key = GetHashKey(query);

            // try to get the query result from the cache
            var result = GetResultCache<T>(key, settings);

            if (result != null)
                return result;

            // materialize the query
            result = query.ToList();

            SetResultCache(key, settings, result, query);

            return result;
        }

        public static string GetHashKey(IQueryable query)
        {
            // locally evaluate as much of the query as possible
            var expression = Evaluator.PartialEval(
                query.Expression,
                CanBeEvaluatedLocally);

            // use the string representation of the query for the cache key
            var key = expression.ToString();

            // make key DB specific
            var info = GetConnectionUniqueInfo(query);
            if (!String.IsNullOrEmpty(info))
                key += "||" + info;

            // the key is potentially very long, so use an md5 fingerprint
            // (fine if the query result data isn't critically sensitive)
            return ToMd5Fingerprint(key);
        }

        public static void SetResultCache<T>(string key, CacheSettings settings, ICollection<T> results)
        {
            SetResultCache(key, settings, results, null);
        }

        public static void SetResultCache<T>(string key, CacheSettings settings, ICollection<T> results, IQueryable query)
        {
            if (settings == null)
                settings = CacheManager.GetProfile();

            if (results == null)
                return;

            // Don't cache empty result.
            if (results.Count == 0 && !settings.CacheEmptyResult)
                return;

            // detach for cache
            Detach(results, query);

            ICacheProvider cacheProvider = CacheManager.GetProvider(settings.Provider);
            cacheProvider.Set(key, results, settings);
        }

        public static ICollection<T> GetResultCache<T>(string key, CacheSettings settings)
        {
            if (settings == null)
                settings = CacheManager.GetProfile();

            ICacheProvider cacheProvider = CacheManager.GetProvider(settings.Provider);
            var collection = cacheProvider.Get<ICollection<T>>(key, settings.Group);
            return collection;
        }

        private static void Detach<T>(ICollection<T> results, IQueryable query)
        {
            var db = DataContextProvider.GetDataConext(query);
            if (db == null)
                return;

            db.Detach(results);
        }

        private static readonly ConcurrentDictionary<string, string> _uniqueInfos = new ConcurrentDictionary<string, string>();
        private static string GetConnectionUniqueInfo(IQueryable query)
        {
            var db = DataContextProvider.GetDataConext(query);
            string connectionString = db == null
                ? String.Empty
                : db.ConnectionString;

            if (String.IsNullOrEmpty(connectionString))
                return "NA";

            // cache so that we aren't parsing config string every single time.
            return _uniqueInfos.GetOrAdd(connectionString, s => {
                var sb = new StringBuilder();

                var config = ParseConfigValues(connectionString);
                if (config.ContainsKey("database"))
                    sb.Append(config["database"]);

                if (config.ContainsKey("initial catalog"))
                    sb.Append(config["initial catalog"]);

                if (config.ContainsKey("data source"))
                    sb.Append(config["data source"]);

                if (config.ContainsKey("server"))
                    sb.Append(config["server"]);

                if (config.ContainsKey("hostname"))
                    sb.Append(config["hostname"]);

                if (sb.Length == 0)
                    sb.Append("NA");

                return sb.ToString();
            });
        }

        private static Dictionary<string, string> ParseConfigValues(string value) {
            var attributes = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            string[] keyValuePairs = value.Split(';');
            foreach (string t in keyValuePairs) {
                string[] keyValuePair = t.Split('=');
                if (keyValuePair.Length == 2)
                    attributes.Add(keyValuePair[0].Trim(), keyValuePair[1].Trim());
                else
                    attributes.Add(t.Trim(), null);
            }

            return attributes;
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
    }
}
