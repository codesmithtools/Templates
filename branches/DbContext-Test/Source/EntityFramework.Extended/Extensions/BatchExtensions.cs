using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Common.CommandTrees;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Data.Objects;
using EntityFramework.Reflection;

namespace EntityFramework.Extensions
{
  public static class BatchExtensions
  {
    public static int Delete<TEntity>(
      this ObjectSet<TEntity> source,
      IQueryable<TEntity> entities)
      where TEntity : class
    {
      var expression = entities.Expression;


      var query = entities as ObjectQuery<TEntity>;
      query.Select("it.Id");

      string sql = query.ToTraceString();


      dynamic queryProxy = new DynamicProxy(query);
      dynamic stateProxy = queryProxy.QueryState;
      dynamic executionPlan = stateProxy.GetExecutionPlan(null);
      DbCommandDefinition commandDefinition = executionPlan.CommandDefinition;

      dynamic converter = stateProxy.CreateExpressionConverter();
      DbExpression dbExpression = converter.Convert();

      return 0;
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
      IQueryable<TEntity> entities,
      Expression<Func<TEntity, TEntity>> updateExpression)
      where TEntity : class
    {

      return 0;
    }

    public static int Update<TEntity>(
      this ObjectSet<TEntity> source,
      Expression<Func<TEntity, bool>> filterExpression,
      Expression<Func<TEntity, TEntity>> updateExpression)
      where TEntity : class
    {
      return source.Update(source.Where(filterExpression), updateExpression);
    }

  }
}
