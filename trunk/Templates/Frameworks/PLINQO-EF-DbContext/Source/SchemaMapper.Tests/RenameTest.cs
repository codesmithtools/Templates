using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SchemaExplorer;
using SchemaExplorer.Serialization;

namespace SchemaMapper.Tests
{
  [TestClass]
  public class RenameTest
  {
    [TestMethod]
    public void RenameEntity()
    {
      DatabaseSchema databaseSchema = GetDatabaseSchema("Tracker");
      Assert.IsNotNull(databaseSchema);

      var generator = new Generator();
      EntityContext entityContext = generator.Generate(databaseSchema);

      Assert.IsNotNull(entityContext);

      var settings = new XmlWriterSettings { Indent = true };
      var serializer = new XmlSerializer(typeof(EntityContext));

      using (var writer = XmlWriter.Create(@"..\..\Tracker.Before.xml", settings))
        serializer.Serialize(writer, entityContext);

      entityContext.RenameEntity("Task", "TaskRename");

      using (var writer = XmlWriter.Create(@"..\..\Tracker.After.xml", settings))
        serializer.Serialize(writer, entityContext);
    }

    [TestMethod]
    public void RenameProperty()
    {
      DatabaseSchema databaseSchema = GetDatabaseSchema("Tracker");
      Assert.IsNotNull(databaseSchema);

      var generator = new Generator();
      EntityContext entityContext = generator.Generate(databaseSchema);

      Assert.IsNotNull(entityContext);

      var settings = new XmlWriterSettings { Indent = true };
      var serializer = new XmlSerializer(typeof(EntityContext));

      using (var writer = XmlWriter.Create(@"..\..\Tracker.Before.xml", settings))
        serializer.Serialize(writer, entityContext);

      entityContext.RenameProperty("Task", "PriorityId", "NewPriorityId");

      using (var writer = XmlWriter.Create(@"..\..\Tracker.After.xml", settings))
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
