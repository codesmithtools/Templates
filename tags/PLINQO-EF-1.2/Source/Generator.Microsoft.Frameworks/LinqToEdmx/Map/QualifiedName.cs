using System.Xml.Schema;
using Xml.Schema.Linq;

namespace LinqToEdmx.Map
{
  public static class QualifiedName
  {
    public static SimpleTypeValidator TypeDefinition = new AtomicSimpleTypeValidator(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Token), new RestrictionFacets(((RestrictionFlags) (8)), null, 0, 0, null, null, 0, null, null, 0, new[]
                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                          "[\\p{L}\\p{Nl}][\\p{L}\\p{Nl}\\p{Nd}\\p{Mn}\\p{Mc}\\p{Pc}\\p{Cf}]{0,}(\\.[\\p{L}\\p{Nl}][\\p{L" + "}\\p{Nl}\\p{Nd}\\p{Mn}\\p{Mc}\\p{Pc}\\p{Cf}]{0,}){0,}"
                                                                                                                                                                                                                                        }, 0, XmlSchemaWhiteSpace.Collapse));
  }
}