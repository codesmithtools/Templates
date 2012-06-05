using System.Xml.Linq;
using System.Xml.Schema;
using LinqToEdmx.Model.Storage;
using Xml.Schema.Linq;

namespace LinqToEdmx.Designer
{
  public class EntityTypeShape : XTypedElement, IXMetaData
  {
    /// <summary>
    /// <para>
    /// Occurrence: required
    /// </para>
    /// </summary>
    public string EntityType
    {
      get
      {
        return XTypedServices.ParseValue<string>(Attribute(XName.Get("EntityType", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("EntityType", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public double? PointX
    {
      get
      {
        if ((Attribute(XName.Get("PointX", "")) == null))
        {
          return null;
        }
        return XTypedServices.ParseValue<double>(Attribute(XName.Get("PointX", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Double).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("PointX", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Double).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public double? PointY
    {
      get
      {
        if ((Attribute(XName.Get("PointY", "")) == null))
        {
          return null;
        }
        return XTypedServices.ParseValue<double>(Attribute(XName.Get("PointY", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Double).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("PointY", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Double).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public double? Width
    {
      get
      {
        if ((Attribute(XName.Get("Width", "")) == null))
        {
          return null;
        }
        return XTypedServices.ParseValue<double>(Attribute(XName.Get("Width", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Double).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Width", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Double).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public double? Height
    {
      get
      {
        if ((Attribute(XName.Get("Height", "")) == null))
        {
          return null;
        }
        return XTypedServices.ParseValue<double>(Attribute(XName.Get("Height", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Double).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Height", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Double).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public bool? IsExpanded
    {
      get
      {
        if ((Attribute(XName.Get("IsExpanded", "")) == null))
        {
          return null;
        }
        return XTypedServices.ParseValue<bool>(Attribute(XName.Get("IsExpanded", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("IsExpanded", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
    }

    #region IXMetaData Members

    XName IXMetaData.SchemaName
    {
      get
      {
        return XName.Get("TEntityTypeShape", "http://schemas.microsoft.com/ado/2008/10/edmx");
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

    public static explicit operator EntityTypeShape(XElement xe)
    {
      return XTypedServices.ToXTypedElement<EntityTypeShape>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }
  }
}