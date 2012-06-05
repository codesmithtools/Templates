using System.Xml.Schema;
using Xml.Schema.Linq;

namespace LinqToEdmx.Model.Storage
{
  public static class UndottedIdentifier
  {
    public static SimpleTypeValidator TypeDefinition = new AtomicSimpleTypeValidator(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String), new RestrictionFacets(((RestrictionFlags) (8)), null, 0, 0, null, null, 0, null, null, 0, new[]
                                                                                                                                                                                                                                         {
                                                                                                                                                                                                                                           "[^.]{1,}"
                                                                                                                                                                                                                                         }, 0, XmlSchemaWhiteSpace.Preserve));
  }
}