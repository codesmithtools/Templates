using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using CodeSmith.Engine;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using System.Text.RegularExpressions;
using System.CodeDom;

namespace CodeSmith.Engine
{
    public class InsertClassMergeStrategy : IMergeStrategy
    {
        public enum NotFoundActionEnum
        {
            None,
            InsertAtBottom,
            InsertInParent
        }

        #region Constructors

        public InsertClassMergeStrategy() { }
        public InsertClassMergeStrategy(SupportedLanguage language, string className)
        {
            this.Language = language;
            this.ClassName = className;
        }

        #endregion

        #region IMergeStrategy Members

        public void Init(string init)
        {
            Hashtable attribs = StringUtil.ParseConfigString(init);

            if (attribs.ContainsKey("Language"))
                SetLanguage((string)attribs["Language"]);

            if (attribs.ContainsKey("ClassName"))
                this.ClassName = (string)attribs["ClassName"];

            if (attribs.ContainsKey("PreserveClassAttributes"))
                Boolean.TryParse((string)attribs["PreserveClassAttributes"], out _preserveClassAttributes);

            if (attribs.ContainsKey("OnlyInsertMatchingClass"))
                Boolean.TryParse((string)attribs["OnlyInsertMatchingClass"], out _onlyInsertMatchingClass);

            if (attribs.ContainsKey("NotFoundAction"))
                this.NotFoundAction = (NotFoundActionEnum)Enum.Parse(typeof(NotFoundActionEnum), (string)attribs["NotFoundAction"]);

            if (attribs.ContainsKey("NotFoundParent"))
                this.NotFoundParent = (string)attribs["NotFoundParent"];

            if (attribs.ContainsKey("MergeImports"))
                this.MergeImports = Boolean.TryParse((string)attribs["MergeImports"], out _mergeImports);
        }
        public string Merge(CodeTemplate context, string sourceContent, string templateOutput)
        {
            // Exit if no source to merge with.
            if (String.IsNullOrEmpty(sourceContent))
                return templateOutput;

            // Check Merge Settings
            if (String.IsNullOrEmpty(this.ClassName))
                throw new Exception("No ClassName specified for InsertClassMerge.");
            if (!this.Language.HasValue && !SetLanguage(context.CodeTemplateInfo.TargetLanguage))
                throw new Exception("InsertClassMergeStrategy only supports Languages 'C#' and 'VB'");

            CodeFileParser templateParser = null;
            AttributeSectionVisitor templateVisitor = null;
                
            // Parse Source
            CodeFileParser sourceParser = new CodeFileParser(sourceContent, this.Language.Value, false);
            AttributeSectionVisitor sourceVisitor = new AttributeSectionVisitor();
            sourceParser.CompilationUnit.AcceptVisitor(sourceVisitor, this.ClassName);

            if (MergeImports)
            {
                MergeImportString = templateOutput;
            }

            // Check OnlyInsertMatchingClass
            if (OnlyInsertMatchingClass)
            {
                // Parse Template
                templateParser = new CodeFileParser(templateOutput, this.Language.Value, false);
                templateVisitor = new AttributeSectionVisitor();
                templateParser.CompilationUnit.AcceptVisitor(templateVisitor, this.ClassName);

                // If no class found (nothing to merge), return the source.
                if (templateVisitor.Type == null)
                    return sourceContent;

                // Update TemplateOutput (Trim everything but the existing class!)
                templateOutput = templateParser.GetSection(
                    GetCorrectedTypeStart(templateVisitor.Type),
                    GetCorrectedTypeEnd(templateVisitor.Type));
            }

            // Setup Variables For Merge
            StringBuilder mergeResult = new StringBuilder();
            Location sourceStart;
            Location sourceEnd;

            // Set Source Start & End Locations
            if (sourceVisitor.Type == null)
            {
                switch (NotFoundAction)
                {
                    case NotFoundActionEnum.InsertAtBottom:
                        // Set templateStart & templateEnd as bottom.
                        sourceStart = sourceParser.SourceEnd;
                        sourceEnd = sourceParser.SourceEnd;
                        break;

                    case NotFoundActionEnum.InsertInParent:
                        // Insert class before end of parent section.
                        sourceStart = GetBeforeEnd(sourceParser);
                        sourceEnd = sourceStart;
                        // Try to fix up our output...
                        Regex findWhiteSpace = new Regex(@"^(\s)*");
                        string whiteSpace = findWhiteSpace.Match(sourceParser.SourceContents[sourceStart.Line - 1]).Value;
                        templateOutput = String.Format("\t{0}{1}{2}", templateOutput, Environment.NewLine, whiteSpace);
                        break;

                    case NotFoundActionEnum.None:
                    default:
                        // Nothing to merge, return the source.
                        return sourceContent;
                }
            }
            else
            {
                // Class exists, set it's start and end.
                sourceStart = GetCorrectedTypeStart(sourceVisitor.Type);
                sourceEnd = GetCorrectedTypeEnd(sourceVisitor.Type);
            }

         
            //Add Imports To Output
            if (MergeImports)
            {
                // Parse Template
                templateParser = new CodeFileParser(MergeImportString, this.Language.Value, false);
                templateVisitor = new AttributeSectionVisitor();
                templateParser.CompilationUnit.AcceptVisitor(templateVisitor, this.ClassName);
            
                UsingDeclaration firstUsing  = sourceVisitor.UsingList.Where<UsingDeclaration>(u => u.StartLocation.Line < sourceStart.Line).FirstOrDefault<UsingDeclaration>();
                ImportsStop = AppendMergedImports(mergeResult, firstUsing, sourceParser,sourceVisitor, templateVisitor);

                // Add Pre-Class Text To Output
                mergeResult.Append(sourceParser.GetSection(ImportsStop, sourceStart));
            }
            else
            {
                // Add Pre-Class Text To Output
                mergeResult.Append(sourceParser.GetSectionFromStart(sourceStart));
            }

            // Add Merged Class to Output
            mergeResult.Append(templateOutput);

            // Add Post-Class Text To Output
            mergeResult.Append(sourceParser.GetSectionToEnd(sourceEnd));

            // Return MergeResult
            return mergeResult.ToString();
        }

