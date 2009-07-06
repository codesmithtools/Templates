using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeSmith.Data.Audit;
using CodeSmith.Data.Rules;
using NUnit.Framework;
using PLINQO.Tracker.Data;
using System.Data.Linq;

namespace PLINQO.Tracker.Tests
{
    [TestFixture]
    public class PLINQOTests
    {

        #region Setup
        private const int STATUS_NOT_STARTED = 1;
        private const int STATUS_DONE = 6;
        private const int ROLE_MANAGER = 2;
        private int UserId
        {
            get;
            set;
        }
        private int WilliamId
        { get; set; }

        private int TaskId
        {
            get;
            set;
        }

        [SetUp]
        public void Setup()
        {
            CreateUsers();
            CreateTask(UserId);
        }

        [TearDown]
        public void TearDown()
        {
            using (var context = new TrackerDataContext())
            {
                context.UserRole.Delete(r => r.UserId == UserId);
                context.UserRole.Delete(r => r.User.EmailAddress.EndsWith("battlestar.com") && r.UserId == UserId);
                context.Audit.Delete(a => a.TaskId == TaskId || a.UserId == UserId);
                context.Task.Delete(a => a.CreatedId == UserId || a.AssignedId == UserId);
                context.Task.Delete(TaskId);
                context.User.Delete(UserId);
                context.User.Delete(u => u.EmailAddress.EndsWith("battlestar.com"));
            }
        }

        private void CreateTask(int userId)
        {
            var task = new Task()
            {
                AssignedId = userId,
                StatusId = 1,
                Summary = "Explain the visions that inspire you",
                PriorityId = Priority.High,
                CreatedId = userId,
            };
            using (var context = new TrackerDataContext())
            {
                context.Task.InsertOnSubmit(task);
                context.SubmitChanges();
                TaskId = task.Id;
            }
        }

        private void CreateUsers()
        {
            var user = new User()
            {
                FirstName = "Laura",
                LastName = "Roslin",
                PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA",
                PasswordSalt = "=Unc%",
                EmailAddress = "laura.roslin@battlestar.com",
                IsApproved = true
            };
            
            using (var context = new TrackerDataContext())
            {
                context.User.InsertOnSubmit(user);
                context.SubmitChanges();
                UserId = user.Id;
            }

            var users = new List<User>();

            users.Add(new User()
            {
                FirstName = "William",
                LastName = "Adama",
                PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA",
                PasswordSalt = "=Unc%",
                EmailAddress = "william.adama@battlestar.com",
                IsApproved = true
            });

            users.Add(new User()
            {
                FirstName = "Kara",
                LastName = "Starbuck",
                PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA",
                PasswordSalt = "=Unc%",
                EmailAddress = "kara.starbuck@battlestar.com",
                IsApproved = false
            });

            users.Add(new User()
            {
                FirstName = "Saul",
                LastName = "Tigh",
                PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA",
                PasswordSalt = "=Unc%",
                EmailAddress = "saul.tigh@battlestar.com",
                IsApproved = false
            });

            users.Add(new User()
            {
                FirstName = "Ellen",
                LastName = "Tigh",
                PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA",
                PasswordSalt = "=Unc%",
                EmailAddress = "ellen.tigh@battlestar.com",
                IsApproved = false
            });

            using (var context = new TrackerDataContext())
            {
                context.User.InsertAllOnSubmit(users);
                context.SubmitChanges();
                WilliamId = context.User.ByFirstName("William").ByLastName("Adama").First().Id;
            }
        }

        #endregion

        [Test]
        public void Test_Manager_And_Query_Gets()
        {
            using (var context = new TrackerDataContext())
            {
                context.Log = Console.Out;

                Task task = context.Manager.Task.GetByKey(UserId);
                IQueryable<Task> tasks = context.Manager.Task.GetByAssignedIdStatusId(UserId, STATUS_NOT_STARTED);
                List<Task> taskList = tasks.ToList();
            }

            using (var context = new TrackerDataContext())
            {
                context.Log = Console.Out;

                Task task = context.Task.ByKey(UserId);
                IQueryable<Task> tasks = context.Task.ByAssignedId(UserId).ByStatusId(STATUS_NOT_STARTED);
                List<Task> taskList = tasks.ToList();
            }
        }

        [Test]
        public void Test_Many_To_Many()
        {
            using (var context = new TrackerDataContext())
            {
                context.Log = Console.Out;

                User u = context.User.ByKey(UserId);
                Role r = context.Role.ByName("Manager").First();
                u.RoleList.Add(r);
                context.SubmitChanges();
           }
        }

