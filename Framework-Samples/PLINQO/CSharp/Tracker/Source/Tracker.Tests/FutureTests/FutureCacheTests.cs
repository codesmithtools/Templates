using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using CodeSmith.Data.Caching;
using NUnit.Framework;
using Tracker.Core.Data;
using CodeSmith.Data.Linq;

namespace Tracker.Tests.FutureTests
{
    [TestFixture]
    public class FutureCacheTests : TestBase
    {
        [Test]
        public void ExecuteBatch()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            // build up queries

            var q1 = db.Task.Take(10).ToList();


            var q2 = db.Task.Take(10);

            var dbCommand = db.GetCommand(q2, true);


            var result = db.ExecuteQuery<Task>(dbCommand.CommandText);
            var list = result.ToList();

        }

        [Test]
        public void FutureTest()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            CacheSettings cache = new CacheSettings(120);

            // build up queries
            var q1 = db.User
                .ByEmailAddress("one@test.com")
                .FutureCache(cache);

            var q2 = db.Task
                .Where(t => t.Summary == "Test")
                .FutureCache(cache);

            // this triggers the loading of all the future queries
            var users = q1.ToList();
            Assert.IsNotNull(users);

            // this should already be loaded
            Assert.IsTrue(((IFutureQuery)q2).IsLoaded);

            var tasks = q2.ToList();
            Assert.IsNotNull(tasks);

            // queries are loaded and cached, run same queries again...
            var c1 = db.User
                .ByEmailAddress("one@test.com")
                .FutureCache(cache);

            var c2 = db.Task
                .Where(t => t.Summary == "Test")
                .FutureCache(cache);

            // should be loaded because it came from cache
            Assert.IsTrue(((IFutureQuery)c1).IsLoaded);
            Assert.IsTrue(((IFutureQuery)c2).IsLoaded);

            users = c1.ToList();
            Assert.IsNotNull(users);

