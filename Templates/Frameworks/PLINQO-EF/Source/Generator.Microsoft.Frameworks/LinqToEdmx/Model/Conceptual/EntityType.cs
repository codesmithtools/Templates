using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using LinqToEdmx.Model.Storage;
using Xml.Schema.Linq;

namespace LinqToEdmx.Model.Conceptual
{
  /// <summary>
  /// <para>
  /// Regular expression: (Documentation?, Key?, (Property* | NavigationProperty*)*, any)
  /// </para>
  /// </summary>
  public class EntityType : XTypedElement, IXMetaData
  {
    private static readonly bool IsAbstractDefaultValue = XmlConvert.ToBoolean("false");

    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static FSM _validationStates;

    private XTypedList<NavigationProperty> _navigationPropertyField;

    private XTypedList<EntityProperty> _propertyField;

    static EntityType()
    {
      BuildElementDictionary();
      InitFsm();
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (Documentation?, Key?, (Property* | NavigationProperty*)*, any)
    /// </para>
    /// </summary>
    public Documentation Documentation
    {
      get
      {
        var x = GetElement(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"));
        return ((Documentation) (x));
      }
      set
      {
        SetElement(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"), value);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (Documentation?, Key?, (Property* | NavigationProperty*)*, any)
    /// </para>
    /// </summary>
    public EntityKeyElement Key
    {
      get
      {
        var x = GetElement(XName.Get("Key", "http://schemas.microsoft.com/ado/2008/09/edm"));
        return ((EntityKeyElement) (x));
      }
      set
      {
        SetElement(XName.Get("Key", "http://schemas.microsoft.com/ado/2008/09/edm"), value);
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
    /// Regular expression: (Documentation?, Key?, (Property* | NavigationProperty*)*, any)
    /// </para>
    /// </summary>
    public IList<EntityProperty> Properties
    {
      get
      {
        if ((_propertyField == null))
        {
          _propertyField = new XTypedList<EntityProperty>(this, LinqToXsdTypeManager.Instance, XName.Get("Property", "http://schemas.microsoft.com/ado/2008/09/edm"));
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
            _propertyField = XTypedList<EntityProperty>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("Property", "http://schemas.microsoft.com/ado/2008/09/edm"));
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
    /// Occurrence: optional, repeating, choice
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: (Documentation?, Key?, (Property* | NavigationProperty*)*, any)
    /// </para>
    /// </summary>
    public IList<NavigationProperty> NavigationProperties
    {
      get
      {
        if ((_navigationPropertyField == null))
        {
          _navigationPropertyField = new XTypedList<NavigationProperty>(this, LinqToXsdTypeManager.Instance, XName.Get("NavigationProperty", "http://schemas.microsoft.com/ado/2008/09/edm"));
        }
        return _navigationPropertyField;
      }
      set
      {
        if ((value == null))
        {
          _navigationPropertyField = null;
        }
        else
        {
          if ((_navigationPropertyField == null))
          {
            _navigationPropertyField = XTypedList<NavigationProperty>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("NavigationProperty", "http://schemas.microsoft.com/ado/2008/09/edm"));
          }
          else
          {
            XTypedServices.SetList(_navigationPropertyField, value);
          }
        }
      }
    }

    /// <summary>
    /// <para>
    /// Regular expression: (Documentation?, Key?, (Property* | NavigationProperty*)*, any)
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

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public string BaseType
    {
      get
      {
        var x = Attribute(XName.Get("BaseType", ""));
        return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("BaseType", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public bool IsAbstract
    {
      get
      {
        var x = Attribute(XName.Get("Abstract", ""));
        return XTypedServices.ParseValue(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype, IsAbstractDefaultValue);
      }
      set
      {
        SetAttribute(XName.Get("Abstract", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public string TypeAccess
    {
      get
      {
        var x = Attribute(XName.Get("TypeAccess", "http://schemas.microsoft.com/ado/2006/04/codegeneration"));
        return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("TypeAccess", "http://schemas.microsoft.com/ado/2006/04/codegeneration"), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
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
        return XName.Get("TEntityType", "http://schemas.microsoft.com/ado/2008/09/edm");
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

    public static explicit operator EntityType(XElement xe)
    {
      return XTypedServices.ToXTypedElement<EntityType>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (Documentation));
      LocalElementDictionary.Add(XName.Get("Key", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (EntityKeyElement));
      LocalElementDictionary.Add(XName.Get("Property", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (EntityProperty));
      LocalElementDictionary.Add(XName.Get("NavigationProperty", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (NavigationProperty));
    }

    private static void InitFsm()
    {
      var transitions = new Dictionary<int, Transitions>();
      transitions.Add(1, new Transitions(new SingleTransition(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"), 2), new SingleTransition(XName.Get("Key", "http://schemas.microsoft.com/ado/2008/09/edm"), 4), new SingleTransition(XName.Get("Property", "http://schemas.microsoft.com/ado/2008/09/edm"), 5), new SingleTransition(XName.Get("NavigationProperty", "http://schemas.microsoft.com/ado/2008/09/edm"), 7), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 9)));
      transitions.Add(2, new Transitions(new SingleTransition(XName.Get("Key", "http://schemas.microsoft.com/ado/2008/09/edm"), 4), new SingleTransition(XName.Get("Property", "http://schemas.microsoft.com/ado/2008/09/edm"), 5), new SingleTransition(XName.Get("NavigationProperty", "http://schemas.microsoft.com/ado/2008/09/edm"), 7), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 9)));
      transitions.Add(4, new Transitions(new SingleTransition(XName.Get("Property", "http://schemas.microsoft.com/ado/2008/09/edm"), 4), new SingleTransition(XName.Get("NavigationProperty", "http://schemas.microsoft.com/ado/2008/09/edm"), 7), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 9)));
      transitions.Add(7, new Transitions(new SingleTransition(XName.Get("NavigationProperty", "http://schemas.microsoft.com/ado/2008/09/edm"), 5), new SingleTransition(XName.Get("Property", "http://schemas.microsoft.com/ado/2008/09/edm"), 5), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 9)));
      transitions.Add(5, new Transitions(new SingleTransition(XName.Get("Property", "http://schemas.microsoft.com/ado/2008/09/edm"), 5), new SingleTransition(XName.Get("NavigationProperty", "http://schemas.microsoft.com/ado/2008/09/edm"), 7), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 9)));
      transitions.Add(9, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 9)));
      _validationStates = new FSM(1, new Set<int>(new[]
                                                   {
                                                     2, 1, 4, 5, 7, 9
                                                   }), transitions);
    }
  }
}