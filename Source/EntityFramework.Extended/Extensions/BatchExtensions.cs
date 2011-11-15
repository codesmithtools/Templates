using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Common.CommandTrees;
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
      var entityMap = source.GetEntityMap<TEntity>();      
      var innerSelect = GetSelectSql(query, entityMap);


      var sqlBuilder = new StringBuilder();

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

      string sql = sqlBuilder.ToString();

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
      IQueryable<TEntity> query,
      Expression<Func<TEntity, TEntity>> updateExpression)
      where TEntity : class
    {
      var entityMap = source.GetEntityMap<TEntity>();
      var innerJoinSql = GetSelectSql(query, entityMap);


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

    private static string GetSelectSql<TEntity>(IQueryable<TEntity> query, EntityMap entityMap)
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

      var selectQuery = DynamicQueryable.Select(query, selector.ToString());
      var objectQuery = selectQuery as ObjectQuery;

      string innerJoinSql = objectQuery.ToTraceString();
      return innerJoinSql;
    }

  }
}
