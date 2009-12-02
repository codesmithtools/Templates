using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
using System.Text;

namespace CodeSmith.Data.Caching
{
    public abstract class CacheProvider : ProviderBase, ICacheProvider
    {
        public abstract void Save<T>(string key, T data, CacheSettings settings);
        public abstract void Remove(string key);
        public abstract T Get<T>(string key);
    }
}
