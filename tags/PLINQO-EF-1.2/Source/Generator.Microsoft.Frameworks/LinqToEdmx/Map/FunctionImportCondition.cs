using System.Xml.Linq;
using System.Xml.Schema;
using LinqToEdmx.Model.Storage;
using Xml.Schema.Linq;

namespace LinqToEdmx.Map
{
  public class FunctionImportCondition : XTypedElement, IXMetaData
  {
    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public string Value
    {
      get
      {
        return XTypedServices.ParseValue<string>(Attribute(XName.Get("Value", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Value", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required
    /// </para>
    /// </summary>
    public string ColumnName
    {
      get
      {
        return XTypedServices.ParseValue<string>(Attribute(XName.Get("ColumnName", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("ColumnName", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public bool? IsNull
    {
      get
      {
        if ((Attribute(XName.Get("IsNull", "")) == null))
        {
          return null;
        }
        return XTypedServices.ParseValue<bool>(Attribute(XName.Get("IsNull", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("IsNull", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
    }

    #region IXMetaData Members

    XName IXMetaData.SchemaName
    {
      get
      {
        return XName.Get("TFunctionImportCondition", "http://schemas.microsoft.com/ado/2008/09/mapping/cs");
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

    public static explicit operator FunctionImportCondition(XElement xe)
    {
      return XTypedServices.ToXTypedElement<FunctionImportCondition>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }
  }
}