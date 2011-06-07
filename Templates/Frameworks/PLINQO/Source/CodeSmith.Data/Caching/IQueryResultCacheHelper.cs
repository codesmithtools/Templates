using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeSmith.Data.Caching
{
    public interface IQueryResultCacheHelper
    {
        void Detach<T>(ICollection<T> results, IQueryable query);

        string GetDbName(IQueryable query);
    }
}