        #endregion

        #region Methods

        protected Location GetCorrectedTypeStart(TypeDeclaration type)
        {
            return (!PreserveClassAttributes && type.Attributes.Count > 0)
                ? type.Attributes.First().StartLocation
                : type.StartLocation;
        }
        protected Location GetCorrectedTypeEnd(TypeDeclaration type)
        {
            Location result = new Location(type.EndLocation.Column, type.EndLocation.Line);
            if (this.Language.Value == SupportedLanguage.CSharp)
                result.Column++;
            return result;
        }

        protected Location GetBeforeEnd(CodeFileParser parser)
        {
            ParentVisitor parentVisitor = new ParentVisitor();
            parser.CompilationUnit.AcceptVisitor(parentVisitor, NotFoundParent);

            if(parentVisitor.Node == null)
                throw new Exception(String.Format("Can not find Namespace or Class with name {0}.", NotFoundParent));

            if (parentVisitor.Node is NamespaceDeclaration)
            {
                return (Language.Value == SupportedLanguage.CSharp)
                    ? new Location(parentVisitor.Node.EndLocation.Column - 1, parentVisitor.Node.EndLocation.Line)  // }
                    : new Location(parentVisitor.Node.EndLocation.Column - 13, parentVisitor.Node.EndLocation.Line);// End Namespace
            }
            else if (parentVisitor.Node is TypeDeclaration)
            {
                return (Language.Value == SupportedLanguage.CSharp)
                    ? new Location(parentVisitor.Node.EndLocation.Column - 0, parentVisitor.Node.EndLocation.Line)  // BUG: The C# Type Delcaration ends early?
                    : new Location(parentVisitor.Node.EndLocation.Column - 9, parentVisitor.Node.EndLocation.Line); // End Class
            }
            else
                throw new Exception(String.Format("ParentVisitor should not return {0}", parentVisitor.Node.GetType().Name));
        }

