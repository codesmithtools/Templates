using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Xml.Schema.Linq;

namespace LinqToEdmx.Model.Storage
{
  /// <summary>
  /// <para>
  /// Regular expression: (Documentation?, (EntitySet | AssociationSet)*, any)
  /// </para>
  /// </summary>
  public class EntityContainer : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static FSM _validationStates;

    private XTypedList<AssociationSetLocalType> _associationSetField;

    private XTypedList<EntitySetLocalType> _entitySetField;

    static EntityContainer()
    {
      BuildElementDictionary();
      InitFsm();
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (Documentation?, (EntitySet | AssociationSet)*, any)
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
    /// Occurrence: required, choice
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: (Documentation?, (EntitySet | AssociationSet)*, any)
    /// </para>
    /// </summary>
    public IList<EntitySetLocalType> EntitySets
    {
      get
      {
        if ((_entitySetField == null))
        {
          _entitySetField = new XTypedList<EntitySetLocalType>(this, LinqToXsdTypeManager.Instance, XName.Get("EntitySet", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"));
        }
        return _entitySetField;
      }
      set
      {
        if ((value == null))
        {
          _entitySetField = null;
        }
        else
        {
          if ((_entitySetField == null))
          {
            _entitySetField = XTypedList<EntitySetLocalType>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("EntitySet", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"));
          }
          else
          {
            XTypedServices.SetList(_entitySetField, value);
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
    /// Regular expression: (Documentation?, (EntitySet | AssociationSet)*, any)
    /// </para>
    /// </summary>
    public IList<AssociationSetLocalType> AssociationSets
    {
      get
      {
        if ((_associationSetField == null))
        {
          _associationSetField = new XTypedList<AssociationSetLocalType>(this, LinqToXsdTypeManager.Instance, XName.Get("AssociationSet", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"));
        }
        return _associationSetField;
      }
      set
      {
        if ((value == null))
        {
          _associationSetField = null;
        }
        else
        {
          if ((_associationSetField == null))
          {
            _associationSetField = XTypedList<AssociationSetLocalType>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("AssociationSet", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"));
          }
          else
          {
            XTypedServices.SetList(_associationSetField, value);
          }
        }
      }
    }

    /// <summary>
    /// <para>
    /// Regular expression: (Documentation?, (EntitySet | AssociationSet)*, any)
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
        return XName.Get("EntityContainer", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl");
      }
    }

    SchemaOrigin IXMetaData.TypeOrigin
    {
      get
      {
        return SchemaOrigin.Element;
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

    public static explicit operator EntityContainer(XElement xe)
    {
      return XTypedServices.ToXTypedElement<EntityContainer>(xe, LinqToXsdTypeManager.Instance);
    }

    public void Save(string xmlFile)
    {
      XTypedServices.Save(xmlFile, Untyped);
    }

    public void Save(TextWriter tw)
    {
      XTypedServices.Save(tw, Untyped);
    }

    public void Save(XmlWriter xmlWriter)
    {
      XTypedServices.Save(xmlWriter, Untyped);
    }

    public static EntityContainer Load(string xmlFile)
    {
      return XTypedServices.Load<EntityContainer>(xmlFile);
    }

    public static EntityContainer Parse(string xml)
    {
      return XTypedServices.Parse<EntityContainer>(xml);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (Documentation));
      LocalElementDictionary.Add(XName.Get("EntitySet", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (EntitySetLocalType));
      LocalElementDictionary.Add(XName.Get("AssociationSet", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (AssociationSetLocalType));
    }

    private static void InitFsm()
    {
      var transitions = new Dictionary<int, Transitions>();
      transitions.Add(1, new Transitions(new SingleTransition(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 2), new SingleTransition(XName.Get("EntitySet", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 3), new SingleTransition(XName.Get("AssociationSet", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 3), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 7)));
      transitions.Add(2, new Transitions(new SingleTransition(XName.Get("EntitySet", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 2), new SingleTransition(XName.Get("AssociationSet", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 2), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 7)));
      transitions.Add(7, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 7)));
      transitions.Add(3, new Transitions(new SingleTransition(XName.Get("EntitySet", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 3), new SingleTransition(XName.Get("AssociationSet", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 3), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 7)));
      _validationStates = new FSM(1, new Set<int>(new[]
                                                   {
                                                     2, 1, 3, 7
                                                   }), transitions);
    }

    #region Nested type: AssociationSetLocalType

    /// <summary>
    /// <para>
    /// Regular expression: (Documentation?, End*, any)
    /// </para>
    /// </summary>
    public class AssociationSetLocalType : XTypedElement, IXMetaData
    {
      private static Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

      private static FSM _validationStates;

      private XTypedList<EndLocalType> EndField;

      static AssociationSetLocalType()
      {
        BuildElementDictionary();
        InitFsm();
      }

      /// <summary>
      /// <para>
      /// Occurrence: optional
      /// </para>
      /// <para>
      /// Regular expression: (Documentation?, End*, any)
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
      /// Occurrence: optional, repeating
      /// </para>
      /// <para>
      /// Regular expression: (Documentation?, End*, any)
      /// </para>
      /// </summary>
      public IList<EndLocalType> Ends
      {
        get
        {
          if ((EndField == null))
          {
            EndField = new XTypedList<EndLocalType>(this, LinqToXsdTypeManager.Instance, XName.Get("End", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"));
          }
          return EndField;
        }
        set
        {
          if ((value == null))
          {
            EndField = null;
          }
          else
          {
            if ((EndField == null))
            {
              EndField = XTypedList<EndLocalType>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("End", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"));
            }
            else
            {
              XTypedServices.SetList(EndField, value);
            }
          }
        }
      }

      /// <summary>
      /// <para>
      /// Regular expression: (Documentation?, End*, any)
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
      /// Occurrence: required
      /// </para>
      /// </summary>
      public string Association
      {
        get
        {
          var x = Attribute(XName.Get("Association", ""));
          return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
        }
        set
        {
          SetAttribute(XName.Get("Association", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
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
          return XName.Get("AssociationSet", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl");
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

      public static explicit operator AssociationSetLocalType(XElement xe)
      {
        return XTypedServices.ToXTypedElement<AssociationSetLocalType>(xe, LinqToXsdTypeManager.Instance);
      }

      public override XTypedElement Clone()
      {
        return XTypedServices.CloneXTypedElement(this);
      }

      private static void BuildElementDictionary()
      {
        LocalElementDictionary.Add(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (Documentation));
        LocalElementDictionary.Add(XName.Get("End", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (EndLocalType));
      }

      private static void InitFsm()
      {
        var transitions = new Dictionary<int, Transitions>();
        transitions.Add(1, new Transitions(new SingleTransition(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 2), new SingleTransition(XName.Get("End", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 3), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 5)));
        transitions.Add(2, new Transitions(new SingleTransition(XName.Get("End", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 2), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 5)));
        transitions.Add(5, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 5)));
        transitions.Add(3, new Transitions(new SingleTransition(XName.Get("End", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 3), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 5)));
        _validationStates = new FSM(1, new Set<int>(new[]
                                                     {
                                                       2, 1, 3, 5
                                                     }), transitions);
      }

      #region Nested type: EndLocalType

      /// <summary>
      /// <para>
      /// Regular expression: ((Documentation?, any)?)
      /// </para>
      /// </summary>
      public class EndLocalType : XTypedElement, IXMetaData
      {
        private static Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

        private static FSM _validationStates;

        static EndLocalType()
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
        /// Occurrence: optional
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

        /// <summary>
        /// <para>
        /// Occurrence: required
        /// </para>
        /// </summary>
        public string EntitySet
        {
          get
          {
            var x = Attribute(XName.Get("EntitySet", ""));
            return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
          }
          set
          {
            SetAttribute(XName.Get("EntitySet", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
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
            return XName.Get("End", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl");
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

        public static explicit operator EndLocalType(XElement xe)
        {
          return XTypedServices.ToXTypedElement<EndLocalType>(xe, LinqToXsdTypeManager.Instance);
        }

        public override XTypedElement Clone()
        {
          return XTypedServices.CloneXTypedElement(this);
        }

        private static void BuildElementDictionary()
        {
          LocalElementDictionary.Add(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (Documentation));
        }

        private static void InitFsm()
        {
          var transitions = new Dictionary<int, Transitions>();
          transitions.Add(1, new Transitions(new SingleTransition(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 2), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 3)));
          transitions.Add(2, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 2)));
          transitions.Add(3, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 3)));
          _validationStates = new FSM(1, new Set<int>(new[]
                                                       {
                                                         2, 1, 3
                                                       }), transitions);
        }
      }

      #endregion
    }

    #endregion

    #region Nested type: EntitySetLocalType

    /// <summary>
    /// <para>
    /// Regular expression: (Documentation?, DefiningQuery?, any)
    /// </para>
    /// </summary>
    public class EntitySetLocalType : XTypedElement, IXMetaData
    {
      private static Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

      private static FSM _validationStates;

      static EntitySetLocalType()
      {
        BuildElementDictionary();
        InitFsm();
      }

      /// <summary>
      /// <para>
      /// Occurrence: optional
      /// </para>
      /// <para>
      /// Regular expression: (Documentation?, DefiningQuery?, any)
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
      /// Occurrence: optional
      /// </para>
      /// <para>
      /// Regular expression: (Documentation?, DefiningQuery?, any)
      /// </para>
      /// </summary>
      public string DefiningQuery
      {
        get
        {
          var x = GetElement(XName.Get("DefiningQuery", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"));
          return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
        }
        set
        {
          SetElementWithValidation(XName.Get("DefiningQuery", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), value, "DefiningQuery", CommandText.TypeDefinition);
        }
      }

      /// <summary>
      /// <para>
      /// Regular expression: (Documentation?, DefiningQuery?, any)
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
      /// Occurrence: required
      /// </para>
      /// </summary>
      public string EntityType
      {
        get
        {
          var x = Attribute(XName.Get("EntityType", ""));
          return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
        }
        set
        {
          SetAttribute(XName.Get("EntityType", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
        }
      }

      /// <summary>
      /// <para>
      /// Occurrence: optional
      /// </para>
      /// </summary>
      public string Schema
      {
        get
        {
          var x = Attribute(XName.Get("Schema", ""));
          return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
        }
        set
        {
          SetAttribute(XName.Get("Schema", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
        }
      }

      /// <summary>
      /// <para>
      /// Occurrence: optional
      /// </para>
      /// </summary>
      public string Table
      {
        get
        {
          var x = Attribute(XName.Get("Table", ""));
          return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
        }
        set
        {
          SetAttribute(XName.Get("Table", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
        }
      }

      /// <summary>
      /// <para>
      /// Occurrence: optional
      /// </para>
      /// </summary>
      public string Type
      {
        get
        {
          var x = Attribute(XName.Get("Type", "http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator"));
          return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
        }
        set
        {
          SetAttribute(XName.Get("Type", "http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator"), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
        }
      }

      /// <summary>
      /// <para>
      /// Occurrence: optional
      /// </para>
      /// </summary>
      public string Schema1
      {
        get
        {
          var x = Attribute(XName.Get("Schema", "http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator"));
          return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
        }
        set
        {
          SetAttribute(XName.Get("Schema", "http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator"), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
        }
      }

      /// <summary>
      /// <para>
      /// Occurrence: optional
      /// </para>
      /// </summary>
      public string Name1
      {
        get
        {
          var x = Attribute(XName.Get("Name", "http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator"));
          return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
        }
        set
        {
          SetAttribute(XName.Get("Name", "http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator"), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
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
          return XName.Get("EntitySet", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl");
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

      public static explicit operator EntitySetLocalType(XElement xe)
      {
        return XTypedServices.ToXTypedElement<EntitySetLocalType>(xe, LinqToXsdTypeManager.Instance);
      }

      public override XTypedElement Clone()
      {
        return XTypedServices.CloneXTypedElement(this);
      }

      private static void BuildElementDictionary()
      {
        LocalElementDictionary.Add(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (Documentation));
        LocalElementDictionary.Add(XName.Get("DefiningQuery", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (string));
      }

      private static void InitFsm()
      {
        var transitions = new Dictionary<int, Transitions>();
        transitions.Add(1, new Transitions(new SingleTransition(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 2), new SingleTransition(XName.Get("DefiningQuery", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 4), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 5)));
        transitions.Add(2, new Transitions(new SingleTransition(XName.Get("DefiningQuery", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 4), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 5)));
        transitions.Add(4, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 4)));
        transitions.Add(5, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 5)));
        _validationStates = new FSM(1, new Set<int>(new[]
                                                     {
                                                       2, 1, 4, 5
                                                     }), transitions);
      }
    }

    #endregion
  }
}