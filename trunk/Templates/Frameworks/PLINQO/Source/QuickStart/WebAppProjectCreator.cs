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
            GetCssFile(projectPath);

            if (ProjectBuilder.IncludeDataServices)
                GetWebServices(projectPath);
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

        private void GetWebServices(PathHelper projectPath)
        {
            string zipFile = (ProjectBuilder.Language == LanguageEnum.CSharp)
                ? "AdoNetDataServiceCSharpWap.zip"
                : "AdoNetDataServiceVBWap.zip";

            UnzipFileAndRename(zipFile, projectPath.DirectoryPath,
                "WebDataService.svc",
                DataServiceFileName,
                String.Concat("WebDataService.svc.", ProjectBuilder.LanguageAppendage),
                DataServiceCodeFileName);

            File.Delete(Path.Combine(projectPath.DirectoryPath, "__TemplateIcon.ico"));
        }

        protected override void ReplaceVariables(PathHelper projectPath, string projectGuid, string projectName)
        {
            VariableUpdateDirectory(projectPath, projectGuid, projectName, this.ProjectBuilder.LanguageAppendage, "Images");

            UpdateWebConfig(projectPath.DirectoryPath);
            UpdateGlobal(projectPath);
            AddReferences(projectPath);

            if (ProjectBuilder.IncludeDataServices)
            {
                UpdateWebServices(projectPath);
                AddWebServicesToProj(projectPath);
            }
        }

        private void UpdateWebConfig(string directoryPath)
        {
            QuickStartUtils.FindAndReplace(Path.Combine(directoryPath, "web.config"), @"<connectionStrings/>",
                @"<connectionStrings>
					<add name=""" + DatabaseName() + @"ConnectionString"" connectionString=""" + ProjectBuilder.SourceDatabase.ConnectionString + @""" providerName=""System.Data.SqlClient""/>
				</connectionStrings>");
        }

        private string DatabaseName()
        {
            return StringUtil.ToPascalCase(ProjectBuilder.SourceDatabase.Name);
        }

        private void UpdateGlobal(PathHelper projectPath)
        {
            CodeTemplate globalTemplate = (ProjectBuilder.Language == LanguageEnum.CSharp)
                ? ProjectBuilder.CodeTemplate.GetCodeTemplateInstance(@"CSharp\Global.cst")
                : globalTemplate = ProjectBuilder.CodeTemplate.GetCodeTemplateInstance(@"VisualBasic\Global.cst");

            globalTemplate.SetProperty("ClassNamespace", ProjectBuilder.InterfaceProjectName);
            globalTemplate.SetProperty("ContextNamespace", ProjectBuilder.DataProjectName);
            globalTemplate.SetProperty("DataContextName",
                String.Format("{0}.{1}DataContext", ProjectBuilder.DataProjectName, DatabaseName()));
            globalTemplate.ContextData["RenderLocation"] = projectPath.DirectoryPath;
            globalTemplate.RenderToString();
        }
        private void AddReferences(PathHelper projectPath)
        {
            string includeFormat = String.Format("{0}\t<Reference Include=\"{{0}}\">{0}\t\t<RequiredTargetFramework>3.5</RequiredTargetFramework>{0}\t</Reference>", Environment.NewLine); ;

            string systemLocationRegex = @"<Reference Include=""System\.Data"" />";
            string systemData = @"<Reference Include=""System.Data"" />";
            string systemDataLinq = String.Format(includeFormat, "System.Data.Linq");

            string codeSmithData = (ProjectBuilder.CopyTemplatesToFolder)
                ? @"..\Templates\LinqToSql"
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

        private void UpdateWebServices(PathHelper projectPath)
        {
            // Update .svc Code Behind
            string dataServicePath = Path.Combine(projectPath.DirectoryPath, DataServiceCodeFileName);
            string dataContextName = String.Format("{0}.{1}DataContext", ProjectBuilder.DataProjectName, DatabaseName());

            if (this.ProjectBuilder.Language == LanguageEnum.CSharp)
            {
                QuickStartUtils.FindAndReplace(dataServicePath,
                    @"// config\.SetEntitySetAccessRule\(""MyEntityset"", EntitySetRights\.AllRead\);",
                    @"config.SetEntitySetAccessRule(""*"", EntitySetRights.AllRead);");

                QuickStartUtils.FindAndReplace(dataServicePath, @" /\* TODO: put your data source class name here \*/ ", dataContextName);
            }
            else
            {
                QuickStartUtils.FindAndReplace(dataServicePath,
                    @"' config\.SetEntitySetAccessRule\(""MyEntityset"", EntitySetRights\.AllRead\)",
                    @"config.SetEntitySetAccessRule(""*"", EntitySetRights.AllRead)");

                QuickStartUtils.FindAndReplace(dataServicePath, @"\[\[class name\]\]", dataContextName);
            }

            UpdateWebServicesVariables(dataServicePath);

            // Update .svc
            UpdateWebServicesVariables(Path.Combine(projectPath.DirectoryPath, DataServiceFileName));
        }
        private void UpdateWebServicesVariables(string fileName)
        {
            QuickStartUtils.FindAndReplace(fileName, @"\$rootnamespace\$", ProjectBuilder.InterfaceProjectName);
            QuickStartUtils.FindAndReplace(fileName, @"\$safeitemname\$", DataServiceName);
        }
        private void AddWebServicesToProj(PathHelper projectPath)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("<ItemGroup>");
            sb.AppendFormat("\t\t<Content Include=\"{0}\" />", DataServiceFileName);
            sb.AppendLine();
            sb.AppendFormat("\t\t<Compile Include=\"{0}\">", DataServiceCodeFileName);
            sb.AppendLine();
            sb.AppendFormat("\t\t\t<DependentUpon>{0}</DependentUpon>", DataServiceFileName);
            sb.AppendLine();
            sb.AppendLine("\t\t</Compile>");
            sb.AppendLine("\t</ItemGroup>");
            sb.Append(ProjectInsertLine);
            QuickStartUtils.FindAndReplace(projectPath.FilePath, ProjectInsertRegex, sb.ToString());
        }

        private string DataServiceName
        {
            get
            {
                return String.Concat(DatabaseName(),
                    "DataService");
            }
        }
        private string DataServiceFileName
        {
            get { return String.Concat(DataServiceName, ".svc"); }
        }
        private string DataServiceCodeFileName
        {
            get { return String.Format("{0}.{1}", DataServiceFileName, ProjectBuilder.LanguageAppendage); }
        }
    }
}
