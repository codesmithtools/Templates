using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using CodeSmith.Data.Caching;
using CodeSmith.Data.Linq;
using NUnit.Framework;
using Tracker.Core.Data;

namespace Tracker.Tests.CacheTests
{
    [TestFixture]
    public class FromCacheGroupTests : RoleTests
    {
        [Test]
        public void TagTest()
        {
            var db = new TrackerDataContext { Log = Console.Out };

            var cache = CacheManager.GetProfile().WithGroup("Role");
            
            var roles = db.Role
                .Where(r => r.Name == "Test Role")
                .FromCache(cache);
            var role = db.Role
                .ByName("Duck Roll")
                .FromCacheFirstOrDefault(cache);

            Assert.IsNotNull(roles);
            Assert.IsNotNull(role);

            var roles2 = db.Role
                .Where(r => r.Name == "Test Role")
                .FromCache(cache);
            var role2 = db.Role
                .ByName("Duck Roll")
                .FromCacheFirstOrDefault(cache);

            Assert.IsNotNull(roles2);
            Assert.IsNotNull(role2);

            // some update expire tag
            CacheManager.GetProvider().InvalidateGroup("Role");

            var roles3 = db.Role
                .Where(r => r.Name == "Test Role")
                .FromCache(cache);
            var role3 = db.Role
                .ByName("Duck Roll")
                .FromCacheFirstOrDefault(cache);

            Assert.IsNotNull(roles3);
            Assert.IsNotNull(role3);
        }
    }
}