        protected bool SetLanguage(string language)
        {
            if (String.Compare(language, "C#", true) == 0 || String.Compare(language, "CSharp", true) == 0)
                Language = SupportedLanguage.CSharp;
            else if (String.Compare(language, "VB", true) == 0 || String.Compare(language, "VBNet", true) == 0)
                Language = SupportedLanguage.VBNet;
            else
                return false;

            return true;
        }

        protected Location AppendMergedImports(StringBuilder mergeResult, UsingDeclaration sourceUsing, CodeFileParser sourceParser,
            AttributeSectionVisitor sourceVisitor, AttributeSectionVisitor templateVisitor)
        {
            string importPrefix = "using ";
            string importSuffix = ";";

            if (sourceParser.Language == SupportedLanguage.VBNet)
            {
                importPrefix = "Imports ";
                importSuffix = String.Empty;
            }

            Location start = new Location(1,sourceUsing.StartLocation.Line);
            Location stop = new Location(1,sourceUsing.EndLocation.Line);

            // Add Pre-Import Text To Output
            mergeResult.Append(sourceParser.GetSectionFromStart(start));

            //Add Pre-Imports Text to Output
            mergeResult.Append(sourceParser.GetSection(start,stop));

            List<string> completeUsingList = new List<string>();
             List<string> newUsingList = new List<string>();

             foreach (Using @using in sourceUsing.Usings)
             {
                 newUsingList.Add(@using.Name);
             }

             foreach (UsingDeclaration usingDeclaration in sourceVisitor.UsingList)
                 foreach (Using @using in usingDeclaration.Usings)
                         completeUsingList.Add(@using.Name);


            foreach (UsingDeclaration usingDeclaration in templateVisitor.UsingList)
                foreach (Using @using in usingDeclaration.Usings)
                    if(!completeUsingList.Contains(@using.Name))
                        newUsingList.Add(@using.Name);

            foreach (string @using in newUsingList)
                mergeResult.AppendLine(string.Concat(importPrefix, @using, importSuffix));


            return new Location(1,stop.Line + 1);

        }

        #endregion

        #region Properties

        public string ClassName { get; set; }
        public string NotFoundParent { get; set; }

        private bool _onlyInsertMatchingClass = false;
        public bool OnlyInsertMatchingClass
        {
            get { return _onlyInsertMatchingClass; }
            set { _onlyInsertMatchingClass = value; }
        }

        private bool _preserveClassAttributes = false;
        public bool PreserveClassAttributes
        {
            get { return _preserveClassAttributes; }
            set { _preserveClassAttributes = value; }
        }
        
        private bool _mergeImports = false;
        public bool MergeImports
        {
            get { return _mergeImports; }
            set { _mergeImports = value; }
        }

        private NotFoundActionEnum _notFoundAction = NotFoundActionEnum.None;
        public NotFoundActionEnum NotFoundAction
        {
            get { return _notFoundAction; }
            set { _notFoundAction = value; }
        }

        private SupportedLanguage? _language = null;
        public SupportedLanguage? Language
        {
            get { return _language; }
            set { _language = value; }
        }

        private List<string> _importsList = new List<string>();
        public List<string> ImportsList
        {
            get { return _importsList; }
        }

        private Location _importsStart;
        public Location ImportsStart 
        {
            get { return _importsStart; }
            set { _importsStart = value; }
        }

        private Location _importsStop;
        public Location ImportsStop
        {
            get { return _importsStop; }
            set { _importsStop = value; }
        }

        private string MergeImportString { get; set; }

        #endregion

    }
}
