using System;
using System.IO;

namespace QuickStartUtils {
    public class DataProjectCreator : ProjectCreator {
        public DataProjectCreator(ProjectBuilderSettings projectBuilder) : base(projectBuilder) { }

        public override string ProjectTemplateFile {
            get { return "DataProject.zip"; }
        }

        protected override void AddFiles() {
            AddCspFile();
        }

        private void AddCspFile() {
            var cspFileName = Path.ChangeExtension(ProjectFile.Name, ".csp");

            string templateCspFile = ProjectBuilder.QueryPattern == QueryPatternEnum.ManagerClasses
                                         ? @"Common\QuickStartData.csp"
                                         : @"Common\QuickStartQuery.csp";

            string linqToSqlPath = (ProjectBuilder.CopyTemplatesToFolder)
                                       ? @"..\Templates\LinqToSql\"
                                       : ProjectBuilder.WorkingDirectory;

            linqToSqlPath = CodeSmith.Core.IO.PathHelper.RelativePathTo(ProjectDirectory, linqToSqlPath);

            if (!linqToSqlPath.EndsWith(@"\"))
                linqToSqlPath += @"\";

            var content = File.ReadAllText(Path.Combine(ProjectBuilder.WorkingDirectory, templateCspFile));

            content = content
                .Replace("$connectionString$", ProjectBuilder.SourceDatabase.ConnectionString)
                .Replace("$databaseName$", ProjectBuilder.DatabaseName)
                .Replace("$datacontext$", ProjectBuilder.DataContextName)
                .Replace("$entityNamespace$", ProjectBuilder.DataProjectName)
                .Replace("$language$", ProjectBuilder.LanguageFolder)
                .Replace("$linqToSql$", linqToSqlPath)
                .Replace("$frameworkEnum$", GetFrameworkVersionString())
                .Replace("$languageExtension$", ProjectBuilder.LanguageAppendage);

            File.WriteAllText(
                Path.Combine(ProjectDirectory, cspFileName),
                content);

            AddNewItem("Generate", cspFileName);
        }
    }
}