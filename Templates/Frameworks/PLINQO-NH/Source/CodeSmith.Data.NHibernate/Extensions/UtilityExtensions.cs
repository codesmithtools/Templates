using System.Linq;
using System.Reflection;
using CodeSmith.Data.Caching;
using CodeSmith.Data.NHibernate;
using NHibernate.Linq;

namespace CodeSmith.Data.Linq
{
    public static class UtilityExtensions
    {
        public static DataContext GetDataContext(this IQueryable query)
        {
            return NHibernateDataContextProvider.GetDataContext(query);
        }

        public static string GetHashKey(this IQueryable query)
        {
            return QueryResultCache.GetHashKey(query);
        }
    }
}