using LinqToSqlShared.DbmlObjectModel;
using LinqToSqlShared.Generator;
using NUnit.Framework;
using SchemaExplorer;
using SchemaExplorer.Serialization;

namespace LinqToSqlShared.Generator.Tests
{
    [TestFixture]
    public class GeneratorTest
    {
        [SetUp()]
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
        public void CreateSample()
        {
            GeneratorSettings settings = new GeneratorSettings();
            settings.MappingFile = "Sample.dbml";
            settings.IncludeViews = true;
            settings.IncludeFunctions = true;
            //settings.IgnoreExpressions.Add(new System.Text.RegularExpressions.Regex("ActiveUser"));

            DbmlGenerator generator = new DbmlGenerator(settings);
            DatabaseSchema databaseSchema = GetDatabaseSchema("Sample");
            Database database = generator.Create(databaseSchema);

            Assert.IsNotNull(database);
        }

        [Test]
        public void CreateLoreSoft()
        {
            GeneratorSettings settings = new GeneratorSettings();
            settings.MappingFile = "LoreSoft.dbml";
            settings.IncludeViews = true;
            settings.IncludeFunctions = true;
            settings.IgnoreExpressions.Add(new System.Text.RegularExpressions.Regex("aspnet_"));

            DbmlGenerator generator = new DbmlGenerator(settings);
            DatabaseSchema databaseSchema = GetDatabaseSchema("LoreSoft");
            Database database = generator.Create(databaseSchema);

            Assert.IsNotNull(database);
        }

        [Test]
        public void CreateUgly()
        {
            GeneratorSettings settings = new GeneratorSettings();
            settings.MappingFile = "Ugly.dbml";
            settings.IncludeViews = true;
            settings.IncludeFunctions = true;

            DbmlGenerator generator = new DbmlGenerator(settings);
            DatabaseSchema databaseSchema = GetDatabaseSchema("Ugly");
            Database database = generator.Create(databaseSchema);

            Assert.IsNotNull(database);

            var t = database.GetTypeByName("UserRole");
            bool b = t.IsManyToMany();
        }

        [Test]
        public void CreateFeedbackRemote()
        {
            GeneratorSettings settings = new GeneratorSettings();
            settings.MappingFile = "Feedback.dbml";
            settings.IncludeViews = true;
            settings.IncludeFunctions = true;

            DbmlGenerator generator = new DbmlGenerator(settings);
            SqlSchemaProvider provider = new SqlSchemaProvider();
            //DatabaseSchema databaseSchema = GetDatabaseSchema("FeedbackRemote");
            DatabaseSchema databaseSchema = new DatabaseSchema(provider, "Data Source=72.249.140.176;Initial Catalog=Feedback;User ID=rbradshaw;Password=get@10code");
            Database database = generator.Create(databaseSchema);

            Assert.IsNotNull(database);
        }

        [Test]
        public void CreateSimple()
        {
            GeneratorSettings settings = new GeneratorSettings();
            settings.MappingFile = "Simple.dbml";
            settings.IncludeViews = true;
            settings.IncludeFunctions = true;

            DbmlGenerator generator = new DbmlGenerator(settings);
            DatabaseSchema databaseSchema = GetDatabaseSchema("Simple");
            Database database = generator.Create(databaseSchema);

            Assert.IsNotNull(database);
        }

        [Test]
        public void CreateTester()
        {
            GeneratorSettings settings = new GeneratorSettings();
            settings.MappingFile = "Tester.dbml";

            DbmlGenerator generator = new DbmlGenerator(settings);
            DatabaseSchema databaseSchema = GetDatabaseSchema("Tester");
            Database database = generator.Create(databaseSchema);

            Assert.IsNotNull(database);
        }

        private DatabaseSchema GetDatabaseSchema(string name)
        {
            var db = DatabaseSchemaSerializer.GetDatabaseSchemaFromName(name);
            db.Database.DeepLoad = true;

            return db;
        }
    }
}
