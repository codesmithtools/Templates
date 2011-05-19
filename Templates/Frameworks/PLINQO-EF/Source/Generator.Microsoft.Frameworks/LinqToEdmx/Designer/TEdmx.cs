using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Schema;
using LinqToEdmx.Model.Storage;
using Xml.Schema.Linq;

namespace LinqToEdmx.Designer
{
  /// <summary>
  /// <para>
  /// Regular expression: (((Designer?)?, (Runtime? | DataServices?))|((Runtime? | DataServices?), (Designer?)?))
  /// </para>
  /// </summary>
  public class TEdmx : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private XTypedList<DataServices> _dataServicesField;

    private XTypedList<Designer> _designerField;

    private XTypedList<Runtime> _runtimeField;

    static TEdmx()
    {
      BuildElementDictionary();
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: (((Designer?)?, (Runtime? | DataServices?))|((Runtime? | DataServices?), (Designer?)?))
    /// </para>
    /// </summary>
    public IList<Designer> Designers
    {
      get
      {
        if ((_designerField == null))
        {
          _designerField = new XTypedList<Designer>(this, LinqToXsdTypeManager.Instance, XName.Get("Designer", "http://schemas.microsoft.com/ado/2008/10/edmx"));
        }
        return _designerField;
      }
      set
      {
        if ((value == null))
        {
          _designerField = null;
        }
        else
        {
          if ((_designerField == null))
          {
            _designerField = XTypedList<Designer>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("Designer", "http://schemas.microsoft.com/ado/2008/10/edmx"));
          }
          else
          {
            XTypedServices.SetList(_designerField, value);
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
    /// Regular expression: (((Designer?)?, (Runtime? | DataServices?))|((Runtime? | DataServices?), (Designer?)?))
    /// </para>
    /// </summary>
    public IList<Runtime> Runtimes
    {
      get
      {
        if ((_runtimeField == null))
        {
          _runtimeField = new XTypedList<Runtime>(this, LinqToXsdTypeManager.Instance, XName.Get("Runtime", "http://schemas.microsoft.com/ado/2008/10/edmx"));
        }
        return _runtimeField;
      }
      set
      {
        if ((value == null))
        {
          _runtimeField = null;
        }
        else
        {
          if ((_runtimeField == null))
          {
            _runtimeField = XTypedList<Runtime>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("Runtime", "http://schemas.microsoft.com/ado/2008/10/edmx"));
          }
          else
          {
            XTypedServices.SetList(_runtimeField, value);
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
    /// Regular expression: (((Designer?)?, (Runtime? | DataServices?))|((Runtime? | DataServices?), (Designer?)?))
    /// </para>
    /// </summary>
    public IList<DataServices> DataServices
    {
      get
      {
        if ((_dataServicesField == null))
        {
          _dataServicesField = new XTypedList<DataServices>(this, LinqToXsdTypeManager.Instance, XName.Get("DataServices", "http://schemas.microsoft.com/ado/2008/10/edmx"));
        }
        return _dataServicesField;
      }
      set
      {
        if ((value == null))
        {
          _dataServicesField = null;
        }
        else
        {
          if ((_dataServicesField == null))
          {
            _dataServicesField = XTypedList<DataServices>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("DataServices", "http://schemas.microsoft.com/ado/2008/10/edmx"));
          }
          else
          {
            XTypedServices.SetList(_dataServicesField, value);
          }
        }
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required
    /// </para>
    /// </summary>
    public string Version
    {
      get
      {
        return XTypedServices.ParseValue<string>(Attribute(XName.Get("Version", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Version", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
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
        return XName.Get("TEdmx", "http://schemas.microsoft.com/ado/2008/10/edmx");
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

    public static explicit operator TEdmx(XElement xe)
    {
      return XTypedServices.ToXTypedElement<TEdmx>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("Designer", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (Designer));
      LocalElementDictionary.Add(XName.Get("Runtime", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (Runtime));
      LocalElementDictionary.Add(XName.Get("DataServices", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (DataServices));
    }
  }
}