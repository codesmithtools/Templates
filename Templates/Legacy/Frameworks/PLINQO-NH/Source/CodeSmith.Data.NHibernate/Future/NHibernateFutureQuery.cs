using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeSmith.Data.Caching;
using CodeSmith.Data.Future;
using NHibernate.Linq;
using CodeSmith.Data.Linq;

namespace CodeSmith.Data.NHibernate
{
    internal class NHibernateFutureQuery<T> : IFutureQuery<T>, INHibernateFutureQuery
    {
        private readonly CacheSettings _cacheSettings;

        private readonly Action _action;

        private readonly IEnumerable<T> _value;

        public NHibernateFutureQuery(IQueryable<T> query, CacheSettings cacheSettings, Action action)
        {
            _action = action;
            _cacheSettings = cacheSettings;
            _value = query.ToFuture();
            Query = query;
        }

        public bool IsLoaded { get; set; }

        public IQueryable Query { get; private set; }

        public IEnumerator<T> GetEnumerator()
        {
            _action.Invoke();
            return _value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Load()
        {
            var values = _value.ToArray();
            
            if (_cacheSettings == null)
                return;

            var key = Query.GetHashKey();
            QueryResultCache.SetResultCache(key, _cacheSettings, values, Query);
        }
    }
}