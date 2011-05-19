using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Schema;
using LinqToEdmx.Model.Storage;
using Xml.Schema.Linq;

namespace LinqToEdmx.Map
{
  /// <summary>
  /// <para>
  /// Regular expression: (ResultMapping?)
  /// </para>
  /// </summary>
  public class FunctionImportMapping : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static readonly ContentModelEntity ContentModel;

    static FunctionImportMapping()
    {
      BuildElementDictionary();
      ContentModel = new SequenceContentModelEntity(new NamedContentModelEntity(XName.Get("ResultMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs")));
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (ResultMapping?)
    /// </para>
    /// </summary>
    public FunctionImportMappingResultMapping ResultMapping
    {
      get
      {
        return ((FunctionImportMappingResultMapping) GetElement(XName.Get("ResultMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs")));
      }
      set
      {
        SetElement(XName.Get("ResultMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), value);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required
    /// </para>
    /// </summary>
    public string FunctionName
    {
      get
      {
        return XTypedServices.ParseValue<string>(Attribute(XName.Get("FunctionName", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("FunctionName", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required
    /// </para>
    /// </summary>
    public string FunctionImportName
    {
      get
      {
        return XTypedServices.ParseValue<string>(Attribute(XName.Get("FunctionImportName", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Token).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("FunctionImportName", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Token).Datatype);
      }
    }

    #region IXMetaData Members

    Dictionary<XName, Type> IXMetaData.LocalElementsDictionary
    {
      get
      {
        return LocalElementDictionary;
      }
    }

    XName IXMetaData.SchemaName
    {
      get
      {
        return XName.Get("TFunctionImportMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs");
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
      return ContentModel;
    }

    #endregion

    public static explicit operator FunctionImportMapping(XElement xe)
    {
      return XTypedServices.ToXTypedElement<FunctionImportMapping>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("ResultMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (FunctionImportMappingResultMapping));
    }
  }
}