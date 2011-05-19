using System;
using System.Collections.Generic;
using System.Xml.Linq;
using LinqToEdmx.Model.Storage;
using Xml.Schema.Linq;

namespace LinqToEdmx.Map
{
  /// <summary>
  /// <para>
  /// Regular expression: (EntityTypeMapping* | ComplexTypeMapping*)
  /// </para>
  /// </summary>
  public class FunctionImportMappingResultMapping : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static readonly ContentModelEntity ContentModel;

    private XTypedList<FunctionImportComplexTypeMapping> _complexTypeMappingField;

    private XTypedList<FunctionImportEntityTypeMapping> _entityTypeMappingField;

    static FunctionImportMappingResultMapping()
    {
      BuildElementDictionary();
      ContentModel = new ChoiceContentModelEntity(new NamedContentModelEntity(XName.Get("EntityTypeMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs")), new NamedContentModelEntity(XName.Get("ComplexTypeMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs")));
    }

    /// <summary>
    /// <para>
    /// Regular expression: (EntityTypeMapping* | ComplexTypeMapping*)
    /// </para>
    /// </summary>
    public FunctionImportMappingResultMapping()
    {
    }

    public FunctionImportMappingResultMapping(IEnumerable<FunctionImportEntityTypeMapping> entityTypeMapping)
    {
      _entityTypeMappingField = XTypedList<FunctionImportEntityTypeMapping>.Initialize(this, LinqToXsdTypeManager.Instance, entityTypeMapping, XName.Get("EntityTypeMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
    }

    public FunctionImportMappingResultMapping(IEnumerable<FunctionImportComplexTypeMapping> complexTypeMapping)
    {
      _complexTypeMappingField = XTypedList<FunctionImportComplexTypeMapping>.Initialize(this, LinqToXsdTypeManager.Instance, complexTypeMapping, XName.Get("ComplexTypeMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, repeating, choice
    /// </para>
    /// <para>
    /// Regular expression: (EntityTypeMapping* | ComplexTypeMapping*)
    /// </para>
    /// </summary>
    public IList<FunctionImportEntityTypeMapping> EntityTypeMappings
    {
      get
      {
        if ((_entityTypeMappingField == null))
        {
          _entityTypeMappingField = new XTypedList<FunctionImportEntityTypeMapping>(this, LinqToXsdTypeManager.Instance, XName.Get("EntityTypeMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
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
            _entityTypeMappingField = XTypedList<FunctionImportEntityTypeMapping>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("EntityTypeMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
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
    /// Regular expression: (EntityTypeMapping* | ComplexTypeMapping*)
    /// </para>
    /// </summary>
    public IList<FunctionImportComplexTypeMapping> ComplexTypeMappings
    {
      get
      {
        if ((_complexTypeMappingField == null))
        {
          _complexTypeMappingField = new XTypedList<FunctionImportComplexTypeMapping>(this, LinqToXsdTypeManager.Instance, XName.Get("ComplexTypeMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
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
            _complexTypeMappingField = XTypedList<FunctionImportComplexTypeMapping>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("ComplexTypeMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"));
          }
          else
          {
            XTypedServices.SetList(_complexTypeMappingField, value);
          }
        }
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
        return XName.Get("TFunctionImportMappingResultMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs");
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

    public static explicit operator FunctionImportMappingResultMapping(XElement xe)
    {
      return XTypedServices.ToXTypedElement<FunctionImportMappingResultMapping>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("EntityTypeMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (FunctionImportEntityTypeMapping));
      LocalElementDictionary.Add(XName.Get("ComplexTypeMapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (FunctionImportComplexTypeMapping));
    }
  }
}