using System.Xml.Schema;
using Xml.Schema.Linq;

namespace LinqToEdmx.Model.Conceptual
{
  public static class EdmSimpleType
  {
    public static SimpleTypeValidator TypeDefinition = new AtomicSimpleTypeValidator(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String), new RestrictionFacets(((RestrictionFlags) (16)), new object[] {"Binary", "Boolean", "Byte", "DateTime", "DateTimeOffset", "Time", "Decimal", "Double", "Single", "Guid", "Int16", "Int32", "Int64", "String", "SByte"}, 0, 0, null, null, 0, null, null, 0, null, 0, XmlSchemaWhiteSpace.Preserve));
  }
}