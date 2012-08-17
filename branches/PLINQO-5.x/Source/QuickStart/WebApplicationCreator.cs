using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;
using Microsoft.Build.BuildEngine;

namespace QuickStartUtils
{
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
            if (!ProjectBuilder.IncludeDataServices)
                return;

            string directoryName = ProjectDirectory;
            string path = Path.Combine(ProjectBuilder.ZipFileFolder, "DataServiceApplication.zip");

            using (ZipFile zip = new ZipFile(path))
            {
                zip.ExtractAll(directoryName, ExtractExistingFileAction.DoNotOverwrite);
            }

            string dataService = ProjectBuilder.DatabaseName + "DataService.svc";
            string dataServiceClass = ProjectBuilder.DatabaseName + "DataService.svc." + ProjectBuilder.LanguageAppendage;

            string dataServicePath = Path.Combine(directoryName, dataService);
            string dataServiceClassPath = Path.Combine(directoryName, dataServiceClass);

            File.Move(
                Path.Combine(directoryName, "DataService.svc"),
                dataServicePath);

            
            File.Move(
                Path.Combine(directoryName, "DataService.svc." + ProjectBuilder.LanguageAppendage),
                dataServiceClassPath);

            Project project = GetProject();
            if (project == null)
                return;

            var serviceItem = project.AddNewItem("Content", dataService);
            var serviceClass = project.AddNewItem("Compile", dataServiceClass);
            serviceClass.SetMetadata("DependentUpon", dataService);

            project.Save(ProjectFile.FullName);

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
