using System;
using System.Diagnostics;
using System.Linq;
using CodeSmith.Data.Caching;
using CodeSmith.Data.Future;
using CodeSmith.Data.Linq;
using NHibernate.Linq;

namespace CodeSmith.Data.NHibernate
{
    internal class NHibernateFutureValue<T> : IFutureValue<T>, INHibernateFutureQuery
    {
        private readonly CacheSettings _cacheSettings;

        private readonly Action _action;

        private readonly global::NHibernate.IFutureValue<T> _value;

        public NHibernateFutureValue(IQueryable<T> query, CacheSettings cacheSettings, Action action)
        {
            _action = action;
            _cacheSettings = cacheSettings;
            _value = query.ToFutureValue();
            Query = query;
        }

        public bool IsLoaded { get; set; }

        public IQueryable Query { get; private set; }
        
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public T Value
        {
            get
            {
                _action.Invoke();
                return _value.Value;
            }
        }
        
        public static implicit operator T(NHibernateFutureValue<T> futureValue)
        {
            return futureValue.Value;
        }

        public void Load()
        {
            var value = _value.Value;

            if (_cacheSettings == null)
                return;

            var key = Query.GetHashKey();
            QueryResultCache.SetResultCache(key, _cacheSettings, new[] { value }, Query);
        }
    }
}