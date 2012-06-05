using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Xml.Schema.Linq;

namespace LinqToEdmx.Model.Storage
{
  /// <summary>
  /// <para>
  /// The Documentation element is used to provide documentation of comments on the contents of the XML file. It is valid under Schema, Type, Index and Relationship elements.
  ///      
  /// </para>
  /// <para>
  /// Regular expression: (Summary?, LongDescription?)
  /// </para>
  /// </summary>
  public class Documentation : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static readonly ContentModelEntity ContentModel;

    static Documentation()
    {
      BuildElementDictionary();
      ContentModel = new SequenceContentModelEntity(new NamedContentModelEntity(XName.Get("Summary", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl")), new NamedContentModelEntity(XName.Get("LongDescription", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl")));
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (Summary?, LongDescription?)
    /// </para>
    /// </summary>
    public Text Summary
    {
      get
      {
        var x = GetElement(XName.Get("Summary", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"));
        return ((Text) (x));
      }
      set
      {
        SetElement(XName.Get("Summary", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), value);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (Summary?, LongDescription?)
    /// </para>
    /// </summary>
    public Text LongDescription
    {
      get
      {
        var x = GetElement(XName.Get("LongDescription", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"));
        return ((Text) (x));
      }
      set
      {
        SetElement(XName.Get("LongDescription", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), value);
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
        return XName.Get("TDocumentation", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl");
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

    public static explicit operator Documentation(XElement xe)
    {
      return XTypedServices.ToXTypedElement<Documentation>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("Summary", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (Text));
      LocalElementDictionary.Add(XName.Get("LongDescription", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (Text));
    }
  }
}