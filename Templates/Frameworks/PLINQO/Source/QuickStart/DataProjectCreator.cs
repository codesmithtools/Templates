using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CodeSmith.Engine;

namespace QuickStartUtils
{
    public class DataProjectCreator : VsProjectCreator
    {
        private string cspFileName;

        public DataProjectCreator(ProjectBuilderSettings projectBuilder)
            : base(projectBuilder) { }

        protected override void GetFiles(PathHelper projectPath)
        {
            GetVsProject(projectPath, "ClassLibrary");

            // Move/Rename CSP
            cspFileName = String.Format("{0}.csp", Path.GetFileNameWithoutExtension(projectPath.FileName));
            string templateCspFile = ProjectBuilder.QueryPattern == QueryPatternEnum.ManagerClasses ? @"Common\QuickStartData.csp" : @"Common\QuickStartQuery.csp";
            File.Copy(Path.Combine(ProjectBuilder.CodeTemplate.CodeTemplateInfo.DirectoryName, templateCspFile),
                Path.Combine(projectPath.DirectoryPath, cspFileName));

            // Delete Class1
            string class1File = String.Concat("Class1.", ProjectBuilder.LanguageAppendage);
            string class1Path = QuickStartUtils.FindFileInDirectory(class1File, projectPath.DirectoryPath);
            if (!String.IsNullOrEmpty(class1Path) && File.Exists(class1Path))
            {
                // Delete File
                File.Delete(class1Path);

                // Remove From Proj
                // Yes, There is a space in the csproj file while there is not in the vbproj file
                string classLine = (ProjectBuilder.Language == LanguageEnum.CSharp)
                    ? String.Format(@"<Compile Include=""{0}"" />", class1File)
                    : String.Format(@"<Compile Include=""{0}""/>", class1File);

                QuickStartUtils.FindAndReplace(projectPath.FilePath, classLine, String.Empty);
            }
        }
        protected override void ReplaceVariables(PathHelper projectPath, string projectGuid, string projectName)
        {
            VariableUpdateDirectory(projectPath, projectGuid, projectName, this.ProjectBuilder.LanguageAppendage, null);

            string linqToSqlPath = (ProjectBuilder.CopyTemplatesToFolder)
                ? @"..\Templates\LinqToSql\"
                : string.Concat(ProjectBuilder.CodeTemplate.CodeTemplateInfo.DirectoryName); // Needs Normal Path here
            
            if (!linqToSqlPath.EndsWith(@"\"))
                linqToSqlPath += @"\";

            string cspPath = Path.Combine(projectPath.DirectoryPath, cspFileName);

            QuickStartUtils.FindAndReplace(cspPath, @"\$connectionString\$", ProjectBuilder.SourceDatabase.ConnectionString);
            QuickStartUtils.FindAndReplace(cspPath, @"\$myDatabase\$", StringUtil.ToPascalCase(ProjectBuilder.SourceDatabase.Database.Name));
            QuickStartUtils.FindAndReplace(cspPath, @"\$myContextNamespace\$", ProjectBuilder.DataProjectName);
            QuickStartUtils.FindAndReplace(cspPath, @"\$language\$", ProjectBuilder.LanguageFolder);
            QuickStartUtils.FindAndReplace(cspPath, @"\$linqToSql\$", linqToSqlPath);
            QuickStartUtils.FindAndReplace(cspPath, @"\$languageExtension\$", ProjectBuilder.LanguageAppendage);

            //The location in the csproj file directly below where we want to insert the csp include
            QuickStartUtils.FindAndReplace(projectPath.FilePath, ProjectInsertRegex,
                String.Format("<ItemGroup>{0}\t\t<Generate Include=\"{1}.csp\" />{0}\t</ItemGroup>{0}\t{2}",
                    Environment.NewLine, ProjectBuilder.DataProjectName, ProjectInsertLine));
            QuickStartUtils.FindAndReplace(projectPath.FilePath, "</Project>",
                String.Format("\t<Import Project=\"$(MSBuildExtensionsPath)\\CodeSmith\\CodeSmith.targets\" />{0}</Project>",
                    Environment.NewLine));

            if (ProjectBuilder.Language == LanguageEnum.VB)
            {
                // Remove RootNamespace
                QuickStartUtils.FindAndReplace(projectPath.FilePath, @"<RootNamespace>.*</RootNamespace>", "<RootNamespace></RootNamespace>");
                //Update Settings File
                QuickStartUtils.FindAndReplace(Path.Combine(projectPath.DirectoryPath, "My Project\\Settings.Designer.vb"),
                    @"Global\." + ProjectBuilder.SourceDatabase.Database.Name + @"\.Data\.My\.MySettings",
                    "Global.My.MySettings");

                string vbFrameworkReplace = String.Format(@"<TargetFrameworkVersion>v{0}</TargetFrameworkVersion>
				<OptionExplicit>On</OptionExplicit>
				<OptionCompare>Binary</OptionCompare>
				<OptionStrict>Off</OptionStrict>
				<OptionInfer>On</OptionInfer>", "3.5");

                QuickStartUtils.FindAndReplace(projectPath.FilePath,
                    @"<TargetFrameworkVersion>v3.5</TargetFrameworkVersion>", vbFrameworkReplace);
            }
        }
    }
}
