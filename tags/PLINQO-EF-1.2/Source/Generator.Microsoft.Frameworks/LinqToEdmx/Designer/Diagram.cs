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
  /// Regular expression: ((EntityTypeShape? | AssociationConnector? | InheritanceConnector?)*)
  /// </para>
  /// </summary>
  public class Diagram : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private XTypedList<AssociationConnector> _associationConnectorField;

    private XTypedList<EntityTypeShape> _entityTypeShapeField;

    private XTypedList<InheritanceConnector> _inheritanceConnectorField;

    static Diagram()
    {
      BuildElementDictionary();
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, choice
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: ((EntityTypeShape? | AssociationConnector? | InheritanceConnector?)*)
    /// </para>
    /// </summary>
    public IList<EntityTypeShape> EntityTypeShapes
    {
      get
      {
        if ((_entityTypeShapeField == null))
        {
          _entityTypeShapeField = new XTypedList<EntityTypeShape>(this, LinqToXsdTypeManager.Instance, XName.Get("EntityTypeShape", "http://schemas.microsoft.com/ado/2008/10/edmx"));
        }
        return _entityTypeShapeField;
      }
      set
      {
        if ((value == null))
        {
          _entityTypeShapeField = null;
        }
        else
        {
          if ((_entityTypeShapeField == null))
          {
            _entityTypeShapeField = XTypedList<EntityTypeShape>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("EntityTypeShape", "http://schemas.microsoft.com/ado/2008/10/edmx"));
          }
          else
          {
            XTypedServices.SetList(_entityTypeShapeField, value);
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
    /// Regular expression: ((EntityTypeShape? | AssociationConnector? | InheritanceConnector?)*)
    /// </para>
    /// </summary>
    public IList<AssociationConnector> AssociationConnectors
    {
      get
      {
        if ((_associationConnectorField == null))
        {
          _associationConnectorField = new XTypedList<AssociationConnector>(this, LinqToXsdTypeManager.Instance, XName.Get("AssociationConnector", "http://schemas.microsoft.com/ado/2008/10/edmx"));
        }
        return _associationConnectorField;
      }
      set
      {
        if ((value == null))
        {
          _associationConnectorField = null;
        }
        else
        {
          if ((_associationConnectorField == null))
          {
            _associationConnectorField = XTypedList<AssociationConnector>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("AssociationConnector", "http://schemas.microsoft.com/ado/2008/10/edmx"));
          }
          else
          {
            XTypedServices.SetList(_associationConnectorField, value);
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
    /// Regular expression: ((EntityTypeShape? | AssociationConnector? | InheritanceConnector?)*)
    /// </para>
    /// </summary>
    public IList<InheritanceConnector> InheritanceConnectors
    {
      get
      {
        if ((_inheritanceConnectorField == null))
        {
          _inheritanceConnectorField = new XTypedList<InheritanceConnector>(this, LinqToXsdTypeManager.Instance, XName.Get("InheritanceConnector", "http://schemas.microsoft.com/ado/2008/10/edmx"));
        }
        return _inheritanceConnectorField;
      }
      set
      {
        if ((value == null))
        {
          _inheritanceConnectorField = null;
        }
        else
        {
          if ((_inheritanceConnectorField == null))
          {
            _inheritanceConnectorField = XTypedList<InheritanceConnector>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("InheritanceConnector", "http://schemas.microsoft.com/ado/2008/10/edmx"));
          }
          else
          {
            XTypedServices.SetList(_inheritanceConnectorField, value);
          }
        }
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required
    /// </para>
    /// </summary>
    public string Name
    {
      get
      {
        return XTypedServices.ParseValue<string>(Attribute(XName.Get("Name", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Name", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public int? ZoomLevel
    {
      get
      {
        if ((Attribute(XName.Get("ZoomLevel", "")) == null))
        {
          return null;
        }
        return XTypedServices.ParseValue<int>(Attribute(XName.Get("ZoomLevel", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Int).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("ZoomLevel", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Int).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public bool? ShowGrid
    {
      get
      {
        if ((Attribute(XName.Get("ShowGrid", "")) == null))
        {
          return null;
        }
        return XTypedServices.ParseValue<bool>(Attribute(XName.Get("ShowGrid", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("ShowGrid", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public bool? SnapToGrid
    {
      get
      {
        if ((Attribute(XName.Get("SnapToGrid", "")) == null))
        {
          return null;
        }
        return XTypedServices.ParseValue<bool>(Attribute(XName.Get("SnapToGrid", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("SnapToGrid", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public bool? DisplayType
    {
      get
      {
        if ((Attribute(XName.Get("DisplayType", "")) == null))
        {
          return null;
        }
        return XTypedServices.ParseValue<bool>(Attribute(XName.Get("DisplayType", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("DisplayType", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
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
        return XName.Get("TDiagram", "http://schemas.microsoft.com/ado/2008/10/edmx");
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

    public static explicit operator Diagram(XElement xe)
    {
      return XTypedServices.ToXTypedElement<Diagram>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("EntityTypeShape", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (EntityTypeShape));
      LocalElementDictionary.Add(XName.Get("AssociationConnector", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (AssociationConnector));
      LocalElementDictionary.Add(XName.Get("InheritanceConnector", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (InheritanceConnector));
    }
  }
}