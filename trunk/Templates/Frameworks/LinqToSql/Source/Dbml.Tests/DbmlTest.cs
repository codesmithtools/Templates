using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using LinqToSqlShared.DbmlObjectModel;

namespace LinqToSqlShared.DbmlObjectModel.Tests
{
    [TestFixture()]
    public class DbmlTest
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

        [Test()]
        public void Load()
        {
            Database db = Dbml.FromFile(@"..\..\LoreSoft.dbml");

            Assert.IsNotNull(db);
        }
    }
}
