using System.IO;

namespace CodeSmith.QuickStart
{
    using CodeSmith.SchemaHelper;

    public class WebApplicationCreator : ProjectCreator
    {
        public WebApplicationCreator(ProjectBuilderSettings projectBuilder)
            : base(projectBuilder)
        {
        }

        public override string ProjectTemplateFile
        {
            get { return "WebApplication.zip"; }
        }

        protected override void AddFiles()
        {
            AddCspFile();

            //if (!ProjectBuilder.IncludeDataServices)
            //    return;

            //string directoryName = ProjectDirectory;
            //string path = Path.Combine(ProjectBuilder.ZipFileFolder, "DataServiceApplication.zip");

            //using (ZipFile zip = new ZipFile(path))
            //{
            //    zip.ExtractAll(directoryName, ExtractExistingFileAction.DoNotOverwrite);
            //}

            //string dataService = ProjectBuilder.DatabaseName + "DataService.svc";
            //string dataServiceClass = ProjectBuilder.DatabaseName + "DataService.svc." + ProjectBuilder.LanguageAppendage;

            //string dataServicePath = Path.Combine(directoryName, dataService);
            //string dataServiceClassPath = Path.Combine(directoryName, dataServiceClass);

            //File.Move(
            //    Path.Combine(directoryName, "DataService.svc"),
            //    dataServicePath);

            //File.Move(
            //    Path.Combine(directoryName, "DataService.svc." + ProjectBuilder.LanguageAppendage),
            //    dataServiceClassPath);

            //Project project = GetProject();
            //if (project == null)
            //    return;

            //var serviceItem = project.AddNewItem("Content", dataService);
            //var serviceClass = project.AddNewItem("Compile", dataServiceClass);
            //serviceClass.SetMetadata("DependentUpon", dataService);

            //project.Save(ProjectFile.FullName);

            //// update vars
            //var content = File.ReadAllText(dataServicePath);
            //content = content
            //    .Replace("$safeitemname$", Path.GetFileNameWithoutExtension(dataService));
            //File.WriteAllText(dataServicePath, content);

            //content = File.ReadAllText(dataServiceClassPath);
            //content = content
            //    .Replace("$safeitemname$", Path.GetFileNameWithoutExtension(dataService));
            //File.WriteAllText(dataServiceClassPath, content);
        }

        private void AddCspFile()
        {
            string templateCspFile = System.IO.Path.Combine("Common", CspFileName);

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
                content = content.Replace("CSharp\\WebLayer\\", "VisualBasic\\WebLayer\\");
            }

            File.WriteAllText(
                Path.Combine(ProjectDirectory, CspFileName),
                content);

            AddNewItem("Generate", CspFileName);
        }

        protected override string ReplaceFileVariables(string content)
        {
            return base.ReplaceFileVariables(content).Replace("$entityNamespace$", ProjectBuilder.DataProjectName);
        }
    }
}
