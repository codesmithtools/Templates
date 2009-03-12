using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using QuickStartUtils;
using SchemaExplorer;
using SchemaExplorer.Serialization;

namespace QuickStart.Tests
{
    [TestFixture]
    public class DataProjectCreatorTest
    {
        [Test]
        public void Create()
        {
            ProjectBuilderSettings pbs = CreateProjectBuilderSettings();

            // Create Data Project
            DataProjectCreator dataProjectCreator = new DataProjectCreator(pbs);
            SolutionItem dataProject = dataProjectCreator.CreateProject(pbs.DataProjectName);
        }

        private ProjectBuilderSettings CreateProjectBuilderSettings()
        {
            
            return new ProjectBuilderSettings()
            {
                SourceDatabase = GetDatabaseSchema("Sample"),
                Location = @".\Sample",
                SolutionName = "Sample",
                Language = LanguageEnum.CSharp,
                ProjectType = ProjectTypeEnum.DynamicDataWebApp,
                IncludeDataServices = true,
                DataProjectName = "Sample.Data",
                CopyTemplatesToFolder = false,
                InterfaceProjectName = "Sample.Web",
                TestProjectName = "Sample.Test",
                IncludeTestProject = true
            };
        }

        private DatabaseSchema GetDatabaseSchema(string name)
        {
            DatabaseSchema db = DatabaseSchemaSerializer.GetDatabaseSchemaFromName(name);
            db.Database.DeepLoad = true;

            return db;
        }

    }
}
