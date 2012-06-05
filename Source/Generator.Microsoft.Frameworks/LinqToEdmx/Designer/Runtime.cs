using System;
using System.Collections.Generic;
using System.Xml.Linq;
using LinqToEdmx.Model.Storage;
using Xml.Schema.Linq;

namespace LinqToEdmx.Designer
{
  /// <summary>
  /// <para>
  /// Regular expression: (StorageModels?, ConceptualModels?, Mappings?)
  /// </para>
  /// </summary>
  public class Runtime : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    static Runtime()
    {
      BuildElementDictionary();
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (StorageModels?, ConceptualModels?, Mappings?)
    /// </para>
    /// </summary>
    public StorageModels StorageModels
    {
      get
      {
        return ((StorageModels) GetElement(XName.Get("StorageModels", "http://schemas.microsoft.com/ado/2008/10/edmx")));
      }
      set
      {
        SetElement(XName.Get("StorageModels", "http://schemas.microsoft.com/ado/2008/10/edmx"), value);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (StorageModels?, ConceptualModels?, Mappings?)
    /// </para>
    /// </summary>
    public ConceptualModels ConceptualModels
    {
      get
      {
        return ((ConceptualModels) GetElement(XName.Get("ConceptualModels", "http://schemas.microsoft.com/ado/2008/10/edmx")));
      }
      set
      {
        SetElement(XName.Get("ConceptualModels", "http://schemas.microsoft.com/ado/2008/10/edmx"), value);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (StorageModels?, ConceptualModels?, Mappings?)
    /// </para>
    /// </summary>
    public Mappings Mappings
    {
      get
      {
        return ((Mappings) GetElement(XName.Get("Mappings", "http://schemas.microsoft.com/ado/2008/10/edmx")));
      }
      set
      {
        SetElement(XName.Get("Mappings", "http://schemas.microsoft.com/ado/2008/10/edmx"), value);
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
        return XName.Get("TRuntime", "http://schemas.microsoft.com/ado/2008/10/edmx");
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

    public static explicit operator Runtime(XElement xe)
    {
      return XTypedServices.ToXTypedElement<Runtime>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("StorageModels", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (StorageModels));
      LocalElementDictionary.Add(XName.Get("ConceptualModels", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (ConceptualModels));
      LocalElementDictionary.Add(XName.Get("Mappings", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (Mappings));
    }
  }
}