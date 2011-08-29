using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;
using Microsoft.Build.BuildEngine;

namespace QuickStartUtils
{
    public abstract class ProjectCreator
    {
        protected ProjectBuilderSettings ProjectBuilder { get; private set; }

        public abstract string ProjectTemplateFile { get; }

        public string ProjectName { get; private set; }
        public string ProjectDirectory { get; protected set; }
        public FileInfo ProjectFile { get; protected set; }
        public SolutionItem SolutionItem { get; protected set; }

        public ProjectCreator(ProjectBuilderSettings projectBuilder)
        {
            ProjectBuilder = projectBuilder;
        }

        public virtual void CreateProject(string projectName)
        {
            CreateProject(projectName, new SolutionItem[] { });
        }

        public virtual void CreateProject(string projectName, params SolutionItem[] dependancies)
        {
            ProjectName = projectName;
            Initialize(projectName, dependancies);


            //1) extract project template
            ExtractTemplate();

            //2) rename project file
            RenameProject();

            //3) add quick start files
            AddFiles();

            //4) replace project variables
            ReplaceVariables();

            //5) add dependancies
            if (dependancies != null && dependancies.Length > 0)
                AddProjectReferences(dependancies);

            AddReferences();
        }

        protected virtual void Initialize(string projectName, IEnumerable<SolutionItem> projectReferences)
        {
            string fileName = string.Format("{0}.{1}proj", projectName, ProjectBuilder.LanguageAppendage);
            ProjectDirectory = Path.Combine(ProjectBuilder.Location, projectName);
            ProjectFile = new FileInfo(Path.Combine(ProjectDirectory, fileName));
            SolutionItem = new SolutionItem(projectName, ProjectFile.FullName, ProjectBuilder.Language, false, projectReferences);
        }

        protected virtual void ExtractTemplate()
        {
            string zipPath = Path.Combine(ProjectBuilder.ZipFileFolder, ProjectTemplateFile);
            using (ZipFile zipFile = new ZipFile(zipPath))
            {
                zipFile.ExtractAll(ProjectDirectory, ExtractExistingFileAction.DoNotOverwrite);
            }
        }

        protected virtual void RenameProject()
        {
            if (ProjectFile == null)
                return;

            string extenstion = string.Format(".{0}proj", ProjectBuilder.LanguageAppendage);

            string original = Path.ChangeExtension(
                Path.GetFileName(ProjectTemplateFile),
                extenstion);

            FileInfo originalFile = new FileInfo(Path.Combine(ProjectDirectory, original));
            originalFile.MoveTo(ProjectFile.FullName);
        }

        protected abstract void AddFiles();

        protected void AddNewItem(string itemName, string fileName)
        {
            Project project = GetProject();
            if (project == null)
                return;

            var buildItem = project.AddNewItem(itemName, fileName);
            project.Save(ProjectFile.FullName);
        }

        protected void AddProjectReference(SolutionItem solutionItem)
        {
            AddProjectReferences(new[] { solutionItem });
        }

        protected void AddProjectReferences(IEnumerable<SolutionItem> solutionItems)
        {
            Project project = GetProject();
            if (project == null)
                return;

            foreach (var solutionItem in solutionItems)
            {
                string path = CodeSmith.Core.IO.PathHelper.RelativePathTo(ProjectDirectory, solutionItem.Path);
                var buildItem = project.AddNewItem("ProjectReference", path);
                buildItem.SetMetadata("Project", solutionItem.Guid.ToString("B"));
                buildItem.SetMetadata("Name", solutionItem.Name);
            }

            project.Save(ProjectFile.FullName);
        }

        protected virtual void ReplaceVariables()
        {
            var files = GetVariableFiles();

            foreach (var f in files)
            {
                var content = File.ReadAllText(f);
                content = ReplaceFileVariables(content);
                File.WriteAllText(f, content);
            }
        }

        protected virtual IEnumerable<string> GetVariableFiles()
        {
            var files = PathHelper.GetFiles(ProjectDirectory, SearchOption.AllDirectories,
                "*.*proj",
                "*.config",
                "*." + ProjectBuilder.LanguageAppendage,
                "*.as*x",
                "*.svc",
                "*.master");

            return files;
        }

        protected virtual string ReplaceFileVariables(string content)
        {
            string connectionString = ProjectBuilder.SourceDatabase.ConnectionString;
            connectionString = QuickStartUtils.EnsureMultipleResultSets(connectionString);

            return content
                .Replace("$projectname$", ProjectName)
                .Replace("$safeprojectname$", ProjectName)
                .Replace("$rootnamespace$", ProjectName)
                .Replace("$guid1$", SolutionItem.Guid.ToString("B"))
                .Replace("$assemblyGuid$", Guid.NewGuid().ToString())
                .Replace("$registeredorganization$", "CodeSmith Tools, LLC")
                .Replace("$year$", DateTime.Now.Year.ToString())
                .Replace("$targetframeworkversion$", ProjectBuilder.FrameworkString)
                .Replace("$frameworkEnum$", ProjectBuilder.FrameworkVersion == FrameworkVersion.v40 ? "v40" : "v35_SP1")
                .Replace("$datacontext$", ProjectBuilder.DataContextName)
                .Replace("$entityNamespace$", ProjectBuilder.DataProjectName)
                .Replace("$databaseName$", ProjectBuilder.DatabaseName)
                .Replace("$connectionString$", connectionString);
        }

        protected virtual void AddReferences()
        {
            var project = GetProject();
            if (project == null)
                return;

            var item = project.AddNewItem("Reference", "CodeSmith.Data");

            string fullPath = Path.Combine(ProjectBuilder.WorkingDirectory, "Common");
            fullPath = Path.Combine(fullPath, ProjectBuilder.FrameworkFolder);
            fullPath = Path.Combine(fullPath, "CodeSmith.Data.dll");

            string relativePath = CodeSmith.Core.IO.PathHelper.RelativePathTo(ProjectDirectory, fullPath);

            item.SetMetadata("HintPath", relativePath);

            item = project.AddNewItem("Reference", "CodeSmith.Data.LinqToSql");

            fullPath = Path.Combine(ProjectBuilder.WorkingDirectory, "Common");
            fullPath = Path.Combine(fullPath, ProjectBuilder.FrameworkFolder);
            fullPath = Path.Combine(fullPath, "CodeSmith.Data.LinqToSql.dll");

            relativePath = CodeSmith.Core.IO.PathHelper.RelativePathTo(ProjectDirectory, fullPath);

            item.SetMetadata("HintPath", relativePath);

            project.Save(ProjectFile.FullName);
        }

        protected Project GetProject()
        {
            if (ProjectFile == null)
                return null;

            var project = new Project();
            project.Load(ProjectFile.FullName, ProjectLoadSettings.IgnoreMissingImports);
            return project;
        }

    }
}
