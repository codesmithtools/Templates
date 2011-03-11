using System;
using System.Collections;
using System.Diagnostics;
using System.Web;

namespace CodeSmith.Data.Caching
{
    /// <summary>
    /// The default cache provider using System.Web.Cache to as the cache store.
    /// </summary>
    public class HttpCacheProvider : CacheProvider
    {
        /// <summary>
        /// Saves the specified key to the cache provider.
        /// </summary>
        /// <typeparam name="T">The type for data being saved,</typeparam>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="data">The data to be cached in the provider.</param>
        /// <param name="settings">The <see cref="CacheSettings"/> to be used when storing in the provider.</param>
        public override void Set<T>(string key, T data, CacheSettings settings)
        {
            DateTime absoluteExpiration = System.Web.Caching.Cache.NoAbsoluteExpiration;
            TimeSpan slidingExpiration = System.Web.Caching.Cache.NoSlidingExpiration;

            switch (settings.Mode)
            {
                case CacheExpirationMode.Duration:
                    absoluteExpiration = DateTime.UtcNow.Add(settings.Duration);
                    break;
                case CacheExpirationMode.Sliding:
                    slidingExpiration = settings.Duration;
                    break;
                case CacheExpirationMode.Absolute:
                    absoluteExpiration = settings.AbsoluteExpiration;
                    break;
            }

            string groupKey = GetGroupKey(key, settings.Group);

            HttpRuntime.Cache.Insert(
               groupKey,
               data,
               settings.CacheDependency,
               absoluteExpiration,
               slidingExpiration,
               settings.Priority,
               settings.CacheItemRemovedCallback);

#if DEBUG
            if (!groupKey.StartsWith(GroupVersionPrefix))
                Debug.WriteLine("Cache Insert for key " + groupKey);
#endif

        }

        /// <summary>
        /// Removes the specified key from the cache provider.
        /// </summary>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="group">The cache group.</param>
        public override bool Remove(string key, string group)
        {
            var groupKey = GetGroupKey(key, group);
            bool result = HttpRuntime.Cache.Remove(groupKey) != null;
#if DEBUG
            if (result && !groupKey.StartsWith(GroupVersionPrefix))
                Debug.WriteLine("Cache Remove for key " + groupKey);
#endif
            return result;
        }

        /// <summary>
        /// Invalidates all cache items.
        /// </summary>
        public override void Clear()
        {
            var cacheEnum = HttpRuntime.Cache.GetEnumerator();
            while (cacheEnum.MoveNext())
            {
                var key = cacheEnum.Key.ToString();
                HttpRuntime.Cache.Remove(key);
            }
        }

        /// <summary>
        /// Gets the data cached for specified key.
        /// </summary>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="group">The cache group.</param>
        /// <returns>
        /// An instance of T if the item exists in the cache, otherwise <see langword="null"/>.
        /// </returns>
        public override object Get(string key, string group)
        {
            var groupKey = GetGroupKey(key, group);
            var value = HttpRuntime.Cache.Get(groupKey);

#if DEBUG
            if (value != null && !groupKey.StartsWith(GroupVersionPrefix))
                Debug.WriteLine("Cache Hit for key " + groupKey);
#endif

            return value;
        }
    }
}
