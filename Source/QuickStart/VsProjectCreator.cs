using System;
using System.IO;

using CodeSmith.SchemaHelper;
using Ionic.Zip;

namespace QuickStart
{
    public abstract class VsProjectCreator
    {
        protected ProjectBuilderSettings ProjectBuilder;

        public VsProjectCreator(ProjectBuilderSettings projectBuilder)
        {
            this.ProjectBuilder = projectBuilder;
        }

        public virtual SolutionItem CreateProject(string projectName, params SolutionItem[] dependancies)
        {
            // Create Path Helper
            PathHelper projectPath = new PathHelper(String.Format("{0}.{1}proj", projectName, ProjectBuilder.LanguageAppendage), projectName, ProjectBuilder.Location);

            // Create TargetDirectory
            if (!Directory.Exists(projectPath.DirectoryPath))
                Directory.CreateDirectory(projectPath.DirectoryPath);

            // Create SolutionItem
            SolutionItem solutionItem = new SolutionItem(projectName, projectPath.DirectoryFile, ProjectBuilder.Language);

            // Get Files
            GetFiles(projectPath);

            // Replace Variables In Files
            ReplaceVariables(projectPath, solutionItem.GuidString, projectName);

            // Add Dependancies
            AddDependancies(projectPath.FilePath, dependancies);

            // Return Solution Item
            return solutionItem;
        }

        protected abstract void GetFiles(PathHelper projectPath);
        protected abstract void ReplaceVariables(PathHelper projectPath, string projectGuid, string projectName);

        protected void AddDependancies(string projectFilePath, SolutionItem[] dependancies)
        {
            System.Text.StringBuilder itemGroup = new System.Text.StringBuilder();
            itemGroup.AppendLine("<ItemGroup>");

            foreach (SolutionItem dependancy in dependancies)
            {
                itemGroup.AppendLine(String.Format("\t\t<ProjectReference Include=\"..\\{0}\\{1}\">",
                    Path.GetDirectoryName(dependancy.Path),
                    Path.GetFileName(dependancy.Path)));
                itemGroup.AppendLine(String.Format("\t\t\t<Project>{{{0}}}</Project>", dependancy.GuidString));
                itemGroup.AppendLine(String.Format("\t\t\t<Name>{0}</Name>", dependancy.Name));
                itemGroup.AppendLine("\t\t</ProjectReference>");
            }

            itemGroup.AppendLine("\t</ItemGroup>");
            itemGroup.Append("\t");

            QuickStartHelper.FindAndReplace(projectFilePath, ProjectInsertRegex,
                String.Concat(itemGroup.ToString(), ProjectInsertLine));
        }

