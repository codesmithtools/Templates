using System;
using System.Collections.Generic;
using System.Linq;
using CodeSmith.Data.Linq;
using NUnit.Framework;
using Tracker.Core.Data;

namespace Tracker.Tests
{
    public abstract class TestBase
    {
        protected List<DateTime> DueDates = new List<DateTime>();
        protected List<DateTime> CreateDates = new List<DateTime>();

        protected List<int> UserIds = new List<int>();
        protected List<int> TaskIds = new List<int>();

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            using (var db = new TrackerDataContext {Log = Console.Out})
            {
                var users = new List<User>
                {
                    new User
                    {
                        FirstName = "Testie McTester",
                        LastName = "The First",
                        PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA",
                        PasswordSalt = "=Unc%",
                        EmailAddress = "one@test.com",
                        IsApproved = false
                    }, new User
                    {
                        FirstName = "Testie McTester",
                        LastName = "The Second",
                        PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA",
                        PasswordSalt = "=Unc%",
                        EmailAddress = "two@test.com",
                        IsApproved = false
                    }, new User
                    {
                        FirstName = "Testie McTester",
                        LastName = "The Third",
                        PasswordHash = "aM/Vndh7cYd3Mxq7msArjl9YU8zoR6fF+sVTSUCcsJi2bx+cwOI0/Bkr5hfq9vYfTe3/rlgPpSMg108acpw+qA",
                        PasswordSalt = "=Unc%",
                        EmailAddress = "three@test.com",
                        IsApproved = false
                    }
                };

                var emails = users.Select(u => u.EmailAddress).ToArray();

                var existing = from u in db.User
                               where emails.Contains(u.EmailAddress)
                               select u;

                var existingUsers = existing.Select(u => u.Id).ToList();

                var testTasks = from t in db.Task
                                where existingUsers.Contains(t.CreatedId) 
                                    || existingUsers.Contains(t.AssignedId.Value)
                                select t;

                db.Task.Delete(testTasks);
                db.User.Delete(existing);

                db.User.InsertAllOnSubmit(users);
                db.SubmitChanges();
                
                UserIds = users.Select(u => u.Id).ToList();
                DueDates.Add(DateTime.Today.AddDays(1));
                DueDates.Add(DateTime.Today.AddDays(2));

                var tasks = new List<Task>();
                tasks.Add(new Task
                {
                    CreatedId = UserIds[0],
                    Priority = Priority.Normal,
                    Status = Status.NotStarted,
                    Summary = "Test",
                    AssignedId = null,
                    DueDate = null,
                    Details = null
                });
                tasks.Add(new Task
                {
                    CreatedId = UserIds[1],
                    Priority = Priority.Normal,
                    Status = Status.InProgress,
                    Summary = "Test",
                    AssignedId = UserIds[0],
                    DueDate = DueDates[0],
                    Details = String.Empty
                });
                tasks.Add(new Task
                {
                    CreatedId = UserIds[2],
                    Priority = Priority.High,
                    Status = Status.Completed,
                    Summary = "Test",
                    AssignedId = UserIds[1],
                    DueDate = DueDates[1],
                    Details = "Hello world!"
                });
                tasks.Add(new Task
                {
                    CreatedId = UserIds[2],
                    Priority = Priority.High,
                    Status = Status.Completed,
                    Summary = "Test",
                    AssignedId = UserIds[1],
                    DueDate = null,
                    Details = "Goodnight moon!"
                });

                db.Task.InsertAllOnSubmit(tasks);
                db.SubmitChanges();

                TaskIds = tasks.Select(t => t.Id).ToList();
                CreateDates = tasks.Select(t => t.CreatedDate).ToList();
            }
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            using (var db = new TrackerDataContext {Log = Console.Out})
            {
                db.Task.Delete(t => TaskIds.Contains(t.Id));
                db.User.Delete(u => UserIds.Contains(u.Id));
            }
        }
    }
}
