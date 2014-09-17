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
  /// Regular expression: ((Documentation?, any)?)
  /// </para>
  /// </summary>
  public class OnAction : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static FSM _validationStates;

    static OnAction()
    {
      BuildElementDictionary();
      InitFsm();
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: ((Documentation?, any)?)
    /// </para>
    /// </summary>
    public Documentation Documentation
    {
      get
      {
        var x = GetElement(XName.Get("Documentation", XMLNamespaceFactory.Edm));
        return ((Documentation) (x));
      }
      set
      {
        SetElement(XName.Get("Documentation", XMLNamespaceFactory.Edm), value);
      }
    }

    /// <summary>
    /// <para>
    /// Regular expression: ((Documentation?, any)?)
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
    public string Action
    {
      get
      {
        var x = Attribute(XName.Get("Action", ""));
        return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Token).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Action", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Token).Datatype);
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
        return XName.Get("TOnAction", XMLNamespaceFactory.Edm);
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

    public static explicit operator OnAction(XElement xe)
    {
      return XTypedServices.ToXTypedElement<OnAction>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("Documentation", XMLNamespaceFactory.Edm), typeof (Documentation));
    }

    private static void InitFsm()
    {
      var transitions = new Dictionary<int, Transitions>();
      transitions.Add(1, new Transitions(new SingleTransition(XName.Get("Documentation", XMLNamespaceFactory.Edm), 2), new SingleTransition(new WildCard("##other", XMLNamespaceFactory.Edm), 3)));
      transitions.Add(2, new Transitions(new SingleTransition(new WildCard("##other", XMLNamespaceFactory.Edm), 2)));
      transitions.Add(3, new Transitions(new SingleTransition(new WildCard("##other", XMLNamespaceFactory.Edm), 3)));
      _validationStates = new FSM(1, new Set<int>(new[]
                                                   {
                                                     2, 1, 3
                                                   }), transitions);
    }
  }
}