using System;
using System.Collections.Generic;

namespace EntityFramework.Future
{
    /// <summary>
    /// An interface defining storage for future queries.
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