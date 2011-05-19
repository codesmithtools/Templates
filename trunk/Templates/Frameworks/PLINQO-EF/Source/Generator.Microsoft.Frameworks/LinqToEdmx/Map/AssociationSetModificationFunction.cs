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
  /// Regular expression: ((EndProperty)+)
  /// </para>
  /// </summary>
  public class AssociationSetModificationFunction : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private XTypedList<FunctionMappingEndProperty> _endPropertyField;

    static AssociationSetModificationFunction()
    {
      BuildElementDictionary();
    }

    /// <summary>
    /// <para>
    /// Occurrence: required, choice
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: ((EndProperty)+)
    /// </para>
    /// </summary>
    public IList<FunctionMappingEndProperty> EndProperties
    {
      get
      {
        if ((_endPropertyField == null))
        {
          _endPropertyField = new XTypedList<FunctionMappingEndProperty>(this, LinqToXsdTypeManager.Instance, XName.Get("EndProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
        }
        return _endPropertyField;
      }
      set
      {
        if ((value == null))
        {
          _endPropertyField = null;
        }
        else
        {
          if ((_endPropertyField == null))
          {
            _endPropertyField = XTypedList<FunctionMappingEndProperty>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("EndProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
          }
          else
          {
            XTypedServices.SetList(_endPropertyField, value);
          }
        }
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
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public string RowsAffectedParameter
    {
      get
      {
        return XTypedServices.ParseValue<string>(Attribute(XName.Get("RowsAffectedParameter", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("RowsAffectedParameter", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
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
        return XName.Get("TAssociationSetModificationFunction", "http://schemas.microsoft.com/ado/2008/09/mapping/cs");
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

    public static explicit operator AssociationSetModificationFunction(XElement xe)
    {
      return XTypedServices.ToXTypedElement<AssociationSetModificationFunction>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("EndProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (FunctionMappingEndProperty));
    }
  }
}