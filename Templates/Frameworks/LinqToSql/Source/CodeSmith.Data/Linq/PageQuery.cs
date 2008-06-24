using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Linq
{
    public static class PageQuery
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int page, int pageSize)
        {
            int skip = Math.Max(pageSize * (page - 1), 0);
            return query.Skip(skip).Take(pageSize);
        }
    }
}
