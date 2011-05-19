using System.Xml.Schema;
using Xml.Schema.Linq;

namespace LinqToEdmx.CodeGen
{
  public static class Access
  {
    public static SimpleTypeValidator TypeDefinition = new AtomicSimpleTypeValidator(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String), new RestrictionFacets(((RestrictionFlags) (16)), new object[]
                                                                                                                                                                                                {
                                                                                                                                                                                                  "Public", "Internal", "Protected", "Private"
                                                                                                                                                                                                }, 0, 0, null, null, 0, null, null, 0, null, 0, XmlSchemaWhiteSpace.Preserve));
  }
}