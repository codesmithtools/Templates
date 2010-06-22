using System;
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
        }

        /// <summary>
        /// Removes the specified key from the cache provider.
        /// </summary>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="group">The cache group.</param>
        public override bool Remove(string key, string group)
        {
            return HttpRuntime.Cache.Remove(GetGroupKey(key, group)) != null;
        }

        /// <summary>
        /// Gets the data cached for specified key.
        /// </summary>
        /// <typeparam name="T">The type for data being retrieved from cache,</typeparam>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="group">The cache group.</param>
        /// <returns>
        /// An instance of T if the item exists in the cache, otherwise <see langword="null"/>.
        /// </returns>
        public override object Get(string key, string group)
        {
            return HttpRuntime.Cache.Get(GetGroupKey(key, group));
        }
    }
}
