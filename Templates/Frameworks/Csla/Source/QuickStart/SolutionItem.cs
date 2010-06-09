using System;
using System.Collections.Generic;

using CodeSmith.SchemaHelper;
namespace QuickStart
{
    public class SolutionItem
    {
        public SolutionItem(string name, string path, LanguageEnum language)
            : this(name, path, language, false, null)
        {

        }

        public SolutionItem(string name, string path, LanguageEnum language, bool website, IEnumerable<SolutionItem> projectReferences)
        {
            Guid = Guid.NewGuid();
            Name = name;
            Path = path;
            Language = language;
            Website = website;
            ProjectReferences = projectReferences != null 
                ? new List<SolutionItem>(projectReferences) 
                : new List<SolutionItem>();
        }

        public List<SolutionItem> ProjectReferences { get; set; }
        public bool Website { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }

        public Guid Guid { get; set; }

        public string GuidString
        {
            get { return Guid.ToString().ToUpper(); }
        }

        public LanguageEnum Language { get; set; }
        public string LanguageGuidString
        {
            get
            {
                return (Language == LanguageEnum.CSharp)
                    ? "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC"
                    : "F184B08F-C81C-45F6-A57F-5ABD9991F28F";
            }
        }

    }
}
