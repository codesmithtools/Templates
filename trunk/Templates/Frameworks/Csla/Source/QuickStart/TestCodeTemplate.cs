using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

using CodeSmith.CustomProperties;
using CodeSmith.Engine;
using CodeSmith.SchemaHelper;

using SchemaExplorer;

using Configuration=CodeSmith.SchemaHelper.Configuration;

namespace QuickStart
{
    public class TestCodeTemplate : EntityCodeTemplate
    {
        #region 6. Test Project
        
        [Category("6. Test Project")]
        [Description("The namespace for the test project.")]
        public string TestProjectName { get; set; }
				
        #endregion

        public override void RegisterReferences()
        {
            base.RegisterReferences();

            RegisterReference("System.Transactions");
            RegisterReference(Path.GetFullPath(Path.Combine(CodeTemplateInfo.DirectoryName, @"..\..\Common\nunit.framework.dll")));
        }
    }
}
