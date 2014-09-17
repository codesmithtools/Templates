using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Schema;
using Xml.Schema.Linq;

namespace LinqToEdmx.Model.Storage
{
  /// <summary>
  /// <para>
  /// Regular expression: (Documentation?, PropertyRef+, any)
  /// </para>
  /// </summary>
  public class ReferentialConstraintRoleElement : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static FSM _validationStates;

    private XTypedList<PropertyRef> _propertyRefField;

    static ReferentialConstraintRoleElement()
    {
      BuildElementDictionary();
      InitFsm();
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (Documentation?, PropertyRef+, any)
    /// </para>
    /// </summary>
    public Documentation Documentation
    {
      get
      {
        var x = GetElement(XName.Get("Documentation", XMLNamespaceFactory.SSDL));
        return ((Documentation) (x));
      }
      set
      {
        SetElement(XName.Get("Documentation", XMLNamespaceFactory.SSDL), value);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required, repeating
    /// </para>
    /// <para>
    /// Regular expression: (Documentation?, PropertyRef+, any)
    /// </para>
    /// </summary>
    public IList<PropertyRef> PropertyRefs
    {
      get
      {
        if ((_propertyRefField == null))
        {
          _propertyRefField = new XTypedList<PropertyRef>(this, LinqToXsdTypeManager.Instance, XName.Get("PropertyRef", XMLNamespaceFactory.SSDL));
        }
        return _propertyRefField;
      }
      set
      {
        if ((value == null))
        {
          _propertyRefField = null;
        }
        else
        {
          if ((_propertyRefField == null))
          {
            _propertyRefField = XTypedList<PropertyRef>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("PropertyRef", XMLNamespaceFactory.SSDL));
          }
          else
          {
            XTypedServices.SetList(_propertyRefField, value);
          }
        }
      }
    }

    /// <summary>
    /// <para>
    /// Regular expression: (Documentation?, PropertyRef+, any)
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
    public string Role
    {
      get
      {
        var x = Attribute(XName.Get("Role", ""));
        return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Role", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
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
        return XName.Get("TReferentialConstraintRoleElement", XMLNamespaceFactory.SSDL);
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

    public static explicit operator ReferentialConstraintRoleElement(XElement xe)
    {
      return XTypedServices.ToXTypedElement<ReferentialConstraintRoleElement>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("Documentation", XMLNamespaceFactory.SSDL), typeof (Documentation));
      LocalElementDictionary.Add(XName.Get("PropertyRef", XMLNamespaceFactory.SSDL), typeof (PropertyRef));
    }

    private static void InitFsm()
    {
      var transitions = new Dictionary<int, Transitions>();
      transitions.Add(1, new Transitions(new SingleTransition(XName.Get("Documentation", XMLNamespaceFactory.SSDL), 2), new SingleTransition(XName.Get("PropertyRef", XMLNamespaceFactory.SSDL), 4)));
      transitions.Add(2, new Transitions(new SingleTransition(XName.Get("PropertyRef", XMLNamespaceFactory.SSDL), 4)));
      transitions.Add(4, new Transitions(new SingleTransition(XName.Get("PropertyRef", XMLNamespaceFactory.SSDL), 4), new SingleTransition(new WildCard("##other", XMLNamespaceFactory.SSDL), 5)));
      transitions.Add(5, new Transitions(new SingleTransition(new WildCard("##other", XMLNamespaceFactory.SSDL), 5)));
      _validationStates = new FSM(1, new Set<int>(new[]
                                                   {
                                                     4, 5
                                                   }), transitions);
    }
  }
}