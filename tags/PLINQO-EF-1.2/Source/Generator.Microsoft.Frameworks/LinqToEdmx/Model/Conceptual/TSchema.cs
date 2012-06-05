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
  /// Regular expression: ((@Using* | Association* | ComplexType* | EntityType* | Function* | EntityContainer)*, any)
  /// </para>
  /// </summary>
  public class TSchema : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static FSM _validationStates;

    private XTypedList<Association> _associationField;

    private XTypedList<ComplexType> _complexTypeField;

    private XTypedList<EntityContainer> _entityContainerField;

    private XTypedList<EntityType> _entityTypeField;

    private XTypedList<Function> _functionField;

    private XTypedList<Using> _usingField;

    static TSchema()
    {
      BuildElementDictionary();
      InitFsm();
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, repeating, choice
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: ((@Using* | Association* | ComplexType* | EntityType* | Function* | EntityContainer)*, any)
    /// </para>
    /// </summary>
    public IList<Using> @Usings
    {
      get
      {
        if ((_usingField == null))
        {
          _usingField = new XTypedList<Using>(this, LinqToXsdTypeManager.Instance, XName.Get("Using", "http://schemas.microsoft.com/ado/2008/09/edm"));
        }
        return _usingField;
      }
      set
      {
        if ((value == null))
        {
          _usingField = null;
        }
        else
        {
          if ((_usingField == null))
          {
            _usingField = XTypedList<Using>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("Using", "http://schemas.microsoft.com/ado/2008/09/edm"));
          }
          else
          {
            XTypedServices.SetList(_usingField, value);
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
    /// Regular expression: ((@Using* | Association* | ComplexType* | EntityType* | Function* | EntityContainer)*, any)
    /// </para>
    /// </summary>
    public IList<Association> Associations
    {
      get
      {
        if ((_associationField == null))
        {
          _associationField = new XTypedList<Association>(this, LinqToXsdTypeManager.Instance, XName.Get("Association", "http://schemas.microsoft.com/ado/2008/09/edm"));
        }
        return _associationField;
      }
      set
      {
        if ((value == null))
        {
          _associationField = null;
        }
        else
        {
          if ((_associationField == null))
          {
            _associationField = XTypedList<Association>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("Association", "http://schemas.microsoft.com/ado/2008/09/edm"));
          }
          else
          {
            XTypedServices.SetList(_associationField, value);
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
    /// Regular expression: ((@Using* | Association* | ComplexType* | EntityType* | Function* | EntityContainer)*, any)
    /// </para>
    /// </summary>
    public IList<ComplexType> ComplexTypes
    {
      get
      {
        if ((_complexTypeField == null))
        {
          _complexTypeField = new XTypedList<ComplexType>(this, LinqToXsdTypeManager.Instance, XName.Get("ComplexType", "http://schemas.microsoft.com/ado/2008/09/edm"));
        }
        return _complexTypeField;
      }
      set
      {
        if ((value == null))
        {
          _complexTypeField = null;
        }
        else
        {
          if ((_complexTypeField == null))
          {
            _complexTypeField = XTypedList<ComplexType>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("ComplexType", "http://schemas.microsoft.com/ado/2008/09/edm"));
          }
          else
          {
            XTypedServices.SetList(_complexTypeField, value);
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
    /// Regular expression: ((@Using* | Association* | ComplexType* | EntityType* | Function* | EntityContainer)*, any)
    /// </para>
    /// </summary>
    public IList<EntityType> EntityTypes
    {
      get
      {
        if ((_entityTypeField == null))
        {
          _entityTypeField = new XTypedList<EntityType>(this, LinqToXsdTypeManager.Instance, XName.Get("EntityType", "http://schemas.microsoft.com/ado/2008/09/edm"));
        }
        return _entityTypeField;
      }
      set
      {
        if ((value == null))
        {
          _entityTypeField = null;
        }
        else
        {
          if ((_entityTypeField == null))
          {
            _entityTypeField = XTypedList<EntityType>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("EntityType", "http://schemas.microsoft.com/ado/2008/09/edm"));
          }
          else
          {
            XTypedServices.SetList(_entityTypeField, value);
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
    /// Regular expression: ((@Using* | Association* | ComplexType* | EntityType* | Function* | EntityContainer)*, any)
    /// </para>
    /// </summary>
    public IList<Function> Functions
    {
      get
      {
        if ((_functionField == null))
        {
          _functionField = new XTypedList<Function>(this, LinqToXsdTypeManager.Instance, XName.Get("Function", "http://schemas.microsoft.com/ado/2008/09/edm"));
        }
        return _functionField;
      }
      set
      {
        if ((value == null))
        {
          _functionField = null;
        }
        else
        {
          if ((_functionField == null))
          {
            _functionField = XTypedList<Function>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("Function", "http://schemas.microsoft.com/ado/2008/09/edm"));
          }
          else
          {
            XTypedServices.SetList(_functionField, value);
          }
        }
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required, choice
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: ((@Using* | Association* | ComplexType* | EntityType* | Function* | EntityContainer)*, any)
    /// </para>
    /// </summary>
    public IList<EntityContainer> EntityContainers
    {
      get
      {
        if ((_entityContainerField == null))
        {
          _entityContainerField = new XTypedList<EntityContainer>(this, LinqToXsdTypeManager.Instance, XName.Get("EntityContainer", "http://schemas.microsoft.com/ado/2008/09/edm"));
        }
        return _entityContainerField;
      }
      set
      {
        if ((value == null))
        {
          _entityContainerField = null;
        }
        else
        {
          if ((_entityContainerField == null))
          {
            _entityContainerField = XTypedList<EntityContainer>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("EntityContainer", "http://schemas.microsoft.com/ado/2008/09/edm"));
          }
          else
          {
            XTypedServices.SetList(_entityContainerField, value);
          }
        }
      }
    }

    /// <summary>
    /// <para>
    /// Regular expression: ((@Using* | Association* | ComplexType* | EntityType* | Function* | EntityContainer)*, any)
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
    public string @Namespace
    {
      get
      {
        var x = Attribute(XName.Get("Namespace", ""));
        return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Namespace", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public string Alias
    {
      get
      {
        var x = Attribute(XName.Get("Alias", ""));
        return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Alias", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
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
        return XName.Get("TSchema", "http://schemas.microsoft.com/ado/2008/09/edm");
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

    public static explicit operator TSchema(XElement xe)
    {
      return XTypedServices.ToXTypedElement<TSchema>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("Using", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (Using));
      LocalElementDictionary.Add(XName.Get("Association", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (Association));
      LocalElementDictionary.Add(XName.Get("ComplexType", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (ComplexType));
      LocalElementDictionary.Add(XName.Get("EntityType", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (EntityType));
      LocalElementDictionary.Add(XName.Get("Function", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (Function));
      LocalElementDictionary.Add(XName.Get("EntityContainer", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (EntityContainer));
    }

    private static void InitFsm()
    {
      var transitions = new Dictionary<int, Transitions>();
      transitions.Add(1, new Transitions(new SingleTransition(XName.Get("Using", "http://schemas.microsoft.com/ado/2008/09/edm"), 1), new SingleTransition(XName.Get("Association", "http://schemas.microsoft.com/ado/2008/09/edm"), 3), new SingleTransition(XName.Get("ComplexType", "http://schemas.microsoft.com/ado/2008/09/edm"), 5), new SingleTransition(XName.Get("EntityType", "http://schemas.microsoft.com/ado/2008/09/edm"), 7), new SingleTransition(XName.Get("Function", "http://schemas.microsoft.com/ado/2008/09/edm"), 9), new SingleTransition(XName.Get("EntityContainer", "http://schemas.microsoft.com/ado/2008/09/edm"), 1), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 13)));
      transitions.Add(3, new Transitions(new SingleTransition(XName.Get("Association", "http://schemas.microsoft.com/ado/2008/09/edm"), 1), new SingleTransition(XName.Get("Using", "http://schemas.microsoft.com/ado/2008/09/edm"), 1), new SingleTransition(XName.Get("ComplexType", "http://schemas.microsoft.com/ado/2008/09/edm"), 5), new SingleTransition(XName.Get("EntityType", "http://schemas.microsoft.com/ado/2008/09/edm"), 7), new SingleTransition(XName.Get("Function", "http://schemas.microsoft.com/ado/2008/09/edm"), 9), new SingleTransition(XName.Get("EntityContainer", "http://schemas.microsoft.com/ado/2008/09/edm"), 1), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 13)));
      transitions.Add(5, new Transitions(new SingleTransition(XName.Get("ComplexType", "http://schemas.microsoft.com/ado/2008/09/edm"), 5), new SingleTransition(XName.Get("Using", "http://schemas.microsoft.com/ado/2008/09/edm"), 1), new SingleTransition(XName.Get("Association", "http://schemas.microsoft.com/ado/2008/09/edm"), 3), new SingleTransition(XName.Get("EntityType", "http://schemas.microsoft.com/ado/2008/09/edm"), 7), new SingleTransition(XName.Get("Function", "http://schemas.microsoft.com/ado/2008/09/edm"), 9), new SingleTransition(XName.Get("EntityContainer", "http://schemas.microsoft.com/ado/2008/09/edm"), 12), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 13)));
      transitions.Add(7, new Transitions(new SingleTransition(XName.Get("EntityType", "http://schemas.microsoft.com/ado/2008/09/edm"), 7), new SingleTransition(XName.Get("Using", "http://schemas.microsoft.com/ado/2008/09/edm"), 1), new SingleTransition(XName.Get("Association", "http://schemas.microsoft.com/ado/2008/09/edm"), 3), new SingleTransition(XName.Get("ComplexType", "http://schemas.microsoft.com/ado/2008/09/edm"), 5), new SingleTransition(XName.Get("Function", "http://schemas.microsoft.com/ado/2008/09/edm"), 9), new SingleTransition(XName.Get("EntityContainer", "http://schemas.microsoft.com/ado/2008/09/edm"), 12), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 13)));
      transitions.Add(9, new Transitions(new SingleTransition(XName.Get("Function", "http://schemas.microsoft.com/ado/2008/09/edm"), 9), new SingleTransition(XName.Get("Using", "http://schemas.microsoft.com/ado/2008/09/edm"), 1), new SingleTransition(XName.Get("Association", "http://schemas.microsoft.com/ado/2008/09/edm"), 3), new SingleTransition(XName.Get("ComplexType", "http://schemas.microsoft.com/ado/2008/09/edm"), 5), new SingleTransition(XName.Get("EntityType", "http://schemas.microsoft.com/ado/2008/09/edm"), 7), new SingleTransition(XName.Get("EntityContainer", "http://schemas.microsoft.com/ado/2008/09/edm"), 12), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 13)));
      transitions.Add(12, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 12)));
      transitions.Add(13, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 13)));
      _validationStates = new FSM(1, new Set<int>(new[] {1, 3, 5, 7, 9, 12, 13}), transitions);
    }
  }
}