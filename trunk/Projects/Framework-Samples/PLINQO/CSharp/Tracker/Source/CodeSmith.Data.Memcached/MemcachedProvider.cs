using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using CodeSmith.Data.Caching;
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

            string groupKey = GetGroupKey(key, settings.Group);

            switch (settings.Mode)
            {
                case CacheExpirationMode.Duration:
                    Client.Store(StoreMode.Set, groupKey, data, DateTime.UtcNow.Add(settings.Duration));
                    break;
                case CacheExpirationMode.Sliding:
                    Client.Store(StoreMode.Set, groupKey, data, settings.Duration);
                    break;
                case CacheExpirationMode.Absolute:
                    Client.Store(StoreMode.Set, groupKey, data, settings.AbsoluteExpiration);
                    break;
                default:
                    Client.Store(StoreMode.Set, groupKey, data);
                    break;
            }
        }

        public override void Remove(string key)
        {
            Client.Remove(key);
        }

        public override T Get<T>(string key)
        {
            var serializer = new System.Runtime.Serialization.DataContractSerializer(typeof(T));

            object data = Client.Get(key);
            return data == null ? default(T) : (T)data;
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
