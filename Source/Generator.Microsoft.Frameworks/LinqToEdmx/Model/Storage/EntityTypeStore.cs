using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Schema;
using Xml.Schema.Linq;

namespace LinqToEdmx.Model.Storage
{
  /// <summary>
  /// <para>
  /// Regular expression: (Documentation?, Key?, (Property*)*, any)
  /// </para>
  /// </summary>
  public class EntityTypeStore : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static FSM _validationStates;

    private XTypedList<EntityProperty> _propertyField;

    static EntityTypeStore()
    {
      BuildElementDictionary();
      InitFsm();
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (Documentation?, Key?, (Property*)*, any)
    /// </para>
    /// </summary>
    public Documentation Documentation
    {
      get
      {
        var x = GetElement(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"));
        return ((Documentation) (x));
      }
      set
      {
        SetElement(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), value);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (Documentation?, Key?, (Property*)*, any)
    /// </para>
    /// </summary>
    public EntityKeyElement Key
    {
      get
      {
        var x = GetElement(XName.Get("Key", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"));
        return ((EntityKeyElement) (x));
      }
      set
      {
        SetElement(XName.Get("Key", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), value);
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
    /// Regular expression: (Documentation?, Key?, (Property*)*, any)
    /// </para>
    /// </summary>
    public IList<EntityProperty> Properties
    {
      get
      {
        if ((_propertyField == null))
        {
          _propertyField = new XTypedList<EntityProperty>(this, LinqToXsdTypeManager.Instance, XName.Get("Property", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"));
        }
        return _propertyField;
      }
      set
      {
        if ((value == null))
        {
          _propertyField = null;
        }
        else
        {
          if ((_propertyField == null))
          {
            _propertyField = XTypedList<EntityProperty>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("Property", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"));
          }
          else
          {
            XTypedServices.SetList(_propertyField, value);
          }
        }
      }
    }

    /// <summary>
    /// <para>
    /// Regular expression: (Documentation?, Key?, (Property*)*, any)
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
    public string Name
    {
      get
      {
        var x = Attribute(XName.Get("Name", ""));
        return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Name", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
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
        return XName.Get("TEntityType", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl");
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

    public static explicit operator EntityTypeStore(XElement xe)
    {
      return XTypedServices.ToXTypedElement<EntityTypeStore>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (Documentation));
      LocalElementDictionary.Add(XName.Get("Key", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (EntityKeyElement));
      LocalElementDictionary.Add(XName.Get("Property", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (EntityProperty));
    }

    private static void InitFsm()
    {
      var transitions = new Dictionary<int, Transitions>();
      transitions.Add(1, new Transitions(new SingleTransition(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 2), new SingleTransition(XName.Get("Key", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 4), new SingleTransition(XName.Get("Property", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 5), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 7)));
      transitions.Add(2, new Transitions(new SingleTransition(XName.Get("Key", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 4), new SingleTransition(XName.Get("Property", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 5), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 7)));
      transitions.Add(4, new Transitions(new SingleTransition(XName.Get("Property", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 4), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 7)));
      transitions.Add(7, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 7)));
      transitions.Add(5, new Transitions(new SingleTransition(XName.Get("Property", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 5), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 7)));
      _validationStates = new FSM(1, new Set<int>(new[]
                                                   {
                                                     2, 1, 4, 5, 7
                                                   }), transitions);
    }
  }
}