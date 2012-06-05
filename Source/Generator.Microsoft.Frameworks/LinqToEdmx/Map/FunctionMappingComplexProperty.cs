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
  /// Regular expression: ((ScalarProperty? | ComplexProperty?)+)
  /// </para>
  /// </summary>
  public class FunctionMappingComplexProperty : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private XTypedList<FunctionMappingComplexProperty> _complexPropertyField;

    private XTypedList<FunctionMappingScalarProperty> _scalarPropertyField;

    static FunctionMappingComplexProperty()
    {
      BuildElementDictionary();
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, choice
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: ((ScalarProperty? | ComplexProperty?)+)
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
    /// Occurrence: optional, choice
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: ((ScalarProperty? | ComplexProperty?)+)
    /// </para>
    /// </summary>
    public IList<FunctionMappingComplexProperty> ComplexProperties
    {
      get
      {
        if ((_complexPropertyField == null))
        {
          _complexPropertyField = new XTypedList<FunctionMappingComplexProperty>(this, LinqToXsdTypeManager.Instance, XName.Get("ComplexProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
        }
        return _complexPropertyField;
      }
      set
      {
        if ((value == null))
        {
          _complexPropertyField = null;
        }
        else
        {
          if ((_complexPropertyField == null))
          {
            _complexPropertyField = XTypedList<FunctionMappingComplexProperty>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("ComplexProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
          }
          else
          {
            XTypedServices.SetList(_complexPropertyField, value);
          }
        }
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required
    /// </para>
    /// </summary>
    public string Name
    {
      get
      {
        return XTypedServices.ParseValue<string>(Attribute(XName.Get("Name", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Token).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Name", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Token).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required
    /// </para>
    /// </summary>
    public string TypeName
    {
      get
      {
        return XTypedServices.ParseValue<string>(Attribute(XName.Get("TypeName", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("TypeName", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
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
        return XName.Get("TFunctionMappingComplexProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs");
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

    public static explicit operator FunctionMappingComplexProperty(XElement xe)
    {
      return XTypedServices.ToXTypedElement<FunctionMappingComplexProperty>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("ScalarProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (FunctionMappingScalarProperty));
      LocalElementDictionary.Add(XName.Get("ComplexProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (FunctionMappingComplexProperty));
    }
  }
}