using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickStartUtils
{
    public class TestProjectCreator : ProjectCreator
    {
        public TestProjectCreator(ProjectBuilderSettings projectBuilder)
            : base(projectBuilder) { }

        public override string ProjectTemplateFile
        {
            get { return "TestProject.zip"; }
        }

        protected override void AddFiles()
        { }
    }
}
