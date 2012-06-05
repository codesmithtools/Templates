using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeSmith.Data.Linq;
using NHibernate;
using NHibernate.Linq;
using NUnit.Framework;
using Tracker.Data;
using Tracker.Data.Entities;

namespace Tracker
{
    [TestFixture]
    public class Tests
    {
        public int UserId { get; set; }

        public int RoleId { get; set; }
        
        #region SetUp and TearDown

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            using (var db = new TrackerDataContext())
            {
                var oldUser = db.User
                    .ByEmailAddress("test@test.test")
                    .FirstOrDefault();
                if (oldUser != null)
                    db.User.DeleteOnSubmit(oldUser);

                var oldRole = db.Role
                    .ByName("Tester")
                    .FirstOrDefault();
                if (oldRole != null)
                    db.Role.DeleteOnSubmit(oldRole);

                if (oldUser != null || oldRole != null)
                    db.SubmitChanges();
            }
        }

        [SetUp]
        public void SetUp()
        {
            using (var db = new TrackerDataContext())
            {
                var user = new User
                {
                    EmailAddress = "test@test.test",
                    FirstName = "Testie",
                    LastName = "McTester",
                    PasswordHash =
                        "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA",
                    PasswordSalt = "=Unc%",
                    IsApproved = true,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    LastActivityDate = DateTime.Now,
                    LastLoginDate = DateTime.Now,
                    LastPasswordChangeDate = DateTime.Now,
                    RoleList = new List<Role>()
                };
                
                db.User.InsertOnSubmit(user);

                var role = new Role
                {
                    Name = "Tester",
                    Description = "Does testing.",
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };
                db.Role.InsertOnSubmit(role);

                user.RoleList.Add(role);
                db.SubmitChanges();

                UserId = user.Id;
                RoleId = role.Id;
            }
        }

        [TearDown]
        public void TearDown()
        {
            using (var db = new TrackerDataContext())
            {
                var user = db.User.GetByKey(UserId);
                if (user != null)
                    db.User.DeleteOnSubmit(user);

                var role = db.Role.GetByKey(RoleId);
                if (role != null)
                    db.Role.DeleteOnSubmit(role);

                db.SubmitChanges();
            }
        }

        #endregion

        [Test]
        public void ManyToMany()
        {
            using (var db = new TrackerDataContext())
            {
                var user = db.User.GetByKey(UserId);
                Assert.AreEqual(1, user.RoleList.Count);

                var role = db.Role.GetByKey(RoleId);
                user.RoleList.Remove(role);
                
                db.SubmitChanges();
                Assert.AreEqual(0, user.RoleList.Count);
            }

            using (var db = new TrackerDataContext())
            {
                var user = db.User.GetByKey(UserId);
                Assert.AreEqual(0, user.RoleList.Count);

                var role = db.Role.GetByKey(RoleId);
                user.RoleList.Add(role);

                db.SubmitChanges();
                Assert.AreEqual(1, user.RoleList.Count);
            }

            using (var db = new TrackerDataContext())
            {
                var user = db.User.GetByKey(UserId);
                Assert.AreEqual(1, user.RoleList.Count);
            }
        }

        [Test]
        public void ManyToOneAndOneToMany()
        {
            int taskId;

            using (var db = new TrackerDataContext())
            {
                var priority = db.Priority.GetByKey(Priority.High);
                var status = db.Status.GetByKey(Status.Completed);
                var user = db.User.GetByKey(UserId);

                var task = new Task
                {
                    CompleteDate = DateTime.Now,
                    CreatedDate = DateTime.Now,
                    Details = "Test'n",
                    DueDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    Priority = priority,
                    StartDate = DateTime.Now,
                    Status = status,
                    Summary = "Test'n",
                    UserCreated = user
                };
                db.Task.InsertOnSubmit(task);
                db.SubmitChanges();

                taskId = task.Id;
            }

            using (var db = new TrackerDataContext())
            {
                var user = db.User.GetByKey(UserId);

                Assert.AreEqual(0, user.TaskAssignedList.Count);
                Assert.AreEqual(1, user.TaskCreatedList.Count);

                var task = user.TaskCreatedList.First();
                user.TaskAssignedList.Add(task);
                db.SubmitChanges();

                Assert.AreEqual(1, user.TaskAssignedList.Count);
            }

            using (var db = new TrackerDataContext())
            {
                var task = db.Task.GetByKey(taskId);
                db.Task.DeleteOnSubmit(task);
                db.SubmitChanges();

                var user = db.User.GetByKey(UserId);

                Assert.AreEqual(0, user.TaskAssignedList.Count);
                Assert.AreEqual(0, user.TaskCreatedList.Count);
            }
        }

