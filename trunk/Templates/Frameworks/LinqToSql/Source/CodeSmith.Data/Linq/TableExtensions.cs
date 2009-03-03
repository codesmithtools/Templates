using System.Collections.Generic;
using System.Data.Common;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using CodeSmith.Data.Linq;

namespace System.Data.Linq
{
    //http://www.aneyfamily.com/terryandann/post/2008/04/LINQ-to-SQL-Batch-UpdatesDeletes-Fix-for-Could-not-translate-expression.aspx
    public static class TableExtensions
    {
        /// <summary>
        /// Immediately deletes all entities from the collection with a single delete command.
        /// </summary>
        /// <typeparam name="TEntity">Represents the object type for rows contained in <paramref name="table"/>.</typeparam>
        /// <param name="table">Represents a table for a particular type in the underlying database containing rows are to be deleted.</param>
        /// <param name="entities">Represents the collection of items which are to be removed from <paramref name="table"/>.</param>
        /// <returns>The number of rows deleted from the database.</returns>
        /// <remarks>
        /// <para>Similar to stored procedures, and opposite from DeleteAllOnSubmit, rows provided in <paramref name="entities"/> will be deleted immediately with no need to call <see cref="DataContext.SubmitChanges()"/>.</para>
        /// <para>Additionally, to improve performance, instead of creating a delete command for each item in <paramref name="entities"/>, a single delete command is created.</para>
        /// </remarks>
        public static int Delete<TEntity>(this Table<TEntity> table, IQueryable<TEntity> entities) where TEntity : class
        {
            DbCommand delete = table.GetDeleteBatchCommand(entities);

            IEnumerable<object> parameters = from p in delete.Parameters.Cast<DbParameter>()
                                             select p.Value;

            return table.Context.ExecuteCommand(delete.CommandText, parameters.ToArray());
        }

        /// <summary>
        /// Immediately deletes all entities from the collection with a single delete command.
        /// </summary>
        /// <typeparam name="TEntity">Represents the object type for rows contained in <paramref name="table"/>.</typeparam>
        /// <param name="table">Represents a table for a particular type in the underlying database containing rows are to be deleted.</param>
        /// <param name="filter">Represents a filter of items to be updated in <paramref name="table"/>.</param>
        /// <returns>The number of rows deleted from the database.</returns>
        public static int Delete<TEntity>(this Table<TEntity> table, Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            return table.Delete(table.Where(filter));
        }

        /// <summary>
        /// Immediately updates all entities in the collection with a single update command based on a <typeparamref name="TEntity"/> created from a Lambda expression.
        /// </summary>
        /// <typeparam name="TEntity">Represents the object type for rows contained in <paramref name="table"/>.</typeparam>
        /// <param name="table">Represents a table for a particular type in the underlying database containing rows are to be updated.</param>
        /// <param name="entities">Represents the collection of items which are to be updated in <paramref name="table"/>.</param>
        /// <param name="evaluator">A Lambda expression returning a <typeparamref name="TEntity"/> that defines the update assignments to be performed on each item in <paramref name="entities"/>.</param>
        /// <returns>The number of rows updated in the database.</returns>
        /// <remarks>
        /// <para>Similar to stored procedures, and opposite from InsertAllOnSubmit, rows provided in <paramref name="entities"/> will be updated immediately with no need to call <see cref="DataContext.SubmitChanges()"/>.</para>
        /// <para>Additionally, to improve performance, instead of creating an update command for each item in <paramref name="entities"/>, a single update command is created.</para>
        /// </remarks>
        public static int Update<TEntity>(this Table<TEntity> table, IQueryable<TEntity> entities, Expression<Func<TEntity, TEntity>> evaluator) where TEntity : class
        {
            DbCommand update = table.GetUpdateBatchCommand(entities, evaluator);

            IEnumerable<object> parameters = from p in update.Parameters.Cast<DbParameter>()
                                             select p.Value;
            return table.Context.ExecuteCommand(update.CommandText, parameters.ToArray());
        }

