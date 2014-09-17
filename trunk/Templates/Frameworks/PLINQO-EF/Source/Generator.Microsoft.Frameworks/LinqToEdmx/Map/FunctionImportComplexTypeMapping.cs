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
  public class FunctionImportComplexTypeMapping : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static readonly ContentModelEntity ContentModel;

    private XTypedList<ScalarProperty> ScalarPropertyField;

    static FunctionImportComplexTypeMapping()
    {
      BuildElementDictionary();
      ContentModel = new SequenceContentModelEntity(new NamedContentModelEntity(XName.Get("ScalarProperty", XMLNamespaceFactory.CS)));
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, repeating
    /// </para>
    /// <para>
    /// Regular expression: (ScalarProperty*)
    /// </para>
    /// </summary>
    public IList<ScalarProperty> ScalarProperties
    {
      get
      {
        if ((ScalarPropertyField == null))
        {
          ScalarPropertyField = new XTypedList<ScalarProperty>(this, LinqToXsdTypeManager.Instance, XName.Get("ScalarProperty", XMLNamespaceFactory.CS));
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
            ScalarPropertyField = XTypedList<ScalarProperty>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("ScalarProperty", XMLNamespaceFactory.CS));
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
        return XName.Get("TFunctionImportComplexTypeMapping", XMLNamespaceFactory.CS);
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

    public static explicit operator FunctionImportComplexTypeMapping(XElement xe)
    {
      return XTypedServices.ToXTypedElement<FunctionImportComplexTypeMapping>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("ScalarProperty", XMLNamespaceFactory.CS), typeof (ScalarProperty));
    }
  }
}