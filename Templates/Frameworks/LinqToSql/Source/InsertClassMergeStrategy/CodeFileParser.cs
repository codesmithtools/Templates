using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Parser;

namespace CodeSmith.Engine
{
    [System.ComponentModel.Editor(typeof(CodeFileParserPicker), typeof(System.Drawing.Design.UITypeEditor))]
    public class CodeFileParser : IParser
    {
        #region Static Content

        public static CodeFileParser Create(string fileName)
        {
            if (!System.IO.File.Exists(fileName))
                throw new FileNotFoundException("Could not find file.", fileName);

            SupportedLanguage language;
            string ext = Path.GetExtension(fileName);
            if (ext.Equals(".cs", StringComparison.InvariantCultureIgnoreCase))
                language = SupportedLanguage.CSharp;
            else if (ext.Equals(".vb", StringComparison.InvariantCultureIgnoreCase))
                language = SupportedLanguage.VBNet;
            else
                throw new Exception("Parser only supports C# or VB.");

            string source = System.IO.File.ReadAllText(fileName);

            return new CodeFileParser(source, language, fileName);
        }
        public static CodeFileParser Create(string source, SupportedLanguage language)
        {
            return new CodeFileParser(source, language, String.Empty);
        }

        #endregion

        #region Declarations

        private IParser _parser;

        #endregion

        #region Constructor

        protected CodeFileParser(string source, SupportedLanguage language, string fileName)
        {
            FileName = fileName;
            Language = language;
            SourceContents = source.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            _parser = ParserFactory.CreateParser(language, new StringReader(source));
        }

        #endregion

        #region IParser Members

        public ICSharpCode.NRefactory.Ast.CompilationUnit CompilationUnit
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

        public void Parse()
        {
            _parser.Parse();
        }
        public ICSharpCode.NRefactory.Ast.BlockStatement ParseBlock()
        {
            return _parser.ParseBlock();
        }
        public ICSharpCode.NRefactory.Ast.Expression ParseExpression()
        {
            return _parser.ParseExpression(); ;
        }

        public bool ParseMethodBodies
        {
            get { return _parser.ParseMethodBodies; }
            set { _parser.ParseMethodBodies = value; }
        }

        public System.Collections.Generic.List<ICSharpCode.NRefactory.Ast.INode> ParseTypeMembers()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _parser.Dispose();
            SourceContents = null;
        }

        #endregion

        #region Methods

        public Location SourceEnd
        {
            get { return new Location(SourceContents[SourceContents.Length - 1].Length + 1, SourceContents.Length); }
        }

        public string GetSectionFromStart(Location end)
        {
            return GetSection(
                new Location(1, 1),
                end);

        }
        public string GetSectionToEnd(Location start)
        {
            return GetSection(
                start,
                SourceEnd);
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

        #endregion

        #region Properties

        public string[] SourceContents { get; protected set; }
        public string FileName { get; protected set; }
        public SupportedLanguage Language { get; protected set; }

        #endregion
    }
}
