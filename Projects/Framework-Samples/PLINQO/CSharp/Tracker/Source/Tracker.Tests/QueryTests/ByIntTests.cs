using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using NUnit.Framework;
using Tracker.Core.Data;

namespace Tracker.Tests.QueryTests
{
    [TestFixture]
    public class ByIntTests : TaskTests
    {
        [Test]
        public void ByTest()
        {

                using (var db = new TrackerDataContext())
                {
                    var a = db.Task.ByCreatedId(userIds[1]).ToList();
                    var b = db.Task.ByCreatedId(userIds[2]).ToList();

                    var c = db.Task.ByCreatedId(userIds[1], null).ToList();
                    Assert.AreEqual(a.Count, c.Count);

                    var d = db.Task.ByCreatedId(userIds[1], userIds[2]).ToList();
                    Assert.AreEqual(a.Count + b.Count, d.Count);
                }

        }

        [Test]
        public void ByNullableTest()
        {

                using (var db = new TrackerDataContext())
                {
                    var a = db.Task.ByAssignedId((int?)null).ToList();
                    var b = db.Task.ByAssignedId(userIds[1]).ToList();
                    var c = db.Task.ByAssignedId(userIds[2]).ToList();

                    var d = db.Task.ByAssignedId(userIds[1], null).ToList();
                    Assert.AreEqual(a.Count + b.Count, d.Count);

                    var e = db.Task.ByAssignedId(userIds[1], null, userIds[2]).ToList();
                    Assert.AreEqual(a.Count + b.Count + c.Count, e.Count);
                }

        }
    }
}
