using System;
using System.Collections.Generic;
using System.Xml.Linq;
using LinqToEdmx.Model.Storage;
using Xml.Schema.Linq;

namespace LinqToEdmx.Designer
{
  /// <summary>
  /// <para>
  /// Regular expression: (DesignerProperty*)
  /// </para>
  /// </summary>
  public class DesignerInfoPropertySet : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static readonly ContentModelEntity ContentModel;

    private XTypedList<DesignerProperty> DesignerPropertyField;

    static DesignerInfoPropertySet()
    {
      BuildElementDictionary();
      ContentModel = new SequenceContentModelEntity(new NamedContentModelEntity(XName.Get("DesignerProperty", "http://schemas.microsoft.com/ado/2008/10/edmx")));
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, repeating
    /// </para>
    /// <para>
    /// Regular expression: (DesignerProperty*)
    /// </para>
    /// </summary>
    public IList<DesignerProperty> DesignerProperties
    {
      get
      {
        if ((DesignerPropertyField == null))
        {
          DesignerPropertyField = new XTypedList<DesignerProperty>(this, LinqToXsdTypeManager.Instance, XName.Get("DesignerProperty", "http://schemas.microsoft.com/ado/2008/10/edmx"));
        }
        return DesignerPropertyField;
      }
      set
      {
        if ((value == null))
        {
          DesignerPropertyField = null;
        }
        else
        {
          if ((DesignerPropertyField == null))
          {
            DesignerPropertyField = XTypedList<DesignerProperty>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("DesignerProperty", "http://schemas.microsoft.com/ado/2008/10/edmx"));
          }
          else
          {
            XTypedServices.SetList(DesignerPropertyField, value);
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
        return XName.Get("TDesignerInfoPropertySet", "http://schemas.microsoft.com/ado/2008/10/edmx");
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

    public static explicit operator DesignerInfoPropertySet(XElement xe)
    {
      return XTypedServices.ToXTypedElement<DesignerInfoPropertySet>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("DesignerProperty", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (DesignerProperty));
    }
  }
}