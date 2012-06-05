using System.Xml.Schema;
using Xml.Schema.Linq;

namespace LinqToEdmx.Model.Conceptual
{
  public static class QualifiedName
  {
    public static SimpleTypeValidator TypeDefinition = new AtomicSimpleTypeValidator(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String), new RestrictionFacets(((RestrictionFlags) (8)), null, 0, 0, null, null, 0, null, null, 0, new[] {"[\\p{L}\\p{Nl}][\\p{L}\\p{Nl}\\p{Nd}\\p{Mn}\\p{Mc}\\p{Pc}\\p{Cf}]{0,}(\\.[\\p{L}\\p{Nl}][\\p{L" + "}\\p{Nl}\\p{Nd}\\p{Mn}\\p{Mc}\\p{Pc}\\p{Cf}]{0,}){0,}"}, 0, XmlSchemaWhiteSpace.Preserve));
  }
}