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
  /// Regular expression: (ConnectorPoint*)
  /// </para>
  /// </summary>
  public class InheritanceConnector : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static readonly ContentModelEntity ContentModel;

    private XTypedList<ConnectorPoint> _connectorPointField;

    static InheritanceConnector()
    {
      BuildElementDictionary();
      ContentModel = new SequenceContentModelEntity(new NamedContentModelEntity(XName.Get("ConnectorPoint", "http://schemas.microsoft.com/ado/2008/10/edmx")));
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, repeating
    /// </para>
    /// <para>
    /// Regular expression: (ConnectorPoint*)
    /// </para>
    /// </summary>
    public IList<ConnectorPoint> ConnectorPoints
    {
      get
      {
        if ((_connectorPointField == null))
        {
          _connectorPointField = new XTypedList<ConnectorPoint>(this, LinqToXsdTypeManager.Instance, XName.Get("ConnectorPoint", "http://schemas.microsoft.com/ado/2008/10/edmx"));
        }
        return _connectorPointField;
      }
      set
      {
        if ((value == null))
        {
          _connectorPointField = null;
        }
        else
        {
          if ((_connectorPointField == null))
          {
            _connectorPointField = XTypedList<ConnectorPoint>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("ConnectorPoint", "http://schemas.microsoft.com/ado/2008/10/edmx"));
          }
          else
          {
            XTypedServices.SetList(_connectorPointField, value);
          }
        }
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required
    /// </para>
    /// </summary>
    public string EntityType
    {
      get
      {
        return XTypedServices.ParseValue<string>(Attribute(XName.Get("EntityType", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("EntityType", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public bool? ManuallyRouted
    {
      get
      {
        if ((Attribute(XName.Get("ManuallyRouted", "")) == null))
        {
          return null;
        }
        return XTypedServices.ParseValue<bool>(Attribute(XName.Get("ManuallyRouted", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("ManuallyRouted", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
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
        return XName.Get("TInheritanceConnector", "http://schemas.microsoft.com/ado/2008/10/edmx");
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

    public static explicit operator InheritanceConnector(XElement xe)
    {
      return XTypedServices.ToXTypedElement<InheritanceConnector>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("ConnectorPoint", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (ConnectorPoint));
    }
  }
}