using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Common.CommandTrees;
using System.Data.EntityClient;
using System.Data.Metadata.Edm;
using System.Data.Objects.DataClasses;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Data.Objects;
using EntityFramework.Reflection;

namespace EntityFramework.Extensions
{
    public static class BatchExtensions
    {
        public static int Delete<TEntity>(
            this ObjectSet<TEntity> source,
            IQueryable<TEntity> query)
            where TEntity : class
        {
            ObjectContext objectContext = source.Context;

            DbConnection deleteConnection = null;
            DbTransaction deleteTransaction = null;
            DbCommand deleteCommand = null;

            try
            {
                deleteConnection = GetStoreConnection(objectContext);
                deleteConnection.Open();

                deleteTransaction = deleteConnection.BeginTransaction();

                deleteCommand = deleteConnection.CreateCommand();
                deleteCommand.Transaction = deleteTransaction;
                if (objectContext.CommandTimeout.HasValue)
                    deleteCommand.CommandTimeout = objectContext.CommandTimeout.Value;

                var entityMap = source.GetEntityMap<TEntity>();
                var innerSelect = GetSelectSql(query, entityMap, deleteCommand);

                var sqlBuilder = new StringBuilder(innerSelect.Length * 2);

                sqlBuilder.Append("DELETE ");
                sqlBuilder.Append(entityMap.TableName);
                sqlBuilder.AppendLine();

                sqlBuilder.AppendFormat("FROM {0} AS j0 INNER JOIN (", entityMap.TableName);
                sqlBuilder.AppendLine();
                sqlBuilder.AppendLine(innerSelect);
                sqlBuilder.Append(") AS j1 ON (");

                bool wroteKey = false;
                foreach (var keyMap in entityMap.KeyMaps)
                {
                    if (wroteKey)
                        sqlBuilder.Append(" AND ");

                    sqlBuilder.AppendFormat("j0.{0} = j1.{0}", keyMap.ColumnName);
                    wroteKey = true;
                }
                sqlBuilder.Append(")");

                deleteCommand.CommandText = sqlBuilder.ToString();

                int result = deleteCommand.ExecuteNonQuery();
                deleteTransaction.Commit();

                return result;
            }
            finally
            {
                if (deleteCommand != null)
                    deleteCommand.Dispose();
                if (deleteTransaction != null)
                    deleteTransaction.Dispose();
                if (deleteConnection != null)
                    deleteConnection.Dispose();
            }
        }


