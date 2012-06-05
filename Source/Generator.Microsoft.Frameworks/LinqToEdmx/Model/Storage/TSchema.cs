using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Schema;
using Xml.Schema.Linq;

namespace LinqToEdmx.Model.Storage
{
  /// <summary>
  /// <para>
  /// Regular expression: ((Association* | EntityType* | EntityContainer | Function*)*, any)
  /// </para>
  /// </summary>
  public class TSchema : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static FSM _validationStates;

    private XTypedList<Association> _associationField;

    private XTypedList<EntityContainer> _entityContainerField;

    private XTypedList<EntityTypeStore> _entityTypeField;

    private XTypedList<Function> _functionField;

    static TSchema()
    {
      BuildElementDictionary();
      InitFsm();
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, repeating, choice
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: ((Association* | EntityType* | EntityContainer | Function*)*, any)
    /// </para>
    /// </summary>
    public IList<Association> Associations
    {
      get
      {
        if ((_associationField == null))
        {
          _associationField = new XTypedList<Association>(this, LinqToXsdTypeManager.Instance, XName.Get("Association", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"));
        }
        return _associationField;
      }
      set
      {
        if ((value == null))
        {
          _associationField = null;
        }
        else
        {
          if ((_associationField == null))
          {
            _associationField = XTypedList<Association>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("Association", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"));
          }
          else
          {
            XTypedServices.SetList(_associationField, value);
          }
        }
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, repeating, choice
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: ((Association* | EntityType* | EntityContainer | Function*)*, any)
    /// </para>
    /// </summary>
    public IList<EntityTypeStore> EntityTypes
    {
      get
      {
        if ((_entityTypeField == null))
        {
          _entityTypeField = new XTypedList<EntityTypeStore>(this, LinqToXsdTypeManager.Instance, XName.Get("EntityType", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"));
        }
        return _entityTypeField;
      }
      set
      {
        if ((value == null))
        {
          _entityTypeField = null;
        }
        else
        {
          if ((_entityTypeField == null))
          {
            _entityTypeField = XTypedList<EntityTypeStore>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("EntityType", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"));
          }
          else
          {
            XTypedServices.SetList(_entityTypeField, value);
          }
        }
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required, choice
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: ((Association* | EntityType* | EntityContainer | Function*)*, any)
    /// </para>
    /// </summary>
    public IList<EntityContainer> EntityContainers
    {
      get
      {
        if ((_entityContainerField == null))
        {
          _entityContainerField = new XTypedList<EntityContainer>(this, LinqToXsdTypeManager.Instance, XName.Get("EntityContainer", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"));
        }
        return _entityContainerField;
      }
      set
      {
        if ((value == null))
        {
          _entityContainerField = null;
        }
        else
        {
          if ((_entityContainerField == null))
          {
            _entityContainerField = XTypedList<EntityContainer>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("EntityContainer", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"));
          }
          else
          {
            XTypedServices.SetList(_entityContainerField, value);
          }
        }
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, repeating, choice
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: ((Association* | EntityType* | EntityContainer | Function*)*, any)
    /// </para>
    /// </summary>
    public IList<Function> Functions
    {
      get
      {
        if ((_functionField == null))
        {
          _functionField = new XTypedList<Function>(this, LinqToXsdTypeManager.Instance, XName.Get("Function", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"));
        }
        return _functionField;
      }
      set
      {
        if ((value == null))
        {
          _functionField = null;
        }
        else
        {
          if ((_functionField == null))
          {
            _functionField = XTypedList<Function>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("Function", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"));
          }
          else
          {
            XTypedServices.SetList(_functionField, value);
          }
        }
      }
    }

    /// <summary>
    /// <para>
    /// Regular expression: ((Association* | EntityType* | EntityContainer | Function*)*, any)
    /// </para>
    /// </summary>
    public IEnumerable<XElement> Any
    {
      get
      {
        return GetWildCards(WildCard.DefaultWildCard);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required
    /// </para>
    /// </summary>
    public string @Namespace
    {
      get
      {
        var x = Attribute(XName.Get("Namespace", ""));
        return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Namespace", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public string Alias
    {
      get
      {
        var x = Attribute(XName.Get("Alias", ""));
        return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Alias", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required
    /// </para>
    /// </summary>
    public string Provider
    {
      get
      {
        var x = Attribute(XName.Get("Provider", ""));
        return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Provider", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required
    /// </para>
    /// </summary>
    public string ProviderManifestToken
    {
      get
      {
        var x = Attribute(XName.Get("ProviderManifestToken", ""));
        return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("ProviderManifestToken", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
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
        return XName.Get("TSchema", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl");
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

    FSM IXMetaData.GetValidationStates()
    {
      return _validationStates;
    }

    #endregion

    public static explicit operator TSchema(XElement xe)
    {
      return XTypedServices.ToXTypedElement<TSchema>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("Association", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (Association));
      LocalElementDictionary.Add(XName.Get("EntityType", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (EntityTypeStore));
      LocalElementDictionary.Add(XName.Get("EntityContainer", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (EntityContainer));
      LocalElementDictionary.Add(XName.Get("Function", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (Function));
    }

    private static void InitFsm()
    {
      var transitions = new Dictionary<int, Transitions>();
      transitions.Add(1, new Transitions(new SingleTransition(XName.Get("Association", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 1), new SingleTransition(XName.Get("EntityType", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 3), new SingleTransition(XName.Get("EntityContainer", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 1), new SingleTransition(XName.Get("Function", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 7), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 9)));
      transitions.Add(3, new Transitions(new SingleTransition(XName.Get("EntityType", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 1), new SingleTransition(XName.Get("Association", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 1), new SingleTransition(XName.Get("EntityContainer", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 1), new SingleTransition(XName.Get("Function", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 7), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 9)));
      transitions.Add(7, new Transitions(new SingleTransition(XName.Get("Function", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 7), new SingleTransition(XName.Get("Association", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 1), new SingleTransition(XName.Get("EntityType", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 3), new SingleTransition(XName.Get("EntityContainer", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 6), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 9)));
      transitions.Add(6, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 6)));
      transitions.Add(9, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 9)));
      _validationStates = new FSM(1, new Set<int>(new[]
                                                   {
                                                     1, 3, 6, 7, 9
                                                   }), transitions);
    }
  }
}