        /// <summary>
        /// Immediately updates all entities in the collection with a single update command based on a <typeparamref name="TEntity"/> created from a Lambda expression.
        /// </summary>
        /// <typeparam name="TEntity">Represents the object type for rows contained in <paramref name="table"/>.</typeparam>
        /// <param name="table">Represents a table for a particular type in the underlying database containing rows are to be updated.</param>
        /// <param name="filter">Represents a filter of items to be updated in <paramref name="table"/>.</param>
        /// <param name="evaluator">A Lambda expression returning a <typeparamref name="TEntity"/> that defines the update assignments to be performed on each item in <paramref name="filter"/>.</param>
        /// <returns>The number of rows updated in the database.</returns>
        public static int Update<TEntity>(this Table<TEntity> table, Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TEntity>> evaluator) where TEntity : class
        {
            return table.Update(table.Where(filter), evaluator);
        }

        private static DbCommand GetDeleteBatchCommand<TEntity>(this Table<TEntity> table, IQueryable<TEntity> entities) where TEntity : class
        {
            DbCommand deleteCommand = table.Context.GetCommand(entities);
            deleteCommand.CommandText = string.Format("DELETE {0}\r\n", table.GetDbName()) + GetBatchJoinQuery(table, entities);
            return deleteCommand;
        }

        private static DbCommand GetUpdateBatchCommand<TEntity>(this Table<TEntity> table, IQueryable<TEntity> entities, Expression<Func<TEntity, TEntity>> evaluator) where TEntity : class
        {
            DbCommand updateCommand = table.Context.GetCommand(entities);

            var setSB = new StringBuilder();
            int memberInitCount = 1;

            // Process the MemberInitExpression (there should only be one in the evaluator Lambda) to convert the expression tree
            // into a valid DbCommand.  The Visit<> method will only process expressions of type MemberInitExpression and requires
            // that a MemberInitExpression be returned - in our case we'll return the same one we are passed since we are building
            // a DbCommand and not 'really using' the evaluator Lambda.
            evaluator.Visit<MemberInitExpression>(delegate(MemberInitExpression expression)
                                                  {
                                                      if (memberInitCount > 1)
                                                          throw new NotImplementedException("Currently only one MemberInitExpression is allowed for the evaluator parameter.");

                                                      memberInitCount++;
                                                      setSB.Append(GetDbSetStatement(expression, table, updateCommand));

                                                      return expression; // just return passed in expression to keep 'visitor' happy.
                                                  });

            // Complete the command text by concatenating bits together.
            updateCommand.CommandText = string.Format("UPDATE {0}\r\n{1}\r\n\r\n{2}",
                                                      table.GetDbName(), // Database table name
                                                      setSB, // SET fld = {}, fld2 = {}, ...
                                                      GetBatchJoinQuery(table, entities)); // Subquery join created from entities command text

            if (updateCommand.CommandText.IndexOf("[arg0]") >= 0)
                // TODO (Chris): Probably a better way to determine this by using an visitor on the expression before the
                //				 var selectExpression = Expression.Call... method call (search for that) and see which funcitons
                //				 are being used and determine if supported by LINQ to SQL
                throw new NotSupportedException(
                    string.Format(
                        "The evaluator Expression<Func<{0},{0}>> has processing that needs to be performed once the query is returned (i.e. string.Format()) and therefore can not be used during batch updating.",
                        table.GetType()));

            return updateCommand;
        }

