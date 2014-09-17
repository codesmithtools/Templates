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
  /// Regular expression: ((ScalarProperty | ComplexProperty | ComplexTypeMapping | Condition)+)
  /// </para>
  /// </summary>
  public class ComplexProperty : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private XTypedList<ComplexProperty> _complexPropertyField;

    private XTypedList<ComplexTypeMapping> _complexTypeMappingField;

    private XTypedList<Condition> _conditionField;

    private XTypedList<ScalarProperty> _scalarPropertyField;

    static ComplexProperty()
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
    /// Regular expression: ((ScalarProperty | ComplexProperty | ComplexTypeMapping | Condition)+)
    /// </para>
    /// </summary>
    public IList<ScalarProperty> ScalarProperties
    {
      get
      {
        if ((_scalarPropertyField == null))
        {
          _scalarPropertyField = new XTypedList<ScalarProperty>(this, LinqToXsdTypeManager.Instance, XName.Get("ScalarProperty", XMLNamespaceFactory.CS));
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
            _scalarPropertyField = XTypedList<ScalarProperty>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("ScalarProperty", XMLNamespaceFactory.CS));
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
    /// Occurrence: required, choice
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: ((ScalarProperty | ComplexProperty | ComplexTypeMapping | Condition)+)
    /// </para>
    /// </summary>
    public IList<ComplexProperty> ComplexProperties
    {
      get
      {
        if ((_complexPropertyField == null))
        {
          _complexPropertyField = new XTypedList<ComplexProperty>(this, LinqToXsdTypeManager.Instance, XName.Get("ComplexProperty", XMLNamespaceFactory.CS));
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
            _complexPropertyField = XTypedList<ComplexProperty>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("ComplexProperty", XMLNamespaceFactory.CS));
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
    /// Occurrence: required, choice
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: ((ScalarProperty | ComplexProperty | ComplexTypeMapping | Condition)+)
    /// </para>
    /// </summary>
    public IList<ComplexTypeMapping> ComplexTypeMappings
    {
      get
      {
        if ((_complexTypeMappingField == null))
        {
          _complexTypeMappingField = new XTypedList<ComplexTypeMapping>(this, LinqToXsdTypeManager.Instance, XName.Get("ComplexTypeMapping", XMLNamespaceFactory.CS));
        }
        return _complexTypeMappingField;
      }
      set
      {
        if ((value == null))
        {
          _complexTypeMappingField = null;
        }
        else
        {
          if ((_complexTypeMappingField == null))
          {
            _complexTypeMappingField = XTypedList<ComplexTypeMapping>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("ComplexTypeMapping", XMLNamespaceFactory.CS));
          }
          else
          {
            XTypedServices.SetList(_complexTypeMappingField, value);
          }
        }
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required, choice
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: ((ScalarProperty | ComplexProperty | ComplexTypeMapping | Condition)+)
    /// </para>
    /// </summary>
    public IList<Condition> Conditions
    {
      get
      {
        if ((_conditionField == null))
        {
          _conditionField = new XTypedList<Condition>(this, LinqToXsdTypeManager.Instance, XName.Get("Condition", XMLNamespaceFactory.CS));
        }
        return _conditionField;
      }
      set
      {
        if ((value == null))
        {
          _conditionField = null;
        }
        else
        {
          if ((_conditionField == null))
          {
            _conditionField = XTypedList<Condition>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("Condition", XMLNamespaceFactory.CS));
          }
          else
          {
            XTypedServices.SetList(_conditionField, value);
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
    /// Occurrence: optional
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

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public bool? IsPartial
    {
      get
      {
        if ((Attribute(XName.Get("IsPartial", "")) == null))
        {
          return null;
        }
        return XTypedServices.ParseValue<bool>(Attribute(XName.Get("IsPartial", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("IsPartial", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
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
        return XName.Get("TComplexProperty", XMLNamespaceFactory.CS);
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

    public static explicit operator ComplexProperty(XElement xe)
    {
      return XTypedServices.ToXTypedElement<ComplexProperty>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("ScalarProperty", XMLNamespaceFactory.CS), typeof (ScalarProperty));
      LocalElementDictionary.Add(XName.Get("ComplexProperty", XMLNamespaceFactory.CS), typeof (ComplexProperty));
      LocalElementDictionary.Add(XName.Get("ComplexTypeMapping", XMLNamespaceFactory.CS), typeof (ComplexTypeMapping));
      LocalElementDictionary.Add(XName.Get("Condition", XMLNamespaceFactory.CS), typeof (Condition));
    }
  }
}