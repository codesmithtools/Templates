using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using CodeSmith.Engine;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;

namespace InsertClassMerge
{
    public class InsertClassMerge : IMergeStrategy
    {
        #region Constructors

        public InsertClassMerge() { }
        public InsertClassMerge(string language, string className)
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
                this.Language = (string)attribs["Language"];
            if (attribs.ContainsKey("ClassName"))
                this.Language = (string)attribs["ClassName"];
        }
        public string Merge(CodeTemplate context, string sourceContent, string templateOutput)
        {
            // Exit if no source to merge with.
            if (String.IsNullOrEmpty(sourceContent))
                return templateOutput;

            // Check Merge Settings
            if (String.IsNullOrEmpty(this.ClassName))
                throw new Exception("No ClassName specified for InsertClassMerge.");
            if (String.IsNullOrEmpty(Language) || Language.Trim().Length == 0)
                Language = context.CodeTemplateInfo.TargetLanguage;

            // Parse Source
            CodeSmithParser sourceParser = CodeSmithParser.Create(sourceContent, this.supportedLanguage);
            sourceParser.ParseMethodBodies = false;
            sourceParser.Parse();
            AttributeSectionVisitor sourceVisitor = new AttributeSectionVisitor();
            sourceParser.CompilationUnit.AcceptVisitor(sourceVisitor, this.ClassName);

            // Parse Template
            CodeSmithParser templateParser = CodeSmithParser.Create(templateOutput, this.supportedLanguage);
            templateParser.ParseMethodBodies = false;
            templateParser.Parse();
            AttributeSectionVisitor templateVisitor = new AttributeSectionVisitor();
            templateParser.CompilationUnit.AcceptVisitor(templateVisitor, this.ClassName);

            // Exit it classes to merge are not found.
            if (sourceVisitor.Type == null || templateVisitor.Type == null)
                return sourceContent;

            // Setup For Merge
            StringBuilder mergeResult = new StringBuilder();

            // Add Pre-Class Text To Output
            mergeResult.Append(sourceParser.GetSectionFromStart(CorrectedTypeStart(sourceVisitor.Type)));

            // Add Merged Class to Output
            mergeResult.Append(templateParser.GetSection(
                CorrectedTypeStart(templateVisitor.Type),
                templateVisitor.Type.EndLocation));

            // Add Post-Class Text To Output
            mergeResult.Append(sourceParser.GetSectionToEnd(sourceVisitor.Type.EndLocation));

            // Return MergeResult
            return mergeResult.ToString();
        }

        #endregion

        #region Methods

        protected Location CorrectedTypeStart(TypeDeclaration typeDeclaration)
        {
            return (typeDeclaration.Attributes.Count > 0)
                ? typeDeclaration.Attributes.First().StartLocation
                : typeDeclaration.StartLocation;
        }

        #endregion

        #region Properties

        public string ClassName { get; set; }
        public string Language { get; set; }

        protected SupportedLanguage supportedLanguage
        {
            get
            {
                if (String.Compare(Language, "C#", true) == 0)
                    return SupportedLanguage.CSharp;
                else if (String.Compare(Language, "VB", true) == 0)
                    return SupportedLanguage.VBNet;
                else
                    throw new Exception(String.Format("{0} is not a supported language for the ClassMerge.", Language));
            }
        }

        #endregion
    }
}
