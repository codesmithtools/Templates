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
  /// Regular expression: ((QueryView*, EntityTypeMapping*)|MappingFragment* | ((ComplexProperty? | ScalarProperty? | Condition?)+))
  /// </para>
  /// </summary>
  public class EntitySetMapping : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private XTypedList<ComplexProperty> _complexPropertyField;

    private XTypedList<Condition> _conditionField;

    private XTypedList<EntityTypeMapping> _entityTypeMappingField;

    private XTypedList<MappingFragment> _mappingFragmentField;

    private XTypedList<QueryView> _queryViewField;

    private XTypedList<ScalarProperty> _scalarPropertyField;

    static EntitySetMapping()
    {
      BuildElementDictionary();
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, repeating
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: ((QueryView*, EntityTypeMapping*)|MappingFragment* | ((ComplexProperty? | ScalarProperty? | Condition?)+))
    /// </para>
    /// </summary>
    public IList<QueryView> QueryViews
    {
      get
      {
        if ((_queryViewField == null))
        {
          _queryViewField = new XTypedList<QueryView>(this, LinqToXsdTypeManager.Instance, XName.Get("QueryView", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
        }
        return _queryViewField;
      }
      set
      {
        if ((value == null))
        {
          _queryViewField = null;
        }
        else
        {
          if ((_queryViewField == null))
          {
            _queryViewField = XTypedList<QueryView>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("QueryView", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
          }
          else
          {
            XTypedServices.SetList(_queryViewField, value);
          }
        }
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, repeating
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: ((QueryView*, EntityTypeMapping*)|MappingFragment* | ((ComplexProperty? | ScalarProperty? | Condition?)+))
    /// </para>
    /// </summary>
    public IList<EntityTypeMapping> EntityTypeMappings
    {
      get
      {
        if ((_entityTypeMappingField == null))
        {
          _entityTypeMappingField = new XTypedList<EntityTypeMapping>(this, LinqToXsdTypeManager.Instance, XName.Get("EntityTypeMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
        }
        return _entityTypeMappingField;
      }
      set
      {
        if ((value == null))
        {
          _entityTypeMappingField = null;
        }
        else
        {
          if ((_entityTypeMappingField == null))
          {
            _entityTypeMappingField = XTypedList<EntityTypeMapping>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("EntityTypeMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
          }
          else
          {
            XTypedServices.SetList(_entityTypeMappingField, value);
          }
        }
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, repeating, choice
    /// </para>
    /// <para>
    /// Regular expression: ((QueryView*, EntityTypeMapping*)|MappingFragment* | ((ComplexProperty? | ScalarProperty? | Condition?)+))
    /// </para>
    /// </summary>
    public IList<MappingFragment> MappingFragments
    {
      get
      {
        if ((_mappingFragmentField == null))
        {
          _mappingFragmentField = new XTypedList<MappingFragment>(this, LinqToXsdTypeManager.Instance, XName.Get("MappingFragment", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
        }
        return _mappingFragmentField;
      }
      set
      {
        if ((value == null))
        {
          _mappingFragmentField = null;
        }
        else
        {
          if ((_mappingFragmentField == null))
          {
            _mappingFragmentField = XTypedList<MappingFragment>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("MappingFragment", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
          }
          else
          {
            XTypedServices.SetList(_mappingFragmentField, value);
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
    /// Regular expression: ((QueryView*, EntityTypeMapping*)|MappingFragment* | ((ComplexProperty? | ScalarProperty? | Condition?)+))
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
    /// Regular expression: ((QueryView*, EntityTypeMapping*)|MappingFragment* | ((ComplexProperty? | ScalarProperty? | Condition?)+))
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
    /// Regular expression: ((QueryView*, EntityTypeMapping*)|MappingFragment* | ((ComplexProperty? | ScalarProperty? | Condition?)+))
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
        return XName.Get("TEntitySetMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs");
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

    public static explicit operator EntitySetMapping(XElement xe)
    {
      return XTypedServices.ToXTypedElement<EntitySetMapping>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("QueryView", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (QueryView));
      LocalElementDictionary.Add(XName.Get("EntityTypeMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (EntityTypeMapping));
      LocalElementDictionary.Add(XName.Get("MappingFragment", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (MappingFragment));
      LocalElementDictionary.Add(XName.Get("ComplexProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (ComplexProperty));
      LocalElementDictionary.Add(XName.Get("ScalarProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (ScalarProperty));
      LocalElementDictionary.Add(XName.Get("Condition", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (Condition));
    }
  }
}