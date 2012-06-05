using System;
using System.Collections.Generic;
using System.Xml.Linq;
using LinqToEdmx.Model.Storage;
using Xml.Schema.Linq;

namespace LinqToEdmx.Designer
{
  /// <summary>
  /// <para>
  /// Regular expression: (Diagram*)
  /// </para>
  /// </summary>
  public class Diagrams : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static readonly ContentModelEntity ContentModel;

    private XTypedList<Diagram> _diagramField;

    static Diagrams()
    {
      BuildElementDictionary();
      ContentModel = new SequenceContentModelEntity(new NamedContentModelEntity(XName.Get("Diagram", "http://schemas.microsoft.com/ado/2008/10/edmx")));
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, repeating
    /// </para>
    /// <para>
    /// Regular expression: (Diagram*)
    /// </para>
    /// </summary>
    public IList<Diagram> Diagram
    {
      get
      {
        if ((_diagramField == null))
        {
          _diagramField = new XTypedList<Diagram>(this, LinqToXsdTypeManager.Instance, XName.Get("Diagram", "http://schemas.microsoft.com/ado/2008/10/edmx"));
        }
        return _diagramField;
      }
      set
      {
        if ((value == null))
        {
          _diagramField = null;
        }
        else
        {
          if ((_diagramField == null))
          {
            _diagramField = XTypedList<Diagram>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("Diagram", "http://schemas.microsoft.com/ado/2008/10/edmx"));
          }
          else
          {
            XTypedServices.SetList(_diagramField, value);
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
        return XName.Get("TDiagrams", "http://schemas.microsoft.com/ado/2008/10/edmx");
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

    public static explicit operator Diagrams(XElement xe)
    {
      return XTypedServices.ToXTypedElement<Diagrams>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("Diagram", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (Diagram));
    }
  }
}