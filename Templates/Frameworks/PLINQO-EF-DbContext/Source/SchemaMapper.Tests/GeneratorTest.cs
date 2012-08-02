using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SchemaExplorer;
using SchemaExplorer.Serialization;

namespace SchemaMapper.Tests
{
    [TestClass]
    public class GeneratorTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void GenerateTrackerDatabaseTest()
        {
            DatabaseSchema databaseSchema = GetDatabaseSchema("Tracker");
            Assert.IsNotNull(databaseSchema);

            var generator = new Generator();
            EntityContext entityContext = generator.Generate(databaseSchema);

            Assert.IsNotNull(entityContext);

            var settings = new XmlWriterSettings { Indent = true };
            var serializer = new XmlSerializer(typeof(EntityContext));

            using (var writer = XmlWriter.Create(@"..\..\Tracker.xml", settings))
                serializer.Serialize(writer, entityContext);
        }

        [TestMethod]
        public void GenerateUglyDatabaseTest()
        {
            DatabaseSchema databaseSchema = GetDatabaseSchema("Ugly");
            Assert.IsNotNull(databaseSchema);

            var generator = new Generator();
            EntityContext entityContext = generator.Generate(databaseSchema);

            Assert.IsNotNull(entityContext);

            var settings = new XmlWriterSettings { Indent = true };
            var serializer = new XmlSerializer(typeof(EntityContext));

            using (var writer = XmlWriter.Create(@"..\..\Ugly.xml", settings))
                serializer.Serialize(writer, entityContext);
        }

        [TestMethod]
        public void GeneratePetshopDatabaseTest()
        {
            DatabaseSchema databaseSchema = GetDatabaseSchema("Petshop");
            Assert.IsNotNull(databaseSchema);

            var generator = new Generator();
            EntityContext entityContext = generator.Generate(databaseSchema);

            Assert.IsNotNull(entityContext);

            var settings = new XmlWriterSettings { Indent = true };
            var serializer = new XmlSerializer(typeof(EntityContext));

            using (var writer = XmlWriter.Create(@"..\..\Petshop.xml", settings))
                serializer.Serialize(writer, entityContext);
        }


        [TestMethod]
        public void GeneratePetshopInclusionModeTest()
        {
            DatabaseSchema databaseSchema = GetDatabaseSchema("Petshop");
            Assert.IsNotNull(databaseSchema);

            var generator = new Generator();
            generator.Settings.TableNaming = TableNaming.Singular;
            generator.Settings.EntityNaming = EntityNaming.Singular;
            generator.Settings.RelationshipNaming = RelationshipNaming.Plural;
            generator.Settings.ContextNaming = ContextNaming.Plural;
            generator.Settings.InclusionMode = true;

            var ignoreList = new List<string>
            {
                "dbo.Account",
                "dbo.Product",
                "dbo.Category"
            };

            foreach (string s in ignoreList)
                if (!string.IsNullOrEmpty(s))
                    generator.Settings.IgnoreExpressions.Add(s);

            var cleanExpressions = new List<string>
            {
                "^(sp|tbl|udf|vw)_"
            };

            foreach (string s in cleanExpressions)
                if (!string.IsNullOrEmpty(s))
                    generator.Settings.CleanExpressions.Add(s);


            EntityContext entityContext = generator.Generate(databaseSchema);

            Assert.IsNotNull(entityContext);

            var settings = new XmlWriterSettings { Indent = true };
            var serializer = new XmlSerializer(typeof(EntityContext));

            using (var writer = XmlWriter.Create(@"..\..\PetshopInclusion.xml", settings))
                serializer.Serialize(writer, entityContext);
        }

        private DatabaseSchema GetDatabaseSchema(string name)
        {
            var db = DatabaseSchemaSerializer.GetDatabaseSchemaFromName(name);
            db.Database.DeepLoad = true;

            return db;
        }

    }
}
