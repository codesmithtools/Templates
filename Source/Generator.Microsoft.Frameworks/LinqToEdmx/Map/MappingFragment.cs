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
  /// Regular expression: ((ComplexProperty? | ScalarProperty? | Condition?)+)
  /// </para>
  /// </summary>
  public class MappingFragment : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private XTypedList<ComplexProperty> _complexPropertyField;

    private XTypedList<Condition> _conditionField;

    private XTypedList<ScalarProperty> _scalarPropertyField;

    static MappingFragment()
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
    /// Regular expression: ((ComplexProperty? | ScalarProperty? | Condition?)+)
    /// </para>
    /// </summary>
    public IList<ComplexProperty> ComplexProperties
    {
      get
      {
        if ((_complexPropertyField == null))
        {
          _complexPropertyField = new XTypedList<ComplexProperty>(this, LinqToXsdTypeManager.Instance, XName.Get("ComplexProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
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
            _complexPropertyField = XTypedList<ComplexProperty>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("ComplexProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
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
    /// Occurrence: optional, choice
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: ((ComplexProperty? | ScalarProperty? | Condition?)+)
    /// </para>
    /// </summary>
    public IList<ScalarProperty> ScalarProperties
    {
      get
      {
        if ((_scalarPropertyField == null))
        {
          _scalarPropertyField = new XTypedList<ScalarProperty>(this, LinqToXsdTypeManager.Instance, XName.Get("ScalarProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
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
            _scalarPropertyField = XTypedList<ScalarProperty>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("ScalarProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
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
    /// Regular expression: ((ComplexProperty? | ScalarProperty? | Condition?)+)
    /// </para>
    /// </summary>
    public IList<Condition> Conditions
    {
      get
      {
        if ((_conditionField == null))
        {
          _conditionField = new XTypedList<Condition>(this, LinqToXsdTypeManager.Instance, XName.Get("Condition", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
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
            _conditionField = XTypedList<Condition>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("Condition", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
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
    public string StoreEntitySet
    {
      get
      {
        return XTypedServices.ParseValue<string>(Attribute(XName.Get("StoreEntitySet", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("StoreEntitySet", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public bool? MakeColumnsDistinct
    {
      get
      {
        if ((Attribute(XName.Get("MakeColumnsDistinct", "")) == null))
        {
          return null;
        }
        return XTypedServices.ParseValue<bool>(Attribute(XName.Get("MakeColumnsDistinct", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("MakeColumnsDistinct", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
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
        return XName.Get("TMappingFragment", "http://schemas.microsoft.com/ado/2008/09/mapping/cs");
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

    public static explicit operator MappingFragment(XElement xe)
    {
      return XTypedServices.ToXTypedElement<MappingFragment>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("ComplexProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (ComplexProperty));
      LocalElementDictionary.Add(XName.Get("ScalarProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (ScalarProperty));
      LocalElementDictionary.Add(XName.Get("Condition", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (Condition));
    }
  }
}