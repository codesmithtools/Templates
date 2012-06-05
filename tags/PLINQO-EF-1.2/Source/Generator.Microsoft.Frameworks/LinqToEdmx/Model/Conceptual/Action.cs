using System.Xml.Schema;
using Xml.Schema.Linq;

namespace LinqToEdmx.Model.Conceptual
{
  public static class Action
  {
    public static SimpleTypeValidator TypeDefinition = new AtomicSimpleTypeValidator(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Token), new RestrictionFacets(((RestrictionFlags) (16)), new object[]
                                                                                                                                                                                               {
                                                                                                                                                                                                 "Cascade", "None"
                                                                                                                                                                                               }, 0, 0, null, null, 0, null, null, 0, null, 0, XmlSchemaWhiteSpace.Collapse));
  }
}