        private static string GetBatchJoinQuery<TEntity>(Table<TEntity> table, IQueryable<TEntity> entities) where TEntity : class
        {
            MetaTable metaTable = table.Context.Mapping.GetTable(typeof(TEntity));

            var keys = from mdm in metaTable.RowType.DataMembers
                       where mdm.IsPrimaryKey
                       select new { mdm.MappedName };

            var joinSB = new StringBuilder();
            var subSelectSB = new StringBuilder();

            foreach (var key in keys)
            {
                joinSB.AppendFormat("j0.[{0}] = j1.[{0}] AND ", key.MappedName);
                // For now, always assuming table is aliased as t0.  Should probably improve at some point.
                // Just writing a smaller sub-select so it doesn't get all the columns of data, but instead
                // only the primary key fields used for joining.
                subSelectSB.AppendFormat("[t0].[{0}], ", key.MappedName);
            }

            DbCommand selectCommand = table.Context.GetCommand(entities);
            string select = selectCommand.CommandText;

            string join = joinSB.ToString();

            if (join == "")
                throw new MissingPrimaryKeyException(string.Format("{0} does not have a primary key defined.  Batch updating/deleting can not be used for tables without a primary key.",
                                                                   metaTable.TableName));

            join = join.Substring(0, join.Length - 5); // Remove last ' AND '
            string selectClause = select.Substring(0, select.IndexOf("[t0]")); // Get 'SELECT ' and any TOP clause if present
            bool needsTopClause = selectClause.IndexOf(" TOP ") < 0 && select.IndexOf("\r\nORDER BY ") > 0;
            string subSelect = selectClause
                               + (needsTopClause ? "TOP 100 PERCENT " : "") // If order by in original select without TOP clause, need TOP
                               + subSelectSB; // Apped just the primary keys.
            subSelect = subSelect.Substring(0, subSelect.Length - 2); // Remove last ', '

            subSelect += select.Substring(select.IndexOf("\r\nFROM ")); // Create a sub SELECT that *only* includes the primary key fields

            string batchJoin = String.Format("FROM {0} AS j0 INNER JOIN (\r\n\r\n{1}\r\n\r\n) AS j1 ON ({2})\r\n", table.GetDbName(), subSelect, join);
            return batchJoin;
        }

        private static string GetDbSetStatement<TEntity>(MemberInitExpression memberInitExpression, Table<TEntity> table, DbCommand updateCommand) where TEntity : class
        {
            Type entityType = typeof(TEntity);

            if (memberInitExpression.Type != entityType)
                throw new NotImplementedException(string.Format("The MemberInitExpression is intializing a class of the incorrect type '{0}' and it should be '{1}'.", memberInitExpression.Type,
                                                                entityType));

            var setSB = new StringBuilder();

            string tableName = table.GetDbName();
            MetaTable metaTable = table.Context.Mapping.GetTable(entityType);
            // Used to look up actual field names when MemberAssignment is a constant,
            // need both the Name (matches the property name on LINQ object) and the
            // MappedName (db field name).
            var dbCols = from mdm in metaTable.RowType.DataMembers
                         select new { mdm.MappedName, mdm.Name };

            // Walk all the expression bindings and generate SQL 'commands' from them.  Each binding represents a property assignment
            // on the TEntity object initializer Lambda expression.
            foreach (MemberBinding binding in memberInitExpression.Bindings)
            {
                var assignment = binding as MemberAssignment;

                if (assignment == null)
                    throw new NotImplementedException("All bindings inside the MemberInitExpression are expected to be of type MemberAssignment.");

                // TODO (Document): What is this doing?  I know it's grabbing existing parameter to pass into Expression.Call() but explain 'why'
                //					I assume it has something to do with fact we can't just access the parameters of assignment.Expression?
                //					Also, any concerns of whether or not if there are two params of type entity type?
                ParameterExpression entityParam = null;
                assignment.Expression.Visit(delegate(ParameterExpression p)
                                            {
                                                if (p.Type == entityType)
                                                    entityParam = p;
                                                return p;
                                            });

                // Get the real database field name.  binding.Member.Name is the 'property' name of the LINQ object
                // so I match that to the Name property of the table mapping DataMembers.
                string name = binding.Member.Name;
                var dbCol = (from c in dbCols
                             where c.Name == name
                             select c).FirstOrDefault();

                if (dbCol == null)
                    throw new ArgumentOutOfRangeException(name, string.Format("The corresponding field on the {0} table could not be found.", tableName));

                // If entityParam is NULL, then no references to other columns on the TEntity row and need to eval 'constant' value...
                if (entityParam == null)
                {
                    // Compile and invoke the assignment expression to obtain the contant value to add as a parameter.
                    object constant = Expression.Lambda(assignment.Expression, null).Compile().DynamicInvoke();

                    // use the MappedName from the table mapping DataMembers - that is field name in DB table.
                    if (constant == null)
                        setSB.AppendFormat("[{0}] = null, ", dbCol.MappedName);
                    else
                    {
                        // Add new parameter with massaged name to avoid clashes.
                        setSB.AppendFormat("[{0}] = @p{1}, ", dbCol.MappedName, updateCommand.Parameters.Count);
                        updateCommand.Parameters.Add(new SqlParameter(string.Format("@p{0}", updateCommand.Parameters.Count), constant));
                    }
                }
                else
                {
                    // TODO (Documentation): Explain what we are doing here again, I remember you telling me why we have to call but I can't remember now.
                    // Wny are we calling Expression.Call and what are we passing it?  Below comments are just 'made up' and probably wrong.

                    // Create a MethodCallExpression which represents a 'simple' select of *only* the assignment part (right hand operator) of
                    // of the MemberInitExpression.MemberAssignment so that we can let the Linq Provider do all the 'sql syntax' generation for
                    // us. 
                    //
                    // For Example: TEntity.Property1 = TEntity.Property1 + " Hello"
                    // This selectExpression will be only dealing with TEntity.Property1 + " Hello"
                    MethodCallExpression selectExpression = Expression.Call(
                        typeof(Queryable),
                        "Select",
                        new[] { entityType, assignment.Expression.Type },
                        // TODO (Documentation): How do we know there are only 'two' parameters?  And what is Expression.Lambda doing?
                        // I assume it's returning a type of assignment.Expression.Type to match above?

                        Expression.Constant(table),
                        Expression.Lambda(assignment.Expression, entityParam));

                    setSB.AppendFormat("[{0}] = {1}, ",
                                       dbCol.MappedName,
                                       GetDbSetAssignment(table, selectExpression, updateCommand, name));
                }
            }

            string setStatements = setSB.ToString();
            return "SET " + setStatements.Substring(0, setStatements.Length - 2); // remove ', '
        }

