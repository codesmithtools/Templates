using System.Xml.Schema;
using Xml.Schema.Linq;

namespace LinqToEdmx.Model.Conceptual
{
  public static class UnwrappedFunctionType
  {
    public static SimpleTypeValidator TypeDefinition = new UnionSimpleTypeValidator(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.AnyAtomicType), null, new[] {QualifiedName.TypeDefinition, new AtomicSimpleTypeValidator(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Token), new RestrictionFacets(((RestrictionFlags) (8)), null, 0, 0, null, null, 0, null, null, 0, new[] {"[^ \\t]{1,}(\\.[^ \\t]{1,}){0,}"}, 0, XmlSchemaWhiteSpace.Collapse))});
  }
}