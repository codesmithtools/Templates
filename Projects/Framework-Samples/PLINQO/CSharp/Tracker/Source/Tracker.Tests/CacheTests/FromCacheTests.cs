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
    public class FromCacheTests : RoleTests
    {
        [Test]
        public void SimpleTest()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            var query = db.Role.Where(r => r.Name == "Test Role");
            var roles = query.FromCache().ToList();

            Assert.IsInstanceOf(typeof(HttpCacheProvider), CacheManager.GetProvider());

            var key = CacheManager.GetProvider().GetGroupKey(query.GetHashKey(), null);

            var cache = CacheManager.GetProvider().Get<byte[]>(key);
            Assert.IsNotNull(cache);

            var list = cache.ToCollection<Role>();
            Assert.IsNotNull(list);
            Assert.AreEqual(roles.Count, list.Count);
        }

        [Test]
        public void DurationTest()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            var query = db.Role.Where(r => r.Name == "Test Role");
            var key = query.GetHashKey();
            var roles = query.FromCache(2).ToList();

            var cache = CacheManager.GetProvider().Get<byte[]>(key);
            Assert.IsNotNull(cache);

            var list = cache.ToCollection<Role>();
            Assert.IsNotNull(list);
            Assert.AreEqual(roles.Count, list.Count);

            System.Threading.Thread.Sleep(3000);

            cache = CacheManager.GetProvider().Get<byte[]>(key);
            Assert.IsNull(cache);
        }

        [Test]
        public void AbsoluteExpirationTest()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            var query = db.Role.Where(r => r.Name == "Test Role");
            var key = query.GetHashKey();
            var roles = query.FromCache(2).ToList();

            var cache = CacheManager.GetProvider().Get<byte[]>(key);
            Assert.IsNotNull(cache);

            var list = cache.ToCollection<Role>();
            Assert.IsNotNull(list);
            Assert.AreEqual(roles.Count, list.Count);

            System.Threading.Thread.Sleep(3000);

            cache = CacheManager.GetProvider().Get<byte[]>(key);
            Assert.IsNull(cache);
        }

        [Test]
        public void SlidingExpirationTest()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            var query = db.Role.Where(r => r.Name == "Test Role");
            var key = query.GetHashKey();
            var roles = query
                .FromCache(new CacheSettings(TimeSpan.FromSeconds(2)))
                .ToList();

            var cache = HttpRuntime.Cache.Get(key) as byte[];
            Assert.IsNotNull(cache);

            var list = cache.ToCollection<Role>();
            Assert.IsNotNull(list);
            Assert.AreEqual(roles.Count, list.Count);

            System.Threading.Thread.Sleep(1500);

            cache = HttpRuntime.Cache.Get(key) as byte[];
            Assert.IsNotNull(cache);

            list = cache.ToCollection<Role>();
            Assert.IsNotNull(list);
            Assert.AreEqual(roles.Count, list.Count);

            System.Threading.Thread.Sleep(1500);

            cache = HttpRuntime.Cache.Get(key) as byte[];
            Assert.IsNotNull(cache);

            list = cache.ToCollection<Role>();
            Assert.IsNotNull(list);
            Assert.AreEqual(roles.Count, list.Count);

            System.Threading.Thread.Sleep(2500);

            cache = HttpRuntime.Cache.Get(key) as byte[];
            Assert.IsNull(cache);

        }

        [Test]
        public void NoCacheEmptyResultTest()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            var guid = System.Guid.NewGuid().ToString();
            var query = db.Role.Where(r => r.Name == guid);
            var key = query.GetHashKey();
            var roles = query
                .FromCache(new CacheSettings(2) { CacheEmptyResult = false })
                .ToList(); ;

            Assert.IsNotNull(roles);
            Assert.AreEqual(0, roles.Count());

            var cache = HttpRuntime.Cache.Get(key);
            Assert.IsNull(cache);
        }

        [Test]
        public void CacheEmptyResultTest()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            var guid = System.Guid.NewGuid().ToString();
            var query = db.Role.Where(r => r.Name == guid);
            var key = query.GetHashKey();
            var roles = query
                .FromCache(new CacheSettings(2) { CacheEmptyResult = true })
                .ToList(); ;

            Assert.IsNotNull(roles);

            var cache = HttpRuntime.Cache.Get(key) as byte[];
            Assert.IsNotNull(cache);

            var list = cache.ToCollection<Role>();
            Assert.IsNotNull(list);
            Assert.AreEqual(0, list.Count);

        }
    }
}
