using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Schema;
using LinqToEdmx.Model.Storage;
using Xml.Schema.Linq;

namespace LinqToEdmx.Model.Conceptual
{
  /// <summary>
  /// <para>
  /// Regular expression: (PropertyRef+, any)
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
    /// Occurrence: required, repeating
    /// </para>
    /// <para>
    /// Regular expression: (PropertyRef+, any)
    /// </para>
    /// </summary>
    public IList<PropertyRef> PropertyRefs
    {
      get
      {
        if ((_propertyRefField == null))
        {
          _propertyRefField = new XTypedList<PropertyRef>(this, LinqToXsdTypeManager.Instance, XName.Get("PropertyRef", "http://schemas.microsoft.com/ado/2008/09/edm"));
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
            _propertyRefField = XTypedList<PropertyRef>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("PropertyRef", "http://schemas.microsoft.com/ado/2008/09/edm"));
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
    /// Regular expression: (PropertyRef+, any)
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
        return XName.Get("TReferentialConstraintRoleElement", "http://schemas.microsoft.com/ado/2008/09/edm");
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
      LocalElementDictionary.Add(XName.Get("PropertyRef", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (PropertyRef));
    }

    private static void InitFsm()
    {
      var transitions = new Dictionary<int, Transitions>();
      transitions.Add(1, new Transitions(new SingleTransition(XName.Get("PropertyRef", "http://schemas.microsoft.com/ado/2008/09/edm"), 2)));
      transitions.Add(2, new Transitions(new SingleTransition(XName.Get("PropertyRef", "http://schemas.microsoft.com/ado/2008/09/edm"), 2), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 3)));
      transitions.Add(3, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 3)));
      _validationStates = new FSM(1, new Set<int>(new[]
                                                   {
                                                     2, 3
                                                   }), transitions);
    }
  }
}