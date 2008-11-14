using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using CodeSmith.Engine;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using System.Text.RegularExpressions;

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

            // Parse Source
            CodeFileParser sourceParser = new CodeFileParser(sourceContent, this.Language.Value);
            sourceParser.ParseMethodBodies = false;
            sourceParser.Parse();
            AttributeSectionVisitor sourceVisitor = new AttributeSectionVisitor();
            sourceParser.CompilationUnit.AcceptVisitor(sourceVisitor, this.ClassName);

            // Check OnlyInsertMatchingClass
            if (OnlyInsertMatchingClass)
            {
                // Parse Template
                CodeFileParser templateParser = new CodeFileParser(templateOutput, this.Language.Value);
                templateParser.ParseMethodBodies = false;
                templateParser.Parse();
                AttributeSectionVisitor templateVisitor = new AttributeSectionVisitor();
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

            // Add Pre-Class Text To Output
            mergeResult.Append(sourceParser.GetSectionFromStart(sourceStart));

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

        #endregion
    }
}
