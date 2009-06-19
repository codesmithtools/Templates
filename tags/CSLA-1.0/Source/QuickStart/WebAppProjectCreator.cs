using System;
using System.Collections.Generic;
using System.IO;

using CodeSmith.SchemaHelper;

namespace QuickStart
{
    public class WebAppProjectCreator : VsProjectCreator
    {
        public string CspFileName { get; set; }

        public List<string> ProjectReferences = new List<string>();

        public WebAppProjectCreator(ProjectBuilderSettings projectBuilder)
            : base(projectBuilder) { }

        protected override void GetFiles(PathHelper projectPath)
        {
            GetVsProject(projectPath, ProjectBuilder.FileName);

            // Move/Rename CSP
            File.Copy(Path.Combine(ProjectBuilder.CodeTemplate.CodeTemplateInfo.DirectoryName, @"Common\" + CspFileName),
                Path.Combine(projectPath.DirectoryPath, CspFileName));
        }

        protected override void ReplaceVariables(PathHelper projectPath, string projectGuid, string projectName)
        {
            VariableUpdateDirectory(projectPath, projectGuid, projectName, this.ProjectBuilder.LanguageAppendage, string.Empty);

            //The location in the csproj file directly below where we want to insert the csp include
            QuickStartHelper.FindAndReplace(projectPath.FilePath, ProjectInsertRegex,
                                            String.Format("<ItemGroup>{0}\t\t<Generate Include=\"{1}\" />{0}\t</ItemGroup>{0}\t{2}",
                                                          Environment.NewLine, CspFileName, ProjectInsertLine));

            string CSLAPath = (ProjectBuilder.CopyTemplatesToFolder)
                                  ? @"..\Templates\CSLA\"
                                  : string.Concat(ProjectBuilder.CodeTemplate.CodeTemplateInfo.DirectoryName, @"\"); // Needs Normal Path here

            string cspPath = Path.Combine(projectPath.DirectoryPath, CspFileName);

            QuickStartHelper.FindAndReplace(cspPath, @"\$connectionString\$", ProjectBuilder.SourceDatabase.ConnectionString);
            QuickStartHelper.FindAndReplace(cspPath, @"\$myDatabase\$", ProjectBuilder.SourceDatabase.Database.Name);
            QuickStartHelper.FindAndReplace(cspPath, @"\$myContextNamespace\$", ProjectBuilder.DataProjectName);
            QuickStartHelper.FindAndReplace(cspPath, @"\$language\$", ProjectBuilder.LanguageFolder);
            QuickStartHelper.FindAndReplace(cspPath, @"\$CSLA\$", CSLAPath);

            UpdateWebConfig(projectPath.DirectoryPath);
            AddReferences(projectPath);

            QuickStartHelper.FindAndReplace(projectPath.FilePath, "</Project>",
                String.Format("\t<Import Project=\"$(MSBuildExtensionsPath)\\CodeSmith\\CodeSmith.targets\" />{0}</Project>",
                    Environment.NewLine));

            QuickStartHelper.FindAndReplace(projectPath.FilePath, "<ItemGroup><ProjectReference /></ItemGroup>", BuildProjectReference());
        }

        private void UpdateWebConfig(string directoryPath)
        {
            QuickStartHelper.FindAndReplace(Path.Combine(directoryPath, "web.config"), @"<connectionStrings/>",
                @"<connectionStrings>
					<add name=""" + ProjectBuilder.SourceDatabase.Name + @"ConnectionString"" connectionString=""" + ProjectBuilder.SourceDatabase.ConnectionString + @""" providerName=""System.Data.SqlClient""/>
				</connectionStrings>");
        }

        private void AddReferences(PathHelper projectPath)
        {
            //string includeFormat = String.Format("{0}\t<Reference Include=\"{{0}}\">{0}\t\t<RequiredTargetFramework>3.5</RequiredTargetFramework>{0}\t</Reference>", Environment.NewLine); ;

            //string systemLocationRegex = @"<Reference Include=""System\.Data"" />";
            //string systemData = @"<Reference Include=""System.Data"" />";
            //string systemDataLinq = String.Format(includeFormat, "System.Data.Linq");


            //QuickStartHelper.FindAndReplace(projectPath.FilePath, systemLocationRegex,
            //    String.Concat(systemData, systemDataLinq, codeSmithData, dataServices, dataServicesClient, serviceModel, serviceModelWeb));
        }

        private string BuildProjectReference()
        {
            string projectReferneces = string.Empty;

            foreach (string reference in ProjectReferences)
            {
                if (ProjectBuilder.Language == LanguageEnum.CSharp)
                {
                    projectReferneces += @"<ItemGroup><ProjectReference Include=" + string.Format("\"..\\{0}\\{0}.csproj\"", reference) + "><Project>{C2A03633-B771-4546-8F1E-F1EBFCDACFF8}</Project>" +
                                         string.Format("<Name>{0}</Name>", reference) + "</ProjectReference></ItemGroup>";
                }
                else
                {
                    //TODO: add vb project reference.   
                }
            }

            return projectReferneces;
        }
    }
}
