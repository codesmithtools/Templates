using System.Xml.Schema;
using Xml.Schema.Linq;

namespace LinqToEdmx.Model.Conceptual
{
  public static class IsUnicodeFacet
  {
    public static SimpleTypeValidator TypeDefinition = new AtomicSimpleTypeValidator(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean), null);
  }
}