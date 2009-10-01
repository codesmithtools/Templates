using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            try
            {
                using (var db = new TrackerDataContext())
                {
                    var query = db.Role.Where(r => r.Name == "Test Role");
                    var key = query.Take(1).GetKey();
                    var role = query.FromCacheFirstOrDefault();

                    var cache = HttpRuntime.Cache.Get(key) as List<Role>;
                    Assert.IsNotNull(cache);
                    Assert.AreSame(role, cache.FirstOrDefault());
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
        public void DurationTest()
        {
            try
            {
                using (var db = new TrackerDataContext())
                {
                    var query = db.Role.Where(r => r.Name == "Test Role");
                    var key = query.Take(1).GetKey();
                    var role = query.FromCacheFirstOrDefault(2);

                    var cache1 = HttpRuntime.Cache.Get(key) as List<Role>;
                    Assert.IsNotNull(cache1);
                    Assert.AreSame(role, cache1.FirstOrDefault());

                    System.Threading.Thread.Sleep(2500);

                    var cache2 = HttpRuntime.Cache.Get(key);
                    Assert.IsNull(cache2);
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
        public void AbsoluteExpirationTest()
        {
            try
            {
                using (var db = new TrackerDataContext())
                {
                    var query = db.Role.Where(r => r.Name == "Test Role");
                    var key = query.Take(1).GetKey();
                    var role = query.FromCacheFirstOrDefault(DateTime.UtcNow.AddSeconds(2));

                    var cache1 = HttpRuntime.Cache.Get(key) as List<Role>;
                    Assert.IsNotNull(cache1);
                    Assert.AreSame(role, cache1.FirstOrDefault());

                    System.Threading.Thread.Sleep(2500);

                    var cache2 = HttpRuntime.Cache.Get(key);
                    Assert.IsNull(cache2);
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
        public void SlidingExpirationTest()
        {
            try
            {
                using (var db = new TrackerDataContext())
                {
                    var query = db.Role.Where(r => r.Name == "Test Role");
                    var key = query.Take(1).GetKey();
                    var role = query.FromCacheFirstOrDefault(new TimeSpan(0, 0, 2));

                    var cache1 = HttpRuntime.Cache.Get(key) as List<Role>;
                    Assert.IsNotNull(cache1);
                    Assert.AreSame(role, cache1.FirstOrDefault());

                    System.Threading.Thread.Sleep(1500);

                    var cache2 = HttpRuntime.Cache.Get(key) as List<Role>;
                    Assert.IsNotNull(cache2);
                    Assert.AreSame(role, cache2.FirstOrDefault());

                    System.Threading.Thread.Sleep(1500);

                    var cache3 = HttpRuntime.Cache.Get(key) as List<Role>;
                    Assert.IsNotNull(cache3);
                    Assert.AreSame(role, cache3.FirstOrDefault());

                    System.Threading.Thread.Sleep(2500);

                    var cache4 = HttpRuntime.Cache.Get(key);
                    Assert.IsNull(cache4);
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
        public void CacheEmptyResultTest()
        {
            try
            {
                using (var db = new TrackerDataContext())
                {
                    var query = db.Role.Where(r => r.Name == System.Guid.NewGuid().ToString());
                    var key = query.Take(1).GetKey();
                    var role = query.FromCacheFirstOrDefault(new CacheSettings { Duration = 2, CacheEmptyResult = false });

                    Assert.IsNull(role);

                    var cache = HttpRuntime.Cache.Get(key);
                    Assert.IsNull(cache);
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
