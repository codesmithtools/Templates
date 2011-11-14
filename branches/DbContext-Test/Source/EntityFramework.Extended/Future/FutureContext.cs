using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Text;
using EntityFramework.Reflection;

namespace EntityFramework.Future
{
  /// <summary>
  /// A class to hold waiting future queries for an ObjectContext.
  /// </summary>
  /// <remarks>
  /// This class creates a link between the ObjectContext and
  /// the waiting future queries. Since the ObjectContext could
  /// be displosed before this class, ObjectContext is stored as
  /// a WeakReference. 
  /// </remarks>
  public class FutureContext : IFutureContext
  {
    private readonly IList<IFutureQuery> _futureQueries;
    private readonly WeakReference _objectContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="FutureContext"/> class.
    /// </summary>
    /// <param name="objectContext">The object context for the future queries.</param>
    public FutureContext(ObjectContext objectContext)
    {
      _objectContext = new WeakReference(objectContext);
      _futureQueries = new List<IFutureQuery>();
    }

    /// <summary>
    /// Gets the future queries.
    /// </summary>
    /// <value>
    /// The future queries.
    /// </value>
    public IList<IFutureQuery> FutureQueries
    {
      get { return _futureQueries; }
    }

    /// <summary>
    /// Gets the <see cref="ObjectContext"/> for the FutureQueries.
    /// </summary>
    /// <remarks>
    /// ObjectContext is stored as a WeakReference.  The value can be disposed.
    /// </remarks>
    public ObjectContext ObjectContext
    {
      get
      {
        return _objectContext.IsAlive
          ? _objectContext.Target as ObjectContext
          : null;
      }
    }

    /// <summary>
    /// Gets an indication whether the object referenced ObjectContext has been garbage collected.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is alive; otherwise, <c>false</c>.
    /// </value>
    public bool IsAlive
    {
      get { return _objectContext.IsAlive; }
    }

    /// <summary>
    /// Executes the future queries as a single batch.
    /// </summary>
    public void ExecuteFutureQueries()
    {
      var context = ObjectContext;
      if (context == null)
        throw new ObjectDisposedException("ObjectContext", "The ObjectContext for the future queries has been displosed.");

      // used to call internal methods
      dynamic contextProxy = new DynamicProxy(context);
      contextProxy.EnsureConnection();

      try
      {
        using (var command = CreateFutureCommand(context))
        using (var reader = command.ExecuteReader())
        {
          foreach (var futureQuery in FutureQueries)
          {
            futureQuery.SetResult(context, reader);
            reader.NextResult();
          }
        }
      }
      catch
      {
        contextProxy.ReleaseConnection();
        throw;
      }
      finally
      {
        // once all queries processed, clear from queue
        _futureQueries.Clear();
      }
    }

    private DbCommand CreateFutureCommand(ObjectContext context)
    {
      DbConnection dbConnection = context.Connection;
      var entityConnection = dbConnection as EntityConnection;

      // by-pass entity connection, doesn't support multiple results.
      var command = entityConnection == null
        ? dbConnection.CreateCommand()
        : entityConnection.StoreConnection.CreateCommand();

      var futureSql = new StringBuilder();
      int paramCount = 0;
      int queryCount = 0;

      foreach (IFutureQuery futureQuery in FutureQueries)
      {
        var plan = futureQuery.GetPlan(context);
        string sql = plan.CommandText;

        // clean up params
        foreach (var parameter in plan.Parameters)
        {
          string orginal = parameter.Name;
          string updated = string.Format("{0}__f__{1}", orginal, paramCount++);

          sql = sql.Replace("@" + orginal, "@" + updated);

          var dbParameter = command.CreateParameter();
          dbParameter.ParameterName = updated;
          dbParameter.Value = parameter.Value;

          command.Parameters.Add(dbParameter);
        }

        // add sql
        if (futureSql.Length > 0)
          futureSql.AppendLine();

        futureSql.Append("-- Query #");
        futureSql.Append(queryCount + 1);
        futureSql.AppendLine();
        futureSql.AppendLine();

        futureSql.Append(sql.Trim());
        futureSql.AppendLine(";"); // TODO, config this by provider?

        queryCount++;
      } // foreach query

      command.CommandText = futureSql.ToString();
      if (context.CommandTimeout.HasValue)
        command.CommandTimeout = context.CommandTimeout.Value;

      return command;
    }
  }
}
