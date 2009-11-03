using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;

namespace CodeSmith.Data.Linq
{
    /// <summary>
    /// A base class for DataContext that includes future query support.
    /// </summary>
    public class DataContextBase : DataContext, IFutureContext
    {
        private readonly List<IFutureQuery> _futureQueries = new List<IFutureQuery>();

        /// <summary>
        /// Gets the readonly future queries list.
        /// </summary>
        /// <value>The future queries.</value>
        public IList<IFutureQuery> FutureQueries
        {
            get { return _futureQueries.AsReadOnly(); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextBase"/> class.
        /// </summary>
        /// <param name="fileOrServerOrConnection">The file or server or connection.</param>
        public DataContextBase(string fileOrServerOrConnection)
            : base(fileOrServerOrConnection)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextBase"/> class.
        /// </summary>
        /// <param name="fileOrServerOrConnection">The file or server or connection.</param>
        /// <param name="mapping">The mapping.</param>
        public DataContextBase(string fileOrServerOrConnection, MappingSource mapping)
            : base(fileOrServerOrConnection, mapping)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextBase"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public DataContextBase(IDbConnection connection)
            : base(connection)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextBase"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="mapping">The mapping.</param>
        public DataContextBase(IDbConnection connection, MappingSource mapping)
            : base(connection, mapping)
        { }


        /// <summary>
        /// Adds the query to the <see cref="FutureQueries"/> queue.
        /// </summary>
        /// <typeparam name="T">The type of the query.</typeparam>
        /// <param name="query">The query as the source.</param>
        /// <returns>
        /// An instance of <see cref="T:CodeSmith.Data.Linq.FutureQuery`1"/> to use to access the query later.
        /// </returns>
        FutureQuery<T> IFutureContext.AddFutureQuery<T>(IQueryable<T> query)
        {
            var futureContext = (IFutureContext)this;
            var future = new FutureQuery<T>(query, futureContext.ExecuteFutureQueries);
            _futureQueries.Add(future);

            return future;
        }

        /// <summary>
        /// Adds the query to the <see cref="FutureQueries"/> queue.
        /// </summary>
        /// <typeparam name="T">The type of the query.</typeparam>
        /// <param name="query">The query as the source.</param>
        /// <returns>
        /// An instance of <see cref="T:CodeSmith.Data.Linq.FutureValue`1"/> to use to access the query later.
        /// </returns>
        FutureValue<T> IFutureContext.AddFutureValueQuery<T>(IQueryable<T> query)
        {
            var futureContext = (IFutureContext)this;
            var future = new FutureValue<T>(query, futureContext.ExecuteFutureQueries);
            _futureQueries.Add(future);

            return future;
        }

        /// <summary>
        /// Adds the query to the <see cref="FutureQueries"/> queue.
        /// </summary>
        /// <typeparam name="T">The type of the query.</typeparam>
        /// <param name="query">The query as the source.</param>
        /// <returns>
        /// An instance of <see cref="T:CodeSmith.Data.Linq.FutureCount"/> to use to access the count later.
        /// </returns>
        FutureCount IFutureContext.AddFutureCountQuery<T>(IQueryable<T> query)
        {
            var futureContext = (IFutureContext)this;
            var future = new FutureCount(query, futureContext.ExecuteFutureQueries);
            _futureQueries.Add(future);

            return future;
        }

        /// <summary>
        /// Executes the future queries.
        /// </summary>
        void IFutureContext.ExecuteFutureQueries()
        {
            if (_futureQueries.Count == 0)
                return;

            var queries = new List<DbCommand>();
            foreach (var future in _futureQueries)
                queries.Add(future.GetCommand(this));

            using (IMultipleResults results = this.ExecuteQuery(queries))
                foreach (IFutureQuery future in _futureQueries)
                    future.SetResult(results);

            // once all queries processed, clear from queue
            _futureQueries.Clear();
        }
    }
}
