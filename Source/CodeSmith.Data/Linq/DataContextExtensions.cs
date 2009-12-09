using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CodeSmith.Data.Linq
{
    //http://www.aneyfamily.com/terryandann/post/2008/04/LINQ-to-SQL-Batch-UpdatesDeletes-Fix-for-Could-not-translate-expression.aspx
    public static class DataContextExtensions
    {
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
                commandList.Add(context.GetCommand(query));

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
        /// Combines multiple SELECT commands into a single <see cref="SqlCommand"/> so that all statements can be executed in a
        /// single round trip to the database and return multiple result sets.
        /// </summary>
        /// <param name="selectCommands">Represents a collection of commands to be batched together.</param>
        /// <returns>Returns a single <see cref="SqlCommand"/> that executes all SELECT statements at once.</returns>
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

        private static void LogCommand(DataContext context, DbCommand cmd)
        {
            if (context == null || context.Log == null || cmd == null)
                return;

            var flags = BindingFlags.Instance | BindingFlags.NonPublic;

            PropertyInfo providerProperty = context.GetType().GetProperty("Provider", flags);
            if (providerProperty == null)
                return;

            object provider = providerProperty.GetValue(context, null);

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
    }
}