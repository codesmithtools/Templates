using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.Visitors;
using ICSharpCode.NRefactory.Ast;

namespace CodeSmith.Engine
{
    internal class ParentVisitor : AbstractAstVisitor
    {
        #region AbstractAstVisitor Overrides

        public override object VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
        {
            if (Node == null && namespaceDeclaration.Name.Equals(data))
            {
                Node = namespaceDeclaration;
            }

            return base.VisitNamespaceDeclaration(namespaceDeclaration, data);
        }
        public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
        {
            if (Node == null && typeDeclaration.Name.Equals(data))
            {
                Node = typeDeclaration;
            }

            return base.VisitTypeDeclaration(typeDeclaration, data);
        }

        #endregion

        public INode Node { get; private set; }
    }
}
