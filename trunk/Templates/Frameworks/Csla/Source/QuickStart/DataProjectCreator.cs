using System;
using System.Collections.Generic;
using System.IO;

using CodeSmith.SchemaHelper;

namespace QuickStart
{
    public class DataProjectCreator : VsProjectCreator
    {
        public string CspFileName { get; set; }

        public List<string> ProjectReferences = new List<string>();

        public DataProjectCreator(ProjectBuilderSettings projectBuilder)
            : base(projectBuilder) { }

        protected override void GetFiles(PathHelper projectPath)
        {
            GetVsProject(projectPath, "ClassLibrary");

            // Move/Rename CSP
            File.Copy(Path.Combine(ProjectBuilder.CodeTemplate.CodeTemplateInfo.DirectoryName, @"Common\" + CspFileName),
                Path.Combine(projectPath.DirectoryPath, CspFileName));

            // Delete Class1
            string class1File = String.Concat("Class1.", ProjectBuilder.LanguageAppendage);
            string class1Path = QuickStartHelper.FindFileInDirectory(class1File, projectPath.DirectoryPath);
            if (!String.IsNullOrEmpty(class1Path) && File.Exists(class1Path))
            {
                // Delete File
                File.Delete(class1Path);

                // Remove From Proj
                // Yes, There is a space in the csproj file while there is not in the vbproj file
                string classLine = (ProjectBuilder.Language == LanguageEnum.CSharp)
                    ? String.Format(@"<Compile Include=""{0}"" />", class1File)
                    : String.Format(@"<Compile Include=""{0}""/>", class1File);

                QuickStartHelper.FindAndReplace(projectPath.FilePath, classLine, String.Empty);
            }
        }
        protected override void ReplaceVariables(PathHelper projectPath, string projectGuid, string projectName)
        {
            VariableUpdateDirectory(projectPath, projectGuid, projectName, this.ProjectBuilder.LanguageAppendage, null);

            string CSLAPath = (ProjectBuilder.CopyTemplatesToFolder)
                ? @"..\Templates\CSLA\"
                : string.Concat(ProjectBuilder.CodeTemplate.CodeTemplateInfo.DirectoryName, @"\"); // Needs Normal Path here

            string cspPath = Path.Combine(projectPath.DirectoryPath, CspFileName);

            QuickStartHelper.FindAndReplace(cspPath, @"\$connectionString\$", ProjectBuilder.SourceDatabase.ConnectionString);
            QuickStartHelper.FindAndReplace(cspPath, @"\$myDatabase\$", ProjectBuilder.SourceDatabase.Database.Name);
            QuickStartHelper.FindAndReplace(cspPath, @"\$myContextNamespace\$", ProjectBuilder.DataProjectName);
            QuickStartHelper.FindAndReplace(cspPath, @"\$language\$", ProjectBuilder.LanguageFolder);
            QuickStartHelper.FindAndReplace(cspPath, @"\$CSLA\$", CSLAPath);

            //The location in the csproj file directly below where we want to insert the csp include
            QuickStartHelper.FindAndReplace(projectPath.FilePath, ProjectInsertRegex,
                String.Format("<ItemGroup>{0}\t\t<Generate Include=\"{1}\" />{0}\t</ItemGroup>{0}\t{2}",
                    Environment.NewLine, CspFileName, ProjectInsertLine));
            QuickStartHelper.FindAndReplace(projectPath.FilePath, "</Project>",
                String.Format("\t<Import Project=\"$(MSBuildExtensionsPath)\\CodeSmith\\CodeSmith.targets\" />{0}</Project>",
                    Environment.NewLine));

            if (ProjectBuilder.Language == LanguageEnum.VB)
            {
                // Remove RootNamespace
                QuickStartHelper.FindAndReplace(projectPath.FilePath, @"<RootNamespace>.*</RootNamespace>", "<RootNamespace></RootNamespace>");
                //Update Settings File
                QuickStartHelper.FindAndReplace(Path.Combine(projectPath.DirectoryPath, "My Project\\Settings.Designer.vb"),
                    @"Global\." + ProjectBuilder.SourceDatabase.Database.Name + @"\.Data\.My\.MySettings",
                    "Global.My.MySettings");

                string vbFrameworkReplace = String.Format(@"<TargetFrameworkVersion>v{0}</TargetFrameworkVersion>
				<OptionExplicit>On</OptionExplicit>
				<OptionCompare>Binary</OptionCompare>
				<OptionStrict>Off</OptionStrict>
				<OptionInfer>On</OptionInfer>", "3.5");

                QuickStartHelper.FindAndReplace(projectPath.FilePath,
                    @"<TargetFrameworkVersion>v3.5</TargetFrameworkVersion>", vbFrameworkReplace);
            }

            QuickStartHelper.FindAndReplace(projectPath.FilePath, "<ItemGroup><ProjectReference /></ItemGroup>", BuildProjectReference());
        }

        private string BuildProjectReference()
        {
            string projectReferneces = string.Empty;

            foreach (string reference in ProjectReferences)
            {
                if(ProjectBuilder.Language == LanguageEnum.CSharp)
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
