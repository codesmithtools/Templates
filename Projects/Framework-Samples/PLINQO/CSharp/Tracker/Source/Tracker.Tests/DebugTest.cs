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
using CodeSmith.Data.Linq.Dynamic;

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

        [Test]
        [Ignore]
        public void ToPageList()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            var q = db.User.Select(t => new
            {
                Id = t.Id,
                FullName = t.FirstName
            })
            .Skip(1)
            .Take(3)
            .Future();

            var users = q.ToList();

            var userList = db.User.Select(t => new
            {
                Id = t.Id,
                FullName = t.FirstName
            }).ToPagedList(1, 3);
        }

        [Test]
        [Ignore]
        public void ManyToMany()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            var user = db.User.GetByKey(1);
            var role = db.Role.GetByKey(1);

            user.RoleList.Add(role);

            role = db.Role.GetByKey(2);

            user.RoleList.Add(role);

            db.SubmitChanges();

            var userRole = user.UserRoleList.FirstOrDefault(u => u.RoleId == 1);
            db.UserRole.DeleteOnSubmit(userRole);

            user.RoleList.Remove(db.Role.GetByKey(1));
            user.RoleList.Remove(db.Role.GetByKey(2));

            var changes = db.GetChangeSet();

            db.SubmitChanges();
        }

        [Test]
        public void MyTest()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            var q1 = db.Task.ByDetails("Hello world!");
            var q2 = db.Task.ByDetails((string)null);
            var q3 = db.Task.Where(t => t.Details == null);

            var e1 = q1.Expression;
            var e2 = q2.Expression;
            var e3 = q3.Expression;

            var list1 = q1.ToList();
            var list2 = q2.ToList();
            var list3 = q3.ToList();
        }

        [Test]
        public void Nullable()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            var q1 = db.Task.Where("AssignedId == null || AssignedId == 1");
            var list = q1.ToList();


        }

        [Test]
        public void ToBinary()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            var task = db.Task.FirstOrDefault();

            var buffer = task.ToBinary();

            var task2 = Task.FromBinary(buffer);
        }


        [Test]
        public void AuditLog()
        {
            var db = new TrackerDataContext { Log = Console.Out };
            db.AuditingEnabled = true;

            var user = new User();
            user.EmailAddress = string.Format("test.{0}@test.com", DateTime.Now.Ticks);
            user.FirstName = "Test";
            user.IsApproved = true;
            user.LastName = "User";
            user.PasswordHash = "asdf";
            user.PasswordSalt = "asdf";

            db.User.InsertOnSubmit(user);            
            db.SubmitChanges();

            var auditLog = db.LastAudit;
            Assert.IsNotNull(auditLog);
        }
    }
}
