using System;
using System.CodeDom;
using System.IO;
using System.Text;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Visitors;

namespace CodeSmith.Engine
{
    /// <summary>
    /// Parser for a single Code File. Supports only C# and VB.
    /// CompilationUnit loaded on intialization, CodeDomCompilationUnit lazy loaded.
    /// </summary>
    [PropertySerializer(typeof(CodeFileParserSerializer))]
    [System.ComponentModel.Editor(typeof(CodeFileParserPicker), typeof(System.Drawing.Design.UITypeEditor))]
    public class CodeFileParser : IDisposable
    {
        #region Declarations

        private bool _disposed = false;
        private IParser _parser;
        private CodeCompileUnit _codeCompileUnit = null;

        #endregion

        #region Constructors & Destructors

        public CodeFileParser(string fileName)
            : this(String.Empty, null, fileName, false) { }

        public CodeFileParser(string fileName, bool parseMethodBodies)
            : this(String.Empty, null, fileName, parseMethodBodies) { }

        public CodeFileParser(string source, SupportedLanguage language)
            : this(source, language, String.Empty, false) { }

        public CodeFileParser(string source, SupportedLanguage language, bool parseMethodBodies)
            : this(source, language, String.Empty, parseMethodBodies) { }

        protected CodeFileParser(string source, SupportedLanguage? language, string fileName, bool parseMethodBodies)
        {
            // Get Source
            if (String.IsNullOrEmpty(source))
            {
                // Find File
                if (!File.Exists(fileName))
                    throw new FileNotFoundException("Could not find file.", fileName);

                // Set Language
                string ext = Path.GetExtension(fileName);
                if (ext.Equals(".cs", StringComparison.InvariantCultureIgnoreCase))
                    language = SupportedLanguage.CSharp;
                else if (ext.Equals(".vb", StringComparison.InvariantCultureIgnoreCase))
                    language = SupportedLanguage.VBNet;
                else
                    throw new Exception("Parser only supports C# or VB.");

                // Read In Source
                source = File.ReadAllText(fileName);
            }

            // Set Local Properties
            this.FileName = fileName;
            this.Language = language.Value;
            this.SourceContents = source.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            // Create and Run Parser
            this._parser = ParserFactory.CreateParser(language.Value, new StringReader(source));
            this._parser.ParseMethodBodies = parseMethodBodies;
            this._parser.Parse();
        }

        ~CodeFileParser()
        {
            Dispose(true);
        }

        #endregion

        #region IParser Members

        public CompilationUnit CompilationUnit
        {
            get { return _parser.CompilationUnit; }
        }
        public Errors Errors
        {
            get { return _parser.Errors; }
        }
        public ILexer Lexer
        {
            get { return _parser.Lexer; }
        }
        public bool ParseMethodBodies
        {
            get { return _parser.ParseMethodBodies; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(false);
        }
        private void Dispose(bool finalizing)
        {
            if (!_disposed)
            {
                if (_parser != null)
                    _parser.Dispose();

                _disposed = true;
                if (!finalizing)
                    GC.SuppressFinalize(this);
            }
        }

        #endregion

        #region String/Location Members

        public string GetSectionFromStart(Location end)
        {
            return GetSection(new Location(1, 1), end);
        }
        public string GetSectionToEnd(Location start)
        {
            return GetSection(start, SourceEnd);
        }
        public string GetSection(Location start, Location end)
        {
            StringBuilder sb = new StringBuilder();

            if (start.Line == end.Line)
            {
                sb.Append(SourceContents[start.Line - 1].Substring(start.Column - 1, end.Column - start.Column));
            }
            else
            {
                sb.AppendLine(SourceContents[start.Line - 1].Substring(start.Column - 1));

                for (int x = start.Line; x < end.Line - 1; x++)
                    sb.AppendLine(SourceContents[x]);

                if (!string.IsNullOrEmpty(SourceContents[end.Line - 1]))
                    sb.Append(SourceContents[end.Line - 1].Substring(0, end.Column - 1));
            }

            return sb.ToString();
        }

        public Location SourceEnd
        {
            get { return new Location(SourceContents[SourceContents.Length - 1].Length + 1, SourceContents.Length); }
        }

        #endregion

        #region Override Methods

        public override string ToString()
        {
            return this.FileName ?? "File Not Specified";
        }

        #endregion

        #region Properties

        public string[] SourceContents { get; protected set; }
        public string FileName { get; protected set; }
        public SupportedLanguage Language { get; protected set; }

        public CodeCompileUnit CodeDomCompilationUnit
        {
            get
            {
                if (_codeCompileUnit == null)
                {
                    CodeDomVisitor codeDomVisitor = new CodeDomVisitor();
                    this.CompilationUnit.AcceptVisitor(codeDomVisitor, null);
                    _codeCompileUnit = codeDomVisitor.codeCompileUnit;
                }
                return _codeCompileUnit;
            }
        }

        #endregion
    }
}
