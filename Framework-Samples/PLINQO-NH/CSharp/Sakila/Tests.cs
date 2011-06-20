using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Sakila.Data;
using CodeSmith.Data.Linq;

namespace Sakila
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Get()
        {
            using (var db = new SakilaDataContext())
            {
                var actors = db.Actor
                    .ByFirstName(ContainmentOperator.Contains, "A")
                    .Take(5)
                    .ToList();

                Assert.AreEqual(5, actors.Count);
            }
        }

        [Test]
        public void FromCache()
        {
            using (var db = new SakilaDataContext())
            {
                var addresses = db.Address
                    .ByDistrict("Alberta")
                    .FromCache();

                var cachedAddresses = db.Address
                    .ByDistrict("Alberta")
                    .FromCache();

                Assert.AreEqual(addresses.Count(), cachedAddresses.Count());
            }
        }

        [Test]
        public void Future()
        {
            using (var db = new SakilaDataContext())
            {
                var action = db.Category
                    .ByName("Action")
                    .FutureFirstOrDefault();

                var animation = db.Category
                    .ByName("Animation")
                    .FutureFirstOrDefault();

                Assert.IsFalse(animation.IsLoaded);
                Assert.IsNotNull(action.Value);
                Assert.IsTrue(animation.IsLoaded);
            }
        }
    }
}
