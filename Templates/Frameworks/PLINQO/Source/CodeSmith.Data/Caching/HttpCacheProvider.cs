using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using CodeSmith.Data.Linq;

namespace CodeSmith.Data.Caching
{
    public class HttpCacheProvider : CacheProvider
    {
        public override void Save<T>(string key, T data, CacheSettings settings)
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
            
            HttpRuntime.Cache.Insert(
               key,
               data,
               settings.CacheDependency,
               absoluteExpiration,
               slidingExpiration,
               settings.Priority,
               settings.CacheItemRemovedCallback);
        }

        public override void Remove(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }

        public override T Get<T>(string key)
        {
            var data = HttpRuntime.Cache.Get(key);
            return data == null ? default(T) : (T)data;
        }
    }
}
