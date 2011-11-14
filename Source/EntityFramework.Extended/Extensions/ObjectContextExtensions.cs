using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using EntityFramework.Reflection;

namespace EntityFramework.Extensions
{
  public static class ObjectContextExtensions
  {
    public static DbCommand GetCommand(this ObjectContext context, IQueryable query)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      if (query == null)
        throw new ArgumentNullException("query");

      // HACK to get DbCommand by calling internal properties and methods
      dynamic queryProxy = new DynamicProxy(query);
      dynamic queryState = queryProxy.QueryState;
      dynamic executionPlan = queryState.GetExecutionPlan(null);
      dynamic commandDefinition = executionPlan.CommandDefinition;
   
      return commandDefinition.CreateCommand();
    }
  }
}
