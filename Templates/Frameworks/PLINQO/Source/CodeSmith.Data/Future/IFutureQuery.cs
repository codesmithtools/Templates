using System.Collections.Generic;
using System.Linq;

namespace CodeSmith.Data.Future
{
#if v40
    // http://blogs.msdn.com/b/csharpfaq/archive/2010/02/16/covariance-and-contravariance-faq.aspx
    public interface IFutureQuery<out T> : IFutureQuery, IEnumerable<T>
#else
    public interface IFutureQuery<T> : IFutureQuery, IEnumerable<T>
#endif
    {
    }

    public interface IFutureQuery
    {
        bool IsLoaded { get; }

        IQueryable Query { get; }
    }
}