using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using CodeSmith.Data.Linq;
using NUnit.Framework;
using Tracker.Data;
using System.Data.Linq;

namespace Tracker.Tests.CacheTests
{
    [TestFixture]
    public class ClearCacheTests : RoleTests
    {
        [Test]
        public void SimpleTest()
        {
            try
            {
                using (var db = new TrackerDataContext())
                {
                    var query = db.Role.Where(r => r.Name == "Test Role");
                    var key = query.GetKey();
                    var roles = query.FromCache();

                    var cache1 = HttpRuntime.Cache.Get(key);
                    Assert.IsNotNull(cache1);
                    Assert.AreSame(roles, cache1);

                    query.ClearCache();

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
    }
}
