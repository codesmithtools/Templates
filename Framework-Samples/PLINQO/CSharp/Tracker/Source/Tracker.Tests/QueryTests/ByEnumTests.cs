using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeSmith.Data.Linq;
using NUnit.Framework;
using Tracker.Core.Data;

namespace Tracker.Tests.QueryTests
{
    [TestFixture]
    public class ByEnumTests : TestBase
    {
        [Test]
        public void ByTest()
        {
                using (var db = new TrackerDataContext {Log = Console.Out})
                {
                    var a = db.Task.ByStatus(Status.InProgress).ToList();
                    var b = db.Task.ByStatus(Status.NotStarted).ToList();

                    var c = db.Task.ByStatus(Status.InProgress, null).ToList();
                    Assert.AreEqual(a.Count, c.Count);

                    var d = db.Task.ByStatus(Status.InProgress, Status.NotStarted).ToList();
                    Assert.AreEqual(a.Count + b.Count, d.Count);
                }

        }

        [Test]
        public void ByNullableTest()
        {
                using (var db = new TrackerDataContext {Log = Console.Out})
                {
                    var a = db.Task.ByPriority((Priority?)null).ToList();
                    var b = db.Task.ByPriority(Priority.Normal).ToList();
                    var c = db.Task.ByPriority(Priority.High).ToList();

                    var d = db.Task.ByPriority(Priority.Normal, null).ToList();
                    Assert.AreEqual(a.Count + b.Count, d.Count);

                    var e = db.Task.ByPriority(Priority.Normal, null, Priority.High).ToList();
                    Assert.AreEqual(a.Count + b.Count + c.Count, e.Count);

                    var f = db.Task.ByPriority(ComparisonOperator.NotEquals, null).ToList();
                    var g = db.Task.ByPriority(ComparisonOperator.Equals, null).ToList();

                }

        }
    }
}
