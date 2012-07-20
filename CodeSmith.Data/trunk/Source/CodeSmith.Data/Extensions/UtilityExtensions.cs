using System.Linq;
using CodeSmith.Data.Caching;
using CodeSmith.Data.Future;

namespace CodeSmith.Data.Linq
{
    public static class UtilityExtensions
    {
        public static bool SupportsFutureQuery(this IQueryable query)
        {
            var db = query.GetFutureConext();
            return db != null;
        }

        public static IFutureContext GetFutureConext(this IQueryable query)
        {
            return DataContextProvider.GetFutureConext(query);
        }

        public static IDataContext GetDataContext(this IQueryable query)
        {
            return DataContextProvider.GetDataConext(query);
        }

        public static string GetHashKey(this IQueryable query)
        {
            return QueryResultCache.GetHashKey(query);
        }
    }
}