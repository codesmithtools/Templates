using System.Xml;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SchemaExplorer;
using SchemaExplorer.Serialization;

namespace SchemaMapper.Tests
{
    [TestClass]
    public class ParserTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void ParseContext()
        {
            string contextFile = @"..\..\..\Tracker.Core\TrackerContext.Generated.cs";
            var result = ContextParser.Parse(contextFile);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ParseMapping()
        {
            string mappingFile = @"..\..\..\Tracker.Core\Mapping\TaskMap.Generated.cs";

            var result = MappingParser.Parse(mappingFile);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ParseManyToManyMapping()
        {
            string mappingFile = @"..\..\..\Tracker.Core\Mapping\RoleMap.Generated.cs";

            var result = MappingParser.Parse(mappingFile);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ParseTwoForeignKeyMapping()
        {
            string mappingFile = @"..\..\..\Ugly.Data\Mapping\TwoForeignKeyMap.Generated.cs";

            var result = MappingParser.Parse(mappingFile);
            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void ParseTracker()
        {
            var generator = new Generator();
            generator.Settings.ContextNaming = ContextNaming.Preserve;
            generator.Settings.EntityNaming = EntityNaming.Singular;
            generator.Settings.RelationshipNaming = RelationshipNaming.ListSuffix;
            generator.Settings.TableNaming = TableNaming.Singular;

            var selector = GetDatabaseSchema("Tracker");
            Assert.IsNotNull(selector);

            EntityContext entityContext = generator.Generate(selector);

            Assert.IsNotNull(entityContext);

            var settings = new XmlWriterSettings { Indent = true };
            var serializer = new XmlSerializer(typeof(EntityContext));

            using (var writer = XmlWriter.Create(@"..\..\Tracker.Generated.xml", settings))
                serializer.Serialize(writer, entityContext);

            string contextDirectory = @"..\..\..\Tracker.Core";
            string mappingDirectory = @"..\..\..\Tracker.Core\Mapping";

            Synchronizer.UpdateFromSource(entityContext, contextDirectory, mappingDirectory);

            using (var writer = XmlWriter.Create(@"..\..\Tracker.Updated.xml", settings))
                serializer.Serialize(writer, entityContext);
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