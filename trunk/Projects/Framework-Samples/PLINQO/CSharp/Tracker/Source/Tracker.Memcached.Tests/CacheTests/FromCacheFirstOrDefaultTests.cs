using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CodeSmith.Data.Caching;
using CodeSmith.Data.Linq;
using NUnit.Framework;
using Tracker.Core.Data;

namespace Tracker.Memcached.Tests.CacheTests
{
    [TestFixture]
    public class FromCacheFirstOrDefaultTests : MemcachedTestBase
    {
        [Test]
        public void SimpleTest()
        {
            using (var db = new TrackerDataContext())
            {
                var query = db.Role.Where(r => r.Name == "Test Role");
                var key = query.Take(1).GetHashKey();
                var role = query.FromCacheFirstOrDefault();

                var cache = CacheManager.Get<ICollection<Role>>(key);
                Assert.IsNotNull(cache);
                Assert.AreEqual(role.Id, cache.FirstOrDefault().Id);
            }
        }

        [Test]
        public void DurationTest()
        {
            using (var db = new TrackerDataContext())
            {
                var query = db.Role.Where(r => r.Name == "Test Role");
                var key = query.Take(1).GetHashKey();
                var role = query.FromCacheFirstOrDefault(CacheSettings.FromDuration(2));

                var cache1 = CacheManager.Get<ICollection<Role>>(key);
                Assert.IsNotNull(cache1);
                Assert.AreEqual(role.Id, cache1.FirstOrDefault().Id);

                System.Threading.Thread.Sleep(3000);

                var cache2 = CacheManager.Get<ICollection<Role>>(key);
                Assert.IsNull(cache2);
            }
        }

        [Test]
        public void AbsoluteExpirationTest()
        {
            using (var db = new TrackerDataContext())
            {
                var query = db.Role.Where(r => r.Name == "Test Role");
                var key = query.Take(1).GetHashKey();
                var role = query.FromCacheFirstOrDefault(CacheSettings.FromAbsolute(DateTime.Now.AddSeconds(2)));

                var cache1 = CacheManager.Get<ICollection<Role>>(key);
                Assert.IsNotNull(cache1);
                Assert.AreEqual(role.Id, cache1.FirstOrDefault().Id);

                System.Threading.Thread.Sleep(2500);

                var cache2 = CacheManager.Get<ICollection<Role>>(key);
                Assert.IsNull(cache2);
            }
        }

        [Test]
        public void NoCacheEmptyResultTest()
        {
            using (var db = new TrackerDataContext())
            {
                var guid = System.Guid.NewGuid().ToString();
                var query = db.Role.Where(r => r.Name == guid);
                var key = query.Take(1).GetHashKey();
                var role = query.FromCacheFirstOrDefault(new CacheSettings(2) { CacheEmptyResult = false });

                Assert.IsNull(role);

                var cache = CacheManager.Get<ICollection<Role>>(key);
                Assert.IsNull(cache);
            }
        }

        [Test]
        public void CacheEmptyResultTest()
        {
            using (var db = new TrackerDataContext())
            {
                var guid = System.Guid.NewGuid().ToString();
                var query = db.Role.Where(r => r.Name == guid);
                var key = query.Take(1).GetHashKey();
                var role = query.FromCacheFirstOrDefault(new CacheSettings(2));

                Assert.IsNull(role);

                var cache = CacheManager.Get<ICollection<Role>>(key);
                Assert.IsNotNull(cache);
                Assert.AreEqual(0, cache.Count());
            }
        }
    }
}