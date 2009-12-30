using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace CodeSmith.Data.Linq
{
    /// <summary>
    /// An interface for future queries from a <see cref="DataContext"/>.
    /// </summary>
    public interface IFutureContext
    {
        /// <summary>
        /// Gets the future queries.
        /// </summary>
        /// <value>The future queries.</value>
        IList<IFutureQuery> FutureQueries { get; }               

        /// <summary>
        /// Executes the future queries.
        /// </summary>
        void ExecuteFutureQueries();
    }
}