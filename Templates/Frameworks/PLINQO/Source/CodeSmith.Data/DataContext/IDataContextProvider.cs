using System.Linq;

namespace CodeSmith.Data
{
    public interface IDataContextProvider
    {
        IDataContext GetDataConext(IQueryable query);
    }
}