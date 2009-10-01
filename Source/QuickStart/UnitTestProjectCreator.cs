using System;

namespace QuickStart
{
    public class UnitTestProjectCreator : VsProjectCreator
    {
        public UnitTestProjectCreator(ProjectBuilderSettings projectBuilder)
            : base(projectBuilder) { }

        protected override void GetFiles(PathHelper projectPath)
        {
            GetVsProject(projectPath, "TestProject");
        }
        protected override void ReplaceVariables(PathHelper projectPath, string projectGuid, string projectName)
        {
            VariableUpdateDirectory(projectPath, projectGuid, projectName, this.ProjectBuilder.LanguageAppendage, null);
        }
    }
}
