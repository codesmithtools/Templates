using System.Linq;
using CodeSmith.Data.Caching;

namespace CodeSmith.Data.Linq
{
    public static class UtilityExtensions
    {
        public static string GetHashKey(this IQueryable query)
        {
            return QueryResultCache.GetHashKey(query);
        }
    }
}