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
  /// Regular expression: (ScalarProperty*)
  /// </para>
  /// </summary>
  public class FunctionMappingAssociationEnd : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static readonly ContentModelEntity ContentModel;

    private XTypedList<FunctionMappingScalarProperty> _scalarPropertyField;

    static FunctionMappingAssociationEnd()
    {
      BuildElementDictionary();
      ContentModel = new SequenceContentModelEntity(new NamedContentModelEntity(XName.Get("ScalarProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs")));
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, repeating
    /// </para>
    /// <para>
    /// Regular expression: (ScalarProperty*)
    /// </para>
    /// </summary>
    public IList<FunctionMappingScalarProperty> ScalarProperties
    {
      get
      {
        if ((_scalarPropertyField == null))
        {
          _scalarPropertyField = new XTypedList<FunctionMappingScalarProperty>(this, LinqToXsdTypeManager.Instance, XName.Get("ScalarProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
        }
        return _scalarPropertyField;
      }
      set
      {
        if ((value == null))
        {
          _scalarPropertyField = null;
        }
        else
        {
          if ((_scalarPropertyField == null))
          {
            _scalarPropertyField = XTypedList<FunctionMappingScalarProperty>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("ScalarProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
          }
          else
          {
            XTypedServices.SetList(_scalarPropertyField, value);
          }
        }
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required
    /// </para>
    /// </summary>
    public string AssociationSet
    {
      get
      {
        return XTypedServices.ParseValue<string>(Attribute(XName.Get("AssociationSet", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Token).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("AssociationSet", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Token).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required
    /// </para>
    /// </summary>
    public string From
    {
      get
      {
        return XTypedServices.ParseValue<string>(Attribute(XName.Get("From", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Token).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("From", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Token).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required
    /// </para>
    /// </summary>
    public string To
    {
      get
      {
        return XTypedServices.ParseValue<string>(Attribute(XName.Get("To", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Token).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("To", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Token).Datatype);
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
        return XName.Get("TFunctionMappingAssociationEnd", "http://schemas.microsoft.com/ado/2008/09/mapping/cs");
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

    public static explicit operator FunctionMappingAssociationEnd(XElement xe)
    {
      return XTypedServices.ToXTypedElement<FunctionMappingAssociationEnd>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("ScalarProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (FunctionMappingScalarProperty));
    }
  }
}