        public static int Delete<TEntity>(
            this ObjectSet<TEntity> source,
            Expression<Func<TEntity, bool>> filterExpression)
            where TEntity : class
        {
            return source.Delete(source.Where(filterExpression));
        }

        
        public static int Update<TEntity>(
            this ObjectSet<TEntity> source,
            IQueryable<TEntity> query,
            Expression<Func<TEntity, TEntity>> updateExpression)
            where TEntity : class
        {
            ObjectContext objectContext = source.Context;

            DbConnection updateConnection = null;
            DbTransaction updateTransaction = null;
            DbCommand updateCommand = null;

            try
            {
                updateConnection = GetStoreConnection(objectContext);
                updateConnection.Open();

                updateTransaction = updateConnection.BeginTransaction();

                updateCommand = updateConnection.CreateCommand();
                updateCommand.Transaction = updateTransaction;
                if (objectContext.CommandTimeout.HasValue)
                    updateCommand.CommandTimeout = objectContext.CommandTimeout.Value;

                var entityMap = source.GetEntityMap<TEntity>();
                var innerSelect = GetSelectSql(query, entityMap, updateCommand);
                var sqlBuilder = new StringBuilder(innerSelect.Length * 2);

                sqlBuilder.Append("UPDATE ");
                sqlBuilder.Append(entityMap.TableName);
                sqlBuilder.AppendLine(" SET ");

                var memberInitExpression = updateExpression.Body as MemberInitExpression;
                if (memberInitExpression == null)
                    throw new ArgumentException("The update expression must be of type MemberInitExpression.", "updateExpression");

                int nameCount = 0;
                bool wroteSet = false;
                foreach (MemberBinding binding in memberInitExpression.Bindings)
                {
                    if (wroteSet)
                        sqlBuilder.AppendLine(", ");

                    string propertyName = binding.Member.Name;
                    string columnName = entityMap.PropertyMaps
                        .Where(p => p.PropertyName == propertyName)
                        .Select(p => p.ColumnName)
                        .FirstOrDefault();

                    string parameterName = "p__update__" + nameCount++;

                    var memberAssignment = binding as MemberAssignment;
                    if (memberAssignment == null)
                        throw new ArgumentException("The update expression MemberBinding must only by type MemberAssignment.", "updateExpression");

                    object value = null;
                    if (memberAssignment.Expression.NodeType == ExpressionType.Constant)
                    {
                        var constantExpression = memberAssignment.Expression as ConstantExpression;
                        if (constantExpression == null)
                            throw new ArgumentException("The MemberAssignment expression is not a ConstantExpression.", "updateExpression");

                        value = constantExpression.Value;
                    }
                    else
                    {
                        LambdaExpression lambda = Expression.Lambda(memberAssignment.Expression, null);
                        value = lambda.Compile().DynamicInvoke();
                    }

                    var parameter = updateCommand.CreateParameter();
                    parameter.ParameterName = parameterName;
                    parameter.Value = value;
                    updateCommand.Parameters.Add(parameter);

                    sqlBuilder.AppendFormat("{0} = @{1}", columnName, parameterName);
                    wroteSet = true;
                }

                sqlBuilder.AppendLine(" ");
                sqlBuilder.AppendFormat("FROM {0} AS j0 INNER JOIN (", entityMap.TableName);
                sqlBuilder.AppendLine();
                sqlBuilder.AppendLine(innerSelect);
                sqlBuilder.Append(") AS j1 ON (");

                bool wroteKey = false;
                foreach (var keyMap in entityMap.KeyMaps)
                {
                    if (wroteKey)
                        sqlBuilder.Append(" AND ");

                    sqlBuilder.AppendFormat("j0.{0} = j1.{0}", keyMap.ColumnName);
                    wroteKey = true;
                }
                sqlBuilder.Append(")");

                updateCommand.CommandText = sqlBuilder.ToString();

                int result = updateCommand.ExecuteNonQuery();
                updateTransaction.Commit();

                return result;
            }
            finally
            {
                if (updateCommand != null)
                    updateCommand.Dispose();
                if (updateTransaction != null)
                    updateTransaction.Dispose();
                if (updateConnection != null)
                    updateConnection.Dispose();
            }
        }
        
        public static int Update<TEntity>(
            this ObjectSet<TEntity> source,
            Expression<Func<TEntity, bool>> filterExpression,
            Expression<Func<TEntity, TEntity>> updateExpression)
            where TEntity : class
        {
            return source.Update(source.Where(filterExpression), updateExpression);
        }


        private static DbConnection GetStoreConnection(ObjectContext objectContext)
        {
            DbConnection dbConnection = objectContext.Connection;
            var entityConnection = dbConnection as EntityConnection;

            // by-pass entity connection
            var connection = entityConnection == null
                ? dbConnection
                : entityConnection.StoreConnection;

            return connection;
        }

        private static string GetSelectSql<TEntity>(IQueryable<TEntity> query, EntityMap entityMap, DbCommand command)
            where TEntity : class
        {
            // changing query to only select keys
            var selector = new StringBuilder(50);
            selector.Append("new(");
            foreach (var propertyMap in entityMap.KeyMaps)
            {
                if (selector.Length > 4)
                    selector.Append((", "));

                selector.Append(propertyMap.PropertyName);
            }
            selector.Append(")");

            var selectQuery = query.Select(selector.ToString());
            var objectQuery = selectQuery as ObjectQuery;

            if (objectQuery == null)
                throw new ArgumentException("The query must be of type ObjectQuery.", "query");

            string innerJoinSql = objectQuery.ToTraceString();

            // create parameters
            foreach (var objectParameter in objectQuery.Parameters)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = objectParameter.Name;
                parameter.Value = objectParameter.Value;

                command.Parameters.Add(parameter);
            }

            return innerJoinSql;
        }

    }
}
