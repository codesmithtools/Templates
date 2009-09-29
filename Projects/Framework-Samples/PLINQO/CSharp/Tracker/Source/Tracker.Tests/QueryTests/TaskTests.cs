using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using NUnit.Framework;
using Tracker.Core.Data;

namespace Tracker.Tests.QueryTests
{
    public abstract class TaskTests
    {
        protected List<DateTime> dueDates = new List<DateTime>();
        protected List<DateTime> createDates = new List<DateTime>();
        protected List<int> userIds;
        private List<int> _taskIds = new List<int>();

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            using (var db = new TrackerDataContext())
            {
                dueDates.Add(DateTime.Today.AddDays(1));
                dueDates.Add(DateTime.Today.AddDays(2));

                userIds = db.User.Select(u => u.Id).Take(3).ToList();

                var task1 = new Task
                {
                    CreatedId = userIds[0],
                    Priority = null,
                    Status = Status.NotStarted,
                    Summary = "Test",
                    AssignedId = null,
                    DueDate = null,
                    Details = null
                };
                var task2 = new Task
                {
                    CreatedId = userIds[1],
                    Priority = Priority.Normal,
                    Status = Status.InProgress,
                    Summary = "Test",
                    AssignedId = userIds[0],
                    DueDate = dueDates[0],
                    Details = String.Empty
                };
                var task3 = new Task
                {
                    CreatedId = userIds[2],
                    Priority = Priority.High,
                    Status = Status.Completed,
                    Summary = "Test",
                    AssignedId = userIds[1],
                    DueDate = dueDates[1],
                    Details = "Hello world!"
                };
                var task4 = new Task
                {
                    CreatedId = userIds[2],
                    Priority = Priority.High,
                    Status = Status.Completed,
                    Summary = "Test",
                    AssignedId = userIds[1],
                    DueDate = null,
                    Details = "Goodnight moon!"
                };

                db.Task.InsertOnSubmit(task1);
                db.SubmitChanges();
                System.Threading.Thread.Sleep(1000);
                db.Task.InsertOnSubmit(task2);
                db.Task.InsertOnSubmit(task3);
                db.SubmitChanges();

                _taskIds.Add(task1.Id);
                _taskIds.Add(task2.Id);
                _taskIds.Add(task3.Id);
                _taskIds.Add(task4.Id);
                createDates.Add(task1.CreatedDate);
                createDates.Add(task2.CreatedDate);
                createDates.Add(task3.CreatedDate);
                createDates.Add(task4.CreatedDate);
            }
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            using (var db = new TrackerDataContext())
                db.Task.Delete(t => _taskIds.Contains(t.Id));
        }

    }
}
