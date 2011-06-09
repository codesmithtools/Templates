using System.Collections.Generic;
using System.Linq;

namespace CodeSmith.Data
{
    public static class DataContextProvider
    {
        private static readonly List<IDataContextProvider> _providers = new List<IDataContextProvider>();

        public static void Register(IDataContextProvider provider)
        {
            if (!_providers.Contains(provider))
                _providers.Add(provider);
        }

        public static IDataContext GetDataConext(IQueryable query)
        {
            foreach (var provider in _providers)
            {
                var db = provider.GetDataConext(query);
                if (db != null)
                    return db;
            }

            return null;
        }
    }
}
