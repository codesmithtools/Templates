using System;
using System.Collections.Generic;
using System.Xml.Linq;
using LinqToEdmx.Map;
using LinqToEdmx.Model.Storage;
using Xml.Schema.Linq;

namespace LinqToEdmx.Designer
{
  /// <summary>
  /// <para>
  /// Regular expression: (Mapping?)
  /// </para>
  /// </summary>
  public class Mappings : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static readonly ContentModelEntity ContentModel;

    static Mappings()
    {
      BuildElementDictionary();
      ContentModel = new SequenceContentModelEntity(new NamedContentModelEntity(XName.Get("Mapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs")));
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (Mapping?)
    /// </para>
    /// </summary>
    public Mapping Mapping
    {
      get
      {
        return ((Mapping) GetElement(XName.Get("Mapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs")));
      }
      set
      {
        SetElement(XName.Get("Mapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), value);
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
        return XName.Get("TRuntimeMappings", "http://schemas.microsoft.com/ado/2008/10/edmx");
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

    public static explicit operator Mappings(XElement xe)
    {
      return XTypedServices.ToXTypedElement<Mappings>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("Mapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"), typeof (Mapping));
    }
  }
}