using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeSmith.Data.Linq
{
    //http://www.aneyfamily.com/terryandann/post/2008/04/LINQ-to-SQL-Batch-UpdatesDeletes-Fix-for-Could-not-translate-expression.aspx
    public static class DataContextExtensions
    {
        /// <summary>
        /// Executes a SQL statement against a <see cref="DataContext"/> connection. 
        /// </summary>
        /// <param name="context">The <see cref="DataContext"/> to execute the batch select against.</param>
        /// <param name="command">The DbCommand to execute.</param>
        /// <returns>The number of rows affected.</returns>
        /// <remarks>The DbCommand is not disposed by this call, the caller must dispose the DbCommand.</remarks>
        public static int ExecuteCommand(this DataContext context, DbCommand command)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (command == null)
                throw new ArgumentNullException("command");

            LogCommand(context, command);

            command.Connection = context.Connection;
            if (command.Connection == null)
                throw new InvalidOperationException("The DataContext must contain a valid SqlConnection.");

            if (context.Transaction != null)
                command.Transaction = context.Transaction;

            if (command.Connection.State == ConnectionState.Closed)
                command.Connection.Open();

            return command.ExecuteNonQuery();
        }

        /// <summary>
        /// Batches together multiple <see cref="IQueryable"/> queries into a single <see cref="DbCommand"/> and returns all data in
        /// a single round trip to the database.
        /// </summary>
        /// <param name="context">The <see cref="DataContext"/> to execute the batch select against.</param>
        /// <param name="queries">Represents a collections of SELECT queries to execute.</param>
        /// <returns>Returns an <see cref="IMultipleResults"/> object containing all results.</returns>
        /// <exception cref="ArgumentNullException">Thrown when context or queries are null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when context.Connection is invalid.</exception>
        public static IMultipleResults ExecuteQuery(this DataContext context, params IQueryable[] queries)
        {
            return ExecuteQuery(context, queries.ToList());
        }

        /// <summary>
        /// Batches together multiple <see cref="IQueryable"/> queries into a single <see cref="DbCommand"/> and returns all data in
        /// a single round trip to the database.
        /// </summary>
        /// <param name="context">The <see cref="DataContext"/> to execute the batch select against.</param>
        /// <param name="queries">Represents a collections of SELECT queries to execute.</param>
        /// <returns>Returns an <see cref="IMultipleResults"/> object containing all results.</returns>
        /// <exception cref="ArgumentNullException">Thrown when context or queries are null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when context.Connection is invalid.</exception>
        public static IMultipleResults ExecuteQuery(this DataContext context, IEnumerable<IQueryable> queries)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (queries == null)
                throw new ArgumentNullException("queries");

            var commandList = new List<DbCommand>();

            foreach (var query in queries)
                commandList.Add(context.GetCommand(query, true));

            return ExecuteQuery(context, commandList);
        }

        /// <summary>
        /// Batches together multiple <see cref="IQueryable"/> queries into a single <see cref="DbCommand"/> and returns all data in
        /// a single round trip to the database.
        /// </summary>
        /// <param name="context">The <see cref="DataContext"/> to execute the batch select against.</param>
        /// <param name="commands">The list of commands to execute.</param>
        /// <returns>Returns an <see cref="IMultipleResults"/> object containing all results.</returns>
        /// <exception cref="ArgumentNullException">Thrown when context or queries are null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when context.Connection is invalid.</exception>
        public static IMultipleResults ExecuteQuery(this DataContext context, IEnumerable<DbCommand> commands)
        {
            using (DbCommand batchCommand = CombineCommands(context, commands))
            {
                LogCommand(context, batchCommand);

                batchCommand.Connection = context.Connection;


                if (batchCommand.Connection == null)
                    throw new InvalidOperationException("The DataContext must contain a valid SqlConnection.");

                if (context.Transaction != null)
                    batchCommand.Transaction = context.Transaction;

                DbDataReader dr;

                if (batchCommand.Connection.State == ConnectionState.Closed)
                {
                    batchCommand.Connection.Open();
                    dr = batchCommand.ExecuteReader(CommandBehavior.CloseConnection);
                }
                else
                    dr = batchCommand.ExecuteReader();

                if (dr == null)
                    return null;

                return context.Translate(dr);
            }
        }

        /// <summary>
        /// Provides information about SQL commands generated by LINQ to SQL.
        /// </summary>
        /// <param name="context">The DataContext to get the command from.</param>
        /// <param name="query">The query whose SQL command information is to be retrieved.</param>
        /// <param name="isForTranslate">if set to <c>true</c>, adjust the sql command to support calling DataContext.Translate().</param>
        /// <returns></returns>
        public static DbCommand GetCommand(this DataContext context, IQueryable query, bool isForTranslate)
        {
            // HACK: GetCommand will not work with transactions and the L2SProfiler.
            var transaction = context.Transaction;
            context.Transaction = null;
            
            var dbCommand = context.GetCommand(query);   
         
            if (transaction != null && transaction.Connection != null)
            {
                dbCommand.Transaction = transaction;
                context.Transaction = transaction;
            }
            
            if (!isForTranslate)
                return dbCommand;

            MetaType metaType = context.Mapping.GetMetaType(query.ElementType);
            if (metaType != null && metaType.IsEntity)
                dbCommand.CommandText = RemoveColumnAlias(dbCommand.CommandText, metaType);

            return dbCommand;
        }

        public static DbCommand GetCommand(this DataContext dataContext, Expression expression)
        {
            // get provider from DataContext
            object provider = GetProvider(dataContext);
            if (provider == null)
                throw new InvalidOperationException("Failed to get the DataContext provider instance.");

            Type providerType = provider.GetType().GetInterface("IProvider");
            if (providerType == null)
                throw new InvalidOperationException("Failed to cast the DataContext provider to IProvider.");

            MethodInfo commandMethod = providerType.GetMethod("GetCommand", BindingFlags.Instance | BindingFlags.Public);
            if (commandMethod == null)
                throw new InvalidOperationException("Failed to get the GetCommand method from the DataContext provider.");

            // run the GetCommand method from the provider directly
            var commandObject = commandMethod.Invoke(provider, new object[] { expression });
            return commandObject as DbCommand;
        }

        /// <summary>
        /// Starts a database transaction.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <returns>An object representing the new transaction.</returns>
        public static DbTransaction BeginTransaction(this DataContext dataContext)
        {
            return BeginTransaction(dataContext, IsolationLevel.Unspecified);
        }

        /// <summary>
        /// Starts a database transaction with the specified isolation level. 
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="isolationLevel">The isolation level for the transaction.</param>
        /// <returns>An object representing the new transaction.</returns>
        public static DbTransaction BeginTransaction(this DataContext dataContext, IsolationLevel isolationLevel)
        {
            if (dataContext == null)
                throw new ArgumentNullException("dataContext");

            if (dataContext.Connection.State == ConnectionState.Closed)
                dataContext.Connection.Open();

            var transaction = dataContext.Connection.BeginTransaction(isolationLevel);
            dataContext.Transaction = transaction;

            return transaction;
        }

        /// <summary>
        /// Combines multiple SELECT commands into a single <see cref="SqlCommand"/> so that all statements can be executed in a
        /// single round trip to the database and return multiple result sets.
        /// </summary>
        /// <param name="context">The DataContext to get the command from.</param>
        /// <param name="selectCommands">Represents a collection of commands to be batched together.</param>
        /// <returns>
        /// Returns a single <see cref="SqlCommand"/> that executes all SELECT statements at once.
        /// </returns>
        private static DbCommand CombineCommands(DataContext context, IEnumerable<DbCommand> selectCommands)
        {
            var batchCommand = context.Connection.CreateCommand();
            DbParameterCollection newParamList = batchCommand.Parameters;
            var sql = new StringBuilder();

            int commandCount = 0;

            foreach (DbCommand cmd in selectCommands)
            {
                if (commandCount > 0)
                    sql.AppendLine();

                sql.AppendFormat("-- Query #{0}", commandCount + 1);
                sql.AppendLine();
                sql.AppendLine();

                DbParameterCollection paramList = cmd.Parameters;
                int paramCount = paramList.Count;

                for (int currentParam = paramCount - 1; currentParam >= 0; currentParam--)
                {
                    DbParameter param = paramList[currentParam];
                    DbParameter newParam = CloneParameter(param);
                    string newParamName = param.ParameterName.Replace("@", string.Format("@q{0}", commandCount));
                    cmd.CommandText = cmd.CommandText.Replace(param.ParameterName, newParamName);
                    newParam.ParameterName = newParamName;
                    newParamList.Add(newParam);
                }

                sql.Append(cmd.CommandText.Trim());
                sql.AppendLine(";");
                commandCount++;
            }

            batchCommand.CommandText = sql.ToString();

            return batchCommand;
        }

        /// <summary>
        /// Returns a clone (via copying all properties) of an existing <see cref="DbParameter"/>.
        /// </summary>
        /// <param name="src">The <see cref="DbParameter"/> to clone.</param>
        /// <returns>Returns a clone (via copying all properties) of an existing <see cref="DbParameter"/>.</returns>
        private static DbParameter CloneParameter(DbParameter src)
        {
            var source = src as SqlParameter;
            if (source == null)
                return src;

            var destination = new SqlParameter();

            destination.Value = source.Value;
            destination.Direction = source.Direction;
            destination.Size = source.Size;
            destination.Offset = source.Offset;
            destination.SourceColumn = source.SourceColumn;
            destination.SourceVersion = source.SourceVersion;
            destination.SourceColumnNullMapping = source.SourceColumnNullMapping;
            destination.IsNullable = source.IsNullable;

            destination.CompareInfo = source.CompareInfo;
            destination.XmlSchemaCollectionDatabase = source.XmlSchemaCollectionDatabase;
            destination.XmlSchemaCollectionOwningSchema = source.XmlSchemaCollectionOwningSchema;
            destination.XmlSchemaCollectionName = source.XmlSchemaCollectionName;
            destination.UdtTypeName = source.UdtTypeName;
            destination.TypeName = source.TypeName;
            destination.ParameterName = source.ParameterName;
            destination.Precision = source.Precision;
            destination.Scale = source.Scale;

            return destination;
        }

        private static string RemoveColumnAlias(string sql, MetaType metaType)
        {
            // Work around issue with DataContext.Translate not supporting column aliases.

            // find the first from
            int fromIndex = sql.IndexOf("\r\nFROM ");

            string selectText = sql.Substring(0, fromIndex);
            string fromText = sql.Substring(fromIndex);

            foreach (MetaDataMember member in metaType.PersistentDataMembers)
            {
                if (member.IsAssociation || member.Name == member.MappedName)
                    continue;

                // remove column alias because DataContext.Translate doesn't work with them
                string search = string.Format("[{0}] AS [{1}]", member.MappedName, member.Name);
                string replace = string.Format("[{0}]", member.MappedName);

                selectText = selectText.Replace(search, replace);
            }

            return selectText + fromText;
        }

        internal static void LogCommand(DataContext context, DbCommand cmd)
        {
            if (context == null || context.Log == null || cmd == null)
                return;

            var flags = BindingFlags.Instance | BindingFlags.NonPublic;

            // get provider from DataContext
            object provider = GetProvider(context);
            if (provider == null)
                return;

            Type providerType = provider.GetType();
            MethodInfo logCommandMethod = providerType.GetMethod("LogCommand", flags);

            while (logCommandMethod == null && providerType.BaseType != null)
            {
                providerType = providerType.BaseType;
                logCommandMethod = providerType.GetMethod("LogCommand", flags);
            }

            if (logCommandMethod == null)
                return;

            logCommandMethod.Invoke(provider, new object[] { context.Log, cmd });
        }

        private static object GetProvider(DataContext dataContext)
        {
            Type contextType = dataContext.GetType();
            PropertyInfo providerProperty = contextType.GetProperty("Provider", BindingFlags.Instance | BindingFlags.NonPublic);
            if (providerProperty == null)
                return null;

            return providerProperty.GetValue(dataContext, null);
        }

    }
}