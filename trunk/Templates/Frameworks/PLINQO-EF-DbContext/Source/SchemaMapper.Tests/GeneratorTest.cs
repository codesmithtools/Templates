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


    private DatabaseSchema GetDatabaseSchema(string name)
    {
      var db = DatabaseSchemaSerializer.GetDatabaseSchemaFromName(name);
      db.Database.DeepLoad = true;

      return db;
    }

  }
}
