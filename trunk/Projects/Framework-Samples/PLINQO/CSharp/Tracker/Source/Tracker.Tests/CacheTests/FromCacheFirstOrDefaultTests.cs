using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CodeSmith.Data.Caching;
using CodeSmith.Data.Linq;
using NUnit.Framework;
using Tracker.Core.Data;

namespace Tracker.Tests.CacheTests
{
    [TestFixture]
    public class FromCacheFirstOrDefaultTests : RoleTests
    {
        [Test]
        public void SimpleTest()
        {
            using (var db = new TrackerDataContext {Log = Console.Out})
            {
                var query = db.Role.Where(r => r.Name == "Test Role");
                var key = query.Take(1).GetHashKey();
                var role = query.FromCacheFirstOrDefault();

                var cache = HttpRuntime.Cache.Get(key) as byte[];
                Assert.IsNotNull(cache);

                var list = cache.ToCollection<Role>();
                Assert.IsNotNull(list);
                Assert.AreEqual(role.Id, list.FirstOrDefault().Id);
            }

        }

        [Test]
        public void DurationTest()
        {
            using (var db = new TrackerDataContext {Log = Console.Out})
            {
                var query = db.Role.Where(r => r.Name == "Test Role");
                var key = query.Take(1).GetHashKey();
                var role = query.FromCacheFirstOrDefault(2);

                var cache = HttpRuntime.Cache.Get(key) as byte[];
                Assert.IsNotNull(cache);

                var list = cache.ToCollection<Role>();
                Assert.IsNotNull(list);
                Assert.AreEqual(role.Id, list.FirstOrDefault().Id);

                System.Threading.Thread.Sleep(2500);

                cache = HttpRuntime.Cache.Get(key) as byte[]; 
                Assert.IsNull(cache);
            }

        }

        [Test]
        public void AbsoluteExpirationTest()
        {
            using (var db = new TrackerDataContext {Log = Console.Out})
            {
                var query = db.Role.Where(r => r.Name == "Test Role");
                var key = query.Take(1).GetHashKey();
                var role = query.FromCacheFirstOrDefault(2);

                var cache = HttpRuntime.Cache.Get(key) as byte[];
                Assert.IsNotNull(cache);

                var list = cache.ToCollection<Role>();
                Assert.IsNotNull(list);
                Assert.AreEqual(role.Id, list.FirstOrDefault().Id);

                System.Threading.Thread.Sleep(2500);

                cache = HttpRuntime.Cache.Get(key) as byte[];
                Assert.IsNull(cache);
            }

        }

        [Test]
        public void SlidingExpirationTest()
        {
            using (var db = new TrackerDataContext {Log = Console.Out})
            {
                var query = db.Role.Where(r => r.Name == "Test Role");
                var key = query.Take(1).GetHashKey();
                var role = query.FromCacheFirstOrDefault(new CacheSettings(TimeSpan.FromSeconds(2)));

                var cache = HttpRuntime.Cache.Get(key) as byte[];
                Assert.IsNotNull(cache);

                var list = cache.ToCollection<Role>();
                Assert.IsNotNull(list);
                Assert.AreEqual(role.Id, list.FirstOrDefault().Id);

                System.Threading.Thread.Sleep(1500);

                cache = HttpRuntime.Cache.Get(key) as byte[];
                Assert.IsNotNull(cache);

                list = cache.ToCollection<Role>();
                Assert.IsNotNull(list);
                Assert.AreEqual(role.Id, list.FirstOrDefault().Id);

                System.Threading.Thread.Sleep(1500);

                cache = HttpRuntime.Cache.Get(key) as byte[];
                Assert.IsNotNull(cache);

                list = cache.ToCollection<Role>();
                Assert.IsNotNull(list);
                Assert.AreEqual(role.Id, list.FirstOrDefault().Id);

                System.Threading.Thread.Sleep(2500);

                cache = HttpRuntime.Cache.Get(key) as byte[];
                Assert.IsNull(cache);
            }

        }

        [Test]
        public void NoCacheEmptyResultTest()
        {
            using (var db = new TrackerDataContext {Log = Console.Out})
            {
                var guid = System.Guid.NewGuid().ToString();
                var query = db.Role.Where(r => r.Name == guid);
                var key = query.Take(1).GetHashKey();
                var role = query.FromCacheFirstOrDefault(new CacheSettings(2) { CacheEmptyResult = false });

                Assert.IsNull(role);

                var cache = HttpRuntime.Cache.Get(key);
                Assert.IsNull(cache);
            }

        }

        [Test]
        public void CacheEmptyResultTest()
        {
            using (var db = new TrackerDataContext {Log = Console.Out})
            {
                var guid = System.Guid.NewGuid().ToString();
                var query = db.Role.Where(r => r.Name == guid);
                var key = query.Take(1).GetHashKey();
                var role = query.FromCacheFirstOrDefault(new CacheSettings(2) { CacheEmptyResult = true });

                Assert.IsNull(role);

                var cache = HttpRuntime.Cache.Get(key) as byte[];
                Assert.IsNotNull(cache);

                var list = cache.ToCollection<Role>();
                Assert.IsNotNull(list);
                Assert.AreEqual(0, list.Count);
            }

        }
    }
}
