using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
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
        private List<int> _taskIds = new List<int>();
        
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

                _taskIds.Add(task1.Id);
                _createdDate1 = task1.CreatedDate;
                _taskIds.Add(task2.Id);
                _createdDate2 = task2.CreatedDate;
                _taskIds.Add(task3.Id);
            }
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            using (var db = new TrackerDataContext())
                db.Task.Delete(t => _taskIds.Contains(t.Id));
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
            catch (AssertionException)
            {
                throw;
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
            catch (AssertionException)
            {
                throw;
            }
            catch
            {
                Assert.Fail();
            }
        }
    }
}
