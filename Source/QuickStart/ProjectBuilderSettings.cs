using System;
using System.IO;
using CodeSmith.Engine;
using CodeSmith.SchemaHelper;

using SchemaExplorer;

namespace QuickStart
{
    public class ProjectBuilderSettings
    {
        public DatabaseSchema SourceDatabase { get; set; }
        public String Location { get; set; }
        public String SolutionName { get; set; }
        public LanguageEnum Language { get; set; }
        public ProjectTypeEnum ProjectType { get; set; }
        public bool IncludeDataServices { get; set; }
        public String DataProjectName { get; set; }
        public bool CopyTemplatesToFolder { get; set; }
        public String InterfaceProjectName { get; set; }
        public String TestProjectName { get; set; }
        public bool IncludeTestProject { get; set; }
        public CodeTemplate CodeTemplate { get; set; }
        public string WebSkin { get; set; }

        public string LanguageFolder
        {
            get { return (this.Language == LanguageEnum.CSharp) ? "CSharp" : "VisualBasic"; }
        }

        public string LanguageAppendage
        {
            get { return (this.Language == LanguageEnum.CSharp) ? "cs" : "vb"; }
        }

        public string FileName
        {
            get { return "WebApplicationProject"; }
        }

        public string ZipFileFolder
        {
            get { return Path.Combine(CodeTemplate.CodeTemplateInfo.DirectoryName, Path.Combine("Common", LanguageFolder)); }
        }
    }
}
