using System.Xml.Schema;
using Xml.Schema.Linq;

namespace LinqToEdmx.SchemaGen
{
  public static class SourceType
  {
    public static SimpleTypeValidator TypeDefinition = new AtomicSimpleTypeValidator(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String), new RestrictionFacets(((RestrictionFlags) (16)), new object[]
                                                                                                                                                                                                {
                                                                                                                                                                                                  "Tables", "Views"
                                                                                                                                                                                                }, 0, 0, null, null, 0, null, null, 0, null, 0, XmlSchemaWhiteSpace.Preserve));
  }
}