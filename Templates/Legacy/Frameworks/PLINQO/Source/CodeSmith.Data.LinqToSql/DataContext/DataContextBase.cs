using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using CodeSmith.Data.Caching;
using CodeSmith.Data.Future;
using CodeSmith.Data.LinqToSql;

namespace CodeSmith.Data.Linq
{
    /// <summary>
    /// A base class for DataContext that includes future query support.
    /// </summary>
    public class DataContextBase : DataContext, IDataContext, IFutureContext
    {
        #region Constructors

        static DataContextBase()
        {
            var provider = new LinqToSqlDataContextProvider();
            DataContextProvider.Register(provider);
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

        #endregion

        #region IDataContext

        string IDataContext.ConnectionString
        {
            get
            {
                return Connection != null
                    ? Connection.ConnectionString
                    : String.Empty;
            }
        }

        void IDataContext.Detach(params object[] enities)
        {
            foreach (var item in enities)
            {
                var entity = item as ILinqEntity;
                if (entity != null)
                    entity.Detach();
            }
        }

        IDisposable IDataContext.BeginTransaction()
        {
            return BeginTransaction();
        }

        /// <summary>
        /// Starts a database transaction.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <returns>An object representing the new transaction.</returns>
        public DbTransaction BeginTransaction()
        {
            return DataContextExtensions.BeginTransaction(this);
        }

        /// <summary>
        /// Starts a database transaction with the specified isolation level. 
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="isolationLevel">The isolation level for the transaction.</param>
        /// <returns>An object representing the new transaction.</returns>
        public DbTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return DataContextExtensions.BeginTransaction(this, isolationLevel);
        }

        public void CommitTransaction()
        {
            Transaction.Commit();
        }

        public void RollbackTransaction()
        {
            Transaction.Rollback();
        }

        public bool HasOpenTransaction
        {
            get { return Transaction != null; }
        }

        #endregion

        #region IFutureContext

        /// <summary>
        /// Executes the future queries.
        /// </summary>
        public void ExecuteFutureQueries()
        {
            if (_futureQueries.Count == 0)
                return;

            // Get all the db comands
            var queries = new List<DbCommand>();
            var futures = new List<ILinqToSqlFutureQuery>();
            foreach (var future in _futureQueries)
            {
                if (future.IsLoaded)
                    continue;

                var command = future.GetCommand(this);
                // keep queries and futures in sync to apply result later
                queries.Add(command);
                futures.Add(future);
            }

            // run all non cached queries
            if (queries.Count > 0)
                using (IMultipleResults results = this.ExecuteQuery(queries))
                    foreach (ILinqToSqlFutureQuery future in futures)
                        future.SetResult(results);

            // once all queries processed, clear from queue
            _futureQueries.Clear();
        }

        private readonly List<ILinqToSqlFutureQuery> _futureQueries = new List<ILinqToSqlFutureQuery>();

        /// <summary>
        /// Gets the readonly future queries list.
        /// </summary>
        /// <value>The future queries.</value>
        IEnumerable<IFutureQuery> IFutureContext.FutureQueries
        {
            get { return _futureQueries as IEnumerable<IFutureQuery>; }
        }

        IEnumerable<T> IFutureContext.Future<T>(IQueryable<T> query, CacheSettings cacheSettings)
        {
            var action = new Action(ExecuteFutureQueries);
            var future = new FutureQuery<T>(query, action, cacheSettings);

            _futureQueries.Add(future);
            return future;
        }

        IFutureValue<T> IFutureContext.FutureFirstOrDefault<T>(IQueryable<T> query, CacheSettings cacheSettings)
        {
            var action = new Action(ExecuteFutureQueries);
            var future = new FutureValue<T>(query, action, cacheSettings);

            _futureQueries.Add(future);
            return future;
        }

        IFutureValue<int> IFutureContext.FutureCount<T>(IQueryable<T> query, CacheSettings cacheSettings)
        {
            var action = new Action(ExecuteFutureQueries);
            var future = new FutureCount(query, action, cacheSettings);

            _futureQueries.Add(future);
            return future;
        }

        #endregion
    }
}
