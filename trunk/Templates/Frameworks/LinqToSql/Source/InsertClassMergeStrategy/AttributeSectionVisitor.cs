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
        #region Declarations

        private Dictionary<string, PropertyDeclaration> _propertyMap = new Dictionary<string, PropertyDeclaration>();

        #endregion

        #region AbstractAstVisitor Overrides

        public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
        {
            if (typeDeclaration.Name.Equals(data))
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

        #endregion

        #region Properties

        public TypeDeclaration Type { get; set; }
        public Dictionary<string, PropertyDeclaration> PropertyMap
        {
            get { return _propertyMap; }
            set { _propertyMap = value; }
        }

        #endregion
    }
}