            tasks = c2.ToList();
            Assert.IsNotNull(tasks);
        }

        [Test]
        public void FutureCountTest()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            CacheSettings cache = new CacheSettings(120);

            // build up queries
            var q1 = db.User
                .ByEmailAddress("two@test.com")
                .FutureCache(cache);

            var q2 = db.Task
                .Where(t => t.Details == "Hello world!")
                .FutureCacheCount(cache);

            // this triggers the loading of all the future queries
            var users = q1.ToList();
            Assert.IsNotNull(users);

            // this should already be loaded
            Assert.IsTrue(((IFutureQuery)q2).IsLoaded);

            var count = q2.Value;
            Assert.Greater(count, 0);

            // queries are loaded and cached, run same queries again...
            var c1 = db.User
                .ByEmailAddress("two@test.com")
                .FutureCache(cache);

            var c2 = db.Task
                .Where(t => t.Details == "Hello world!")
                .FutureCacheCount(cache);

            // should be loaded because it came from cache
            Assert.IsTrue(((IFutureQuery)c1).IsLoaded);
            Assert.IsTrue(((IFutureQuery)c2).IsLoaded);

            users = c1.ToList();
            Assert.IsNotNull(users);

            count = c2.Value;
            Assert.Greater(count, 0);
        }

        [Test]
        public void FutureCountReverseTest()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            CacheSettings cache = new CacheSettings(120);

            // build up queries
            var q1 = db.User
                .ByEmailAddress("one@test.com")
                .FutureCache(cache);

            var q2 = db.Task
                .Where(t => t.Summary == "Test")
                .FutureCacheCount(cache);

            // access q2 first to trigger loading, testing loading from FutureCount
            // this triggers the loading of all the future queries
            var count = q2.Value;
            Assert.Greater(count, 0);

            // this should already be loaded
            Assert.IsTrue(((IFutureQuery)q1).IsLoaded);

            var users = q1.ToList();
            Assert.IsNotNull(users);

            // queries are loaded and cached, run same queries again...
            var c1 = db.User
                .ByEmailAddress("one@test.com")
                .FutureCache(cache);

            var c2 = db.Task
                .Where(t => t.Summary == "Test")
                .FutureCacheCount(cache);

            // should be loaded because it came from cache
            Assert.IsTrue(((IFutureQuery)c1).IsLoaded);
            Assert.IsTrue(((IFutureQuery)c2).IsLoaded);

            count = c2.Value;
            Assert.Greater(count, 0);

            users = c1.ToList();
            Assert.IsNotNull(users);
        }

        [Test]
        public void FutureValueTest()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            CacheSettings cache = new CacheSettings(120);

            // build up queries
            var q1 = db.User
                .ByEmailAddress("one@test.com")
                .FutureCacheFirstOrDefault(cache);

            var q2 = db.Task
                .Where(t => t.Summary == "Test")
                .FutureCacheCount(cache);

            // duplicate query except count
            var q3 = db.Task
                .Where(t => t.Summary == "Test")
                .FutureCache(cache);

            // this triggers the loading of all the future queries
            var user = q1.Value;
            Assert.IsNotNull(user);

            // this should already be loaded
            Assert.IsTrue(((IFutureQuery)q2).IsLoaded);

            var count = q2.Value;
            Assert.Greater(count, 0);

            var tasks = q3.ToList();
            Assert.IsNotNull(tasks);

            // queries are loaded and cached, run same queries again...
            var c1 = db.User
                .ByEmailAddress("one@test.com")
                .FutureCacheFirstOrDefault(cache);

            var c2 = db.Task
                .Where(t => t.Summary == "Test")
                .FutureCacheCount(cache);

            var c3 = db.Task
                .Where(t => t.Summary == "Test")
                .FutureCache(cache);

            // should be loaded because it came from cache
            Assert.IsTrue(((IFutureQuery)c1).IsLoaded);
            Assert.IsTrue(((IFutureQuery)c2).IsLoaded);
            Assert.IsTrue(((IFutureQuery)c3).IsLoaded);

            user = c1.Value;
            Assert.IsNotNull(user);

            count = c2.Value;
            Assert.Greater(count, 0);

            tasks = c3.ToList();
            Assert.IsNotNull(tasks);
        }

        [Test]
        public void FutureValueReverseTest()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            CacheSettings cache = new CacheSettings(120);

            // build up queries
            var q1 = db.User
                .Where(u => u.EmailAddress == "one@test.com")
                .FutureCacheFirstOrDefault(cache);

            var q2 = db.Task
                .Where(t => t.Summary == "Test")
                .FutureCacheCount(cache);

            // duplicate query except count
            var q3 = db.Task
                .Where(t => t.Summary == "Test")
                .FutureCache(cache);

            // access q2 first to trigger loading, testing loading from FutureCount
            // this triggers the loading of all the future queries
            var count = q2.Value;
            Assert.Greater(count, 0);

            // this should already be loaded
            Assert.IsTrue(((IFutureQuery)q1).IsLoaded);

            var user = q1.Value;
            Assert.IsNotNull(user);

            var tasks = q3.ToList();
            Assert.IsNotNull(tasks);

            // queries are loaded and cached, run same queries again...
            var c1 = db.User
                .ByEmailAddress("one@test.com")
                .FutureCacheFirstOrDefault(cache);

            var c2 = db.Task
                .Where(t => t.Summary == "Test")
                .FutureCacheCount(cache);

            var c3 = db.Task
                .Where(t => t.Summary == "Test")
                .FutureCache(cache);

            // should be loaded because it came from cache
            Assert.IsTrue(((IFutureQuery)c1).IsLoaded);
            Assert.IsTrue(((IFutureQuery)c2).IsLoaded);
            Assert.IsTrue(((IFutureQuery)c3).IsLoaded);

            user = c1.Value;
            Assert.IsNotNull(user);

            count = c2.Value;
            Assert.Greater(count, 0);

            tasks = c3.ToList();
            Assert.IsNotNull(tasks);
        }

        [Test]
        public void FromCacheFutre()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            CacheSettings cache = new CacheSettings(120);

            // build up queries
            var q1 = db.Task
                .Where(t => t.Summary == "Test")
                .FromCache(cache)
                .ToList();

            // duplicate query except count
            var q2 = db.Task
                .Where(t => t.Summary == "Test")
                .FutureCache(120);

            var q3 = db.Task
                .Where(t => t.Summary == "Test")
                .FutureCacheCount(cache);

            var count = q3.Value;
            Assert.Greater(count, 0);

            var tasks = q2.ToList();
            Assert.IsNotNull(tasks);
            Assert.AreEqual(q1.Count, tasks.Count);

        }
    }
}
