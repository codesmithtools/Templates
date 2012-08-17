using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;
using Microsoft.Build.BuildEngine;

namespace QuickStartUtils
{
    public class WebSiteCreator : ProjectCreator
    {
        public WebSiteCreator(ProjectBuilderSettings projectBuilder)
            : base(projectBuilder)
        {
        }

        public override string ProjectTemplateFile
        {
            get { return "WebSite.zip"; }
        }

        protected override void Initialize(string projectName, IEnumerable<SolutionItem> projectReferences)
        {
            string fileName = String.Format("{0}.{1}.webproj", projectName, ProjectBuilder.LanguageAppendage);
            ProjectDirectory = Path.Combine(ProjectBuilder.Location, projectName);
            ProjectFile = new FileInfo(Path.Combine(ProjectDirectory, fileName));
            SolutionItem = new SolutionItem(projectName, ProjectFile.FullName, ProjectBuilder.Language, true, projectReferences);
            ProjectFile = null;
        }

        protected override void AddFiles()
        {
            if (!ProjectBuilder.IncludeDataServices)
                return;

            string directoryName = ProjectDirectory;
            string path = Path.Combine(ProjectBuilder.ZipFileFolder, "DataServiceWebSite.zip");

            using (ZipFile zip = new ZipFile(path))
            {
                zip.ExtractAll(directoryName, ExtractExistingFileAction.DoNotOverwrite);
            }

            string dataService = ProjectBuilder.DatabaseName + "DataService.svc";
            string dataServiceClass = ProjectBuilder.DatabaseName + "DataService." + ProjectBuilder.LanguageAppendage;

            string dataServicePath = Path.Combine(directoryName, dataService);
            string dataServiceClassPath = Path.Combine(directoryName, "App_Code");
            if (!Directory.Exists(dataServiceClassPath))
                Directory.CreateDirectory(dataServiceClassPath);

            dataServiceClassPath = Path.Combine(dataServiceClassPath, dataServiceClass);

            File.Move(
                Path.Combine(directoryName, "DataService.svc"),
                dataServicePath);

            File.Move(
                Path.Combine(directoryName, "DataService." + ProjectBuilder.LanguageAppendage),
                dataServiceClassPath);

            // update vars
            var content = File.ReadAllText(dataServicePath);
            content = content
                .Replace("$safeitemname$", Path.GetFileNameWithoutExtension(dataService));
            File.WriteAllText(dataServicePath, content);

            content = File.ReadAllText(dataServiceClassPath);
            content = content
                .Replace("$safeitemname$", Path.GetFileNameWithoutExtension(dataService));
            File.WriteAllText(dataServiceClassPath, content);
        }

        protected override string ReplaceFileVariables(string content)
        {
            return base.ReplaceFileVariables(content)
                .Replace("$entityNamespace$", ProjectBuilder.DataProjectName)
                .Replace("$datacontext$", ProjectBuilder.DataContextName);
        }
    }
}
