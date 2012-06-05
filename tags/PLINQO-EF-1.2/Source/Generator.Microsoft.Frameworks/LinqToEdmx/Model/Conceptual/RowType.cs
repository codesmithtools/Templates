using System;
using System.Collections.Generic;
using System.Xml.Linq;
using LinqToEdmx.Model.Storage;
using Xml.Schema.Linq;

namespace LinqToEdmx.Model.Conceptual
{
  /// <summary>
  /// <para>
  /// Regular expression: (Property? | any)+
  /// </para>
  /// </summary>
  public class RowType : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static FSM _validationStates;

    private XTypedList<Property> _propertyField;

    static RowType()
    {
      BuildElementDictionary();
      InitFsm();
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, choice
    /// </para>
    /// <para>
    /// Regular expression: (Property? | any)+
    /// </para>
    /// </summary>
    public IList<Property> Properties
    {
      get
      {
        if ((_propertyField == null))
        {
          _propertyField = new XTypedList<Property>(this, LinqToXsdTypeManager.Instance, XName.Get("Property", "http://schemas.microsoft.com/ado/2008/09/edm"));
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
            _propertyField = XTypedList<Property>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("Property", "http://schemas.microsoft.com/ado/2008/09/edm"));
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
    /// Regular expression: (Property? | any)+
    /// </para>
    /// </summary>
    public IEnumerable<XElement> Any
    {
      get
      {
        return GetWildCards(WildCard.DefaultWildCard);
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
        return XName.Get("TRowType", "http://schemas.microsoft.com/ado/2008/09/edm");
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

    public static explicit operator RowType(XElement xe)
    {
      return XTypedServices.ToXTypedElement<RowType>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("Property", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (Property));
    }

    private static void InitFsm()
    {
      var transitions = new Dictionary<int, Transitions>();
      transitions.Add(1, new Transitions(new SingleTransition(XName.Get("Property", "http://schemas.microsoft.com/ado/2008/09/edm"), 2), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 3)));
      transitions.Add(2, new Transitions(new SingleTransition(XName.Get("Property", "http://schemas.microsoft.com/ado/2008/09/edm"), 2), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 3)));
      transitions.Add(3, new Transitions(new SingleTransition(XName.Get("Property", "http://schemas.microsoft.com/ado/2008/09/edm"), 2), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 3)));
      _validationStates = new FSM(1, new Set<int>(new[]
                                                   {
                                                     2, 1, 3
                                                   }), transitions);
    }
  }
}