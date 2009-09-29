using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using NUnit.Framework;
using Tracker.Data;

namespace Tracker.Tests.QueryTests
{
    [TestFixture]
    public class ByStringTests
    {
        private string _details1 = String.Empty;
        private string _details2 = "Hello world!";
        private string _details3 = "Good night moon!";
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
                    Details = null
                };
                var task2 = new Task
                {
                    CreatedId = userId,
                    PriorityId = Priority.Normal,
                    StatusId = statusId,
                    Summary = "Test",
                    Details = _details1
                };
                var task3 = new Task
                {
                    CreatedId = userId,
                    PriorityId = Priority.Normal,
                    StatusId = statusId,
                    Summary = "Test",
                    Details = _details2
                };
                var task4 = new Task
                {
                    CreatedId = userId,
                    PriorityId = Priority.Normal,
                    StatusId = statusId,
                    Summary = "Test",
                    Details = _details3
                };

                db.Task.InsertOnSubmit(task1);
                db.Task.InsertOnSubmit(task2);
                db.Task.InsertOnSubmit(task3);
                db.Task.InsertOnSubmit(task4);
                db.SubmitChanges();

                _taskIds.Add(task1.Id);
                _taskIds.Add(task2.Id);
                _taskIds.Add(task3.Id);
                _taskIds.Add(task4.Id);
            }
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            using (var db = new TrackerDataContext())
                db.Task.Delete(t => _taskIds.Contains(t.Id));
        }

        [Test]
        public void ByNullableStringTest()
        {
            try
            {
                using (var db = new TrackerDataContext())
                {
                    var a = db.Task.ByDetails(null).ToList();
                    var b = db.Task.ByDetails(_details1).ToList();
                    var c = db.Task.ByDetails(_details2).ToList();
                    var d = db.Task.ByDetails(_details3).ToList();

                    var e = db.Task.ByDetails(_details2, null).ToList();
                    Assert.AreEqual(a.Count + c.Count, e.Count);

                    var f = db.Task.ByDetails(_details1, _details3).ToList();
                    Assert.AreEqual(b.Count + d.Count, f.Count);

                    var g = db.Task.ByDetails(null, _details1, _details2, _details3).ToList();
                    Assert.AreEqual(a.Count + b.Count + c.Count + d.Count, g.Count);
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
