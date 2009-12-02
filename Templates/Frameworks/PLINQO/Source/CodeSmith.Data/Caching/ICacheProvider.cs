using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeSmith.Data.Linq;

namespace CodeSmith.Data.Caching
{
    public interface ICacheProvider
    {
        void Save<T>(string key, T data, CacheSettings settings);
        void Remove(string key);
        T Get<T>(string key);
    }
}
