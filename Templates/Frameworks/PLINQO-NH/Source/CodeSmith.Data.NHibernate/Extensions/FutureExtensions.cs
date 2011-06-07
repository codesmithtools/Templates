using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;

namespace CodeSmith.Data.Linq
{
    public static class FutureExtensions
    {
        public static IEnumerable<T> Future<T>(this IQueryable<T> query)
        {
            return query.ToFuture();
        }

        public static IFutureValue<T> FutureFirstOrDefault<T>(this IQueryable<T> query)
        {
            return query.ToFutureValue();
        }
    }
}
