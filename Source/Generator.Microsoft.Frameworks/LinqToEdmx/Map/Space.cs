using System.Xml.Schema;
using Xml.Schema.Linq;

namespace LinqToEdmx.Map
{
  public static class Space
  {
    public static SimpleTypeValidator TypeDefinition = new AtomicSimpleTypeValidator(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Token), new RestrictionFacets(((RestrictionFlags) (16)), new object[]
                                                                                                                                                                                               {
                                                                                                                                                                                                 "C-S"
                                                                                                                                                                                               }, 0, 0, null, null, 0, null, null, 0, null, 0, XmlSchemaWhiteSpace.Collapse));
  }
}