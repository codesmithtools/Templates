using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CodeSmith.Data.Caching;
using CodeSmith.Data.Linq;
using CodeSmith.Data.Memcached;
using NUnit.Framework;
using Tracker.Core.Data;

namespace Tracker.Memcached.Tests.CacheTests
{
    [TestFixture]
    public class FromCacheTests : MemcachedTestBase
    {
        [Test]
        public void SimpleTest()
        {

            using (var db = new TrackerDataContext())
            {
                var query = db.Role.Where(r => r.Name == "Duck Roll");
                var key = query.GetHashKey();
                var roles = query.FromCache().ToList();

                Assert.IsInstanceOf(typeof(MemcachedProvider), CacheManager.Current.DefaultProvider);

                var cache = CacheManager.Current.DefaultProvider.Get<ICollection<Role>>(key);

                Assert.IsNotNull(cache);
                Assert.AreEqual(roles.Count, cache.Count);
            }

        }

        [Test]
        public void LongProfile()
        {
            using (var db = new TrackerDataContext())
            {
                var query = db.Role.Where(r => r.Name == "Duck Roll");
                var key = query.GetHashKey();
                var roles = query.FromCache("Long").ToList();

                Assert.IsInstanceOf(typeof(MemcachedProvider), CacheManager.Current.DefaultProvider);

                var cache = CacheManager.Current.DefaultProvider.Get<ICollection<Role>>(key);

                Assert.IsNotNull(cache);
                Assert.AreEqual(roles.Count, cache.Count);
            }
        }

        [Test]
        public void DurationTest()
        {

            using (var db = new TrackerDataContext())
            {
                var query = db.Role.Where(r => r.Name == "Test Role");
                var key = query.GetHashKey();
                var roles = query.FromCache(2).ToList();

                Assert.IsInstanceOf(typeof(MemcachedProvider), CacheManager.Current.DefaultProvider);

                var cache1 = CacheManager.Current.DefaultProvider.Get<ICollection<Role>>(key);
                Assert.IsNotNull(cache1);
                Assert.AreEqual(roles.Count, cache1.Count);

                System.Threading.Thread.Sleep(3000);

                var cache2 = CacheManager.Current.DefaultProvider.Get<ICollection<Role>>(key);
                Assert.IsNull(cache2);
            }

        }

        [Test]
        public void AbsoluteExpirationTest()
        {

            using (var db = new TrackerDataContext())
            {
                var query = db.Role.Where(r => r.Name == "Test Role");
                var key = query.GetHashKey();
                var roles = query.FromCache(2).ToList();

                Assert.IsInstanceOf(typeof(MemcachedProvider), CacheManager.Current.DefaultProvider);

                var cache1 = CacheManager.Current.DefaultProvider.Get<ICollection<Role>>(key);
                Assert.IsNotNull(cache1);
                Assert.AreEqual(roles.Count, cache1.Count);

                System.Threading.Thread.Sleep(3000);

                var cache2 = CacheManager.Current.DefaultProvider.Get<ICollection<Role>>(key);
                Assert.IsNull(cache2);
            }

        }

        [Test]
        public void SlidingExpirationTest()
        {

            using (var db = new TrackerDataContext())
            {
                var query = db.Role.Where(r => r.Name == "Test Role");
                var key = query.GetHashKey();
                var roles = query.FromCache(new CacheSettings(TimeSpan.FromSeconds(2))).ToList();

                Assert.IsInstanceOf(typeof(MemcachedProvider), CacheManager.Current.DefaultProvider);

                var cache1 = CacheManager.Current.DefaultProvider.Get<ICollection<Role>>(key);
                Assert.IsNotNull(cache1);
                Assert.AreEqual(roles.Count, cache1.Count);

                System.Threading.Thread.Sleep(3000);

                var cache4 = CacheManager.Current.DefaultProvider.Get<ICollection<Role>>(key);
                Assert.IsNull(cache4);
            }

        }

        [Test]
        public void NoCacheEmptyResultTest()
        {

            using (var db = new TrackerDataContext())
            {
                var guid = System.Guid.NewGuid().ToString();
                var query = db.Role.Where(r => r.Name == guid);
                var key = query.GetHashKey();
                var roles = query.FromCache(new CacheSettings(2) { CacheEmptyResult = false });

                Assert.IsNotNull(roles);
                Assert.AreEqual(0, roles.Count());

                Assert.IsInstanceOf(typeof(MemcachedProvider), CacheManager.Current.DefaultProvider);


                var cache = CacheManager.Current.DefaultProvider.Get<ICollection<Role>>(key);
                Assert.IsNull(cache);
            }

        }
    }
}