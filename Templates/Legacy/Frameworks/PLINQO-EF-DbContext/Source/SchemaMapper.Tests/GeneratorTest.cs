using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SchemaExplorer;
using SchemaExplorer.Serialization;
using Formatting = Newtonsoft.Json.Formatting;

namespace SchemaMapper.Tests
{
    [TestClass]
    public class GeneratorTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void GenerateTrackerDatabaseTest()
        {
            var selector = GetDatabaseSchema("Tracker");
            Assert.IsNotNull(selector);
            var generator = new Generator();
            EntityContext entityContext = generator.Generate(selector);

            Assert.IsNotNull(entityContext);

            var settings = new XmlWriterSettings { Indent = true };
            var serializer = new XmlSerializer(typeof(EntityContext));

            using (var writer = XmlWriter.Create(@"..\..\Tracker.xml", settings))
                serializer.Serialize(writer, entityContext);
        }

        [TestMethod]
        public void GenerateUglyDatabaseTest()
        {
            var selector = GetDatabaseSchema("Ugly");
            Assert.IsNotNull(selector);

            var generator = new Generator();
            EntityContext entityContext = generator.Generate(selector);

            Assert.IsNotNull(entityContext);

            var settings = new XmlWriterSettings { Indent = true };
            var serializer = new XmlSerializer(typeof(EntityContext));

            using (var writer = XmlWriter.Create(@"..\..\Ugly.xml", settings))
                serializer.Serialize(writer, entityContext);
        }

        [TestMethod]
        public void GeneratePetshopDatabaseTest()
        {
            var selector = GetDatabaseSchema("Petshop");
            Assert.IsNotNull(selector);

            var generator = new Generator();
            EntityContext entityContext = generator.Generate(selector);

            Assert.IsNotNull(entityContext);

            var settings = new XmlWriterSettings { Indent = true };
            var serializer = new XmlSerializer(typeof(EntityContext));

            using (var writer = XmlWriter.Create(@"..\..\Petshop.xml", settings))
                serializer.Serialize(writer, entityContext);
        }


        [TestMethod]
        public void GeneratePetshopInclusionModeTest()
        {
            var selector = GetDatabaseSchema("Petshop");
            Assert.IsNotNull(selector);

            selector.SelectMode = SelectMode.Include;
            selector.SelectedTables.Add("dbo.Account");
            selector.SelectedTables.Add("dbo.Product");
            selector.SelectedTables.Add("dbo.Category");

            selector.SelectedColumns.Add("dbo.Product.Category");

            selector.ColumnExpressions.Add(@"(ID|Id)$");
            selector.ColumnExpressions.Add(@"Name$");

            var generator = new Generator();
            generator.Settings.TableNaming = TableNaming.Singular;
            generator.Settings.EntityNaming = EntityNaming.Singular;
            generator.Settings.RelationshipNaming = RelationshipNaming.Plural;
            generator.Settings.ContextNaming = ContextNaming.Plural;

            var cleanExpressions = new List<string>
            {
                "^(sp|tbl|udf|vw)_"
            };

            foreach (string s in cleanExpressions)
                if (!string.IsNullOrEmpty(s))
                    generator.Settings.CleanExpressions.Add(s);


            EntityContext entityContext = generator.Generate(selector);

            Assert.IsNotNull(entityContext);

            string json = JsonConvert.SerializeObject(entityContext, Formatting.Indented);
            File.WriteAllText(@"..\..\PetshopInclusion.json", json);
        }

        [TestMethod]
        public void GenerateConnexInclusionModeTest()
        {
            var selector = GetDatabaseSchema("Connex");
            Assert.IsNotNull(selector);

            selector.SelectMode = SelectMode.Include;
            selector.SelectedTables.Add("dbo.Agency");
            selector.SelectedTables.Add("dbo.Client");
            selector.SelectedTables.Add("dbo.Contact");
            selector.SelectedTables.Add("dbo.Contract");
            selector.SelectedTables.Add("dbo.MediaSupplier");
            selector.SelectedTables.Add("dbo.Vendor");


            selector.ColumnExpressions.Add(@"\.Id$");
            selector.ColumnExpressions.Add(@"\.Name$");
            selector.ColumnExpressions.Add(@"UniversalId$");

            var generator = new Generator();
            generator.Settings.TableNaming = TableNaming.Singular;
            generator.Settings.EntityNaming = EntityNaming.Singular;
            generator.Settings.RelationshipNaming = RelationshipNaming.Plural;
            generator.Settings.ContextNaming = ContextNaming.Plural;

            var cleanExpressions = new List<string>
            {
                "^(sp|tbl|udf|vw)_"
            };

            foreach (string s in cleanExpressions)
                if (!string.IsNullOrEmpty(s))
                    generator.Settings.CleanExpressions.Add(s);


            EntityContext entityContext = generator.Generate(selector);

            Assert.IsNotNull(entityContext);

            string json = JsonConvert.SerializeObject(entityContext, Formatting.Indented);
            File.WriteAllText(@"..\..\Connex.json", json);
        }

        private SchemaSelector GetDatabaseSchema(string name)
        {
            var databaseSchema = DatabaseSchemaSerializer.GetDatabaseSchemaFromName(name);            
            
            var selector = new SchemaSelector(databaseSchema.Provider, databaseSchema.ConnectionString);
            selector.Database.DeepLoad = true;

            return selector;
        }

    }
}
