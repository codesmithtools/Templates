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
  /// Regular expression: (QueryView?, EndProperty*, Condition*, ModificationFunctionMapping?)
  /// </para>
  /// </summary>
  public class AssociationSetMapping : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static readonly ContentModelEntity ContentModel;

    private XTypedList<Condition> _conditionField;

    private XTypedList<EndProperty> _endPropertyField;

    static AssociationSetMapping()
    {
      BuildElementDictionary();
      ContentModel = new SequenceContentModelEntity(new NamedContentModelEntity(XName.Get("QueryView", "http://schemas.microsoft.com/ado/2008/09/mapping/cs")), new NamedContentModelEntity(XName.Get("EndProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs")), new NamedContentModelEntity(XName.Get("Condition", "http://schemas.microsoft.com/ado/2008/09/mapping/cs")), new NamedContentModelEntity(XName.Get("ModificationFunctionMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs")));
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (QueryView?, EndProperty*, Condition*, ModificationFunctionMapping?)
    /// </para>
    /// </summary>
    public string QueryView
    {
      get
      {
        return XTypedServices.ParseValue<string>(GetElement(XName.Get("QueryView", "http://schemas.microsoft.com/ado/2008/09/mapping/cs")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetElement(XName.Get("QueryView", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, repeating
    /// </para>
    /// <para>
    /// Regular expression: (QueryView?, EndProperty*, Condition*, ModificationFunctionMapping?)
    /// </para>
    /// </summary>
    public IList<EndProperty> EndProperties
    {
      get
      {
        if ((_endPropertyField == null))
        {
          _endPropertyField = new XTypedList<EndProperty>(this, LinqToXsdTypeManager.Instance, XName.Get("EndProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
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
            _endPropertyField = XTypedList<EndProperty>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("EndProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
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
    /// Occurrence: optional, repeating
    /// </para>
    /// <para>
    /// Regular expression: (QueryView?, EndProperty*, Condition*, ModificationFunctionMapping?)
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
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (QueryView?, EndProperty*, Condition*, ModificationFunctionMapping?)
    /// </para>
    /// </summary>
    public AssociationSetModificationFunctionMapping ModificationFunctionMapping
    {
      get
      {
        return ((AssociationSetModificationFunctionMapping) GetElement(XName.Get("ModificationFunctionMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs")));
      }
      set
      {
        SetElement(XName.Get("ModificationFunctionMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), value);
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
        return XTypedServices.ParseValue<string>(Attribute(XName.Get("TypeName", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Token).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("TypeName", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Token).Datatype);
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
        return XName.Get("TAssociationSetMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs");
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

    public static explicit operator AssociationSetMapping(XElement xe)
    {
      return XTypedServices.ToXTypedElement<AssociationSetMapping>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("QueryView", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (string));
      LocalElementDictionary.Add(XName.Get("EndProperty", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (EndProperty));
      LocalElementDictionary.Add(XName.Get("Condition", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (Condition));
      LocalElementDictionary.Add(XName.Get("ModificationFunctionMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (AssociationSetModificationFunctionMapping));
    }
  }
}