using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;

namespace CodeSmith.Engine
{
    public class AttributeSectionVisitor : AbstractAstVisitor
    {
        #region AbstractAstVisitor Overrides

        public override object VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
        {
            if (Namespace == null)
            {
                Namespace = namespaceDeclaration;
            }

            return base.VisitNamespaceDeclaration(namespaceDeclaration, data);
        }
        public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
        {
            if (Type == null && typeDeclaration.Name.Equals(data))
            {
                Type = typeDeclaration;
            }

            return base.VisitTypeDeclaration(typeDeclaration, data);
        }
        public override object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
        {
            TypeDeclaration typeDeclaration = propertyDeclaration.Parent as TypeDeclaration;
            if (typeDeclaration == Type)
            {
                PropertyMap.Add(propertyDeclaration.Name, propertyDeclaration);
            }

            return base.VisitPropertyDeclaration(propertyDeclaration, data);
        }

        public override object VisitUsingDeclaration(UsingDeclaration usingDeclaration, object data)
        {
            TypeDeclaration typeDeclaration = usingDeclaration.Parent as TypeDeclaration;
            if (typeDeclaration == Type)
            {
                UsingList.Add(usingDeclaration);
            }

            return base.VisitUsingDeclaration(usingDeclaration, data);
        }

        #endregion

        #region Properties

        private NamespaceDeclaration _namespace = null;
        public NamespaceDeclaration Namespace
        {
            get { return _namespace; }
            private set { _namespace = value; }
        }

        private TypeDeclaration _type = null;
        public TypeDeclaration Type
        {
            get { return _type; }
            private set { _type = value; }
        }

        private Dictionary<string, PropertyDeclaration> _propertyMap = new Dictionary<string, PropertyDeclaration>();
        public Dictionary<string, PropertyDeclaration> PropertyMap
        {
            get { return _propertyMap; }
            private set { _propertyMap = value; }
        }

        private List<UsingDeclaration> _usingList = new List<UsingDeclaration>();
        public List<UsingDeclaration> UsingList 
        { 
            get { return _usingList; }
            set { _usingList = value; }
        }

        #endregion
    }
}
