using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using NUnit.Framework;
using Tracker.Core.Data;

namespace Tracker.Tests.QueryTests
{
    [TestFixture]
    public class ByDateTimeTests : TaskTests
    {
        [Test]
        public void ByTest()
        {
            try
            {
                using (var db = new TrackerDataContext())
                {
                    var a = db.Task.ByCreatedDate(createDates[0]).ToList();
                    var b = db.Task.ByCreatedDate(createDates[1]).ToList();

                    var c = db.Task.ByCreatedDate(createDates[0], null).ToList();
                    Assert.AreEqual(a.Count, c.Count);

                    var d = db.Task.ByCreatedDate(createDates[0], createDates[1]).ToList();
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
                    var a = db.Task.ByDueDate((DateTime?)null).ToList();
                    var b = db.Task.ByDueDate(dueDates[0]).ToList();
                    var c = db.Task.ByDueDate(dueDates[1]).ToList();

                    var d = db.Task.ByDueDate(dueDates[0], null).ToList();
                    Assert.AreEqual(a.Count + b.Count, d.Count);

                    var e = db.Task.ByDueDate(dueDates[0], null, dueDates[1]).ToList();
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
