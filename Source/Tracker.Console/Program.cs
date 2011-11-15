using System;
using System.Collections.Generic;
using System.Data.Mapping;
using System.Data.Metadata.Edm;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using System.Text;
using EntityFramework.Extensions;
using EntityFramework.Reflection;
using Tracker.Core;
using Tracker.Entity;

namespace Tracker.Console
{
  class Program
  {
    static void Main(string[] args)
    {
      Expression<Func<int, int, int>> expression = (a, b) => a + b;

      var db = new TrackerEntities();




      string value = "@test.com";
      var q = db.Users.Where(u => u.Email.EndsWith(value));


      dynamic queryProxy = new DynamicProxy(q);
      dynamic queryState = queryProxy.QueryState;
      dynamic executionPlan = queryState.GetExecutionPlan(null);
      dynamic commandDefinition = executionPlan.CommandDefinition;
      dynamic sets = commandDefinition.EntitySets;


      IEnumerable<EntitySet> entitySets = sets;
      var entitySetsList = entitySets.ToList();

      EntitySet storeEntitySet = entitySetsList.FirstOrDefault(entitySet =>
        MetadataProperyCompare(entitySet.ElementType.MetadataProperties, "DataSpace", DataSpace.SSpace));
      EntitySet modelEntitySet = entitySetsList.FirstOrDefault(entitySet =>
        MetadataProperyCompare(entitySet.ElementType.MetadataProperties, "DataSpace", DataSpace.CSpace));

      EntityType storeType = storeEntitySet.ElementType;
      EntityType modelType = modelEntitySet.ElementType;



      var e = q.Expression as MethodCallExpression;
      var o = q as ObjectQuery<User>;
      string sql = o.ToTraceString();
      var p = o.Parameters;

      var omap = db.MetadataWorkspace.GetItemCollection(DataSpace.OCSpace);

      var objectMap = new Dictionary<EdmType, EdmType>();

      foreach (var item in omap.GetItems<GlobalItem>())
      {
        dynamic proxy = new DynamicProxy(item);
        EdmType clrType = proxy.ClrType;
        EdmType edmType = proxy.EdmType;

        objectMap.Add(clrType, edmType);
      }


      var smap = db.MetadataWorkspace.GetItemCollection(DataSpace.CSSpace);



      var q2 = o.Select(s => s.Id);
      var e2 = q2.AsQueryable().Expression;
      var o2 = q2 as ObjectQuery<int>;
      string sql2 = o2.ToTraceString();




    }

    private static bool MetadataProperyCompare<TValue>(ReadOnlyMetadataCollection<MetadataProperty> readOnlyMetadataCollection, string name, TValue value)
    {
      if (!readOnlyMetadataCollection.Contains(name))
        return false;


      var dataSpace = readOnlyMetadataCollection[name].Value;
      if (dataSpace == null || dataSpace.GetType() != typeof(TValue))
        return false;

      return Equals((TValue)dataSpace, value);
    }
  }

}

