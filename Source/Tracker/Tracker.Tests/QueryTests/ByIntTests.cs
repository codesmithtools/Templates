using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tracker.Data;

namespace Tracker.Tests.QueryTests
{
    [TestFixture]
    public class ByIntTests
    {
        private List<int> _userIds;
        private List<int> _taskIds = new List<int>();

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            using (var db = new TrackerDataContext())
            {
                _userIds = db.User.Select(u => u.Id).Take(3).ToList();
                var statusId = db.Status.Select(s => s.Id).First();

                var task1 = new Task
                {
                    CreatedId = _userIds[0],
                    PriorityId = Priority.Normal,
                    StatusId = statusId,
                    Summary = "Test",
                    AssignedId = null
                };
                var task2 = new Task
                {
                    CreatedId = _userIds[1],
                    PriorityId = Priority.Normal,
                    StatusId = statusId,
                    Summary = "Test",
                    AssignedId = _userIds[0]
                };
                var task3 = new Task
                {
                    CreatedId = _userIds[2],
                    PriorityId = Priority.Normal,
                    StatusId = statusId,
                    Summary = "Test",
                    AssignedId = _userIds[1]
                };

                db.Task.InsertOnSubmit(task1);
                db.Task.InsertOnSubmit(task2);
                db.Task.InsertOnSubmit(task3);
                db.SubmitChanges();

                _taskIds.Add(task1.Id);
                _taskIds.Add(task2.Id);
                _taskIds.Add(task3.Id);
            }
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            using (var db = new TrackerDataContext())
            {
                db.Task.Delete(_taskIds[0]);
                db.Task.Delete(_taskIds[1]);
                db.Task.Delete(_taskIds[2]);
            }
        }

        [Test]
        public void IntTest()
        {
            try
            {
                using (var db = new TrackerDataContext())
                {
                    var a = db.Task.ByCreatedId(_userIds[1]).ToList();
                    var b = db.Task.ByCreatedId(_userIds[2]).ToList();

                    var c = db.Task.ByCreatedId(_userIds[1], null).ToList();
                    Assert.AreEqual(a.Count, c.Count);

                    var d = db.Task.ByCreatedId(_userIds[1], _userIds[2]).ToList();
                    Assert.AreEqual(a.Count + b.Count, d.Count);
                }
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Test]
        public void NullableIntTest()
        {
            try
            {
                using (var db = new TrackerDataContext())
                {
                    var a = db.Task.ByAssignedId(null).ToList();
                    var b = db.Task.ByAssignedId(_userIds[1]).ToList();
                    var c = db.Task.ByAssignedId(_userIds[2]).ToList();

                    var d = db.Task.ByAssignedId(_userIds[1], null).ToList();
                    Assert.AreEqual(a.Count + b.Count, d.Count);

                    var e = db.Task.ByAssignedId(_userIds[1], null, _userIds[2]).ToList();
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
