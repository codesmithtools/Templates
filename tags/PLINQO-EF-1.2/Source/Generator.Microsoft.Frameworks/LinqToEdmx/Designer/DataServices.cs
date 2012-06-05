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
  /// Regular expression: (Schema*)
  /// </para>
  /// </summary>
  public class DataServices : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static readonly ContentModelEntity ContentModel;

    private XTypedList<ConceptualSchema> _schemaField;

    static DataServices()
    {
      BuildElementDictionary();
      ContentModel = new SequenceContentModelEntity(new NamedContentModelEntity(XName.Get("Schema", "http://schemas.microsoft.com/ado/2008/09/edm")));
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, repeating
    /// </para>
    /// <para>
    /// Regular expression: (Schema*)
    /// </para>
    /// </summary>
    public IList<ConceptualSchema> Schemas
    {
      get
      {
        if ((_schemaField == null))
        {
          _schemaField = new XTypedList<ConceptualSchema>(this, LinqToXsdTypeManager.Instance, XName.Get("Schema", "http://schemas.microsoft.com/ado/2008/09/edm"));
        }
        return _schemaField;
      }
      set
      {
        if ((value == null))
        {
          _schemaField = null;
        }
        else
        {
          if ((_schemaField == null))
          {
            _schemaField = XTypedList<ConceptualSchema>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("Schema", "http://schemas.microsoft.com/ado/2008/09/edm"));
          }
          else
          {
            XTypedServices.SetList(_schemaField, value);
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
        return XName.Get("TDataServices", "http://schemas.microsoft.com/ado/2008/10/edmx");
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

    public static explicit operator DataServices(XElement xe)
    {
      return XTypedServices.ToXTypedElement<DataServices>(xe, LinqToXsdTypeManager.Instance);
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