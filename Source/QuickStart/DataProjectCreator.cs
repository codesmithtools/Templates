using System;
using System.Collections.Generic;
using System.IO;

using CodeSmith.SchemaHelper;

namespace QuickStart
{
    public class DataProjectCreator : ProjectCreator
    {
        public DataProjectCreator(ProjectBuilderSettings projectBuilder)
            : base(projectBuilder) { }

        public override string ProjectTemplateFile
        {
            get { return "DataProject.zip"; }
        }

        protected override void AddFiles()
        {
            AddCspFile();
        }

        private void AddCspFile()
        {
            string templateCspFile =  System.IO.Path.Combine("Common", CspFileName);

            string templatePath = CodeSmith.Engine.Utility.PathUtil.RelativePathTo(ProjectDirectory, ProjectBuilder.WorkingDirectory);

            if (!templatePath.EndsWith(@"\"))
                templatePath += @"\";

            var content = File.ReadAllText(Path.Combine(ProjectBuilder.WorkingDirectory, templateCspFile));

            content = content
                .Replace("$connectionString$", ProjectBuilder.SourceDatabase.ConnectionString)
                .Replace("$databaseName$", ProjectBuilder.DatabaseName)
                .Replace("$entityNamespace$", ProjectBuilder.DataProjectName)
                .Replace("$language$", ProjectBuilder.LanguageFolder)
                .Replace("$linqToSql$", templatePath)
                .Replace("$frameworkEnum$", ProjectBuilder.FrameworkVersion == FrameworkVersion.v40 ? "v40" : "v35_SP1")
                .Replace("$languageExtension$", ProjectBuilder.LanguageAppendage);

            if (ProjectBuilder.Language == LanguageEnum.VB)
            {
                 content = content.Replace("CSharp\\BusinessLayer\\", "VisualBasic\\BusinessLayer\\");
                 content = content.Replace("CSharp\\DataAccessLayer\\", "VisualBasic\\DataAccessLayer\\");
            }

            File.WriteAllText(
                Path.Combine(ProjectDirectory, CspFileName),
                content);

            AddNewItem("Generate", CspFileName);
        }
    }
}
