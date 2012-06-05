using System;
using System.Collections.Generic;
using System.Xml.Linq;
using LinqToEdmx.Model.Conceptual;
using LinqToEdmx.Model.Storage;
using Xml.Schema.Linq;

namespace LinqToEdmx.Designer
{
  /// <summary>
  /// <para>
  /// Regular expression: (Schema?)
  /// </para>
  /// </summary>
  public class ConceptualModels : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static readonly ContentModelEntity ContentModel;

    static ConceptualModels()
    {
      BuildElementDictionary();
      ContentModel = new SequenceContentModelEntity(new NamedContentModelEntity(XName.Get("Schema", "http://schemas.microsoft.com/ado/2008/09/edm")));
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (Schema?)
    /// </para>
    /// </summary>
    public ConceptualSchema ConceptualSchema
    {
      get
      {
        return ((ConceptualSchema) GetElement(XName.Get("Schema", "http://schemas.microsoft.com/ado/2008/09/edm")));
      }
      set
      {
        SetElement(XName.Get("Schema", "http://schemas.microsoft.com/ado/2008/09/edm"), value);
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
        return XName.Get("TRuntimeConceptualModels", "http://schemas.microsoft.com/ado/2008/10/edmx");
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

    public static explicit operator ConceptualModels(XElement xe)
    {
      return XTypedServices.ToXTypedElement<ConceptualModels>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("Schema", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (ConceptualSchema));
    }
  }
}