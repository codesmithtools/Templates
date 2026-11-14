using System.Data.Common;
using System.Data.Linq;
using System.Linq;
using CodeSmith.Data.Future;

namespace CodeSmith.Data.Linq
{
    /// <summary>
    /// Interface for defering the future execution of a batch of queries.
    /// </summary>
    public interface ILinqToSqlFutureQuery : IFutureQuery
    {
        /// <summary>
        /// Gets the data command for this query.
        /// </summary>
        /// <param name="dataContext">The data context to get the command from.</param>
        /// <returns>The requested command object.</returns>
        DbCommand GetCommand(DataContext dataContext);

        /// <summary>
        /// Sets the underling value after the query has been executed.
        /// </summary>
        /// <param name="result">The <see cref="IMultipleResults"/> to get the result from.</param>
        void SetResult(IMultipleResults result);
    }
}