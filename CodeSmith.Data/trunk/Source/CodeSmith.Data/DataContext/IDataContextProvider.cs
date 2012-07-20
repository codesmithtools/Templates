using System.Linq;
using CodeSmith.Data.Future;

namespace CodeSmith.Data
{
    public interface IDataContextProvider
    {
        IDataContext GetDataConext(IQueryable query);

        IFutureContext GetFutureContext(IQueryable query);
    }
}