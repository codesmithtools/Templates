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
    public class ByDateTimeTests : TestBase
    {
        [Test]
        public void ByTest()
        {
            using (var db = new TrackerDataContext { Log = Console.Out })
            {
                var a = db.Task.ByCreatedDate(CreateDates[0]).ToList();
                var b = db.Task.ByCreatedDate(CreateDates[1]).ToList();

                var c = db.Task.ByCreatedDate(CreateDates[0], null).ToList();
                Assert.AreEqual(a.Count, c.Count);
            }
        }

        [Test]
        public void ByNullableTest()
        {

            using (var db = new TrackerDataContext { Log = Console.Out })
            {
                var a = db.Task.ByDueDate((DateTime?)null).ToList();
                var b = db.Task.ByDueDate(DueDates[0]).ToList();
                var c = db.Task.ByDueDate(DueDates[1]).ToList();

                var d = db.Task.ByDueDate(DueDates[0], null).ToList();
                Assert.AreEqual(a.Count + b.Count, d.Count);

                var e = db.Task.ByDueDate(DueDates[0], null, DueDates[1]).ToList();
                Assert.AreEqual(a.Count + b.Count + c.Count, e.Count);

                var f = db.Task.ByDueDate(null, ComparisonOperator.NotEquals).ToList();
                var g = db.Task.ByDueDate(null, ComparisonOperator.Equals).ToList();

            }

        }
    }
}