        protected virtual void GetVsProject(PathHelper projectPath, string zipFileName)
        {
            // Unzip Folder
            string zipFilePath = String.Format("{0}.zip", zipFileName);
            string zipProjFileName = String.Format("{0}.{1}proj", zipFileName, ProjectBuilder.LanguageAppendage);
            UnzipFileAndRename(zipFilePath, projectPath.DirectoryPath, zipProjFileName, projectPath.FileName);

            // Move Files To SubFolder
            if (ProjectBuilder.Language == LanguageEnum.CSharp)
            {
                FileMoveToSubDirectory("assemblyinfo.cs", projectPath.DirectoryPath, "Properties");
            }
            else if (ProjectBuilder.Language == LanguageEnum.VB)
            {
                FileMoveToSubDirectory("assemblyinfo.vb", projectPath.DirectoryPath, "My Project");
                FileMoveToSubDirectory("Resources.Designer.vb", projectPath.DirectoryPath, "My Project");
                FileMoveToSubDirectory("Resources.resx", projectPath.DirectoryPath, "My Project");
                FileMoveToSubDirectory("Settings.Designer.vb", projectPath.DirectoryPath, "My Project");
                FileMoveToSubDirectory("Settings.settings", projectPath.DirectoryPath, "My Project");
                FileMoveToSubDirectory("MyApplication.Designer.vb", "Application.Designer.vb", projectPath.DirectoryPath, "My Project");
                FileMoveToSubDirectory("MyApplication.myapp", "Application.myapp", projectPath.DirectoryPath, "My Project");
                FileMoveToSubDirectory("MyWebExtension.vb", projectPath.DirectoryPath, "My Project\\MyExtensions");
            }
        }
        protected void FileMoveToSubDirectory(string fileName, string directoryPath, string destinationDirectory)
        {
            FileMoveToSubDirectory(fileName, fileName, directoryPath, destinationDirectory);
        }
        protected void FileMoveToSubDirectory(string fileName, string newFileName, string directoryPath, string destinationDirectory)
        {
            string filePath = QuickStartHelper.FindFileInDirectory(fileName, directoryPath);
            if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                string destinationPath = Path.Combine(directoryPath, destinationDirectory);
                if (!Directory.Exists(destinationPath))
                    Directory.CreateDirectory(destinationPath);
                File.Move(filePath, Path.Combine(destinationPath, newFileName));
            }
        }
        protected void UnzipFileAndRename(string zipFileName, string targetDirectory, params string[] renameFiles)
        {
            // Unzip Files
            ZipFile zip = new ZipFile(Path.Combine(ProjectBuilder.ZipFileFolder, zipFileName));
            foreach (ZipEntry entry in zip)
            {
                if (!File.Exists(Path.Combine(targetDirectory, entry.FileName)))
                {
                    entry.Extract(targetDirectory);
                }
            }

            // Delete the vstemplate
            string vstemplate = QuickStartHelper.FindFileInDirectory(".vstemplate", targetDirectory);
            if (!String.IsNullOrEmpty(vstemplate) && File.Exists(vstemplate))
                File.Delete(vstemplate);

            // Rename Files
            if (renameFiles.Length % 2 != 0)
                throw new Exception("UnzipFile() needs an even number of params: SourceFile, DestinationFile");
            for (int x = 0; x < renameFiles.Length; x += 2)
            {
                string sourFile = Path.Combine(targetDirectory, renameFiles[x]);
                string destFile = Path.Combine(targetDirectory, renameFiles[x + 1]);
                File.Move(sourFile, destFile);
            }
        }

        protected virtual void VariableUpdateDirectory(PathHelper helper, string projectGuid, string projectName, string language, params string[] exemptDirectories)
        {
            string assemblyPath = (ProjectBuilder.Language == LanguageEnum.CSharp)
                ? @"Properties\AssemblyInfo.cs"
                : @"My Project\AssemblyInfo.vb";

            QuickStartHelper.ReplaceAllInDirectory(helper.DirectoryPath, @"\$projectname\$", projectName, exemptDirectories);
            QuickStartHelper.ReplaceAllInDirectory(helper.DirectoryPath, @"\$safeprojectname\$", projectName, exemptDirectories);
            QuickStartHelper.FindAndReplace(helper.FilePath, @"\$guid1\$", @"{" + projectGuid + @"}");
            QuickStartHelper.FindAndReplace(Path.Combine(helper.DirectoryPath, assemblyPath), @"\$guid1\$", Guid.NewGuid().ToString());
            QuickStartHelper.ReplaceAllInDirectory(helper.DirectoryPath, @"\$if\$ \(\$targetframeworkversion\$ == 3.5\)", String.Empty, exemptDirectories);
            QuickStartHelper.ReplaceAllInDirectory(helper.DirectoryPath, @"\$endif\$", String.Empty, exemptDirectories);
            QuickStartHelper.ReplaceAllInDirectory(helper.DirectoryPath, @"\$registeredorganization\$", "CodeSmith Tools, LLC", exemptDirectories);
            QuickStartHelper.ReplaceAllInDirectory(helper.DirectoryPath, @"\$year\$", DateTime.Now.Year.ToString(), exemptDirectories);
            QuickStartHelper.ReplaceAllInDirectory(helper.DirectoryPath, @"\$targetframeworkversion\$", "3.5", exemptDirectories);
        }

        protected virtual string ProjectInsertRegex
        {
            get { return String.Format(@"<Import Project=""\$\(MSBuild(\w){{3,5}}Path\)\\Microsoft\.{0}\.targets"" />", ProjectBuilder.LanguageFolder); }
        }
        protected virtual string ProjectInsertLine
        {
            get { return String.Format("<Import Project=\"$(MSBuildToolsPath)\\Microsoft.{0}.targets\" />", ProjectBuilder.LanguageFolder); }
        }
    }
}
