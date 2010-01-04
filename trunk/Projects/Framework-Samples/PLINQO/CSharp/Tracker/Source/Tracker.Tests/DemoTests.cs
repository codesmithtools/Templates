using System;
using System.Collections.Generic;
using System.Linq;
using CodeSmith.Data.Rules;
using HibernatingRhinos.Profiler.Appender.LinqToSql;
using NUnit.Framework;
using Tracker.Core.Data;
using System.Data.Linq;
using CodeSmith.Data.Linq;

namespace Tracker.Tests
{
    [TestFixture]
    public class DemoTests
    {

        #region Setup
        private const int STATUS_NOT_STARTED = 1;
        private const int STATUS_DONE = 6;
        private const int ROLE_MANAGER = 2;
        private int SpockId
        {
            get;
            set;
        }
        private int JamesId
        { get; set; }

        private int TaskId
        {
            get;
            set;
        }

        [SetUp]
        public void Setup()
        {
            LinqToSqlProfiler.SetupProfilingFor<TrackerDataContext>(TrackerDataContext.MappingCache);
            LinqToSqlProfiler.Initialize();
            TearDown();
            CreateUsers();
            CreateTask(SpockId);
        }

        [TearDown]
        public void TearDown()
        {
            using (var db = new TrackerDataContext { Log = Console.Out })
            {
                db.Audit.Delete(a => a.User.EmailAddress.EndsWith("startrek.com"));
                db.UserRole.Delete(ur => ur.User.EmailAddress.EndsWith("startrek.com"));
                db.Task.Delete(t =>
                    t.AssignedUser.EmailAddress.EndsWith("startrek.com") ||
                    t.CreatedUser.EmailAddress.EndsWith("startrek.com"));
                db.User.Delete(u => u.EmailAddress.EndsWith("startrek.com"));
            }
        }

        private void CreateTask(int userId)
        {
            var task = new Task()
            {
                AssignedId = userId,
                Status = Status.InProgress,
                Summary = "Explain the visions that inspire you",
                Priority = Priority.High,
                CreatedId = userId,
            };
            using (var db = new TrackerDataContext { Log = Console.Out })
            {
                db.Task.InsertOnSubmit(task);
                db.SubmitChanges();
                TaskId = task.Id;
            }
        }

        private void CreateUsers()
        {
            var user = new User()
            {
                FirstName = "Spock",
                LastName = "Sarekson",
                PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA",
                PasswordSalt = "=Unc%",
                EmailAddress = "spock@startrek.com",
                IsApproved = true
            };

            using (var db = new TrackerDataContext { Log = Console.Out })
            {
                db.User.InsertOnSubmit(user);
                db.SubmitChanges();
                SpockId = user.Id;
            }

            var users = new List<User>();

            users.Add(new User()
            {
                FirstName = "James",
                LastName = "Kirk",
                PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA",
                PasswordSalt = "=Unc%",
                EmailAddress = "james.tiberius.kirk@startrek.com",
                IsApproved = true
            });

            users.Add(new User()
            {
                FirstName = "Bones",
                LastName = "McCoy",
                PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA",
                PasswordSalt = "=Unc%",
                EmailAddress = "bones.mccory@startrek.com",
                IsApproved = false
            });

            users.Add(new User()
            {
                FirstName = "Nyota",
                LastName = "Uhura",
                PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA",
                PasswordSalt = "=Unc%",
                EmailAddress = "uhura@startrek.com",
                IsApproved = false
            });

            users.Add(new User()
            {
                FirstName = "Ellen",
                LastName = "Tigh",
                PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA",
                PasswordSalt = "=Unc%",
                EmailAddress = "ellen.tigh@startrek.com",
                IsApproved = false
            });

            using (var db = new TrackerDataContext { Log = Console.Out })
            {
                db.User.InsertAllOnSubmit(users);
                db.SubmitChanges();
                JamesId = db.User.ByFirstName("James").ByLastName("Kirk").First().Id;
            }
        }

        #endregion

