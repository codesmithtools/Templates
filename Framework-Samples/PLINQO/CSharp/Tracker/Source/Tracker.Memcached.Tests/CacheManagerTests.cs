using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeSmith.Data.Caching;
using NUnit.Framework;
using Tracker.Core.Data;

namespace Tracker.Memcached.Tests
{
    [TestFixture]
    public class CacheManagerTests
    {
        [SetUp]
        public void Setup()
        {
            //TODO: NUnit setup
        }

        [TearDown]
        public void TearDown()
        {
            //TODO: NUnit TearDown
        }

        [Test]
        public void Example()
        {
            var cacheManager = new CacheManager();

            Assert.IsNotNull(cacheManager);

        }
    }
}
