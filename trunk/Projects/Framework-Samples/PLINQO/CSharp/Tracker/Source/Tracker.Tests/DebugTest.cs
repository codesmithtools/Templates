using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Tracker.Core.Data;
using CodeSmith.Data.Linq;

namespace Tracker.Tests
{
    [TestFixture]
    public class DebugTest
    {
        [Test]
        public void FutureLoadWith()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            DataLoadOptions options = new DataLoadOptions();
            options.LoadWith<Task>(t => t.CreatedUser);

            db.LoadOptions = options;

            var q1 = db.User
                .ByEmailAddress("laura.roslin@battlestar.com")
                .Future();

            var q2 = db.Task
                .Where(t => t.LastModifiedBy == "laura.roslin@battlestar.com")
                .Future();

            var users = q1.ToList();
            Assert.IsNotNull(users);

            var tasks = q2.ToList();
            Assert.IsNotNull(tasks);

        }

        [Test]
        public void ExecuteQueryLoadWith()
        {
            var db = new TrackerDataContext { Log = Console.Out };
            db.DeferredLoadingEnabled = false;
            db.ObjectTrackingEnabled = false;

            DataLoadOptions options = new DataLoadOptions();
            options.LoadWith<Task>(t => t.CreatedUser);

            db.LoadOptions = options;

            var q1 = db.User
                .ByEmailAddress("laura.roslin@battlestar.com");

            var q2 = db.Task
                .Where(t => t.LastModifiedBy == "laura.roslin@battlestar.com");

            var result = db.ExecuteQuery(q1, q2);

            Assert.IsNotNull(result);

            var userResult = result.GetResult<User>();
            Assert.IsNotNull(userResult);

            var users = userResult.ToList();
            Assert.IsNotNull(users);

            var taskResult = result.GetResult<Task>();
            Assert.IsNotNull(taskResult);

            var tasks = taskResult.ToList();
            Assert.IsNotNull(tasks);
        }

        [Test]
        public void Reflection()
        {
            var db = new TrackerDataContext { Log = Console.Out };
            IQueryable source = db.Task.Where(t => t.LastModifiedBy == "laura.roslin@battlestar.com").OrderBy(t => t.CreatedId);


            // can be static
            Type queryableType = typeof(Queryable);
            // can be static
            MethodInfo countMethod = (from m in queryableType.GetMethods(BindingFlags.Static | BindingFlags.Public)
                                      where m.Name == "Count"
                                        && m.IsGenericMethod
                                        && m.GetParameters().Length == 1
                                      select m).FirstOrDefault();


            Assert.IsNotNull(countMethod);

            var genericMethod = countMethod.MakeGenericMethod(new[] { source.ElementType });
            var expression = Expression.Call(null, genericMethod, source.Expression);

            Assert.IsNotNull(expression);

            Type dataContextType = db.GetType();
            PropertyInfo providerProperty = dataContextType.GetProperty("Provider", BindingFlags.Instance | BindingFlags.NonPublic);

            object provider = providerProperty.GetValue(db, null);

            Type providerType = provider.GetType().GetInterface("IProvider");
            MethodInfo getCommandMethod = providerType.GetMethod("GetCommand", BindingFlags.Instance | BindingFlags.Public);

            object commandObject = getCommandMethod.Invoke(provider, new object[] { expression });

            Assert.IsNotNull(commandObject);
        }
    }
}
