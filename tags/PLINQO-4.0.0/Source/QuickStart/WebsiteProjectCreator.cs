using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeSmith.Engine;
using System.IO;

namespace QuickStartUtils
{
    public class WebsiteProjectCreator : VsProjectCreator
    {
        public WebsiteProjectCreator(ProjectBuilderSettings projectBuilder)
            : base(projectBuilder) { }

        public override SolutionItem CreateProject(string projectName, params SolutionItem[] dependancies)
        {

            // Create Path Helper
            PathHelper projectPath = new PathHelper(String.Format("{0}.{1}.webproj", projectName, ProjectBuilder.LanguageAppendage), projectName, ProjectBuilder.Location);

            // Create TargetDirectory
            if (!Directory.Exists(projectPath.DirectoryPath))
                Directory.CreateDirectory(projectPath.DirectoryPath);

            // Create SolutionItem
            SolutionItem solutionItem = new SolutionItem(projectName, projectPath.DirectoryFile, ProjectBuilder.Language, true, dependancies);

            // Get Files
            GetFiles(projectPath);

            // Replace Variables In Files
            ReplaceVariables(projectPath, solutionItem.GuidString, projectName);

            // Add Dependancies
            AddDependancies(projectPath.FilePath, dependancies);

            string webprojFile = String.Format("{0}.{1}.webproj", ProjectBuilder.InterfaceProjectName, ProjectBuilder.LanguageAppendage);
            string webprojPath = QuickStartUtils.FindFileInDirectory(webprojFile, projectPath.DirectoryPath);

            File.Delete(webprojPath);

            if (ProjectBuilder.IncludeDataServices)
            {
                Directory.CreateDirectory(Path.Combine(projectPath.DirectoryPath, "App_Code"));
                string svcDataFile = String.Format("{0}.{1}", DataServiceName, ProjectBuilder.LanguageAppendage);
                string svcDataFileFixed = String.Format("{0}.{1}", DataServiceName, ProjectBuilder.LanguageAppendage);
                string svcDataPath = Path.Combine(Path.Combine(projectPath.DirectoryPath, "App_Code"), svcDataFileFixed);
                File.Move(Path.Combine(projectPath.DirectoryPath, svcDataFile), svcDataPath);
            }

            // Return Solution Item
            return solutionItem;
        }

        protected override void GetFiles(PathHelper projectPath)
        {
            GetVsProject(projectPath, ProjectBuilder.FileName);
            GetWebServices(projectPath);
            GetCssFile(projectPath);
        }
        protected override void ReplaceVariables(PathHelper projectPath, string projectGuid, string projectName)
        {
            string[] exemptArray = { "Images" };
            VariableUpdateDirectory(projectPath, projectGuid, projectName, this.ProjectBuilder.LanguageAppendage, exemptArray);
            UpdateWebConfig(projectPath.DirectoryPath);
            UpdateGlobal(projectPath);
            AddWebServicesToProj(projectPath);
        }

        private void GetWebServices(PathHelper projectPath)
        {
            if (ProjectBuilder.IncludeDataServices)
            {
                string zipFile = (ProjectBuilder.Language == LanguageEnum.CSharp)
                    ? "AdoNetDataServiceWebsite.zip"
                    : "AdoNetDataServiceWebsite.zip";
                UnzipFileAndRename(zipFile, projectPath.DirectoryPath,
                    "WebDataService.svc",
                    String.Format("{0}.svc", DataServiceName),
                    String.Format("WebDataService.{0}", ProjectBuilder.LanguageAppendage),
                    String.Format("{0}.{1}", DataServiceName, ProjectBuilder.LanguageAppendage));
            }
        }

