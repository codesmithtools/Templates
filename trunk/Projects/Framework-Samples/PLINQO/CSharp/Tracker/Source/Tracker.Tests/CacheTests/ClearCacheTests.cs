using System;
using System.Linq;
using System.Web;
using CodeSmith.Data.Linq;
using NUnit.Framework;
using Tracker.Core.Data;

namespace Tracker.Tests.CacheTests
{
    [TestFixture]
    public class ClearCacheTests : RoleTests
    {
        [Test]
        public void SimpleTest()
        {

            using (var db = new TrackerDataContext { Log = Console.Out })
            {
                var query = db.Role.Where(r => r.Name == "Test Role");
                var key = query.GetHashKey();
                var roles = query.FromCache();

                var cache1 = HttpRuntime.Cache.Get(key);
                Assert.IsNotNull(cache1);

                query.ClearCache();

                var cache2 = HttpRuntime.Cache.Get(key);
                Assert.IsNull(cache2);
            }

        }
    }
}
