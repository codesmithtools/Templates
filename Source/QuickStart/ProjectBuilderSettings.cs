using System;
using System.IO;
using CodeSmith.Engine;
using CodeSmith.SchemaHelper;

using SchemaExplorer;

namespace QuickStart
{
    public class ProjectBuilderSettings
    {
        public ProjectBuilderSettings()
        {
        }
        public DatabaseSchema SourceDatabase { get; set; }
        public string Location { get; set; }
        public string WorkingDirectory { get; set; }

        public string SolutionName { get; set; }
        public string BusinessProjectName { get; set; }
        public string DataProjectName { get; set; }
        public string InterfaceProjectName { get; set; }
        public string TestProjectName { get; set; }

        public LanguageEnum Language { get; set; }
        public ProjectTypeEnum ProjectType { get; set; }

        public bool IncludeDataProject { get; set; }
        public bool IncludeTestProject { get; set; }
        public bool CopyTemplatesToFolder { get; set; }
        
        public string WebSkin { get; set; }
        public FrameworkVersion FrameworkVersion { get; set; }

        public string FrameworkFolder
        {
            get { return (FrameworkVersion == FrameworkVersion.v35) ? "v3.5" : "v4.0"; }
        }

        public string FrameworkString
        {
            get { return (FrameworkVersion == FrameworkVersion.v35) ? "3.5" : "4.0"; }
        }

        public string LanguageFolder
        {
            get { return (Language == LanguageEnum.CSharp) ? "CSharp" : "VisualBasic"; }
        }

        public string LanguageAppendage
        {
            get { return (Language == LanguageEnum.CSharp) ? "cs" : "vb"; }
        }

        private string _zipFileFolder;

        public string ZipFileFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_zipFileFolder))
                {
                    _zipFileFolder = Path.Combine(WorkingDirectory, "Common");
                    _zipFileFolder = Path.Combine(_zipFileFolder, FrameworkFolder);
                    _zipFileFolder = Path.Combine(_zipFileFolder, LanguageFolder);
                }

                return _zipFileFolder;
            }
        }

        private string _databaseName;

        public string DatabaseName
        {
            get
            {
                if (string.IsNullOrEmpty(_databaseName))
                    _databaseName = StringUtil.ToPascalCase(SourceDatabase.Database.Name);

                return _databaseName;
            }
        }
    }
}
