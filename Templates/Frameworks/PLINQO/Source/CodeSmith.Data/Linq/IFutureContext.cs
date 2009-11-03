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
        /// Adds the query to the <see cref="FutureQueries"/> queue.
        /// </summary>
        /// <typeparam name="T">The type of the query.</typeparam>
        /// <param name="query">The query as the source.</param>
        /// <returns>An instance of <see cref="T:CodeSmith.Data.Linq.FutureQuery`1"/> to use to access the query later.</returns>
        FutureQuery<T> AddFutureQuery<T>(IQueryable<T> query);

        /// <summary>
        /// Adds the query to the <see cref="FutureQueries"/> queue.
        /// </summary>
        /// <typeparam name="T">The type of the query.</typeparam>
        /// <param name="query">The query as the source.</param>
        /// <returns>An instance of <see cref="T:CodeSmith.Data.Linq.FutureValue`1"/> to use to access the query later.</returns>
        FutureValue<T> AddFutureValueQuery<T>(IQueryable<T> query);

        /// <summary>
        /// Adds the query to the <see cref="FutureQueries"/> queue.
        /// </summary>
        /// <typeparam name="T">The type of the query.</typeparam>
        /// <param name="query">The query as the source.</param>
        /// <returns>An instance of <see cref="T:CodeSmith.Data.Linq.FutureCount"/> to use to access the query later.</returns>
        FutureCount AddFutureCountQuery<T>(IQueryable<T> query);

        /// <summary>
        /// Executes the future queries.
        /// </summary>
        void ExecuteFutureQueries();
    }
}