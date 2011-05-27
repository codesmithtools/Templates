using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Plinqo.NHibernate;
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
                    PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA",
                    PasswordSalt = "=Unc%",
                    IsApproved = true,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    LastActivityDate = DateTime.Now,
                    LastLoginDate = DateTime.Now,
                    LastPasswordChangeDate = DateTime.Now
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

                var role = user.RoleList.First();
                user.RoleList.Remove(role);
                db.SubmitChanges();

                Assert.AreEqual(0, user.RoleList.Count);
            }

            using (var db = new TrackerDataContext())
            {
                var user = db.User.GetByKey(UserId);

                Assert.AreEqual(0, user.RoleList.Count);
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
                var a = db.GetRolesForUser(UserId);

                Assert.AreEqual(1, a.Count);
            }
        }
    }
}
