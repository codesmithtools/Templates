using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using NUnit.Framework;
using Tracker.Core.Data;

namespace Tracker.Tests.QueryTests
{
    [TestFixture]
    public class ByStringTests
    {
        [Test]
        public void ByNullableTest()
        {

                using (var db = new TrackerDataContext {Log = Console.Out})
                {
                    var a = db.Task.ByDetails((string)null).ToList();
                    var b = db.Task.ByDetails(String.Empty).ToList();
                    var c = db.Task.ByDetails("Hello world!").ToList();
                    var d = db.Task.ByDetails("Goodnight moon!").ToList();

                    var e = db.Task.ByDetails("Hello world!", null).ToList();
                    Assert.AreEqual(a.Count + c.Count, e.Count);

                    var f = db.Task.ByDetails(String.Empty, "Goodnight moon!").ToList();
                    Assert.AreEqual(b.Count + d.Count, f.Count);

                    var g = db.Task.ByDetails(null, String.Empty, "Hello world!", "Goodnight moon!").ToList();
                    Assert.AreEqual(a.Count + b.Count + c.Count + d.Count, g.Count);
                }

        }

    }
}
