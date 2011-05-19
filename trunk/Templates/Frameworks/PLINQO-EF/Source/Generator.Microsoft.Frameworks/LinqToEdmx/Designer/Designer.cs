using System;
using System.Collections.Generic;
using System.Xml.Linq;
using LinqToEdmx.Model.Storage;
using Xml.Schema.Linq;

namespace LinqToEdmx.Designer
{
  /// <summary>
  /// <para>
  /// Regular expression: (Connection?, Options?, Diagrams?, any)
  /// </para>
  /// </summary>
  public class Designer : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static FSM _validationStates;

    static Designer()
    {
      BuildElementDictionary();
      InitFsm();
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (Connection?, Options?, Diagrams?, any)
    /// </para>
    /// </summary>
    public Connection Connection
    {
      get
      {
        return ((Connection) GetElement(XName.Get("Connection", "http://schemas.microsoft.com/ado/2008/10/edmx")));
      }
      set
      {
        SetElement(XName.Get("Connection", "http://schemas.microsoft.com/ado/2008/10/edmx"), value);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (Connection?, Options?, Diagrams?, any)
    /// </para>
    /// </summary>
    public Options Options
    {
      get
      {
        return ((Options) GetElement(XName.Get("Options", "http://schemas.microsoft.com/ado/2008/10/edmx")));
      }
      set
      {
        SetElement(XName.Get("Options", "http://schemas.microsoft.com/ado/2008/10/edmx"), value);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (Connection?, Options?, Diagrams?, any)
    /// </para>
    /// </summary>
    public Diagrams Diagrams
    {
      get
      {
        return ((Diagrams) GetElement(XName.Get("Diagrams", "http://schemas.microsoft.com/ado/2008/10/edmx")));
      }
      set
      {
        SetElement(XName.Get("Diagrams", "http://schemas.microsoft.com/ado/2008/10/edmx"), value);
      }
    }

    /// <summary>
    /// <para>
    /// Regular expression: (Connection?, Options?, Diagrams?, any)
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
        return XName.Get("TDesigner", "http://schemas.microsoft.com/ado/2008/10/edmx");
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

    public static explicit operator Designer(XElement xe)
    {
      return XTypedServices.ToXTypedElement<Designer>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("Connection", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (Connection));
      LocalElementDictionary.Add(XName.Get("Options", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (Options));
      LocalElementDictionary.Add(XName.Get("Diagrams", "http://schemas.microsoft.com/ado/2008/10/edmx"), typeof (Diagrams));
    }

    private static void InitFsm()
    {
      var transitions = new Dictionary<int, Transitions>();
      transitions.Add(1, new Transitions(new SingleTransition(XName.Get("Connection", "http://schemas.microsoft.com/ado/2008/10/edmx"), 2), new SingleTransition(XName.Get("Options", "http://schemas.microsoft.com/ado/2008/10/edmx"), 4), new SingleTransition(XName.Get("Diagrams", "http://schemas.microsoft.com/ado/2008/10/edmx"), 6), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/10/edmx"), 7)));
      transitions.Add(2, new Transitions(new SingleTransition(XName.Get("Options", "http://schemas.microsoft.com/ado/2008/10/edmx"), 4), new SingleTransition(XName.Get("Diagrams", "http://schemas.microsoft.com/ado/2008/10/edmx"), 6), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/10/edmx"), 7)));
      transitions.Add(4, new Transitions(new SingleTransition(XName.Get("Diagrams", "http://schemas.microsoft.com/ado/2008/10/edmx"), 6), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/10/edmx"), 7)));
      transitions.Add(6, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/10/edmx"), 6)));
      transitions.Add(7, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/10/edmx"), 7)));
      _validationStates = new FSM(1, new Set<int>(new[]
                                                   {
                                                     2, 1, 4, 6, 7
                                                   }), transitions);
    }
  }
}