        private void UpdateWebConfig(string directoryPath)
        {
            QuickStartUtils.FindAndReplace(Path.Combine(directoryPath, "web.config"), @"<connectionStrings/>",
                @"<connectionStrings>
					<add name=""" + DatabaseName + @"ConnectionString"" connectionString=""" + ProjectBuilder.SourceDatabase.ConnectionString + @""" providerName=""System.Data.SqlClient""/>
				</connectionStrings>");
        }

        private string DatabaseName
        {
            get { return StringUtil.ToPascalCase(ProjectBuilder.SourceDatabase.Name); }
        }

        private void UpdateGlobal(PathHelper projectPath)
        {
            string globalPath = QuickStartUtils.FindFileInDirectory("Global.asax", projectPath.DirectoryPath);
            string contextLine = string.Empty;
            string contextReplace = string.Empty;

            if (ProjectBuilder.Language == LanguageEnum.CSharp)
            {
                contextLine = @"//model.RegisterContext\(typeof\(YourDataContextType\), new ContextConfiguration\(\) \{ ScaffoldAllTables = false \}\);";
                contextReplace = String.Concat(@"model.RegisterContext(typeof(",
                ProjectBuilder.DataProjectName,
                @".",
                DatabaseName,
                @"DataContext), new ContextConfiguration() { ScaffoldAllTables = false });");
            }
            else
            {
                contextLine = @"' model.RegisterContext\(GetType\(YourDataContextType\), New ContextConfiguration\(\) With \{\.ScaffoldAllTables = False\}\)";
                contextReplace = String.Concat(@"model.RegisterContext(GetType(",
                ProjectBuilder.DataProjectName,
                @".",
                DatabaseName,
                @"DataContext), New ContextConfiguration() With { .ScaffoldAllTables = False })");
            }

            QuickStartUtils.FindAndReplace(globalPath, contextLine, contextReplace);
        }
        private void AddWebServicesToProj(PathHelper projectPath)
        {
            if (ProjectBuilder.IncludeDataServices)
            {
                // Update .svc Code Behind
                string fileName = Path.Combine(projectPath.DirectoryPath, String.Format("{0}.{1}", DataServiceName, ProjectBuilder.LanguageAppendage));
                UpdateWebServicesVariables(fileName);
                string dataContextName = (ProjectBuilder.Language == LanguageEnum.CSharp)
                    ? @" /\* TODO: put your data source class name here \*/ "
                    : @"\[\[class name\]\]";
                QuickStartUtils.FindAndReplace(fileName, dataContextName,
                    String.Format("{0}.{1}DataContext", ProjectBuilder.DataProjectName, DatabaseName));

                // Update .svc
                fileName = Path.Combine(projectPath.DirectoryPath, String.Format("{0}.svc", DataServiceName));
                UpdateWebServicesVariables(fileName);

                // Update web.config
                fileName = Path.Combine(projectPath.DirectoryPath, "web.config");
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendLine(@"  <system.serviceModel>");
                sb.AppendLine(@"    <serviceHostingEnvironment aspNetCompatibilityEnabled=""true"" />");
                sb.AppendLine(@"  </system.serviceModel>");
                sb.AppendLine(@"</configuration>");
                QuickStartUtils.FindAndReplace(fileName, @"</configuration>", sb.ToString());

                sb = new System.Text.StringBuilder();
                sb.AppendLine(@"        <add assembly=""System.Data.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089"" />");
                sb.AppendLine(@"        <add assembly=""System.Data.Services, Version=3.5.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089"" />");
                sb.AppendLine(@"        <add assembly=""System.Data.Services.Client, Version=3.5.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089"" />");
                sb.AppendLine(@"      </assemblies>");

                QuickStartUtils.FindAndReplace(fileName, @"      </assemblies>", sb.ToString());
            }
        }

        private void UpdateWebServicesVariables(string fileName)
        {
            QuickStartUtils.FindAndReplace(fileName, @"\$rootnamespace\$", ProjectBuilder.InterfaceProjectName);
            QuickStartUtils.FindAndReplace(fileName, @"\$safeitemname\$", DataServiceName);
        }


        protected override void VariableUpdateDirectory(PathHelper helper, string projectGuid, string projectName, string language, string[] exemptDirectories)
        {
            QuickStartUtils.ReplaceAllInDirectory(helper.DirectoryPath, @"\$projectname\$", projectName, exemptDirectories);
            QuickStartUtils.ReplaceAllInDirectory(helper.DirectoryPath, @"\$safeprojectname\$", projectName, exemptDirectories);
            QuickStartUtils.FindAndReplace(helper.FilePath, @"\$guid1\$", @"{" + projectGuid + @"}");
            QuickStartUtils.ReplaceAllInDirectory(helper.DirectoryPath, @"\$if\$ \(\$targetframeworkversion\$ == 3.5\)", String.Empty, exemptDirectories);
            QuickStartUtils.ReplaceAllInDirectory(helper.DirectoryPath, @"\$endif\$", String.Empty, exemptDirectories);
            QuickStartUtils.ReplaceAllInDirectory(helper.DirectoryPath, @"\$registeredorganization\$", "CodeSmith Tools, LLC", exemptDirectories);
            QuickStartUtils.ReplaceAllInDirectory(helper.DirectoryPath, @"\$year\$", DateTime.Now.Year.ToString(), exemptDirectories);
            QuickStartUtils.ReplaceAllInDirectory(helper.DirectoryPath, @"\$targetframeworkversion\$", "3.5", exemptDirectories);
        }

        protected override void GetVsProject(PathHelper projectPath, string zipFileName)
        {
            // Unzip Folder
            string zipFilePath = String.Format("{0}.zip", zipFileName);
            string zipProjFileName = String.Format("{0}.{1}.webproj", zipFileName, ProjectBuilder.LanguageAppendage);
            UnzipFileAndRename(zipFilePath, projectPath.DirectoryPath, zipProjFileName, projectPath.FileName);
        }

        private string DataServiceName
        {
            get { return String.Format("{0}DataService", StringUtil.ToPascalCase(ProjectBuilder.SourceDatabase.Name)); }
        }

        private void GetCssFile(PathHelper projectPath)
        {
            string stylesPath = Path.Combine(ProjectBuilder.CodeTemplate.CodeTemplateInfo.DirectoryName, @"Common\Styles");
            string styleFile = Path.Combine(stylesPath, @"Site.css");

            string cssLocation = Path.Combine(projectPath.DirectoryPath, @"Site.css");
            File.Copy(styleFile, cssLocation, true);

            string imageDirectory = Path.Combine(projectPath.DirectoryPath, @"DynamicData\Content\Images");
            string imageLocation = Path.Combine(imageDirectory, @"bg.gif");
            string imageFile = Path.Combine(stylesPath, @"bg.gif");
            File.Copy(imageFile, imageLocation);

            imageLocation = Path.Combine(imageDirectory, @"table-header-background.gif");
            imageFile = Path.Combine(stylesPath, @"table-header-background.gif");
            File.Copy(imageFile, imageLocation);
        }
    }
}
