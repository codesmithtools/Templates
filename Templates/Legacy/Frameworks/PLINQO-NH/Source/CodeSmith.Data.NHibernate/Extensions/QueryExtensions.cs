using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NHibernate.Linq;

namespace CodeSmith.Data.Linq
{
    public static class QueryExtensions
    {
        public static INhFetchRequest<TOriginating,TRelated> Fetch<TOriginating, TRelated>(this IQueryable<TOriginating> query, Expression<Func<TOriginating,TRelated>> relatedObjectSelector)
        {
            return EagerFetchingExtensionMethods.Fetch(query, relatedObjectSelector);
        }

        public static INhFetchRequest<TOriginating,TRelated> FetchMany<TOriginating, TRelated>(this IQueryable<TOriginating> query, Expression<Func<TOriginating,IEnumerable<TRelated>>> relatedObjectSelector)
        {
            return EagerFetchingExtensionMethods.FetchMany(query, relatedObjectSelector);
        }

        public static INhFetchRequest<TQueried,TRelated> ThenFetch<TQueried, TFetch, TRelated>(this INhFetchRequest<TQueried,TFetch> query, Expression<Func<TFetch,TRelated>> relatedObjectSelector)
        {
            return EagerFetchingExtensionMethods.ThenFetch(query, relatedObjectSelector);
        }

        public static INhFetchRequest<TQueried,TRelated> ThenFetchMany<TQueried, TFetch, TRelated>(this INhFetchRequest<TQueried,TFetch> query, Expression<Func<TFetch,IEnumerable<TRelated>>> relatedObjectSelector)
        {
            return EagerFetchingExtensionMethods.ThenFetchMany(query, relatedObjectSelector);
        }
    }
}