        [Test]
        public void Queries()
        {
            var lastLogin = DateTime.Now.AddMonths(-1);

            using (var db = new TrackerDataContext())
            {
                var user1 = db.User
                    .ByFirstName(ContainmentOperator.StartsWith, "Test")
                    .ByLastName("QA", "McTester")
                    .ByIsApproved(true)
                    .ByLastLoginDate(ComparisonOperator.GreaterThan,lastLogin )
                    .First();

                var user2 = db.User
                    .Where(u => u.FirstName.StartsWith("Test"))
                    .Where(u => u.LastName == "QA" || u.LastName == "McTester")
                    .Where(u => u.IsApproved == true)
                    .Where(u => u.LastLoginDate > lastLogin)
                    .First();

                Assert.AreEqual(user1.Id, user2.Id);
            }
        }

        [Test]
        public void StoreProcs()
        {
            using (var db = new TrackerDataContext())
            {
                var roles = db.RolesForUser(UserId);
                var users = db.GetUsersWithRoles();

                Assert.AreEqual(1, roles.Count);
                Assert.Greater(users.Count, 0);
            }
        }

        [Test]
        public void Views()
        {
            using (var db = new TrackerDataContext())
            {
                var taskDetails = db.TaskDetail
                    .ByPriority("High")
                    .ToList();

                Assert.AreEqual(1, taskDetails.Count);
            }
        }

        [Test]
        public void ObjectTrackingEnabled()
        {
            using (var db = new TrackerDataContext { ObjectTrackingEnabled = false })
            {
                var user = db.User
                    .ByFirstName("Testie")
                    .FirstOrDefault();

                Assert.IsNotNull(user);

                try
                {
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    Assert.IsTrue(ex.Message.Contains("ObjectTrackingEnabled"));
                }
            }
        }

        [Test]
        public void GetHashKey()
        {
            using (var db = new TrackerDataContext())
            {
                var q1 = db.User.ByFirstName("James");
                var q2 = db.User.ByFirstName("Spock");

                var q3 = db.User.Where(u => u.FirstName == "James");
                var q4 = db.User.Where(u => u.FirstName == "Spock");

                var s1 = "James";
                var s2 = "Spock";
                var q5 = db.User.Where(u => u.FirstName == s1);
                var q6 = db.User.Where(u => u.FirstName == s2);
                var q7 = GetWhere(db, s1);
                var q8 = GetWhere(db, s2);

                var h1 = q1.GetHashKey();
                var h2 = q2.GetHashKey();
                var h3 = q3.GetHashKey();
                var h4 = q4.GetHashKey();
                var h5 = q5.GetHashKey();
                var h6 = q6.GetHashKey();
                var h7 = q7.GetHashKey();
                var h8 = q8.GetHashKey();

                Assert.AreNotEqual(h1, h2);

                Assert.AreEqual(h1, h3);
                Assert.AreEqual(h1, h5);
                Assert.AreEqual(h1, h7);

                Assert.AreEqual(h2, h4);
                Assert.AreEqual(h2, h6);
                Assert.AreEqual(h2, h8);
            }
        }

        private IQueryable<User> GetWhere(TrackerDataContext db, string s)
        {
            return db.User.Where(u => u.FirstName == s);
        }

