using System.Xml.Schema;
using Xml.Schema.Linq;

namespace LinqToEdmx.Model.Storage
{
  public static class MaxLengthFacet
  {
    public static SimpleTypeValidator TypeDefinition = new UnionSimpleTypeValidator(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.AnyAtomicType), null, new[]
                                                                                                                                                           {
                                                                                                                                                             Max.TypeDefinition, new AtomicSimpleTypeValidator(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.NonNegativeInteger), null)
                                                                                                                                                           });
  }
}