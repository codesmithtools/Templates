using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using CodeSmith.Data.Caching;

namespace CodeSmith.Data.Linq
{
    public class LinqToSqlDataContextProvider : IDataContextProvider
    {
        public static DataContext GetDataContext(IQueryable query)
        {
            Type type = query.GetType();
            FieldInfo field = type.GetField("context", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
                return null;

            object value = field.GetValue(query);
            if (value == null)
                return null;

            var dataContext = value as DataContext;
            return dataContext;
        }

        IDataContext IDataContextProvider.GetDataConext(IQueryable query)
        {
            return GetDataContext(query) as IDataContext;
        }
    }

    /// <summary>
    /// A base class for DataContext that includes future query support.
    /// </summary>
    public class DataContextBase : DataContext, IDataContext, IFutureContext
    {
        public string ConnectionString
        {
            get
            {
                return Connection != null
                    ? Connection.ConnectionString
                    : String.Empty;
            }
        }

        public void Detach(params object[] enities)
        {
            foreach (var item in enities)
            {
                var entity = item as ILinqEntity;
                if (entity != null)
                    entity.Detach();
            }
        }

        private readonly List<IFutureQuery> _futureQueries = new List<IFutureQuery>();

        /// <summary>
        /// Gets the readonly future queries list.
        /// </summary>
        /// <value>The future queries.</value>
        public IList<IFutureQuery> FutureQueries
        {
            get { return _futureQueries; }
        }

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

        /// <summary>
        /// Executes the future queries.
        /// </summary>
        public void ExecuteFutureQueries()
        {
            if (_futureQueries.Count == 0)
                return;

            // Get all the db comands
            var queries = new List<DbCommand>();
            var futures = new List<IFutureQuery>();
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
                    foreach (IFutureQuery future in futures)
                        future.SetResult(results);

            // once all queries processed, clear from queue
            _futureQueries.Clear();
        }
    }
}
