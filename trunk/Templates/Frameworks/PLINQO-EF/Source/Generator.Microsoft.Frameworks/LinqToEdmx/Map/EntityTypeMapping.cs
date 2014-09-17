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
  /// Regular expression: (MappingFragment*, ModificationFunctionMapping?)
  /// </para>
  /// </summary>
  public class EntityTypeMapping : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static readonly ContentModelEntity ContentModel;

    private XTypedList<MappingFragment> MappingFragmentField;

    static EntityTypeMapping()
    {
      BuildElementDictionary();
      ContentModel = new SequenceContentModelEntity(new NamedContentModelEntity(XName.Get("MappingFragment", XMLNamespaceFactory.CS)), new NamedContentModelEntity(XName.Get("ModificationFunctionMapping", XMLNamespaceFactory.CS)));
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, repeating
    /// </para>
    /// <para>
    /// Regular expression: (MappingFragment*, ModificationFunctionMapping?)
    /// </para>
    /// </summary>
    public IList<MappingFragment> MappingFragments
    {
      get
      {
        if ((MappingFragmentField == null))
        {
          MappingFragmentField = new XTypedList<MappingFragment>(this, LinqToXsdTypeManager.Instance, XName.Get("MappingFragment", XMLNamespaceFactory.CS));
        }
        return MappingFragmentField;
      }
      set
      {
        if ((value == null))
        {
          MappingFragmentField = null;
        }
        else
        {
          if ((MappingFragmentField == null))
          {
            MappingFragmentField = XTypedList<MappingFragment>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("MappingFragment", XMLNamespaceFactory.CS));
          }
          else
          {
            XTypedServices.SetList(MappingFragmentField, value);
          }
        }
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (MappingFragment*, ModificationFunctionMapping?)
    /// </para>
    /// </summary>
    public EntityTypeModificationFunctionMapping ModificationFunctionMapping
    {
      get
      {
        return ((EntityTypeModificationFunctionMapping) GetElement(XName.Get("ModificationFunctionMapping", XMLNamespaceFactory.CS)));
      }
      set
      {
        SetElement(XName.Get("ModificationFunctionMapping", XMLNamespaceFactory.CS), value);
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
        return XName.Get("TEntityTypeMapping", XMLNamespaceFactory.CS);
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

    public static explicit operator EntityTypeMapping(XElement xe)
    {
      return XTypedServices.ToXTypedElement<EntityTypeMapping>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("MappingFragment", XMLNamespaceFactory.CS), typeof (MappingFragment));
      LocalElementDictionary.Add(XName.Get("ModificationFunctionMapping", XMLNamespaceFactory.CS), typeof (EntityTypeModificationFunctionMapping));
    }
  }
}