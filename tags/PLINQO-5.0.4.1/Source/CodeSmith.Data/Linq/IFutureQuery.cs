using System.Data.Common;
using System.Data.Linq;
using System.Linq;

namespace CodeSmith.Data.Linq
{
    /// <summary>
    /// Interface for defering the future execution of a batch of queries.
    /// </summary>
    public interface IFutureQuery
    {
        /// <summary>
        /// Gets a value indicating whether this instance is loaded.
        /// </summary>
        /// <value><c>true</c> if this instance is loaded; otherwise, <c>false</c>.</value>
        bool IsLoaded { get; }
        
        /// <summary>
        /// Gets the query source to use when materializing.
        /// </summary>
        /// <value>The query source to use when materializing.</value>
        IQueryable Query { get; }

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