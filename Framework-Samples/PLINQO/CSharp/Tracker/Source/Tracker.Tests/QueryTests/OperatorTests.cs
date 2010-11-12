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
    public class OperatorTests : TestBase
    {       
        [Test]
        public void Contains()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            var assignedId = new List<int?> { null, 1 };
            // null and int
            var q1 = db.Task.ByAssignedId(assignedId);

            var assignedList = q1.ToList();
            Assert.IsNotNull(assignedList);


            var details = new List<string> { null, "test" };
            // null and string
            var q2 = db.Task.ByDetails(details);
            var detailList = q2.ToList();
            Assert.IsNotNull(detailList);

            // null and int direct
            var q3 = db.Task.ByAssignedId(null, 1);

            var assignedList2 = q3.ToList();
            Assert.IsNotNull(assignedList2);

            // string, null direct
            var q4 = db.Task.ByDetails("test", null, "other");
            var detailList2 = q4.ToList();
            Assert.IsNotNull(detailList2);

            // should use IN operator
            var q5 = db.Task.ByCreatedId(1, 2);
            var createdList = q5.ToList();
            Assert.IsNotNull(createdList);
        }

        [Test]
        public void ContainmentTest()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            var q1 = db.Task.BySummary(ContainmentOperator.Contains, "Earth");
            var list = q1.ToList();
            Assert.IsNotNull(list);

            var q2 = db.Task.BySummary(ContainmentOperator.StartsWith, "Make");
            list = q2.ToList();
            Assert.IsNotNull(list);

            var q3 = db.Task.BySummary(ContainmentOperator.EndsWith, "Earth");
            list = q3.ToList();
            Assert.IsNotNull(list);

            var q4 = db.Task.BySummary(ContainmentOperator.NotContains, "test");
            list = q4.ToList();
            Assert.IsNotNull(list);

            var q5 = db.Task.ByDetails(ContainmentOperator.Equals, null);
            list = q5.ToList();
            Assert.IsNotNull(list);

            var q6 = db.Task.ByDetails(ContainmentOperator.NotEquals, null);
            list = q6.ToList();
            Assert.IsNotNull(list);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContainmentStartsWith()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            var q5 = db.Task.ByDetails(ContainmentOperator.StartsWith, null);
            var list = q5.ToList();
            Assert.IsNotNull(list);          
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContainmentEndsWith()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            var q6 = db.Task.ByDetails(ContainmentOperator.EndsWith, null);
            var list = q6.ToList();
            Assert.IsNotNull(list);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContainmentContains()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            var q8 = db.Task.ByDetails(ContainmentOperator.Contains, null);
            var list = q8.ToList();
            Assert.IsNotNull(list);
        }

        [Test]
        public void ComparisonTest()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            var q1 = db.Task.ByStartDate(ComparisonOperator.GreaterThan, DateTime.Now);
            var list = q1.ToList();
            Assert.IsNotNull(list);

            var q2 = db.Task.ByStartDate(ComparisonOperator.GreaterThanOrEquals, DateTime.Now);
            list = q2.ToList();
            Assert.IsNotNull(list);

            var q3 = db.Task.ByStartDate(ComparisonOperator.LessThan, DateTime.Now);
            list = q3.ToList();
            Assert.IsNotNull(list);

            var q4 = db.Task.ByStartDate(ComparisonOperator.LessThanOrEquals, DateTime.Now);
            list = q4.ToList();
            Assert.IsNotNull(list);

            var q5 = db.Task.ByStartDate(ComparisonOperator.NotEquals, DateTime.Now);
            list = q5.ToList();
            Assert.IsNotNull(list);

            var q6 = db.Task.ByStartDate(ComparisonOperator.Equals, null);
            list = q6.ToList();
            Assert.IsNotNull(list);

            var q7 = db.Task.ByStartDate(ComparisonOperator.NotEquals, null);
            list = q7.ToList();
            Assert.IsNotNull(list);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ComparisonGreaterThan()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            var q6 = db.Task.ByStartDate(ComparisonOperator.GreaterThan, null);
            var list = q6.ToList();
            Assert.IsNotNull(list);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ComparisonGreaterThanOrEquals()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            var q6 = db.Task.ByStartDate(ComparisonOperator.GreaterThanOrEquals, null);
            var list = q6.ToList();
            Assert.IsNotNull(list);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ComparisonLessThan()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            var q6 = db.Task.ByStartDate(ComparisonOperator.LessThan, null);
            var list = q6.ToList();
            Assert.IsNotNull(list);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ComparisonLessThanOrEquals()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            var q6 = db.Task.ByStartDate(ComparisonOperator.LessThanOrEquals, null);
            var list = q6.ToList();
            Assert.IsNotNull(list);
        }
    }
}
