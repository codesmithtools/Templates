using System;
using System.Linq;
using CodeSmith.Data.Linq;
using NUnit.Framework;
using Tracker.Core.Data;

namespace Tracker.Tests
{
    [TestFixture]
    public class InheritanceTests
    {
        public class One
        {
            public string FirstName { get; set; }
        }

        public class Two : One
        {
            public string LastName { get; set; }
        }

        [Test]
        [Ignore]
        public void Inheritance()
        {
            try
            {
                using (var db = new TrackerDataContext())
                {
                    var x = db.User
                        .Select(u => new Two { FirstName = u.FirstName, LastName = u.LastName })
                        .FirstOrDefault();

                    var y = db.User
                        .Select(u => new Two { FirstName = u.FirstName, LastName = u.LastName })
                        .FutureFirstOrDefault();

                    var z = y.Value;

                    Assert.AreEqual(x.FirstName, z.FirstName);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