        [Test]
        public void Test_Enum()
        {
            using (var context = new TrackerDataContext())
            {
                context.Log = Console.Out;

                var task = context.Task.ByKey(TaskId);
                task.PriorityId = Priority.High;
                context.SubmitChanges();
            }
        }

        [Test]
        public void Test_Auditing()
        {
            using (var context = new TrackerDataContext())
            {
                context.Log = Console.Out;

                var user = context.User.ByKey(UserId);
                user.Comment = "Vote Baltar and move to New Caprica";

                var task = new Task()
                {
                    AssignedId = UserId,
                    CreatedId = UserId,
                    StatusId = STATUS_NOT_STARTED,
                    PriorityId = Priority.High,
                    Summary = "Rig the election"
                };
                context.Task.InsertOnSubmit(task);
                context.SubmitChanges();
                Console.Write(context.LastAudit.ToXml());
            }
        }

        [Test]
        public void Test_Rules()
        {
            int brokenRules = 0;
            try
            {
                using (var context = new TrackerDataContext())
                {
                    context.Log = Console.Out;

                    User user = new User();
                    context.User.InsertOnSubmit(user);
                    context.SubmitChanges();
                }
            }
            catch (BrokenRuleException e)
            {
                brokenRules = e.BrokenRules.Count;
                Console.Write(e.ToString());
            }
            Assert.AreEqual(brokenRules, 4);
        }

        [Test]
        public void Test_Entity_Detach()
        {
            Task task = null;
            using (var context = new TrackerDataContext())
            {
                context.Log = Console.Out;

                task = context.Task.ByKey(TaskId);
                task.Detach();
            }

            task.StatusId = STATUS_DONE;

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
            using (var context = new TrackerDataContext())
            {
                context.Log = Console.Out; 
                
                task = context.Task.ByKey(TaskId);
                task.Detach();
            }

            using (var context2 = new TrackerDataContext())
            {
                context2.Log = Console.Out;
                // attach, then update properties
                context2.Task.Attach(task);
                task.StatusId = STATUS_DONE;

                context2.SubmitChanges();
            }
        }

        [Test]
        public void Test_Entity_Clone()
        {
            using (var context = new TrackerDataContext())
            {
                context.Log = Console.Out;

                var u = context.Task.ByKey(TaskId);
                Task taskCopy = u.Clone();
                taskCopy.Id = 0;
                context.Task.InsertOnSubmit(taskCopy);
                context.SubmitChanges();
            }
        }

        [Test]
        public void Test_Serialization()
        {
            using (var context = new TrackerDataContext())
            {
                context.Log = Console.Out;

                // Write to Console/Output ... or break point.

                var task = context.Task.ByKey(TaskId);
                var s = task.ToXml();
                Console.Write(s);
            }
        }

        [Test]
        public void Test_Batch_Update()
        {
            using (var context = new TrackerDataContext())
            {
                context.Log = Console.Out;
                
                context.Task.Update(
                    u => u.StatusId == STATUS_NOT_STARTED, 
                    u2 => new Task() { StatusId = STATUS_DONE });

                var tasks = from t in context.Task
                            where t.StatusId == STATUS_DONE
                            select t;
                context.Task.Update(tasks, u => new Task { StatusId = STATUS_NOT_STARTED });

            }
        }

        [Test]
        public void Test_Batch_Delete()
        {
            using (var context = new TrackerDataContext())
            {
                context.Log = Console.Out;

                
                context.User.Delete(WilliamId);

                context.User.Delete(u => u.IsApproved == false && u.LastName != "Tigh" && u.EmailAddress.EndsWith("battlestar.com"));

                IQueryable<User> usersToDelete = from u in context.User 
                                                 where u.IsApproved == false && u.LastName == "Tigh" 
                                                 select u;
                context.User.Delete(usersToDelete);
            }
        }

        [Test]
        public void Test_Stored_Procedure_with_Multiple_Results()
        {
            using (var context = new TrackerDataContext())
            {
                context.Log = Console.Out;

                // Create Procedure [dbo].[GetUsersWithRoles]
                // As
                // Select * From [User]
                // Select * From UserRole
                // GO

                var results = context.GetUsersWithRoles();
                List<User> users = results.GetResult<User>().ToList();
                List<UserRole> roles = results.GetResult<UserRole>().ToList();
            }
        }

        [Test]
        public void Test_Batch_Queries()
        {
            using (var context = new TrackerDataContext())
            {
                context.Log = Console.Out;

                var q1 = from u in context.User select u;
                var q2 = from ur in context.UserRole select ur;
                IMultipleResults results = context.ExecuteQuery(q1, q2);
                List<User> users = results.GetResult<User>().ToList();
                List<UserRole> roles = results.GetResult<UserRole>().ToList();
            }
        }

    }
}
