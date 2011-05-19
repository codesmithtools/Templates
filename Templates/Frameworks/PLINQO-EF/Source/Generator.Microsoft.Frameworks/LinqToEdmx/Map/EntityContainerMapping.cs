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
  /// Regular expression: ((EntitySetMapping? | AssociationSetMapping? | FunctionImportMapping?)+)
  /// </para>
  /// </summary>
  public class EntityContainerMapping : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private XTypedList<AssociationSetMapping> _associationSetMappingField;

    private XTypedList<EntitySetMapping> _entitySetMappingField;

    private XTypedList<FunctionImportMapping> _functionImportMappingField;

    static EntityContainerMapping()
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
    /// Regular expression: ((EntitySetMapping? | AssociationSetMapping? | FunctionImportMapping?)+)
    /// </para>
    /// </summary>
    public IList<EntitySetMapping> EntitySetMappings
    {
      get
      {
        if ((_entitySetMappingField == null))
        {
          _entitySetMappingField = new XTypedList<EntitySetMapping>(this, LinqToXsdTypeManager.Instance, XName.Get("EntitySetMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
        }
        return _entitySetMappingField;
      }
      set
      {
        if ((value == null))
        {
          _entitySetMappingField = null;
        }
        else
        {
          if ((_entitySetMappingField == null))
          {
            _entitySetMappingField = XTypedList<EntitySetMapping>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("EntitySetMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
          }
          else
          {
            XTypedServices.SetList(_entitySetMappingField, value);
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
    /// Regular expression: ((EntitySetMapping? | AssociationSetMapping? | FunctionImportMapping?)+)
    /// </para>
    /// </summary>
    public IList<AssociationSetMapping> AssociationSetMappings
    {
      get
      {
        if ((_associationSetMappingField == null))
        {
          _associationSetMappingField = new XTypedList<AssociationSetMapping>(this, LinqToXsdTypeManager.Instance, XName.Get("AssociationSetMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
        }
        return _associationSetMappingField;
      }
      set
      {
        if ((value == null))
        {
          _associationSetMappingField = null;
        }
        else
        {
          if ((_associationSetMappingField == null))
          {
            _associationSetMappingField = XTypedList<AssociationSetMapping>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("AssociationSetMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
          }
          else
          {
            XTypedServices.SetList(_associationSetMappingField, value);
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
    /// Regular expression: ((EntitySetMapping? | AssociationSetMapping? | FunctionImportMapping?)+)
    /// </para>
    /// </summary>
    public IList<FunctionImportMapping> FunctionImportMappings
    {
      get
      {
        if ((_functionImportMappingField == null))
        {
          _functionImportMappingField = new XTypedList<FunctionImportMapping>(this, LinqToXsdTypeManager.Instance, XName.Get("FunctionImportMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
        }
        return _functionImportMappingField;
      }
      set
      {
        if ((value == null))
        {
          _functionImportMappingField = null;
        }
        else
        {
          if ((_functionImportMappingField == null))
          {
            _functionImportMappingField = XTypedList<FunctionImportMapping>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("FunctionImportMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
          }
          else
          {
            XTypedServices.SetList(_functionImportMappingField, value);
          }
        }
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required
    /// </para>
    /// </summary>
    public string CdmEntityContainer
    {
      get
      {
        return XTypedServices.ParseValue<string>(Attribute(XName.Get("CdmEntityContainer", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Token).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("CdmEntityContainer", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Token).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required
    /// </para>
    /// </summary>
    public string StorageEntityContainer
    {
      get
      {
        return XTypedServices.ParseValue<string>(Attribute(XName.Get("StorageEntityContainer", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("StorageEntityContainer", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public bool? GenerateUpdateViews
    {
      get
      {
        if ((Attribute(XName.Get("GenerateUpdateViews", "")) == null))
        {
          return null;
        }
        return XTypedServices.ParseValue<bool>(Attribute(XName.Get("GenerateUpdateViews", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("GenerateUpdateViews", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
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
        return XName.Get("TEntityContainerMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs");
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

    public static explicit operator EntityContainerMapping(XElement xe)
    {
      return XTypedServices.ToXTypedElement<EntityContainerMapping>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("EntitySetMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (EntitySetMapping));
      LocalElementDictionary.Add(XName.Get("AssociationSetMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (AssociationSetMapping));
      LocalElementDictionary.Add(XName.Get("FunctionImportMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (FunctionImportMapping));
    }
  }
}