        private static string GetDbSetAssignment(ITable table, MethodCallExpression selectExpression, DbCommand updateCommand, string bindingName)
        {
            ValidateExpression(table, selectExpression);

            // Convert the selectExpression into an IQueryable query so that I can get the CommandText
            IQueryable selectQuery = table.Provider.CreateQuery(selectExpression);

            // Get the DbCommand so I can grab relavent parts of CommandText to construct a field 
            // assignment and based on the 'current TEntity row'.  Additionally need to massage parameter 
            // names from temporary command when adding to the final update command.
            DbCommand selectCmd = table.Context.GetCommand(selectQuery);
            string selectStmt = selectCmd.CommandText;
            selectStmt = selectStmt.Substring(7, // Remove 'SELECT ' from front ( 7 )
                                              selectStmt.IndexOf("\r\nFROM ") - 7) // Return only the selection field expression
                .Replace("[t0].", "") // Remove table alias from the select
                .Replace(" AS [value]", "") // If the select is not a direct field (constant or expression), remove the field alias
                .Replace("@p", "@p" + bindingName); // Replace parameter name so doesn't conflict with existing ones.

            foreach (DbParameter selectParam in selectCmd.Parameters.Cast<DbParameter>())
            {
                string paramName = string.Format("@p{0}", updateCommand.Parameters.Count);

                // DataContext.ExecuteCommand ultimately just takes a object array of parameters and names them p0-N.  
                // So I need to now do replaces on the massaged value to get it in proper format.
                selectStmt = selectStmt.Replace(
                    selectParam.ParameterName.Replace("@p", "@p" + bindingName),
                    paramName);

                updateCommand.Parameters.Add(new SqlParameter(paramName, selectParam.Value));
            }

            return selectStmt;
        }

        private static string GetDbName<TEntity>(this Table<TEntity> table) where TEntity : class
        {
            Type entityType = typeof(TEntity);
            MetaTable metaTable = table.Context.Mapping.GetTable(entityType);
            string tableName = metaTable.TableName;

            if (!tableName.StartsWith("["))
            {
                string[] parts = tableName.Split('.');
                tableName = string.Format("[{0}]", string.Join("].[", parts));
            }

            return tableName;
        }

        private static void ValidateExpression(ITable table, Expression expression)
        {
            DataContext context = table.Context;
            PropertyInfo providerProperty = context.GetType().GetProperty("Provider", BindingFlags.Instance | BindingFlags.NonPublic);
            object provider = providerProperty.GetValue(context, null);
            MethodInfo compileMI = provider.GetType().GetMethod("System.Data.Linq.Provider.IProvider.Compile", BindingFlags.Instance | BindingFlags.NonPublic);

            // Simply compile the expression to see if it will work.
            compileMI.Invoke(provider, new object[] { expression });
        }
    }
}