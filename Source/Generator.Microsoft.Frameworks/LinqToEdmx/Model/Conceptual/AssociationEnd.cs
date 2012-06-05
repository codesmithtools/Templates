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
  /// Regular expression: (Documentation?, (OnDelete?)*, any)
  /// </para>
  /// </summary>
  public class AssociationEnd : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static FSM _validationStates;

    private XTypedList<OnAction> _onDeleteField;

    static AssociationEnd()
    {
      BuildElementDictionary();
      InitFsm();
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (Documentation?, (OnDelete?)*, any)
    /// </para>
    /// </summary>
    public Documentation Documentation
    {
      get
      {
        return ((Documentation) GetElement(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm")));
      }
      set
      {
        SetElement(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"), value);
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
    /// Regular expression: (Documentation?, (OnDelete?)*, any)
    /// </para>
    /// </summary>
    public IList<OnAction> OnDelete
    {
      get
      {
        if ((_onDeleteField == null))
        {
          _onDeleteField = new XTypedList<OnAction>(this, LinqToXsdTypeManager.Instance, XName.Get("OnDelete", "http://schemas.microsoft.com/ado/2008/09/edm"));
        }
        return _onDeleteField;
      }
      set
      {
        if ((value == null))
        {
          _onDeleteField = null;
        }
        else
        {
          if ((_onDeleteField == null))
          {
            _onDeleteField = XTypedList<OnAction>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("OnDelete", "http://schemas.microsoft.com/ado/2008/09/edm"));
          }
          else
          {
            XTypedServices.SetList(_onDeleteField, value);
          }
        }
      }
    }

    /// <summary>
    /// <para>
    /// Regular expression: (Documentation?, (OnDelete?)*, any)
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
    public string Type
    {
      get
      {
        return XTypedServices.ParseValue<string>(Attribute(XName.Get("Type", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Type", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public string Role
    {
      get
      {
        return XTypedServices.ParseValue<string>(Attribute(XName.Get("Role", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Role", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public string Multiplicity
    {
      get
      {
        return XTypedServices.ParseValue<string>(Attribute(XName.Get("Multiplicity", "")), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Token).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Multiplicity", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Token).Datatype);
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
        return XName.Get("TAssociationEnd", "http://schemas.microsoft.com/ado/2008/09/edm");
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

    public static explicit operator AssociationEnd(XElement xe)
    {
      return XTypedServices.ToXTypedElement<AssociationEnd>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (Documentation));
      LocalElementDictionary.Add(XName.Get("OnDelete", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (OnAction));
    }

    private static void InitFsm()
    {
      var transitions = new Dictionary<int, Transitions>();
      transitions.Add(1, new Transitions(new SingleTransition(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"), 2), new SingleTransition(XName.Get("OnDelete", "http://schemas.microsoft.com/ado/2008/09/edm"), 3), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 5)));
      transitions.Add(2, new Transitions(new SingleTransition(XName.Get("OnDelete", "http://schemas.microsoft.com/ado/2008/09/edm"), 2), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 5)));
      transitions.Add(5, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 5)));
      transitions.Add(3, new Transitions(new SingleTransition(XName.Get("OnDelete", "http://schemas.microsoft.com/ado/2008/09/edm"), 3), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 5)));
      _validationStates = new FSM(1, new Set<int>(new[]
                                                   {
                                                     2, 1, 3, 5
                                                   }), transitions);
    }
  }
}