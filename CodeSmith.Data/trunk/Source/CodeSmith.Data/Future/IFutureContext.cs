using System.Collections.Generic;
using System.Linq;
using CodeSmith.Data.Caching;

namespace CodeSmith.Data.Future
{
    public interface IFutureContext
    {
        IEnumerable<T> Future<T>(IQueryable<T> query, CacheSettings cacheSettings);

        IFutureValue<T> FutureFirstOrDefault<T>(IQueryable<T> query, CacheSettings cacheSettings);

        IFutureValue<int> FutureCount<T>(IQueryable<T> query, CacheSettings cacheSettings);

        IEnumerable<IFutureQuery> FutureQueries { get; }

        void ExecuteFutureQueries();
    }
}
