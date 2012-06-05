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
  /// Regular expression: ((CollectionType? | ReferenceType? | RowType?)?, any)
  /// </para>
  /// </summary>
  public class FunctionReturnType : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static FSM _validationStates;

    static FunctionReturnType()
    {
      BuildElementDictionary();
      InitFsm();
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, choice
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: ((CollectionType? | ReferenceType? | RowType?)?, any)
    /// </para>
    /// </summary>
    public TCollectionType CollectionType
    {
      get
      {
        var x = GetElement(XName.Get("CollectionType", "http://schemas.microsoft.com/ado/2008/09/edm"));
        return ((TCollectionType) (x));
      }
      set
      {
        SetElement(XName.Get("CollectionType", "http://schemas.microsoft.com/ado/2008/09/edm"), value);
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
    /// Regular expression: ((CollectionType? | ReferenceType? | RowType?)?, any)
    /// </para>
    /// </summary>
    public ReferenceType ReferenceType
    {
      get
      {
        var x = GetElement(XName.Get("ReferenceType", "http://schemas.microsoft.com/ado/2008/09/edm"));
        return ((ReferenceType) (x));
      }
      set
      {
        SetElement(XName.Get("ReferenceType", "http://schemas.microsoft.com/ado/2008/09/edm"), value);
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
    /// Regular expression: ((CollectionType? | ReferenceType? | RowType?)?, any)
    /// </para>
    /// </summary>
    public RowType RowType
    {
      get
      {
        var x = GetElement(XName.Get("RowType", "http://schemas.microsoft.com/ado/2008/09/edm"));
        return ((RowType) (x));
      }
      set
      {
        SetElement(XName.Get("RowType", "http://schemas.microsoft.com/ado/2008/09/edm"), value);
      }
    }

    /// <summary>
    /// <para>
    /// Regular expression: ((CollectionType? | ReferenceType? | RowType?)?, any)
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
    public object Type
    {
      get
      {
        var x = Attribute(XName.Get("Type", ""));
        return XTypedServices.ParseValue<object>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
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
    public bool? Nullable
    {
      get
      {
        var x = Attribute(XName.Get("Nullable", ""));
        if ((x == null))
        {
          return null;
        }
        return XTypedServices.ParseValue<bool>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Nullable", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public string DefaultValue
    {
      get
      {
        var x = Attribute(XName.Get("DefaultValue", ""));
        return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("DefaultValue", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public object MaxLength
    {
      get
      {
        var x = Attribute(XName.Get("MaxLength", ""));
        return XTypedServices.ParseValue<object>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("MaxLength", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public bool? FixedLength
    {
      get
      {
        var x = Attribute(XName.Get("FixedLength", ""));
        if ((x == null))
        {
          return null;
        }
        return XTypedServices.ParseValue<bool>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("FixedLength", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public decimal? Precision
    {
      get
      {
        var x = Attribute(XName.Get("Precision", ""));
        if ((x == null))
        {
          return null;
        }
        return XTypedServices.ParseValue<decimal>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.NonNegativeInteger).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Precision", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.NonNegativeInteger).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public decimal? Scale
    {
      get
      {
        var x = Attribute(XName.Get("Scale", ""));
        if ((x == null))
        {
          return null;
        }
        return XTypedServices.ParseValue<decimal>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.NonNegativeInteger).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Scale", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.NonNegativeInteger).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public bool? Unicode
    {
      get
      {
        var x = Attribute(XName.Get("Unicode", ""));
        if ((x == null))
        {
          return null;
        }
        return XTypedServices.ParseValue<bool>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Unicode", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public string Collation
    {
      get
      {
        var x = Attribute(XName.Get("Collation", ""));
        return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Collation", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
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
        return XName.Get("TFunctionReturnType", "http://schemas.microsoft.com/ado/2008/09/edm");
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

    public static explicit operator FunctionReturnType(XElement xe)
    {
      return XTypedServices.ToXTypedElement<FunctionReturnType>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("CollectionType", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (TCollectionType));
      LocalElementDictionary.Add(XName.Get("ReferenceType", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (ReferenceType));
      LocalElementDictionary.Add(XName.Get("RowType", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (RowType));
    }

    private static void InitFsm()
    {
      var transitions = new Dictionary<int, Transitions>();
      transitions.Add(1, new Transitions(new SingleTransition(XName.Get("CollectionType", "http://schemas.microsoft.com/ado/2008/09/edm"), 2), new SingleTransition(XName.Get("ReferenceType", "http://schemas.microsoft.com/ado/2008/09/edm"), 4), new SingleTransition(XName.Get("RowType", "http://schemas.microsoft.com/ado/2008/09/edm"), 6), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 7)));
      transitions.Add(2, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 2)));
      transitions.Add(4, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 4)));
      transitions.Add(6, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 6)));
      transitions.Add(7, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 7)));
      _validationStates = new FSM(1, new Set<int>(new[]
                                                   {
                                                     2, 1, 4, 6, 7
                                                   }), transitions);
    }
  }
}