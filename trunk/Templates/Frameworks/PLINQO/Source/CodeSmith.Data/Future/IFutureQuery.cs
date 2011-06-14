using System.Collections.Generic;
using System.Linq;

namespace CodeSmith.Data.Future
{
    public interface IFutureQuery<out T> : IFutureQuery, IEnumerable<T>
    {
    }

    public interface IFutureQuery
    {
        bool IsLoaded { get; }

        IQueryable Query { get; }
    }
}