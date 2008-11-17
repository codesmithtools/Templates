using System;
using System.Collections.Generic;
using System.Text;
using CodeSmith.Engine;
using ICSharpCode.NRefactory;
using NUnit.Framework;
using System.CodeDom;

namespace LinqToSqlShared.Generator.Tests
{
    [TestFixture]
    public class CodeFileParserTest
    {
        [Test]
        public void Blah()
        {
            CodeFileParser cfp = new CodeFileParser(TestCode, SupportedLanguage.CSharp);

            Assert.AreEqual(cfp.CodeDomCompilationUnit.Namespaces.Count, 2);
            Assert.AreEqual(cfp.CodeDomCompilationUnit.Namespaces[0].Imports.Count, 3);
            Assert.AreEqual(cfp.CodeDomCompilationUnit.Namespaces[0].Types.Count, 1);
            Assert.AreEqual(cfp.CodeDomCompilationUnit.Namespaces[0].Types[0].Members.Count, 1);
            Assert.AreEqual(cfp.CodeDomCompilationUnit.Namespaces[1].Name, "Global");
        }

        protected string TestCode
        {
            get
            {
                return @"
using System;
using System.Collections.Generic;
using System.Text;

namespace LinqToSqlShared.Generator.Tests
{
    public class CodeFileParserTest
    {
        public void Main()
        {
        }
    }
}";
            }
        }
    }
}
