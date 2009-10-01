using System.Collections.Generic;
using System.Data.Linq;
using NUnit.Framework;
using Tracker.Data;

namespace Tracker.Tests.CacheTests
{
    public abstract class RoleTests
    {
        private List<int> _roleIds = new List<int>();

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            using (var db = new TrackerDataContext())
            {
                var role1 = new Role
                {
                    Name = "Test Role"
                };
                var role2 = new Role
                {
                    Name = "Duck Roll"
                };

                db.Role.InsertOnSubmit(role1);
                db.Role.InsertOnSubmit(role2);
                db.SubmitChanges();

                _roleIds.Add(role1.Id);
                _roleIds.Add(role2.Id);
            }
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            using (var db = new TrackerDataContext())
                db.Role.Delete(r => _roleIds.Contains(r.Id));
        }
    }
}
