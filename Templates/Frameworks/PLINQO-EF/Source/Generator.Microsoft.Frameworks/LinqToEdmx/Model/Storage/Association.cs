using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Schema;
using Xml.Schema.Linq;

namespace LinqToEdmx.Model.Storage
{
  /// <summary>
  /// <para>
  /// Regular expression: (Documentation?, End+, ReferentialConstraint?, any)
  /// </para>
  /// </summary>
  public class Association : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static FSM _validationStates;

    private XTypedList<AssociationEnd> _endField;

    static Association()
    {
      BuildElementDictionary();
      InitFsm();
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (Documentation?, End+, ReferentialConstraint?, any)
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
    /// Occurrence: required, repeating
    /// </para>
    /// <para>
    /// Regular expression: (Documentation?, End+, ReferentialConstraint?, any)
    /// </para>
    /// </summary>
    public IList<AssociationEnd> Ends
    {
      get
      {
        if ((_endField == null))
        {
          _endField = new XTypedList<AssociationEnd>(this, LinqToXsdTypeManager.Instance, XName.Get("End", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"));
        }
        return _endField;
      }
      set
      {
        if ((value == null))
        {
          _endField = null;
        }
        else
        {
          if ((_endField == null))
          {
            _endField = XTypedList<AssociationEnd>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("End", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"));
          }
          else
          {
            XTypedServices.SetList(_endField, value);
          }
        }
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (Documentation?, End+, ReferentialConstraint?, any)
    /// </para>
    /// </summary>
    public Constraint ReferentialConstraint
    {
      get
      {
        var x = GetElement(XName.Get("ReferentialConstraint", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"));
        return ((Constraint) (x));
      }
      set
      {
        SetElement(XName.Get("ReferentialConstraint", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), value);
      }
    }

    /// <summary>
    /// <para>
    /// Regular expression: (Documentation?, End+, ReferentialConstraint?, any)
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
        return XName.Get("TAssociation", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl");
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

    public static explicit operator Association(XElement xe)
    {
      return XTypedServices.ToXTypedElement<Association>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (Documentation));
      LocalElementDictionary.Add(XName.Get("End", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (AssociationEnd));
      LocalElementDictionary.Add(XName.Get("ReferentialConstraint", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (Constraint));
    }

    private static void InitFsm()
    {
      var transitions = new Dictionary<int, Transitions>();
      transitions.Add(1, new Transitions(new SingleTransition(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 2), new SingleTransition(XName.Get("End", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 4)));
      transitions.Add(2, new Transitions(new SingleTransition(XName.Get("End", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 4)));
      transitions.Add(4, new Transitions(new SingleTransition(XName.Get("End", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 4), new SingleTransition(XName.Get("ReferentialConstraint", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 6), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 7)));
      transitions.Add(6, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 6)));
      transitions.Add(7, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 7)));
      _validationStates = new FSM(1, new Set<int>(new[]
                                                   {
                                                     4, 6, 7
                                                   }), transitions);
    }
  }
}