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
    public class ClearCacheTests : MemcachedTestBase
    {
        [Test]
        public void SimpleTest()
        {
            using (var db = new TrackerDataContext())
            {
                var query = db.Role.Where(r => r.Name == "Test Role");
                var key = query.GetHashKey();
                var roles = query.FromCache().ToList();
                
                var cache1 = CacheManager.Get<ICollection<Role>>(key);
                Assert.IsNotNull(cache1);
                Assert.AreEqual(roles.Count, cache1.Count);

                bool success = query.ClearCache();
                Assert.IsTrue(success);
                success = query.ClearCache();
                Assert.IsFalse(success);

                var cache2 = CacheManager.Get<ICollection<Role>>(key);
                Assert.IsNull(cache2);
            }
        }

        [Test]
        public void SimpleTestWithGroup()
        {
            using (var db = new TrackerDataContext())
            {
                CacheManager.InvalidateGroup("group1");

                var query = db.Role.Where(r => r.Name == "Test Role");
                var key = query.GetHashKey();
                var roles = query.FromCache(CacheManager.GetProfile().WithGroup("group1")).ToList();

                var cache1 = CacheManager.Get<ICollection<Role>>(key, "group1");
                Assert.IsNotNull(cache1);
                Assert.AreEqual(roles.Count, cache1.Count);

                query.ClearCache("group1");

                var cache2 = CacheManager.Get<ICollection<Role>>(key, "group1");
                Assert.IsNull(cache2);
            }
        }
    }
}