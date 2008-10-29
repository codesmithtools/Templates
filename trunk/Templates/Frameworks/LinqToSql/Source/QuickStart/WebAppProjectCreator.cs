using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeSmith.Engine;
using System.IO;

namespace QuickStartUtils
{
    public class WebAppProjectCreator : VsProjectCreator
    {
        public WebAppProjectCreator(ProjectBuilderSettings projectBuilder)
            : base(projectBuilder) { }

        protected override void GetFiles(PathHelper projectPath)
        {
            GetVsProject(projectPath, ProjectBuilder.FileName);

            GetWebServices(projectPath);
        }
        protected override void ReplaceVariables(PathHelper projectPath, string projectGuid, string projectName)
        {
            string[] exemptArray = { "Images" };
            VariableUpdateDirectory(projectPath, projectGuid, projectName, this.ProjectBuilder.LanguageAppendage, exemptArray);
            UpdateWebConfig(projectPath.DirectoryPath);
            UpdateGlobal(projectPath);
            AddReferences(projectPath);
            AddWebServicesToProj(projectPath);
        }

        private void GetWebServices(PathHelper projectPath)
        {
            if (ProjectBuilder.IncludeDataServices)
            {
                string zipFile = (ProjectBuilder.Language == LanguageEnum.CSharp)
                    ? "AdoNetDataServiceCSharpWap.zip"
                    : "AdoNetDataServiceVBWap.zip";
                UnzipFileAndRename(zipFile, projectPath.DirectoryPath,
                    "WebDataService.svc",
                    String.Format("{0}.svc", DataServiceName),
                    String.Format("WebDataService.svc.{0}", ProjectBuilder.LanguageAppendage),
                    String.Format("{0}.svc.{1}", DataServiceName, ProjectBuilder.LanguageAppendage));
            }
        }

        private void UpdateWebConfig(string directoryPath)
        {
            QuickStartUtils.FindAndReplace(Path.Combine(directoryPath, "web.config"), @"<connectionStrings/>",
                @"<connectionStrings>
					<add name=""" + ProjectBuilder.SourceDatabase.Name + @"ConnectionString"" connectionString=""" + ProjectBuilder.SourceDatabase.ConnectionString + @""" providerName=""System.Data.SqlClient""/>
				</connectionStrings>");
        }
        private void UpdateGlobal(PathHelper projectPath)
        {
            CodeTemplate globalTemplate = (ProjectBuilder.Language == LanguageEnum.CSharp)
                ? ProjectBuilder.CodeTemplate.GetCodeTemplateInstance(@"CSharp\Global.cst")
                : globalTemplate = ProjectBuilder.CodeTemplate.GetCodeTemplateInstance(@"VisualBasic\Global.cst");

            globalTemplate.SetProperty("ClassNamespace", ProjectBuilder.InterfaceProjectName);
            globalTemplate.SetProperty("ContextNamespace", ProjectBuilder.DataProjectName);
            globalTemplate.SetProperty("DataContextName",
                String.Format("{0}.{1}DataContext", ProjectBuilder.DataProjectName, ProjectBuilder.SourceDatabase.Name));
            globalTemplate.ContextData["RenderLocation"] = projectPath.DirectoryPath;
            globalTemplate.RenderToString();
        }
        private void AddWebServicesToProj(PathHelper projectPath)
        {
            if (ProjectBuilder.IncludeDataServices)
            {
                // Update .svc Code Behind
                string fileName = Path.Combine(projectPath.DirectoryPath, String.Format("{0}.svc.{1}", DataServiceName, ProjectBuilder.LanguageAppendage));
                UpdateWebServicesVariables(fileName);
                string dataContextName = (ProjectBuilder.Language == LanguageEnum.CSharp)
                    ? @" /\* TODO: put your data source class name here \*/ "
                    : @"\[\[class name\]\]";
                QuickStartUtils.FindAndReplace(fileName, dataContextName,
                    String.Format("{0}.{1}DataContext", ProjectBuilder.DataProjectName, ProjectBuilder.SourceDatabase.Name));

                // Update .svc
                fileName = Path.Combine(projectPath.DirectoryPath, String.Format("{0}.svc", DataServiceName));
                UpdateWebServicesVariables(fileName);

                // Update Project
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendLine("<ItemGroup>");
                sb.AppendFormat("\t\t<Content Include=\"{0}.svc\" />", DataServiceName);
                sb.AppendLine();
                sb.AppendFormat("\t\t<Compile Include=\"{0}.svc.{1}\">", DataServiceName, ProjectBuilder.LanguageAppendage);
                sb.AppendLine();
                sb.AppendFormat("\t\t\t<DependentUpon>{0}.svc</DependentUpon>", DataServiceName);
                sb.AppendLine();
                sb.AppendLine("\t\t</Compile>");
                sb.AppendLine("\t</ItemGroup>");
                sb.Append(ProjectInsertLine);
                QuickStartUtils.FindAndReplace(projectPath.FilePath, ProjectInsertRegex, sb.ToString());
            }
        }
        private void UpdateWebServicesVariables(string fileName)
        {
            QuickStartUtils.FindAndReplace(fileName, @"\$rootnamespace\$", ProjectBuilder.InterfaceProjectName);
            QuickStartUtils.FindAndReplace(fileName, @"\$safeitemname\$", DataServiceName);
        }
        private void AddReferences(PathHelper projectPath)
        {
            string includeFormat = String.Format("{0}\t<Reference Include=\"{{0}}\">{0}\t\t<RequiredTargetFramework>3.5</RequiredTargetFramework>{0}\t</Reference>", Environment.NewLine); ;

            string systemLocationRegex = @"<Reference Include=""System\.Data"" />";
            string systemData = @"<Reference Include=""System.Data"" />";
            string systemDataLinq = String.Format(includeFormat, "System.Data.Linq");

            string codeSmithData = (ProjectBuilder.CopyTemplatesToFolder)
                ? @"..\LinqToSql"
                : ProjectBuilder.CodeTemplate.CodeTemplateInfo.DirectoryName;

            codeSmithData = String.Concat(@"<Reference Include=""CodeSmith.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=596a5eee5d207fdd, processorArchitecture=MSIL"">
      <SpecificVersion>False</SpecificVersion>
      <HintPath>", codeSmithData, @"\Common\CodeSmith.Data.dll</HintPath>
	</Reference>");
            
            string dataServices = String.Format(includeFormat, "System.Data.Services");
            string dataServicesClient = String.Format(includeFormat, "System.Data.Services.Client");
            string serviceModel = String.Format(includeFormat, "System.ServiceModel");
            string serviceModelWeb = String.Format(includeFormat, "System.ServiceModel.Web");

            QuickStartUtils.FindAndReplace(projectPath.FilePath, systemLocationRegex,
                String.Concat(systemData, systemDataLinq, codeSmithData, dataServices, dataServicesClient, serviceModel, serviceModelWeb));
        }

        private string DataServiceName
        {
            get { return String.Format("{0}DataService", ProjectBuilder.SourceDatabase.Name); }
        }
    }
}
