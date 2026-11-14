using System;
using System.Collections;
using CodeSmith.Data.Caching;
using CodeSmith.Data.Linq;
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace CodeSmith.Data.Memcached
{
    public class MemcachedProvider : CacheProvider
    {
        public override void Set<T>(string key, T data, CacheSettings settings)
        {
            if (data == null)
                return;

            object cacheData = data;

            if (!data.GetType().IsPrimitive)
                cacheData = data.ToBinary();

            string groupKey = GetGroupKey(key, settings.Group);

            switch (settings.Mode)
            {
                case CacheExpirationMode.Duration:
                    Client.Store(StoreMode.Set, groupKey, cacheData, DateTime.UtcNow.Add(settings.Duration));
                    break;
                case CacheExpirationMode.Sliding:
                    throw new NotSupportedException("Memcached does not support sliding expirations.");
                case CacheExpirationMode.Absolute:
                    Client.Store(StoreMode.Set, groupKey, cacheData, settings.AbsoluteExpiration);
                    break;
                default:
                    Client.Store(StoreMode.Set, groupKey, cacheData);
                    break;
            }
        }

        public override bool Remove(string key, string group)
        {
            return Client.Remove(GetGroupKey(key, group));
        }

        /// <summary>
        /// Invalidates all items in the cache.
        /// </summary>
        public override void Clear()
        {
            Client.FlushAll();
        }

        public override object Get(string key, string group)
        {
            return Client.Get(GetGroupKey(key, group));
        }

        private static MemcachedClient Client
        {
            get { return Nested.Client; }
        }

        /// <summary>
        /// Nested class to lazy-load singleton.
        /// </summary>
        private class Nested
        {
            internal static readonly MemcachedClient Client = new MemcachedClient();
        }

    }
}
