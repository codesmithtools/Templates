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
    }
}