        [Test]
        public void Test_Manager_And_Query_Gets()
        {
            using (var db = new TrackerDataContext { Log = Console.Out })
            {
                Task task = db.Manager.Task.GetByKey(SpockId);
                IQueryable<Task> tasks = db.Manager.Task.GetByAssignedIdStatus(SpockId, Status.InProgress);
                List<Task> taskList = tasks.ToList();
            }

            using (var db = new TrackerDataContext { Log = Console.Out })
            {
                try
                {
                    Task task = db.Task.GetByKey(SpockId);
                    IQueryable<Task> tasks = db.Task.ByAssignedId(SpockId).ByStatus(Status.InProgress);
                    //tasks = tasks.OrderBy(t => t.CompleteDate);
                    var list = tasks.ToPagedList<Task>(1, 10);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        [Test]
        public void Test_Query_Advanced()
        {
            using (var db = new TrackerDataContext { Log = Console.Out })
            {
                var priorites = db.Task.Where(t => !t.Priority.HasValue || t.Priority.Value == Priority.High).ToList();
                var something = db.Task.ByPriority(null, Priority.High).ToList();
            }
        }

        [Test]
        public void Test_Many_To_Many()
        {
            using (var db = new TrackerDataContext { Log = Console.Out })
            {
                User u = db.User.GetByKey(SpockId);
                Role r = db.Role.ByName("Manager").First();
                u.RoleList.Add(r);
                db.SubmitChanges();
            }
        }

        [Test]
        public void Test_Enum()
        {
            using (var db = new TrackerDataContext { Log = Console.Out })
            {
                var task = db.Task.GetByKey(TaskId);
                task.Priority = Priority.High;
                db.SubmitChanges();
            }
        }

        [Test]
        public void Test_Auditing()
        {
            using (var db = new TrackerDataContext { Log = Console.Out })
            {
                var user = db.User.GetByKey(SpockId);
                user.Comment = "I love my mom, but I hate Winona Ryder.";

                var task = new Task()
                {
                    AssignedId = SpockId,
                    CreatedId = SpockId,
                    Status = Status.NotStarted,
                    Priority = Priority.High,
                    Summary = "Punch Kirk in the face!"
                };
                db.Task.InsertOnSubmit(task);
                db.SubmitChanges();
                Console.Write(db.LastAudit.ToXml());
            }
        }

        [Test]
        public void Test_Rules()
        {
            int brokenRules = 0;
            try
            {
                using (var db = new TrackerDataContext { Log = Console.Out })
                {
                    User user = new User();
                    user.EmailAddress = "spock@startrek.com";
                    db.User.InsertOnSubmit(user);
                    db.SubmitChanges();
                }
            }
            catch (BrokenRuleException e)
            {
                brokenRules = e.BrokenRules.Count;
                Console.Write(e.ToString());
            }
            Assert.AreEqual(brokenRules, 5);
        }

        [Test]
        public void Test_Entity_Detach()
        {
            Task task = null;
            using (var db = new TrackerDataContext { Log = Console.Out })
            {
                task = db.Task.GetByKey(TaskId);
                task.Detach();
            }

            task.Status = Status.Done;

            using (var context2 = new TrackerDataContext())
            {
                context2.Log = Console.Out;

                context2.Task.Attach(task, true);
                context2.SubmitChanges();
            }
        }

        [Test]
        public void Test_Entity_Detach_Update()
        {
            Task task = null;
            using (var db = new TrackerDataContext { Log = Console.Out })
            {
                task = db.Task.GetByKey(TaskId);
                task.Detach();
            }

            using (var context2 = new TrackerDataContext())
            {
                context2.Log = Console.Out;
                // attach, then update properties
                context2.Task.Attach(task);
                task.Status = Status.Done;

                context2.SubmitChanges();
            }
        }

        [Test]
        public void Test_Entity_Clone()
        {
            using (var db = new TrackerDataContext { Log = Console.Out })
            {
                var u = db.Task.GetByKey(TaskId);
                Task taskCopy = u.Clone();
                taskCopy.Id = 0;
                db.Task.InsertOnSubmit(taskCopy);
                db.SubmitChanges();
            }
        }

        [Test]
        public void Test_Serialization()
        {
            using (var db = new TrackerDataContext { Log = Console.Out })
            {
                // Write to Console/Output ... or break point.

                var task = db.Task.GetByKey(TaskId);
                var s = task.ToXml();
                Console.Write(s);
            }
        }

        [Test]
        public void Test_Query_Result_Cache()
        {
            using (var db = new TrackerDataContext { Log = Console.Out })
            {
                //By default, the cached results use a one minute sliding expiration with
                //no absolute expiration.
                var tasks = db.Task.ByAssignedId(SpockId).FromCache();
                var cachedTasks = db.Task.ByAssignedId(SpockId).FromCache();

                //query result is now cached 300 seconds
                var approvedUsers = db.User.ByIsApproved(true).FromCache(300);
                var cachedApprovedUsers = db.User.ByIsApproved(true).FromCache(300);
            }
        }

        [Test]
        public void Test_Batch_Update()
        {
            using (var db = new TrackerDataContext { Log = Console.Out })
            {
                db.Task.Update(
                    u => u.Status == Status.NotStarted,
                    u2 => new Task() { Status = Status.Done });

                var tasks = from t in db.Task
                            where t.Status == Status.Done
                            select t;
                db.Task.Update(tasks, u => new Task { Status = Status.NotStarted });

            }
        }

        [Test]
        public void Test_Batch_Delete()
        {
            using (var db = new TrackerDataContext { Log = Console.Out })
            {
                db.User.Delete(JamesId);

                db.User.Delete(u => u.IsApproved == false && u.LastName != "McCoy" && u.EmailAddress.EndsWith("startrek.com"));

                IQueryable<User> usersToDelete = from u in db.User
                                                 where u.IsApproved == false && u.LastName == "McCoy"
                                                 select u;
                db.User.Delete(usersToDelete);
            }
        }

        [Test]
        public void Test_Stored_Procedure_with_Multiple_Results()
        {
            using (var db = new TrackerDataContext { Log = Console.Out })
            {
                // Create Procedure [dbo].[GetUsersWithRoles]
                // As
                // Select * From [User]
                // Select * From UserRole
                // GO

                var results = db.GetUsersWithRoles();
                List<User> users = results.GetResult<User>().ToList();
                List<UserRole> roles = results.GetResult<UserRole>().ToList();
            }
        }

        [Test]
        public void Test_Batch_Queries()
        {
            using (var db = new TrackerDataContext { Log = Console.Out })
            {
                var q1 = from u in db.User select u;
                var q2 = from ur in db.UserRole select ur;
                IMultipleResults results = db.ExecuteQuery(q1, q2);
                List<User> users = results.GetResult<User>().ToList();
                List<UserRole> roles = results.GetResult<UserRole>().ToList();

            }
        }

        [Test]
        public void Test_ToPagedList()
        {
            using (var db = new TrackerDataContext { Log = Console.Out })
            {
                var q1 = from u in db.User 
                         orderby u.EmailAddress
                         select u;

                var users = q1.ToPagedList(0, 5);

                Assert.IsNotNull(users);
                Assert.AreEqual(5, users.Count);
            }
        }
    }
}
