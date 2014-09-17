using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Xml.Schema.Linq;

namespace LinqToEdmx.Model.Storage
{
  /// <summary>
  /// <para>
  /// Regular expression: (Documentation?, Principal, Dependent, any)
  /// </para>
  /// </summary>
  public class Constraint : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static FSM _validationStates;

    static Constraint()
    {
      BuildElementDictionary();
      InitFsm();
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (Documentation?, Principal, Dependent, any)
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
    /// Occurrence: required
    /// </para>
    /// <para>
    /// Regular expression: (Documentation?, Principal, Dependent, any)
    /// </para>
    /// </summary>
    public ReferentialConstraintRoleElement Principal
    {
      get
      {
        var x = GetElement(XName.Get("Principal", XMLNamespaceFactory.SSDL));
        return ((ReferentialConstraintRoleElement) (x));
      }
      set
      {
        SetElement(XName.Get("Principal", XMLNamespaceFactory.SSDL), value);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required
    /// </para>
    /// <para>
    /// Regular expression: (Documentation?, Principal, Dependent, any)
    /// </para>
    /// </summary>
    public ReferentialConstraintRoleElement Dependent
    {
      get
      {
        var x = GetElement(XName.Get("Dependent", XMLNamespaceFactory.SSDL));
        return ((ReferentialConstraintRoleElement) (x));
      }
      set
      {
        SetElement(XName.Get("Dependent", XMLNamespaceFactory.SSDL), value);
      }
    }

    /// <summary>
    /// <para>
    /// Regular expression: (Documentation?, Principal, Dependent, any)
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
        return XName.Get("TConstraint", XMLNamespaceFactory.SSDL);
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

    public static explicit operator Constraint(XElement xe)
    {
      return XTypedServices.ToXTypedElement<Constraint>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("Documentation", XMLNamespaceFactory.SSDL), typeof (Documentation));
      LocalElementDictionary.Add(XName.Get("Principal", XMLNamespaceFactory.SSDL), typeof (ReferentialConstraintRoleElement));
      LocalElementDictionary.Add(XName.Get("Dependent", XMLNamespaceFactory.SSDL), typeof (ReferentialConstraintRoleElement));
    }

    private static void InitFsm()
    {
      var transitions = new Dictionary<int, Transitions>();
      transitions.Add(1, new Transitions(new SingleTransition(XName.Get("Documentation", XMLNamespaceFactory.SSDL), 2), new SingleTransition(XName.Get("Principal", XMLNamespaceFactory.SSDL), 4)));
      transitions.Add(2, new Transitions(new SingleTransition(XName.Get("Principal", XMLNamespaceFactory.SSDL), 4)));
      transitions.Add(4, new Transitions(new SingleTransition(XName.Get("Dependent", XMLNamespaceFactory.SSDL), 6)));
      transitions.Add(6, new Transitions(new SingleTransition(new WildCard("##other", XMLNamespaceFactory.SSDL), 6)));
      _validationStates = new FSM(1, new Set<int>(6), transitions);
    }
  }
}