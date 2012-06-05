using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using LinqToEdmx.Model.Storage;
using Xml.Schema.Linq;

namespace LinqToEdmx.Model.Conceptual
{
  /// <summary>
  /// <para>
  /// Regular expression: (Documentation?, (FunctionImport | EntitySet | AssociationSet)*, any)
  /// </para>
  /// </summary>
  public class EntityContainer : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static FSM _validationStates;

    private XTypedList<AssociationSetLocalType> _associationSetField;

    private XTypedList<EntitySetLocalType> _entitySetField;

    private XTypedList<FunctionImportLocalType> _functionImportField;

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
    /// Regular expression: (Documentation?, (FunctionImport | EntitySet | AssociationSet)*, any)
    /// </para>
    /// </summary>
    public Documentation Documentation
    {
      get
      {
        var x = GetElement(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"));
        return ((Documentation) (x));
      }
      set
      {
        SetElement(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"), value);
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
    /// Regular expression: (Documentation?, (FunctionImport | EntitySet | AssociationSet)*, any)
    /// </para>
    /// </summary>
    public IList<FunctionImportLocalType> FunctionImports
    {
      get
      {
        if ((_functionImportField == null))
        {
          _functionImportField = new XTypedList<FunctionImportLocalType>(this, LinqToXsdTypeManager.Instance, XName.Get("FunctionImport", "http://schemas.microsoft.com/ado/2008/09/edm"));
        }
        return _functionImportField;
      }
      set
      {
        if ((value == null))
        {
          _functionImportField = null;
        }
        else
        {
          if ((_functionImportField == null))
          {
            _functionImportField = XTypedList<FunctionImportLocalType>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("FunctionImport", "http://schemas.microsoft.com/ado/2008/09/edm"));
          }
          else
          {
            XTypedServices.SetList(_functionImportField, value);
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
    /// Regular expression: (Documentation?, (FunctionImport | EntitySet | AssociationSet)*, any)
    /// </para>
    /// </summary>
    public IList<EntitySetLocalType> EntitySets
    {
      get
      {
        if ((_entitySetField == null))
        {
          _entitySetField = new XTypedList<EntitySetLocalType>(this, LinqToXsdTypeManager.Instance, XName.Get("EntitySet", "http://schemas.microsoft.com/ado/2008/09/edm"));
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
            _entitySetField = XTypedList<EntitySetLocalType>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("EntitySet", "http://schemas.microsoft.com/ado/2008/09/edm"));
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
    /// Regular expression: (Documentation?, (FunctionImport | EntitySet | AssociationSet)*, any)
    /// </para>
    /// </summary>
    public IList<AssociationSetLocalType> AssociationSets
    {
      get
      {
        if ((_associationSetField == null))
        {
          _associationSetField = new XTypedList<AssociationSetLocalType>(this, LinqToXsdTypeManager.Instance, XName.Get("AssociationSet", "http://schemas.microsoft.com/ado/2008/09/edm"));
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
            _associationSetField = XTypedList<AssociationSetLocalType>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("AssociationSet", "http://schemas.microsoft.com/ado/2008/09/edm"));
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
    /// Regular expression: (Documentation?, (FunctionImport | EntitySet | AssociationSet)*, any)
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
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public string Extends
    {
      get
      {
        var x = Attribute(XName.Get("Extends", ""));
        return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Extends", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public string TypeAccess
    {
      get
      {
        var x = Attribute(XName.Get("TypeAccess", "http://schemas.microsoft.com/ado/2006/04/codegeneration"));
        return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("TypeAccess", "http://schemas.microsoft.com/ado/2006/04/codegeneration"), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public bool? LazyLoadingEnabled
    {
      get
      {
        var x = Attribute(XName.Get("LazyLoadingEnabled", "http://schemas.microsoft.com/ado/2009/02/edm/annotation"));
        if ((x == null))
        {
          return null;
        }
        return XTypedServices.ParseValue<bool>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("LazyLoadingEnabled", "http://schemas.microsoft.com/ado/2009/02/edm/annotation"), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
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
        return XName.Get("EntityContainer", "http://schemas.microsoft.com/ado/2008/09/edm");
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
      LocalElementDictionary.Add(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (Documentation));
      LocalElementDictionary.Add(XName.Get("FunctionImport", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (FunctionImportLocalType));
      LocalElementDictionary.Add(XName.Get("EntitySet", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (EntitySetLocalType));
      LocalElementDictionary.Add(XName.Get("AssociationSet", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (AssociationSetLocalType));
    }

    private static void InitFsm()
    {
      var transitions = new Dictionary<int, Transitions>();
      transitions.Add(1, new Transitions(new SingleTransition(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"), 2), new SingleTransition(XName.Get("FunctionImport", "http://schemas.microsoft.com/ado/2008/09/edm"), 3), new SingleTransition(XName.Get("EntitySet", "http://schemas.microsoft.com/ado/2008/09/edm"), 3), new SingleTransition(XName.Get("AssociationSet", "http://schemas.microsoft.com/ado/2008/09/edm"), 3), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 9)));
      transitions.Add(2, new Transitions(new SingleTransition(XName.Get("FunctionImport", "http://schemas.microsoft.com/ado/2008/09/edm"), 2), new SingleTransition(XName.Get("EntitySet", "http://schemas.microsoft.com/ado/2008/09/edm"), 2), new SingleTransition(XName.Get("AssociationSet", "http://schemas.microsoft.com/ado/2008/09/edm"), 2), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 9)));
      transitions.Add(9, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 9)));
      transitions.Add(3, new Transitions(new SingleTransition(XName.Get("FunctionImport", "http://schemas.microsoft.com/ado/2008/09/edm"), 3), new SingleTransition(XName.Get("EntitySet", "http://schemas.microsoft.com/ado/2008/09/edm"), 3), new SingleTransition(XName.Get("AssociationSet", "http://schemas.microsoft.com/ado/2008/09/edm"), 3), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 9)));
      _validationStates = new FSM(1, new Set<int>(new[]
                                                   {
                                                     2, 1, 3, 9
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
      private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

      private static FSM _validationStates;

      private XTypedList<EndLocalType> _endField;

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
          var x = GetElement(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"));
          return ((Documentation) (x));
        }
        set
        {
          SetElement(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"), value);
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
          if ((_endField == null))
          {
            _endField = new XTypedList<EndLocalType>(this, LinqToXsdTypeManager.Instance, XName.Get("End", "http://schemas.microsoft.com/ado/2008/09/edm"));
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
              _endField = XTypedList<EndLocalType>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("End", "http://schemas.microsoft.com/ado/2008/09/edm"));
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
          return XName.Get("AssociationSet", "http://schemas.microsoft.com/ado/2008/09/edm");
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
        LocalElementDictionary.Add(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (Documentation));
        LocalElementDictionary.Add(XName.Get("End", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (EndLocalType));
      }

      private static void InitFsm()
      {
        var transitions = new Dictionary<int, Transitions>();
        transitions.Add(1, new Transitions(new SingleTransition(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"), 2), new SingleTransition(XName.Get("End", "http://schemas.microsoft.com/ado/2008/09/edm"), 3), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 5)));
        transitions.Add(2, new Transitions(new SingleTransition(XName.Get("End", "http://schemas.microsoft.com/ado/2008/09/edm"), 2), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 5)));
        transitions.Add(5, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 5)));
        transitions.Add(3, new Transitions(new SingleTransition(XName.Get("End", "http://schemas.microsoft.com/ado/2008/09/edm"), 3), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 5)));
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
            var x = GetElement(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"));
            return ((Documentation) (x));
          }
          set
          {
            SetElement(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"), value);
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
            return XName.Get("End", "http://schemas.microsoft.com/ado/2008/09/edm");
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
          LocalElementDictionary.Add(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (Documentation));
        }

        private static void InitFsm()
        {
          var transitions = new Dictionary<int, Transitions>();
          transitions.Add(1, new Transitions(new SingleTransition(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"), 2), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 3)));
          transitions.Add(2, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 2)));
          transitions.Add(3, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 3)));
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
    /// Regular expression: ((Documentation?, any)?)
    /// </para>
    /// </summary>
    public class EntitySetLocalType : XTypedElement, IXMetaData
    {
      private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

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
          var x = GetElement(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"));
          return ((Documentation) (x));
        }
        set
        {
          SetElement(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"), value);
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
      public string GetterAccess
      {
        get
        {
          var x = Attribute(XName.Get("GetterAccess", "http://schemas.microsoft.com/ado/2006/04/codegeneration"));
          return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
        }
        set
        {
          SetAttribute(XName.Get("GetterAccess", "http://schemas.microsoft.com/ado/2006/04/codegeneration"), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
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
          return XName.Get("EntitySet", "http://schemas.microsoft.com/ado/2008/09/edm");
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
        LocalElementDictionary.Add(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (Documentation));
      }

      private static void InitFsm()
      {
        var transitions = new Dictionary<int, Transitions>();
        transitions.Add(1, new Transitions(new SingleTransition(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"), 2), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 3)));
        transitions.Add(2, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 2)));
        transitions.Add(3, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 3)));
        _validationStates = new FSM(1, new Set<int>(new[]
                                                     {
                                                       2, 1, 3
                                                     }), transitions);
      }
    }

    #endregion

    #region Nested type: FunctionImportLocalType

    /// <summary>
    /// <para>
    /// Regular expression: (Documentation?, Parameter*, any)
    /// </para>
    /// </summary>
    public class FunctionImportLocalType : XTypedElement, IXMetaData
    {
      private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

      private static FSM _validationStates;

      private XTypedList<FunctionImportParameter> _parameterField;

      static FunctionImportLocalType()
      {
        BuildElementDictionary();
        InitFsm();
      }

      /// <summary>
      /// <para>
      /// Occurrence: optional
      /// </para>
      /// <para>
      /// Regular expression: (Documentation?, Parameter*, any)
      /// </para>
      /// </summary>
      public Documentation Documentation
      {
        get
        {
          var x = GetElement(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"));
          return ((Documentation) (x));
        }
        set
        {
          SetElement(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"), value);
        }
      }

      /// <summary>
      /// <para>
      /// Occurrence: optional, repeating
      /// </para>
      /// <para>
      /// Regular expression: (Documentation?, Parameter*, any)
      /// </para>
      /// </summary>
      public IList<FunctionImportParameter> Parameters
      {
        get
        {
          if ((_parameterField == null))
          {
            _parameterField = new XTypedList<FunctionImportParameter>(this, LinqToXsdTypeManager.Instance, XName.Get("Parameter", "http://schemas.microsoft.com/ado/2008/09/edm"));
          }
          return _parameterField;
        }
        set
        {
          if ((value == null))
          {
            _parameterField = null;
          }
          else
          {
            if ((_parameterField == null))
            {
              _parameterField = XTypedList<FunctionImportParameter>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("Parameter", "http://schemas.microsoft.com/ado/2008/09/edm"));
            }
            else
            {
              XTypedServices.SetList(_parameterField, value);
            }
          }
        }
      }

      /// <summary>
      /// <para>
      /// Regular expression: (Documentation?, Parameter*, any)
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
      /// Occurrence: optional
      /// </para>
      /// </summary>
      public object ReturnType
      {
        get
        {
          var x = Attribute(XName.Get("ReturnType", ""));
          return XTypedServices.ParseValue<object>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
        }
        set
        {
          SetAttribute(XName.Get("ReturnType", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
        }
      }

      /// <summary>
      /// <para>
      /// Occurrence: optional
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

      /// <summary>
      /// <para>
      /// Occurrence: optional
      /// </para>
      /// </summary>
      public string MethodAccess
      {
        get
        {
          var x = Attribute(XName.Get("MethodAccess", "http://schemas.microsoft.com/ado/2006/04/codegeneration"));
          return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
        }
        set
        {
          SetAttribute(XName.Get("MethodAccess", "http://schemas.microsoft.com/ado/2006/04/codegeneration"), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
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
          return XName.Get("FunctionImport", "http://schemas.microsoft.com/ado/2008/09/edm");
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

      public static explicit operator FunctionImportLocalType(XElement xe)
      {
        return XTypedServices.ToXTypedElement<FunctionImportLocalType>(xe, LinqToXsdTypeManager.Instance);
      }

      public override XTypedElement Clone()
      {
        return XTypedServices.CloneXTypedElement(this);
      }

      private static void BuildElementDictionary()
      {
        LocalElementDictionary.Add(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (Documentation));
        LocalElementDictionary.Add(XName.Get("Parameter", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (FunctionImportParameter));
      }

      private static void InitFsm()
      {
        var transitions = new Dictionary<int, Transitions>();
        transitions.Add(1, new Transitions(new SingleTransition(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"), 2), new SingleTransition(XName.Get("Parameter", "http://schemas.microsoft.com/ado/2008/09/edm"), 3), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 5)));
        transitions.Add(2, new Transitions(new SingleTransition(XName.Get("Parameter", "http://schemas.microsoft.com/ado/2008/09/edm"), 2), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 5)));
        transitions.Add(5, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 5)));
        transitions.Add(3, new Transitions(new SingleTransition(XName.Get("Parameter", "http://schemas.microsoft.com/ado/2008/09/edm"), 3), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 5)));
        _validationStates = new FSM(1, new Set<int>(new[]
                                                     {
                                                       2, 1, 3, 5
                                                     }), transitions);
      }
    }

    #endregion
  }
}