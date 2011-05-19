using System.Xml.Linq;
using System.Xml.Schema;
using LinqToEdmx.Model.Storage;
using Xml.Schema.Linq;

namespace LinqToEdmx.Designer
{
  public class ConnectorPoint : XTypedElement, IXMetaData
  {
    /// <summary>
    /// <para>
    /// Occurrence: required
    /// </para>
    /// </summary>
    public double PointX
    {
      get
      {
        return XTypedServices.ParseValue<double>(Attribute(XName.Get("PointX", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Double).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("PointX", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Double).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required
    /// </para>
    /// </summary>
    public double PointY
    {
      get
      {
        return XTypedServices.ParseValue<double>(Attribute(XName.Get("PointY", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Double).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("PointY", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Double).Datatype);
      }
    }

    #region IXMetaData Members

    XName IXMetaData.SchemaName
    {
      get
      {
        return XName.Get("TConnectorPoint", "http://schemas.microsoft.com/ado/2008/10/edmx");
      }
    }

    SchemaOrigin IXMetaData.TypeOrigin
    {
      get
      {
        return SchemaOrigin.Fragment;
      }
    }

    ILinqToXsdTypeManager IXMetaData.TypeManager
    {
      get
      {
        return LinqToXsdTypeManager.Instance;
      }
    }

    ContentModelEntity IXMetaData.GetContentModel()
    {
      return ContentModelEntity.Default;
    }

    #endregion

    public static explicit operator ConnectorPoint(XElement xe)
    {
      return XTypedServices.ToXTypedElement<ConnectorPoint>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }
  }
}