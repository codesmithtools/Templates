using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using CodeSmith.Data.Linq;
using NUnit.Framework;
using Tracker.Core.Data;

namespace Tracker.Tests.QueryTests
{
    [TestFixture]
    public class ByIntTests : TestBase
    {
        [Test]
        public void ByTest()
        {

            using (var db = new TrackerDataContext { Log = Console.Out })
            {
                var a = db.Task.ByCreatedId(UserIds[1]).ToList();
                var b = db.Task.ByCreatedId(UserIds[2]).ToList();

                var c = db.Task.ByCreatedId(UserIds[1], null).ToList();
                Assert.AreEqual(a.Count, c.Count);

                var d = db.Task.ByCreatedId(UserIds[1], UserIds[2]).ToList();
                Assert.AreEqual(a.Count + b.Count, d.Count);
            }

        }

        [Test]
        public void ByNullableTest()
        {

            using (var db = new TrackerDataContext { Log = Console.Out })
            {
                var a = db.Task.ByAssignedId((int?)null).ToList();
                var b = db.Task.ByAssignedId(UserIds[1]).ToList();
                var c = db.Task.ByAssignedId(UserIds[2]).ToList();

                var d = db.Task.ByAssignedId(UserIds[1], null).ToList();
                Assert.AreEqual(a.Count + b.Count, d.Count);

                var e = db.Task.ByAssignedId(UserIds[1], null, UserIds[2]).ToList();
                Assert.AreEqual(a.Count + b.Count + c.Count, e.Count);

                var f = db.Task.ByAssignedId(ComparisonOperator.NotEquals, null).ToList();
                var g = db.Task.ByAssignedId(ComparisonOperator.Equals, null).ToList();
                
            }

        }
    }
}