        [Test]
        public void GetDataContext()
        {
            using (var db1 = new TrackerDataContext())
            {
                var q = db1.User.ByFirstName("James");
                var db2 = q.GetDataContext();

                Assert.AreSame(db1, db2);
            }

            using (var db3 = new TrackerDataContext { ObjectTrackingEnabled = false })
            {
                var q = db3.User.ByFirstName("James");
                var db4 = q.GetDataContext();

                Assert.AreSame(db3, db4);
            }
        }

        [Test]
        public void Future()
        {
            using (var db = new TrackerDataContext())
            {
                var users = db.User
                    .ByFirstName(ContainmentOperator.NotEquals, "James")
                    .OrderBy(u => u.Id)
                    .Future();

                var user = db.User
                    .ByFirstName(ContainmentOperator.NotEquals, "James")
                    .OrderBy(u => u.Id)
                    .FutureFirstOrDefault();

                var value = users.FirstOrDefault();

                Assert.AreEqual(value.Id, user.Value.Id);
            }
        }

        [Test]
        public void FutureCache()
        {
            using (var db = new TrackerDataContext())
            {
                var u1 = db.User
                    .ByFirstName(ContainmentOperator.NotEquals, "James")
                    .OrderBy(u => u.Id)
                    .FutureCache();

                var u2 = db.User
                    .ByFirstName(ContainmentOperator.NotEquals, "James")
                    .OrderBy(u => u.Id)
                    .FutureCache();

                Assert.AreEqual(u1.First(), u2.First());

                db.User
                    .ByFirstName(ContainmentOperator.NotEquals, "James")
                    .OrderBy(u => u.Id)
                    .ClearCache();

                var u3 = db.User
                    .ByFirstName(ContainmentOperator.NotEquals, "James")
                    .OrderBy(u => u.Id)
                    .FutureCache();

                Assert.AreEqual(u1.First(), u3.First());

                var u4 = db.User
                    .ByFirstName(ContainmentOperator.NotEquals, "James")
                    .OrderBy(u => u.Id)
                    .FutureCache();

                Assert.AreEqual(u3.First(), u4.First());
            }
        }

        [Test]
        public void QueryByAssociation()
        {
            using (var db = new TrackerDataContext())
            {
                var count1 = db.Task.Where(t => t.Priority.Id == 1).Count();
                var count2 = db.Task.ByPriority(1).Count();

                Assert.AreEqual(count1, count2);

                var count3 = db.Task.Where(t => t.Priority.Id != 1).Count();
                var count4 = db.Task.ByPriority(ComparisonOperator.NotEquals, 1).Count();

                Assert.AreEqual(count3, count4);

                var priority = db.Priority.GetByKey(1);
                var count5 = db.Task.ByPriority(priority).Count();
                var count6 = db.Task.ByPriority(ComparisonOperator.NotEquals, priority).Count();

                Assert.AreEqual(count1, count5);
                Assert.AreEqual(count3, count6);
            }
        }

        [Test]
        public void Fetch()
        {
            User user1, user2;
            using (var db = new TrackerDataContext())
            {
                user1 = db.User
                    .Skip(1)
                    .FirstOrDefault();

                user2 = db.User
                    .FetchMany(u => u.TaskAssignedList)
                    .FirstOrDefault();

                db.User.Detach(user1);
                db.User.Detach(user2);
            }

            Assert.AreEqual(0, user1.TaskAssignedList.Count);
            Assert.AreEqual(1, user2.TaskAssignedList.Count);
        }

        [Test]
        public void Self()
        {
            int parentId;

            using (var db = new TrackerDataContext())
            {
                var parent = new Self {Name = "Parent"};
                db.Self.InsertOnSubmit(parent);
                db.SubmitChanges();

                parentId = parent.Id;

                var child = new Self {Name = "Child", MySelf = parent};
                db.Self.InsertOnSubmit(child);
                db.SubmitChanges();
            }

            using (var db = new TrackerDataContext())
            {
                var child = db.Self
                    .ByName("Child")
                    .FirstOrDefault();

                Assert.AreEqual(parentId, child.MySelf.Id);

                db.Self.DeleteOnSubmit(child);
                db.Self.DeleteOnSubmit(child.MySelf);
                db.SubmitChanges();
            }
        }
    }
}
