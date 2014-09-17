using System;
using System.Collections.Generic;
using System.Xml.Linq;
using LinqToEdmx.Model.Storage;
using Xml.Schema.Linq;

namespace LinqToEdmx.Designer
{
  /// <summary>
  /// <para>
  /// Regular expression: (DesignerInfoPropertySet?)
  /// </para>
  /// </summary>
  public class Connection : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static readonly ContentModelEntity ContentModel;

    static Connection()
    {
      BuildElementDictionary();
      ContentModel = new SequenceContentModelEntity(new NamedContentModelEntity(XName.Get("DesignerInfoPropertySet", XMLNamespaceFactory.Edmx)));
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (DesignerInfoPropertySet?)
    /// </para>
    /// </summary>
    public DesignerInfoPropertySet DesignerInfoPropertySet
    {
      get
      {
        return ((DesignerInfoPropertySet) GetElement(XName.Get("DesignerInfoPropertySet", XMLNamespaceFactory.Edmx)));
      }
      set
      {
        SetElement(XName.Get("DesignerInfoPropertySet", XMLNamespaceFactory.Edmx), value);
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
        return XName.Get("TConnection", XMLNamespaceFactory.Edmx);
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

    public static explicit operator Connection(XElement xe)
    {
      return XTypedServices.ToXTypedElement<Connection>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("DesignerInfoPropertySet", XMLNamespaceFactory.Edmx), typeof (DesignerInfoPropertySet));
    }
  }
}