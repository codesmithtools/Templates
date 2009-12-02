using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
using System.Text;

namespace CodeSmith.Data.Caching
{
    public class CacheProviderCollection : ProviderCollection
    {
        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (!(provider is ICacheProvider))
                throw new ArgumentException("Provider must implement ICacheProvider.");
            base.Add(provider);
        }
    }
}
