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
  /// Regular expression: ((ScalarProperty? | AssociationEnd? | ComplexProperty?)+)
  /// </para>
  /// </summary>
  public class EntityTypeModificationFunction : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private XTypedList<FunctionMappingAssociationEnd> AssociationEndField;

    private XTypedList<FunctionMappingComplexProperty> ComplexPropertyField;

    private XTypedList<FunctionMappingScalarProperty> ScalarPropertyField;

    static EntityTypeModificationFunction()
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
    /// Regular expression: ((ScalarProperty? | AssociationEnd? | ComplexProperty?)+)
    /// </para>
    /// </summary>
    public IList<FunctionMappingScalarProperty> ScalarProperties
    {
      get
      {
        if ((ScalarPropertyField == null))
        {
          ScalarPropertyField = new XTypedList<FunctionMappingScalarProperty>(this, LinqToXsdTypeManager.Instance, XName.Get("ScalarProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
        }
        return ScalarPropertyField;
      }
      set
      {
        if ((value == null))
        {
          ScalarPropertyField = null;
        }
        else
        {
          if ((ScalarPropertyField == null))
          {
            ScalarPropertyField = XTypedList<FunctionMappingScalarProperty>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("ScalarProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
          }
          else
          {
            XTypedServices.SetList(ScalarPropertyField, value);
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
    /// Regular expression: ((ScalarProperty? | AssociationEnd? | ComplexProperty?)+)
    /// </para>
    /// </summary>
    public IList<FunctionMappingAssociationEnd> AssociationEnds
    {
      get
      {
        if ((AssociationEndField == null))
        {
          AssociationEndField = new XTypedList<FunctionMappingAssociationEnd>(this, LinqToXsdTypeManager.Instance, XName.Get("AssociationEnd", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
        }
        return AssociationEndField;
      }
      set
      {
        if ((value == null))
        {
          AssociationEndField = null;
        }
        else
        {
          if ((AssociationEndField == null))
          {
            AssociationEndField = XTypedList<FunctionMappingAssociationEnd>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("AssociationEnd", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
          }
          else
          {
            XTypedServices.SetList(AssociationEndField, value);
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
    /// Regular expression: ((ScalarProperty? | AssociationEnd? | ComplexProperty?)+)
    /// </para>
    /// </summary>
    public IList<FunctionMappingComplexProperty> ComplexProperties
    {
      get
      {
        if ((ComplexPropertyField == null))
        {
          ComplexPropertyField = new XTypedList<FunctionMappingComplexProperty>(this, LinqToXsdTypeManager.Instance, XName.Get("ComplexProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
        }
        return ComplexPropertyField;
      }
      set
      {
        if ((value == null))
        {
          ComplexPropertyField = null;
        }
        else
        {
          if ((ComplexPropertyField == null))
          {
            ComplexPropertyField = XTypedList<FunctionMappingComplexProperty>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("ComplexProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
          }
          else
          {
            XTypedServices.SetList(ComplexPropertyField, value);
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
        return XName.Get("TEntityTypeModificationFunction", "http://schemas.microsoft.com/ado/2008/09/mapping/cs");
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

    public static explicit operator EntityTypeModificationFunction(XElement xe)
    {
      return XTypedServices.ToXTypedElement<EntityTypeModificationFunction>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("ScalarProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (FunctionMappingScalarProperty));
      LocalElementDictionary.Add(XName.Get("AssociationEnd", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (FunctionMappingAssociationEnd));
      LocalElementDictionary.Add(XName.Get("ComplexProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (FunctionMappingComplexProperty));
    }
  }
}