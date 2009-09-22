using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tracker.Data;

namespace Tracker.Tests.QueryTests
{
    [TestFixture]
    public class ByDateTimeTests
    {
        private DateTime _dueDate1 = DateTime.Today.AddDays(1);
        private DateTime _dueDate2 = DateTime.Today.AddDays(2);
        private DateTime _createdDate1;
        private DateTime _createdDate2;
        private int _taskId1;
        private int _taskId2;
        private int _taskId3;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            using (var db = new TrackerDataContext())
            {
                var userId = db.User.Select(u => u.Id).First();
                var statusId = db.Status.Select(s => s.Id).First();

                var task1 = new Task
                {
                    CreatedId = userId,
                    PriorityId = Priority.Normal,
                    StatusId = statusId,
                    Summary = "Test",
                    DueDate = null
                };
                var task2 = new Task
                {
                    CreatedId = userId,
                    PriorityId = Priority.Normal,
                    StatusId = statusId,
                    Summary = "Test",
                    DueDate = _dueDate1
                };
                var task3 = new Task
                {
                    CreatedId = userId,
                    PriorityId = Priority.Normal,
                    StatusId = statusId,
                    Summary = "Test",
                    DueDate = _dueDate2
                };

                db.Task.InsertOnSubmit(task1);
                db.SubmitChanges();
                System.Threading.Thread.Sleep(1000);
                db.Task.InsertOnSubmit(task2);
                db.Task.InsertOnSubmit(task3);
                db.SubmitChanges();

                _taskId1 = task1.Id;
                _createdDate1 = task1.CreatedDate;
                _taskId2 = task2.Id;
                _createdDate2 = task2.CreatedDate;
                _taskId3 = task3.Id;
            }
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            using (var db = new TrackerDataContext())
            {
                db.Task.Delete(_taskId1);
                db.Task.Delete(_taskId2);
                db.Task.Delete(_taskId3);
            }
        }

        [Test]
        public void ByTest()
        {
            try
            {
                using (var db = new TrackerDataContext())
                {
                    var a = db.Task.ByCreatedDate(_createdDate1).ToList();
                    var b = db.Task.ByCreatedDate(_createdDate2).ToList();

                    var c = db.Task.ByCreatedDate(_createdDate1, null).ToList();
                    Assert.AreEqual(a.Count, c.Count);

                    var d = db.Task.ByCreatedDate(_createdDate1, _createdDate2).ToList();
                    Assert.AreEqual(a.Count + b.Count, d.Count);
                }
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Test]
        public void ByNullableTest()
        {
            try
            {
                using (var db = new TrackerDataContext())
                {
                    var a = db.Task.ByDueDate(null).ToList();
                    var b = db.Task.ByDueDate(_dueDate1).ToList();
                    var c = db.Task.ByDueDate(_dueDate2).ToList();

                    var d = db.Task.ByDueDate(_dueDate1, null).ToList();
                    Assert.AreEqual(a.Count + b.Count, d.Count);

                    var e = db.Task.ByDueDate(_dueDate1, null, _dueDate2).ToList();
                    Assert.AreEqual(a.Count + b.Count + c.Count, e.Count);
                }
            }
            catch
            {
                Assert.Fail();
            }
        }
    }
}
