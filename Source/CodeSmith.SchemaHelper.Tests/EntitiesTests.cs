using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using SchemaExplorer;

namespace CodeSmith.SchemaHelper.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestFixture()]
    public class EntitiesTests
    {
        [SetUp()]
        public void Setup()
        {
            //TODO: NUnit setup
        }

        [TearDown()]
        public void TearDown()
        {
            //TODO: NUnit TearDown
        }

        //private const string DatabaseName = "Feedback";
        private const string DatabaseName = "Tester";
        //private const string DatabaseName = "NorthwindSQL2000";

        [Test()]
        public void TableDeepLoadTest()
        {
            Stopwatch watch = Stopwatch.StartNew();

            var DatabaseSource = new DatabaseSchema(new SqlSchemaProvider(), "server=.;database=test;integrated security=true;Connect Timeout=300");
            var Entities = new EntityManager(DatabaseSource).Entities;
            Assert.IsTrue(DatabaseSource.ConnectionString == "server=.;database=test;integrated security=true;Connect Timeout=300");
            Assert.IsTrue(DatabaseSource.Tables.Count > 0);
            Assert.IsTrue(Entities.Count == DatabaseSource.Tables.Count);

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }
    }
}
